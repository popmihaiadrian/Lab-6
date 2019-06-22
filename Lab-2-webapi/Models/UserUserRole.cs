﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.Models
{
    public class UserUserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
