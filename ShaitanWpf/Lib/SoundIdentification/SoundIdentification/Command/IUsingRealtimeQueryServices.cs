using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification.Command
{
    public interface IUsingRealtimeQueryServices
    {
        IRealtimeQueryCommand UsingServices(IDataStorage realtimeCollection);
    }
}
