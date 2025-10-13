using System;
using System.Collections.Generic;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    /// <summary>
    /// Combines event search results, recommendations and supporting metadata for
    /// the Local Events and Announcements page.
    /// </summary>
    public class LocalEventsViewModel
    {
        public IReadOnlyCollection<string> Categories { get; init; }
            = Array.Empty<string>();

        public IReadOnlyCollection<LocalEvent> UpcomingEvents { get; init; }
            = Array.Empty<LocalEvent>();

        public IReadOnlyCollection<LocalEvent> Announcements { get; init; }
            = Array.Empty<LocalEvent>();

        public IReadOnlyCollection<LocalEvent> SearchResults { get; init; }
            = Array.Empty<LocalEvent>();

        public IReadOnlyCollection<LocalEvent> Recommendations { get; init; }
            = Array.Empty<LocalEvent>();

        public IReadOnlyCollection<EventSearchRecord> RecentSearches { get; init; }
            = Array.Empty<EventSearchRecord>();

        public string? SelectedCategory { get; init; }
            = string.Empty;

        public DateOnly? SelectedDate { get; init; }
            = null;

        public bool HasSearched { get; init; }
            = false;
    }
}
