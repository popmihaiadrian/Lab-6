using Lab_2_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.ViewModels
{
    public class PermissionPostModel
    {
      
        public string UserRole { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
       
    }
}