namespace SoundIdentification.Command
{
    public interface IUsingQueryModelService
    {
        IQueryCommand UsingModelService(IDataStorage dataStorageToUse);
    }
}