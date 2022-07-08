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
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using Windows.ApplicationModel.DataTransfer;

namespace MyMusicPlayer
{
    public sealed partial class MainPage : Page
    {
        class StartPageInfo
        {
            private string textButton = "Добавить файлы...";
            private string labelOrText = "или";
            private Brush labelOrColor = new SolidColorBrush(Windows.UI.Color.FromArgb(66, 00, 00, 00));
            private string labelDragAndDropText = "перетащите файлы в это окно";
            private Brush labelDragAndDropColor = new SolidColorBrush(Windows.UI.Color.FromArgb(66, 00, 00, 00));

            public string TextButton { get { return textButton; } set { textButton = value; } }
            public string LabelOrText { get { return labelOrText; } set { labelOrText = value; } }
            public string LabelDragAndDropText { get { return labelDragAndDropText; } set { labelDragAndDropText = value; } }
            public Brush LabelDragAndDropColor { get { return labelDragAndDropColor; } set { labelDragAndDropColor = value; } }
            public Brush LabelOrColor { get { return labelOrColor; } set { labelOrColor = value; } }
        }

        private StartPageInfo pageInfo;

        public MainPage()
        {
            this.pageInfo = new StartPageInfo();
            this.InitializeComponent();
            this.DataContext = this.pageInfo;

            OpenFileButton.Content = pageInfo.TextButton;
            LabelOR.Text = pageInfo.LabelOrText;
            LabelOR.Foreground = pageInfo.LabelOrColor;
            LabelDragAndDrop.Text = pageInfo.LabelDragAndDropText;
            LabelDragAndDrop.Foreground = pageInfo.LabelDragAndDropColor;
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            List<StorageFile> files = null;

            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".wav");
                var filesList = await openPicker.PickMultipleFilesAsync();

                files = filesList.ToList();
            }
            catch
            {
                var dialog = new MessageDialog("Не удалось загрузить файлы, попробуйте ещё раз");
                await dialog.ShowAsync();
            }

            if (files != null && files.Count != 0)
            {
                try
                {
                    Frame.Navigate(typeof(MusicPage), files, new SuppressNavigationTransitionInfo());
                }
                catch
                {
                    var dialog = new MessageDialog("Не удалось открыть страницу с плеером, попробуйте снова выбрать файлы");
                    await dialog.ShowAsync();
                }
            }
        }

        private void Grid_DragOverCustomized(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Отпустите, чтобы добавить";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            this.Opacity = 0.35;
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            this.Opacity = 1.0;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            List<StorageFile> listMusic = new List<StorageFile>();

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var storageFile = item as StorageFile;

                        if (storageFile.FileType == ".mp3" || storageFile.FileType == ".wav")
                        {
                            listMusic.Add(storageFile);
                        }
                    }

                    if (listMusic.Count != 0)
                    {
                        try
                        {
                            Frame.Navigate(typeof(MusicPage), listMusic, new SuppressNavigationTransitionInfo());
                        }
                        catch
                        {
                            var dialog = new MessageDialog("Не удалось открыть страницу с плеером, попробуйте снова выбрать файл");
                            await dialog.ShowAsync();
                        }
                    }
                }
            }
        }
    }
}
