using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp_microservices_example.LoginService;

var builder = WebApplication.CreateBuilder(args);

// ADD CONFIGURATION
// health
builder.Services.AddHealthChecks();
builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddPolicy("byIP", httpContext => 
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Request.Host.ToString(),
            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromSeconds(30)
                }
            )
        );
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
            };
        });

// authorization 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
    options.AddPolicy("authenticatedAndAdmin", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("isAdmin", "true");
    });
});

// reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


// USE CONFIGURATION
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// health
app.MapHealthChecks("/healthz");

// cors
app.UseCors();

// authorization
app.UseAuthentication();
app.UseAuthorization();

// use rate limiter AFTER setting auth, so it can access to the user property and check it is authorized
app.UseRateLimiter();


app.MapGet("/generateToken", (IConfiguration configuration) =>
{
    var jwtSettingsSections = configuration.GetSection("JwtSettings");
    
    var jwtSettings = new JwtConfiguration
    {
        Issuer = jwtSettingsSections["Issuer"],
        Audience = jwtSettingsSections["Audience"],
        SecretKey = jwtSettingsSections["SecretKey"]
    };

    var userId = "123"; 

    var token = GenerateJwtToken(jwtSettings, userId);
    return Results.Text(token);
});

// reverse proxy
app.MapReverseProxy();

app.Run("http://localhost:5000");

string GenerateJwtToken(JwtConfiguration jwtSettings, string userId)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new ("userId", userId),
        new (ClaimTypes.Name, "tonde")
    };

    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );

    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
}