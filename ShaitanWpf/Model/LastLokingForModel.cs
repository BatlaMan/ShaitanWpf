using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System;

namespace ShaitanWpf.Model
{
    [Serializable]
    public class LastLokingForModel : INotifyPropertyChanged
    {
        private string title;
        private string performer;
        private string pathToFile;
        private int matching;
        private ImageSource image;
        


        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }
        public int Matching
        {
            get { return matching; }
            set
            {
                matching = value;
                OnPropertyChanged("Matching");
            }
        }
        public string Performer
        {
            get { return performer; }
            set
            {
                performer = value;
                OnPropertyChanged("Performer");
            }
        }
       
        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public string PathToFile
        {
            get { return pathToFile; }
            set
            {
                pathToFile = value;
                OnPropertyChanged("PathToFile");
            }
        }

        public LastLokingForModel(string title, string performer)
        {
            Title = title;
            Performer = performer;
            GoogleImageParser googleImage = new GoogleImageParser(Performer);
            var imgSource = googleImage.GetImageSourse();
            imgSource.Freeze();
            Image = imgSource;
        }

        public LastLokingForModel(string title, string performer,int matched)
        {
            Title = title;
            Performer = performer;
            GoogleImageParser googleImage = new GoogleImageParser(Performer);
            var imgSource = googleImage.GetImageSourse();
            imgSource.Freeze();
            Image = imgSource;
            Matching = matched;
        }

        public LastLokingForModel(string title, string performer,string pathtoFile)
        {
            Title = title;
            Performer = performer;
            PathToFile = pathtoFile;
            GoogleImageParser googleImage = new GoogleImageParser(Performer);
            var imgSource = googleImage.GetImageSourse();
            imgSource.Freeze();
            Image = imgSource;
        } 

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public LastLokingForModel()
        {

        }
    }
}
