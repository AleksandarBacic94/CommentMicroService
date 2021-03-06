﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WallCommentMicroService.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
        public decimal Rating { get; set; }
        public int Views { get; set; }
        public byte[] Attachment { get; set; }
        public string Location { get; set; }
        public List<Guid> Labels { get; set; }
        public bool Active { get; set; }
        public string UserId { get; set; }
    }
}