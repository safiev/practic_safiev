using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using QRCoder;

namespace QRCodeGeneratorApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateQRCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text;
            if (text.Length > 125)
            {
                MessageBox.Show("Максимальное количество символов - 125");
                return;
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                QRCodeImage.Source = bitmapImage;
            }
        }

        private void SaveQRCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (QRCodeImage.Source == null)
            {
                MessageBox.Show("Сначала сгенерируйте QR-код");
                return;
            }

            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = "QRCode";
            saveDialog.DefaultExt = ".png";
            saveDialog.Filter = "PNG Image (.png)|*.png";
            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveDialog.FileName;
                BitmapSource bitmapSource = (BitmapSource)QRCodeImage.Source;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                MessageBox.Show("QR-код успешно сохранен");
            }
        }
    }
}
