namespace TrafficMonitor.Models
{
    internal struct StatsModel
    {
        public string Section { get; set; }
        public int Hits { get; set; }
        public int Successes { get; set; }
        public int Errors { get; set; }
    }
}
