using MongoDB.Bson;

namespace SoundIdentification
{
    public class DataPoint
    {
        public ObjectId Id { get; set; }
        public long Hash { get; set; }
        public int SongId { get; set; }
        public int Time { get; set; }
    }
}
