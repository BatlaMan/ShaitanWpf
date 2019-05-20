using System;
using TagLib;
using MongoDB.Bson.Serialization.Attributes;


namespace SoundIdentification
{
   public class Song
    {
        string title;
        string artist;
        string album;
        string year;
        string comment;
        string genre;

        [BsonId]
        public int Id { get; set; }
        public string Title { get => title; set => title = value; }
        public string Artist { get => artist; set => artist = value; }
        public string Album { get => album; set => album = value; }
        public string Year { get => year; set => year = value; }
        public string Comment { get => comment; set => comment = value; }
        public string Genre { get => genre; set => genre = value; }

        public Song()
        {
            this.title = string.Empty;
            this.artist = string.Empty;
            this.album = string.Empty;
            this.year = string.Empty;
            this.comment = string.Empty;
            this.genre = string.Empty;
        }

        public Song(string title, string artist, string album, string year, string comment, string genre)
        {
            this.title = title;
            this.artist = artist;
            this.album = album;
            this.year = year;
            this.comment = comment;
            this.genre = genre;
        }

        public Song(string filePath)
        {

            var tfile = File.Create(filePath);
            title = tfile.Tag.Title;
            artist = tfile.Tag.FirstPerformer;
            album = tfile.Tag.Album;
            year = Convert.ToString(tfile.Tag.Year);
            comment = tfile.Tag.Comment;
            genre = tfile.Tag.FirstGenre;


        }

        public override string ToString()
        {
            return string.Format($"Title {title} Artist {artist} Album {album} Year {year} Comment {comment} Genre {genre}");
        }
    }
}
