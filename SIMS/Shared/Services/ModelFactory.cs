// File: Services/ModelFactory.cs
using SIMS.Data.Entities;
using SIMS.Data.Entities.Admin;
using SIMS.Pages.Admin.Course;
using System;

namespace SIMS.Shared.Services
{
    public static class ModelFactory
    {
        public static object CreateModel(string entity)
        {
            Console.WriteLine("Creating model for entity: " + entity);
            return entity.ToLower() switch
            {
                "account" => new UserInfos(),
                "course" => new Courses(),
                "subject" => new Subjects(),
                // Uncomment and add additional cases as needed
                // "major" => new Major(),
                // "semester" => new Semester(),
                _ => throw new ArgumentException("Invalid Entity")
            };
        }
    }
}
