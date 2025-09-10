using System;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    /// <summary>
    /// Represents a municipal issue report submitted by a citizen.
    /// This model contains all the necessary information to track and manage
    /// reported issues including location, category, description, and metadata.
    /// </summary>
    public class Issue
    {
        /// <summary>
        /// Unique identifier for the issue report.
        /// Automatically generated using GUID to ensure uniqueness across the system.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Timestamp when the issue was reported.
        /// Automatically set to current UTC time when the issue is created.
        /// </summary>
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Name of the person reporting the issue.
        /// This field is optional and can be left empty for anonymous reports.
        /// </summary>
        public string ReporterName { get; set; } 
        
        /// <summary>
        /// Location where the issue was observed or occurred.
        /// This is a required field that helps municipal staff locate the problem.
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Category classification of the issue (e.g., Sanitation, Roads, Utilities).
        /// Used for routing the issue to the appropriate municipal department.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Detailed description of the issue.
        /// This is a required field that provides context about the problem.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Relative path to any uploaded file attachment.
        /// Points to files stored in the wwwroot/uploads directory.
        /// Can be null if no file was uploaded.
        /// </summary>
        public string AttachmentPath { get; set; } // relative path under wwwroot/uploads
        
        /// <summary>
        /// Points awarded to the reporter for submitting the issue.
        /// Part of the gamification system to encourage citizen participation.
        /// Defaults to 0, typically set to 10 when issue is submitted.
        /// </summary>
        public int EngagementPointsAwarded { get; set; } = 0;
        
        /// <summary>
        /// Current status of the issue in the municipal workflow.
        /// Defaults to "Submitted" when first created.
        /// Other possible values: "In Progress", "Resolved", "Closed"
        /// </summary>
        public string Status { get; set; } = "Submitted";
    }
}
