namespace WorkHive.Models
{
    public class Application
    {
        public int app_id { get; set; }
        public int freelancer_id { get; set; }
        public int job_id { get; set; }
        public string proposal { get; set; }=string.Empty;
        public DateTime appliedAt { get; set; }= DateTime.Now;
        public string status { get; set; } = "Submitted";

    }

}
