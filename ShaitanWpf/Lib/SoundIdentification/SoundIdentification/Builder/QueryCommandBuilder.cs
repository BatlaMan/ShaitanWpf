using SoundIdentification.Command;
using System;
using System.Collections.Generic;
using System.Text;


namespace SoundIdentification.Builder
{
    public class QueryCommandBuilder:IQueryCommandBuilder
    {
        private readonly IFingerprintCommandBuilder fingerprintCommandBuilder;
        

        public QueryCommandBuilder() 
        {
        }

   

        public IQuerySource BuildQueryCommand()
        {
            return new QueryCommand();
        }

      
        public IRealtimeSource BuildRealtimeQueryCommand()
        {
            return new RealtimeQueryCommand();
        }

        public static IQueryCommandBuilder Instance { get; } = new QueryCommandBuilder();
    }
}
