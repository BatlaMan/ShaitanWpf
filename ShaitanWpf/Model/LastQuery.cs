using SoundIdentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaitanWpf.Model
{
    [Serializable]
    public class LastQuery
    {
        public string Title { get; set; }
        public string Performer { get; set; }

        public LastQuery()
        {
               
        }

        public LastQuery(string title, string performer)
        {
            Title = title;
            Performer = performer;
        }

        public LastQuery(QueryResult queryResult)
        {
            Title = queryResult.BestMath.Title;
            Performer = queryResult.BestMath.Artist;
        }
    }
}
