using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
    }
}
