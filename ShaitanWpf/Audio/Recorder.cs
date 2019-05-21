using System;
using System.Collections.Concurrent;
using NAudio.Wave;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace ShaitanWpf.Audio
{
    class Recorder : IRecorder
    {
        public WaveInEvent waveSource = null;
        public WaveFileWriter waveFile = null;

        public BlockingCollection<float[]> RealTimeCollider { get; set; }
        public string FilePath { get => filePath; }
        public int Time_Out_Sec { get => time_Out_Sec; set => time_Out_Sec = value; }
        public int Silent_Sec { get => silent_Sec; set => silent_Sec = value; }

        private string filePath;
        private IProcessData processData;
        private Timer timer;
        private int secFromTimeOut = 0; 
        private int secFromSilent = 0;
        private int time_Out_Sec;
        private int silent_Sec;
        public event Action<AbortType> OnRecordingAbort;

        public Recorder():this
         (Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dat.wav")
            ,new AudioDataProcesser())
        {
          
        }
        public Recorder(string filePath,IProcessData processData)
        {
            this.filePath = filePath;
            this.processData = processData;
            
            time_Out_Sec = 60;
            silent_Sec = 2;
        }

        

        //Получение данных из входного буфера 
        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {

            if (waveFile != null)
            {
                if (secFromTimeOut < Time_Out_Sec)
                {
                    waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                    waveFile.Flush();
                    if (processData.ProcessData(e))
                    {
                        secFromSilent = 0;
                        
                    }
                    else if (secFromSilent > Silent_Sec)
                    {
                        Abort(AbortType.Silent);
                    }
                }
                else Abort(AbortType.TimeOut);
                
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }

        }

        public void Start()
        {
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(44100, 16, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
            waveFile = new WaveFileWriter(filePath, waveSource.WaveFormat);
            waveSource.StartRecording();

            secFromTimeOut = 0;
            secFromSilent = 0;
            TimerCallback tm = new TimerCallback(Count);
          
             int sec = 0;
             timer = new Timer(tm, sec, 0, 1000);

        }
        public void Count(object obj)
        {
            secFromSilent++;
            secFromTimeOut++; 
        }
        public void Stop()
        {
            if (waveSource == null)
            {
                return;
            }
            waveSource.StopRecording();
            Thread.Sleep(500);
            // RealTimeCollider.CompleteAdding();
        }

        public Task StartAsync()
        {
            return Task.Run(() => Start());
        }
        private void Abort(AbortType abortType)
        {
            Stop();
            OnRecordingAbort?.Invoke(abortType);
        }
    }
}
