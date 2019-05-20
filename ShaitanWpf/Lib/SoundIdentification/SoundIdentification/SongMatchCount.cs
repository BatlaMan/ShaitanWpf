using System;
using System.Collections.Generic;
using System.Text;

namespace SoundIdentification
{
    public struct SongMatchCount
    {
        private int num;
        private int sondId;

        public int Num { get => num; set => num = value; }
        public int SongId { get => sondId; set => sondId = value; }

        public SongMatchCount(int sondId, int num)
        {
            this.num = num;
            this.sondId = sondId;
        }
    }
}
