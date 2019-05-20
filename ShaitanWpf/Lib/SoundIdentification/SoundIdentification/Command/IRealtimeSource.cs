using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.Command
{
    public interface IRealtimeSource
    {
        IUsingRealtimeQueryServices From(BlockingCollection<float[]> audioSamples);
    }
}
