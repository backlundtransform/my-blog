using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View(db.Comments.ToList());
        }

        [HttpPost]
        public ActionResult Like(int id, bool like)
        {
            if (like)
            {
                db.Comments.Find(id).Like = db.Comments.Find(id).Like + 1;
                db.SaveChanges();
            }
            return Json(new { Result = db.Comments.Find(id).Like });
        }

        [HttpPost]
        public ActionResult Dislike(int id, bool dislike)
        {
            if (dislike)
            {
                db.Comments.Find(id).Dislike = db.Comments.Find(id).Dislike - 1;
                db.SaveChanges();
            }
            return Json(new { Result = db.Comments.Find(id).Dislike });
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [Authorize]
        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
       
        public ActionResult Upload(HttpPostedFileBase file, int postid)
        {
            


            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";

                    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                    var userManager = new UserManager<ApplicationUser>(store);
                    ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                    user.Avatar = Path.Combine("~/Content/images", Path.GetFileName(file.FileName));


                    userManager.UpdateAsync(user);
                  
                    store.Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return RedirectToAction("Details", "posts", new { id = postid });
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Text")] Comment comment, int PostId, int ParentId)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                comment.User = user.ApplicationUserName(db);
                comment.DateAdded = DateTime.Now;
                comment.ParentId = ParentId;
                comment.PostId = db.Blogs.Find(PostId);
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "posts", new { id = comment.PostId.Id });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [Authorize]
        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id, string text)
        {
            Comment edit = db.Comments.Find(id);

            if (edit.User.Id.Equals(User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    edit.Text = text;
                    db.Entry(edit).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("details", "posts", new { id = edit.PostId.Id });
                }

                return Json(new { Result = "Comment updated sucessfully" },
                JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = "Comment was not updated" },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            Comment comment = db.Comments.Find(id);
            int postid = comment.PostId.Id;

            if (comment.User.Id.Equals(User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);

                db.SaveChanges();
            }
            return RedirectToAction("Details", "posts", new { id = postid });
        }

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