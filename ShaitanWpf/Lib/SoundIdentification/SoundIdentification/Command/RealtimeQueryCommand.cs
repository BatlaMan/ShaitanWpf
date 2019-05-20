using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundIdentification.Command
{
    class RealtimeQueryCommand : IRealtimeSource, IUsingRealtimeQueryServices, IRealtimeQueryCommand
    {

        IDataStorage dataStorage;
        BlockingCollection<float[]> realtimeCollection;
        private const int MinSamplesForOneFingerprint = 81928;
        private const int SupportedFrequency = 44100;
        private const int MillisecondsDelay = (int)((double)MinSamplesForOneFingerprint / SupportedFrequency * 1000);

        public IUsingRealtimeQueryServices From(BlockingCollection<float[]> realtimeCollection)
        {
            this.realtimeCollection = realtimeCollection;
            return this;
        }

        public IRealtimeQueryCommand UsingServices(IDataStorage modelService)
        {
            dataStorage = modelService;
            return this;
        }

        public async Task<double> Query(CancellationToken cancellationToken)
        {
            return await QueryAndHash(cancellationToken);
        }

        private async Task<double> QueryAndHash(CancellationToken cancellationToken)
        {
            double queryLength = 0d;
            Dictionary<int, Dictionary<int, int>> tmpMap = new Dictionary<int, Dictionary<int, int>>();
            while (!realtimeCollection.IsAddingCompleted && !cancellationToken.IsCancellationRequested)
            {
                float[] audioSamples;

                try
                {
                    if (!realtimeCollection.TryTake(out audioSamples, MillisecondsDelay, cancellationToken))
                    {
                        continue;
                    }
                }
                catch (OperationCanceledException)
                {
                    return queryLength;
                }
                MusicSpectrum musicSpectrum = new MusicSpectrum(dataStorage);
                int totalSize = audioSamples.Length;

                int amountPossible = totalSize / 4096;

                tmpMap = musicSpectrum.FFTAnalys(tmpMap, audioSamples, amountPossible);
                var q = musicSpectrum.Matching(tmpMap);
                Console.WriteLine(q.BestMath);      
            }
            return queryLength;
        }
    }
}
