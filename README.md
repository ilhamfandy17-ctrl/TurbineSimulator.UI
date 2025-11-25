# Turbine Sensor Simulator (TurbineSimulator.UI)

Aplikasi WPF untuk Windows yang mensimulasikan dan memvisualisasikan sinyal sensor yang umum digunakan dalam pemantauan turbin/hidro. UI ini menampilkan plot domain waktu dan domain frekuensi untuk beberapa model sensor dan menyediakan slider untuk mengubah parameter sinyal secara interaktif.

---

## 🚀 Ringkasan

Turbine Sensor Simulator menyediakan antarmuka interaktif untuk:
- Menghasilkan sinyal sinusoidal yang meredam secara eksponensial untuk lima model sensor.
- Memvisualisasikan sinyal domain waktu dan amplitudo domain frekuensi menggunakan FFT dari MathNet.Numerics.
- Memvisualisasikan contoh plot domain S dan Z, serta menampilkan gambar instalasi hidro.

UI menggunakan ScottPlot.WPF untuk plotting dan dapat dibangun dengan Visual Studio atau dotnet CLI.

---

## ✅ Fitur

- Slider interaktif untuk Amplitudo (A), Frekuensi (F), Decay (R) dan Phase (P) pada setiap sensor.
- Plot domain waktu dan domain frekuensi untuk Kistler 601C, PAC R15a, PCB 333B50, Reson TC4013 dan Siemens 5100W.
- Sinyal gabungan R(t) = z1(t) + z2(t) + z3(t) + z4(t) + z5(t).
- Contoh visualisasi domain S dan Z.
- Utilitas simulasi sensor diimplementasikan pada folder `2_Sensors/` dan implementasi kelas sensor.

---

## ⚙️ Prasyarat

- Windows 10/11 (proyek ini menargetkan WPF sehingga membutuhkan Windows untuk UI).
- .NET 8.0 SDK (atau versi lebih baru) dengan komponen Windows Desktop terpasang.
- Visual Studio 2022/2023 (direkomendasikan) dengan workload .NET desktop, atau dotnet CLI.

Paket NuGet yang digunakan (secara otomatis akan di-restore):
- MathNet.Numerics (FFT)
- ScottPlot.WPF (plot interaktif)
- OxyPlot.Wpf (opsional; termasuk sebagai referensi)

Catatan: UI ini mereferensikan proyek saudara `TurbineSimulator.Core` pada folder parent. Pastikan folder `TurbineSimulator.Core` berada pada tingkat yang sama dengan folder ini, misalnya:

```
..\TurbineSimulator.Core\TurbineSimulator.Core.csproj
..\TurbineSimulator.UI\TurbineSimulator.UI.csproj
```

Jika Anda tidak memiliki `TurbineSimulator.Core`, Anda dapat meng-clone-nya ke parent folder atau mengedit `TurbineSimulator.UI.csproj` untuk menghapus/memodifikasi referensi project tersebut.

---

## 🛠️ Membangun & Menjalankan

Menggunakan Visual Studio:
1. Buka solusi `TurbineSimulator.UI.sln`.
2. Restore paket NuGet bila diperlukan (Visual Studio umumnya akan meng-restorenya otomatis saat membuka solusi).
3. Pastikan jalur `TurbineSimulator.Core` dapat ditemukan relatif terhadap solusi (referensi project: `..\\TurbineSimulator.Core\\TurbineSimulator.Core.csproj`).
4. Atur `TurbineSimulator.UI` sebagai startup project lalu tekan F5 untuk menjalankan.

Menggunakan dotnet CLI (Windows PowerShell):

```powershell
# Dari root solusi (tempat file TurbineSimulator.UI.sln berada)
dotnet restore
dotnet build
dotnet run --project .\TurbineSimulator.UI\TurbineSimulator.UI.csproj
```

---

## 🗂️ Struktur Proyek

- `Views/` — WPF Views (.xaml) termasuk `MainWindow.xaml`.
- `2_Sensors/` — Interface sensor (`ISensor.cs`), kelas dasar dan implementasi sensor.
- `Assets/` — Aset statis (gambar, dll.). `hydropower.png` disertakan sebagai Resource pada proyek.
- `RelayCommand.cs` — RelayCommand sederhana untuk penggunaan MVVM.
- `TurbineSimulator.UI.csproj` — File proyek WPF yang menggunakan .NET 8.0.

---

## 🔍 Ringkasan `MainWindow` (catatan singkat)

- Kontrol: Slider untuk amplitudo, frekuensi, decay, phase pada 5 sensor.
- Plot domain waktu: menggunakan komponen `WpfPlot` dari ScottPlot.
- Plot domain frekuensi: memakai FFT dari MathNet.Numerics untuk menghitung magnitudo spektral.
- Metode `UpdatePlot` dan `ComputeFFT` diimplementasikan di `MainWindow.xaml.cs` untuk perhitungan sinyal dan plotting.

---

## 🧑‍💻 Catatan Pengembangan

- Fork repositori dan buat feature-branch untuk perubahan baru.
- Kontribusi: tambahkan sensor baru di bawah `2_Sensors/Implementations` dan perbarui `MainWindow` untuk menghubungkan sensor tersebut ke UI.
- Kelas sensor sebaiknya mengimplementasikan interface `ISensor` atau memperluas `SensorBase`.
- Pastikan UI tetap responsif — banyak plot diperbarui saat slider diubah.

---

## ⚠️ Pemecahan Masalah

- Jika terjadi referensi `TurbineSimulator.Core` hilang, pastikan folder `TurbineSimulator.Core` berada sejajar dengan folder proyek ini dan file `TurbineSimulator.Core.csproj` ada pada jalur yang dirujuk.
- Jika kontrol plot tidak tampil, pastikan paket NuGet ScottPlot.WPF telah di-restore.
- Jika terjadi exception saat menjalankan: jalankan `dotnet build` dan baca pesan error build; mayoritas masalah berasal dari referensi project yang hilang atau SDK yang tidak terinstal.

---

## 📎 Kontribusi

- Silakan buka issue untuk bug atau permintaan fitur baru.
- Gunakan pesan commit dan deskripsi PR yang informatif.
- Pertimbangkan menambahkan unit test agar perilaku tetap konsisten.

---

## 📜 Lisensi

Tidak ditemukan file lisensi di repositori. Tambahkan file `LICENSE` jika Anda ingin membuka proyek ini sebagai open-source (mis. MIT, Apache 2.0). Jika proyek ini bersifat privat, abaikan bagian ini.

---

## 📞 Kontak

- Pemilik repositori / kontributor: Tambahkan info kontak atau username GitHub Anda di sini.

---

Jika Anda mau, saya juga bisa:
- Menambahkan file `LICENSE` (pilih: MIT/Apache/GPL).
- Menambahkan panduan pengembangan singkat atau screenshot / GIF pada README.
- Menambahkan instruksi langkah-demi-langkah untuk membangun `TurbineSimulator.Core` agar README dapat membangun kedua proyek secara otomatis.

💡 Tips: Jika mau versi README dalam bahasa lain selain Bahasa Indonesia, beri tahu saya dan saya akan siapkan terjemahan.