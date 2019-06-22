using Lab_2_webapi.Models;
using Lab_2_webapi.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiTests
{
    class CommentsServiceTest
    {


        [Test]
        public void GetAllShouldReturnCorrectNumberOfPages()
        {
            var options = new DbContextOptionsBuilder<TasksDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllShouldReturnCorrectNumberOfPages))
              .Options;

            using (var context = new TasksDbContext(options))
            {

                var commentService = new CommentService(context);
                var taskService = new TaskService(context);
                var addedTask = taskService.Create(new Lab_2_webapi.Models.Task
                {
                    Title = "fdsfsd",
                    DateAdded = new DateTime(),
                    Description = "large",
                  
                    Deadline = new DateTime(),
                    ClosedAt = new DateTime()
                });
                addedTask.Comments.Add(new Comment
                {
                 Text = "vvv",
                Important = true
               });
             
                var allComments = commentService.GetAll(1, string.Empty);
                Assert.AreEqual(1, allComments.NumberOfPages);
            }
        }
    }
}
