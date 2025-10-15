using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PROG7312_Part1_POE_ST10318273.Models;
using PROG7312_Part1_POE_ST10318273.Services;

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
        public IReadOnlyCollection<SortOption> SortOptions { get; init; }
           = Array.Empty<SortOption>();

        public IReadOnlyCollection<EventSearchRecord> RecentSearches { get; init; }
            = Array.Empty<EventSearchRecord>();

        public string? SelectedCategory { get; init; }
            = string.Empty;

        public string SelectedSortOrder { get; init; }
             = SortOption.Date;

        public DateOnly? SelectedDate { get; init; }
            = null;

        public readonly record struct SortOption(string Value, string Label)
        {
            public const string Date = "date";
            public const string Category = "category";
            public const string Name = "name";
        }
        public bool HasSearched { get; init; }
            = false;
    }
}