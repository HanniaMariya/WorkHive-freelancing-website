using Microsoft.Data.SqlClient;

namespace WorkHive.Models.Repositories
{
    
public class JobRepository
{
        private readonly string _connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=WorkHive;Integrated Security=True;";
        
        // CREATE Job and SkillsRequired
        public void CreateJob(Job job)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();               
                var jobId = InsertJob(connection, job);
                InsertSkillsRequired(connection, job.skillsRequired, jobId);
            }
        }

        private int InsertJob(SqlConnection connection, Job job)
        {
            var query = @"INSERT INTO Jobs (client_id, title, description, budget, status, posted_at)
                      OUTPUT INSERTED.job_id
                      VALUES (@client_id, @title, @description, @budget, @status, @posted_at)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@client_id", job.client_id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@title", job.title);
                command.Parameters.AddWithValue("@description", job.description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@budget", job.budget ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@status", job.status);
                command.Parameters.AddWithValue("@posted_at", job.PostedAt);

                return (int)command.ExecuteScalar(); // Returns the inserted job_id
            }
        }

        private void InsertSkillsRequired(SqlConnection connection, List<SkillRequired> skills, int jobId)
        {
            var query = @"INSERT INTO SkillsRequired (job_id, skill_name)
                      VALUES (@job_id, @skill_name)";

            foreach (var skill in skills)
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@job_id", jobId);
                    command.Parameters.AddWithValue("@skill_name", skill.skill_name);
                    command.ExecuteNonQuery();
                }
            }
        }

        // UPDATE Job
        public void UpdateJob(Job job)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"UPDATE Jobs
                          SET 
                              title = @title,
                              description = @description,
                              budget = @budget,
                              status = @status,
                          WHERE job_id = @job_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@job_id", job.job_id);
                    command.Parameters.AddWithValue("@title", job.title);
                    command.Parameters.AddWithValue("@description", job.description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@budget", job.budget ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@status", job.status);
                   
                    command.ExecuteNonQuery();
                }
            }
        }

        // DELETE Job
        public void DeleteJob(int jobId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"DELETE FROM SkillsRequired WHERE job_id = @job_id;
                          DELETE FROM Jobs WHERE job_id = @job_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@job_id", jobId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // READ Job

        public List<Job> GetAllJobs()
        {
            var jobs = new List<Job>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                j.job_id, j.client_id, j.title, j.description, j.budget, j.status, j.posted_at,
                sr.skillRequired_id, sr.skill_name
            FROM Jobs j
            LEFT JOIN SkillsRequired sr ON j.job_id = sr.job_id";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    var jobMap = new Dictionary<int, Job>();

                    while (reader.Read())
                    {
                        int jobId = (int)reader["job_id"];

                        if (!jobMap.ContainsKey(jobId))
                        {
                            var job = new Job
                            {
                                job_id = jobId,
                                client_id = reader["client_id"] as int?,
                                title = reader["title"].ToString(),
                                description = reader["description"].ToString(),
                                budget = reader["budget"] as decimal?,
                                status = reader["status"].ToString(),
                                PostedAt = (DateTime)reader["posted_at"]
                            };

                            jobMap[jobId] = job;
                            jobs.Add(job);
                        }

                        if (reader["skillRequired_id"] != DBNull.Value)
                        {
                            var skill = new SkillRequired
                            {
                                skillRequired_id = (int)reader["skillRequired_id"],
                                job_id = jobId,
                                skill_name = reader["skill_name"].ToString()
                            };

                            jobMap[jobId].skillsRequired.Add(skill);
                        }
                    }
                }
            }

            return jobs;
        }


        public Job GetJobById(int jobId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"SELECT job_id, client_id, title, description, budget, status, posted_at
                          FROM Jobs WHERE job_id = @job_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@job_id", jobId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Job job = new Job()
                            {
                                job_id = (int)reader["job_id"],
                                client_id = reader["client_id"] as int?,
                                title = reader["title"].ToString(),
                                description = reader["description"] as string,
                                budget = reader["budget"] as decimal?,
                                status = reader["status"].ToString(),
                                PostedAt = (DateTime)reader["posted_at"]
                            };
                           reader.Close();
                            var query1 = @"SELECT skillRequired_id, skill_name
                          FROM SkillsRequired WHERE job_id = @job_id";

                            using (var command1 = new SqlCommand(query1, connection))
                            {
                                command1.Parameters.AddWithValue("@job_id", jobId);
                                var reader1= command1.ExecuteReader();
                                while (reader1.Read())
                                {
                                    job.skillsRequired.Add(new SkillRequired
                                    {
                                        skillRequired_id = (int)reader1["skillRequired_id"],
                                        job_id = jobId,
                                        skill_name = reader1["skill_name"].ToString()
                                    });
                                }
                                reader1.Close() ;    
                            }
                            return job;
                        }
                    }
                }

                return null;
            }
        }
        public List<Job> GetJobsByClientId(int clientId)
        {
            var jobs = new List<Job>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"SELECT job_id, client_id, title, description, budget, status, posted_at
                      FROM Jobs WHERE client_id = @client_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@client_id", clientId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var job = new Job
                            {
                                job_id = (int)reader["job_id"],
                                client_id = reader["client_id"] as int?,
                                title = reader["title"].ToString(),
                                description = reader["description"] as string,
                                budget = reader["budget"] as decimal?,
                                status = reader["status"].ToString(),
                                PostedAt = (DateTime)reader["posted_at"]
                            };

                            jobs.Add(job);
                        }
                    }
                }

                // Populate the skills for each job
                foreach (var job in jobs)
                {
                    var skillQuery = @"SELECT skillRequired_id, skill_name 
                               FROM SkillsRequired WHERE job_id = @job_id";

                    using (var skillCommand = new SqlCommand(skillQuery, connection))
                    {
                        skillCommand.Parameters.AddWithValue("@job_id", job.job_id);

                        using (var skillReader = skillCommand.ExecuteReader())
                        {
                            while (skillReader.Read())
                            {
                                job.skillsRequired.Add(new SkillRequired
                                {
                                    skillRequired_id = (int)skillReader["skillRequired_id"],
                                    job_id = job.job_id,
                                    skill_name = skillReader["skill_name"].ToString()
                                });
                            }
                        }
                    }
                }
            }

            return jobs;
        }


        // READ SkillsRequired by Job ID
        public List<SkillRequired> GetSkillsByJobId(int jobId)
        {
            var skills = new List<SkillRequired>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                
                connection.Open();  
                var query = @"SELECT skillRequired_id, skill_name
                          FROM SkillsRequired WHERE job_id = @job_id";

                using (var command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@job_id", jobId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skills.Add(new SkillRequired
                            {
                                skillRequired_id = (int)reader["skillRequired_id"],
                                job_id = jobId,
                                skill_name = reader["skill_name"].ToString()
                            });
                        }
                    }
                }
            }
            return skills;
        }
     
        public List<Job> SearchJobsBySkills(List<string> requiredSkills)
        {
            var jobs = new List<Job>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create dynamic conditions for each skill
                var placeholders = string.Join(",", requiredSkills.Select((_, index) => $"@skill{index}"));
                var query = $@"
SELECT DISTINCT j.job_id, j.client_id, j.title, j.description, j.budget, j.status, j.posted_at
FROM Jobs j
INNER JOIN SkillsRequired sr ON j.job_id = sr.job_id
WHERE sr.skill_name IN ({placeholders})
GROUP BY j.job_id, j.client_id, j.title, j.description, j.budget, j.status, j.posted_at
HAVING COUNT(DISTINCT sr.skill_name) >= @skillsCount";

             

                using (var command = new SqlCommand(query, connection))
                {
                    for (int i = 0; i < requiredSkills.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@skill{i}", requiredSkills[i]);
                    }
                    command.Parameters.AddWithValue("@skillsCount", requiredSkills.Count);

                    //command.Parameters.AddWithValue("@skills", string.Join(",", requiredSkills));
                    //command.Parameters.AddWithValue("@skillsCount", requiredSkills.Count);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var job = new Job
                            {
                                job_id = (int)reader["job_id"],
                                client_id = reader["client_id"] as int?,
                                title = reader["title"].ToString(),
                                description = reader["description"] as string,
                                budget = reader["budget"] as decimal?,
                                status = reader["status"].ToString(),
                                PostedAt = (DateTime)reader["posted_at"]
                            };


                            // Now populate the skillsRequired for each job
                            job.skillsRequired = GetSkillsByJobId(job.job_id);

                            jobs.Add(job);
                        }
                    }
                }
            }

            return jobs;
        } 
        // Helper function to get the skills required for a specific job
      

        //// SEARCH Jobs by List of Skills
        //public List<Job> SearchJobsBySkills(List<string> requiredSkills)
        //{
        //    var jobs = new List<Job>();

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        // Create dynamic conditions for each skill
        //        var query = @"
        //                SELECT DISTINCT j.job_id, j.client_id, j.title, j.description, j.budget, j.status, j.posted_at
        //                FROM Jobs j
        //                INNER JOIN SkillsRequired sr ON j.job_id = sr.job_id
        //                WHERE sr.skill_name IN (@skills) 
        //                GROUP BY j.job_id, j.client_id, j.title, j.description, j.budget, j.status, j.posted_at
        //                HAVING COUNT(DISTINCT sr.skill_name) = @skillsCount";

        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@skills", string.Join(",", requiredSkills));
        //            command.Parameters.AddWithValue("@skillsCount", requiredSkills.Count);

        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    jobs.Add(new Job
        //                    {
        //                        job_id = (int)reader["job_id"],
        //                        client_id = reader["client_id"] as int?,
        //                        title = reader["title"].ToString(),
        //                        description = reader["description"] as string,
        //                        budget = reader["budget"] as decimal?,
        //                        status = reader["status"].ToString(),
        //                        PostedAt = (DateTime)reader["posted_at"]
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return jobs;
        //}
    }

}
