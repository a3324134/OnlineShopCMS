using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShopCMS.Areas.Identity.Data;
using OnlineShopCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopCMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<OnlineShopUser> _userManager;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<OnlineShopUser> userManager)
        {
            this._roleManager = roleManager; // 宣告roleManager
            this._userManager = userManager;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(OnlineShopUserRole role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role.RoleName); //判斷角色是否已存在
            if (!roleExist)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.users = await _userManager.GetUsersInRoleAsync(role.Name);
            }
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(IdentityRole role)
        {
            if (role == null)
            {
                return NotFound();
            }
            else
            {
                var reviseName = role.Name;
                role = await _roleManager.FindByIdAsync(role.Id);
                role.Name = reviseName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ListRoles");
        }

        public async Task<IActionResult> ListUsers()
        {
            List<OnlineShopUserViewModel> userViewModels = new List<OnlineShopUserViewModel>();
            var AllUsers = _userManager.Users.ToList();
            foreach (var user in AllUsers)
            {
                userViewModels.Add(new OnlineShopUserViewModel
                {
                    User = user,
                    RoleName = string.Join("", await _userManager.GetRolesAsync(user))
                });
            }

            return View(userViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ListUsers");
        }
    }
}
