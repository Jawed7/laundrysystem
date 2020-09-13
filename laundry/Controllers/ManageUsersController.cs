using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using laundry.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace laundry.Controllers
{
    public class ManageUsersController : Controller
    {
        private UserManager<laundryUser> userManager;
        private IPasswordHasher<laundryUser> passwordHasher;

        public ManageUsersController(UserManager<laundryUser> usrMgr, IPasswordHasher<laundryUser> passwordHash)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
        }
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            laundryUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(email))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
