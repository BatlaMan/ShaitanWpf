using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaitanWpf.Audio
{
    public interface IRecorder
    {
        string FilePath { get; }
        int Time_Out_Sec { get ; set ; }
        int Silent_Sec { get; set ; }
        event Action<AbortType> OnRecordingAbort;
        void Start();
        Task StartAsync();
        void Stop();

    }
}
