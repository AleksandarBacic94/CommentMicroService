using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using URISUtil.DataAccess;
using WallCommentMicroService.DataAccess;
using WallCommentMicroService.Models;

namespace WallCommentMicroService.Tests.UnitTests
{
    public class CommentUnitTest
    {
        ActiveStatusEnum active = ActiveStatusEnum.Active;
        SqlConnection connection;
        string post_id;

        [SetUp]
        public void TestInitialize()
        {
            FileInfo file = new FileInfo("C:\\Users\\Gligoric\\Documents\\URIS\\commentMSTestInserts.sql");
            string script = file.OpenText().ReadToEnd();
            connection = new SqlConnection(DBFunctions.ConnectionString);
            post_id = "post_1";
            SqlCommand command = connection.CreateCommand();
            command.CommandText = script;
            connection.Open();
            command.ExecuteNonQuery();
        }

        [Test]
        public void GetCommentsByPostIdSuccess()
        {
            Assert.AreEqual(CommentDB.GetCommentsByPostId(active, post_id).Count, 2);
        }

        [Test]
        public void GetCommentsByPostIdFailed()
        {
            Assert.IsEmpty(CommentDB.GetCommentsByPostId(active, "postIdThatNotExists"));
        }

        [Test]
        public void GetCommentSuccess()
        {
            Guid id = CommentDB.GetCommentsByPostId(active, post_id)[0].Id;
            Assert.IsNotNull(CommentDB.GetComment(id));
        }

        [Test]
        public void GetCommentFailed()
        {
            Guid id = Guid.NewGuid();
            Assert.IsNull(CommentDB.GetComment(id));
        }

        [Test]
        public void InsertCommentSuccess()
        {
            Comment comment = new Comment
            {
                TextComment = "new text comment",
                UserId = "user_1",
                PostId = "post_1",
                Active = true
            };
            
            Assert.IsNotNull(CommentDB.InsertComment(comment));
        }
        [Test]
        public void InsertCommentFailed()
        {
            Comment comment = new Comment
            {
                TextComment = "",
                UserId = "user_1",
                PostId = "post_1",
                Active = true
            };
            Assert.IsNull(CommentDB.InsertComment(comment));
        }

        [Test]
        public void UpdateCommentSuccess()
        {
            Guid id = CommentDB.GetCommentsByPostId(active, post_id)[0].Id;
            Comment comment = new Comment
            {
                TextComment = "updated text comment",
                UserId = "user_1",
                PostId = "post_1",
                Active = true
            };
            Comment updatedComment = CommentDB.UpdateComment(comment, id);
            Assert.AreEqual(comment.TextComment, updatedComment.TextComment);
        }

        [Test]
        public void UpdateCommentFailed()
        {
            Guid id = CommentDB.GetCommentsByPostId(active, post_id)[0].Id;
            Comment comment = new Comment
            {
                TextComment = "",
                UserId = "user_1",
                PostId = "post_1",
                Active = true
            };
            Comment updatedComment = CommentDB.UpdateComment(comment, id);
            Assert.IsNull(updatedComment);
        }

        [Test]
        public void DeleteCommentSuccess()
        {
            Guid id = CommentDB.GetCommentsByPostId(active, post_id)[0].Id;
            int oldNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, post_id).Count;
            CommentDB.DeleteComment(id);
            int newNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, post_id).Count;
            Assert.AreEqual(oldNumberOfCommentsByPostId - 1, newNumberOfCommentsByPostId);
        }

        [Test]
        public void DeleteCommentFailed()
        {
            Guid id = Guid.NewGuid();

            int oldNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, post_id).Count;
            CommentDB.DeleteComment(id);
            int newNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, post_id).Count;
            Assert.AreEqual(oldNumberOfCommentsByPostId, newNumberOfCommentsByPostId);
        }

        [TearDown]
        public void TestCleanup()
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = String.Format(@"DROP TABLE [comment].[Comment]");
            command.ExecuteNonQuery();
            connection.Close();

        }
    }
}
