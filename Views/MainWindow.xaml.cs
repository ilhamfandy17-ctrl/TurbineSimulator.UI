using MathNet.Numerics.IntegralTransforms;
using ScottPlot.WPF;
using System;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace TurbineSimulator.UI
{
    public partial class MainWindow : Window
    {
        // ============================
        // MODEL DATA SENSOR
        // ============================
        public class SensorData
        {
            public double A = 1;      // amplitude
            public double F = 1000;   // frequency (Hz)
            public double R = -10;    // decay koefisien
            public double P = 0;      // phase (rad)

            public double[] TimeSignal = Array.Empty<double>();
            public double[] FreqMag = Array.Empty<double>();
        }

        // Lima sensor
        private readonly SensorData kistler = new();
        private readonly SensorData pac = new();
        private readonly SensorData pcb = new();
        private readonly SensorData reson = new();
        private readonly SensorData siemens = new();

        // Waktu sampling
        private readonly double[] timeVector;
        private readonly double sampleRate = 1000.0; // Hz (Δt = 1 ms)

        public MainWindow()
        {
            InitializeComponent();

            // vektor waktu 0–1 detik, 1000 sampel
            timeVector = Enumerable.Range(0, 1000)
                                   .Select(i => i / sampleRate)
                                   .ToArray();

            InitPlots();

            // set nilai awal slider & gambar pertama kali setelah Window siap
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // kasih nilai default biar semua sensor punya sinyal awal
            if (SliderA_Kistler != null) SliderA_Kistler.Value = 5;
            if (SliderF_Kistler != null) SliderF_Kistler.Value = 1000;
            if (SliderR_Kistler != null) SliderR_Kistler.Value = -500;
            if (SliderP_Kistler != null) SliderP_Kistler.Value = 0;

            if (SliderA_PAC != null) SliderA_PAC.Value = 5;
            if (SliderF_PAC != null) SliderF_PAC.Value = 20000;
            if (SliderR_PAC != null) SliderR_PAC.Value = -1000;
            if (SliderP_PAC != null) SliderP_PAC.Value = 0;

            if (SliderA_PCB != null) SliderA_PCB.Value = 5;
            if (SliderF_PCB != null) SliderF_PCB.Value = 1000;
            if (SliderR_PCB != null) SliderR_PCB.Value = -100;
            if (SliderP_PCB != null) SliderP_PCB.Value = 0;

            if (SliderA_Reson != null) SliderA_Reson.Value = 5;
            if (SliderF_Reson != null) SliderF_Reson.Value = 50000;
            if (SliderR_Reson != null) SliderR_Reson.Value = -500;
            if (SliderP_Reson != null) SliderP_Reson.Value = 0;

            if (SliderA_Siemens != null) SliderA_Siemens.Value = 5;
            if (SliderF_Siemens != null) SliderF_Siemens.Value = 10;
            if (SliderR_Siemens != null) SliderR_Siemens.Value = -2;
            if (SliderP_Siemens != null) SliderP_Siemens.Value = 0;

            // paksa update semua grafik awal
            UpdatePlotSafe(PlotTime_Kistler, PlotFreq_Kistler, kistler);
            UpdatePlotSafe(PlotTime_PAC, PlotFreq_PAC, pac);
            UpdatePlotSafe(PlotTime_PCB, PlotFreq_PCB, pcb);
            UpdatePlotSafe(PlotTime_Reson, PlotFreq_Reson, reson);
            UpdatePlotSafe(PlotTime_Siemens, PlotFreq_Siemens, siemens);
        }

        // ==========================================
        // Inisialisasi plot kosong
        // ==========================================
        private void InitPlots()
        {
            WpfPlot[] plots =
            {
                PlotTime_Kistler, PlotTime_PAC, PlotTime_PCB, PlotTime_Reson, PlotTime_Siemens,
                PlotFreq_Kistler, PlotFreq_PAC, PlotFreq_PCB, PlotFreq_Reson, PlotFreq_Siemens,
                PlotSDomain, PlotZDomain
            };

            foreach (var wp in plots)
            {
                if (wp == null) continue;

                wp.Plot.Clear();
                wp.Plot.Axes.AutoScale();
                wp.Refresh();
            }
        }

        // ==========================================
        // Generate sinyal time-domain: 
        // z(t) = A * e^(R t) * cos(2π F t + P)
        // ==========================================
        private double[] GenerateSignal(SensorData s)
        {
            return timeVector
                .Select(t => s.A * Math.Exp(s.R * t) * Math.Cos(2 * Math.PI * s.F * t + s.P))
                .ToArray();
        }

        // ==========================================
        // FFT dengan MathNet
        // ==========================================
        private double[] ComputeFFT(double[] x)
        {
            if (x == null || x.Length == 0)
                return Array.Empty<double>();

            int n = 1;
            while (n < x.Length) n <<= 1;

            Complex[] buffer = new Complex[n];

            for (int i = 0; i < x.Length; i++)
                buffer[i] = new Complex(x[i], 0);

            // sisanya nol (default)

            Fourier.Forward(buffer, FourierOptions.Matlab);

            int half = n / 2;
            double[] mag = new double[half];
            for (int i = 0; i < half; i++)
                mag[i] = buffer[i].Magnitude;

            return mag;
        }

        // ==========================================
        // Update satu sensor (time + freq)
        // ==========================================
        private void UpdatePlot(WpfPlot timePlot, WpfPlot freqPlot, SensorData s)
        {
            if (timePlot == null || freqPlot == null || s == null)
                return;

            s.TimeSignal = GenerateSignal(s);
            s.FreqMag = ComputeFFT(s.TimeSignal);

            // Time domain
            timePlot.Plot.Clear();
            timePlot.Plot.Add.Scatter(timeVector, s.TimeSignal);
            timePlot.Plot.Axes.AutoScale();
            timePlot.Refresh();

            // Frequency domain
            int n = s.FreqMag.Length;
            if (n > 0)
            {
                double[] freqAxis = Enumerable.Range(0, n)
                                              .Select(i => i * sampleRate / (2.0 * n))
                                              .ToArray();

                freqPlot.Plot.Clear();
                freqPlot.Plot.Add.Scatter(freqAxis, s.FreqMag);
                freqPlot.Plot.Axes.AutoScale();
                freqPlot.Refresh();
            }
            else
            {
                freqPlot.Plot.Clear();
                freqPlot.Refresh();
            }

            UpdateCombinedDomains();
        }

        // versi aman (dipakai di Loaded)
        private void UpdatePlotSafe(WpfPlot timePlot, WpfPlot freqPlot, SensorData s)
        {
            try
            {
                UpdatePlot(timePlot, freqPlot, s);
            }
            catch
            {
                // kalau ada masalah kecil, jangan bikin crash
            }
        }

        // ==========================================
        // Hitung gabungan R(t) + plot S & Z domain
        // ==========================================
        private void UpdateCombinedDomains()
        {
            // pastikan semua sensor sudah punya TimeSignal
            if (kistler.TimeSignal == null || kistler.TimeSignal.Length == 0)
                return;
            if (pac.TimeSignal == null || pac.TimeSignal.Length == 0)
                return;
            if (pcb.TimeSignal == null || pcb.TimeSignal.Length == 0)
                return;
            if (reson.TimeSignal == null || reson.TimeSignal.Length == 0)
                return;
            if (siemens.TimeSignal == null || siemens.TimeSignal.Length == 0)
                return;

            int n = timeVector.Length;
            double[] R = new double[n];

            for (int i = 0; i < n; i++)
            {
                R[i] =
                    kistler.TimeSignal[i] +
                    pac.TimeSignal[i] +
                    pcb.TimeSignal[i] +
                    reson.TimeSignal[i] +
                    siemens.TimeSignal[i];
            }

            // Contoh S-domain: G(s) = 1 / (15 s + 1)
            double[] sAxis = Enumerable.Range(-50, 101)
                                       .Select(i => i / 10.0)
                                       .ToArray();
            double[] sVal = sAxis
                .Select(s => 1.0 / (15.0 * s + 1.0))
                .ToArray();

            if (PlotSDomain != null)
            {
                PlotSDomain.Plot.Clear();
                PlotSDomain.Plot.Add.Scatter(sAxis, sVal);
                PlotSDomain.Plot.Axes.AutoScale();
                PlotSDomain.Refresh();
            }

            // Z-domain: contoh mapping ke lingkaran satuan
            double[] zReal = sAxis.Select(s => Math.Cos(s)).ToArray();
            double[] zImag = sAxis.Select(s => Math.Sin(s)).ToArray();

            if (PlotZDomain != null)
            {
                PlotZDomain.Plot.Clear();
                PlotZDomain.Plot.Add.Scatter(zReal, zImag);
                PlotZDomain.Plot.Axes.AutoScale();
                PlotZDomain.Refresh();
            }
        }

        // ======================================================
        // HANDLER SLIDER – KISTLER (SUDAH DIPERBAIKI & AMAN)
        // ======================================================
        private void SliderA_Kistler_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kistler.A = e.NewValue;

            if (TxtA_Kistler != null)
                TxtA_Kistler.Text = e.NewValue.ToString("F2");

            // cek window sudah loaded supaya tidak error saat konstruksi
            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Kistler, PlotFreq_Kistler, kistler);
        }

        private void SliderF_Kistler_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kistler.F = e.NewValue;

            if (TxtF_Kistler != null)
                TxtF_Kistler.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Kistler, PlotFreq_Kistler, kistler);
        }

        private void SliderR_Kistler_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kistler.R = e.NewValue;

            if (TxtR_Kistler != null)
                TxtR_Kistler.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Kistler, PlotFreq_Kistler, kistler);
        }

        private void SliderP_Kistler_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kistler.P = e.NewValue;

            if (TxtP_Kistler != null)
                TxtP_Kistler.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Kistler, PlotFreq_Kistler, kistler);
        }

        // ======================================================
        // HANDLER SENSOR LAIN (POLA SAMA, JUGA AMAN NULL)
        // ======================================================

        // PAC R15a
        private void SliderA_PAC_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pac.A = e.NewValue;
            if (TxtA_PAC != null)
                TxtA_PAC.Text = e.NewValue.ToString("F2");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PAC, PlotFreq_PAC, pac);
        }

        private void SliderF_PAC_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pac.F = e.NewValue;
            if (TxtF_PAC != null)
                TxtF_PAC.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PAC, PlotFreq_PAC, pac);
        }

        private void SliderR_PAC_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pac.R = e.NewValue;
            if (TxtR_PAC != null)
                TxtR_PAC.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PAC, PlotFreq_PAC, pac);
        }

        private void SliderP_PAC_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pac.P = e.NewValue;
            if (TxtP_PAC != null)
                TxtP_PAC.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PAC, PlotFreq_PAC, pac);
        }

        // PCB 333B50
        private void SliderA_PCB_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pcb.A = e.NewValue;
            if (TxtA_PCB != null)
                TxtA_PCB.Text = e.NewValue.ToString("F2");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PCB, PlotFreq_PCB, pcb);
        }

        private void SliderF_PCB_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pcb.F = e.NewValue;
            if (TxtF_PCB != null)
                TxtF_PCB.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PCB, PlotFreq_PCB, pcb);
        }

        private void SliderR_PCB_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pcb.R = e.NewValue;
            if (TxtR_PCB != null)
                TxtR_PCB.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PCB, PlotFreq_PCB, pcb);
        }

        private void SliderP_PCB_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pcb.P = e.NewValue;
            if (TxtP_PCB != null)
                TxtP_PCB.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_PCB, PlotFreq_PCB, pcb);
        }

        // Reson TC4013
        private void SliderA_Reson_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reson.A = e.NewValue;
            if (TxtA_Reson != null)
                TxtA_Reson.Text = e.NewValue.ToString("F2");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Reson, PlotFreq_Reson, reson);
        }

        private void SliderF_Reson_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reson.F = e.NewValue;
            if (TxtF_Reson != null)
                TxtF_Reson.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Reson, PlotFreq_Reson, reson);
        }

        private void SliderR_Reson_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reson.R = e.NewValue;
            if (TxtR_Reson != null)
                TxtR_Reson.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Reson, PlotFreq_Reson, reson);
        }

        private void SliderP_Reson_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            reson.P = e.NewValue;
            if (TxtP_Reson != null)
                TxtP_Reson.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Reson, PlotFreq_Reson, reson);
        }

        // Siemens 5100W
        private void SliderA_Siemens_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            siemens.A = e.NewValue;
            if (TxtA_Siemens != null)
                TxtA_Siemens.Text = e.NewValue.ToString("F2");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Siemens, PlotFreq_Siemens, siemens);
        }

        private void SliderF_Siemens_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            siemens.F = e.NewValue;
            if (TxtF_Siemens != null)
                TxtF_Siemens.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Siemens, PlotFreq_Siemens, siemens);
        }

        private void SliderR_Siemens_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            siemens.R = e.NewValue;
            if (TxtR_Siemens != null)
                TxtR_Siemens.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Siemens, PlotFreq_Siemens, siemens);
        }

        private void SliderP_Siemens_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            siemens.P = e.NewValue;
            if (TxtP_Siemens != null)
                TxtP_Siemens.Text = e.NewValue.ToString("F0");

            if (IsLoaded)
                UpdatePlotSafe(PlotTime_Siemens, PlotFreq_Siemens, siemens);
        }

        // ======================================================
        // TOMBOL RUMUS
        // ======================================================
        private void BtnKistlerFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kistler 601C:\nz(t) = A · e^(r t) · cos(2π f t + φ)");
        }

        private void BtnPACFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("PAC R15a:\nz(t) = A · e^(r t) · cos(2π f t + φ)");
        }

        private void BtnPCBFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("PCB 333B50:\nz(t) = A · e^(r t) · cos(2π f t + φ)");
        }

        private void BtnResonFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reson TC4013:\nz(t) = A · e^(r t) · cos(2π f t + φ)");
        }

        private void BtnSiemensFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Siemens 5100W:\nz(t) = A · e^(r t) · cos(2π f t + φ)");
        }
    }
}
