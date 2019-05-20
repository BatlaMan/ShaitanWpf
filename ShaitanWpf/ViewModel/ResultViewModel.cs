using ShaitanWpf.Model;
using SoundIdentification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;

namespace ShaitanWpf.ViewModel
{
    class ResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string songName;
        private string performer;
        private ImageSource imageSourceFromByte;
        public string SongName
        {
            get { return songName; }
            set
            {

                if (songName != value)
                {
                    songName = value;
                    OnPropertyChanged("SongName");
                }
            }
        }
        public ImageSource ImageSourceFromByte
        {
            get { return imageSourceFromByte; }
            set
            {
                
                if (imageSourceFromByte != value)
                {
                    imageSourceFromByte = value;
                    OnPropertyChanged("ImageSourceFromByte");
                }
            }
        }
        public string Performer
        {
            get { return performer; }
            set
            {
                if (performer != value)
                {
                    performer = value;
                    OnPropertyChanged("Performer");
                }
            }
        }
        public ResultViewModel(QueryResult query)
        {
            SongName = query.BestMath.Title;
            Performer = query.BestMath.Artist;
            GoogleImageParser googleImage = new GoogleImageParser(Performer);

            ImageSourceFromByte = googleImage.GetImageSourse();
            SaveLastQueryAsync(query);
        }

        public async void SaveLastQueryAsync(QueryResult query)
        {
            await Task.Run(() => SaveLastQuery(query));
        }

        public void SaveLastQuery(QueryResult query)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<LastQuery>));
            List<LastQuery> temp;
            if (!File.Exists("Saves\\lastQ.xml"))
                temp = LoadLastQuery();
            else temp = new List<LastQuery>();
            using (FileStream fs = new FileStream("Saves\\lastQ.xml", FileMode.OpenOrCreate))
            {
                temp.Add(new LastQuery(query));
                formatter.Serialize(fs, temp);
                Console.WriteLine("Объект сериализован");
            }
        }

        public List<LastQuery> LoadLastQuery()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<LastQuery>));
            using (FileStream fs = new FileStream("Saves\\lastQ.xml", FileMode.Open))
            {
                List<LastQuery> deserilizeQ = (List<LastQuery>)formatter.Deserialize(fs);

                return deserilizeQ;
            }
        }


        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
