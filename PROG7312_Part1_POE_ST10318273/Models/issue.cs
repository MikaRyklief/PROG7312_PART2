using System;

namespace PROG7312_Part1_POE_ST10318273.Models
{
 
    public class Issue
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public string ReporterName { get; set; } 

        public string Location { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        // relative path under wwwroot/uploads
        public string AttachmentPath { get; set; } 
        
  
        // Part of the gamification system to encourage citizen participation.
        public int EngagementPointsAwarded { get; set; } = 0;
        
        public string Status { get; set; } = "Submitted";
    }
}
