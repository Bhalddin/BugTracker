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
    [Authorize(Roles="Administrator,Developer")]
    public class CommentsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        #region Index
        // GET: Comments
        public async Task<ActionResult> Index(int? id)
        {
            //check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.TicketID = id;
            ViewBag.TicketTitle = db.Tickets.Find(id).Title;
            var comments = db.Comments.Include(c => c.Ticket).Include(c => c.User).Where(c=>c.TicketID == id).OrderBy(c=>c.ID);

            if (ControllerContext.IsChildAction)
            {
                return PartialView("_Index", comments);
            }

            return View(await comments.ToListAsync());
        }
        #endregion

        #region Details
        // GET: Comments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = await db.Comments.FindAsync(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }
        #endregion

        #region Create
        // GET: Comments/Create
        public ActionResult Create(int? id)
        {
            //check your inputs
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var username = HttpContext.User.Identity.Name;

            ViewBag.TicketID = new SelectList(db.Tickets.Where(t=>t.ID==id), "ID", "Title", id);
            ViewBag.CommentorID = new SelectList(db.Users.Where(u => u.ASPUserName == username), "ID", "ASPUserName");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TicketID,CommentorID,CommentDate,Comment1")] Comment comment)
        {
            // set commentDate b/c it's not in the view.
            comment.CommentDate = DateTime.UtcNow;

            // no commentViewModel so need to check the modelState here.
            // check ticketID
            // check commenterID
            // check comment
            if ( comment.TicketID == 0 || comment.CommentorID == 0 || comment.Comment1 == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { id = comment.TicketID });
            }

            ViewBag.TicketID = comment.TicketID;
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", comment.TicketID);
            ViewBag.CommentorID = new SelectList(db.Users, "ID", "FirstName", comment.CommentorID);
            return View(comment);
        }
        #endregion

        #region Edit
        // GET: Comments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", comment.TicketID);
            ViewBag.CommentorID = new SelectList(db.Users, "ID", "FirstName", comment.CommentorID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TicketID,CommentorID,CommentDate,Comment1")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", comment.TicketID);
            ViewBag.CommentorID = new SelectList(db.Users, "ID", "FirstName", comment.CommentorID);
            return View(comment);
        }
        #endregion

        #region Delete
        // GET: Comments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Comment comment = await db.Comments.FindAsync(id);
            db.Comments.Remove(comment);
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
