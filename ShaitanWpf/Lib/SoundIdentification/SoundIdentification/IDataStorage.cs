using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification
{
    public interface IDataStorage
    {
        void InsertSong(int songID, Song song);
        long GetSongsCount();
        Song GetSong(int songID);
        void InsertDataPoint(long hash, DataPoint dataPoint);
        List<DataPoint> GetDataPoints(long hash);
    }
}
