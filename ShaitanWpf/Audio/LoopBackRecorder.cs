using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShaitanWpf.Audio
{
    class LoopBackRecorder: IRecorder
    {
        private IWaveIn _waveIn;
        private WaveFileWriter _writer;
        private bool _isRecording = false;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public LoopBackRecorder() : this
         (Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dat.wav")
            , new AudioDataProcesser())
        {

        }
        public LoopBackRecorder(string filePath, IProcessData processData)
        {
            this.filePath = filePath;
            this.processData = processData;
            time_Out_Sec = 15;
            silent_Sec = 2;
        }

        /// <summary>
        /// Starts the recording.
        /// </summary>
        public void Start()
        {
            // If we are currently record then go ahead and exit out.
            if (_isRecording == true)
            {
                return;
            }
            _waveIn = new WasapiLoopbackCapture();
            _writer = new WaveFileWriter(FilePath, _waveIn.WaveFormat);
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
            _waveIn.StartRecording();
            _isRecording = true;
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
            _waveIn.StopRecording();
            Thread.Sleep(500);
            // RealTimeCollider.CompleteAdding();
        }
        /// <summary>
        /// Stops the recording
        /// </summary>
        public void StopRecording()
        {
            if (_waveIn == null)
            {
                return;
            }
            _waveIn.StopRecording();
        }

        /// <summary>
        /// Event handled when recording is stopped.  We will clean up open objects here that are required to be 
        /// closed and/or disposed of.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            // Writer Close() needs to come first otherwise NAudio will lock up.
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
            if (_waveIn != null)
            {
                _waveIn.Dispose();
                _waveIn = null;
            }
            _isRecording = false;
            if (e.Exception != null)
            {
                throw e.Exception;
            }
        } // end void OnRecordingStopped

        /// <summary>
        /// Event handled when data becomes available.  The data will be written out to disk at this point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
                _writer.Write(e.Buffer, 0, e.BytesRecorded);
            if (secFromTimeOut < time_Out_Sec)
            {
                if (processData.ProcessData(e))
                {
                    secFromSilent = 0;
                }
                else if (secFromSilent > silent_Sec)
                {
                    Abort(AbortType.Silent);
                }
            }
            else Abort(AbortType.TimeOut);
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
