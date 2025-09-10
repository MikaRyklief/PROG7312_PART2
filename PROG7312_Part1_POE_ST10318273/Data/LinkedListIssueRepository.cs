using PROG7312_Part1_POE_ST10318273.Data;
using PROG7312_Part1_POE_ST10318273.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PROG7312_Part1_POE_ST10318273.Data
{

    internal class Node
    {

        public Issue Value { get; set; }
     
        public Node Next { get; set; }

        public Node(Issue value) => Value = value;
    }

    public class LinkedListIssueRepository : IIssueRepository
    {
        // Head node of the linked list, entry point for traversal
        private Node head;
        
        // Counter to track the number of issues in the repository
        private int count = 0;
        
        // File path for persisting data to disk
        private readonly string persistenceFile;

        public Queue<string> RecentEngagementMessages { get; } = new Queue<string>();

        public LinkedListIssueRepository(string persistenceFilePath)
        {
            persistenceFile = persistenceFilePath;
        }

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
                var cur = head;
                while (cur.Next != null) cur = cur.Next;
                cur.Next = node;
            }

            count++;
            
            // Add engagement message to the queue for user feedback
            EnqueueEngagementMessage($"Thanks! Your report ({issue.Category}) was received.");
            
            return Task.CompletedTask;
        }

        private void EnqueueEngagementMessage(string msg)
        {
            // Add message to the end of the queue
            RecentEngagementMessages.Enqueue(msg);
            
            // Maintain queue size limit by removing oldest messages
            if (RecentEngagementMessages.Count > 5) 
                RecentEngagementMessages.Dequeue();
        }

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

        public Task<IEnumerable<Issue>> GetAllAsync()
        {
            // Create a temporary list for enumeration 
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

        public Task<int> CountAsync() => Task.FromResult(count);

        public async Task SaveToDiskAsync()
        {
            // Get all issues from the repository
            var all = await GetAllAsync();
            
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            
            // Serialize to JSON 
            var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
            
            // Write to file asynchronously
            await File.WriteAllTextAsync(persistenceFile, json);
        }

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