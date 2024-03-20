using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace ScanProduct
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        //private BarcodeReader barcodeReader;


        public MainWindow()
        {
            InitializeComponent();
            InitializeCamera();
            //InitializeBarcodeReader();
        }

        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No video devices found.");
            }
        }
        //private void InitializeBarcodeReader()
        //{
        //    barcodeReader = new BarcodeReader();
        //}

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                Dispatcher.Invoke(() =>
                {
                    webcamImage.Source = ConvertBitmapToBitmapSource(bitmap);

                    // Chuyển đổi Bitmap thành mảng byte
                   
                    BarcodeReader reader = new BarcodeReader();

                    // Thiết lập cài đặt cho việc quét mã vạch (nếu cần)
                    reader.Options = new DecodingOptions
                    {
                        TryHarder = true, // Thử cách quét khó hơn để tìm mã vạch trong các hình ảnh phức tạp hơn
                        PossibleFormats = new BarcodeFormat[] { BarcodeFormat.CODE_39, BarcodeFormat.CODE_128 } // Các định dạng mã vạch bạn muốn quét (ví dụ: CODE_39)
                    };

                    // Quét mã vạch từ bitmap
                    Result result = reader.Decode(bitmap);

                    // Kiểm tra nếu đã quét thành công
                    if (result != null)
                    {
                        string barcodeText = result.Text;
                        inputTextBox.Text = barcodeText;

                    }
                    else
                    {
                        // Xử lý khi không quét được mã vạch
                        // Ví dụ: thông báo cho người dùng rằng không tìm thấy mã vạch
                        // MessageBox.Show("Không tìm thấy mã vạch");
                    }

                });
            }
        }

        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }



        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
            }
            base.OnClosing(e);
        }
    }
}
