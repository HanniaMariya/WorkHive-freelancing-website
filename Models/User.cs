namespace WorkHive.Models
{
    public class User
    {
        public int user_id {  get; set; }   
        public string name { get; set; } = string.Empty;
        public string password { get; set; }
        public string email { get; set; }
        
        public string role { get; set; }
        public DateTime joinedAt { get; set; } = DateTime.Now;
    }
}
