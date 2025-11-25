namespace TurbineSimulator.Core.Sensors.Base
{
    /// <summary>
    /// Sensor dengan karakter low-pass / quasi-DC.
    /// </summary>
    public abstract class LowPassSensor : SensorBase
    {
        protected LowPassSensor(
            string name,
            string shortName,
            string inputQuantity,
            string inputUnit,
            string outputUnit,
            double sensitivity,
            double minFrequencyHz,
            double maxFrequencyHz,
            double cutoffFrequencyHz)
            : base(name, shortName, inputQuantity, inputUnit, outputUnit,
                   sensitivity, minFrequencyHz, maxFrequencyHz)
        {
            CutoffFrequencyHz = cutoffFrequencyHz;
        }

        public double CutoffFrequencyHz { get; }
    }
}
