using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    public partial class Lanes
    {
        public string PhotoPath
        {
            get { return PhotoHelper.LanePhoto(LaneTypes != null ? LaneTypes.Name : ""); }
        }
    }
}
