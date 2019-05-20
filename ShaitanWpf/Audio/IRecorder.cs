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
        event Action OnRecordingAbort;
        void Start();
        Task StartAsync();
        void Stop();

    }
}
