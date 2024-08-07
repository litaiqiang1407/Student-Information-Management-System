using System;

namespace SIMS.Shared.Helpers
{
    public static class ApiEndpointHelper
    {
        public static string GetApiEndpoint(string entity, string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UserInfos/{id}",
            "course" => $"api/Admin/GetCourseById/{id}",
            "subject" => $"api/Admin/GetSubjectById/{id}",
            "semester" => $"api/Admin/GetSemesterById/{id}",
            "department" => $"api/Admin/GetDepartmentById/{id}",
            "major" => $"api/Admin/GetMajorById/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string GetApiEndpointWithoutid(string type) => type.ToLower() switch
        {
            "major" => $"api/Admin/GetMajors",
            "role" => $"api/Admin/GetRoles",
            "subject" => $"api/Admin/GetSubjects",
            "semester" => $"api/Admin/GetSemesters",
            "lecturer" => $"api/Admin/GetLecturer",
            "department" => $"api/Admin/GetDepartments",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string AddApiEndpoint(string entity) => entity.ToLower() switch
        {
            "course" => $"api/Admin/AddCourse",
            "account" => $"api/Admin/AddAccount",
            "subject" => $"api/Admin/AddSubject",
            "semester" => $"api/Admin/AddSemester",
            "department" => $"api/Admin/AddDepartment",
            "major" => $"api/Admin/AddMajor",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string UpdateApiEndpoint(string entity,string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UpdateUserInfos/{id}",
            "course" => $"api/Admin/UpdateCourseById/{id}",
            "subject" => $"api/Admin/UpdateSubject/{id}",
            "semester" => $"api/Admin/UpdateSemesterById/{id}",
            "department" => $"api/Admin/UpdateDepartmentById/{id}",
            "major" => $"api/Admin/UpdateMajorById/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
    }
}
