using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AstronomyPicOfTheDay.Classes
{
    class Utils
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public string SendGet(string url)
        {
            string content = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                request.Proxy = null;
                request.Timeout = 30000;

                request.KeepAlive = true;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                request.Host = "api.nasa.gov";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36";

                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.8,pt;q=0.6,es;q=0.4");
                request.Headers.Add("Cache-Control", "max-age=0");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = new StreamReader(response.GetResponseStream());
                    content = stream.ReadToEnd();
                    stream.Close();
                }
                else content = string.Empty;

                response.Close();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    content = string.Empty;
                }
            }

            return content;
        }

        public void WriteToFile(PictureOfTheDay p)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(PictureOfTheDay));
            TextWriter WriteFileStream = new StreamWriter(@"C:\pic.xml");

            SerializerObj.Serialize(WriteFileStream, p);
            WriteFileStream.Close();
        }

        public PictureOfTheDay ReadFromFile()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(PictureOfTheDay));
            FileStream ReadFileStream = new FileStream(@"C:\pic.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

            PictureOfTheDay p = (PictureOfTheDay)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();

            return p;
        }

        /* Returns true if d1 < d2 or false otherwise */
        public bool CompareDates(string d1, string d2)
        {
            DateTime date1 = Convert.ToDateTime(d1);
            DateTime date2 = Convert.ToDateTime(d2);

            return DateTime.Compare(date1, date2) < 0 ? true : false;
        }

        public void SetWallpaper()
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, @"C:\APOD.jpg", SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public void addToWindowsStartUp()
        {
            string registryValue = "\"" + Application.ExecutablePath.ToString() + "\"";

            RegistryKey r = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            r.SetValue("APOD", Application.ExecutablePath);
        }
    }
}
