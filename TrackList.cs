using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.ObjectModel;

namespace MyMusicPlayer
{
    public class TrackList
    {
        static string jsonPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "TrackList.json");

        public static string ReadFile(string file)
        {
            string info = "";
            using (StreamReader reader = new StreamReader(file))
            {
                info = reader.ReadToEnd();
            }
            return info;
        }

        public static List<string> GetListFiles(ObservableCollection<Track> currentTracks)
        {
            List<string> names = new List<string>();

            foreach (var track in currentTracks)
            {
                names.Add(track.Name);
            }

            return names;
        }

        public static ObservableCollection<Track> GetTracks()
        {
            try
            {
                if (!File.Exists(jsonPath))
                    File.Create(jsonPath).Close();

                string jsonString = ReadFile(jsonPath);

                var tracks = JsonConvert.DeserializeObject<ObservableCollection<Track>>(jsonString);
                if (tracks == null) return new ObservableCollection<Track>();
                return tracks;
            }
            catch
            {
                return new ObservableCollection<Track>();
            }
        }

        public static void SaveTracks(ObservableCollection<Track> tracks)
        {
            if (!File.Exists(jsonPath))
                File.Create(jsonPath).Close();

            string jsonString = JsonConvert.SerializeObject(tracks);
            File.WriteAllText(jsonPath, jsonString);
        }
    }
}
