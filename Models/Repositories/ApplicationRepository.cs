﻿using Microsoft.Data.SqlClient;

namespace WorkHive.Models.Repositories
{

    public class ApplicationRepository
    {
        private readonly string _connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=WorkHive;Integrated Security=True;";

        public void CreateApplication(Application application)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Applications (freelancer_id, job_id, proposal, appliedAt) " +
                               "VALUES (@freelancer_id, @job_id, @proposal, @appliedAt)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@freelancer_id", application.freelancer_id);
                command.Parameters.AddWithValue("@job_id", application.job_id);
                command.Parameters.AddWithValue("@proposal", application.proposal);
                command.Parameters.AddWithValue("@appliedAt", application.appliedAt);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Application GetApplicationById(int appId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Applications WHERE app_id = @app_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@app_id", appId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Application
                        {
                            app_id = (int)reader["app_id"],
                            freelancer_id = (int)reader["freelancer_id"],
                            job_id = (int)reader["job_id"],
                            proposal = reader["proposal"].ToString() ?? string.Empty,
                            appliedAt = (DateTime)reader["appliedAt"]
                        };
                    }
                }
            }
            return null;
        }

        public void UpdateApplication(Application application)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Applications SET freelancer_id = @freelancer_id, job_id = @job_id, " +
                               "proposal = @proposal, appliedAt = @appliedAt WHERE app_id = @app_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@freelancer_id", application.freelancer_id);
                command.Parameters.AddWithValue("@job_id", application.job_id);
                command.Parameters.AddWithValue("@proposal", application.proposal);
                command.Parameters.AddWithValue("@appliedAt", application.appliedAt);
                command.Parameters.AddWithValue("@app_id", application.app_id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteApplication(int appId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Applications WHERE app_id = @app_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@app_id", appId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Application> GetAppsByFreelancerId(int freelancerId)
        {
            List<Application> applications = new List<Application>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Applications WHERE freelancer_id = @freelancer_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@freelancer_id", freelancerId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            app_id = (int)reader["app_id"],
                            freelancer_id = (int)reader["freelancer_id"],
                            job_id = (int)reader["job_id"],
                            proposal = reader["proposal"].ToString() ?? string.Empty,
                            appliedAt = (DateTime)reader["appliedAt"]
                        });
                    }
                }
            }
            return applications;
        }

        public List<Application> GetAppsByJobId(int jobId)
        {
            List<Application> applications = new List<Application>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Applications WHERE job_id = @job_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@job_id", jobId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            app_id = (int)reader["app_id"],
                            freelancer_id = (int)reader["freelancer_id"],
                            job_id = (int)reader["job_id"],
                            proposal = reader["proposal"].ToString() ?? string.Empty,
                            appliedAt = (DateTime)reader["appliedAt"]
                        });
                    }
                }
            }
            return applications;
        } 
        public List<Application> GetAllApplications()
        {
            List<Application> applications = new List<Application>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Applications";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            app_id = (int)reader["app_id"],
                            freelancer_id = (int)reader["freelancer_id"],
                            job_id = (int)reader["job_id"],
                            proposal = reader["proposal"].ToString() ?? string.Empty,
                            appliedAt = (DateTime)reader["appliedAt"]
                        });
                    }
                }
            }
            return applications;
        }
    }
}