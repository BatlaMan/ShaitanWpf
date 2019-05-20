using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ShaitanWpf.Audio
{
    class AudioDataProcesser : IProcessData
    {
        public bool ProcessData(WaveInEventArgs e)
        {
            double porog = 0.02;
            return ProcessData(e, porog); 
        }

        public bool ProcessData(WaveInEventArgs e, double porog)
        {
            bool result = false;
            bool Tr = false;
            double Sum2 = 0;
            int Count = e.BytesRecorded / 2;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                double Tmp = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                Tmp /= 32768.0;
                Sum2 += Tmp * Tmp;
                if (Tmp > porog)
                    Tr = true;
            }
            Sum2 /= Count;
            if (Tr || Sum2 > porog)
            { result = true; }
            else
            { result = false; }
            return result;
        }
    }
}
