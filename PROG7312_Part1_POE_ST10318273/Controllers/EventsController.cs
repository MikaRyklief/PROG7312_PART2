using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Models;
using PROG7312_Part1_POE_ST10318273.Services;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    /* Controller responsible for displaying and managing municipal events.
       Handles event searching, filtering, and sorting in the web app. */
    public class EventsController : Controller
    {
        private readonly LocalEventCatalog _catalog;

        // Predefined sorting options available to users to sort event listings.
        private static readonly LocalEventsViewModel.SortOption[] s_sortOptions = new[]
        {
            new LocalEventsViewModel.SortOption(LocalEventsViewModel.SortOption.Date, "Date"),
            new LocalEventsViewModel.SortOption(LocalEventsViewModel.SortOption.Category, "Category"),
            new LocalEventsViewModel.SortOption(LocalEventsViewModel.SortOption.Name, "Name")
        };

        public EventsController(LocalEventCatalog catalog)
        {
            _catalog = catalog;
        }

        [HttpGet]
        public IActionResult Index(string? category, DateTime? date, string? sortOrder)
        {
            // Convert nullable DateTime to DateOnly
            var selectedDate = date.HasValue
                ? DateOnly.FromDateTime(date.Value)
                : (DateOnly?)null;

            // Default sort option is by Date
            var selectedSort = LocalEventsViewModel.SortOption.Date;

            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                // Try to find a matching sort option 
                var matchedOption = s_sortOptions.FirstOrDefault(option =>
                    string.Equals(option.Value, sortOrder, StringComparison.OrdinalIgnoreCase));

                // If a valid option was found, update the selected sort
                if (!string.IsNullOrWhiteSpace(matchedOption.Value))
                {
                    selectedSort = matchedOption.Value;
                }
            }

            // Check whether any filters were applied by the user
            var hasFilters = !string.IsNullOrWhiteSpace(category) || selectedDate.HasValue;

            // Perform search only if filters exist, and then apply sorting
            var searchResults = hasFilters
                ? ApplySort(_catalog.Search(category, selectedDate), selectedSort).ToArray()
                : Array.Empty<LocalEvent>();

            // Apply sorting to upcoming events
            var upcomingEvents = ApplySort(_catalog.GetUpcomingEvents(), selectedSort).ToArray();

            var viewModel = new LocalEventsViewModel
            {
                Categories = _catalog.GetCategories(),

                UpcomingEvents = upcomingEvents,

                Announcements = _catalog.GetAnnouncements().ToArray(),

                SearchResults = searchResults,

                Recommendations = _catalog.GetRecommendations().ToArray(),

                RecentSearches = _catalog.GetRecentSearches().ToArray(),

                SelectedCategory = category,
                SelectedDate = selectedDate,

                SortOptions = s_sortOptions,

                SelectedSortOrder = selectedSort,

                HasSearched = hasFilters
            };

            return View(viewModel);
        }

        /* Helper method that applies sorting logic to a collection of events.
           Uses a switch expression to choose the correct sort order. */
        private static IEnumerable<LocalEvent> ApplySort(IEnumerable<LocalEvent> events, string sortOrder)
        {
            return sortOrder switch
            {
                LocalEventsViewModel.SortOption.Category => events
                    .OrderBy(e => e.Category, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(e => e.StartDate),

                LocalEventsViewModel.SortOption.Name => events
                    .OrderBy(e => e.Title, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(e => e.StartDate),

                _ => events
                    .OrderBy(e => e.StartDate)
                    .ThenBy(e => e.Title, StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
