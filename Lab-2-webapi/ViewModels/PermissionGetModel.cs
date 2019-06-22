using Lab_2_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.ViewModels
{
    public class PermissionGetModel
    {
        public int Id { get; set; }
        public string UserRole { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public static PermissionGetModel ToPermission(UserUserRole userModel)
        {

            return new PermissionGetModel
            {
                Id = userModel.Id,
                UserRole = userModel.UserRole.Name,
                StartTime = userModel.StartTime,
                EndTime = userModel.EndTime
            };
        }
    }
}