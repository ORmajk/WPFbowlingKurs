using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    internal class AiHelper
    {
        private const string NotReady = "AI-помощник пока не может ответить. Попробуйте позже.";

        public static string AnalyzeLoad()
        {
            return NotReady;
        }

        public static string Forecast()
        {
            return NotReady;
        }

        public static string Recommendations()
        {
            return NotReady;
        }

        public static string PickInventory(int weightKg, int age, int shoeSize)
        {
            return NotReady;
        }

        public static string EventDescription(string name, DateTime date, Lanes lane)
        {
            return "";
        }

        public static string FreeTime(DateTime day)
        {
            return NotReady;
        }
    }
}
