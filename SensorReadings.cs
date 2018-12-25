namespace Mapper.Wpf
{
    public class SensorReadings
    {
        public SensorReadings() { }

        public SensorReadings(double reading0,
                              double reading45,
                              double reading90,
                              double reading135,
                              double reading180)
        {
            Reading0 = reading0;
            Reading45 = reading45;
            Reading90 = reading90;
            Reading135 = reading135;
            Reading180 = reading180;
        }

        public double Reading0 { get; set; }
        public double Reading45 { get; set; }
        public double Reading90 { get; set; }
        public double Reading135 { get; set; }
        public double Reading180 { get; set; }
    }
}
