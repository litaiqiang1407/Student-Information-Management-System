using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SIMS_APIs.Functions;
using SIMS.Data.Entities.Enums;
using SIMS.Data.Entities;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SIMS_APIs.Models;

namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DatabaseInteraction _dbInteraction;
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _dbInteraction = new DatabaseInteraction(configuration, env);
        }

        private async Task<JsonResult> GetList(string query)
        {
            DataTable dt = await _dbInteraction.GetData(query);
            return new JsonResult(dt);
        }

        private async Task<JsonResult> GetData(string query, SqlParameter[] sqlParameters = null)
        {
            DataTable dt = await _dbInteraction.GetData(query, sqlParameters);
            return new JsonResult(dt);
        }

        private async Task<JsonResult> Create(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.ExecuteNonQuery(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        private async Task<JsonResult> Update(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.ExecuteNonQuery(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        private async Task<JsonResult> Delete(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.ExecuteNonQuery(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        [HttpGet]
        [Route("GetAccount")]
        public async Task<JsonResult> GetAccount()
        {
            string getAccountQuery = @"SELECT
                                       A.ID,
                                       A.MemberCode, 
                                       A.Email, 
                                       CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt,
                                       CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, 
                                       UI.Name AS Name, 
                                       R.Name AS Role 
                                       FROM Account A 
                                       LEFT JOIN UserInfo UI ON A.ID = UI.AccountID 
                                       LEFT JOIN UserRole UR ON A.ID = UR.AccountID 
                                       LEFT JOIN Role R ON UR.RoleID = R.ID";

            return await GetList(getAccountQuery);
        }

        [HttpGet]
        [Route("GetRoleFilter")]
        public async Task<JsonResult> GetRoleFilter()
        {
            string getRoleAccountQuery = "SELECT R.Name AS Role FROM Role R";
            return await GetList(getRoleAccountQuery);
        }

        [HttpPost]
        [Route("AddAccount")]
        public async Task<IActionResult> AddAccount([FromBody] AddAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Invalid input data.", errors });
            }

            try
            {
                var result = await _dbInteraction.AddAccountWithTransaction(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while adding the account.", details = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteAccount/{id}")]
        public async Task<JsonResult> DeleteAccount(int id)
        {
            return await _dbInteraction.DeleteAccountAndRelatedData(id);
        }

        [HttpGet]
        [Route("GetAdmin")]
        public async Task<JsonResult> GetAdmin()
        {
            return await GetDataByRole("Admin");
        }

        private async Task<JsonResult> GetDataByRole(string roleName)
        {
            string getQuery = @$"SELECT
                                A.ID,
                                A.MemberCode, 
                                A.Email, 
                                CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, 
                                CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, 
                                UI.Name AS Name 
                                FROM Account A 
                                LEFT JOIN UserInfo UI ON A.ID = UI.AccountID 
                                LEFT JOIN UserRole UR ON A.ID = UR.AccountID 
                                LEFT JOIN Role R ON UR.RoleID = R.ID 
                                WHERE R.Name = '{roleName}'";

            return await GetList(getQuery);
        }

        [HttpGet]
        [Route("GetLecturer")]
        public async Task<JsonResult> GetLecturer()
        {
            return await GetDataByRole("Lecturer");
        }

        [HttpGet]
        [Route("GetStudent")]
        public async Task<JsonResult> GetStudent()
        {
            return await GetDataByRole("Student");
        }

        [HttpGet]
        [Route("GetCourses")]
        public async Task<JsonResult> GetCourses()
        {
            string getCoursesQuery = @"SELECT
                                       C.ID AS CourseID,
                                       S.Name AS Subject,
                                       D.Name AS Department,
                                       SEM.Name AS Semester,
                                       UI.Name AS Lecturer,
                                       C.ClassName AS Class,
                                       C.StartDate,
                                       C.EndDate
                                       FROM Course C
                                       INNER JOIN Subject S ON C.SubjectID = S.ID
                                       INNER JOIN Department D ON C.DepartmentID = D.ID
                                       INNER JOIN Semester SEM ON C.SemesterID = SEM.ID
                                       INNER JOIN Account A ON C.AccountID = A.ID
                                       INNER JOIN UserRole UR ON A.ID = UR.AccountID
                                       INNER JOIN Role R ON UR.RoleID = R.ID AND R.Name = 'Lecturer'
                                       LEFT JOIN UserInfo UI ON A.ID = UI.AccountID";

            return await GetList(getCoursesQuery);
        }

        [HttpGet]
        [Route("GetDepartmentFilter")]
        public async Task<JsonResult> GetDepartmentFilter()
        {
            string getDepartmentsQuery = "SELECT D.Name AS Department FROM Department D";
            return await GetList(getDepartmentsQuery);
        }

        [HttpGet]
        [Route("GetSemesterFilter")]
        public async Task<JsonResult> GetSemesterFilter()
        {
            string getSemestersQuery = "SELECT SEM.Name AS Semester FROM Semester SEM";
            return await GetList(getSemestersQuery);
        }

        [HttpGet]
        [Route("GetLecturerFilter")]
        public async Task<JsonResult> GetLecturerFilter()
        {
            string getLecturersQuery = @"SELECT 
                                        UI.Name AS Lecturer 
                                        FROM UserInfo UI 
                                        INNER JOIN Account A ON UI.AccountID = A.ID 
                                        INNER JOIN UserRole UR ON A.ID = UR.AccountID 
                                        INNER JOIN Role R ON UR.RoleID = R.ID AND R.Name = 'Lecturer'";

            return await GetList(getLecturersQuery);
        }

        [HttpGet]
        [Route("GetSubjects")]
        public async Task<JsonResult> GetSubjects()
        {
            string getSubjectsQuery = @"SELECT SubjectCode, Name, Credits, Slots, Fee FROM Subject";
            return await GetList(getSubjectsQuery);
        }
        [HttpGet]
        [Route("GetDepartments")]
        public async Task<JsonResult> GetDepartments()
        {
            string getDepartmentsQuery = "SELECT Name FROM Department";
            return await GetList(getDepartmentsQuery);
        }

        [HttpGet("UserInfos/{id}")]
        public async Task<IActionResult> GetUserInfoById(int id)
        {
            string getUserInfoByIdQuery = @"
                SELECT 
            UI.[AccountID],
            UI.[Name] AS UserName,
            UI.[Gender],
            UI.[DateOfBirth],
            UI.[PersonalAvatar],
            UI.[OfficialAvatar],
            UI.[PersonalPhone],
            UI.[ContactPhone1],
            UI.[ContactPhone2],
            UI.[PermanentAddress],
            UI.[TemporaryAddress],
	        UI.OfficialAvatar,
            A.[MemberCode],
            A.[Email],
            R.[Name] AS RoleName,  
            M.[Name] AS MajorName, 
            D.[Name] AS DepartmentName 
        FROM 
            [SIMS].[dbo].[UserInfo] UI
        LEFT JOIN 
            [SIMS].[dbo].[StudentDetail] SD
            ON UI.[AccountID] = SD.[AccountID]
        LEFT JOIN 
            [SIMS].[dbo].[Major] M
            ON SD.[MajorID] = M.[ID]
        LEFT JOIN 
            [SIMS].[dbo].[Department] D
            ON M.[DepartmentID] = D.[ID]
        JOIN 
            [SIMS].[dbo].[UserRole] UR
            ON UI.[AccountID] = UR.[AccountID]
        JOIN 
            [SIMS].[dbo].[Role] R
            ON UR.[RoleID] = R.[ID]
        JOIN 
            [SIMS].[dbo].[Account] A
            ON UI.[AccountID] = A.[ID] -- Joined with Account table to get MemberCode and Email
        WHERE 
            UI.[AccountID] = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
        new SqlParameter("@ID", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getUserInfoByIdQuery, sqlParameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound();
            }

            DataRow row = dataTable.Rows[0];
            bool genderParsed = Enum.TryParse(row["Gender"].ToString(), out Gender genderEnum);

            UserInfos userInfos = new UserInfos
            {
                AccountID = Convert.ToInt32(row["AccountID"]),
                Name = Convert.ToString(row["UserName"]),
                Gender = genderParsed ? genderEnum : Gender.Other,
                DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]),
                PersonalAvatar = Convert.ToString(row["PersonalAvatar"]),
                ImagePath = Convert.ToString(row["OfficialAvatar"]),
                PersonalPhone = Convert.ToString(row["PersonalPhone"]),
                ContactPhone1 = Convert.ToString(row["ContactPhone1"]),
                ContactPhone2 = Convert.ToString(row["ContactPhone2"]),
                PermanentAddress = Convert.ToString(row["PermanentAddress"]),
                TemporaryAddress = Convert.ToString(row["TemporaryAddress"]),
                Email = Convert.ToString(row["Email"]),
                Role = Convert.ToString(row["RoleName"]),
                Major = Convert.ToString(row["MajorName"]),
                DepartmentName = Convert.ToString(row["DepartmentName"]),
                MemberCode = Convert.ToString(row["MemberCode"]) // Include MemberCode
            };
            return Ok(userInfos);
        }
        [HttpPut("UpdateUserInfos/{id}")]
        public async Task<IActionResult> UpdateUserInfos(int id, [FromBody] UpdateAccountRequest request)
        {
            Console.WriteLine($"Received data: {JsonConvert.SerializeObject(request)}");

            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var result = await _dbInteraction.UpdateUserInfosAsync(id, request);

                if (result)
                {
                    return Ok(new { success = true, message = "Account updated successfully." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Failed to update the account." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateUserInfos Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the account.", details = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetMajors")]
        public async Task<JsonResult> GetMajors()
        {
            string getMajorsQuery = @"
            SELECT
            M.ID,
            M.Name AS Name,
            D.Name AS Department
            FROM
            Major M
            INNER JOIN
            Department D ON M.DepartmentID = D.ID
            ORDER BY
            M.ID";
  
            return await GetList(getMajorsQuery);
        }
        [HttpGet]
        [Route("GetRoles")]
        public async Task<JsonResult> GetRoles()
        {
            string getRolesQuery = @"
            SELECT
            R.Name AS Name
            FROM
            Role R
            ORDER BY
            R.ID";
            return await GetList(getRolesQuery);
        }
        [HttpGet]
        [Route("GetSubjets")]
        public async Task<JsonResult> GetSubjets()
        {
            string getSubjetsQuery = @"
            SELECT
            [ID],
            [SubjectCode],
            [Name]
            FROM [SIMS].[dbo].[Subject]";
            return await GetList(getSubjetsQuery);
        }
    }
}
