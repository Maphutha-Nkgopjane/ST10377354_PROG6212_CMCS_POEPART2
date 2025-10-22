using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    [Table("ClaimDocuments")]
    public class ClaimDocument
    {
        public int Id { get; set; }

        [Required]
        public string MentorName { get; set; } = "";

        public decimal HoursWorked { get; set; }

        public decimal HourlyRate { get; set; }

        [MaxLength(400)]
        public string? Notes { get; set; }

        public string? EvidenceFileName { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public string? ReviewNote { get; set; }

        [Required]
        public string PaymentStatus { get; set; } = "Unpaid";

        public string? PaymentReference { get; set; }

        public DateTime? PaidAtUtc { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
