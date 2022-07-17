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
    /// <summary>
    /// The class allows to get tracks and save them
    /// using the automatically generated TrackList.json file
    /// in the local application folder.
    /// </summary>
    public class TrackList
    {
        static string jsonPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "TrackList.json");

        private static string ReadFile(string file)
        {
            string info = "";
            using (StreamReader reader = new StreamReader(file))
            {
                info = reader.ReadToEnd();
            }
            return info;
        }

        /// <summary>Getting a list of track names from the current playlist.</summary>
        /// <param name="currentTracks">The current playlist.</param>
        /// <returns>
        /// List of track names.
        /// </returns>
        public static List<string> GetListFiles(ObservableCollection<Track> currentTracks)
        {
            List<string> names = new List<string>();

            foreach (var track in currentTracks)
            {
                names.Add(track.Name);
            }

            return names;
        }

        /// <summary>Getting the current track list from the TrackList.json file.</summary>
        /// <returns>
        /// List of tracks.
        /// </returns>
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

        /// <summary>Saves the current track list from the playlist to the file TrackList.json.</summary>
        /// <param name="tracks">Tracks.</param>
        public static void SaveTracks(ObservableCollection<Track> tracks)
        {
            if (!File.Exists(jsonPath))
                File.Create(jsonPath).Close();

            string jsonString = JsonConvert.SerializeObject(tracks);
            File.WriteAllText(jsonPath, jsonString);
        }

        /// <summary>
        /// Sets the order of the current tracks from 1 to n,
        /// saves the track and returns an renumbered list of tracks.</summary>
        /// <param name="tracks">Tracks.</param>
        /// <returns>
        /// Renumbered track list.
        /// </returns>
        public static ObservableCollection<Track> ReindexList(ObservableCollection<Track> tracks)
        {
            var id = 1;

            foreach (var track in tracks) track.Id = id++;

            SaveTracks(tracks);

            return GetTracks();
        }

        /// <summary>
        /// Get track numbers from the current playlist
        /// that have been deleted or moved.
        /// </summary>
        /// <param name="tracks">Tracks.</param>
        /// <returns>
        /// List of track numbers.
        /// </returns>
        public static List<int> RefreshTracks(ObservableCollection<Track> tracks)
        {
            List<int> trackIds= new List<int>();

            try
            {
                foreach (var track in tracks)
                {
                    try
                    {
                        if (StorageFile.GetFileFromPathAsync(track.Name).AsTask().Result != null) { }
                    }
                    catch
                    {
                        trackIds.Add(track.Id);
                    }
                }
            }
            catch
            {
                return new List<int>();
            }

            return trackIds;
        }
    }
}
