using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

            PlayImage.Source = bitmapPlay;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var files = (List<StorageFile>)e.Parameter;

            var musicList = new List<string>();

            foreach (StorageFile file in files)
            {
                musicList.Add(file.Name);
            }

            listMusic.ItemsSource = musicList;
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
    }
}
