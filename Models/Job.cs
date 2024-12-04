namespace WorkHive.Models
{
    public class Job
    {
        public int job_id { get; set; }
        public int? client_id { get; set; }
        public string title { get; set; } 
        public string description { get; set; }
        public decimal? budget { get; set; }
        public string status { get; set; } = "Open";
        public DateTime PostedAt { get; set; } = DateTime.Now; 
    }
}
