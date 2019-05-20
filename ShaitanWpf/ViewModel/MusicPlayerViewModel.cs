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

namespace ShaitanWpf.ViewModel
{
    class MusicPlayerViewModel: INotifyPropertyChanged, IFileDragDropTarget
    {
        private MediaPlayer player = new MediaPlayer();
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
            PlayOrPauseIcon = PackIconKind.Play;
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
                    Cards.Add(new LastLokingForModel(song.Title, song.Artist, songs[i]));
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


        bool isPlaying = false;
        private void PlayOrPauseMusic(object obj)
        {
            if (isPlaying)
            {

                PlayOrPauseIcon = PackIconKind.Pause;
                PlayMusic();
                isPlaying = !isPlaying;
            }
            else
            {
                PlayOrPauseIcon = PackIconKind.Play;
                isPlaying = !isPlaying;
                PauseMusic();
            }
        }

        private void PlayMusic()
        {
            player.Open(new Uri(SelectedMusic.PathToFile, UriKind.Relative));
            player.Play();
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
            LoadImageForCardAsync(filepaths);
        }
    }
}
