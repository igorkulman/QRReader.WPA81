using System;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Nokia.Graphics.Imaging;
using QRReader.Messages;
using ZXing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace QRReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private CameraPreviewImageSource _cameraPreviewImageSource; 
        private WriteableBitmap _writeableBitmap; 
        private WriteableBitmapRenderer _writeableBitmapRenderer;
        private readonly BarcodeReader _reader;

        private bool _initialized;
        private bool _isRendering;

        private readonly IEventAggregator _eventAggregator;

        public MainView()
        {
            this.InitializeComponent();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _reader = new BarcodeReader();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            StatusBar.GetForCurrentView().HideAsync();
            Init();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            StatusBar.GetForCurrentView().ShowAsync();
            Clean();
        }

        private async Task Init()
        {
            _cameraPreviewImageSource = new CameraPreviewImageSource();
            await _cameraPreviewImageSource.InitializeAsync(string.Empty);
            var properties = await _cameraPreviewImageSource.StartPreviewAsync();
            
            var width = 640.0;
            var height = (width / properties.Width) * properties.Height;
            var bitmap = new WriteableBitmap((int)width, (int)height);

            _writeableBitmap = bitmap;

            PreviewImage.Source = _writeableBitmap;

            _writeableBitmapRenderer = new WriteableBitmapRenderer(_cameraPreviewImageSource, _writeableBitmap);

            _cameraPreviewImageSource.PreviewFrameAvailable += OnPreviewFrameAvailable;

            _initialized = true;
        }

        private async void Clean()
        {
            if (_cameraPreviewImageSource != null)
            {
                await _cameraPreviewImageSource.StopPreviewAsync();
            }
        }

        private async void OnPreviewFrameAvailable(IImageSize args)
        {
            if (_initialized && !_isRendering)
            {
                _isRendering = true;

                await _writeableBitmapRenderer.RenderAsync();

                await Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    () =>
                    {
                        _writeableBitmap.Invalidate();
                        var decoded = _reader.Decode(_writeableBitmap);

                        if (decoded != null && decoded.BarcodeFormat == BarcodeFormat.QR_CODE)
                        {
                            _eventAggregator.PublishOnUIThread(new QRDecodedMessage(decoded.Text));
                            Stop();
                        }
                    });

                _isRendering = false;
            }
        }

        private async Task Stop()
        {
            _cameraPreviewImageSource.PreviewFrameAvailable -= OnPreviewFrameAvailable;
            await _cameraPreviewImageSource.StopPreviewAsync();
        }
    }
}
