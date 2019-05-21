using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ShaitanWpf.Model;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShaitanWpf.Model
{
    class GoogleImageParser
    {
        string imageName = string.Empty;
        string target = "'Target' AND ( исполнитель OR группа OR музыка OR music OR band OR artist ) AND (фото | photo)";
        public GoogleImageParser(string imageName)
        {
            if (imageName == null || imageName == string.Empty)
            {
                imageName = "default image";
                target = "Target";
            }
            this.imageName = imageName;
        }

        public MemoryStream GetMemoryImage()
        {

            string html = GetHtmlCode(imageName);
            if (html == null || html == "")
                return new MemoryStream();
            string urls = GetUrls(html);
            if (urls == null || urls == "")
                return new MemoryStream();
            byte[] image = GetBytedImage(urls);
            if (image == null || image.Length == 0)
                return new MemoryStream();
            return new MemoryStream(image);
        }

        public ImageSource GetImageSourse()
        {

            string html = GetHtmlCode(imageName);
            if (html == null )
                return new BitmapImage() ;
            string urls = GetUrls(html);
            string luckyUrl = urls;
            byte[] image = GetBytedImage(luckyUrl);
            return ByteImageConverter.ByteToImage(image);
        }
        private string GetHtmlCode(string imageName)
        {


            string topic = target.Replace("Target", imageName);

            string url = "https://www.google.com/search?q=" + topic + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception)
            {

                return null;
            }

            
            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private string GetUrls(string html)
        {
            var urls = string.Empty;

                int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls =url;
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
           
            return urls;
        }
        private byte[] GetBytedImage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = null;
            try
            {
                 response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception)
            {
                return new byte[0];
            }

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return null;
                using (var sr = new BinaryReader(dataStream))
                {
                    byte[] bytes = sr.ReadBytes(100000000);

                    return bytes;
                }
            }
        }
    }
}
