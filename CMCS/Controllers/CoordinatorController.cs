using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CMCS.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CoordinatorController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Dashboard(string? mentorName)
        {
            var q = _db.ClaimDocuments
                .AsNoTracking()
                .Where(x => x.Status == "Pending");

            if (!string.IsNullOrWhiteSpace(mentorName))
            {
                var term = mentorName.Trim();
                q = q.Where(x => EF.Functions.Like(x.MentorName, $"%{term}%"));
            }

            var list = await q.OrderBy(x => x.CreatedAt).ToListAsync();

            ViewBag.MentorName = mentorName ?? "";
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var row = await _db.ClaimDocuments.FindAsync(id); 

            if (row == null) return NotFound();

            row.Status = "CoordinatorApproved";
            row.ReviewNote = null;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var row = await _db.ClaimDocuments.FindAsync(id); 

            if (row == null) return NotFound();

            row.Status = "NeedsFix";
            row.ReviewNote = string.IsNullOrWhiteSpace(reason)
                ? "Please fix and resubmit"
                : reason.Trim();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }
    }
}
