using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AstronomyPicOfTheDay.Classes
{
    [Serializable()]
    public class PictureOfTheDay
    {
        public string url;
        public string media_type;
        public string explanation;
        public string title;
        public string date;        

        public bool DownloadFromUrl()
        {
            if (string.IsNullOrEmpty(url))
                return false;

            string localFilename = @"C:\APOD.jpg";
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, localFilename);
            }

            return true;
        }
    }
}
