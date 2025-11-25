using System;

namespace TurbineSimulator.Core.Sensors.Base
{
    /// <summary>
    /// Implementasi dasar untuk semua sensor.
    /// </summary>
    public abstract class SensorBase : ISensor
    {
        protected SensorBase(
            string name,
            string shortName,
            string inputQuantity,
            string inputUnit,
            string outputUnit,
            double sensitivity,
            double minFrequencyHz,
            double maxFrequencyHz)
        {
            Name = name;
            ShortName = shortName;
            InputQuantity = inputQuantity;
            InputUnit = inputUnit;
            OutputUnit = outputUnit;
            Sensitivity = sensitivity;
            MinFrequencyHz = minFrequencyHz;
            MaxFrequencyHz = maxFrequencyHz;
        }

        public string Name { get; }
        public string ShortName { get; }

        public string InputQuantity { get; }
        public string InputUnit { get; }
        public string OutputUnit { get; }

        public double MinFrequencyHz { get; }
        public double MaxFrequencyHz { get; }

        public double Sensitivity { get; }

        public virtual double ToVoltage(double inputPhysical)
            => inputPhysical * Sensitivity;

        public virtual double ToPhysical(double voltage)
            => Sensitivity == 0 ? 0.0 : voltage / Sensitivity;

        public virtual double[] SimulateTimeSeries(double[] time, Func<double, double> inputFunction)
        {
            if (time == null) throw new ArgumentNullException(nameof(time));
            if (inputFunction == null) throw new ArgumentNullException(nameof(inputFunction));

            var output = new double[time.Length];

            for (int i = 0; i < time.Length; i++)
            {
                double physical = inputFunction(time[i]);
                output[i] = ToVoltage(physical);
            }

            return output;
        }
    }
}
