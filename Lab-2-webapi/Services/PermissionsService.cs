
using Lab_2_webapi.Models;
using Lab_2_webapi.Validators;
using Lab_2_webapi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2_webapi.Services
{
    public interface IPermissionsService
    {

        object Delete(int id, int permissionid);
        PermissionsGetModel GetPermission(int id);
        UserRole GetPermissionFromDb(String permissionName);
        object PermissionUpsert(int userId, PermissionPostModel permissionPostModel);
        IEnumerable<UserRole> GetAllUserRole();
        UserRole DeleteUserRole(int id);
        UserRole Upsert(UserRolePostModel userRole);

    }

    public class PermissionsService : IPermissionsService
    {
        private TasksDbContext context;
        private readonly AppSettings appSettings;
        private IPermissionValidator permissionValidator;
        public PermissionsService(TasksDbContext context, IOptions<AppSettings> appSettings, IPermissionValidator permissionValidator)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
            this.permissionValidator = permissionValidator;

        }


        public object Delete(int userId, int idPermission)
        {
            var existing = context.Users.Include("UserUserRoles.UserRole").FirstOrDefault(u => u.Id == userId);
            var permission = context.UserUserRoles.FirstOrDefault(u => u.Id == idPermission);
            if (existing == null)
            {
                return null;
            }
            if (existing.UserUserRoles.Contains(permission))
            {
                UserUserRole currentUseRole = existing.UserUserRoles.FirstOrDefault(u => u.Id.Equals(permission.Id));
                context.UserUserRoles.Remove(currentUseRole);
            }
            else
            {
                return null;
            }

            context.SaveChanges();
            return this.GetPermission(userId);

        }

        public PermissionsGetModel GetPermission(int id)
        {

            // return users without passwords
            User user = context.Users.Include("UserUserRoles.UserRole").FirstOrDefault(u => u.Id == id);
            List<PermissionGetModel> permissions = new List<PermissionGetModel>();
            if (user != null)
            {
                foreach (UserUserRole u in user.UserUserRoles)
                { permissions.Add(this.PermissionHelper(u)); }
                PermissionsGetModel permissionGetModel = new PermissionsGetModel
                {
                    Id = user.Id,
                    UserName = user.Username,
                    UserUserRoles = permissions.OrderBy(o => o.StartTime).ToList()

                    //UserUserRoles = user.UserUserRoles.OrderBy(o=>o.StartTime).ToList()
                };
                return permissionGetModel;
            }
            else { return null; }

        }
        public PermissionGetModel PermissionHelper(UserUserRole userModel)
        {
            return new PermissionGetModel
            {
                Id = userModel.Id,
                UserRole = userModel.UserRole.Name,
                StartTime = userModel.StartTime,
                EndTime = userModel.EndTime
            };
        }
        public UserRole GetPermissionFromDb(String permissionName)
        {
            UserRole permission = context.UserRoles.FirstOrDefault(u => u.Name.Equals(permissionName));
            if (permission == null) { return null; }
            else return permission;
        }
        public object PermissionUpsert(int userId, PermissionPostModel permissionPostModel)
        {
            var errors = permissionValidator.Validate(permissionPostModel, context);
            if (errors != null)
            {
                return errors;
            }
            var existing = context.Users.Include("UserUserRoles.UserRole").FirstOrDefault(u => u.Id == userId);
            var permission = this.GetPermissionFromDb(permissionPostModel.UserRole);
            if (existing == null)
            {
                return null;
            }
            if (existing.UserUserRoles.Any(u => u.
           UserRole.Equals(permission)))
            {
                UserUserRole currentUseRole = existing.UserUserRoles.FirstOrDefault(u => u.UserRole.Equals(permission));
                currentUseRole.EndTime = permissionPostModel.EndTime;
                currentUseRole.StartTime = permissionPostModel.StartTime;
                context.UserUserRoles.Update(currentUseRole);
            }
            else
            {
                context.UserUserRoles.Add(new UserUserRole
                {
                    User = existing,
                    UserRole = permission,
                    StartTime = permissionPostModel.StartTime,
                    EndTime = permissionPostModel.EndTime,
                });
            }

            context.SaveChanges();
            return this.GetPermission(userId);
        }





        public UserRole RoleCreate(UserRole userRole)
        {
            context.UserRoles.Add(userRole);
            context.SaveChanges();
            return userRole;
        }

        public UserRole DeleteUserRole(int id)
        {
            var existing = context.UserRoles
                .FirstOrDefault(userRole => userRole.Id == id);
            if (existing == null)
            {
                return null;
            }
            context.UserRoles.Remove(existing);
            context.SaveChanges();
            return existing;
        }

        public IEnumerable<UserRole> GetAllUserRole()
        {
            return context.UserRoles;
        }

       

        public UserRole Upsert(UserRolePostModel userRole)
        {

            var existing = context.UserRoles.FirstOrDefault(f => f.Name == userRole.UserRole);
            if (existing == null)
            {
                context.UserRoles.Add(new UserRole
                {
                    Description = userRole.Description,
                    Name = userRole.UserRole

                });
                context.SaveChanges();
            }
            else

            {
                existing.Name = userRole.UserRole;
                existing.Description = userRole.Description;
                context.UserRoles.Update(existing);
            }
            context.SaveChanges();
            return existing = context.UserRoles.FirstOrDefault(f => f.Name == userRole.UserRole);
        }
    }
}