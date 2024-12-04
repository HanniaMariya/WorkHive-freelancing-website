using Microsoft.Data.SqlClient;

namespace WorkHive.Models.Repositories
{
    public class FreelancerRepository
    {
        private readonly string _connectionString= "Data Source=(localdb)\\ProjectModels;Initial Catalog=WorkHive;Integrated Security=True;";

        // Create a freelancer and associated skills
        public void CreateFreelancer(Freelancer freelancer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Insert freelancer into Freelancers table
                string freelancerQuery = @"
                INSERT INTO Freelancers (name, profile_pic, language, language_level, portfolio_link, phone)
                OUTPUT INSERTED.freelancer_id
                VALUES (@Name, @ProfilePic, @Language, @LanguageLevel, @PortfolioLink, @Phone)";

                int freelancerId;
                using (SqlCommand cmd = new SqlCommand(freelancerQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", freelancer.Name);
                    cmd.Parameters.AddWithValue("@ProfilePic", freelancer.ProfilePic ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Language", freelancer.Language);
                    cmd.Parameters.AddWithValue("@LanguageLevel", freelancer.LanguageLevel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PortfolioLink", freelancer.PortfolioLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", freelancer.Phone ?? (object)DBNull.Value);

                    freelancerId = (int)cmd.ExecuteScalar();
                }

                // Insert skills into Skills table
                string skillsQuery = @"
                INSERT INTO Skills (freelancer_id, skill_name, experience_level)
                VALUES (@FreelancerId, @SkillName, @ExperienceLevel)";

                foreach (var skill in freelancer.skills)
                {
                    using (SqlCommand cmd = new SqlCommand(skillsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FreelancerId", freelancerId);
                        cmd.Parameters.AddWithValue("@SkillName", skill.skill_name);
                        cmd.Parameters.AddWithValue("@ExperienceLevel", skill.experience_level);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public List<Freelancer> GetFreelancersBySkill(string skillName)
        {
            List<Freelancer> freelancers = new List<Freelancer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT f.freelancer_id, f.name, f.profile_pic, f.language, f.language_level, f.portfolio_link, f.phone
                FROM Freelancers f
                INNER JOIN Skills s ON f.freelancer_id = s.freelancer_id
                WHERE s.skill_name = @SkillName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SkillName", skillName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Freelancer freelancer = new Freelancer
                            {
                                FreelancerId = (int)reader["freelancer_id"],
                                Name = (string)reader["name"],
                                ProfilePic = reader["profile_pic"] == DBNull.Value ? null : (string)reader["profile_pic"],
                                Language = (string)reader["language"],
                                LanguageLevel = reader["language_level"] == DBNull.Value ? null : (string)reader["language_level"],
                                PortfolioLink = reader["portfolio_link"] == DBNull.Value ? null : (string)reader["portfolio_link"],
                                Phone = reader["phone"] == DBNull.Value ? null : (string)reader["phone"],
                                skills = GetSkillsByFreelancerId((int)reader["freelancer_id"])
                            };
                            freelancers.Add(freelancer);
                        }
                    }
                }
            }

            return freelancers;
        }

        private List<Skill> GetSkillsByFreelancerId(int freelancerId)
        {
            List<Skill> skills = new List<Skill>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT skill_id, freelancer_id, skill_name, experience_level
                FROM Skills
                WHERE freelancer_id = @FreelancerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FreelancerId", freelancerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skills.Add(new Skill
                            {
                                skill_id = (int)reader["skill_id"],
                                freelancer_id = (int)reader["freelancer_id"],
                                skill_name = (string)reader["skill_name"],
                                experience_level = (string)reader["experience_level"]
                            });
                        }
                    }
                }
            }

            return skills;
        }
        public bool UpdateFreelancer(Freelancer freelancer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string updateFreelancerQuery = "UPDATE Freelancers SET Name = @Name, Language = @Language, " +
                                               "LanguageLevel = @LanguageLevel, PortfolioLink = @PortfolioLink, Phone = @Phone " +
                                               "WHERE freelancer_id = @FreelancerId";

                using (SqlCommand cmd = new SqlCommand(updateFreelancerQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", freelancer.Name);
                    cmd.Parameters.AddWithValue("@Language", freelancer.Language);
                    cmd.Parameters.AddWithValue("@LanguageLevel", freelancer.LanguageLevel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PortfolioLink", freelancer.PortfolioLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", freelancer.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FreelancerId", freelancer.FreelancerId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0) return false; // If no rows are updated
                }

                // Delete existing skills and re-insert new skills
                string deleteSkillsQuery = "DELETE FROM Skills WHERE freelancer_id = @FreelancerId";
                using (SqlCommand cmd = new SqlCommand(deleteSkillsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FreelancerId", freelancer.FreelancerId);
                    cmd.ExecuteNonQuery();
                }

                // Insert new skills
                if (freelancer.skills != null && freelancer.skills.Count > 0)
                {
                    foreach (var skill in freelancer.skills)
                    {
                        string insertSkillQuery = "INSERT INTO Skills (freelancer_id, skill_name, experience_level) " +
                                                  "VALUES (@FreelancerId, @SkillName, @ExperienceLevel)";
                        using (SqlCommand cmd = new SqlCommand(insertSkillQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FreelancerId", freelancer.FreelancerId);
                            cmd.Parameters.AddWithValue("@SkillName", skill.skill_name);
                            cmd.Parameters.AddWithValue("@ExperienceLevel", skill.experience_level);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return true;
        }
        public bool DeleteFreelancer(int freelancerId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string deleteSkillsQuery = "DELETE FROM Skills WHERE freelancer_id = @FreelancerId";
                using (SqlCommand cmd = new SqlCommand(deleteSkillsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FreelancerId", freelancerId);
                    cmd.ExecuteNonQuery();
                }

     
                string deleteFreelancerQuery = "DELETE FROM Freelancers WHERE freelancer_id = @FreelancerId";
                using (SqlCommand cmd = new SqlCommand(deleteFreelancerQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FreelancerId", freelancerId);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
    }
}
