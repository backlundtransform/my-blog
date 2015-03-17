using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Tag
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Post PostId { get; set; }
        

    }
}