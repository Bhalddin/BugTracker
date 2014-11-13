using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProjectsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        #region Index
        // GET: Projects
        public async Task<ActionResult> Index()
        {
            return View(await db.Projects.ToListAsync());
        }
        #endregion

        #region Details
        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        #endregion

        #region Create
        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProjectName,ProjectDescription")] ProjectCreateViewModel projectCreateVM)
        {
            if (ModelState.IsValid)
            {
                Project proj = new Project
                {
                    ProjectDescription = projectCreateVM.ProjectDescription,
                    ProjectName = projectCreateVM.ProjectName
                };

                db.Projects.Add(proj);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(projectCreateVM);
        }
        #endregion

        #region Edit
        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,ProjectName,ProjectDescription")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        #endregion

        #region Add User
        // GET:  /Projects/AddUsers
        public ActionResult AddUsers(int projectID)
        {
            // check your inputs
            if (projectID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // We are ADDING users NOT already in the project, so find user not in project.
            //var usersNotInProj = db.Users
            //                    .Include(u => u.Projects)
            //                    .Where(u => !u.Projects.Any(p => p.ID == projectID));

            // much cleaner ways of selecting users in a project.
            var usersInProj = db.Projects.Find(projectID).Users.Select(u => u.ID);
            var usersNotInProj = db.Users.Where(u => !usersInProj.Contains(u.ID));

            // build VM
            ProjectUserViewModel model = new ProjectUserViewModel
            {
                ID = projectID,
                ProjectName = db.Projects.Find(projectID).ProjectName,
                Users = new MultiSelectList(usersNotInProj, "ID", "ASPUserName")
            };

            // return list.
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUsers([Bind(Include = "ID,SelectedUsers")] ProjectUserViewModel projectUserVM)
        {
            // check your inputs
            if (ModelState.IsValid)
            {
                // add each selected user to the project
                //var proj = db.Projects.Where(p => p.ID == projectVM.ID).ToList();
                var proj = new Project { ID = projectUserVM.ID };
                db.Projects.Attach(proj);

                foreach (int userName in projectUserVM.SelectedUsers)
                {
                    // get User, add current project, save changes.
                    var user = db.Users.Include(u => u.Projects).Single(u => u.ID == userName);

                    // add the project to the user.
                    user.Projects.Add(proj);

                    // save changes
                    db.SaveChanges();
                }

                // return to Project Index
                return RedirectToAction("Index");
            }

            // ViewModel was messed up, need to return with error message.
            return View("Error");
        }
        #endregion

        #region Remove User
        // GET:  /Projects/RemoveUsers
        public ActionResult RemoveUsers(int projectID)
        {
            // check your inputs
            if (projectID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // We are REMOVING users ALREADY in the project, so find user in project.
            //var userInProj = db.Users
            //                    .Include(u => u.Projects)
            //                    .Where(u => u.Projects.Any(p => p.ID == projectID));

            // much cleaner ways of selecting users in a project.
            var usersInProj = db.Projects.Find(projectID).Users;

            // build VM
            ProjectUserViewModel model = new ProjectUserViewModel
            {
                ID = projectID,
                ProjectName = db.Projects.Find(projectID).ProjectName,
                Users = new MultiSelectList(usersInProj, "ID", "ASPUserName")
            };

            // return list.
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUsers([Bind(Include = "ID,SelectedUsers")] ProjectUserViewModel projectUserVM)
        {
            // check your inputs
            if (ModelState.IsValid)
            {
                // add each selected user to the project
                //var proj = db.Projects.Where(p => p.ID == projectVM.ID).ToList();
                var proj = new Project { ID = projectUserVM.ID };
                db.Projects.Attach(proj);

                foreach (int userName in projectUserVM.SelectedUsers)
                {
                    // get User, add current project, save changes.
                    var user = db.Users.Include(u => u.Projects).Single(u => u.ID == userName);

                    // add the project to the user.
                    user.Projects.Remove(proj);

                    // save changes
                    db.SaveChanges();
                }

                // return to Project Index
                return RedirectToAction("Index");
            }
            // ViewModel was messed up, need to return with error message.
            return View("Error");
        }
        #endregion

        #region Delete
        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
