using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.AudioProxy
{
    interface ISamplesProvider
    {
        int GetNextSamples(float[] buffer);
    }
}
