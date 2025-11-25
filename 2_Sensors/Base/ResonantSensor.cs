namespace TurbineSimulator.Core.Sensors.Base
{
    /// <summary>
    /// Sensor dengan respon resonan (band-pass di sekitar frekuensi pusat).
    /// Di sini kita hanya menyimpan info frekuensi; filtering bisa dikerjakan di DSP.
    /// </summary>
    public abstract class ResonantSensor : SensorBase
    {
        protected ResonantSensor(
            string name,
            string shortName,
            string inputQuantity,
            string inputUnit,
            string outputUnit,
            double sensitivity,
            double minFrequencyHz,
            double maxFrequencyHz,
            double centerFrequencyHz,
            double bandwidthHz)
            : base(name, shortName, inputQuantity, inputUnit, outputUnit,
                   sensitivity, minFrequencyHz, maxFrequencyHz)
        {
            CenterFrequencyHz = centerFrequencyHz;
            BandwidthHz = bandwidthHz;
        }

        public double CenterFrequencyHz { get; }
        public double BandwidthHz { get; }
    }
}
