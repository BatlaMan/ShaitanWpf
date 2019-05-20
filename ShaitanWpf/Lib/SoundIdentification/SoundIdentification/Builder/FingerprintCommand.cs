using SoundIdentification.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SoundIdentification.Builder
{
    class FingerprintCommand : ISourceFrom, IFingerprintCommand, IUsingFingerprintServices, IUsingStorageType
    {

        private Action createFingerprintsMethod;
        IDataStorage dataStorageToUse;
        IAudioService audioServiceToUse;

        public IUsingFingerprintServices From(string pathToAudioFile)
        {
            createFingerprintsMethod = () =>
            {
                MusicSpectrum music = new MusicSpectrum(dataStorageToUse);
                music.InsertSongToDB(audioServiceToUse, pathToAudioFile);
            };
            return this;
        }

        public IUsingFingerprintServices From(float[] audioSamples,Song song)
        {
            createFingerprintsMethod = () =>
            {
                MusicSpectrum music = new MusicSpectrum(dataStorageToUse);
                music.InsertSongToDB(audioSamples, song);
            };
            return this;
        }

        public IUsingStorageType UsingAudioReaderServices(IAudioService audioServiceToUse)
        {
            this.audioServiceToUse = audioServiceToUse;
            return this;
        }

        public IFingerprintCommand UsingStorage(IDataStorage dataStorageToUse)
        {
            this.dataStorageToUse = dataStorageToUse;
            return this;
        }

        public Task Add()
        {
            return Task.Factory.StartNew(createFingerprintsMethod);
        }
    }
}
