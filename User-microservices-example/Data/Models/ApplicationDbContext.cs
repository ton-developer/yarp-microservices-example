using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace User_microservices_example.Data.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class with the specified DbContext options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // The base constructor handles initializing the DbContext with the provided options.
    }
}