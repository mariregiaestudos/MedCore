namespace MedCore.Business.DTOs.Response;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }
}
