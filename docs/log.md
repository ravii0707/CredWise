using System;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;

namespace Loggers.service.Services
{
public class AuditLogger
{
private readonly string \_connectionString;
private static readonly object \_lockObj = new object();

        public AuditLogger(string connectionString = null)
        {
            _connectionString = connectionString ??
                @"Server=(localdb)\mssqllocaldb;Database=CredWiseDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            lock (_lockObj)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Execute(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Logs')
                        BEGIN
                            CREATE TABLE Logs (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Timestamp DATETIME2 NOT NULL,
                                Level NVARCHAR(20) NOT NULL,
                                UserType NVARCHAR(50),
                                UserId NVARCHAR(50),
                                Message NVARCHAR(MAX) NOT NULL,
                                ApiEndpoint NVARCHAR(255),
                                ApiMethod NVARCHAR(20)
                            )
                        END");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[InitializeDatabase] Error: {ex.Message}");
                }
            }
        }

        public void LogInfo(string message, string apiEndpoint = null, string apiMethod = null)
            => Log("INFO", message, null, null, apiEndpoint, apiMethod);

        public void LogWarning(string message, string apiEndpoint = null, string apiMethod = null)
            => Log("WARNING", message, null, null, apiEndpoint, apiMethod);

        public void LogError(string message, string apiEndpoint = null, string apiMethod = null)
            => Log("ERROR", message, null, null, apiEndpoint, apiMethod);

        public void LogUserLogin(string userType, string userId, string apiEndpoint = null)
            => Log("INFO", $"{userType} logged in", userType, userId, apiEndpoint, "POST");

        public void LogUserLogout(string userType, string userId, string apiEndpoint = null)
            => Log("INFO", $"{userType} logged out", userType, userId, apiEndpoint, "POST");

        public void LogUserAction(string userType, string userId, string action, string apiEndpoint = null, string apiMethod = null)
            => Log("INFO", action, userType, userId, apiEndpoint, apiMethod);

        public void LogApiRequest(string httpMethod, string endpoint, string message = null)
            => Log("INFO", message ?? $"API {httpMethod} request", null, null, endpoint, httpMethod);

        private void Log(string level, string message, string userType, string userId, string apiEndpoint, string apiMethod)
        {
            lock (_lockObj)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Execute(
                            @"INSERT INTO Logs (Timestamp, Level, UserType, UserId, Message, ApiEndpoint, ApiMethod)
                              VALUES (@Timestamp, @Level, @UserType, @UserId, @Message, @ApiEndpoint, @ApiMethod)",
                            new
                            {
                                Timestamp = DateTime.Now,
                                Level = level,
                                UserType = userType,
                                UserId = userId,
                                Message = message,
                                ApiEndpoint = apiEndpoint,
                                ApiMethod = apiMethod
                            });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Log] Error: {ex.Message}");
                }
            }
        }

        public IEnumerable<dynamic> GetRecentLogs(int count = 100)
        {
            lock (_lockObj)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    return connection.Query(
                        "SELECT TOP (@Count) * FROM Logs ORDER BY Timestamp DESC",
                        new { Count = count });
                }
            }
        }
    }

}
