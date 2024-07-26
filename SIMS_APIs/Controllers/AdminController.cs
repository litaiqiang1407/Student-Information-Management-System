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

        // Get Data
        private async Task<JsonResult> GetData(string getQuery)
        {
            DataTable dt = await _dbInteraction.GetData(getQuery);

            return new JsonResult(dt);
        }

        // Delete Data
        private async Task<JsonResult> DeleteData(string deleteQuery, string id)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            int rowsAffected = await _dbInteraction.ExecuteQuery(deleteQuery, sqlParameters);

            return new JsonResult(rowsAffected);
        }

        // Get Data by Filter
        private async Task<JsonResult> GetDataByFilter(string getQuery, string col)
        {
            DataTable dt = await _dbInteraction.GetData(getQuery);

            List<string> filter = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                filter.Add(row[col].ToString());
            }

            return new JsonResult(filter);
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

            return await GetData(getAccountQuery);
        }

        [HttpGet]
        [Route("GetRoleFilter")]
        public async Task<JsonResult> GetRoleFilter()
        {
            string getRoleAccountQuery = "SELECT R.Name AS Role FROM Role R";

            return await GetDataByFilter(getRoleAccountQuery, "Role");
        }

        [HttpPost]
        [Route("AddAccount")]
        public async Task<JsonResult> AddAccount([FromForm] string newAccount)
        {
            string addAccountQuery = "INSERT INTO Account VALUES (@newAccount)";

            DataTable dt = await _dbInteraction.GetData(addAccountQuery);

            return new JsonResult("Add Successfully");
        }

        [HttpDelete]
        [Route("DeleteAccount/{id}")]
        public async Task<JsonResult> DeleteAccount(string id)
        {
            string deleteAccountQuery = "DELETE FROM Account WHERE ID = @id";

            return await DeleteData(deleteAccountQuery, id);
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

            return await GetData(getQuery);
        }

        // Admins
        [HttpGet]
        [Route("GetAdmin")]
        public async Task<JsonResult> GetAdmin()
        {
            return await GetDataByRole("Admin");
        }

        // Lecturers
        [HttpGet]
        [Route("GetLecturer")]
        public async Task<JsonResult> GetLecturer()
        {
            return await GetDataByRole("Lecturer");
        }

        // Students
        [HttpGet]
        [Route("GetStudent")]
        public async Task<JsonResult> GetStudent()
        {
            return await GetDataByRole("Student");
        }

        // Courses
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

            return await GetData(getCoursesQuery);
        }

        // Departments for filtering
        [HttpGet]
        [Route("GetDepartmentFilter")]
        public async Task<JsonResult> GetDepartmentFilter()
        {
            string getDepartmentsQuery = "SELECT D.Name AS Department FROM Department D";

            return await GetDataByFilter(getDepartmentsQuery, "Department");
        }

        // Semesters for filtering
        [HttpGet]
        [Route("GetSemesterFilter")]
        public async Task<JsonResult> GetSemesterFilter()
        {
            string getSemestersQuery = "SELECT SEM.Name AS Semester FROM Semester SEM";

            return await GetDataByFilter(getSemestersQuery, "Semester");
        }

        // Lecturer for filtering
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

            return await GetDataByFilter(getLecturersQuery, "Lecturer");
        }

        // Subjects
        [HttpGet]
        [Route("GetSubjects")]
        public async Task<JsonResult> GetSubjects()
        {
            string getSubjectsQuery = @"SELECT SubjectCode, Name, Credits, Slots, Fee FROM Subject";

            return await GetData(getSubjectsQuery);
        }

        // Semester
        [HttpGet]
        [Route("GetSemesters")]
        public async Task<JsonResult> GetSemesters()
        {
            string getSemestersQuery = @"SELECT Name,
                                         CONVERT(VARCHAR(10), StartDate, 103) AS StartDate,
                                         CONVERT(VARCHAR(10), EndDate, 103) AS EndDate
                                         FROM Semester";

            return await GetData(getSemestersQuery);
        }

        // Department
        [HttpGet]
        [Route("GetDepartments")]
        public async Task<JsonResult> GetDepartments()
        {
            string getDepartmentsQuery = "SELECT Name FROM Department";

            return await GetData(getDepartmentsQuery);
        }

        // Major
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

                if (dt.Rows.Count == 0)
                {
                    return NotFound("User not found.");
                }

                // Convert DataTable to an anonymous object
                var userInfo = dt.AsEnumerable().Select(row => new
                {
                    ID = row.Field<int>("ID"),
                    AccountID = row.Field<int>("AccountID"),
                    //AccountName = row.Field<string>("AccountName"),
                    Name = row.Field<string>("UserName"),
                    Gender = row.Field<string>("Gender"),
                    DateOfBirth = row.Field<DateTime>("DateOfBirth"),
                    PersonalAvatar = row.Field<string>("PersonalAvatar"),
                    OfficialAvatar = row.Field<string>("OfficialAvatar"),
                    PersonalPhone = row.Field<string>("PersonalPhone"),
                    ContactPhone1 = row.Field<string>("ContactPhone1"),
                    ContactPhone2 = row.Field<string>("ContactPhone2"),
                    PermanentAddress = row.Field<string>("PermanentAddress"),
                    TemporaryAddress = row.Field<string>("TemporaryAddress"),
                    RoleName = row.Field<string>("RoleName"),
                    MajorName = row.Field<string>("MajorName"),
                    DepartmentName = row.Field<string>("DepartmentName")
                }).FirstOrDefault();

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                // Log detailed exception
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}