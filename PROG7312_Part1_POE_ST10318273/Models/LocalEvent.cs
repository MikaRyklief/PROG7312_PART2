using System;
using System.Collections.Generic;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    // Represents a local community event or municipal announcement.
    public record LocalEvent
    {
        // Unique identifier for the event
        public Guid Id { get; init; } = Guid.NewGuid();

        // Event title or name
        public string Title { get; init; } = string.Empty;

        // Category of the event (e.g., Cleanup, Fundraiser)
        public string Category { get; init; } = string.Empty;

        // Event start date and time
        public DateTime StartDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(9);

        // Event end date and time (optional)
        public DateTime? EndDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(12);

        // Location where the event will be held
        public string Location { get; init; } = string.Empty;

        // Description or details about the event
        public string Description { get; init; } = string.Empty;

        // Tags or keywords related to the event
        public IReadOnlyCollection<string> Tags { get; init; }
            = Array.Empty<string>();

        // Indicates if the item is an announcement instead of a standard event
        public bool IsAnnouncement { get; init; } = false;
    }
}
