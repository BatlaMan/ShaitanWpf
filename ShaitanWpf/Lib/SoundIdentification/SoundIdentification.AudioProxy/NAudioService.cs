using System;
using NAudio.MediaFoundation;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SoundIdentification.AudioProxy
{
    public class NAudioService : IAudioService
    {
        private const int Mono = 1;
        public NAudioService()
        {
            downSamplingQuality = 25;
        }
        private readonly int downSamplingQuality;
        public float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond)
        {
            SamplesAggregator samplesAggregator = new SamplesAggregator();
            using (var stream = GetStream(filename))
            {
                SeekToSecondInCaseIfRequired(startmillisecond, stream);
                using (var resampler = GetResampler(stream, samplerate, Mono, downSamplingQuality))
                {
                    var waveToSampleProvider = new WaveToSampleProvider(resampler);
                    return samplesAggregator.ReadSamplesFromSource(new NAudioSamplesProviderAdapter(waveToSampleProvider), milliseconds, samplerate);
                }
            }
        }
        public WaveStream GetStream(string pathToAudioFile)
        {
            // This class assumes media foundation libraries are installed on target machine
            // In case you are running on Azure (Windows Server 2012) install Server Media Foundation feature
            return new MediaFoundationReader(pathToAudioFile);
        }

        public float[] ReadMonoFromFile(string filename, int samplerate)
        {
            return ReadMonoFromFile(filename, samplerate, 0, 0);
        }

        private void SeekToSecondInCaseIfRequired(double startAtSecond, WaveStream stream)
        {
            if (startAtSecond > 0)
            {
                stream.CurrentTime = stream.CurrentTime.Add(TimeSpan.FromSeconds(startAtSecond));
            }
        }
        public MediaFoundationTransform GetResampler(WaveStream streamToResample, int sampleRate, int numberOfChannels, int resamplerQuality)
        {
            return new MediaFoundationResampler(streamToResample, GetWaveFormat(sampleRate, numberOfChannels))
            {
                ResamplerQuality = resamplerQuality
            };
        }
        public WaveFormat GetWaveFormat(int sampleRate, int numberOfChannels)
        {
            return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, numberOfChannels);
        }

    }
}
