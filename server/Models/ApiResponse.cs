namespace Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
        public static ApiResponse<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

}
