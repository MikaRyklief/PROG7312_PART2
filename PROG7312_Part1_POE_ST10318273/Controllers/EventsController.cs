using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Models;
using PROG7312_Part1_POE_ST10318273.Services;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    public class EventsController : Controller
    {
        private readonly LocalEventCatalog _catalog;

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
            var selectedDate = date.HasValue
                ? DateOnly.FromDateTime(date.Value)
                : (DateOnly?)null;

            var selectedSort = LocalEventsViewModel.SortOption.Date;
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                var matchedOption = s_sortOptions.FirstOrDefault(option =>
                    string.Equals(option.Value, sortOrder, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(matchedOption.Value))
                {
                    selectedSort = matchedOption.Value;
                }
            }

            var hasFilters = !string.IsNullOrWhiteSpace(category) || selectedDate.HasValue;

            var searchResults = hasFilters
                ? ApplySort(_catalog.Search(category, selectedDate), selectedSort).ToArray()
                : Array.Empty<LocalEvent>();

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