using client_web.Helpers;
using client_web.Schemas.Auth;
using client_web.Services.Http;

namespace client_web.Services;

public class AuthService
{
    private readonly ApiClient _api;

    public AuthService(ApiClient api)
    {
        _api = api;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest requestData)
    {
        requestData.Password = EncryptData.Sha256(requestData.Password);

        LoginResponse response = await _api.RequestAsync<LoginResponse>(new RequestHttp
        {
            Endpoint = "/login",
            Method = HttpMethod.Post,
            Body = requestData,
        });

        return response;
    }
}