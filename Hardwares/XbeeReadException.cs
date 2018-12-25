using System;

namespace Mapper.Wpf.Hardwares
{
    public class XbeeReadException : Exception
    {
        public XbeeReadError Error { get; set; }
    }
}
