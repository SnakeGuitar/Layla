namespace client_web.Interfaces.Auth;

public interface ILogin
{
    public string User { get; set; }
    public string Password { get; set; }
    public string ErrorMessage { get; set; }
    public bool IsLoading { get; set; }

}