using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PROG7312_Part1_POE_ST10318273.Models;
using PROG7312_Part1_POE_ST10318273.Services;

namespace PROG7312_Part1_POE_ST10318273.Models
{
    // ViewModel used to pass event-related data from the controller to the view.
    public class LocalEventsViewModel
    {
        // List of available event categories
        public IReadOnlyCollection<string> Categories { get; init; }
            = Array.Empty<string>();

        // List of upcoming events
        public IReadOnlyCollection<LocalEvent> UpcomingEvents { get; init; }
            = Array.Empty<LocalEvent>();

        // List of current announcements
        public IReadOnlyCollection<LocalEvent> Announcements { get; init; }
            = Array.Empty<LocalEvent>();

        // Results returned from an event search
        public IReadOnlyCollection<LocalEvent> SearchResults { get; init; }
            = Array.Empty<LocalEvent>();

        // Recommended events for the user
        public IReadOnlyCollection<LocalEvent> Recommendations { get; init; }
            = Array.Empty<LocalEvent>();

        // Available sorting options (Date, Category, Name)
        public IReadOnlyCollection<SortOption> SortOptions { get; init; }
            = Array.Empty<SortOption>();

        // Recent searches performed by the user
        public IReadOnlyCollection<EventSearchRecord> RecentSearches { get; init; }
            = Array.Empty<EventSearchRecord>();

        // Currently selected category filter
        public string? SelectedCategory { get; init; }
            = string.Empty;

        // Currently selected sort order
        public string SelectedSortOrder { get; init; }
            = SortOption.Date;

        // Currently selected date filter
        public DateOnly? SelectedDate { get; init; }
            = null;

        // Defines possible sorting options
        public readonly record struct SortOption(string Value, string Label)
        {
            public const string Date = "date";
            public const string Category = "category";
            public const string Name = "name";
        }

        // Indicates whether a search has been performed
        public bool HasSearched { get; init; }
            = false;
    }
}
