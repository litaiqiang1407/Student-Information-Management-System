using System;

namespace SIMS.Shared.Helpers
{
    public static class ApiEndpointHelper
    {
        public static string GetApiEndpoint(string entity, string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UserInfos/{id}",
            "course" => $"api/Admin/GetCourseById/{id}",
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
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string UpdateApiEndpoint(string entity,string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UpdateUserInfos/{id}",
            "course" => $"api/Admin/UpdateCourseById/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
    }
}
