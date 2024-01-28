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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
            };
        });

// authorization 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
    options.AddPolicy("authenticatedAndAdmin", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireClaim("user_id");
        policyBuilder.RequireRole("Admin");
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

// reverse proxy
app.MapReverseProxy();

app.Run("http://localhost:5000");