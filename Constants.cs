namespace Mapper.Wpf
{
    public static class Constants
    {
        // ReSharper disable InconsistentNaming
        public const char SERIAL_START_CHAR                          = '$';
        public const char SERIAL_STOP_CHAR                           = '*';
        public const string SERIAL_INVALID                           = "INVALID";
        public const string SERIAL_CMD_CONNECT                       = "CONNECT";
        public const string SERIAL_CMD_DISCONNECT                    = "DISCONNECT";
        public const string SERIAL_CMD_START                         = "START";
        public const string SERIAL_CMD_STOP                          = "STOP";
        public const string SERIAL_CMD_GET_MAP_DATA                  = "GET_MAP_DATA";
        public const string SERIAL_CMD_GET_SETTINGS_SPEED            = "GET_SETTINGS_SPEED";
        public const string SERIAL_CMD_GET_SETTINGS_MIN_DISTANCE     = "GET_SETTINGS_MIN_DISTANCE";
        public const string SERIAL_CMD_SET_SETTINGS_SPEED            = "SET_SETTINGS_SPEED";
        public const string SERIAL_CMD_SET_SETTINGS_MIN_DISTANCE     = "SET_SETTINGS_MIN_DISTANCE";
        public const string SERIAL_CMDRSPN_CONNECT                   = "TEST-CAR-1";
        public const string SERIAL_CMDRSPN_DISCONNECT                = "DISCONNECTED";
        public const string SERIAL_CMDRSPN_SUCCESS                   = "SUCCESS";
        public const string SERIAL_CMDRSPN_GET_MAP_DATA_000_DEG      = "MAP_DATA_000_DEG";
        public const string SERIAL_CMDRSPN_GET_MAP_DATA_045_DEG      = "MAP_DATA_045_DEG";
        public const string SERIAL_CMDRSPN_GET_MAP_DATA_090_DEG      = "MAP_DATA_090_DEG";
        public const string SERIAL_CMDRSPN_GET_MAP_DATA_135_DEG      = "MAP_DATA_135_DEG";
        public const string SERIAL_CMDRSPN_GET_MAP_DATA_180_DEG      = "MAP_DATA_180_DEG";
        public const string SERIAL_CMDRSPN_GET_SETTINGS_SPEED        = "SETTINGS_SPEED";
        public const string SERIAL_CMDRSPN_GET_SETTINGS_MIN_DISTANCE = "SETTINGS_MIN_DISTANCE";

        public const byte SETTINGS_SPEED_MIN_VALUE                   = 80;
        public const byte SETTINGS_SPEED_MAX_VALUE                   = 250;
        public const byte SETTINGS_MIN_DISTANCE_MIN_VALUE            = 10;
        public const byte SETTINGS_MIN_DISTANCE_MAX_VALUE            = 50;
        // ReSharper restore InconsistentNaming
    }
}
