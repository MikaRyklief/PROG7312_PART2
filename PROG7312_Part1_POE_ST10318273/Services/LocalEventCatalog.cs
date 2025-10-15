using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PROG7312_Part1_POE_ST10318273.Models;

namespace PROG7312_Part1_POE_ST10318273.Services
{
    /// <summary>
    /// In-memory catalogue that organises local events with a mix of advanced
    /// data structures.  The catalogue is initialised with seed data but exposes
    /// methods that could be used to hydrate from persistent storage in future
    /// iterations of the project.
    /// </summary>
    public class LocalEventCatalog
    {
        private readonly SortedDictionary<DateOnly, List<LocalEvent>> _eventsByDate
            = new();

        private readonly SortedDictionary<string, SortedSet<LocalEvent>> _eventsByCategory
            = new(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _categorySet
            = new(StringComparer.OrdinalIgnoreCase);

        private readonly Queue<EventSearchRecord> _recentSearches
            = new();

        private readonly Stack<LocalEvent> _announcementStack
            = new();

        private readonly Dictionary<string, int> _searchFrequency
            = new(StringComparer.OrdinalIgnoreCase);

        private readonly LocalEventStartDateComparer _eventComparer
            = new();

        public LocalEventCatalog()
        {
            SeedSampleEvents();
        }

        /// <summary>
        /// Returns a chronologically ordered enumeration of upcoming events and
        /// announcements.
        /// </summary>
        public IEnumerable<LocalEvent> GetUpcomingEvents()
        {
            foreach (var bucket in _eventsByDate)
            {
                foreach (var localEvent in bucket.Value)
                {
                    yield return localEvent;
                }
            }
        }

        /// <summary>
        /// Provides the list of announcements maintained in a stack so that the
        /// most recent priority item is surfaced first on the UI.
        /// </summary>
        public IReadOnlyCollection<LocalEvent> GetAnnouncements()
            => _announcementStack.ToList();

        /// <summary>
        /// Returns the unique set of available categories as a sorted list for
        /// drop-down population.
        /// </summary>
        public IReadOnlyCollection<string> GetCategories()
            => _categorySet.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToArray();

        /// <summary>
        /// Exposes the queue of recent search records, reversed so that the
        /// latest search appears first when rendered.
        /// </summary>
        public IReadOnlyCollection<EventSearchRecord> GetRecentSearches()
            => _recentSearches.Reverse().ToArray();

        /// <summary>
        /// Attempts to find matching events using the advanced data structures in
        /// this catalogue.  The method records the search in the queue, uses sets
        /// to intersect category/date filters, and updates search frequency data
        /// used by the recommendation engine.
        /// </summary>
        public IReadOnlyCollection<LocalEvent> Search(string? category, DateOnly? date)
        {
            RecordSearch(category, date);

            var resultSet = new HashSet<LocalEvent>(_eventComparer);
            var hasCategoryFilter = !string.IsNullOrWhiteSpace(category);
            var hasDateFilter = date.HasValue;

            if (!hasCategoryFilter && !hasDateFilter)
            {
                return GetUpcomingEvents().Take(6).ToArray();
            }

            if (hasCategoryFilter &&
                _eventsByCategory.TryGetValue(category!, out var categorySet))
            {
                resultSet.UnionWith(categorySet);
            }

            if (hasDateFilter &&
                _eventsByDate.TryGetValue(date!.Value, out var dateList))
            {
                if (resultSet.Count == 0 && hasCategoryFilter)
                {
                    resultSet.UnionWith(dateList);
                }
                else if (resultSet.Count == 0)
                {
                    resultSet.UnionWith(dateList);
                }
                else
                {
                    resultSet.IntersectWith(dateList);
                }
            }
            else if (hasDateFilter)
            {
                resultSet.Clear();
            }

            return resultSet.OrderBy(e => e.StartDate).ToArray();
        }

        /// <summary>
        /// Builds a list of recommended events using a priority queue that ranks
        /// matches based on category search frequency, proximity of the event date
        /// and tag overlap with recent searches.
        /// </summary>
        public IReadOnlyCollection<LocalEvent> GetRecommendations()
        {
            var recommendations = new List<LocalEvent>();

            if (_searchFrequency.Count == 0)
            {
                return GetUpcomingEvents().Take(3).ToArray();
            }

            var queue = new PriorityQueue<LocalEvent, int>();
            var seen = new HashSet<Guid>();

            foreach (var entry in _searchFrequency)
            {
                if (!_eventsByCategory.TryGetValue(entry.Key, out var candidates))
                {
                    continue;
                }

                foreach (var candidate in candidates)
                {
                    var priority = CalculatePriority(candidate, entry.Value);
                    queue.Enqueue(candidate, -priority); // invert for max-heap behaviour
                }
            }

            while (queue.Count > 0 && recommendations.Count < 3)
            {
                var next = queue.Dequeue();
                if (seen.Add(next.Id))
                {
                    recommendations.Add(next);
                }
            }

            if (recommendations.Count == 0)
            {
                return GetUpcomingEvents().Take(3).ToArray();
            }

            return recommendations;
        }

        private int CalculatePriority(LocalEvent localEvent, int searchFrequency)
        {
            var daysAway = (int)Math.Max(0, (localEvent.StartDate.Date - DateTime.Today).TotalDays);
            var freshnessScore = Math.Max(0, 45 - daysAway); // nearer events receive higher scores

            var tagMatches = 0;
            if (_recentSearches.Count > 0 && localEvent.Tags.Count > 0)
            {
                var lastSearch = _recentSearches.Last();
                if (!string.IsNullOrWhiteSpace(lastSearch.Category) &&
                    localEvent.Tags.Any(tag => tag.Equals(lastSearch.Category, StringComparison.OrdinalIgnoreCase)))
                {
                    tagMatches += 5;
                }

                if (lastSearch.Date.HasValue)
                {
                    var dateTag = lastSearch.Date.Value.ToString("yyyy-MM", CultureInfo.InvariantCulture);
                    if (localEvent.Tags.Any(tag => tag.Equals(dateTag, StringComparison.OrdinalIgnoreCase)))
                    {
                        tagMatches += 3;
                    }
                }
            }

            return (searchFrequency * 15) + freshnessScore + tagMatches;
        }

        private void RecordSearch(string? category, DateOnly? date)
        {
            var record = new EventSearchRecord(category, date);
            _recentSearches.Enqueue(record);

            if (_recentSearches.Count > 8)
            {
                _recentSearches.Dequeue();
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                if (_searchFrequency.TryGetValue(category!, out var existing))
                {
                    _searchFrequency[category!] = existing + 1;
                }
                else
                {
                    _searchFrequency[category!] = 1;
                }
            }
        }

        private void SeedSampleEvents()
        {
            var events = new[]
            {
                new LocalEvent
                {
                    Title = "Community Clean-up Day",
                    Category = "Community",
                    StartDate = DateTime.Today.AddDays(2).AddHours(9),
                    EndDate = DateTime.Today.AddDays(2).AddHours(13),
                    Location = "Greenfield Park",
                    Description = "Join your neighbours to clean public spaces and learn about recycling initiatives.",
                    Tags = new[] { "Community", "Environment", DateTime.Today.AddDays(2).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Water Conservation Workshop",
                    Category = "Utilities",
                    StartDate = DateTime.Today.AddDays(4).AddHours(10),
                    EndDate = DateTime.Today.AddDays(4).AddHours(12),
                    Location = "Municipal Hall",
                    Description = "Interactive session on saving water and understanding current restrictions.",
                    Tags = new[] { "Water", "Utilities", DateTime.Today.AddDays(4).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Youth Career Fair",
                    Category = "Education",
                    StartDate = DateTime.Today.AddDays(6).AddHours(11),
                    Location = "Civic Centre",
                    Description = "Local businesses and tertiary institutions share opportunities for young people.",
                    Tags = new[] { "Education", "Youth", DateTime.Today.AddDays(6).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Road Maintenance Schedule - Ward 12",
                    Category = "Roads",
                    StartDate = DateTime.Today.AddDays(1).AddHours(8),
                    Location = "Ward 12",
                    Description = "Announcement on planned resurfacing causing temporary lane closures.",
                    Tags = new[] { "Roads", "Maintenance", DateTime.Today.AddDays(1).ToString("yyyy-MM", CultureInfo.InvariantCulture) },
                    IsAnnouncement = true
                },
                new LocalEvent
                {
                    Title = "Community Health Screening",
                    Category = "Health",
                    StartDate = DateTime.Today.AddDays(8).AddHours(9),
                    EndDate = DateTime.Today.AddDays(8).AddHours(15),
                    Location = "Newtown Clinic",
                    Description = "Free health screenings including blood pressure, diabetes, and vaccinations.",
                    Tags = new[] { "Health", "Screening", DateTime.Today.AddDays(8).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Library Storytelling Morning",
                    Category = "Culture",
                    StartDate = DateTime.Today.AddDays(3).AddHours(10),
                    Location = "Central Library",
                    Description = "Family-friendly storytelling with local authors.",
                    Tags = new[] { "Culture", "Family", DateTime.Today.AddDays(3).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Waste Collection Update",
                    Category = "Sanitation",
                    StartDate = DateTime.Today.AddDays(5).AddHours(7),
                    Location = "Citywide",
                    Description = "Notice on adjusted waste collection routes due to public holiday.",
                    Tags = new[] { "Sanitation", "Waste", DateTime.Today.AddDays(5).ToString("yyyy-MM", CultureInfo.InvariantCulture) },
                    IsAnnouncement = true
                  },
                new LocalEvent
                {
                    Title = "Farmers' Market Weekend",
                    Category = "Community",
                    StartDate = DateTime.Today.AddDays(9).AddHours(8),
                    EndDate = DateTime.Today.AddDays(9).AddHours(14),
                    Location = "Harbour Square",
                    Description = "Sample locally grown produce, live music and crafts from community vendors.",
                    Tags = new[] { "Community", "Market", DateTime.Today.AddDays(9).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Emergency Preparedness Drill",
                    Category = "Safety",
                    StartDate = DateTime.Today.AddDays(7).AddHours(14),
                    Location = "Disaster Management Centre",
                    Description = "Public demonstration on household emergency plans and evacuation procedures.",
                    Tags = new[] { "Safety", "Emergency", DateTime.Today.AddDays(7).ToString("yyyy-MM", CultureInfo.InvariantCulture) },
                    IsAnnouncement = true
                },
                new LocalEvent
                {
                    Title = "Senior Fitness Morning",
                    Category = "Health",
                    StartDate = DateTime.Today.AddDays(10).AddHours(9),
                    Location = "Waterfront Promenade",
                    Description = "Low-impact exercises led by municipal wellness coaches for residents over 60.",
                    Tags = new[] { "Health", "Wellness", DateTime.Today.AddDays(10).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Art Exhibition Opening Night",
                    Category = "Culture",
                    StartDate = DateTime.Today.AddDays(11).AddHours(18),
                    Location = "Heritage Gallery",
                    Description = "Celebrating local artists with live performances and curator walkabouts.",
                    Tags = new[] { "Culture", "Art", DateTime.Today.AddDays(11).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Entrepreneurship Bootcamp",
                    Category = "Business",
                    StartDate = DateTime.Today.AddDays(12).AddHours(9),
                    EndDate = DateTime.Today.AddDays(12).AddHours(16),
                    Location = "Innovation Hub",
                    Description = "Workshops and mentorship for emerging small business owners.",
                    Tags = new[] { "Business", "Entrepreneurship", DateTime.Today.AddDays(12).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Public Transport Feedback Session",
                    Category = "Transport",
                    StartDate = DateTime.Today.AddDays(3).AddHours(17),
                    Location = "Civic Auditorium",
                    Description = "Share your feedback on bus and rail services with transit planners.",
                    Tags = new[] { "Transport", "Engagement", DateTime.Today.AddDays(3).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Tree Planting Campaign",
                    Category = "Environment",
                    StartDate = DateTime.Today.AddDays(15).AddHours(9),
                    Location = "Riverbend Nature Reserve",
                    Description = "Volunteer to plant indigenous trees and learn about water-wise gardening.",
                    Tags = new[] { "Environment", "Trees", DateTime.Today.AddDays(15).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Holiday Lights Parade",
                    Category = "Culture",
                    StartDate = DateTime.Today.AddDays(20).AddHours(19),
                    Location = "Main Street",
                    Description = "Evening parade featuring community groups, marching bands and festive displays.",
                    Tags = new[] { "Culture", "Festival", DateTime.Today.AddDays(20).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Digital Literacy Class",
                    Category = "Education",
                    StartDate = DateTime.Today.AddDays(5).AddHours(15),
                    Location = "Community ICT Lab",
                    Description = "Beginner course on using municipal online services and staying safe online.",
                    Tags = new[] { "Education", "Technology", DateTime.Today.AddDays(5).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Township Sports Day",
                    Category = "Sports",
                    StartDate = DateTime.Today.AddDays(13).AddHours(8),
                    EndDate = DateTime.Today.AddDays(13).AddHours(17),
                    Location = "Umlazi Sports Complex",
                    Description = "Soccer, netball and athletics tournaments supporting youth development programmes.",
                    Tags = new[] { "Sports", "Youth", DateTime.Today.AddDays(13).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Housing Subsidy Information Session",
                    Category = "Housing",
                    StartDate = DateTime.Today.AddDays(4).AddHours(18),
                    Location = "Ward 5 Multi-purpose Centre",
                    Description = "Learn about qualifying criteria and application timelines for municipal housing support.",
                    Tags = new[] { "Housing", "Support", DateTime.Today.AddDays(4).ToString("yyyy-MM", CultureInfo.InvariantCulture) }
                },
                new LocalEvent
                {
                    Title = "Water Service Interruption Notice",
                    Category = "Utilities",
                    StartDate = DateTime.Today.AddDays(1).AddHours(6),
                    Location = "Northdale",
                    Description = "Urgent maintenance on the main supply line will cause temporary outages in select suburbs.",
                    Tags = new[] { "Utilities", "Maintenance", DateTime.Today.AddDays(1).ToString("yyyy-MM", CultureInfo.InvariantCulture) },
                    IsAnnouncement = true
                }
            };

            foreach (var localEvent in events)
            {
                AddEvent(localEvent);
            }
        }

        private void AddEvent(LocalEvent localEvent)
        {
            var dateKey = DateOnly.FromDateTime(localEvent.StartDate);

            if (!_eventsByDate.TryGetValue(dateKey, out var dateList))
            {
                dateList = new List<LocalEvent>();
                _eventsByDate[dateKey] = dateList;
            }

            dateList.Add(localEvent);
            dateList.Sort(_eventComparer);

            if (!_eventsByCategory.TryGetValue(localEvent.Category, out var categorySet))
            {
                categorySet = new SortedSet<LocalEvent>(_eventComparer);
                _eventsByCategory[localEvent.Category] = categorySet;
            }

            categorySet.Add(localEvent);
            _categorySet.Add(localEvent.Category);

            if (localEvent.IsAnnouncement)
            {
                _announcementStack.Push(localEvent);
            }
        }

        private sealed class LocalEventStartDateComparer : IComparer<LocalEvent>, IEqualityComparer<LocalEvent>
        {
            public int Compare(LocalEvent? x, LocalEvent? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x is null)
                {
                    return -1;
                }

                if (y is null)
                {
                    return 1;
                }

                var compare = DateTime.Compare(x.StartDate, y.StartDate);
                if (compare != 0)
                {
                    return compare;
                }

                return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
            }

            public bool Equals(LocalEvent? x, LocalEvent? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.Id == y.Id;
            }

            public int GetHashCode(LocalEvent obj)
                => obj.Id.GetHashCode();
        }
    }

    /// <summary>
    /// Immutable record describing a single search interaction.  Instances are
    /// stored in a queue to maintain history.
    /// </summary>
    public record struct EventSearchRecord(string? Category, DateOnly? Date);
}