var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/user/{userId}/AddBlog", (string userId) => $"Hello {userId}")
    .RequireAuthorization();

app.Run("http://localhost:5100");