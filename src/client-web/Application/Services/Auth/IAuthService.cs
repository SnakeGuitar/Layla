using client_web.Application.Schemas.Auth;

public interface IAuthService
{
    /// <summary>
    /// Servicio para iniciar autenticación y obtener un token JWT.
    /// </summary>
    /// <param name="LoginRequest">Datos de solicitud de inicio de sesión.</param>
    public Task<LoginResponse> LoginAsync(LoginRequest requestData);
}