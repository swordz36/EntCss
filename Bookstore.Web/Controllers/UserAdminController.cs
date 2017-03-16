using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;

namespace BabyStore.Web.Controllers
{

    [Authorize(Roles = "admin")]
    public class UserAdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManger;

        public ApplicationUserManager UserManger {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager { get {
                return _roleManger ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set {
                _roleManger = value;
            } }

        public UserAdminController()
        {

        }

        public UserAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManger = userManager;
            RoleManager = roleManager;
       
        }
        // GET: UserAdmin
        public async Task<ActionResult> Index()
        {
            return View(await UserManger.Users.ToListAsync());
        }

        // GET: UserAdmin/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManger.FindByIdAsync(id);
            ViewBag.RoleNames = await UserManger.GetRolesAsync(user.Id);
           

            return View(user);
        }

        // GET: UserAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserAdmin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserAdmin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserAdmin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
