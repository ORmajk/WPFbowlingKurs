using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    internal class AppConnect
    {
        public static BowlingClubEntities model01;

        public static Users currentUser;

        public static Clients GetCurrentClient()
        {
            if (currentUser == null || currentUser.Role != "Client")
                return null;

            var client = FindClientByPhone(currentUser.Phone);
            if (client == null)
            {
                client = new Clients();
                client.FullName = currentUser.FullName;
                client.Phone = currentUser.Phone;
                client.Email = currentUser.Email;
                client.RegistrationDate = DateTime.Today;
                client.Points = currentUser.Points;
                client.IsSubscribedToAl = currentUser.IsSubscribedToAI;
                model01.Clients.Add(client);
                model01.SaveChanges();
            }
            return client;
        }

        public static string PhoneDigits(string phone)
        {
            if (phone == null) return "";
            string digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length > 10)
                digits = digits.Substring(digits.Length - 10);
            return digits;
        }

        public static Clients FindClientByPhone(string phone)
        {
            string digits = PhoneDigits(phone);
            if (digits == "") return null;
            return model01.Clients.ToList().FirstOrDefault(x => PhoneDigits(x.Phone) == digits);
        }

        public static Users FindClientUserByPhone(string phone)
        {
            string digits = PhoneDigits(phone);
            if (digits == "") return null;
            return model01.Users.Where(x => x.Role == "Client").ToList().FirstOrDefault(x => PhoneDigits(x.Phone) == digits);
        }

        public static string GetRoleName(string role)
        {
            if (role == "Admin") return "Администратор";
            if (role == "Manager") return "Менеджер";
            if (role == "Client") return "Клиент";
            return "Оператор";
        }

        public static void AddLog(string actionType, string details)
        {
            try
            {
                UserActions action = new UserActions();
                action.UserId = currentUser.Id;
                action.ActionType = actionType;
                action.Details = details;
                action.Timestamp = DateTime.Now;
                model01.UserActions.Add(action);
                model01.SaveChanges();
            }
            catch
            {
            }
        }

        public static void RefreshData()
        {
            foreach (var entry in model01.ChangeTracker.Entries().ToList())
            {
                if (entry.State == EntityState.Added)
                    entry.State = EntityState.Detached;
                else
                    entry.Reload();
            }
        }

        public static string GetError(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex.Message;
        }
    }
}
