using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamificationService.Model
{
    public class Trophy
    {
        public int TrophyId { get; set; }
        public string TrophyName { get; set; }
        public string TrophyDescription { get; set; }
        public string RuleConditionTitle { get; set; }
        public int RuleConditionValue { get; set; }
    }
}
