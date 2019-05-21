using Notifications.Wpf;
using ShaitanWpf.Model;
using SoundIdentification;
using SoundIdentification.AudioProxy;
using SoundIdentification.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ShaitanWpf.ViewModel
{
    class DevToolsViewModel: INotifyPropertyChanged, IFileDragDropTarget
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


        private RelayCommand startHashingCommand;
        public RelayCommand StartHashingCommand
        {
            get
            {
                return startHashingCommand;
            }
        }

        Visibility imageVisibility;
        public Visibility ImageVisibility
        {
            get { return imageVisibility; }
            set
            {
                if (imageVisibility != value)
                {
                    imageVisibility = value;
                    OnPropertyChanged("ImageVisibility");
                }
            }
        }
        bool isDropAllow;
        public bool IsDropAllow
        {
            get { return isDropAllow; }
            set
            {
                if (isDropAllow != value)
                {
                    isDropAllow = value;
                    OnPropertyChanged("IsDropAllow");
                }
            }
        }
        bool isHashingBtnEnable;
        public bool IsHashingBtnEnable
        {
            get { return isHashingBtnEnable; }
            set
            {
                if (isHashingBtnEnable != value)
                {
                    isHashingBtnEnable = value;
                    OnPropertyChanged("IsHashingBtnEnable");
                }
            }
        }

        int progressValue;
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (progressValue != value)
                {
                    progressValue = value;
                    OnPropertyChanged("ProgressValue");
                }
            }
        }

        
        public DevToolsViewModel()
        {
            IsDropAllow = true;
            IsHashingBtnEnable = true;
            ProgressValue = 0;
            ImageVisibility = Visibility.Hidden;
            startHashingCommand = new RelayCommand(StartHashing);
        }

        private int dx = 0;
        private void StartHashing(object obj)
        {
            if (cards.Count > 0 && cards!= null)
            {
                dx = 100 / cards.Count;
                IsDropAllow = false;
                IsHashingBtnEnable = false;
                ImageVisibility = Visibility.Visible;
                HashAndQueryAsync();
            }
        }

        private async void HashAndQueryAsync()
        {
            await Task.Run(() => HashAndQuery());
        }
        string conectionString = "mongodb://localhost";
        private async void HashAndQuery()
        {

            foreach (var item in cards)
            {
                IAudioService audioService = new NAudioService();
                IDataStorage dataStorage = new MongoDatabaseHandler(conectionString, "Shaitan",
               "Songs", "Hash");
                if (File.Exists(item.PathToFile))
                {
                    try
                    {
                      await FingerprintCommandBuilder.Instance.BuildFingerprintCommand()
                     .From(item.PathToFile)
                     .UsingAudioReaderServices(audioService)
                     .UsingStorage(dataStorage)
                     .Add();
                    }
                    catch (Exception ex)
                    {
                        MakeNotification("Ошибка", $"Ошибка типа {ex.Message}. Добавление треков отмененно ", NotificationType.Error);
                        cards.Clear();
                        IsDropAllow = true;
                        IsHashingBtnEnable = true;
                        ImageVisibility = Visibility.Hidden;
                        return;
                    }
                    
                    MakeNotification("Добавлен " + item.Title, $"Песня  {item.Title} {item.Performer}  успешна добавленна ", NotificationType.Information);
                    ProgressValue += dx;
                }
                else
                {
                    MakeNotification("Файл не найден", $"Путь к песне {item.Title} {item.Performer} не найден. Не беспокойтесь добавлнение остальных треков будет продолженно ", NotificationType.Warning);
                    ProgressValue += dx;
                }
                
            }
            MakeNotification("Работа завершена", "Все успешно добавленно", NotificationType.Success);
            cards.Clear();
            IsDropAllow = true;
            IsHashingBtnEnable = true;
            ImageVisibility = Visibility.Hidden;
        }

        private void MakeNotification(string title,string message, NotificationType notification)
        {
            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = notification
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void OnFileDrop(string[] filepaths)
        {
            for (int i = 0; i < filepaths.Length; i++)
            {
                if (Path.GetExtension(filepaths[i]) != ".mp3"
                               && Path.GetExtension(filepaths[i]) != ".wav")
                    return;
            }
            LoadImageForCardAsync(filepaths);
        }

        private async void LoadImageForCardAsync(string[] songs)
        {
            await Task.Run(() => LoadImageForCard(songs));
        }

        private void LoadImageForCard(string[] songs)
        {
            for (int i = 0; i < songs.Length; i++)
            {
                if (File.Exists(songs[i]))
                {
                    Song song = new Song(songs[i]);
                    Cards.Add(new LastLokingForModel(song.Title, song.Artist, songs[i]));
                }
            }
            return;
        }

    }
}
