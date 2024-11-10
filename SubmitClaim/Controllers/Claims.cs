using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubmitClaim.Data;
using SubmitClaim.Models;

namespace SubmitClaim.Controllers
{
    [Authorize]
    public class ClaimsController(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        ILogger<ClaimsController> logger)
        : Controller
    {
        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClaims = await context.LecturerClaims
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(userClaims);
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var lecturerClaim = await context.LecturerClaims
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (lecturerClaim == null) return NotFound();

            return View(lecturerClaim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")] LecturerClaim lecturerClaim,
            IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (uploadedFile != null && uploadedFile.Length > 0)
                    {
                        if (!IsValidFile(uploadedFile))
                        {
                            ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
                            return View(lecturerClaim);
                        }

                        // Save file and store file path in the database
                        string uniqueFileName = await SaveUploadedFile(uploadedFile);
                        lecturerClaim.FilePath = uniqueFileName;
                    }

                    lecturerClaim.Id = Guid.NewGuid();
                    lecturerClaim.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Associate claim with logged-in user
                    lecturerClaim.Status = "Pending";  // Default status

                    context.Add(lecturerClaim);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while creating claim");
                    ModelState.AddModelError("", "An error occurred while creating the claim.");
                    return View(lecturerClaim);
                }
            }

            return View(lecturerClaim);
        }

        // Helper method to save the uploaded file and return the filename with extension
        private async Task<string> SaveUploadedFile(IFormFile uploadedFile)
        {
            try
            {
                var uploadsPath = Path.Combine(env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate a unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                string filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving file");
                throw;
            }
        }

        // Helper method to validate the uploaded file type
        private bool IsValidFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
