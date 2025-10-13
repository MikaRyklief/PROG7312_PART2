using System;
using System.Collections.Generic;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    /// <summary>
    /// Represents a local community event or municipal announcement that can be
    /// surfaced in the Local Events and Announcements experience.
    /// </summary>
    public record LocalEvent
    {
        /// <summary>
        /// Unique identifier used for equality comparisons and UI anchors.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Short headline describing the event.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Name of the category that the event belongs to (e.g. "Health", "Community").
        /// </summary>
        public string Category { get; init; } = string.Empty;

        /// <summary>
        /// Event start date and time.
        /// </summary>
        public DateTime StartDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(9);

        /// <summary>
        /// Optional end date for the event.
        /// </summary>
        public DateTime? EndDate { get; init; }
            = DateTime.Today.AddDays(1).AddHours(12);

        /// <summary>
        /// Venue or area in which the event takes place.
        /// </summary>
        public string Location { get; init; } = string.Empty;

        /// <summary>
        /// Expanded description presented in the details card.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Collection of descriptive tags used by the recommendation feature.
        /// </summary>
        public IReadOnlyCollection<string> Tags { get; init; }
            = Array.Empty<string>();

        /// <summary>
        /// Flag indicating whether the event should surface under the announcements
        /// banner (stack of high priority announcements).
        /// </summary>
        public bool IsAnnouncement { get; init; }
            = false;
    }
}
