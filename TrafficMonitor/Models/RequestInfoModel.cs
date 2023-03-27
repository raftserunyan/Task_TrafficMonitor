namespace TrafficMonitor.Models
{
    public struct RequestInfoModel
    {
        public string Section { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
