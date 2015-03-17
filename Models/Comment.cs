using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;


namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
       
        [Required]
        [StringLength(512)]
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
            

            if(HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.Identity.IsAuthenticated && (id.ToString().Equals(HttpContext.Current.User.Identity.GetUserId())))
            {
                return true;

            }
            else{
                return false;
            }
            
        
           
        }
        public List<Comment> GetReply(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Comments.Where(x => x.ParentId == Id).ToList();
        }
    }

    

    
   
}