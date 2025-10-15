using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Models;
using PROG7312_Part1_POE_ST10318273.Services;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    public class EventsController : Controller
    {
        private readonly LocalEventCatalog _catalog;

        public EventsController(LocalEventCatalog catalog)
        {
            _catalog = catalog;
        }

        [HttpGet]
        public IActionResult Index(string? category, DateTime? date)
        {
            var selectedDate = date.HasValue
                ? DateOnly.FromDateTime(date.Value)
                : (DateOnly?)null;

            var hasFilters = !string.IsNullOrWhiteSpace(category) || selectedDate.HasValue;

            var searchResults = hasFilters
                ? _catalog.Search(category, selectedDate)
                : Array.Empty<LocalEvent>();

            var viewModel = new LocalEventsViewModel
            {
                Categories = _catalog.GetCategories(),
                UpcomingEvents = _catalog.GetUpcomingEvents().ToArray(),
                Announcements = _catalog.GetAnnouncements().ToArray(),
                SearchResults = searchResults,
                Recommendations = _catalog.GetRecommendations().ToArray(),
                RecentSearches = _catalog.GetRecentSearches().ToArray(),
                SelectedCategory = category,
                SelectedDate = selectedDate,
                HasSearched = hasFilters
            };

            return View(viewModel);
        }
    }
}
