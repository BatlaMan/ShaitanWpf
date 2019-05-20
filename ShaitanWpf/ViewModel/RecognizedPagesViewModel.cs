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

namespace ShaitanWpf.ViewModel
{
    class RecognizedPagesViewModel : INotifyPropertyChanged,IFileDragDropTarget
    {
        private RelayCommand recognizCommand;
        public RelayCommand RecognizCommand
        {
            get
            {
                return recognizCommand;
                      
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

        public RecognizedPagesViewModel()
        {
            recognizCommand = new RelayCommand(RecognizSound);
            recorder = new Recorder();
            FragPage = new MusicPlayerPage();
            ImageVisibility = Visibility.Hidden;
        }
        string conectionString = "mongodb://localhost";
        private IRecorder recorder;
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
            RecognizeFile(filepaths[0]);
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

        private async void RecognizeFile(string filePath)
        {
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
        }
      
        private void OnRecordingAbort()
        {
            
            ImageVisibility = Visibility.Hidden;
            isRecording = false;
            Application.Current.Dispatcher.Invoke((Action)delegate {
                RecognizeFile(recorder.FilePath);
            });


        }
    }
}
