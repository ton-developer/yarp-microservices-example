using Microsoft.AspNetCore.Identity;
using User_microservices_example.Data.Models;

namespace User_microservices_example.Services;

public class UserService(UserManager<IdentityUser> userManager)
{
    public async Task<UserResponse?> GetUserByUserId(string userId)
    {
        var response = await userManager.FindByIdAsync(userId);
        
        return response != null ? new UserResponse { Email = response.Email, UserName = response.UserName} : null;
    }
}