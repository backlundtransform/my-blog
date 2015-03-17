using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogs

       
        public ActionResult Index(string search, string tag, string month, string year, int Page = 1, int PageSize = 5)
        {
            ViewBag.archive = Post.Archive(db.Blogs.ToList()).OrderByDescending(x => x.Key);
            ViewBag.comments = db.Comments.Take(4);

           if (search != null)
            {

               return View(db.Blogs.Where(x => x.Name.Contains(search) || x.Text.Contains(search)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
              }
            else if (tag != null)
            {
                return View(db.Blogs.Where(x => x.Tags.Select(m => m.Name).Contains(tag)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
            }
            else if (month != null && year != null)
            {
                return View(db.Blogs.Where(x => x.DateAdded.Year.ToString().Equals(year) && x.DateAdded.Month.ToString().Equals(month)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));

            }
          
            else 
            {
                return View(db.Blogs.OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
            }
        }

        public PartialViewResult GetPartialIndex(string search, string tag, string month, string year, int Page = 1, int PageSize = 5)
        {

            if (search != null)
            {

                return PartialView("_Postlist", db.Blogs.Where(x => x.Name.Contains(search) || x.Text.Contains(search)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
            }
            else if (tag != null)
            {
                return PartialView("_Postlist", db.Blogs.Where(x => x.Tags.Select(m => m.Name).Contains(tag)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
            }
            else if (month != null && year != null)
            {
                return PartialView("_Postlist", db.Blogs.Where(x => x.DateAdded.Year.ToString().Equals(year) && x.DateAdded.Month.ToString().Equals(month)).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));

            }

            else
            {
                return PartialView("_Postlist", db.Blogs.OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));
            }
        }

        public ActionResult Commentform(int postid, int parentid)
        {

            return View("ReplyCommentForm", new Blog.Models.Comment() { PostId = db.Blogs.Find(postid), ParentId = parentid });

        }
      

        [HttpPost]
        public ActionResult AngularTagInit(int id)
        {


            return Json(new { Result = db.Tags.Where(x => x.PostId.Id == id).Select(x => new { x.Name, x.Id }).ToList() });

        }

        [HttpPost]
        public ActionResult AngularTagRemove(int tagid, int postid)
        {

            Tag tag = db.Tags.Find(tagid);

            db.Tags.Remove(tag);
            db.SaveChanges();

            return Json(new { Result = db.Tags.Where(x => x.PostId.Id == postid).Select(x => new { x.Name, x.Id }).ToList() });

        }

        [HttpPost]
        public ActionResult AngularTag(String value, int id)
        {
            Post blog = db.Blogs.Find(id);
            Tag tag = new Tag();
            tag.PostId = blog;
            tag.Name = value;
            blog.Tags.Add(tag);
          
            db.SaveChanges();

            return Json(new { Result = db.Tags.Where(x => x.PostId.Id ==id).Select(x => new { x.Name, x.Id }).ToList() });

        }


        [HttpPost]
        public ActionResult TagWidget()
        {

     
            return Json(new { Result = db.Tags.GroupBy(x => x.Name).Select(x => new { Name = x.Key, Count = x.Distinct().Count() }).ToList() });

        }

       

        public ActionResult Search(string search, int Page = 1, int PageSize = 5)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            return RedirectToAction("Index", db.Blogs.Where(x => x.Name == search || x.Text == search).OrderByDescending(s => s.DateAdded).ToPagedList(Page, PageSize));

        }

        // GET: Blogs/Details/5
        public ActionResult Details(int? id, int Page = 1)
        {
            ViewBag.archive = Post.Archive(db.Blogs.ToList()).OrderByDescending(x => x.Key);
            ViewBag.comments = db.Comments.Take(4);
          

            
            ViewData["Page"] = Page;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Post blog = db.Blogs.Find(id);
          
            if (blog == null)
            {
                return HttpNotFound();
            }
          
               return View(blog);
           
            
        }

        // GET: Blogs/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.archive = Post.Archive(db.Blogs.ToList()).OrderByDescending(x => x.Key);
            ViewBag.comments = db.Comments.Take(4);

            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Text,DateAdded,LastEdit")]  Post blog)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                blog.Authour = user.ApplicationUserName(db);
               
               
               
                blog.DateAdded = DateTime.Now;
                blog.LastEdit= DateTime.Now;
              
                blog.Authour.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }
          [Authorize(Roles="Admin")]
        // GET: Blogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Text,DateAdded,LastEdit")] Post blog)
          {

              ViewBag.archive = Post.Archive(db.Blogs.ToList()).OrderByDescending(x => x.Key);
            ViewBag.comments = db.Comments.Take(4);

             if (ModelState.IsValid)
            {
                DateTime Dateadded = db.Blogs.Find(blog.Id).DateAdded; 
                blog.LastEdit = DateTime.Now;
                blog.DateAdded = Dateadded;
                db.Entry(db.Blogs.Find(blog.Id)).CurrentValues.SetValues(blog); 
              
              
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }


         [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Post blog = db.Blogs.Find(id);

            List<Comment> comments = new List<Comment>(blog.Comments);

            foreach (var comment in comments)
            {
                db.Comments.Remove(comment);
            }

            List<Tag> tags = new List<Tag>(blog.Tags);
            foreach (var tag in tags)
            {
                db.Tags.Remove(tag);
            }
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
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
