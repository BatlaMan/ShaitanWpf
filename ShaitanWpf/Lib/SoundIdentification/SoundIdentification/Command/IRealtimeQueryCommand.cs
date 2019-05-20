using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundIdentification.Command
{
    public interface IRealtimeQueryCommand
    {
        Task<double> Query(CancellationToken cancellationToken);
    }
}
