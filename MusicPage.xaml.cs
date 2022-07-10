using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public MusicPage()
        {
            this.InitializeComponent();

            this.TrackListView = new TracksViewModel();

            this.Track = new Track();

            PlayImage.Source = bitmapPlay;
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
    }
}
