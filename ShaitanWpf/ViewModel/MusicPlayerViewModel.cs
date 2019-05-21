using ShaitanWpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SoundIdentification;
using System.Windows.Media;
using MaterialDesignThemes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ControlzEx;
using System.Windows.Threading;
using System.Threading;
using System.Xml.Serialization;
using Notifications.Wpf;

namespace ShaitanWpf.ViewModel
{
    class MusicPlayerViewModel: INotifyPropertyChanged, IFileDragDropTarget
    {
        private MediaPlayer player = new MediaPlayer();
        private ObservableCollection<LastLokingForModel> cards
            = new AsyncObservableCollection<LastLokingForModel>();
        public DispatcherTimer _currentTimeUpdateTimer;
        public ObservableCollection<LastLokingForModel> Cards
        {
            get { return cards; }
            set
            {
                cards = value;
                OnPropertyChanged("Cards");
            }
        }
       
        LastLokingForModel selectedMusic;
        public LastLokingForModel SelectedMusic
        {
            get { return selectedMusic; }
            set
            {
                if (selectedMusic != value)
                {
                    selectedMusic = value;
                    ChangePlayingMusic();
                    OnPropertyChanged("SelectedMusic");
                }
            }
        }

        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return playPauseCommand;
            }
        }

        private RelayCommand repeatCommand;
        public RelayCommand RepeatCommand
        {
            get
            {
                return repeatCommand;
            }
        }

        private RelayCommand prevTrackCommand;
        public RelayCommand PrevTrackCommand
        {
            get
            {
                return prevTrackCommand;
            }
        }
        private RelayCommand nextTrackCommand;
        public RelayCommand NextTrackCommand
        {
            get
            {
                return nextTrackCommand;
            }
        }
        private RelayCommand savePlayListCommand;
        public RelayCommand SavePlayListCommand
        {
            get
            {
                return savePlayListCommand;
            }
        }
        private RelayCommand loadPlayListCommand;
        public RelayCommand LoadPlayListCommand
        {
            get
            {
                return loadPlayListCommand;
            }
        }

        
        string nowPlayingPerformer;
        public string NowPlayingPerformer
        {
            get { return nowPlayingPerformer; }
            set
            {               
                if (nowPlayingPerformer != value)
                {
                    nowPlayingPerformer = value;
                    OnPropertyChanged("NowPlayingPerformer");
                }
            }
        }

        string nowPlayingTitle;
        public string NowPlayingTitle
        {
            get { return nowPlayingTitle; }
            set
            {
                if (nowPlayingTitle != value)
                {
                    nowPlayingTitle = value;
                    OnPropertyChanged("NowPlayingTitle");
                }
            }
        }

        ImageBrush playingPerformerImage;
        public ImageBrush PlayingPerformerImage
        {
            get { return playingPerformerImage; }
            set
            {
                if (playingPerformerImage != value)
                {
                    playingPerformerImage = value;
                    OnPropertyChanged("PlayingPerformerImage");
                }
            }
        }


        double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    player.Volume = (double)volume / 100;
                    OnPropertyChanged("Volume");
                }
            }
        }

        double maximum;
        public double Maximum
        {
            get { return maximum; }
            set
            {
                if (maximum != value)
                {
                    maximum = value;
                    ChangeMusicPos(curPos);
                    OnPropertyChanged("Maximum");
                }
            }
        }

        int curPos;
        public int CurPos
        {
            get { return curPos; }
            set
            {
                if (curPos != value)
                {
                    curPos = value;
                    ChangeMusicPos(curPos);
                    OnPropertyChanged("CurPos");
                }
            }
        }


        PackIconKind playOrPauseIcon;
        public PackIconKind PlayOrPauseIcon
        {
            get { return playOrPauseIcon; }
            set
            {
                if (playOrPauseIcon != value)
                {
                    playOrPauseIcon = value;
                    OnPropertyChanged("PlayOrPauseIcon");
                }
            }
        }

        
        public MusicPlayerViewModel()
        {
            playPauseCommand = new RelayCommand(PlayOrPauseMusic);
            repeatCommand = new RelayCommand(RepeatSong);
            nextTrackCommand = new RelayCommand(NextTrack);
            prevTrackCommand = new RelayCommand(PrevTrack);
            savePlayListCommand = new RelayCommand(savePlayListC);
            loadPlayListCommand = new RelayCommand(loadPlayListC);
            Volume = 100;
            PlayOrPauseIcon = PackIconKind.Play;
        }

        private void savePlayListC(object obj)
        {
            SavePlayListAsync();
        }
        private void loadPlayListC(object obj)
        {
            LoadPlayListAsync();
        }

        private void NextTrack(object obj)
        {
            ChangeTrackToNext();
        }
        private void PrevTrack(object obj)
        {
            ChangeTrackToPrev();
        }

        private async void LoadImageForCardAsync(string[] songs)
        {
            await Task.Run(() => LoadImageForCard(songs));
        }

        private void LoadImageForCard(string[] songs)
        {
            for (int i = 0; i < songs.Length; i++)
            {
                if(File.Exists(songs[i]))
                {
                    Song song = new Song(songs[i]);
                    if(song.Pictures.Length != 0 && song.Pictures != null)
                    {
                        MemoryStream ms = new MemoryStream(song.Pictures[0].Data.Data);
                        Cards.Add(new LastLokingForModel(song.Title, song.Artist,songs[i],ms));
                    }
                    else
                    {
                        Cards.Add(new LastLokingForModel(song.Title, song.Artist, songs[i]));
                    }
                }
            }
            return;
        }


        private void ChangePlayingMusic()
        {
            
            PlayingPerformerImage = new ImageBrush(selectedMusic.Image);
            NowPlayingPerformer = selectedMusic.Performer;
            NowPlayingTitle = selectedMusic.Title;
        }

        private void RepeatSong(object obj)
        {
            player.Stop();
            player.Play();
        }
        
        
        bool isPlaying = false;
        private void PlayOrPauseMusic(object obj)
        {
            if (!isPlaying)
            { 
                if(cards.Count !=0 )
                {
                    PlayOrPauseIcon = PackIconKind.Pause;
                    pos = cards.Select((x, i) => new { Element = x, Index = i })
                        .First(x => x.Element == SelectedMusic).Index;
                    PlayMusic();
                    isPlaying = !isPlaying;
                    _currentTimeUpdateTimer = new DispatcherTimer();
                    _currentTimeUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
                    _currentTimeUpdateTimer.Tick += new EventHandler(_currentTimeUpdateTimer_Tick);
                    _currentTimeUpdateTimer.Start();
                }           
            }
            else
            {
                
                PlayOrPauseIcon = PackIconKind.Play;
                isPlaying = !isPlaying;
                PauseMusic();
            }
        }

        int pos = 0;
        void _currentTimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            CurPos = (int)player.Position.TotalSeconds;
            if(CurPos == 100)
            {
              ChangeTrackToNext();
            }
        }
       
        void ChangeMusicPos(int curPos)
        {
            
            if(Math.Abs(player.Position.TotalSeconds - curPos) > 3)
            player.Position = new TimeSpan(0, 0, curPos);
        }

        private void ChangeTrackToPrev()
        {
            pos--;
            if (pos <= 0)
            {
                pos++;
            }
            else
            {
                SelectedMusic = cards[pos];
                PlayMusic(cards[pos].PathToFile);
            }
        }

        private void ChangeTrackToNext()
        {
            pos++;
            if(pos >= cards.Count)
            {
                pos--;
            }
            else
            {
                SelectedMusic = cards[pos];
                PlayMusic(cards[pos].PathToFile);
            }
        }

        private void PlayMusic()
        {
            player.Open(new Uri(SelectedMusic.PathToFile, UriKind.Relative));
            player.MediaOpened += MediaOpened;     
            player.Play();
        }

        private void PlayMusic(string filePath)
        {
            player.Open(new Uri(filePath, UriKind.Relative));    
             
            player.MediaOpened += MediaOpened;
            player.Play();
        }

        private void MediaOpened(object sender, EventArgs e)
        {

            if (player.NaturalDuration.HasTimeSpan)
                Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void PauseMusic()
        {   
            player.Pause();
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
                {
                    MakeNotification("Неверный формат", "Файл какой-то из файлом имел не вернный формат"
                        , NotificationType.Warning);
                    return;
                }
            }
                LoadImageForCardAsync(filepaths);
        }
        private void MakeNotification(string title, string message, NotificationType notification)
        {
            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = notification
            });
        }

        private async void SavePlayListAsync()
        {
            await Task.Run(() => SavePlayList());
        }

        private void SavePlayList()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<LastLokingForModel>));
           
            using (FileStream fs = new FileStream("Saves\\PlayList.xml", FileMode.OpenOrCreate))
            {     
                formatter.Serialize(fs, cards);
                Console.WriteLine("Объект сериализован");
            }
        }

        private async void LoadPlayListAsync()
        {
            await Task.Run(() => LoadLastQuery());
        }
     
        public void LoadLastQuery()
        {
            if(File.Exists("Saves\\lastQ.xml"))
            using (FileStream fs = new FileStream("Saves\\lastQ.xml", FileMode.Open))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<LastQuery>));
                cards = (AsyncObservableCollection<LastLokingForModel>)formatter.Deserialize(fs);
             
            }
        }

    }
}
