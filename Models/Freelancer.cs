namespace WorkHive.Models
{
    public class Freelancer
    {
        public int FreelancerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }  
        public string? ProfilePic { get; set; } 
        public string Language { get; set; } = string.Empty;
        public string? LanguageLevel { get; set; } 
        public string? PortfolioLink { get; set; } 
        public string? Phone { get; set; } 
        public List<Skill> skills { get; set; } = new List<Skill>();
    }
}
