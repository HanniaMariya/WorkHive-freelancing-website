using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Security.Principal;
using System;
using WorkHive.Models.Repositories;
using WorkHive.Models;
using Microsoft.Data.SqlClient;

namespace WorkHive.Models.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=WorkHive;Integrated Security=True;";

        public void AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Users (username, password, email, role, joinedAt) VALUES (@Name, @Password, @Email, @Role, @date)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.name);
                    command.Parameters.AddWithValue("@Password", user.password);
                    command.Parameters.AddWithValue("@Email", user.email);
                    command.Parameters.AddWithValue("@Role", user.role);
                    command.Parameters.AddWithValue("@date", user.joinedAt);
                    command.ExecuteNonQuery();
                }
            }
        }
        public User GetUser(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE email = @Email AND password = @Password";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                user_id = (int)reader["user_id"],
                                name = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                joinedAt = (DateTime)reader["joinedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }
        public User GetUserByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                user_id = (int)reader["user_id"],
                                name = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                joinedAt = (DateTime)reader["joinedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }
        public User GetUserById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE user_id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                user_id = (int)reader["user_id"],
                                name = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                joinedAt = (DateTime)reader["joinedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE Users SET username = @Name, password = @Password, email = @Email, role = @Role WHERE user_id = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.name);
                    command.Parameters.AddWithValue("@Password", user.password);
                    command.Parameters.AddWithValue("@Email", user.email);
                    command.Parameters.AddWithValue("@Role", user.role);
                    command.Parameters.AddWithValue("@UserId", user.user_id);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteUser(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "DELETE FROM Users WHERE user_id = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                user_id = (int)reader["user_id"],
                                name = reader["username"].ToString(),
                                password = reader["password"].ToString(),
                                email = reader["email"].ToString(),
                                role = reader["role"].ToString(),
                                joinedAt =(DateTime)reader["joinedAt"]
                            });
                        }
                    }
                }
            }
            return users;
        }
    }
}
