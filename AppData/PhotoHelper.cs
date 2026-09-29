using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    internal class PhotoHelper
    {
        private static string ImagesFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"); }
        }

        public static string NoPhoto
        {
            get { return Path.Combine(ImagesFolder, "no-photo.png"); }
        }

        public static string Find(string subFolder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return NoPhoto;

            string name = Path.GetFileNameWithoutExtension(fileName.Replace('/', '\\'));
            foreach (string ext in new[] { ".jpg", ".jpeg", ".png" })
            {
                string path = Path.Combine(ImagesFolder, subFolder, name + ext);
                if (File.Exists(path))
                    return path;
            }
            return NoPhoto;
        }

        public static string LanePhoto(string typeName)
        {
            if (typeName == "VIP")
                return Find("lanes", "VIP_lane");
            if (typeName == "Детская")
                return Find("lanes", "Tiny_lane");
            return Find("lanes", "Norm_lanes");
        }
    }
}
