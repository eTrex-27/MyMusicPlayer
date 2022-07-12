using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class MusicPage : Page
    {
        BitmapImage bitmapPlay = new BitmapImage(new Uri("ms-appx:///Assets/play.png", UriKind.Absolute));
        BitmapImage bitmapPause = new BitmapImage(new Uri("ms-appx:///Assets/pause.png", UriKind.Absolute));

        public AudioGraph audioGraph { get; set; }
        public AudioDeviceOutputNode deviceOutputNode { get; set; }
        public AudioFileInputNode fileInputNode { get; set; }
        public MusicPage()
        {
            this.InitializeComponent();

            this.TrackListView = new TracksViewModel();

            this.Track = new Track();

            PlayImage.Source = bitmapPlay;

            listMusic.SelectedIndex = 0;
        }

        private async void SetDuration()
        {
            foreach (var track in TrackListView.Tracks)
            {
                audioGraph = await AudioClass.CreateGraph();
                fileInputNode = await AudioClass.CreateFileInputNode(track, audioGraph);

                track.Duration = $"{fileInputNode.Duration.Minutes}:{fileInputNode.Duration.Seconds}";
            }
        }

        public TracksViewModel TrackListView { get; set; }
        public Track Track { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tracks = (ObservableCollection<Track>)e.Parameter;

            foreach (Track track in tracks)
            {
                TrackListView.Tracks.Add(track);
            }

            TrackList.SaveTracks(tracks);

            //SetDuration();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (PlayImage.Source == bitmapPlay)
            {
                PlayImage.Source = bitmapPause;
            }
            else
            {
                PlayImage.Source = bitmapPlay;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            
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

                var id = TrackList.GetTracks().Count + 1;

                var listFiles= TrackList.GetListFiles(TrackListView.Tracks);

                foreach (var file in filesList)
                {
                    if (!listFiles.Contains(file.Path))
                        TrackListView.Tracks.Add(new Track(id++, file.Path, "0:00"));
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

            var refreshing = false;

            foreach (var id in listIds)
            {
                if (id < TrackListView.Tracks.Count + 1 || TrackListView.Tracks.Count == 0)
                {
                    refreshing = true;
                    break;
                }
            }

            if (refreshing)
            {
                var refreshList = TrackList.ReindexList(TrackListView.Tracks);

                TrackListView.Tracks.Clear();

                foreach (var item in refreshList)
                    TrackListView.Tracks.Add(item);
            }
        }

        private void RefreshingList(int id)
        {
            if (id < TrackListView.Tracks.Count + 1 || TrackListView.Tracks.Count == 0)
            {
                var refreshList = TrackList.ReindexList(TrackListView.Tracks);

                TrackListView.Tracks.Clear();

                foreach (var item in refreshList)
                    TrackListView.Tracks.Add(item);
            }
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
                RefreshingList((command.Id as Track).Id);
            }
            else if (command.Label.Equals("Отмена"))
            {
                return;
            }
        }

        private async void listMusic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (audioGraph != null) audioGraph.Dispose();

            int trackId;
            try
            {
                trackId = (sender as ListView).SelectedIndex;
            }
            catch
            {
                return;
            }

            audioGraph = await AudioClass.CreateGraph();
            deviceOutputNode = await AudioClass.CreateDefaultDeviceOutputNode(audioGraph);
            fileInputNode = await AudioClass.CreateFileInputNode(TrackListView.Tracks[trackId], audioGraph);
            AudioClass.ConnectNodes(fileInputNode, deviceOutputNode);

            audioGraph.Start();
        }
    }
}
