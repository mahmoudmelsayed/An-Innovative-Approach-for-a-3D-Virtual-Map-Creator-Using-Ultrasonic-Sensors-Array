using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

using Newtonsoft.Json;
using Mapper.Wpf.Hardwares;
using System.Windows.Threading;

namespace Mapper.Wpf
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            MapPlotter.Points.Clear();
        }

        #endregion


        #region Fields

        private int _position;
        private DispatcherTimer _timer;
        private bool _isStopRequested;

        #endregion


        #region Methods

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Logger.LogWritten += OnLogWritten;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            _timer.Tick += TimerOnTick;

            await Task.Delay(500);
            Viewport.Camera.LookDirection = new Vector3D(-2, 16, -20);
            Viewport.ZoomExtents(new Rect3D(0, -10, 0, 20, 20, 20), 20);


            var status = XbeeTransceiver.ConnectToCar(out string carName);
            switch (status)
            {
                case XbeeConnectStatus.Connected:
                    CarNameTextBlock.Text = carName;
                    break;

                case XbeeConnectStatus.NoXbeeFound:
                    MessageBox.Show("Please connect the Xbee module to the PC before launching this application.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown(1);
                    return;

                case XbeeConnectStatus.NoCarFound:
                    MessageBox.Show("Please turn on the car before launching this application.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown(2);
                    return;

                default:
                    break;
            }

            try
            {
                var speed = XbeeTransceiver.GetSpeedSettings();
                var minDistance = XbeeTransceiver.GetMinDistanceSettings();

                SpeedSlider.Value = speed;
                MinDistanceSlider.Value = minDistance;

                StartButton.IsEnabled = true;
                DisconnectButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                SpeedSlider.IsEnabled = true;
                MinDistanceSlider.IsEnabled = true;
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to get settings value from the car!");
                Logger.Write($"Reason: {ex.Error}");
                await ExitAsync(3);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_timer?.IsEnabled == true)
            {
                _timer?.Stop();
            }
        }

        private void OnLogWritten(object sender, string e)
        {
            LogTextBox.Text += e;
            LogScrollViewer.ScrollToEnd();
        }

        private async void TimerOnTick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();

                var data = XbeeTransceiver.GetMapData();
                await Draw(data);
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to get map data from the car!");
                Logger.Write($"Reason: {ex.Error}");
            }

            finally
            {
                if (!_isStopRequested)
                {
                    _timer.Start();
                }
            }
        }

        private async Task Draw(SensorReadings readings)
        {
            Point3D point1 = new Point3D(-readings.Reading0, _position, 2);
            Point3D point2 = new Point3D(-(readings.Reading45 * Math.Cos(Math.PI / 2)), _position, (readings.Reading45 * Math.Sin(Math.PI / 2)) + 2);
            Point3D point3 = new Point3D(0, _position, readings.Reading90 + 2);
            Point3D point4 = new Point3D(readings.Reading135 * Math.Cos(Math.PI / 2), _position, (readings.Reading135 * Math.Sin(Math.PI / 2)) + 2);
            Point3D point5 = new Point3D(readings.Reading180, _position, 2);

            Debug.WriteLine($"{{{point1}}}, {{{point2}}}, {{{point3}}}, {{{point4}}}, {{{point5}}}");

            MapPlotter.Points.Add(point1);
            MapPlotter.Points.Add(point2);

            MapPlotter.Points.Add(point2);
            MapPlotter.Points.Add(point3);

            MapPlotter.Points.Add(point3);
            MapPlotter.Points.Add(point4);

            MapPlotter.Points.Add(point4);
            MapPlotter.Points.Add(point5);

            _position++;
            Viewport.Camera.LookDirection = Vector3D.Add(Viewport.Camera.LookDirection, new Vector3D(0, 1, 0));
            await Task.Delay(200);
        }

        private void StartButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (XbeeTransceiver.StartCar())
                {
                    StartButton.IsEnabled = false;
                    DisconnectButton.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                    SpeedSlider.IsEnabled = false;
                    MinDistanceSlider.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    Logger.Write("Car started!");

                    _timer.Start();
                    _isStopRequested = false;
                }
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to start the car!");
                Logger.Write($"Reason: {ex.Error}");
            }
        }

        private void StopButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (XbeeTransceiver.StopCar())
                {
                    StartButton.IsEnabled = true;
                    DisconnectButton.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                    SpeedSlider.IsEnabled = true;
                    MinDistanceSlider.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    Logger.Write("Car stopped!");

                    _isStopRequested = true;
                }
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to stop the car!");
                Logger.Write($"Reason: {ex.Error}");
            }
        }

        private async void DisconnectButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (XbeeTransceiver.DisconnectFromCar())
                {
                    StartButton.IsEnabled = false;
                    StopButton.IsEnabled = false;
                    DisconnectButton.IsEnabled = false;

                    Logger.Write("Disconnected from car!");
                    await ExitAsync(0);
                }
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to disconnect from the car!");
                Logger.Write($"Reason: {ex.Error}");
            }
        }

        private void SaveButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                XbeeTransceiver.SetSpeedSettings((int)SpeedSlider.Value);
                XbeeTransceiver.SetMinDistanceSettings((int)MinDistanceSlider.Value);
            }

            catch (XbeeReadException ex)
            {
                Logger.Write("Failed to send settings to the car!");
                Logger.Write($"Reason: {ex.Error}");
            }
        }

        private void ResetViewButtonOnClick(object sender, RoutedEventArgs e)
        {
            MapPlotter.Points.Clear();
            Viewport.Camera.LookDirection = new Vector3D(-2, 16, -20);
            Viewport.ZoomExtents(new Rect3D(0, -10, 0, 20, 20, 20), 20);
            _position = 0;
        }

        private async Task ExitAsync(int code)
        {
            Logger.Write("Application will exit in 5 seconds...");
            await Task.Delay(1000);

            Logger.Write("Application will exit in 4 seconds...");
            await Task.Delay(1000);

            Logger.Write("Application will exit in 3 seconds...");
            await Task.Delay(1000);

            Logger.Write("Application will exit in 2 seconds...");
            await Task.Delay(1000);

            Logger.Write("Application will exit in 1 seconds...");
            await Task.Delay(1000);

            Application.Current.Shutdown(code);
        }

        #endregion
    }
}
