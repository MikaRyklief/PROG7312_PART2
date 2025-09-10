using PROG7312_Part1_POE_ST10318273.Data;
using PROG7312_Part1_POE_ST10318273.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PROG7312_Part1_POE_ST10318273.Data
{
    /// <summary>
    /// Simple node class for the custom linked list implementation.
    /// Each node contains an Issue object and a reference to the next node.
    /// This demonstrates the fundamental structure of a linked list data structure.
    /// </summary>
    internal class Node
    {
        /// <summary>
        /// The Issue object stored in this node.
        /// </summary>
        public Issue Value { get; set; }
        
        /// <summary>
        /// Reference to the next node in the linked list.
        /// Null if this is the last node in the list.
        /// </summary>
        public Node Next { get; set; }
        
        /// <summary>
        /// Initializes a new node with the specified issue value.
        /// </summary>
        /// <param name="value">The issue to store in this node</param>
        public Node(Issue value) => Value = value;
    }

    /// <summary>
    /// Repository implementation using a custom linked list for data storage.
    /// This class demonstrates the use of linked lists and queues as data structures
    /// for managing municipal issue reports. It provides persistence through JSON serialization.
    /// </summary>
    public class LinkedListIssueRepository : IIssueRepository
    {
        // Head node of the linked list - entry point for traversal
        private Node head;
        
        // Counter to track the number of issues in the repository
        private int count = 0;
        
        // File path for persisting data to disk
        private readonly string persistenceFile;

        /// <summary>
        /// Queue for storing recent engagement messages (FIFO data structure).
        /// This demonstrates the use of Queue<T> for managing chronological message history.
        /// Messages are automatically dequeued when the limit is exceeded.
        /// </summary>
        public Queue<string> RecentEngagementMessages { get; } = new Queue<string>();

        /// <summary>
        /// Initializes a new instance of the LinkedListIssueRepository.
        /// </summary>
        /// <param name="persistenceFilePath">Path to the JSON file for data persistence</param>
        public LinkedListIssueRepository(string persistenceFilePath)
        {
            persistenceFile = persistenceFilePath;
        }

        /// <summary>
        /// Adds a new issue to the linked list repository.
        /// This method demonstrates linked list traversal and node insertion at the tail.
        /// </summary>
        /// <param name="issue">The issue to be added to the repository</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public Task AddAsync(Issue issue)
        {
            // Create a new node for the issue
            var node = new Node(issue);
            
            // If the list is empty, make this node the head
            if (head == null)
                head = node;
            else
            {
                // Traverse to the end of the list to append the new node
                // This demonstrates linked list traversal
                var cur = head;
                while (cur.Next != null) cur = cur.Next;
                cur.Next = node;
            }
            
            // Increment the counter
            count++;
            
            // Add engagement message to the queue for user feedback
            EnqueueEngagementMessage($"Thanks! Your report ({issue.Category}) was received.");
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds a message to the engagement queue and maintains size limit.
        /// This method demonstrates queue operations (enqueue/dequeue) and size management.
        /// </summary>
        /// <param name="msg">The message to add to the queue</param>
        private void EnqueueEngagementMessage(string msg)
        {
            // Add message to the end of the queue
            RecentEngagementMessages.Enqueue(msg);
            
            // Maintain queue size limit by removing oldest messages
            if (RecentEngagementMessages.Count > 5) 
                RecentEngagementMessages.Dequeue();
        }

        /// <summary>
        /// Retrieves a specific issue by its unique identifier.
        /// This method demonstrates linear search through a linked list.
        /// </summary>
        /// <param name="id">The unique identifier of the issue to find</param>
        /// <returns>Task containing the issue if found, null otherwise</returns>
        public Task<Issue> GetAsync(Guid id)
        {
            // Start traversal from the head node
            var cur = head;
            
            // Linear search through the linked list
            while (cur != null)
            {
                // Check if current node contains the target issue
                if (cur.Value.Id == id) 
                    return Task.FromResult(cur.Value);
                
                // Move to the next node
                cur = cur.Next;
            }
            
            // Return null if issue not found
            return Task.FromResult<Issue>(null);
        }

        /// <summary>
        /// Retrieves all issues from the repository.
        /// This method demonstrates linked list traversal and collection building.
        /// </summary>
        /// <returns>Task containing a collection of all issues</returns>
        public Task<IEnumerable<Issue>> GetAllAsync()
        {
            // Create a temporary list for enumeration (not used for storage)
            var list = new List<Issue>();
            
            // Traverse the entire linked list
            var cur = head;
            while (cur != null)
            {
                // Add each issue to the temporary list
                list.Add(cur.Value);
                cur = cur.Next;
            }
            
            return Task.FromResult<IEnumerable<Issue>>(list);
        }

        /// <summary>
        /// Gets the total count of issues in the repository.
        /// This is used for progress tracking and statistics.
        /// </summary>
        /// <returns>Task containing the count of issues</returns>
        public Task<int> CountAsync() => Task.FromResult(count);

        /// <summary>
        /// Persists all repository data to disk as JSON.
        /// This method demonstrates file I/O operations and JSON serialization.
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task SaveToDiskAsync()
        {
            // Get all issues from the repository
            var all = await GetAllAsync();
            
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            
            // Serialize to JSON with pretty printing
            var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
            
            // Write to file asynchronously
            await File.WriteAllTextAsync(persistenceFile, json);
        }

        /// <summary>
        /// Loads persisted data from disk and rebuilds the linked list.
        /// This method demonstrates JSON deserialization and data restoration.
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task LoadFromDiskAsync()
        {
            // Check if the persistence file exists
            if (!File.Exists(persistenceFile)) return;
            
            // Read JSON content from file
            var json = await File.ReadAllTextAsync(persistenceFile);
            
            // Deserialize JSON back to a list of issues
            var issues = JsonSerializer.Deserialize<List<Issue>>(json);
            
            // Reset repository state
            head = null;
            count = 0;
            
            // If no issues were loaded, return early
            if (issues == null) return;
            
            // Rebuild the linked list by adding each issue
            foreach (var issue in issues)
            {
                await AddAsync(issue);
            }
        }
    }
}