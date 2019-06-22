using Lab_2_webapi.Models;
using Lab_2_webapi.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = Lab_2_webapi.Models.Task;

namespace Lab_2_webapi.Services
{


    public interface ICommentService
    {
        PaginatedList<TaskCommentlModel> GetAll(int page, string keyword);
    }

    public class CommentService : ICommentService
    {
        private TasksDbContext context;

        public CommentService(TasksDbContext context)
        {
            this.context = context;
        }

        public PaginatedList<TaskCommentlModel> GetAll(int page, string keyword)
        {
            IQueryable<Comment> result = context
                .Comments
                .Where(c => string.IsNullOrEmpty(keyword) || c.Text.Contains(keyword))
                .OrderBy(c => c.Id);
            var paginatedResult = new PaginatedList<TaskCommentlModel>();
            paginatedResult.CurrentPage = page;

            paginatedResult.NumberOfPages = (result.Count() - 1) / PaginatedList<TaskCommentlModel>.EntriesPerPage + 1;
            result = result
                .Skip((page - 1) * PaginatedList<TaskCommentlModel>.EntriesPerPage)
                .Take(PaginatedList<TaskCommentlModel>.EntriesPerPage);
            paginatedResult.Entries = result.Select(c => TaskCommentlModel.FromComment(c)).ToList();

            return paginatedResult;


        }
    }
}