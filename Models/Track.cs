using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
using Windows.Media.Audio;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyMusicPlayer
{
    public class Track
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "Duration")]
        public string Duration { get; set; }

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
        public void SetId(int id) => Id = id;
        public string GetFullName() => Name;
        public string GetName
        {
            get => Path.GetFileNameWithoutExtension(Name);
        }
        public string GetDuration() => Duration;
        public void SetDuration(string duration) => Duration = duration;
        public static string SetFormatString(Track track) => $"{track.Id} {track.GetName} {track.Duration}";
        public string FormatString
        {
            get
            {
                return $"{Id} {GetName} {Duration}";
            }
        }
    }

    public class TracksViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<Track> tracks;
        public ObservableCollection<Track> Tracks 
        {
            get { return this.tracks; }
            set 
            {
                this.tracks = value;
                this.OnPropertyChanged(nameof(this.tracks));
            } 
        }

        

        public TracksViewModel()
        {
            this.tracks = new ObservableCollection<Track>();
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Track defaultTrack = new Track();

        public Track DefaultTrack { get { return this.defaultTrack; } }
    }
}
