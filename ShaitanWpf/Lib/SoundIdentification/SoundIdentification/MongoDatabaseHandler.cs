using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SoundIdentification
{
    public class MongoDatabaseHandler : IDataStorage
    {
        private string dataBaseName;
        private string songTable;
        private string hashTable;
        private IMongoCollection<DataPoint> collectionDataPoint;
        private IMongoCollection<Song> collectionSong;
        private MongoClient client;
        private IMongoDatabase database;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка подключения для вашей монго базы данных.</param>
        /// <param name="dataBaseName"></param>
        /// <param name="songTable">колекция для хранения информации о треках.</param>
        /// <param name="hashTable">колекция для хранения хеша треков.</param>
        /// <returns></returns>
        public MongoDatabaseHandler(string connectionString, string dataBaseName, string songTable, string hashTable)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(dataBaseName);
            collectionDataPoint = database.GetCollection<DataPoint>(hashTable);
            collectionSong = database.GetCollection<Song>(songTable);
            this.dataBaseName = dataBaseName;
            this.songTable = songTable;
            this.hashTable = hashTable;
        }

        public List<DataPoint> GetDataPoints(long hash)
        {
            var filter = new BsonDocument("Hash", hash);
            return collectionDataPoint.Find(filter).ToList();
        }


        public Song GetSong(int songID)
        {
            var filter = new BsonDocument("_id", songID);
            return collectionSong.Find(filter).First();
        }

        public long GetSongsCount()
        {
            return collectionSong.CountDocuments(new BsonDocument());
        }

        public async void InsertDataPoint(long hash, DataPoint dataPoint)
        {
            await collectionDataPoint.InsertOneAsync(dataPoint);
        }

        public async void InsertSong(int songID, Song song)
        {
            song.Id = songID;
            await collectionSong.InsertOneAsync(song);
        }
    }
}
