using PROG7312_Part1_POE_ST10318273.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROG7312_Part1_POE_ST10318273.Data
{
    /// <summary>
    /// Interface defining the contract for issue data repository operations.
    /// This interface abstracts the data access layer and allows for different
    /// implementations (linked list, database, etc.) to be used interchangeably.
    /// </summary>
    public interface IIssueRepository
    {
        /// <summary>
        /// Adds a new issue to the repository asynchronously.
        /// </summary>
        /// <param name="issue">The issue to be added</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task AddAsync(Issue issue);
        
        /// <summary>
        /// Retrieves a specific issue by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the issue</param>
        /// <returns>Task containing the issue if found, null otherwise</returns>
        Task<Issue> GetAsync(Guid id);
        
        /// <summary>
        /// Retrieves all issues from the repository.
        /// </summary>
        /// <returns>Task containing a collection of all issues</returns>
        Task<IEnumerable<Issue>> GetAllAsync();
        
        /// <summary>
        /// Gets the total count of issues in the repository.
        /// Used for progress tracking and statistics.
        /// </summary>
        /// <returns>Task containing the count of issues</returns>
        Task<int> CountAsync();
        
        /// <summary>
        /// Persists all repository data to disk asynchronously.
        /// This method saves the current state of the repository to a file.
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        Task SaveToDiskAsync();   
        
        /// <summary>
        /// Loads persisted data from disk asynchronously.
        /// This method restores the repository state from a previously saved file.
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        Task LoadFromDiskAsync(); 
    }
}
