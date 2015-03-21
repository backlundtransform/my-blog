using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [AllowHtml]
        public string Text { get; set; }

        public int Like { get; set; }

        public int Dislike { get; set; }

        public virtual Post PostId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public DateTime DateAdded { get; set; }

        public int ParentId { get; set; }

        public virtual ICollection<Comment> Children { get; set; }

        public bool CanEdit(string id)
        {
            if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.Identity.IsAuthenticated && (id.ToString().Equals(HttpContext.Current.User.Identity.GetUserId())))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Comment> GetReply(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Comments.Where(x => x.ParentId == Id).ToList();
        }

        public string ToRelativeDate(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return string.Format("{0} seconds ago", timeSpan.Seconds);

            if (timeSpan <= TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? String.Format("about {0} minutes ago", timeSpan.Minutes) : "about a minute ago";

            if (timeSpan <= TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? String.Format("about {0} hours ago", timeSpan.Hours) : "about an hour ago";

            if (timeSpan <= TimeSpan.FromDays(30))
                return timeSpan.Days > 1 ? String.Format("about {0} days ago", timeSpan.Days) : "yesterday";

            if (timeSpan <= TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? String.Format("about {0} months ago", timeSpan.Days / 30) : "about a month ago";

            return timeSpan.Days > 365 ? String.Format("about {0} years ago", timeSpan.Days / 365) : "about a year ago";
        }
    }


}