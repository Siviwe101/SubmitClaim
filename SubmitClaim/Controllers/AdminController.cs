using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubmitClaim.Models;
using System.Threading.Tasks;

namespace SubmitClaim.Controllers
{
    public class AdminController(UserManager<IdentityUser> userManager) : Controller
    {
        // GET: Admin/Users
        public IActionResult Index()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }

        // GET: Admin/EditUser/id
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/EditUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id,UserName,Email,PhoneNumber,FullName")] IdentityUser model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                // Update user details
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded) return RedirectToAction(nameof(Index));

                // Display errors if any
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}
