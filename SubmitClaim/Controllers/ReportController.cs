using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SubmitClaim.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SubmitClaim.Controllers
{
    [Authorize] // Restrict access to only Admin users
    public class ReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;

        // GET: Report/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Report/GeneratePdfReport for all approved claims
        [HttpPost]
        public async Task<IActionResult> GeneratePdfReport()
        {
            try
            {
                // Fetch all approved claims
                var approvedClaims = await context.LecturerClaims
                    .Where(c => c.Status == "Approved")
                    .ToListAsync();

                if (!approvedClaims.Any())
                {
                    ViewBag.Message = "No approved claims available for the report.";
                    return View("Index");
                }

                // Initialize a MemoryStream for the PDF document
                using var stream = new MemoryStream();
                var pdfDoc = new Document();
                var writer = PdfWriter.GetInstance(pdfDoc, stream);
                writer.CloseStream = false;

                // Generate PDF
                pdfDoc.Open();
                pdfDoc.Add(new Paragraph("Approved Claims Report"));
                pdfDoc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                pdfDoc.Add(new Paragraph(" "));

                foreach (var claim in approvedClaims)
                {
                    pdfDoc.Add(new Paragraph($"Lecturer: {claim.UserId} | " +
                                             $"Claim Amount: {claim.HoursWorked * claim.HourlyRate:C} | " +
                                             $"Submission Date: {claim.SubmissionDate}"));
                }

                pdfDoc.Close();

                // Return the PDF as a file
                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "ApprovedClaimsReport.pdf");
            }
            catch (Exception ex)
            {
                // Log the error and return to the view
                ViewBag.Message = "An error occurred while generating the report: " + ex.Message;
                return View("Index");
            }
        }
    }
}
