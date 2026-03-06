namespace client_web.Schemas;

public class ApiResponse<T>
{
    public string Message { get; set; } = string.Empty;


    public bool IsError { get; set; } = false;
    public List<string>? ErrorDetails { get; set; } = default;
    public T? Data { get; set; } = default;
}