using System;

namespace TurbineSimulator.Core.Sensors
{
    /// <summary>
    /// Interface umum semua sensor fisik.
    /// </summary>
    public interface ISensor
    {
        string Name { get; }
        string ShortName { get; }

        // Besaran fisis yang diukur
        string InputQuantity { get; }
        string InputUnit { get; }

        // Keluaran tipikal (di sini kita pakai Volt)
        string OutputUnit { get; }

        // Rentang frekuensi kerja (untuk dokumentasi / validasi)
        double MinFrequencyHz { get; }
        double MaxFrequencyHz { get; }

        /// <summary>
        /// Sensitivitas efektif (Volt per unit input) di daerah kerja utama.
        /// </summary>
        double Sensitivity { get; }

        /// <summary>
        /// Konversi dari besaran fisis ke tegangan sensor.
        /// </summary>
        double ToVoltage(double inputPhysical);

        /// <summary>
        /// Konversi dari tegangan ke besaran fisis.
        /// </summary>
        double ToPhysical(double voltage);

        /// <summary>
        /// Simulasi sinyal waktu berdasarkan fungsi input(t).
        /// </summary>
        double[] SimulateTimeSeries(double[] time, Func<double, double> inputFunction);
    }
}
