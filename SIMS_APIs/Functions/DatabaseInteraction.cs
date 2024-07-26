using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Enums;
using System.Threading.Tasks;

namespace SIMS_APIs.Functions
{
    public class DatabaseInteraction
    {
        private readonly IConfiguration _configuration;

        private string SIMSConnection => _configuration.GetConnectionString("SIMSConnection");

        public DatabaseInteraction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DataTable> GetList(string query)
        // Method to get data without parameters
        public async Task<DataTable> GetData(string query)
        {
            return await GetData(query);
        }

        public async Task<DataTable> GetDataByID(string query, SqlParameter[] sqlParameters)
        {
            return await GetData(query, sqlParameters);
        }

        public async Task<int> Create(string query, SqlParameter[] sqlParameters)
        {
            return await ExecuteNonQuery(query, sqlParameters);
        }

        public async Task<int> Update(string query, SqlParameter[] sqlParameters)
        {
            return await ExecuteNonQuery(query, sqlParameters);
        }

        public async Task<int> Delete(string query, SqlParameter[] sqlParameters)
        {
            return await ExecuteNonQuery(query, sqlParameters);
        }

        public async Task<DataTable> GetData(string query, SqlParameter[] sqlParameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        dt.Load(myReader);
                    }
                }
            }
            return dt;
        }

        // Overloaded method to get data with parameters
        public async Task<DataTable> GetData(string query, SqlParameter[] sqlParameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    if (sqlParameters != null)
                    {
                        myCommand.Parameters.AddRange(sqlParameters);
                    }

                    myReader = await myCommand.ExecuteReaderAsync();
                    dt.Load(myReader);
                    myReader.Close();
                    await myCon.CloseAsync();
                    if (sqlParameters != null)
                    {
                        myCommand.Parameters.AddRange(sqlParameters);
                    }

                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        dt.Load(myReader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> ExecuteQuery(string query, SqlParameter[] sqlParameters = null)
        {
            int rowsAffected;

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    if (sqlParameters != null)
                    {
                        myCommand.Parameters.AddRange(sqlParameters);
                    }

                    rowsAffected = await myCommand.ExecuteNonQueryAsync();
                }
            }

            return rowsAffected;
        }

        public async Task<int> ExecuteNonQuery(string query, SqlParameter[] sqlParameters = null)
        {
            return await ExecuteQuery(query, sqlParameters);
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(SIMSConnection))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM Account WHERE Email = @Email AND Password = @Password", connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Note: Consider hashing passwords for security

                int count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<UserInfos> GetUserInfoAsync(string email)
        {
            using (SqlConnection connection = new SqlConnection(SIMSConnection))
            {
                await connection.OpenAsync();
                string query = @"SELECT UI.*, R.Name AS Role
                                FROM UserInfo UI
                                INNER JOIN Account A ON UI.AccountID = A.ID
                                INNER JOIN UserRole UR ON UR.AccountID = A.ID
                                INNER JOIN Role R ON UR.RoleID = R.ID
                                WHERE A.Email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserInfos
                        {
                            ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                            AccountID = reader.IsDBNull(reader.GetOrdinal("AccountID")) ? 0 : reader.GetInt32(reader.GetOrdinal("AccountID")),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                            Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? string.Empty : reader.GetString(reader.GetOrdinal("Role")),
                            Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? Gender.Unknown : (Gender)Enum.Parse(typeof(Gender), reader.GetString(reader.GetOrdinal("Gender"))),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            PersonalAvatar = reader.IsDBNull(reader.GetOrdinal("PersonalAvatar")) ? string.Empty : reader.GetString(reader.GetOrdinal("PersonalAvatar")),
                            OfficialAvatar = reader.IsDBNull(reader.GetOrdinal("OfficialAvatar")) ? string.Empty : reader.GetString(reader.GetOrdinal("OfficialAvatar")),
                            PersonalPhone = reader.IsDBNull(reader.GetOrdinal("PersonalPhone")) ? string.Empty : reader.GetString(reader.GetOrdinal("PersonalPhone")),
                            ContactPhone1 = reader.IsDBNull(reader.GetOrdinal("ContactPhone1")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContactPhone1")),
                            ContactPhone2 = reader.IsDBNull(reader.GetOrdinal("ContactPhone2")) ? string.Empty : reader.GetString(reader.GetOrdinal("ContactPhone2")),
                            PermanentAddress = reader.IsDBNull(reader.GetOrdinal("PermanentAddress")) ? string.Empty : reader.GetString(reader.GetOrdinal("PermanentAddress")),
                            TemporaryAddress = reader.IsDBNull(reader.GetOrdinal("TemporaryAddress")) ? string.Empty : reader.GetString(reader.GetOrdinal("TemporaryAddress")),
                        };
                    }
                }
            }

            return null;
        }
    }  
}
