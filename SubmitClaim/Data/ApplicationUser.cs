using Microsoft.AspNetCore.Identity;

namespace SubmitClaim.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } // New property for full name
}