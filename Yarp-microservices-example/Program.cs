using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ADD CONFIGURATION
// health
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policyBuilder => policyBuilder.RequireAuthenticatedUser());
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

// authorization
app.UseAuthentication();
app.UseAuthorization();

// reverse proxy
app.MapReverseProxy();

app.Run();