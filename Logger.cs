using System;


namespace Mapper.Wpf
{
    public static class Logger
    {
        #region Events

        public static EventHandler<string> LogWritten;

        #endregion


        #region Methods

        public static void Write(string log)
        {
            LogWritten?.Invoke(null, $"{log}\r\n");
        }

        #endregion
    }
}
