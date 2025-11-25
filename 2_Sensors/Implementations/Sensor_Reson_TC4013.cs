using ScottPlot.WPF;
using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace TurbineSimulator.UI.Sensors
{
    public class Sensor_Reson
    {
        public double A = 1;
        public double F = 30000;
        public double R = -900;
        public double P = 0;

        public double[] TimeSignal = Array.Empty<double>();
        public double[] FreqMag = Array.Empty<double>();

        private readonly double[] time;
        private readonly double sampleRate = 1000.0;

        public Sensor_Reson(double[] timeVector)
        {
            time = timeVector;
        }

        private double[] Generate()
        {
            return time.Select(t =>
                A * Math.Exp(R * t) * Math.Cos(2 * Math.PI * F * t + P)
            ).ToArray();
        }

        private double[] FFT(double[] x)
        {
            int n = 1;
            while (n < x.Length) n <<= 1;

            Complex[] buffer = new Complex[n];
            for (int i = 0; i < x.Length; i++)
                buffer[i] = new Complex(x[i], 0);

            Fourier.Forward(buffer, FourierOptions.Matlab);

            return buffer.Take(n / 2).Select(c => c.Magnitude).ToArray();
        }

        public void UpdatePlots(WpfPlot timePlot, WpfPlot freqPlot)
        {
            TimeSignal = Generate();
            FreqMag = FFT(TimeSignal);

            timePlot.Plot.Clear();
            timePlot.Plot.Add.Scatter(time, TimeSignal);
            timePlot.Refresh();

            double[] freqAxis = Enumerable.Range(0, FreqMag.Length)
                .Select(i => i * sampleRate / (2.0 * FreqMag.Length))
                .ToArray();

            freqPlot.Plot.Clear();
            freqPlot.Plot.Add.Scatter(freqAxis, FreqMag);
            freqPlot.Refresh();
        }
    }
}
