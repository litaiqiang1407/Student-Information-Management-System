using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SIMS_APIs.Functions;
using System.Data;
using System.Threading.Tasks;

namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DatabaseInteraction _dbInteraction;

        private readonly IConfiguration _configuration; // Khai báo biến _configuration

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration; // Khởi tạo biến _configuration
            _dbInteraction = new DatabaseInteraction(configuration);
        }

        // Get List of Data
        private async Task<JsonResult> GetList(string query)
        {
            DataTable dt = await _dbInteraction.GetList(query);
            return new JsonResult(dt);
        }

        // Create Data
        private async Task<JsonResult> Create(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.Create(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        // Update Data
        private async Task<JsonResult> Update(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.Update(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        // Delete Data
        private async Task<JsonResult> Delete(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected = await _dbInteraction.Delete(query, sqlParameters);
            return new JsonResult(new { success = rowsAffected > 0 });
        }

        // Accounts
        [HttpGet]
        [Route("GetAccount")]
        public async Task<JsonResult> GetAccount()
        {
            string getAccountQuery = @"SELECT 
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
        public async Task<JsonResult> AddAccount([FromForm] string memberCode, [FromForm] string email, [FromForm] string name, [FromForm] string role)
        {
            string addAccountQuery = @"INSERT INTO Account (MemberCode, Email, CreatedAt, UpdatedAt) 
                                       VALUES (@MemberCode, @Email, GETDATE(), GETDATE())";

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@MemberCode", memberCode),
                new SqlParameter("@Email", email)
            };

            return await Create(addAccountQuery, sqlParameters);
        }

        [HttpDelete]
        [Route("DeleteAccount/{id}")]
        public async Task<JsonResult> DeleteAccount(string id)
        {
            string deleteAccountQuery = "DELETE FROM Account WHERE ID = @id";

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            return await Delete(deleteAccountQuery, sqlParameters);
        }

        [HttpGet]
        [Route("GetAdmin")]
        public async Task<JsonResult> GetAdmin()
        {
            string getQuery = @$"SELECT 
                                A.MemberCode, 
                                A.Email, 
                                CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, 
                                CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, 
                                UI.Name AS Name 
                                FROM Account A 
                                LEFT JOIN UserInfo UI ON A.ID = UI.AccountID 
                                LEFT JOIN UserRole UR ON A.ID = UR.AccountID 
                                LEFT JOIN Role R ON UR.RoleID = R.ID 
                                WHERE R.Name = 'Admin'";

            return await GetList(getQuery);
        }

        [HttpGet]
        [Route("GetLecturer")]
        public async Task<JsonResult> GetLecturer()
        {
            string getQuery = @$"SELECT 
                                A.MemberCode, 
                                A.Email, 
                                CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, 
                                CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, 
                                UI.Name AS Name 
                                FROM Account A 
                                LEFT JOIN UserInfo UI ON A.ID = UI.AccountID 
                                LEFT JOIN UserRole UR ON A.ID = UR.AccountID 
                                LEFT JOIN Role R ON UR.RoleID = R.ID 
                                WHERE R.Name = 'Lecturer'";

            return await GetList(getQuery);
        }

        [HttpGet]
        [Route("GetStudent")]
        public async Task<JsonResult> GetStudent()
        {
            string getQuery = @$"SELECT 
                                A.MemberCode, 
                                A.Email, 
                                CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, 
                                CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, 
                                UI.Name AS Name 
                                FROM Account A 
                                LEFT JOIN UserInfo UI ON A.ID = UI.AccountID 
                                LEFT JOIN UserRole UR ON A.ID = UR.AccountID 
                                LEFT JOIN Role R ON UR.RoleID = R.ID 
                                WHERE R.Name = 'Student'";

            return await GetList(getQuery);
        }

        [HttpGet]
        [Route("GetCourses")]
        public async Task<JsonResult> GetCourses()
        {
            string getCoursesQuery = @"SELECT
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
                                       INNER JOIN UserRole UR ON A.ID = UR.AccountIDINNER JOIN Role R ON UR.RoleID = R.ID AND R.Name = 'Lecturer'
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
        [Route("GetSemesters")]
        public async Task<JsonResult> GetSemesters()
        {
            string getSemestersQuery = @"SELECT Name,
                                         CONVERT(VARCHAR(10), StartDate, 103) AS StartDate,
                                         CONVERT(VARCHAR(10), EndDate, 103) AS EndDate
                                         FROM Semester";

            return await GetList(getSemestersQuery);
        }

        [HttpGet]
        [Route("GetDepartments")]
        public async Task<JsonResult> GetDepartments()
        {
            string getDepartmentsQuery = "SELECT Name FROM Department";

            return await GetList(getDepartmentsQuery);
        }

        [HttpGet]
        [Route("GetMajors")]
        public async Task<JsonResult> GetMajors()
        {
            string getMajorsQuery = @"SELECT M.Name, D.Name AS Department 
                                      FROM Major M 
                                      JOIN Department D ON M.DepartmentID = D.ID";
            return await GetData(getMajorsQuery);
        }

        //Userinfos
        [HttpGet]
        [Route("GetUserInfo/{id}")]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            string getUserInfoByIdQuery = @"SELECT 
    UI.[ID],
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
WHERE 
    UI.[ID] = @id";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
        new SqlParameter("@id", id)
            };

            try
            {
                var dbInteraction = new DatabaseInteraction(_configuration);
                DataTable dt = await dbInteraction.GetData(getUserInfoByIdQuery, sqlParameters);

                // Debug: Print column names
                foreach (DataColumn column in dt.Columns)
                {
                    Console.WriteLine($"Column Name: {column.ColumnName}");
                }

            return await GetList(getMajorsQuery);
        }
    }
}