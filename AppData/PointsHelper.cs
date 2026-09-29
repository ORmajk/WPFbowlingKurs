using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    internal class PointsHelper
    {
        public const int PointsForBooking = 10;
        public const int PointsForEvent = 5;
        public const int PointsForExport = 2;
        public const int PriceAI = 100;

        public class AwardRule
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int NeedPoints { get; set; }
        }

        public static List<AwardRule> Rules = new List<AwardRule>
        {
            new AwardRule { Name = "Новичок клуба", Description = "Получена при первом бронировании", NeedPoints = 10 },
            new AwardRule { Name = "Постоянный игрок", Description = "10 посещений клуба", NeedPoints = 100 },
            new AwardRule { Name = "Знаток боулинга", Description = "Набрано 300 баллов", NeedPoints = 300 },
            new AwardRule { Name = "Легенда клуба", Description = "Набрано 500 баллов", NeedPoints = 500 }
        };

        public static string AddPoints(Users user, int points)
        {
            if (user == null) return "";
            user.Points += points;
            return CheckAwards(user);
        }

        public static string AddClientPoints(Clients client, int points)
        {
            if (client == null) return "";
            client.Points += points;

            var user = AppConnect.FindClientUserByPhone(client.Phone);
            if (user != null)
                return AddPoints(user, points);
            return "";
        }

        public static bool SpendPoints(Users user, int points)
        {
            if (user == null || user.Points < points) return false;
            user.Points -= points;

            if (user.Role == "Client")
            {
                var client = AppConnect.GetCurrentClient();
                if (client != null)
                    client.Points = Math.Max(0, client.Points - points);
            }
            return true;
        }

        public static string CheckAwards(Users user)
        {
            StringBuilder result = new StringBuilder();
            foreach (var rule in Rules)
            {
                if (user.Points >= rule.NeedPoints)
                {
                    if (GiveAward(user, rule.Name, rule.Description))
                        result.AppendLine("Получена награда: «" + rule.Name + "»");
                }
            }
            return result.ToString();
        }

        public static bool GiveAward(Users user, string awardName, string description)
        {
            var award = AppConnect.model01.Awards.Local.FirstOrDefault(x => x.Name == awardName);
            if (award == null)
                award = AppConnect.model01.Awards.FirstOrDefault(x => x.Name == awardName);
            if (award == null)
            {
                award = new Awards();
                award.Name = awardName;
                award.Description = description;
                AppConnect.model01.Awards.Add(award);
            }

            bool has = user.UserAwards.Any(x => x.Awards == award || (award.Id != 0 && x.AwardId == award.Id));
            if (has) return false;

            UserAwards ua = new UserAwards();
            ua.Users = user;
            ua.Awards = award;
            ua.AwardedAt = DateTime.Now;
            AppConnect.model01.UserAwards.Add(ua);
            return true;
        }

        public static AwardRule NextRule(int points)
        {
            return Rules.Where(x => x.NeedPoints > points).OrderBy(x => x.NeedPoints).FirstOrDefault();
        }
    }
}
