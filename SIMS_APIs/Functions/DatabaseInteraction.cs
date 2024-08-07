using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Enums;
using static SIMS_APIs.Functions.DatabaseInteraction;
using SIMS_APIs.Models;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<JsonResult> UpdateAccountWithTransaction(int accountId, string memberCode, string email, string name, string role, string imagePath)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        string storedProcedure = "UpdateAccountAndRelatedData";

                        using (SqlCommand cmd = new SqlCommand(storedProcedure, myCon, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@AccountID", accountId);
                            cmd.Parameters.AddWithValue("@MemberCode", memberCode);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Role", role);
                            cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                            await cmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();

                        return new JsonResult(new { success = true, message = "Account updated successfully" });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        return new JsonResult(new { success = false, message = ex.Message });
                    }
                }
            }
        }

        public async Task<JsonResult> AddCourseWithTransaction(AddCourseRequest request)
        {
            using (SqlConnection connection = new SqlConnection(SIMSConnection))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string storedProcedure = "AddCourse";
                        using (SqlCommand cmd = new SqlCommand(storedProcedure, connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SubjectName", request.Subject ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@SemesterName", request.Semester ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@LecturerName", request.Lecturer ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DepartmentName", request.Department ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ClassName", request.Class ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@StartDate", request.StartDate);
                            cmd.Parameters.AddWithValue("@EndDate", request.EndDate);

                            await cmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return new JsonResult(new { success = true, message = "Course added successfully" });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new JsonResult(new { success = false, message = "An error occurred while adding the course.", details = ex.Message });
                    }
                }
            }
        }

        public async Task AddSemesterWithTransaction(AddSemesterRequest request)
        {
            string storedProcedureName = "AddSemester";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@Name", request.Name),
            new SqlParameter("@StartDate", (object)request.StartDate ?? DBNull.Value),
            new SqlParameter("@EndDate", (object)request.EndDate ?? DBNull.Value)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception to be handled by the caller
                    }
                }
            }
        }

        public async Task UpdateSemesterWithTransaction(int id, UpdateSemesterRequest updateRequest)
        {
            string storedProcedureName = "UpdateSemester";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id),
        new SqlParameter("@Name", updateRequest.Name),
        new SqlParameter("@StartDate", (object)updateRequest.StartDate ?? DBNull.Value),
        new SqlParameter("@EndDate", (object)updateRequest.EndDate ?? DBNull.Value)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception to be handled by the caller
                    }
                }
            }
        }


        public async Task AddSubjectWithTransaction(AddSubjectRequest request)
        {
            string storedProcedureName = "AddSubject";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@SubjectCode", request.SubjectCode),
            new SqlParameter("@Name", request.Name),
            new SqlParameter("@Credits", request.Credits),
            new SqlParameter("@Slots", request.Slots),
            new SqlParameter("@Fee", request.Fee)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; 
                    }
                }
            }
        }

        public async Task<JsonResult> AddAccountWithTransaction(AddAccountRequest request)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        int? majorID = null;
                        if (!string.IsNullOrEmpty(request.Major))
                        {
                            majorID = await GetMajorIDByNameAsync(request.Major);
                        }

                        string storedProcedure = "InsertAccountAndRelatedData";

                        using (SqlCommand cmd = new SqlCommand(storedProcedure, myCon, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MemberCode", request.MemberCode);
                            cmd.Parameters.AddWithValue("@Email", request.Email);

                            cmd.Parameters.AddWithValue("@Password", (object)request.Password ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Name", request.Name);
                            cmd.Parameters.AddWithValue("@Gender", request.Gender);
                            cmd.Parameters.AddWithValue("@DateOfBirth", (object)request.DateOfBirth ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PersonalPhone", (object)request.PersonalPhone ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ContactPhone1", (object)request.ContactPhone1 ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ContactPhone2", (object)request.ContactPhone2 ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PermanentAddress", (object)request.PermanentAddress ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@TemporaryAddress", (object)request.TemporaryAddress ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Role", request.Role);
                            cmd.Parameters.AddWithValue("@MajorID", (object)majorID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OfficialAvatar", (object)request.ImagePath ?? DBNull.Value);

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

        public async Task DeleteSemesterWithTransaction(int semesterId)
        {
            string storedProcedureName = "DeleteSemester";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@SemesterID", semesterId)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; 
                    }
                }
            }
        }

        public async Task DeleteSubjectWithTransaction(int subjectId)
        {
            string storedProcedureName = "DeleteSubject";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@SubjectID", subjectId)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error deleting subject: {ex.Message}");
                        throw; 
                    }
                }
            }
        }
        public async Task UpdateSubjectWithTransaction(int id, UpdateSubjectRequest updateRequest)
        {
            string storedProcedureName = "UpdateSubject";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id),
        new SqlParameter("@SubjectCode", updateRequest.SubjectCode),
        new SqlParameter("@Name", updateRequest.Name),
        new SqlParameter("@Credits", updateRequest.Credits),
        new SqlParameter("@Slots", updateRequest.Slots),
        new SqlParameter("@Fee", updateRequest.Fee)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error updating subject: {ex.Message}");
                        throw; // Re-throw the exception to be handled by the caller
                    }
                }
            }
        }

        public async Task<JsonResult> DeleteCourseAsync(int courseId)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        // Xóa dữ liệu từ bảng Course
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM [SIMS].[dbo].[Course] WHERE [ID] = @CourseID", myCon, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CourseID", courseId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Commit transaction nếu không có lỗi
                        transaction.Commit();
                        return new JsonResult(new { success = true, message = "Course deleted successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi
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

                using (SqlCommand cmd = new SqlCommand("SELECT [OfficialAvatar] FROM [SIMS].[dbo].[UserInfo] WHERE [AccountID] = @AccountID", myCon))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Loại bỏ khoảng trắng ở đầu và cuối của đường dẫn
                            imagePath = reader["OfficialAvatar"].ToString().Trim();
                        }
                    }
                }
            }
            string fullPath = Path.Combine(basePath, imagePath);
            return fullPath;
        }

        public async Task<int?> GetMajorIDByNameAsync(string majorName)
        {
            const string query = "SELECT [ID] FROM [SIMS].[dbo].[Major] WHERE [Name] = @MajorName";

            try
            {
                using (var connection = new SqlConnection(SIMSConnection))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MajorName", majorName);
                        var result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : (int?)null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
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
                    isDeleted = true; 
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
                            AccountID = reader.IsDBNull(reader.GetOrdinal("AccountID")) ? 0 : reader.GetInt32(reader.GetOrdinal("AccountID")),
                            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                            Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? string.Empty : reader.GetString(reader.GetOrdinal("Role")),
                            Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? Gender.Unknown : (Gender)Enum.Parse(typeof(Gender), reader.GetString(reader.GetOrdinal("Gender"))),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            PersonalAvatar = reader.IsDBNull(reader.GetOrdinal("PersonalAvatar")) ? string.Empty : reader.GetString(reader.GetOrdinal("PersonalAvatar")),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("OfficialAvatar")) ? string.Empty : reader.GetString(reader.GetOrdinal("OfficialAvatar")),
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
        public async Task<bool> UpdateUserInfosAsync(int id, UpdateAccountRequest request)
        {
            // In thông tin của request
            Console.WriteLine($"UpdateAccountRequest Data: {request.ToString()}");

            int? majorID = null;
            if (!string.IsNullOrEmpty(request.Major))
            {
                majorID = await GetMajorIDByNameAsync(request.Major);
            }

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();

                using (var command = new SqlCommand("UpdateAccountAndRelatedData", myCon))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AccountID", id);
                    command.Parameters.AddWithValue("@MemberCode", (object)request.MemberCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)request.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Password", (object)request.Password ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Name", (object)request.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Gender", (object)request.Gender ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", request.DateOfBirth.HasValue ? (object)request.DateOfBirth.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@PersonalPhone", (object)request.PersonalPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactPhone1", (object)request.ContactPhone1 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactPhone2", (object)request.ContactPhone2 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PermanentAddress", (object)request.PermanentAddress ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TemporaryAddress", (object)request.TemporaryAddress ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MajorID", (object)majorID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ImagePath", (object)request.ImagePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Role", (object)request.Role ?? DBNull.Value);

                    try
                    {
                        int affectedRows = await command.ExecuteNonQueryAsync();
                        return affectedRows > 0;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task UpdateCourseWithTransaction(int id, UpdateCourseRequest updateRequest)
        {
            string storedProcedureName = "UpdateCourse";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id),
        new SqlParameter("@SubjectName", updateRequest.Subject),
        new SqlParameter("@SemesterName", updateRequest.Semester),
        new SqlParameter("@LecturerName", updateRequest.Lecturer),
        new SqlParameter("@DepartmentName", updateRequest.Department),
        new SqlParameter("@ClassName", updateRequest.Class),
        new SqlParameter("@StartDate", (object)updateRequest.StartDate ?? DBNull.Value),
        new SqlParameter("@EndDate", (object)updateRequest.EndDate ?? DBNull.Value)
            };

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (var transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(storedProcedureName, myCon, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception to be handled by the caller
                    }
                }
            }
        }

        public async Task<JsonResult> AddData(string storedProcedure, SqlParameter[] parameters)
        {
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(storedProcedure, myCon, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (parameters != null)
                            {
                                cmd.Parameters.AddRange(parameters);
                            }

                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Commit transaction if no errors
                        transaction.Commit();
                        return new JsonResult(new { success = true, message = "Data added successfully" });
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
    }
}


