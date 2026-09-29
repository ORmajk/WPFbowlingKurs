using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    public partial class Events
    {
        public string LaneText
        {
            get { return Lanes != null ? "Дорожка №" + Lanes.LaneNumber + " (" + Lanes.LaneTypes.Name + ")" : "Все дорожки клуба"; }
        }

        public string MembersText
        {
            get { return "Участников: " + EventRegistrations.Count; }
        }

        public string MyStatus { get; set; }
    }
}
