
using Lab_2_webapi.Models;
using Lab_2_webapi.Services;
using Lab_2_webapi.Validators;
using Lab_2_webapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.Validators
{
   
    public interface IPermissionValidator
    {
        ErrorsCollection Validate(PermissionPostModel permissionPostModel, TasksDbContext context);
    }

    public class PermissionValidator : IPermissionValidator
    {
        public ErrorsCollection Validate(PermissionPostModel permissionPostModel, TasksDbContext context)
        {
            ErrorsCollection errorsCollection = new ErrorsCollection { Entity = nameof(permissionPostModel) };
            UserRole permission = context.UserRoles.FirstOrDefault(u => u.Name.Equals(permissionPostModel.UserRole));
            if (permission == null) { errorsCollection.ErrorMessages.Add($"The userrole doesn't exist!"); }
            if (permission != null) { return null; }
            return errorsCollection;
        }
    }
}
