using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace SoundIdentification
{
    public class MusicSpectrum
    {
        public MusicSpectrum(IDataStorage dataStorage)
        {
            db = dataStorage;
        }

        /// <summary>
        /// Добавляет треки только форматом mp3 или wav. При не соотвестивии выходит из метода без ошибок.
        /// Используйте классы BassProxy или NaudioService для получения нужного сэмпла.
        /// Они находятся в пространстве имен SoundIdentification.AudioProxy. ОНИ РАБОТАЮТ ТОЛЬКО НА PC.
        /// Также вы можете реализоваать Iterface IAudioService для своей реализации.
        /// </summary>
        public void InsertSongToDB(IAudioService audioReader, string filePath)
        {
            if (!File.Exists(filePath))
                return;
            if ((Path.GetExtension(filePath) != ".mp3") && (Path.GetExtension(filePath) != ".wav"))
                return;

            Song song = new Song(filePath);

            if (!File.Exists(filePath))
                return;
            float[] audio = audioReader.ReadMonoFromFile(filePath, 44100);

            AddSpectrumToDB(audio, song);
        }

        /// <summary>
        /// Бит рейт входного файла должен быть 44100. Используйте классы BassProxy или NaudioService для получения нужного сэмпла.
        /// Они находятся в пространстве имен SoundIdentification.AudioProxy. ОНИ РАБОТАЮТ ТОЛЬКО НА PC. 
        /// </summary>
        public void InsertSongToDB(float[] audioSample, Song song)
        {
            AddSpectrumToDB(audioSample, song);
        }

        /// <summary>
        /// Добавляет треки только форматом mp3 или wav. При не соотвестивии выходит из метода без ошибок
        /// </summary>
        public async void InsertSongToDBAsync(IAudioService audioReader, string filePath)
        {
            await Task.Run(() => InsertSongToDB(audioReader, filePath));
        }

        /// <summary>
        /// Бит рейт входного файла должен быть 44100. Используйте классы BassProxy или NaudioService для получения нужного сэмпла.
        /// Они находятся в пространстве имен SoundIdentification.AudioProxy. ОНИ РАБОТАЮТ ТОЛЬКО НА PC. 
        /// </summary>
        public async void InsertSongToDBAsync(float[] audioSample, Song song)
        {
            await Task.Run(() => InsertSongToDB(audioSample, song));
        }

        /// <summary>
        /// Если трек не найден возвращает пустой класс Song 
        /// </summary>
        public QueryResult SeekSong(IAudioService audioReader, string filePath)
        {

            float[] audio = audioReader.ReadMonoFromFile(filePath, 44100);

            return AnalysAndMatching(audio);
        }

        public QueryResult SeekSong(float[] audioSample)
        {
           
            return AnalysAndMatching(audioSample);

        }

       

        /// <summary>
        /// Если трек не найден возвращает пустой класс Song 
        /// </summary>
        public async Task<QueryResult> SeekSongAsync(IAudioService audioReader, string filePath)
        {
            return await Task.Run(() => SeekSong(audioReader, filePath));
        }

        internal Dictionary<int, Dictionary<int, int>> FFTAnalys(Dictionary<int, Dictionary<int, int>> tmpMap, float[] audio, int amountPossible)
        {
            Complex[][] results = MakeFFT(audio, amountPossible);

            highscores = InitMatr<double>(results.Length, 5);
            recordPoints = InitMatr<double>(results.Length, UPPER_LIMIT);
            long[,] points = InitMatr<long>(results.Length, 5);
            double x, y;
            double abs;
            for (int t = 0; t < results.Length; t++)
            {
                for (int freq = LOWER_LIMIT; freq < UPPER_LIMIT - 1; freq++)
                {
                    // Get the magnitude:
                    x = results[t][freq].Real;
                    y = results[t][freq].Imaginary;
                    abs = Math.Sqrt(x * x + y * y);
                    double mag = Math.Log(abs + 1);

                    // Find out which range we are in:
                    int index = getIndex(freq);

                    // Save the highest magnitude and corresponding frequency:
                    if (mag > highscores[t, index])
                    {
                        highscores[t, index] = mag;
                        recordPoints[t, freq] = 1;
                        points[t, index] = freq;
                    }
                }

                long h = hash(points[t, 0], points[t, 1], points[t, 2],
                    points[t, 3]);
                pointCount++;
                tmpMap = BuildMap(tmpMap, t, h);

            }

            return tmpMap;
        }

        #region Private
        private const int UPPER_LIMIT = 300;
        private const int LOWER_LIMIT = 40;
        private double[,] highscores, recordPoints;
        private static int[] RANGE = new int[] { 40, 80, 120, 180, UPPER_LIMIT + 1 };
        private static int FUZ_FACTOR = 2;
        private int tag;
        private IDataStorage db;

        private void AddDataPointToBD(int t, long h)
        {

            DataPoint point = new DataPoint() { Hash = h, SongId = tag, Time = t };
            db.InsertDataPoint(h, point);

        }

        private void AddSpectrumToDB(float[] audio, Song song)
        {     
            int totalSize = audio.Length;

            long tagId = db.GetSongsCount() + 1;
            tag = Convert.ToUInt16(tagId);
            db.InsertSong(tag, song);

            int amountPossible = totalSize / 4096;

            Complex[][] results = MakeFFT(audio, amountPossible);

            highscores = InitMatr<double>(results.Length, 5);
            recordPoints = InitMatr<double>(results.Length, UPPER_LIMIT);
            long[,] points = InitMatr<long>(results.Length, 5);
            double x, y;
            double abs;

            for (int t = 0; t < results.Length; t++)
            {
                for (int freq = LOWER_LIMIT; freq < UPPER_LIMIT - 1; freq++)
                {
                    // Get the magnitude:
                    x = results[t][freq].Real;
                    y = results[t][freq].Imaginary;
                    abs = Math.Sqrt(x * x + y * y);
                    double mag = Math.Log(abs + 1);

                    // Find out which range we are in:
                    int index = getIndex(freq);

                    // Save the highest magnitude and corresponding frequency:
                    if (mag > highscores[t, index])
                    {
                        highscores[t, index] = mag;
                        recordPoints[t, freq] = 1;
                        points[t, index] = freq;
                    }
                }

                long h = hash(points[t, 0], points[t, 1], points[t, 2],
                    points[t, 3]);
                AddDataPointToBD(t, h);
            }
            //add song to db

        }

        private T[,] InitMatr<T>(int n, int m)
        {
            T[,] temp = new T[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temp[i, j] = default(T);
                }
            }
            return temp;
        }

        private Complex[][] MakeFFT(float[] audio, int amountPossible)
        {
            Complex[][] results = new Complex[amountPossible][];
            //Выполняем преоброзование быстрое преоброзование фуурье для отдельных цанков файла
            for (int times = 0; times < amountPossible; times++)
            {
                Complex[] complex = new Complex[4096];
                for (int i = 0; i < 4096; i++)
                {
                    // Put the time domain data into a complex number with imaginary
                    // part as 0:
                    complex[i] = new Complex(audio[(times * 4096) + i], 0);
                }
                // Perform FFT analysis on the chunk:

                results[times] = FFT.fft(complex);
            }
            return results;
        }

        private long hash(long p1, long p2, long p3, long p4)
        {
            return (p4 - (p4 % FUZ_FACTOR)) * 100000000 + (p3 - (p3 % FUZ_FACTOR))
                   * 100000 + (p2 - (p2 % FUZ_FACTOR)) * 100
                   + (p1 - (p1 % FUZ_FACTOR));
        }

        private static int getIndex(int freq)
        {
            int i = 0;
            while (RANGE[i] < freq)
                i++;
            return i;
        }

        private double pointCount = 0;
        internal QueryResult Matching(Dictionary<int, Dictionary<int, int>> tmpMap)
        {
            List<SongMatchCount> matchCounts = new List<SongMatchCount>();
            foreach (var item in tmpMap)
            {

                foreach (var Titem in item.Value)
                {
                    //item.Key Id= песни" + Titem.Key Значение "+ Titem.Value+ "\n");
                    matchCounts.Add(new SongMatchCount(Titem.Key, Titem.Value));
                }

            }

            if (matchCounts.Count == 0)
                return new QueryResult();
            int max = matchCounts.Select((x) => x.Num).Max();
            var result = matchCounts.First(x => x.Num == max);

            //List<SongMatchCount> matchGrouped = matchCounts.GroupBy(id => id.SongId).Select(x => new SongMatchCount()
            //{ SongId = x.Key , Num = x.Where(y => y.Num > 10).Sum(z => z.Num) }).ToList();
            //int max = matchGrouped.Select(x => x.Num).Max();
            //var result = matchGrouped.First(x => x.Num == max);
            List<SongMatchCount> matchGrouped = matchCounts.GroupBy(p => p.SongId)
                        .Select(g => new SongMatchCount { SongId = g.Key, Num = g.Count(x => x.Num > 10) }).ToList();
            Console.WriteLine($"Максимальный = {max} результат {result.SongId} кол-во совпадений {result.Num}");
            Console.WriteLine($"Длина макс группа {matchGrouped.Count} 1");
            if (max <= 5)
            {
                return new QueryResult(matchGrouped);
            }
            Song song = db.GetSong(result.SongId);
            QueryResult queryResult = new QueryResult(song, matchGrouped);
            return queryResult;
        }

        //Формат словаря {отступ{idпесни,колво встреч}}
        private Dictionary<int, Dictionary<int, int>> BuildMap(Dictionary<int, Dictionary<int, int>> tmpMap, int t, long h)
        {
            List<DataPoint> listPoints = db.GetDataPoints(h);

            if (listPoints != null)
            {
                //tmpMap = null;
                foreach (DataPoint dP in listPoints)
                {

                    //tmpMap = null;
                    int offset = Math.Abs(dP.Time - t);

                    int count;
                    Dictionary<int, int> keyValues = new Dictionary<int, int>();

                    if (!tmpMap.TryGetValue(offset, out keyValues))
                    {
                        tmpMap.Add(offset, new Dictionary<int, int>() { { dP.SongId, 1 } });
                    }
                    else
                    {
                        if (!tmpMap[offset].TryGetValue(dP.SongId, out count))
                            tmpMap[offset].Add(dP.SongId, 1);
                        else
                        {

                            tmpMap[offset][dP.SongId] += 1;
                            Console.WriteLine($"По sonqid {dP.SongId} с hash{h}");
                        }
                    }

                }
            }
            return tmpMap;
        }

        private QueryResult AnalysAndMatching(float[] audioSample)
        {
            Dictionary<int, Dictionary<int, int>> tmpMap = new Dictionary<int, Dictionary<int, int>>();
            pointCount = 0;
            int totalSize = audioSample.Length;

            int amountPossible = totalSize / 4096;

            tmpMap = FFTAnalys(tmpMap, audioSample, amountPossible);

            return Matching(tmpMap);
        }
        #endregion Private
    }
}
