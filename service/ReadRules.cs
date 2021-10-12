using GamificationService.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GamificationService.service
{
    static public class ReadRules
    {
        static public List<Trophy> getRules(string url)
        {
            List<Trophy> allTrophy = new List<Trophy>();
            DataTable table = new DataTable();

            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(url))
            {
                var query = "select TrophyId, TrophyName, TrophyDescription, RuleConditionTitle, RuleConditionValue from usr_mgmt.Trophy";
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }
            foreach (DataRow row in table.Rows)
            {
                var trophyObj = new Trophy();
                trophyObj.TrophyId = Convert.ToInt32(row["TrophyId"].ToString()) ;
                trophyObj.TrophyName = row["TrophyName"].ToString();
                trophyObj.TrophyDescription = row["TrophyDescription"].ToString();
                trophyObj.RuleConditionTitle = row["RuleConditionTitle"].ToString();
                trophyObj.RuleConditionValue = Convert.ToInt32(row["RuleConditionValue"].ToString());
                allTrophy.Add(trophyObj);
            }
            return allTrophy;
        }
    }
}
