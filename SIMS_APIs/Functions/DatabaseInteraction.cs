﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Enums;

namespace SIMS_APIs.Functions
{
    public class DatabaseInteraction
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        private string SIMSConnection => _configuration.GetConnectionString("SIMSConnection");

        public DatabaseInteraction(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public async Task<JsonResult> AddAccountWithTransaction(string memberCode, string email, string name, string role, string imagePath)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        string storedProcedure = "InsertAccountAndRelatedData";

                        using (SqlCommand cmd = new SqlCommand(storedProcedure, myCon, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MemberCode", memberCode);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Role", role);
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                            // Default parameters do not need to be added

                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Commit transaction if no errors
                        transaction.Commit();

                        return new JsonResult(new { success = true, message = "Account added successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if there is an error
                        transaction.Rollback();

                        return new JsonResult(new { success = false, message = ex.Message });
                    }
                }
            }
        }

        public async Task<JsonResult> DeleteAccountAndRelatedData(int accountId)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();

                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        // Retrieve the image path before deleting the account
                        string imagePath = await GetOfficialAvatarPathByAccountId(accountId);

                        // Delete Enrollment related to AccountID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Enrollment] WHERE AccountID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Delete StudentDetail related to AccountID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[StudentDetail] WHERE AccountID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Delete UserRole related to AccountID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[UserRole] WHERE AccountID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Delete UserInfo related to AccountID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[UserInfo] WHERE AccountID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Delete Course related to AccountID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Course] WHERE AccountID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Delete Account
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Account] WHERE ID = @AccountID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Commit transaction if no errors
                        transaction.Commit();

                        // Delete the image file
                        var deleteImageResult = await DeleteImage(imagePath);
                        if (!deleteImageResult.Success)
                        {
                            // If image deletion fails, log the error or handle it as needed
                            return new JsonResult(new { success = false, message = "Account deleted but image deletion failed: " + deleteImageResult.Message });
                        }

                        return new JsonResult(new { success = true, message = "Transaction committed successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if there is an error
                        transaction.Rollback();

                        return new JsonResult(new { success = false, message = ex.Message });
                    }
                }
            }
        }

        private async Task<string> GetOfficialAvatarPathByAccountId(int accountId)
        {
            string basePath = @"C:\WorkSpace\SIMS_ASM2\SIMS\wwwroot\";
            string imagePath = string.Empty;

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT OfficialAvatar FROM [dbo].[UserInfo] WHERE AccountID = @AccountID", myCon))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            imagePath = reader["OfficialAvatar"].ToString();
                        }
                    }
                }
            }

            return Path.Combine(basePath, imagePath);
        }

        private async Task<OperationResult> DeleteImage(string imagePath)
        {
            bool isDeleted = false;
            string message = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    message = "Image path is null or empty.";
                }
                else if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    isDeleted = true;
                    message = "Image file deleted successfully.";
                }
                else
                {
                    message = "Image file does not exist.";
                    isDeleted = true; // Allow proceeding even if the image does not exist
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new OperationResult
            {
                Success = isDeleted,
                Message = message
            };
        }

        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }


        public async Task<DataTable> GetList(string query)
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
            SqlDataReader myReader;

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
                    await myCon.CloseAsync();
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