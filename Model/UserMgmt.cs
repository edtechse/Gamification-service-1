using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamificationService.Model
{
    public class UserMgmt
    {
            public int UserId { get; set; }
            public string UserEmailId { get; set; }
            public string UserName { get; set; }
            public int Age { get; set; }
            public string PhoneNumber { get; set; }
            public string BadgeIds { get; set; }
            public string TrophyIds { get; set; }
            public string Genres { get; set; }
            public bool InUse { get; set; }
    }
}
