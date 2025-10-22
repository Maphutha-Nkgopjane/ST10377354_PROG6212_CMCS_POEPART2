using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public LecturerController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimDocument model, IFormFile? evidence)
        {
            if (!ModelState.IsValid) return View(model);

            if (evidence != null && evidence.Length > 0)
            {
                var ext = Path.GetExtension(evidence.FileName).ToLowerInvariant();
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Evidence", "Only PDF, DOCX, and XLSX files are allowed.");
                    return View(model);
                }

                if (evidence.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("Evidence", "File size must be less than 5 MB.");
                    return View(model);
                }

                var dir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(dir);

                var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(dir, uniqueFileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await evidence.CopyToAsync(stream);
                }

                model.EvidenceFileName = uniqueFileName;
            }

            model.Status = "Pending";
            model.PaymentStatus = "Unpaid";
            model.ReviewNote = null;
            model.CreatedAt = DateTime.UtcNow;

            _db.ClaimDocuments.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyLogs), new { mentorName = model.MentorName });
        }

        [HttpGet]
        public async Task<IActionResult> MyLogs(string? mentorName, string? status)
        {
            var query = _db.ClaimDocuments.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(mentorName))
                query = query.Where(x => EF.Functions.Like(x.MentorName, $"%{mentorName.Trim()}%"));

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();

            ViewBag.MentorName = mentorName ?? "";
            ViewBag.Status = status ?? "All";
            ViewBag.Statuses = new[] { "All", "Pending", "NeedsFix", "CoordinatorApproved", "ManagerApproved", "Rejected" };

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var row = await _db.ClaimDocuments.FindAsync(id);
            if (row == null) return NotFound();

            if (row.Status != "NeedsFix")
            {
                TempData["Message"] = "Only logs with status 'NeedsFix' can be edited.";
                return RedirectToAction(nameof(MyLogs), new { mentorName = row.MentorName });
            }

            return View(row);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClaimDocument input, IFormFile? evidence)
        {
            var row = await _db.ClaimDocuments.FindAsync(id);
            if (row == null) return NotFound();

            if (row.Status != "NeedsFix")
            {
                TempData["Message"] = "Only logs marked as 'NeedsFix' can be edited.";
                return RedirectToAction(nameof(MyLogs), new { mentorName = row.MentorName });
            }

            if (!ModelState.IsValid) return View(row);

            row.HoursWorked = input.HoursWorked;
            row.HourlyRate = input.HourlyRate;
            row.Notes = input.Notes;

            if (evidence != null && evidence.Length > 0)
            {
                var ext = Path.GetExtension(evidence.FileName).ToLowerInvariant();
                var allowed = new[] { ".pdf", ".docx", ".xlsx" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Evidence", "Only PDF, DOCX, and XLSX files are allowed.");
                    return View(row);
                }

                if (evidence.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("Evidence", "File size must be less than 5 MB.");
                    return View(row);
                }

                var dir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(dir);

                var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
                using (var stream = System.IO.File.Create(Path.Combine(dir, uniqueFileName)))
                {
                    await evidence.CopyToAsync(stream);
                }

                row.EvidenceFileName = uniqueFileName;
            }

            row.Status = "Pending";
            row.ReviewNote = null;
            row.PaymentStatus = "Unpaid";
            row.PaymentReference = null;
            row.PaidAtUtc = null;

            await _db.SaveChangesAsync();

            TempData["Message"] = "Log updated and resubmitted.";
            return RedirectToAction(nameof(MyLogs), new { mentorName = row.MentorName });
        }
    }
}
