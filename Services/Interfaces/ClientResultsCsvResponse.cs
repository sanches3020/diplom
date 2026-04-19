namespace Sofia.Web.Services.Interfaces
{
    public class ClientResultsCsvResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
    }
}