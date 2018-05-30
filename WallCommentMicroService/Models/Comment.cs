using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WallCommentMicroService.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string TextComment { get; set; }
        public Guid  UserId { get; set; }
        public Guid PostId { get; set; }
        public bool Active { get; set; }
    }
}