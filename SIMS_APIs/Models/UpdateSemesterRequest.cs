﻿namespace SIMS_APIs.Models
{
    public class UpdateSemesterRequest
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}