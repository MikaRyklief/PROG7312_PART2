using System;
using System.Collections.Generic;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    /// <summary>
    /// Represents a local community event or municipal announcement that can be
    /// surfaced in the Local Events and Announcements experience.
    /// </summary>
    // Represents a local community event or municipal announcement.
    public record LocalEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Title { get; init; } = string.Empty;

        public string Category { get; init; } = string.Empty;

        public DateTime StartDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(9);

        public DateTime? EndDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(12);

        public string Location { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public IReadOnlyCollection<string> Tags { get; init; }
            = Array.Empty<string>();

    }
}
