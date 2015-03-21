using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    public class Post
    {


        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        [Display(Name="Title")]
        public string Name { get; set; }
        [Required]
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Text { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastEdit { get; set; }
        public virtual ApplicationUser Authour { get; set; }
        public virtual ICollection<Comment> Comments {get; set;}
        public virtual ICollection<Tag> Tags { get; set; }

        public static Dictionary<DateTime, int> Archive(IEnumerable<Post> post)
       {
           DateTime date = post.Select(x => x.DateAdded).Take(1).First();
          
           Dictionary<DateTime, int> dates =
       new Dictionary<DateTime, int>();
         
           while (date.Month <= DateTime.Now.Month)
            {


                dates.Add(date, post.Where(x => x.DateAdded.Year == date.Year && x.DateAdded.Month == date.Month).Count());
                  date=date.AddMonths(1);
               
           }
            
          
         return dates;

       }




    }

 
}