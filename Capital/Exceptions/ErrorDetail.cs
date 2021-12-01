using System.Text.Json;

namespace Capital.Exceptions
{
    /// <summary>
    /// Adds common error 
    /// </summary>
    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorDetail()
        {
        }

        public ErrorDetail(int statuscode, string message)
        {
            StatusCode = statuscode;
            Message = message;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
