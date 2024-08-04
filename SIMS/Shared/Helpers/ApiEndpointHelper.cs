using System;

namespace SIMS.Shared.Helpers
{
    public static class ApiEndpointHelper
    {
        public static string GetApiEndpoint(string entity, string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UserInfos/{id}",
            "course" => $"api/Admin/Courses/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string AddApiEndpoint(string entity) => entity.ToLower() switch
        {
            "account" => $"api/Admin/AddAccount",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string UpdateApiEndpoint(string entity,string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UpdateUserInfos/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
    }
}
