using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MyMusicPlayer
{
    /// <summary>
    /// Music player page with the ability to add and edit a playlist, as well as control the playback of tracks.
    /// </summary>
    public sealed partial class MusicPage : Page
    {
        BitmapImage bitmapPlay = new BitmapImage(new Uri("ms-appx:///Assets/play.png", UriKind.Absolute));
        BitmapImage bitmapPause = new BitmapImage(new Uri("ms-appx:///Assets/pause.png", UriKind.Absolute));
        BitmapImage bitmapActiveSound = new BitmapImage(new Uri("ms-appx:///Assets/activeSound.png", UriKind.Absolute));
        BitmapImage bitmapDisactiveSound = new BitmapImage(new Uri("ms-appx:///Assets/disactiveSound256.png", UriKind.Absolute));

        bool repeatPressed = false;
        bool shufflePressed = false;
        bool manipulation = false;

        int currentTrackId = -1;
        bool ignoreChange = false;
        private TracksViewModel TrackListView { get; set; }
        private Track Track { get; set; }
        private AudioGraph audioGraph { get; set; }
        private AudioDeviceOutputNode deviceOutputNode { get; set; }
        private AudioFileInputNode fileInputNode { get; set; }

        DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPage" /> class.
        /// </summary>
        public MusicPage()
        {
            this.InitializeComponent();

            this.TrackListView = new TracksViewModel();

            this.Track = new Track();

            PlayImage.Source = bitmapPlay;

            listMusic.SelectedIndex = 0;

            TrackListView.Tracks.CollectionChanged += Tracks_CollectionChanged;

            repeatPressed = false;
            shufflePressed = false;

            SliderVolume.Visibility = Visibility.Collapsed;
            SliderVolume.Value = 100;

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Tracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    if (e.NewItems?[0] is Track newTrack)
                    {
                        Console.WriteLine($"Добавлен новый объект: {newTrack.Name}");
                    }
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    if (e.OldItems?[0] is Track oldTrack)
                    {
                        int id = 1;
                        foreach (var item in TrackListView.Tracks)
                        {
                            item.Id = id++;
                        }
                        TrackList.SaveTracks(TrackListView.Tracks);
                        Console.WriteLine($"Удален объект: {oldTrack.Name}");
                    }
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    if ((e.NewItems?[0] is Track replacingTrack) &&
                        (e.OldItems?[0] is Track replacedTrack))
                        Console.WriteLine($"Объект {replacedTrack.Name} заменен объектом {replacingTrack.Name}");
                    break;
            }
        }

        /// <summary>Sets the duration of the newly added track.</summary>
        /// <param name="newTrack">Track.</param>
        public static async Task SetDurationTrack(Track newTrack)
        {
            AudioGraph audioGraphTemp = await AudioClass.CreateGraph();
            AudioFileInputNode fileInputNodeTemp = await AudioClass.CreateFileInputNode(newTrack, audioGraphTemp);
            string duration = GetDuration(fileInputNodeTemp);

            newTrack.SetDuration(duration);

            fileInputNodeTemp.Dispose();
            audioGraphTemp.Dispose();
        }

        private static string GetDuration(AudioFileInputNode fileInputNode)
        {
            var duration = "";

            var seconds = fileInputNode.Duration.Seconds.ToString();

            var minutes = fileInputNode.Duration.Minutes.ToString();

            var hours = fileInputNode.Duration.Hours.ToString();

            if (minutes.Length == 1)
                minutes = "0" + minutes;

            if (seconds.Length == 1)
                seconds = "0" + seconds;

            if (!hours.Equals("0"))
            {
                if (hours.Length == 1)
                    hours = "0" + hours;

                duration = $"{hours}:{minutes}:{seconds}";
            }
            else
            {
                duration = $"{minutes}:{seconds}";
            }

            return duration;
        }

        /// <summary>
        /// Called when the Page is loaded and becomes the origin of the parent Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that can be checked with code overrides.
        /// These events represent delayed navigation that will cause the current Page to load.
        /// The Parameter property is most often checked.
        /// </param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tracks = (ObservableCollection<Track>)e.Parameter;

            foreach (Track track in tracks)
            {
                await SetDurationTrack(track);
                TrackListView.Tracks.Add(track);
            }

            TrackList.SaveTracks(tracks);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (PlayImage.Source == bitmapPause)
            {
                if (audioGraph != null)
                    audioGraph.Stop();
                trackImage.Source = bitmapDisactiveSound;
                PlayImage.Source = bitmapPlay;
            }
            else
            {
                if (audioGraph != null)
                    audioGraph.Start();
                trackImage.Source = bitmapActiveSound;
                PlayImage.Source = bitmapPause;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (listMusic.SelectedIndex == -1)
            {
                if (currentTrackId != -1)
                {
                    if (shufflePressed && !repeatPressed)
                    {
                        PlayRandomTrack();
                    }
                    else if (repeatPressed)
                    {
                        listMusic_SelectionChanged(listMusic, null);
                    }
                    else
                    {
                        if (currentTrackId == 0)
                            listMusic.SelectedIndex = TrackListView.Tracks.Count - 1;
                        else
                            listMusic.SelectedIndex = currentTrackId - 1;
                    }
                }
                else
                {
                    listMusic.SelectedIndex = 0;
                }
            }
            else
            {
                if (shufflePressed && !repeatPressed)
                {
                    PlayRandomTrack();
                }
                else if (repeatPressed)
                {
                    listMusic_SelectionChanged(listMusic, null);
                }
                else
                {
                    if (listMusic.SelectedIndex == 0)
                    {
                        if (TrackListView.Tracks.Count == 1)
                        {
                            listMusic_SelectionChanged(listMusic, null);
                        }
                        else
                        {
                            listMusic.SelectedIndex = TrackListView.Tracks.Count - 1;
                        }
                    }
                    else
                        listMusic.SelectedIndex -= 1;
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            PlayNextTrack();
        }

        private void PlayNextTrack()
        {
            if (listMusic.SelectedIndex == -1)
            {
                if (currentTrackId != -1)
                {
                    if (shufflePressed && !repeatPressed)
                    {
                        PlayRandomTrack();
                    }
                    else if (repeatPressed)
                    {
                        listMusic_SelectionChanged(listMusic, null);
                    }
                    else
                    {
                        if (currentTrackId == TrackListView.Tracks.Count - 1)
                            listMusic.SelectedIndex = 0;
                        else
                            listMusic.SelectedIndex = currentTrackId + 1;
                    }
                }
                else
                {
                    listMusic.SelectedIndex = 0;
                }
            }
            else
            {
                if (shufflePressed && !repeatPressed)
                {
                    PlayRandomTrack();
                }
                else if (repeatPressed)
                {
                    listMusic_SelectionChanged(listMusic, null);
                }
                else
                {
                    if (listMusic.SelectedIndex == TrackListView.Tracks.Count - 1)
                    {
                        if (TrackListView.Tracks.Count == 1)
                        {
                            listMusic_SelectionChanged(listMusic, null);
                        }
                        else
                        {
                            listMusic.SelectedIndex = 0;
                        }
                    }
                    else
                        listMusic.SelectedIndex += 1;
                }
            }
        }

        private void PlayRandomTrack()
        {
            var random = new Random().Next(TrackListView.Tracks.Count - 1);
            if (listMusic.SelectedIndex == random)
            {
                try
                {
                    listMusic.SelectedIndex = random + 1;
                }
                catch
                {
                    listMusic.SelectedIndex = random - 1;
                }
            }
            else
            {
                listMusic.SelectedIndex = random;
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (repeatPressed)
            {
                RepeatButton.Style = (Style)Resources["CircleButtonStyle"];
                repeatPressed = false;
            }
            else
            {
                RepeatButton.Style = (Style)Resources["CircleButtonStylePressed"];
                repeatPressed = true;
            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (shufflePressed)
            {
                ShuffleButton.Style = (Style)Resources["CircleButtonStyle"];
                shufflePressed = false;
            }
            else
            {
                ShuffleButton.Style = (Style)Resources["CircleButtonStylePressed"];
                shufflePressed = true;
            }
        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            if (SliderVolume.Visibility == Visibility.Collapsed)
            {
                VolumeButton.Style = (Style)Resources["CircleButtonStylePressed"];
                SliderVolume.Visibility = Visibility.Visible;
            }
            else
            {
                VolumeButton.Style = (Style)Resources["CircleButtonStyle"];
                SliderVolume.Visibility = Visibility.Collapsed;
            }
        }

        private void SliderVolume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (fileInputNode == null) return;
            fileInputNode.OutgoingGain = SliderVolume.Value / 100;
        }

        private async void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<StorageFile> filesList = null;

            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".wav");
                filesList = await openPicker.PickMultipleFilesAsync();

                if (filesList != null)
                {
                    foreach (StorageFile file in filesList)
                        StorageApplicationPermissions.FutureAccessList.Add(file);
                }

                var id = TrackList.GetTracks().Count + 1;

                var listFiles= TrackList.GetListFiles(TrackListView.Tracks);

                foreach (var file in filesList)
                {
                    if (!listFiles.Contains(file.Path))
                    {
                        var track = new Track(id++, file.Path, "0:00");
                        await SetDurationTrack(track);
                        TrackListView.Tracks.Add(track);
                    }
                }
            }
            catch
            {
                var dialog = new MessageDialog("Не удалось загрузить файлы, попробуйте ещё раз");
                await dialog.ShowAsync();
            }

            if (filesList != null && filesList.Count != 0)
            {
                try
                {
                    TrackList.SaveTracks(TrackListView.Tracks);
                }
                catch
                {
                    var dialog = new MessageDialog("Не удалось добавить файлы, попробуйте ещё раз");
                    await dialog.ShowAsync();
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var listIds = TrackList.RefreshTracks(TrackListView.Tracks);

            for (int i = TrackListView.Tracks.Count - 1; i >= 0; i--)
            {
                var item = TrackListView.Tracks[i];

                if (listIds.Contains(item.Id))
                    TrackListView.Tracks.RemoveAt(item.Id - 1);
            }

            var refreshList = TrackList.ReindexList(TrackListView.Tracks);

            TrackListView.Tracks.Clear();

            foreach (var item in refreshList)
                TrackListView.Tracks.Add(item);
        }

        private async void listMusic_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource as TextBlock != null)
                {
                    var messageDialog = new MessageDialog($"Вы хотите удалить трек {((e.OriginalSource as TextBlock).DataContext as Track).GetName}?");

                    messageDialog.Commands.Add(new UICommand("Да", new UICommandInvokedHandler(this.CommandInvokedHandler), (e.OriginalSource as TextBlock).DataContext as Track));
                    messageDialog.Commands.Add(new UICommand("Отмена", new UICommandInvokedHandler(this.CommandInvokedHandler), (e.OriginalSource as TextBlock).DataContext as Track));

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;

                    await messageDialog.ShowAsync();
                }
            }
            catch { }
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label.Equals("Да"))
            {
                TrackListView.Tracks.RemoveAt((command.Id as Track).Id - 1);
            }
            else if (command.Label.Equals("Отмена"))
            {
                return;
            }
        }

        private async void listMusic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            manipulation = false;
            int trackId;
            try
            {
                trackId = (sender as ListView).SelectedIndex;

                if (trackId == -1)
                {
                    return;
                }

                try
                {
                    if (audioGraph != null) audioGraph.Dispose();
                    currentTrackId = trackId;
                }
                catch { }
            }
            catch
            {
                return;
            }

            try
            {
                audioGraph = await AudioClass.CreateGraph();
                deviceOutputNode = await AudioClass.CreateDefaultDeviceOutputNode(audioGraph);
                fileInputNode = await AudioClass.CreateFileInputNode(TrackListView.Tracks[trackId], audioGraph);
                AudioClass.ConnectNodes(fileInputNode, deviceOutputNode);
                SetTrackInfo(trackId);

                audioGraph.Start();

                fileInputNode.FileCompleted += FileInputNode_FileCompleted;
                audioGraph.UnrecoverableErrorOccurred += AudioGraph_UnrecoverableErrorOccurred;

                SliderTime.Value = 0;
                SliderTime.Maximum = fileInputNode.Duration.TotalSeconds;

                fileInputNode.OutgoingGain = SliderVolume.Value / 100;

                trackImage.Source = bitmapActiveSound;
                PlayImage.Source = bitmapPause;
            }
            catch
            {
                return;
            }
        }

        private void SetTrackInfo(int trackId)
        {
            DurationTime.Text = GetDuration(fileInputNode);
            trackName.Text = TrackListView.Tracks[trackId].GetName;
        }

        private void AudioGraph_UnrecoverableErrorOccurred(AudioGraph sender, AudioGraphUnrecoverableErrorOccurredEventArgs args)
        {
            listMusic.SelectedIndex = 0;
        }

        private async void FileInputNode_FileCompleted(AudioFileInputNode sender, object args)
        {
            if (!repeatPressed)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { PlayNextTrack(); });
            else
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { SliderTime.Value = 0; });

        }

        private void Timer_Tick(object sender, object e)
        {
            ignoreChange = true;
            if (fileInputNode == null) return;
            if (!manipulation) SliderTime.Value = Convert.ToDouble(fileInputNode.Position.TotalSeconds);
            ignoreChange = false;
        }

        private void SliderTime_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (ignoreChange) return;
            if (SliderTime.Value == SliderTime.Maximum) { PlayNextTrack(); return; }
            try
            {
                fileInputNode.Seek(TimeSpan.FromSeconds(SliderTime.Value));
            }
            catch
            {
                PlayNextTrack();
            }
            manipulation = false;
        }

        private void SliderTime_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            manipulation = true;
        }
    }
}
