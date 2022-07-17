using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace MyMusicPlayer
{
    /// <summary>
    /// Allows to create AudioGraph, AudioFileInputNode and AudioDeviceOutputNode.
    /// </summary>
    public class AudioClass
    {

        /// <summary>Creates the AudioGraph.</summary>
        /// <returns>
        /// AudioGraph object.
        /// </returns>
        public static async Task<AudioGraph> CreateGraph()
        {
            // Specify settings for graph, the AudioRenderCategory helps to optimize audio processing
            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                var dialog = new MessageDialog(result.Status.ToString());
                await dialog.ShowAsync();
            }

            return result.Graph;
        }

        /// <summary>Creates the AudioFileInputNode.</summary>
        /// <param name="track">Track.</param>
        /// <param name="audioGraph">AudioGraph.</param>
        /// <returns>
        /// AudioFileInputNode object.
        /// </returns>
        public static async Task<AudioFileInputNode> CreateFileInputNode(Track track, AudioGraph audioGraph)
        {
            StorageFile trackFile = null;

            try
            {
                trackFile = await StorageFile.GetFileFromPathAsync(track.Name);
            }
            catch
            {
                return null;
            }

            // file null check code omitted

            CreateAudioFileInputNodeResult result = await audioGraph.CreateFileInputNodeAsync(trackFile);

            if (result.Status != AudioFileNodeCreationStatus.Success)
            {
                var dialog = new MessageDialog(result.Status.ToString());
                await dialog.ShowAsync();
            }

            return result.FileInputNode;
        }

        /// <summary>Creates the AudioDeviceOutputNode.</summary>
        /// <param name="audioGraph">AudioGraph.</param>
        /// <returns>
        /// AudioDeviceOutputNode object.
        /// </returns>
        public static async Task<AudioDeviceOutputNode> CreateDefaultDeviceOutputNode(AudioGraph audioGraph)
        {
            CreateAudioDeviceOutputNodeResult result = await audioGraph.CreateDeviceOutputNodeAsync();

            if (result.Status != AudioDeviceNodeCreationStatus.Success)
            {
                var dialog = new MessageDialog(result.Status.ToString());
                await dialog.ShowAsync();
            }

            return result.DeviceOutputNode;
        }

        /// <summary>Connects the nodes AudioFileInputNode and AudioDeviceOutputNode.</summary>
        /// <param name="fileInputNode">AudioFileInputNode.</param>
        /// <param name="deviceOutputNode">AudioDeviceOutputNode.</param>
        public static void ConnectNodes(AudioFileInputNode fileInputNode, AudioDeviceOutputNode deviceOutputNode)
        {
            if (fileInputNode == null) return;
            fileInputNode.AddOutgoingConnection(deviceOutputNode);
        }
    }
}
