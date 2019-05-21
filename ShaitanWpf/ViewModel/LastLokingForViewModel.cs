using ShaitanWpf.Model;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ShaitanWpf.ViewModel
{
    class LastLokingForViewModel: INotifyPropertyChanged
    {
        
        private ObservableCollection<LastLokingForModel> cards 
            = new AsyncObservableCollection<LastLokingForModel>();
        public ObservableCollection<LastLokingForModel> Cards
        {
            get { return cards; }
            set
            {
                cards = value;
                OnPropertyChanged("Cards");
            }
        }


        public LastLokingForViewModel()
        {
            LoadImageForCardAsync();
        }

        private async void LoadImageForCardAsync()
        { 
            await Task.Run(() => LoadImageForCard());
        }

        private void LoadImageForCard()
        {
            var temp = DeserializerListOfLastQ();
            if (temp != null)
            {
                foreach (var item in temp)
                {
                    Cards.Add(new LastLokingForModel(item.Title, item.Performer));

                }
            }
            else
            {
                Cards.Add(new LastLokingForModel("Ничего не найденно", ""));
            }
              

            return;
        }

        private List<LastQuery> DeserializerListOfLastQ()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<LastQuery>));
            using (FileStream fs = new FileStream("Saves\\lastQ.xml", FileMode.Open))
            {
                List<LastQuery> newpeople = (List<LastQuery>)formatter.Deserialize(fs);
                return newpeople;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
