namespace Startapp.Shared.Models
{
    public class ApiResponse<T>
    {
        public T Json { get; set; }
        public string Message { get; set; }
    }

    public class JsonResponse
    {
        public object Json { get; set; } = new { };
        public string Message { get; set; } = "Successfully loaded!";
    }
    
}
