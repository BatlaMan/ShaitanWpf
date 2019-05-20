namespace SoundIdentification.Command
{
    public interface IQueryCommandBuilder
    {
        /// <summary>
        ///  
        /// </summary>
        IQuerySource BuildQueryCommand();

        
        
        IRealtimeSource BuildRealtimeQueryCommand();
    }
}