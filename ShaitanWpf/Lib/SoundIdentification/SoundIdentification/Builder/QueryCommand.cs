using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SoundIdentification.Command;
namespace SoundIdentification.Builder
{
    class QueryCommand : IQuerySource, IUsingQueryServices, IUsingQueryModelService, IQueryCommand
    {

        private Func<QueryResult> createQueryMethod;
        IDataStorage dataStorageToUse;
        IAudioService audioServiceToUse;

        public IUsingQueryServices From(string pathToAudioFile)
        {
            createQueryMethod = () =>
            {
                MusicSpectrum music = new MusicSpectrum(dataStorageToUse);
                return music.SeekSong(audioServiceToUse, pathToAudioFile);
                 
            };
            return this;
        }

        public IUsingQueryModelService From(float[] audioSamples)
        {
            createQueryMethod = () =>
            {
                MusicSpectrum music = new MusicSpectrum(dataStorageToUse);
                return music.SeekSong(audioSamples);

            };
            return this;
        }

       

        public IQueryCommand UsingModelService(IDataStorage dataStorageToUse)
        {
             this.dataStorageToUse = dataStorageToUse;
             return this;
        }

        public IQueryCommand UsingServices(IDataStorage dataStorageToUse, IAudioService audioServiceToUse)
        {
             this.dataStorageToUse = dataStorageToUse;
             this.audioServiceToUse = audioServiceToUse;
             return this;
        }

        public Task<QueryResult> Query()
        {
            return Task.Factory.StartNew(createQueryMethod);
        }
    }
}
