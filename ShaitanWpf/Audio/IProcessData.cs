using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaitanWpf.Audio
{
    public interface IProcessData
    {
        /// <summary>
        /// Определяет если звук в  записи
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool ProcessData(WaveInEventArgs e);
        /// <summary>
        /// Определяет если звук в  записи
        /// </summary>
        /// <param name="e"></param>
        /// <param porog="porog">Определяет порог чувствительности. По умолчанию 0,02</param>
        /// <returns></returns>
        bool ProcessData(WaveInEventArgs e,double  porog);
    }
}
