using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mapper.Wpf.Hardwares
{
    public static class XbeeTransceiver
    {
        #region Properties

        public static bool IsConnectedToCar { get; set; }
        public static SerialPort Port { get; set; }

        #endregion


        #region Methods

        public static XbeeConnectStatus ConnectToCar(out string carName)
        {
            carName = null;

            Logger.Write("Getting list of connected ports...");
            string[] ports = SerialPort.GetPortNames();

            if (!ports.Any())
            {
                Logger.Write("0 connected ports found!");
                MessageBox.Show("Please connect the Xbee module before launching this application.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                return XbeeConnectStatus.NoXbeeFound;
            }

            foreach (var port in ports)
            {
                var serial = new SerialPort(port)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                    ReadBufferSize = 20480,
                    WriteBufferSize = 20480
                };

                try
                {
                    Logger.Write($"Opening port {port}...");
                    serial.Open();

                    Logger.Write($"Sending connect command to port {port}...");
                    serial.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_CONNECT + Constants.SERIAL_STOP_CHAR);

                    var response = ReadSerial(serial);
                    if (response == Constants.SERIAL_CMDRSPN_CONNECT)
                    {
                        Logger.Write($"Port {port} responded with {response}...");
                        Port = serial;
                        carName = response;
                        break;
                    }

                }

                catch
                {
                    continue;
                }
            }

            if (Port == null)
            {
                Logger.Write("No car found!");
                return XbeeConnectStatus.NoCarFound;
            }

            IsConnectedToCar = true;
            return XbeeConnectStatus.Connected;
        }

        public static bool DisconnectFromCar()
        {
            Logger.Write($"Sending disconnect command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_DISCONNECT + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_DISCONNECT)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                return true;
            }

            return false;
        }

        public static bool StartCar()
        {
            Logger.Write($"Sending start command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_START + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_SUCCESS)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                return true;
            }

            return false;
        }

        public static bool StopCar()
        {
            Logger.Write($"Sending stop command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_STOP + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_SUCCESS)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                return true;
            }

            return false;
        }

        public static SensorReadings GetMapData()
        {
            Logger.Write($"Sending get map data command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_GET_MAP_DATA + Constants.SERIAL_STOP_CHAR);

            var readings = new SensorReadings();

            try
            {
                for (int i = 0; i < 5; i++)
                {
                    var response = ReadSerial(Port);
                    switch (response)
                    {
                        case Constants.SERIAL_CMDRSPN_GET_MAP_DATA_000_DEG:
                            ReadDoubleFromSerial(Port);
                            var data000Degree = ReadDoubleFromSerial(Port);
                            readings.Reading0 = data000Degree;
                            break;

                        case Constants.SERIAL_CMDRSPN_GET_MAP_DATA_045_DEG:
                            ReadDoubleFromSerial(Port);
                            var data045Degree = ReadDoubleFromSerial(Port);
                            readings.Reading45 = data045Degree;
                            break;

                        case Constants.SERIAL_CMDRSPN_GET_MAP_DATA_090_DEG:
                            ReadDoubleFromSerial(Port);
                            var data090Degree = ReadDoubleFromSerial(Port);
                            readings.Reading90 = data090Degree;
                            break;

                        case Constants.SERIAL_CMDRSPN_GET_MAP_DATA_135_DEG:
                            ReadDoubleFromSerial(Port);
                            var data135Degree = ReadDoubleFromSerial(Port);
                            readings.Reading135 = data135Degree;
                            break;

                        case Constants.SERIAL_CMDRSPN_GET_MAP_DATA_180_DEG:
                            ReadDoubleFromSerial(Port);
                            var data180Degree = ReadDoubleFromSerial(Port);
                            readings.Reading180 = data180Degree;
                            break;
                    }
                }
            }

            catch (TimeoutException)
            {
                return readings;
            }

            return readings;
        }

        public static double GetSpeedSettings()
        {
            Logger.Write($"Sending get speed setting command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_GET_SETTINGS_SPEED + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_GET_SETTINGS_SPEED)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                var setting = ReadDoubleFromSerial(Port);
                if (setting < Constants.SETTINGS_SPEED_MIN_VALUE || setting > Constants.SETTINGS_SPEED_MAX_VALUE)
                {
                    throw new XbeeReadException
                    {
                        Error = XbeeReadError.InvalidData
                    };
                }

                return setting;
            }

            throw new XbeeReadException
            {
                Error = XbeeReadError.InvalidResponse
            };
        }

        public static double GetMinDistanceSettings()
        {
            Logger.Write($"Sending get min distance setting command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_GET_SETTINGS_MIN_DISTANCE + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_GET_SETTINGS_MIN_DISTANCE)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                var setting = ReadDoubleFromSerial(Port);
                if (setting < Constants.SETTINGS_MIN_DISTANCE_MIN_VALUE || setting > Constants.SETTINGS_MIN_DISTANCE_MAX_VALUE)
                {
                    throw new XbeeReadException
                    {
                        Error = XbeeReadError.InvalidData
                    };
                }

                return setting;
            }

            throw new XbeeReadException
            {
                Error = XbeeReadError.InvalidResponse
            };
        }

        public static bool SetSpeedSettings(int speed)
        {
            if (speed < Constants.SETTINGS_SPEED_MIN_VALUE || speed > Constants.SETTINGS_SPEED_MAX_VALUE)
            {
                throw new XbeeReadException
                {
                    Error = XbeeReadError.InvalidData
                };
            }

            Logger.Write($"Sending set speed settings command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_SET_SETTINGS_SPEED + Constants.SERIAL_STOP_CHAR);
            Port.Write(Constants.SERIAL_START_CHAR + speed.ToString() + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_SUCCESS)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                return true;
            }

            return false;
        }

        public static bool SetMinDistanceSettings(int distance)
        {
            if (distance < Constants.SETTINGS_MIN_DISTANCE_MIN_VALUE || distance > Constants.SETTINGS_MIN_DISTANCE_MAX_VALUE)
            {
                throw new XbeeReadException
                {
                    Error = XbeeReadError.InvalidData
                };
            }

            Logger.Write($"Sending set min distance settings command to port {Port.PortName}...");
            Port.DiscardInBuffer();
            Port.Write(Constants.SERIAL_START_CHAR + Constants.SERIAL_CMD_SET_SETTINGS_MIN_DISTANCE + Constants.SERIAL_STOP_CHAR);
            Port.Write(Constants.SERIAL_START_CHAR + distance.ToString() + Constants.SERIAL_STOP_CHAR);

            var response = ReadSerial(Port);
            if (response == Constants.SERIAL_CMDRSPN_SUCCESS)
            {
                Logger.Write($"Port {Port.PortName} responded with {response}...");
                return true;
            }

            return false;
        }

        private static string ReadSerial(SerialPort port)
        {
            if (port == null)
            {
                throw new XbeeReadException
                {
                    Error = XbeeReadError.NoCarConnected
                };
            }

            var data = port.ReadTo($"{Constants.SERIAL_STOP_CHAR}");
            var length = data.Length;
            var startIndex = 0;
            var validData = false;

            for (startIndex = 0; startIndex < length; startIndex++)
            {
                if (data[startIndex] == Constants.SERIAL_START_CHAR)
                {
                    validData = true;
                    break;
                }
            }

            if (!validData)
            {
                throw new XbeeReadException
                {
                    Error = XbeeReadError.InvalidData
                };
            }

            startIndex++;
            var dataString = new StringBuilder();
            for (int i = startIndex; i < length; i++)
            {
                dataString.Append(data[i]);
            }

            return dataString.ToString();

        }

        private static double ReadDoubleFromSerial(SerialPort port)
        {
            var response = ReadSerial(Port);
            if (!double.TryParse(response, out double data))
            {
                throw new XbeeReadException
                {
                    Error = XbeeReadError.InvalidData
                };
            }

            return data;
        }

        #endregion
    }
}
