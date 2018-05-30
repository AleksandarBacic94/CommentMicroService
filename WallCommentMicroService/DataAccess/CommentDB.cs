using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;
using WallCommentMicroService.Models;

namespace WallCommentMicroService.DataAccess
{
    public class CommentDB
    {
        private static Comment ReadRow(SqlDataReader reader)
        {
            Comment retVal = new Comment
            {
                Id = (Guid)reader["id"],
                TextComment = reader["textComment"] as string,
                UserId = (Guid)reader["user_id"] ,
                PostId = (Guid)reader["post_id"] ,
                Active = (bool)reader["active"]
            };

            return retVal;
        }

        private static void ReadId(SqlDataReader reader, Comment comment)
        {
            comment.Id = (Guid)reader["id"];
        }

        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [Comment].[id],
                    [Comment].[textComment],
                    [Comment].[user_id],
                    [Comment].[post_id],
                    [Comment].[active]
	                
                ";
            }
        }

        private static void FillData(SqlCommand command, Comment comment)
        {
            command.AddParameter("@Id", SqlDbType.UniqueIdentifier, comment.Id);
            command.AddParameter("@TextComment", SqlDbType.NVarChar, comment.TextComment);
            command.AddParameter("@UserId", SqlDbType.UniqueIdentifier, comment.UserId);
            command.AddParameter("@PostId", SqlDbType.UniqueIdentifier, comment.PostId);
            command.AddParameter("@Active", SqlDbType.Bit, comment.Active);
        }

        public static List<Comment> GetCommentsByPostId(ActiveStatusEnum active, string id)
        {
            try
            {
                List<Comment> retVal = new List<Comment>();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                    SELECT {0}  
                    FROM [comment].[Comment]
                    WHERE ([comment].[Comment].post_id = @Id) 
                    AND ([comment].[Comment].active = @Active)
                    
                ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.NVarChar, id);
                    command.Parameters.Add("@Active", SqlDbType.Bit);

                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
            
        }

        public static Comment GetComment(Guid id)
        {
            try
            {
                Comment retVal = new Comment();
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT {0} FROM [comment].[Comment] 
                        WHERE [comment].[Comment].Id = @Id;
                    ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, id);
                    connection.Open();

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadRow(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return retVal;
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Comment InsertComment(Comment comment)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    Guid id = Guid.NewGuid();
                    comment.Id = id;
                    command.CommandText = String.Format(@"
                        INSERT INTO [comment].[Comment] 
                         (
                            [id],
                            [textComment],
                            [post_id],
                            [user_id],
                            [active]
                                          
                        )
                        VALUES
                        (
                            @Id,
                            @TextComment,
                            @PostId,
                            @UserId,
                            @Active
                        )
                    ");
                    FillData(command, comment);
                    if(comment.TextComment == null || comment.TextComment == "")
                    {
                        return null;
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                    return GetComment(comment.Id);
                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Comment UpdateComment(Comment comment, Guid id)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE [comment].[Comment] 
                        SET 
                            [textComment] = @TextComment 
                           
                        WHERE
                            [id] = @Id
                    ");

                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, id);
                    command.AddParameter("@TextComment", SqlDbType.NVarChar, comment.TextComment);
                    if(comment.TextComment == null || comment.TextComment == "")
                    {
                        return null;
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                    return GetComment(id);
                }
            }
            catch(Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void DeleteComment(Guid id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [comment].[Comment]
                        SET
                            [Active] = 0
                        WHERE
                            [Id] = @Id     
                    ");

                    command.AddParameter("@Id", SqlDbType.UniqueIdentifier, id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}