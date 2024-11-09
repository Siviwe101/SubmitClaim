using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SubmitClaim.Models;

public class LecturerClaim
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Range(1, 100, ErrorMessage = "Hours worked must be between 1 and 100.")]
    public double HoursWorked { get; set; }
    
    [Required]
    [Range(10, 1000, ErrorMessage = "Hourly rate must be between 10 and 1000.")]
    public double HourlyRate { get; set; }
    
    public string AdditionalNotes { get; set; }
    public DateTime SubmissionDate { get; set; }
    [DefaultValue("Pending")]  // Set default value to "Pending"
    public string Status { get; set; } = "Pending"; // Default status value --- "Pending", "Approved", "Rejected"
    public string? FilePath { get; set; } // For storing file path
    public decimal FinalPayment => (decimal)(HoursWorked * HourlyRate);
}