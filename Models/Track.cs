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
    /// <summary>
    /// Class of object Track.
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Gets or sets the track Id.
        /// </summary>
        /// <value>The track Id.</value>
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the track.
        /// </summary>
        /// <value>The full name of the track.</value>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the track duration.
        /// </summary>
        /// <value>The track duration.</value>
        [JsonProperty(PropertyName = "Duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Initializes a default instance of the <see cref="Track" /> class.
        /// </summary>
        public Track()
        {
            Id = 0;
            Name = "";
            Duration = "00:00";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Track" /> class.
        /// </summary>
        /// <param name="id">The track Id.</param>
        /// <param name="name">The full name of the track.</param>
        /// <param name="duration">The track duration.</param>
        public Track(int id, string name, string duration)
        {
            Id = id;
            Name = name;
            Duration = duration;
        }

        /// <summary>
        /// Gets the track name without directory and extension.
        /// </summary>
        /// <value>The full name of the track.</value>
        public string GetName
        {
            get => Path.GetFileNameWithoutExtension(Name);
        }

        /// <summary>
        /// Sets the duration of track.
        /// </summary>
        /// <param name="duration">The duration of track.</param>
        public void SetDuration(string duration) => Duration = duration;
    }

    /// <summary>
    /// Track list display model class.
    /// </summary>
    public class TracksViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<Track> tracks;

        /// <summary>
        /// Gets or sets the tracks.
        /// </summary>
        /// <value>The tracks.</value>
        public ObservableCollection<Track> Tracks 
        {
            get { return this.tracks; }
            set 
            {
                this.tracks = value;
                this.OnPropertyChanged(nameof(this.tracks));
            } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TracksViewModel" /> class.
        /// </summary>
        public TracksViewModel()
        {
            this.tracks = new ObservableCollection<Track>();
        }

        /// <summary>Called when [property changed].</summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
