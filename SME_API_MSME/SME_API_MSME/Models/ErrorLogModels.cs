namespace SME_API_MSME.Models
{
    public class ErrorLogModels
    {
        public int Id { get; set; }

        public string? SystemCode { get; set; }

        public string? Message { get; set; }

        public string? StackTrace { get; set; }

        public string? Source { get; set; }

        public string? TargetSite { get; set; }

        public DateTime? ErrorDate { get; set; }

        public string? UserName { get; set; }

        public string? Path { get; set; }

        public string? HttpMethod { get; set; }

        public string? RequestData { get; set; }

        public string? InnerException { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? Createdate { get; set; }
    }
}
