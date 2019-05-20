using System;
using System.Collections.Generic;
using System.Text;
using SoundIdentification.Command;

namespace SoundIdentification.Builder
{
    public class FingerprintCommandBuilder : IFingerprintCommandBuilder
    {

        public FingerprintCommandBuilder()
        {
        }


        public static IFingerprintCommandBuilder Instance { get; } = new FingerprintCommandBuilder();

        public ISourceFrom BuildFingerprintCommand()
        {
            return new FingerprintCommand();
        }
    }
}
