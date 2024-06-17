using Microsoft.AspNetCore.Mvc;
using SIMS_APIs.Functions;
using System.Data;

namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SIMSController : ControllerBase
    {
        private readonly DatabaseInteraction _dbInteraction;

        public SIMSController(IConfiguration configuration)
        {
            _dbInteraction = new DatabaseInteraction(configuration);
        }

        // Accounts
        [HttpGet]
        [Route("GetAccount")]
        public async Task<JsonResult> GetAccount()
        {
            string getAccountQuery = "SELECT " +
                                     "A.MemberCode, " +
                                     "A.Email, " +
                                     "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
                                     "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
                                     "UI.Name AS Name, " +
                                     "R.Name AS Role " +
                                     "FROM Account A " +
                                     "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
                                     "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
                                     "LEFT JOIN Role R ON UR.RoleID = R.ID";

            DataTable dt = await _dbInteraction.GetData(getAccountQuery);

            return new JsonResult(dt);
        }

        [HttpGet]
        [Route("GetRoleFilter")]
        public async Task<JsonResult> GetRoleAccount()
        {
            string getRoleAccountQuery = "SELECT " +
                                         "R.Name AS Role " +
                                         "FROM Role R";

            DataTable dt = await _dbInteraction.GetData(getRoleAccountQuery);

            List<string> roles = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                roles.Add(row["Role"].ToString());
            }

            return new JsonResult(roles);
        }    

        [HttpPost]
        [Route("AddAccount")]
        public async Task<JsonResult> AddAccount([FromForm] string newAccount)
        {
            string addAccountQuery = "INSERT INTO Account VALUES (@newAccount)";

            DataTable dt = await _dbInteraction.GetData(addAccountQuery);

            return new JsonResult("Add Successfully");
        }

        //Admins
        [HttpGet]
        [Route("GetAdmin")]
        public async Task<JsonResult> GetAdmin()
        {
            string getAdminQuery = "SELECT " +
                                   "A.MemberCode, " +
                                   "A.Email, " +
                                   "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
                                   "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
                                   "UI.Name AS Name " +
                                   "FROM Account A " +
                                   "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
                                   "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
                                   "LEFT JOIN Role R ON UR.RoleID = R.ID " +
                                   "WHERE R.Name = 'Admin'";

            DataTable dt = await _dbInteraction.GetData(getAdminQuery);

            return new JsonResult(dt);
        }
        
        // Lecturers
        [HttpGet]
        [Route("GetLecturer")]
        public async Task<JsonResult> GetLecturer()
        {
            string getLecturerQuery = "SELECT " +
                                      "A.MemberCode, " +
                                      "A.Email, " +
                                      "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
                                      "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
                                      "UI.Name AS Name " +
                                      "FROM Account A " +
                                      "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
                                      "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
                                      "LEFT JOIN Role R ON UR.RoleID = R.ID " +
                                      "WHERE R.Name = 'Lecturer'";

            DataTable dt = await _dbInteraction.GetData(getLecturerQuery);

            return new JsonResult(dt);
        }

        // Students
        [HttpGet]
        [Route("GetStudent")]
        public async Task<JsonResult> GetStudent()
        {
            string getStudentQuery = "SELECT " +
                                    "A.MemberCode, " +
                                    "A.Email, " +
                                    "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
                                    "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
                                    "UI.Name AS Name " +
                                    "FROM Account A " +
                                    "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
                                    "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
                                    "LEFT JOIN Role R ON UR.RoleID = R.ID " +
                                    "WHERE R.Name = 'Student'";

            DataTable dt = await _dbInteraction.GetData(getStudentQuery);

            return new JsonResult(dt);
        }

        // Courses
        [HttpGet]
        [Route("GetCourses")]
        public async Task<JsonResult> GetCourses()
        {
            string getCoursesQuery = @"
                SELECT
                    S.Name AS Subject,
                    D.Name AS Department,
                    SEM.Name AS Semester,
                    UI.Name AS Lecturer,
                    C.ClassName AS Class,
                    C.StartDate,
                    C.EndDate
                FROM
                    Course C
                INNER JOIN
                    Subject S ON C.SubjectID = S.ID
                INNER JOIN
                    Department D ON C.DepartmentID = D.ID
                INNER JOIN
                    Semester SEM ON C.SemesterID = SEM.ID
                INNER JOIN
                    Account A ON C.AccountID = A.ID
                INNER JOIN
                    UserRole UR ON A.ID = UR.AccountID
                INNER JOIN
                    Role R ON UR.RoleID = R.ID AND R.Name = 'Lecturer'
                LEFT JOIN
                    UserInfo UI ON A.ID = UI.AccountID";

            DataTable dt = await _dbInteraction.GetData(getCoursesQuery);

            return new JsonResult(dt);
        }

        // Departments for filtering
        [HttpGet]
        [Route("GetDepartmentFilter")]
        public async Task<JsonResult> GetDepartments()
        {
            string getDepartmentsQuery = "SELECT " +
                                         "D.Name AS Department " +
                                         "FROM Department D";

            DataTable dt = await _dbInteraction.GetData(getDepartmentsQuery);

            List<string> departments = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                departments.Add(row["Department"].ToString());
            }

            return new JsonResult(departments);
        }

        // Semesters for filtering
        [HttpGet]
        [Route("GetSemesterFilter")]
        public async Task<JsonResult> GetSemesters()
        {
            string getSemestersQuery = "SELECT " +
                                       "SEM.Name AS Semester " +
                                       "FROM Semester SEM";

            DataTable dt = await _dbInteraction.GetData(getSemestersQuery);

            List<string> semesters = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                semesters.Add(row["Semester"].ToString());
            }

            return new JsonResult(semesters);
        }

        // Lecturer for filtering
        [HttpGet]
        [Route("GetLecturerFilter")]
        public async Task<JsonResult> GetLecturers()
        {
            string getLecturersQuery = "SELECT " +
                                      "UI.Name AS Lecturer " +
                                      "FROM UserInfo UI " +
                                      "INNER JOIN Account A ON UI.AccountID = A.ID " +
                                      "INNER JOIN UserRole UR ON A.ID = UR.AccountID " +
                                      "INNER JOIN Role R ON UR.RoleID = R.ID AND R.Name = 'Lecturer'";

            DataTable dt = await _dbInteraction.GetData(getLecturersQuery);

            List<string> lecturers = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                lecturers.Add(row["Lecturer"].ToString());
            }

            return new JsonResult(lecturers);
        }
    }
}
