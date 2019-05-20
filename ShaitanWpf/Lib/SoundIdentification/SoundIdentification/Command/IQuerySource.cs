using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.Command
{
    public interface IQuerySource
    {
        /// <summary>
        ///   Источник в аудио файле
        /// </summary>
        /// <param name="pathToAudioFile">Полный путь к аудиофайлу</param>
        IUsingQueryServices From(string pathToAudioFile);

        /// <summary>
        ///   Бит рейт источника должен быть 44100
        /// </summary>
        /// <param name="audioSamples">Audio samples to build the fingerprints from</param>
        /// <returns>Configuration selector</returns>
        IUsingQueryModelService From(float[] audioSamples);
    }
}
