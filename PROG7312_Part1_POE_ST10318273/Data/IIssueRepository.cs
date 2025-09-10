using PROG7312_Part1_POE_ST10318273.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PROG7312_Part1_POE_ST10318273.Data
{
 
    public interface IIssueRepository
    {
    
        Task AddAsync(Issue issue);
       
        Task<Issue> GetAsync(Guid id);
       
        Task<IEnumerable<Issue>> GetAllAsync();
      
        Task<int> CountAsync();
       
        Task SaveToDiskAsync();   
       
        Task LoadFromDiskAsync(); 
    }
}
