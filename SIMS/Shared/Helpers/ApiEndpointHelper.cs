﻿using System;

namespace SIMS.Shared.Helpers
{
    public static class ApiEndpointHelper
    {
        public static string GetApiEndpoint(string entity, string id) => entity.ToLower() switch
        {
            "account" => $"api/Admin/UserInfos/{id}",
            "course" => $"api/Admin/GetCourses/{id}",
            _ => throw new ArgumentException("Invalid Entity")
        };
        public static string GetApiEndpointWithoutid(string type) => type.ToLower() switch
        {
            "major" => $"api/Admin/GetMajors",
            "role" => $"api/Admin/GetRoles",
            "subject" => $"api/Admin/GetSubjets",
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
