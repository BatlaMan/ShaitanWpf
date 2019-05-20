using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification
{
    public interface IAudioService
    {
        float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond);
        float[] ReadMonoFromFile(string filename, int samplerate);
    }
}
