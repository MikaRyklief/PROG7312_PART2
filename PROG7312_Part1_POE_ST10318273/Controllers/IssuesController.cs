using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Data;
using PROG7312_Part1_POE_ST10318273.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    public class IssuesController : Controller
    {
        // Repository for data access operations using linked list implementation
        private readonly IIssueRepository _repo;
        
        // Web hosting environment for file upload handling
        private readonly IWebHostEnvironment _env;

        public IssuesController(IIssueRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Report()
        {
            // Define available issue categories for the dropdown
            ViewData["Categories"] = new[] { "Sanitation", "Roads", "Utilities", "Other" };
            
            // Get recent engagement messages from the queue
            ViewData["RecentMessages"] = (_repo as LinkedListIssueRepository)?.RecentEngagementMessages;
            
            // Get current submission count for progress bar calculation
            var count = await _repo.CountAsync();
            ViewData["SubmittedCount"] = count;
            
            return View();
        }

        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> Report(Issue model)
        {
            // Explicit server-side validation to ensure required fields are provided
            if (string.IsNullOrWhiteSpace(model.Location) ||
                string.IsNullOrWhiteSpace(model.Category) ||
                string.IsNullOrWhiteSpace(model.Description))
            {
                TempData["Error"] = "Please complete required fields.";
                return RedirectToAction(nameof(Report));
            }

            // Handle optional reporter name - convert null to empty string
            model.ReporterName = model.ReporterName?.Trim() ?? string.Empty;

            // Process file upload if provided
            var file = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;
            if (file != null && file.Length > 0)
            {
                // Create uploads directory if it doesn't exist
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                
                // Generate unique filename to prevent conflicts
                var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploads, uniqueName);
                
                // Save file to disk asynchronously
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                
                // Store relative path for database reference
                model.AttachmentPath = $"/uploads/{uniqueName}";
            }

            // Award engagement points for gamification 
            model.EngagementPointsAwarded = 10;

            // Add issue to linked list repository
            await _repo.AddAsync(model);
            
            // Persist data to disk (JSON file)
            await _repo.SaveToDiskAsync();

            // Set success message for user feedback
            TempData["Success"] = "Issue submitted successfully, thank you for participating!";
            
            // Redirect to prevent duplicate submissions on refresh
            return RedirectToAction(nameof(Report));
        }
    }
}
