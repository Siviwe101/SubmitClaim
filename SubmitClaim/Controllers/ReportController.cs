using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubmitClaim.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;

namespace SubmitClaim.Controllers
{
    public class ReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        // GET: Report/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Report/GeneratePdfReport for all approved claims
        [HttpPost]
        public async Task<IActionResult> GeneratePdfReport()
        {
            // Fetch all approved claims
            var approvedClaims = await context.LecturerClaims
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            // Initialize a MemoryStream for the PDF document
            using var stream = new MemoryStream();

            // Create a new PDF document and write to the stream
            var pdfDoc = new Document();
            var writer = PdfWriter.GetInstance(pdfDoc, stream);
            writer.CloseStream = false; // Prevent iTextSharp from closing the MemoryStream

            pdfDoc.Open();
            pdfDoc.Add(new Paragraph("Approved Claims Report"));
            pdfDoc.Add(new Paragraph(" "));

            foreach (var claim in approvedClaims)
            {
                pdfDoc.Add(new Paragraph($"Lecturer: {claim.UserId} | " +
                                         $"Claim Amount: {claim.HoursWorked * claim.HourlyRate:C} | " +
                                         $"Date: {claim.SubmissionDate}"));
            }

            pdfDoc.Close();

            // Convert the PDF stream to a Base64 string
            stream.Position = 0;
            var pdfBase64 = Convert.ToBase64String(stream.ToArray());

            // Pass the Base64 string to the view
            ViewBag.PdfBase64 = pdfBase64;

            return View("Index");
        }
    }
}
