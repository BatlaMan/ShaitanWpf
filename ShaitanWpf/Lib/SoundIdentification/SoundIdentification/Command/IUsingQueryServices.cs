namespace SoundIdentification.Command
{
    public interface IUsingQueryServices
    {
        /// <summary>
        /// Sets model service as well as audio service using in querying the source
        /// </summary>
        /// <param name="dataStorageToUse">База данных в котором нааходятся музыкальныее отпечатки</param>
        /// <param name="audioServiceToUse"></param>
        /// <returns>Query command</returns>
        IQueryCommand UsingServices(IDataStorage dataStorageToUse, IAudioService audioServiceToUse);
    }
}