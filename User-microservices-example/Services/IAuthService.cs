using User_microservices_example.Data.Models;

namespace User_microservices_example.Services;

public interface IAuthService
{
    Task<(int, string)> Registration(RegistrationModel model, string role);
    Task<(int, string)> Login(LoginModel model);

    Task CreateRoles();
}