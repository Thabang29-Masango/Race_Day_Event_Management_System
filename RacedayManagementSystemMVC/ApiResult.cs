namespace RaceDayManagementSystemMVC.Services
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; } = string.Empty;
        public bool Unauthorized => StatusCode == 401;
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        public static ApiResult<T> Ok(T? data, int status = 200) =>
            new() { Success = true, Data = data, StatusCode = status };

        public static ApiResult<T> Fail(int status, string error) =>
            new() { Success = false, StatusCode = status, Error = error };
    }
}
