using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification
{
    public class QueryResult
    {
        public Song BestMath { get => bestMath; }
        public List<SongMatchCount> MatchList { get => matchList; }

        public QueryResult(Song bestMath,List<SongMatchCount> matchList)
        {
            this.bestMath = bestMath;
            this.matchList = matchList;
        }

        public QueryResult(List<SongMatchCount> matchList)
        {
            this.matchList = matchList;
        }

        public QueryResult()
        {
            
        }

        private Song bestMath;
        private List<SongMatchCount> matchList;
    }
}
