﻿using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection_WebApp.Models;

namespace PropertyInspection_WebApp.Controllers
{
    public class OperationsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;


        public OperationsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var RoleIDForUser = user.RoleID;
                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                //Adding User to Admin Role
                if (RoleIDForUser == 1)
                {
                    await userManager.AddToRoleAsync(appUser, "SYSTEM");
                }
                else if (RoleIDForUser == 2)
                {
                    await userManager.AddToRoleAsync(appUser, "ADMIN");
                }
                else if (RoleIDForUser == 3)
                {
                    await userManager.AddToRoleAsync(appUser, "INSPECTOR");
                }

                if (result.Succeeded)
                    ViewBag.Message = "User Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            ModelState.Clear();
            return View(user);
        }

        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new ApplicationRole() { Name = name });
                if (result.Succeeded)
                    ViewBag.Message = "Role Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
    }
}

