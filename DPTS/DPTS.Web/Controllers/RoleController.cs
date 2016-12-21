using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Web;
using DPTS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace dpts.portal.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    //[RequireHttps]
    public class RoleController : Controller
    {
        ApplicationDbContext context;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        public RoleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                var Roles = context.Roles.ToList();
                //var Roles = new SelectList(context.Roles.ToList().Where(e => e.Name != "SuperAdmin"), "Name", "Name");
                return View(Roles);
            }
            catch (Exception e)
            {
                ////ExceptionHandler.HandleException(e);
                throw;
            }
        }

        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns> 
        public ActionResult Create()
        {
            try
            {
                var Role = new IdentityRole();
                return View(Role);
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            try
            {
                context.Roles.Add(Role);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        /// <summary>
        /// Set Role for Users
        /// </summary>
        /// <returns></returns> 
        public ActionResult SetRoleToUser()
        {
            try
            {
                SetPageData();
                return View();
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        private void SetPageData(bool filter = false)
        {
            try
            {
                ViewBag.Roles =
                    context.Roles.OrderBy(role => role.Name)
                        .ToList()
                        .Select(role => new SelectListItem {Value = role.Name.ToString(), Text = role.Name})
                        .ToList();
                /*if (filter)
                {
                    var db = new TxAirportManagerEntities();
                    ViewBag.Users = from c in db.AspNetRoles
                                    join o in db.AspNetUsers on c.Id equals o.Id
                                    select new SelectListItem { Value = o.UserName.ToString(), Text = o.UserName };
                }
                else*/
                ViewBag.Users =
                    context.Users.OrderBy(role => role.UserName)
                        .ToList()
                        .Select(role => new SelectListItem {Value = role.UserName.ToString(), Text = role.UserName})
                        .ToList();
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAddToRole(string UserName, string rolename)
        {
            try
            {
                ApplicationUser user =
                    context.Users.Where(usr => usr.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();
                SetPageData();
                if (user != null)
                {
                    this.UserManager.AddToRole(user.Id, rolename);
                    ViewBag.ResultMessage = "Role created successfully !";
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = "Sorry user is not available";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        public ActionResult Delete(string RoleName)
        {
            try
            {
                var thisRole =
                    context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();
                context.Roles.Remove(thisRole);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        public ActionResult Edit(string roleName)
        {
            try
            {
                var thisRole =
                    context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();
                return View(thisRole);
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                return View();
            }
        }

        #region Delete Role Of User

        public ActionResult DeleteRoleOfUser()
        {
            try
            {
                SetPageData(true);
                return View();
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleOfUser(string UserName, string RoleName)
        {
            try
            {
                ApplicationUser user =
                    context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();

                if (this.UserManager.IsInRole(user.Id, RoleName))
                {
                    this.UserManager.RemoveFromRole(user.Id, RoleName);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                // prepopulat roles for the view dropdown
                var list =
                    context.Roles.OrderBy(r => r.Name)
                        .ToList()
                        .Select(rr => new SelectListItem {Value = rr.Name.ToString(), Text = rr.Name})
                        .ToList();
                ViewBag.Roles = list;

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                //ExceptionHandler.HandleException(e);
                throw;
            }
        }

        #endregion
    }
}