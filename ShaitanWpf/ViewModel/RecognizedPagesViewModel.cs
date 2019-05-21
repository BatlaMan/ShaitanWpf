using ShaitanWpf.Model;
using SoundIdentification;
using SoundIdentification.AudioProxy;
using SoundIdentification.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using ShaitanWpf.View;
using ShaitanWpf.Audio;
using Hardcodet;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using Notifications.Wpf;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShaitanWpf.ViewModel
{
    class RecognizedPagesViewModel : INotifyPropertyChanged, IFileDragDropTarget
    {
        private RelayCommand recognizCommand;
        public RelayCommand RecognizCommand
        {
            get
            {
                return recognizCommand;

            }
        }
        private RelayCommand openDevToolsPageCommand;
        public RelayCommand OpenDevToolsPageCommand
        {
            get
            {
                return openDevToolsPageCommand;

            }
        }

        private RelayCommand openLastLokingForPageCommand;
        public RelayCommand OpenLastLokingForPageCommand
        {
            get
            {
                return openLastLokingForPageCommand;

            }
        }



        Page fragPage;
        public Page FragPage
        {
            get { return fragPage; }
            set
            {
                if (fragPage != value)
                {
                    fragPage = value;
                    OnPropertyChanged("FragPage");
                }
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

        Visibility downImageVisibility;
        public Visibility DownImageVisibility
        {
            get { return downImageVisibility; }
            set
            {
                if (downImageVisibility != value)
                {
                    downImageVisibility = value;
                    OnPropertyChanged("DownImageVisibility");
                }
            }
        }
        
        bool recType;
        public bool RecType
        {
            get { return recType; }
            set
            {
                if (recType != value)
                {
                    recType = value;
                    ChangeRecType(recType);
                    OnPropertyChanged("RecType");
                }
            }
        }

        bool recButtonEnable;
        public bool RecButtonEnable
        {
            get { return recButtonEnable; }
            set
            {
                if (recButtonEnable != value)
                {
                    recButtonEnable = value;
                    OnPropertyChanged("RecButtonEnable");
                }
            }
        }

        bool snuggler;
        public bool Snuggler
        {
            get { return snuggler; }
            set
            {
                if (snuggler != value)
                {
                    snuggler = value;
                    ChangeSnugglerType(snuggler);
                    OnPropertyChanged("Snuggler");
                }
            }
        }

        
        public RecognizedPagesViewModel()
        {
            recognizCommand = new RelayCommand(RecognizSound);
            openDevToolsPageCommand = new RelayCommand(OpenDevToolsPage);
            openLastLokingForPageCommand = new RelayCommand(OpenLastLokingForPage);
            DownImageVisibility = Visibility.Hidden;
            recorder = new Recorder();
            FragPage = new MusicPlayerPage();
            RecButtonEnable = true;

            ImageVisibility = Visibility.Hidden;

        }


        string conectionString = "mongodb://localhost";
        private IRecorder recorder;

        private void OpenDevToolsPage(object obj)
        {
            FragPage = new DevToolsPage();
        }

        private void OpenLastLokingForPage(object obj)
        {
            FragPage = new LastLokingFor();
        }

        private bool isRecording = false;
        private async void RecognizSound(object obj)
        {
            if (!isRecording)
            {
                ImageVisibility = Visibility.Visible;
                recorder.OnRecordingAbort += OnRecordingAbort;
                await recorder.StartAsync();
                isRecording = true;
            }
            else
            {
                ImageVisibility = Visibility.Hidden;
                recorder.Stop();
                RecognizeFile(recorder.FilePath);
                isRecording = false;
            }
          
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void OnFileDrop(string[] filepaths)
        {           
            if(System.IO.Path.GetExtension(filepaths[0]) ==".mp3"
                || System.IO.Path.GetExtension(filepaths[0]) == ".wav")
            RecognizeFile(filepaths[0]);
            else MakeNotification("Неверный формат", "Файл имел не верный формат", NotificationType.Success);
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
        private void ChangeRecType(bool type)
        {
            if (type)
            {
                recorder = new LoopBackRecorder();
            }
            else
            {
                recorder = new Recorder();
            }
        }

        private void ChangeSnugglerType(bool type)
        {
            if (type)
            {
                RecButtonEnable = false;
                var notificationManager = new NotificationManager();

                notificationManager.Show(new NotificationContent
                {
                    Title = "Внимание",
                    Message = "Вы включили режим фоновое распознование музыки. В это режиме доступ к обычному распознованию заблокирован",
                    Type = NotificationType.Information
                });
                SnugglerRecognizeAsync();
            }
            else
            {
                RecButtonEnable = true;
            }
        }


        private async void SnugglerRecognizeAsync()
        {
            await Task.Run(() => SnugglerRecognize());
        }

        IRecorder recorderSnuggler ;
        private bool isRecordingSunn = false;
        private async void SnugglerRecognize()
        {
            recorderSnuggler = new LoopBackRecorder();
            recorderSnuggler.Silent_Sec = 43;
            recorderSnuggler.OnRecordingAbort += OnRecordingSnugglerAbort;
            while (true)
            {
                if (!isRecordingSunn)
                {
                    await recorderSnuggler.StartAsync();
                    isRecordingSunn = !isRecordingSunn;
                }
                if (!Snuggler)
                {
                    isRecordingSunn = false;
                    break;
                }
            }
            recorderSnuggler.Stop();
        }

        private async void RecognizeFile(string filePath)
        {
            DownImageVisibility = Visibility.Visible;
            IAudioService audioService = new NAudioService();
            IDataStorage dataStorage = new MongoDatabaseHandler(conectionString, "Shaitan",
           "Songs", "Hash");
            var result = await QueryCommandBuilder.Instance.BuildQueryCommand()
             .From(filePath)
             .UsingServices(dataStorage, audioService)
             .Query();
            PushResultOrNoResultPape(result);
        } 

        private void PushResultOrNoResultPape(QueryResult queryResult)
        {
            if (queryResult.BestMath != null)
                FragPage = new ResultPage(queryResult);
            else FragPage = new NoResultPage();
            DownImageVisibility = Visibility.Hidden;
        }
      
        private void OnRecordingAbort(AbortType abortType)
        {
            
            ImageVisibility = Visibility.Hidden;
            isRecording = false;
            Application.Current.Dispatcher.Invoke((Action)delegate {
                RecognizeFile(recorder.FilePath);
            });

        }
        Song prevMatch;
        bool isNoty;
        private async void OnRecordingSnugglerAbort(AbortType abortType)
        {

            if(abortType == AbortType.TimeOut)
            {
                IAudioService audioService = new NAudioService();
                IDataStorage dataStorage = new MongoDatabaseHandler(conectionString, "Shaitan",
               "Songs", "Hash");
                var result = await QueryCommandBuilder.Instance.BuildQueryCommand()
                 .From(recorderSnuggler.FilePath)
                 .UsingServices(dataStorage, audioService)
                 .Query();
                if (result.BestMath != null)
                {

                    var notificationManager = new NotificationManager();

                    notificationManager.Show(new NotificationContent
                    {
                        Title = result.BestMath.Title,
                        Message = result.BestMath.Artist,
                        Type = NotificationType.Success
                    });
                    prevMatch = result.BestMath;
                    isNoty = true;
                }
            }     
                isRecordingSunn = !isRecordingSunn;
           
        }

      
    }
}
