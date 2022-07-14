using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace MyMusicPlayer.Models
{
    public class AudioTrack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        AudioFileInputNode fileInputNode;
        AudioGraph audioGraph;
        TimeSpan position;
        public AudioFileInputNode FileInputNode {
            get { return this.fileInputNode; }
            set
            {
                this.fileInputNode = value;
                this.OnPropertyChanged(nameof(this.fileInputNode));
            }
        }
        public AudioGraph AudioGraph {
            get { return this.audioGraph; }
            set
            {
                this.audioGraph = value;
                this.OnPropertyChanged(nameof(this.audioGraph));
            }
        }

        public TimeSpan Position {
            get { return this.fileInputNode.Position; }
            set
            {
                this.position = value;
                this.OnPropertyChanged(nameof(this.fileInputNode.Position));
            }
        }

        public AudioTrack(AudioFileInputNode fileInputNode, AudioGraph audioGraph)
        {
            this.fileInputNode = fileInputNode;
            this.audioGraph = audioGraph;
            position = fileInputNode.Position;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
