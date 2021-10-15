using GamificationService.Model;
using GamificationService.service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GamificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamificationController : ControllerBase
    {
        private IConfiguration _configuration;

        public GamificationController(IConfiguration iConfig)
        {
            _configuration = iConfig;
        }

        [HttpGet("updateBadgeAndTrophy")]
        public async Task<IActionResult> RunRules([FromQuery] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest(new Exception("Email Id is empty"));
            }
            //getinteractions of Author
            var url = _configuration["InteractionAPIUrl"].ToString() + userName;
            List<Interaction> interactionIds = new List<Interaction>();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                interactionIds = JsonConvert.DeserializeObject<List<Interaction>>(result);
            }

            //get posts of Author
            url = _configuration["BlogsAPIUrl"].ToString() + userName;
            List<string> blogsIds = new List<string>();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                blogsIds = JsonConvert.DeserializeObject<List<string>>(result);
            }

            //get rules of Gamification
            string sqlDataSource = _configuration["DBReadConnectionString"].ToString();
            var rules = ReadRules.getRules(sqlDataSource);
            var trophycount = 0;
            //Process logic for gamification and assign trophy Id
            StringBuilder sb = new StringBuilder();
            foreach(var rule in rules)
            {
                if(rule.RuleConditionTitle == "Like")
                {
                    if (interactionIds.Count >= rule.RuleConditionValue)
                    {
                        sb.Append(rule.TrophyId + ",");
                        trophycount++;
                    }
                }
                else if(rule.RuleConditionTitle == "Post")
                {
                    if (blogsIds.Count >= rule.RuleConditionValue)
                    {
                        sb.Append(rule.TrophyId + ",");
                        trophycount++;
                    }
                }
            }
            var trophyIds = RemoveLast(sb, ",");

            //Get profile information
            url = _configuration["UserManagementAPIUrl"].ToString() + "getProfileByUserName?username=" +userName;
            var userProfile = new UserMgmt();
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                userProfile = JsonConvert.DeserializeObject<List<UserMgmt>>(result).First();
            }
            //checkwhether it needs to be updated
            if (userProfile.TrophyIds == trophyIds.ToString())
                return Ok();
            else
            {
                //update if we can
                url = _configuration["UserManagementAPIUrl"].ToString() + "modifyTrophyIdsProfile";
                userProfile.TrophyIds = trophyIds.ToString();
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PutAsync(url, GetStringContent(userProfile));
                    var result = await response.Content.ReadAsStringAsync();
                }
               var badgeLevel = CheckBadgeIdUpdate(trophycount);
                if (badgeLevel != Convert.ToInt32(userProfile.BadgeIds))
                {
                    url = _configuration["UserManagementAPIUrl"].ToString() + "modifyBadgeIdsProfile";
                    userProfile.BadgeIds = badgeLevel.ToString();
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.PutAsync(url, GetStringContent(userProfile));
                        var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return Ok("Trophy and badge have been checked and updated accordingly.");
        }
        
        [NonAction]
        public int CheckBadgeIdUpdate(int trophyCount)
        {
            int level = 1;
            if (trophyCount > 10 && trophyCount < 20)
                level = 2;
            else if (trophyCount > 20)
                level = 3;
            return level;
        }
        
        [NonAction]
        public StringContent GetStringContent(object obj)
        {
            var jsonContent = JsonConvert.SerializeObject(obj);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return contentString;
        }

        [NonAction]
        public StringBuilder RemoveLast(StringBuilder sb, string value)
        {
            if (sb.Length < 1) return sb;
            sb.Remove(sb.ToString().LastIndexOf(value), value.Length);
            return sb;
        }
    }
}
