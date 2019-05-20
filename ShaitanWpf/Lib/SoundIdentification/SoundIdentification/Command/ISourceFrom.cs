using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.Command
{
    public interface ISourceFrom
    {
        IUsingFingerprintServices From(string pathToAudioFile);

        IUsingFingerprintServices From(float[] audioSamples,Song song);
    }
}
