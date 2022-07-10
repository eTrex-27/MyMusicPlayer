using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace MyMusicPlayer
{
    public class Track
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id;
        [JsonProperty(PropertyName = "Name")]
        public string Name;
        [JsonProperty(PropertyName = "Duration")]
        public string Duration;

        public Track()
        {
            Id = 0;
            Name = "123";
            Duration = "0:00";
        }
        public Track(int id, string name, string duration)
        {
            Id = id;
            Name = name;
            Duration = duration;
        }

        public int GetId() => Id;
        public string GetFullName() => Name;
        public string GetName
        {
            get => Path.GetFileNameWithoutExtension(Name);
        }
        public string GetDuration() => Duration;
        public static string SetFormatString(Track track) => $"{track.Id} {track.GetName} {track.Duration}";
        public string FormatString
        {
            get
            {
                return $"{Id} {GetName} {Duration}";
            }
        }
    }

    public class TracksViewModel
    {
        private ObservableCollection<Track> tracks = new ObservableCollection<Track>();
        public ObservableCollection<Track> Tracks { get { return this.tracks; } }

        public TracksViewModel()
        {
            this.tracks = new ObservableCollection<Track>();
        }

        private Track defaultTrack = new Track();
        public Track DefaultTrack { get { return this.defaultTrack; } }
    }
}
