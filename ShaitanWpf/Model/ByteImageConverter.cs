using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShaitanWpf.Model
{
    public class ByteImageConverter
    {
        public static ImageSource ByteToImage(byte[] imageData)
        {
            MemoryStream ms = new MemoryStream(imageData);
            BitmapImage biImg = new BitmapImage();
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }
    }
}
