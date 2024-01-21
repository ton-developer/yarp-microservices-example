namespace Yarp_microservices_example.LoginService;

public class JwtConfiguration
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}