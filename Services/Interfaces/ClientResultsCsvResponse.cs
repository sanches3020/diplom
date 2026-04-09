namespace Sofia.Web.Services.Interfaces
{
    public class ClientResultsCsvResponse
    {
        public object Message { get; internal set; }
        public bool Success { get; internal set; }
        public bool FileName { get; internal set; }
        public object FileBytes { get; internal set; }
    }
}