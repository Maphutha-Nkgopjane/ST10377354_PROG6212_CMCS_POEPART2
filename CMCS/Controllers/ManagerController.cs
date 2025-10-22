using System;
using System.Linq;
using System.Threading.Tasks;
using CMCS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ManagerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Dashboard(string? mentorName)
        {
            var q = _db.ClaimDocuments
                .AsNoTracking()
                .Where(x => x.Status == "CoordinatorApproved");

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

            row.Status = "ManagerApproved";
            row.PaymentStatus = "Paid";
            row.PaymentReference = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
            row.PaidAtUtc = DateTime.UtcNow;
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
                ? "Manager requested changes, please fix and resubmit"
                : reason.Trim();

            row.PaymentStatus = "Unpaid";
            row.PaymentReference = null;
            row.PaidAtUtc = null;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
