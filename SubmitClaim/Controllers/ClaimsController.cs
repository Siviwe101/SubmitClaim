// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using SubmitClaim.Data;
// using SubmitClaim.Models;
//
// namespace SubmitClaim.Controllers
// {
//     public class ClaimsController(
//         ApplicationDbContext context,
//         IWebHostEnvironment env,
//         ILogger<ClaimsController> logger)
//         : Controller
//     {
//         // GET: Claims
//         public async Task<IActionResult> Index()
//         {
//             var claims = await context.LecturerClaims.ToListAsync();
//             return View(claims);
//         }
//
//         // GET: Claims/Details/5
//         public async Task<IActionResult> Details(string? id)
//         {
//             if (id == null) return NotFound();
//
//             var lecturerClaim = await context.LecturerClaims.FindAsync(id);
//             if (lecturerClaim == null) return NotFound();
//
//             return View(lecturerClaim);
//         }
//
//         // GET: Claims/Create
//         public IActionResult Create()
//         {
//             return View();
//         }
//
//         // POST: Claims/Create
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create(
//             [Bind("UserId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")]
//             LecturerClaim lecturerClaim, IFormFile uploadedFile)
//         {
//             if (!ModelState.IsValid)
//             {
//                 logger.LogDebug("Model validation failed for creating claim: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
//                 return View(lecturerClaim);
//             }
//
//             try
//             {
//                 // Handle file upload
//                 if (uploadedFile != null && uploadedFile.Length > 0)
//                 {
//                     if (!IsValidFile(uploadedFile))
//                     {
//                         ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
//                         return View(lecturerClaim);
//                     }
//
//                     var uniqueFileName = await SaveUploadedFile(uploadedFile);
//                     lecturerClaim.FilePath = uniqueFileName; // Store full file name with extension
//                 }
//
//                 lecturerClaim.Status = "Pending";  // Default status for a new claim
//                 context.Add(lecturerClaim);
//
//                 int result = await context.SaveChangesAsync();
//                 if (result > 0)
//                 {
//                     logger.LogDebug("Claim created successfully with ID: {ClaimId}", lecturerClaim.Id);
//                 }
//                 else
//                 {
//                     logger.LogError("Failed to save claim to the database.");
//                     ModelState.AddModelError("", "An error occurred while saving the claim.");
//                     return View(lecturerClaim);
//                 }
//
//                 return RedirectToAction(nameof(Index));
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Error while creating claim");
//                 ModelState.AddModelError("", "An error occurred while creating the claim.");
//                 return View(lecturerClaim);
//             }
//         }
//
//         // GET: Claims/Edit/5
//         public async Task<IActionResult> Edit(string? id)
//         {
//             if (id == null) return NotFound();
//
//             var lecturerClaim = await context.LecturerClaims.FindAsync(id);
//             if (lecturerClaim == null) return NotFound();
//
//             return View(lecturerClaim);
//         }
//
//         // POST: Claims/Edit/5
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(string id,
//             [Bind("Id,LecturerId,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status")]
//             LecturerClaim lecturerClaim, IFormFile uploadedFile)
//         {
//             if (id != lecturerClaim.Id.ToString()) return NotFound();
//
//             if (!ModelState.IsValid)
//             {
//                 logger.LogDebug("Model validation failed for editing claim with ID: {ClaimId}", id);
//                 return View(lecturerClaim);
//             }
//
//             try
//             {
//                 var existingClaim = await context.LecturerClaims.FindAsync(id);
//                 if (existingClaim == null) return NotFound();
//
//                 // Handle file upload
//                 if (uploadedFile != null && uploadedFile.Length > 0)
//                 {
//                     if (!IsValidFile(uploadedFile))
//                     {
//                         ModelState.AddModelError("", "Invalid file type. Allowed types: .pdf, .docx, .xlsx");
//                         return View(lecturerClaim);
//                     }
//
//                     // Delete old file if exists
//                     if (!string.IsNullOrEmpty(existingClaim.FilePath))
//                     {
//                         DeleteFile(existingClaim.FilePath);
//                     }
//
//                     string uniqueFileName = await SaveUploadedFile(uploadedFile);
//                     existingClaim.FilePath = uniqueFileName;
//                 }
//
//                 // Update other fields
//                 existingClaim.HoursWorked = lecturerClaim.HoursWorked;
//                 existingClaim.HourlyRate = lecturerClaim.HourlyRate;
//                 existingClaim.AdditionalNotes = lecturerClaim.AdditionalNotes;
//                 existingClaim.SubmissionDate = lecturerClaim.SubmissionDate;
//                 existingClaim.Status = lecturerClaim.Status;
//
//                 int result = await context.SaveChangesAsync();
//                 if (result > 0)
//                 {
//                     logger.LogDebug("Claim updated successfully with ID: {ClaimId}", lecturerClaim.Id);
//                 }
//                 else
//                 {
//                     logger.LogError("Failed to update claim in the database.");
//                     ModelState.AddModelError("", "An error occurred while updating the claim.");
//                     return View(lecturerClaim);
//                 }
//
//                 return RedirectToAction(nameof(Index));
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!LecturerClaimExists(lecturerClaim.Id.ToString())) return NotFound();
//                 throw;
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Error while editing claim");
//                 ModelState.AddModelError("", "An error occurred while editing the claim.");
//                 return View(lecturerClaim);
//             }
//         }
//
//         // Helper method to save the uploaded file and return only the filename with its extension
//         private async Task<string> SaveUploadedFile(IFormFile uploadedFile)
//         {
//             try
//             {
//                 var uploadsPath = Path.Combine(env.WebRootPath, "uploads");
//                 if (!Directory.Exists(uploadsPath))
//                 {
//                     Directory.CreateDirectory(uploadsPath);
//                 }
//                 // Generate a unique file name
//                 string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
//                 string filePath = Path.Combine(uploadsPath, uniqueFileName);
//
//                 // Save the file to disk
//                 using (var fileStream = new FileStream(filePath, FileMode.Create))
//                 {
//                     await uploadedFile.CopyToAsync(fileStream);
//                 }
//
//                 return uniqueFileName; // Return the filename with extension
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Error while saving file");
//                 throw;
//             }
//         }
//
//         // Helper method to delete a file from the server
//         private void DeleteFile(string fileName)
//         {
//             try
//             {
//                 var filePath = Path.Combine(env.WebRootPath, "uploads", fileName);
//                 if (System.IO.File.Exists(filePath))
//                 {
//                     System.IO.File.Delete(filePath);
//                     logger.LogDebug("File deleted successfully: {FilePath}", fileName);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "Error while deleting file: {FilePath}", fileName);
//             }
//         }
//
//         // Helper method to validate the uploaded file type
//         private bool IsValidFile(IFormFile file)
//         {
//             var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
//             var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
//             return allowedExtensions.Contains(fileExtension);
//         }
//
//         private bool LecturerClaimExists(string id) => context.LecturerClaims.Any(e => e.Id.ToString() == id);
//     }
// }
