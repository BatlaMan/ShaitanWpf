using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SoundIdentification.Command
{
    public interface IFingerprintCommand
    {
        Task Add();
    }
}
