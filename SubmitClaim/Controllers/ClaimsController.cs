using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubmitClaim.Data;

namespace SubmitClaim.Controllers;

public class ClaimsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClaimsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Other methods...
}