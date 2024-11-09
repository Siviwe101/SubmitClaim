using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubmitClaim.Data;
using SubmitClaim.Models;

namespace SubmitClaim.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Claims
        public async Task<IActionResult> Index()
        {
            return View(await _context.LecturerClaims.ToListAsync());
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await _context.LecturerClaims
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

            return View(lecturerClaim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status,FilePath,UserId")] LecturerClaim lecturerClaim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lecturerClaim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lecturerClaim);
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }
            return View(lecturerClaim);
        }

        // POST: Claims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,HoursWorked,HourlyRate,AdditionalNotes,SubmissionDate,Status,FilePath,UserId")] LecturerClaim lecturerClaim)
        {
            if (id != lecturerClaim.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lecturerClaim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LecturerClaimExists(lecturerClaim.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lecturerClaim);
        }

        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecturerClaim = await _context.LecturerClaims
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecturerClaim == null)
            {
                return NotFound();
            }

            return View(lecturerClaim);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lecturerClaim = await _context.LecturerClaims.FindAsync(id);
            if (lecturerClaim != null)
            {
                _context.LecturerClaims.Remove(lecturerClaim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LecturerClaimExists(string id)
        {
            return _context.LecturerClaims.Any(e => e.Id == id);
        }
    }
}
