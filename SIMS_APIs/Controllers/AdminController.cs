using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SIMS_APIs.Functions;
using SIMS.Data.Entities.Enums;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Admin;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SIMS_APIs.Models;
using Microsoft.EntityFrameworkCore;

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

        [Route("DeleteMajor/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            try
            {
                // Xóa Major bằng cách gọi phương thức DeleteWithTransaction trong DatabaseInteraction
                await _dbInteraction.DeleteMajorWithTransaction(id);

                return Ok(new { Message = "Major deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting major", Error = ex.Message });
            }
        }

        [Route("AddMajor")]
        [HttpPost]
        public async Task<IActionResult> AddMajor([FromBody] AddMajorRequest newMajor)
        {
            try
            {
                await _dbInteraction.AddMajorWithTransaction(newMajor);

                return Ok(new { Message = "Major added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding major", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task<IActionResult> AddCourse([FromBody] AddCourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Invalid input data.", errors });
            }

            try
            {
                var result = await _dbInteraction.AddCourseWithTransaction(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while adding the course.", details = ex.Message });
            }
        }

        [HttpDelete]
        [Route("course/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var result = await _dbInteraction.DeleteCourseAsync(courseId);
            return result;
        }

        [Route("DeleteSubject/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            try
            {
                await _dbInteraction.DeleteSubjectWithTransaction(id);

                return Ok(new { Message = "Subject deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting subject", Error = ex.Message });
            }
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
            string getCoursesQuery = @"
        SELECT
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

            DataTable dataTable = await _dbInteraction.GetData(getCoursesQuery, new SqlParameter[0]);

            List<Courses> coursesList = new List<Courses>();

            foreach (DataRow row in dataTable.Rows)
            {
                var course = new Courses
                {
                    ID = Convert.ToInt32(row["CourseID"]),
                    Subject = Convert.ToString(row["Subject"]),
                    Department = Convert.ToString(row["Department"]),
                    Semester = Convert.ToString(row["Semester"]),
                    Lecturer = Convert.ToString(row["Lecturer"]),
                    Class = Enum.TryParse<Class>(Convert.ToString(row["Class"]), true, out var classEnum) ? classEnum : (Class?)null,
                    StartDate = row.IsNull("StartDate") ? (DateTime?)null : Convert.ToDateTime(row["StartDate"]),
                    EndDate = row.IsNull("EndDate") ? (DateTime?)null : Convert.ToDateTime(row["EndDate"])
                };

                coursesList.Add(course);
            }

            return new JsonResult(coursesList);
        }

        [Route("GetCourseById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCourseById(int id)
        {
            string getCourseByIdQuery = @"
            SELECT 
                c.ID AS CourseID,
                s.Name AS SubjectName,
                sm.Name AS SemesterName,
                u.Name AS LecturerName,
                d.Name AS DepartmentName,
                c.ClassName,
                c.StartDate,
                c.EndDate
            FROM [SIMS].[dbo].[Course] c
            INNER JOIN [SIMS].[dbo].[Subject] s ON c.SubjectID = s.ID
            INNER JOIN [SIMS].[dbo].[Semester] sm ON c.SemesterID = sm.ID
            INNER JOIN [SIMS].[dbo].[UserInfo] u ON c.AccountID = u.ID
            INNER JOIN [SIMS].[dbo].[Department] d ON c.DepartmentID = d.ID
            WHERE c.ID = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getCourseByIdQuery, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound(new { Message = "Course not found" });
            }

            var row = dataTable.Rows[0];

            var course = new Courses
            {
                ID = Convert.ToInt32(row["CourseID"]),
                Subject = Convert.ToString(row["SubjectName"]),
                Department = Convert.ToString(row["DepartmentName"]),
                Semester = Convert.ToString(row["SemesterName"]),
                Lecturer = Convert.ToString(row["LecturerName"]),
                Class = Enum.TryParse<Class>(Convert.ToString(row["ClassName"]), true, out var classEnum) ? classEnum : (Class?)null,
                StartDate = row.IsNull("StartDate") ? (DateTime?)null : Convert.ToDateTime(row["StartDate"]),
                EndDate = row.IsNull("EndDate") ? (DateTime?)null : Convert.ToDateTime(row["EndDate"])
            };

            return Ok(course);
        }

        [Route("UpdateSemesterById/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSemesterById(int id, [FromBody] UpdateSemesterRequest updateRequest)
        {
            try
            {
                await _dbInteraction.UpdateSemesterWithTransaction(id, updateRequest);

                return Ok(new { Message = "Semester updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating semester", Error = ex.Message });
            }
        }

        [Route("GetDepartmentById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            string getDepartmentByIdQuery = @"
    SELECT 
        d.ID AS DepartmentID,
        d.Name AS DepartmentName
    FROM [SIMS].[dbo].[Department] d
    WHERE d.ID = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getDepartmentByIdQuery, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound(new { Message = "Department not found" });
            }

            var row = dataTable.Rows[0];

            var department = new
            {
                ID = Convert.ToInt32(row["DepartmentID"]),
                Name = Convert.ToString(row["DepartmentName"])
            };

            return Ok(department);
        }

        [Route("UpdateMajorById/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateMajorById(int id, [FromBody] AddMajorRequest updateRequest)
        {
            if (updateRequest == null)
            {
                return BadRequest(new { Message = "Invalid request data" });
            }

            try
            {
                // Call the method to update the major
                await _dbInteraction.UpdateMajorWithTransaction(id, updateRequest);

                return Ok(new { Message = "Major updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating major", Error = ex.Message });
            }
        }

        [Route("GetMajorById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetMajorById(int id)
        {
            string getMajorByIdQuery = @"
    SELECT 
        m.ID AS MajorID,
        m.Name AS MajorName
    FROM [SIMS].[dbo].[Major] m
    WHERE m.ID = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getMajorByIdQuery, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound(new { Message = "Major not found" });
            }

            var row = dataTable.Rows[0];

            var major = new
            {
                ID = Convert.ToInt32(row["MajorID"]),
                Name = Convert.ToString(row["MajorName"])
            };

            return Ok(major);
        }

        [Route("GetSemesterById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            string getSemesterByIdQuery = @"
        SELECT 
            s.ID AS SemesterID,
            s.Name AS SemesterName,
            s.StartDate,
            s.EndDate
        FROM [SIMS].[dbo].[Semester] s
        WHERE s.ID = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getSemesterByIdQuery, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound(new { Message = "Semester not found" });
            }

            var row = dataTable.Rows[0];

            var semester = new
            {
                ID = Convert.ToInt32(row["SemesterID"]),
                Name = Convert.ToString(row["SemesterName"]),
                StartDate = row.IsNull("StartDate") ? (DateTime?)null : Convert.ToDateTime(row["StartDate"]),
                EndDate = row.IsNull("EndDate") ? (DateTime?)null : Convert.ToDateTime(row["EndDate"])
            };

            return Ok(semester);
        }

        [Route("DeleteSemester/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            try
            {
                await _dbInteraction.DeleteSemesterWithTransaction(id);
                return Ok(new { Message = "Semester deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting semester", Error = ex.Message });
            }
        }

        [Route("AddSemester")]
        [HttpPost]
        public async Task<IActionResult> AddSemester([FromBody] AddSemesterRequest addSemesterRequest)
        {
            try
            {
                await _dbInteraction.AddSemesterWithTransaction(addSemesterRequest);
                return Ok(new { Message = "Semester added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding semester", Error = ex.Message });
            }
        }

        [Route("GetSubjectById/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            string getSubjectByIdQuery = @"
            SELECT 
                s.ID AS SubjectID,
                s.SubjectCode,
                s.Name AS SubjectName,
                s.Credits,
                s.Slots,
                s.Fee
            FROM [SIMS].[dbo].[Subject] s
            WHERE s.ID = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Id", id)
            };

            DataTable dataTable = await _dbInteraction.GetData(getSubjectByIdQuery, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return NotFound(new { Message = "Subject not found" });
            }

            var row = dataTable.Rows[0];

            var subject = new Subjects
            {
                ID = Convert.ToInt32(row["SubjectID"]),
                SubjectCode = Convert.ToString(row["SubjectCode"]),
                Name = Convert.ToString(row["SubjectName"]),
                Credits = row.IsNull("Credits") ? (int?)null : Convert.ToInt32(row["Credits"]),
                Slots = row.IsNull("Slots") ? (int?)null : Convert.ToInt32(row["Slots"]),
                Fee = row.IsNull("Fee") ? (decimal?)null : Convert.ToDecimal(row["Fee"])
            };

            return Ok(subject);
        }

        [Route("UpdateCourseById/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateCourseById(int id, [FromBody] UpdateCourseRequest updateRequest)
        {
            try
            {
                await _dbInteraction.UpdateCourseWithTransaction(id, updateRequest);

                return Ok(new { Message = "Course updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating course", Error = ex.Message });
            }
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
            string getSubjectsQuery = @"SELECT ID, SubjectCode, Name, Credits, Slots, Fee FROM Subject";
            return await GetList(getSubjectsQuery);
        }

        [Route("UpdateSubject/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectRequest updateRequest)
        {
            try
            {
                // Gọi phương thức cập nhật với giao dịch
                await _dbInteraction.UpdateSubjectWithTransaction(id, updateRequest);

                return Ok(new { Message = "Subject updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating subject", Error = ex.Message });
            }
        }

        [Route("AddSubject")]
        [HttpPost]
        public async Task<IActionResult> AddSubject([FromBody] AddSubjectRequest request)
        {
            try
            {
                await _dbInteraction.AddSubjectWithTransaction(request);

                return Ok(new { Message = "Subject added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding subject", Error = ex.Message });
            }
        }

        [Route("UpdateDepartmentById/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateDepartmentById(int id, [FromBody] UpdateDepartmentRequest updatedDepartment)
        {
            try
            {
                await _dbInteraction.UpdateDepartmentWithTransaction(id, updatedDepartment);

                return Ok(new { Message = "Department updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating department", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetDepartments")]
        public async Task<JsonResult> GetDepartments()
        {
            string getDepartmentsQuery = "SELECT ID, Name FROM Department";
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
            ON UI.[AccountID] = A.[ID]
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
            var userInfos = new UserInfos
            {
                AccountID = Convert.ToInt32(row["AccountID"]),
                Name = Convert.ToString(row["UserName"]),
                Gender = Enum.TryParse<Gender>(Convert.ToString(row["Gender"]), true, out var gender) ? gender : Gender.Unknown, 
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
                Department = Convert.ToString(row["DepartmentName"]),
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

        [Route("DeleteDepartment/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                await _dbInteraction.DeleteDepartmentWithTransaction(id);
                return Ok(new { Message = "Department deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting department", Error = ex.Message });
            }
        }

        [Route("AddDepartment")]
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] AddDepartmentRequest newDepartment)
        {
            try
            {
                await _dbInteraction.AddDepartmentWithTransaction(newDepartment);

                return Ok(new { Message = "Department added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding department", Error = ex.Message });
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
        [Route("GetSemesters")]
        public async Task<JsonResult> GetSemesters()
        {
            string getSemestersQuery = @"
             SELECT
             [ID],
             [Name],
             [StartDate],
             [EndDate]
             FROM [SIMS].[dbo].[Semester]";
            return await GetList(getSemestersQuery);
        }

    }
}
