using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NugopacEditor
{
    public partial class MainWindow : Window
    {
        private Image currentImage;

        public MainWindow() => InitializeComponent();

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.jpeg" };
            if (op.ShowDialog() == true)
            {
                currentImage = Image.Load(op.FileName);
                UpdateDisplay();
                StatusText.Text = "status: image loaded";
            }
        }

        private void Gray_Click(object sender, RoutedEventArgs e) => ApplyEffect(i => i.Grayscale());
        private void Invert_Click(object sender, RoutedEventArgs e) => ApplyEffect(i => i.Invert());
        private void Sepia_Click(object sender, RoutedEventArgs e) => ApplyEffect(i => i.Sepia());
        private void Pixel_Click(object sender, RoutedEventArgs e) => ApplyEffect(i => i.Pixelate(15));

        private void ApplyEffect(Action<IImageProcessingContext> action)
        {
            if (currentImage == null) return;
            try {
                currentImage.Mutate(action);
                UpdateDisplay();
                StatusText.Text = "status: effect applied";
            } catch (Exception ex) {
                StatusText.Text = $"error: {ex.Message}";
            }
        }

        private void UpdateDisplay()
        {
            using (var ms = new MemoryStream())
            {
                currentImage.SaveAsPng(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                MainImage.Source = bitmap;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage == null) return;
            var save = new SaveFileDialog { Filter = "PNG Image|*.png" };
            if (save.ShowDialog() == true) currentImage.Save(save.FileName);
        }
    }
}