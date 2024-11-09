using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubmitClaim.Models;

namespace SubmitClaim.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DBsets
        public DbSet<LecturerClaim> LecturerClaims { get; set; }

    }

    public class ApplicationUser : IdentityUser
    {
        // Additional properties
        public string FullName { get; set; }
    }
}