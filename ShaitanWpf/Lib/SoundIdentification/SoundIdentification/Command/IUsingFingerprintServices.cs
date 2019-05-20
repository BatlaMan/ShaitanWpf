using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.Command
{
    public interface IUsingFingerprintServices
    {
        IUsingStorageType UsingAudioReaderServices(IAudioService audioService);
    }
}
