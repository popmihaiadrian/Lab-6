using Lab_2_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.ViewModels
{
    public class PermissionsGetModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<PermissionGetModel> UserUserRoles { get; set; }
    }
}
