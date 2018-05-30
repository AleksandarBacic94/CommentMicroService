using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUtil.DataAccess;
using WallCommentMicroService.DataAccess;
using WallCommentMicroService.Models;
using WallCommentMicroService.ServiceCalls;

namespace WallCommentMicroService.Controllers
{
    [RoutePrefix("api")]
    public class CommentController : ApiController
    {
        [Route("Comment/PostComments/{id}"), HttpGet]
        public IEnumerable<Comment> GetCommentsByPostId(string id, [FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return CommentDB.GetCommentsByPostId(active, id);
        }

        [Route("Comment/{id}"), HttpGet] 
        public Comment GetComment(Guid id)
        {
            return CommentDB.GetComment(id);
        }

        [Route("Comment"), HttpPost]
        public Comment InsertComment([FromBody]Comment comment)
        {
            return CommentDB.InsertComment(comment);
        }

        [Route("Comment/{id}"), HttpPut]
        public Comment UpdateComment([FromBody]Comment comment, Guid id)
        {
            return CommentDB.UpdateComment(comment, id);
        }

        [Route("Comment/{id}"), HttpDelete]
        public void DeleteComment(Guid id)
        {
            CommentDB.DeleteComment(id);
        }

        [Route("User/{id}"), HttpGet]
        public User GetUser(Guid id)
        {
            return UserService.GetUser(id);
        }
    }
}