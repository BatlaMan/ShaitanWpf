using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoundIdentification
{
    [Obsolete("Используйте NaudioService или BassProxy")]
    public class AudioService : IAudioService
    {
        public float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond)
        {
            return ReadMonoFromFile(filename, samplerate);
        }

        public float[] ReadMonoFromFile(string filename, int samplerate)
        {
            byte[] wav = File.ReadAllBytes(filename);

            // Determine if mono or stereo
            int channels = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels
            if (channels == 2)
                throw new ArgumentException("Файл иммел не верный формат. Кол-во каналов должно быть 1");
            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12;   // First Subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            int samples = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
            if (channels == 2) samples /= 2;        // 4 bytes per sample (16 bit stereo)

            // Allocate memory (right will be null if only mono sound)
            var left = new float[samples];          

            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length)
            {
                left[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                i++;
            }
            return left;
        }

        static float bytesToFloat(byte firstByte, byte secondByte)
        {
                // convert two bytes to one short (little endian)
                short s = (short)((secondByte << 8) | firstByte);
                // convert to range from -1 to (just below) 1
                return s / 32768.0f;

        }
    }
}
