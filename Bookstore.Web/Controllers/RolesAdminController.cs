using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BabyStore.Web.Models;
using BabyStore.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BabyStore.Web.Controllers
{
    [Authorize(Roles = "admin")]public class RolesAdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public RolesAdminController()
        {
            
        }

        public RolesAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set { _roleManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                
            }
            private set { _userManager = value; }
        }

        // GET: RolesAdmin
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        // GET: RolesAdmin/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);

            var users = new List<ApplicationUser>();

            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        // GET: RolesAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesAdmin/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new IdentityRole(viewModel.Name);
                    var roleResult = await RoleManager.CreateAsync(role);

                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RolesAdmin/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return  new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
                    
            };

            return View(viewModel);
        }

        // POST: RolesAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleViewModel viewModel)
        {
           
                if (ModelState.IsValid)
                {
                    var role = await RoleManager.FindByIdAsync(viewModel.Id);
                    role.Name = viewModel.Name;
                    await RoleManager.UpdateAsync(role);
                    return RedirectToAction("Index");
                }
           
                return View();
        }

        // GET: RolesAdmin/Delete/5
        public async Task<ActionResult> Delete(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: RolesAdmin/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
