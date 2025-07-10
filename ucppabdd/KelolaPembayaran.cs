using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Caching; // Tambahkan namespace ini untuk MemoryCache

namespace ucppabdd
{
    public partial class KelolaPembayaran : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";
       
        public KelolaPembayaran()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
            InitializeCustomComponents(); // Panggil metode inisialisasi kustom
        }

        // Metode inisialisasi kustom untuk mengatur semua komponen saat form dimuat
        private void InitializeCustomComponents()
        {
            LoadComboBoxData(); // Muat data untuk ComboBoxes (dengan optimasi cache)
            LoadData(); // Muat data pembayaran ke DataGridView
            // Menghubungkan event CellClick pada DataGridView ke fungsi
            // Ketika sel di DataGridView diklik, data akan mengisi kolom input.
            dataGridViewKelolaPembayaran.CellClick += dataGridViewKelolaPembayaran_CellContentClick;

            // Jangan set MinDate. Biarkan pengguna bisa melihat tanggal sebelumnya.
            // dtpTanggalPembayaran.MinDate = DateTime.Today; // Baris ini DIHAPUS atau DIKOMEN
        }

        private void KelolaPembayaran_Load(object sender, EventArgs e)
        {
            // Event Load ini tidak memerlukan kode tambahan karena semua inisialisasi
            // sudah ditangani di konstruktor melalui InitializeCustomComponents().
        }

        /// Metode untuk memuat data ke ComboBoxes dengan MemoryCache
        private void LoadComboBoxData()
        {
            // Bersihkan item ComboBoxes sebelum memuat data baru
            cmbNamaPeserta.DataSource = null;
            cmbNamaPeserta.Items.Clear();
            cmbMetodePembayaran.Items.Clear();
            cmbStatus.Items.Clear();

            // 1. Memuat data Nama Peserta ke cmbNamaPeserta (dengan MemoryCache)
            DataTable dtPeserta = null;
            // Dapatkan instance cache default
            MemoryCache cache = MemoryCache.Default;
            string cacheKey = "PesertaDataCache"; // Kunci unik untuk data peserta di cache

            // Coba ambil data peserta dari cache
            dtPeserta = cache.Get(cacheKey) as DataTable;

            if (dtPeserta == null) // Jika data tidak ada di cache, muat dari database
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string queryPeserta = "SELECT id_peserta, nama FROM data_peserta ORDER BY nama";
                        SqlDataAdapter daPeserta = new SqlDataAdapter(queryPeserta, con);
                        dtPeserta = new DataTable();
                        daPeserta.Fill(dtPeserta);

                        // Tambahkan data ke cache dengan kebijakan kadaluarsa (misal: 10 menit)
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10) // Data akan kadaluarsa setelah 10 menit
                        };
                        cache.Set(cacheKey, dtPeserta, policy);

                        Console.WriteLine("Data peserta dimuat dari database dan disimpan ke cache."); // Untuk debug
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Terjadi kesalahan database saat memuat daftar peserta: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Keluar dari fungsi jika ada kesalahan database
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Terjadi kesalahan tidak terduga saat memuat daftar peserta: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Keluar dari fungsi jika ada kesalahan umum
                    }
                }
            }
            else
            {
                Console.WriteLine("Data peserta dimuat dari cache."); // Untuk debug
            }

            // Pastikan dtPeserta tidak null sebelum digunakan
            if (dtPeserta != null)
            {
                // Tambahkan item placeholder jika ada data. Jika tidak ada data sama sekali, placeholder saja.
                DataRow placeholderPeserta = dtPeserta.NewRow();
                placeholderPeserta["id_peserta"] = -1; // Nilai ID yang tidak valid untuk placeholder
                placeholderPeserta["nama"] = "-- Pilih Peserta --"; // Teks placeholder
                dtPeserta.Rows.InsertAt(placeholderPeserta, 0); // Sisipkan di awal DataTable

                cmbNamaPeserta.DataSource = dtPeserta;
                cmbNamaPeserta.DisplayMember = "nama"; // Kolom yang ditampilkan di ComboBox
                cmbNamaPeserta.ValueMember = "id_peserta"; // Kolom nilai yang akan diambil
                cmbNamaPeserta.SelectedIndex = 0; // Pilih item placeholder secara default
            }

            // 2. Mengisi cmbMetodePembayaran (data statis, tidak perlu cache)
            cmbMetodePembayaran.Items.Add("-- Pilih Metode --"); // Placeholder
            cmbMetodePembayaran.Items.Add("Transfer");
            cmbMetodePembayaran.Items.Add("Tunai");
            cmbMetodePembayaran.SelectedIndex = 0; // Pilih placeholder secara default

            // 3. Mengisi cmbStatus (data statis, tidak perlu cache)
            cmbStatus.Items.Add("-- Pilih Status --"); // Placeholder
            cmbStatus.Items.Add("Pending");
            cmbStatus.Items.Add("Lunas");
            cmbStatus.SelectedIndex = 0; // Pilih placeholder secara default
        }

        // --- Penambahan Data ---
        private void btnTambah_Click(object sender, EventArgs e)
        {
            // --- Validasi Input Kosong (termasuk ComboBox) ---
            if (cmbNamaPeserta.SelectedValue == null || (int)cmbNamaPeserta.SelectedValue == -1 || // Pastikan bukan placeholder peserta
                cmbMetodePembayaran.SelectedIndex == 0 || // Pastikan bukan placeholder metode
                cmbStatus.SelectedIndex == 0 || // Pastikan bukan placeholder status
                string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("Semua kolom (Nama Peserta, Jumlah, Metode Pembayaran, Status) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Parsing Input Numerik dan Lainnya dari Control ---
            int idPeserta = (int)cmbNamaPeserta.SelectedValue;
            decimal jumlah;
            // Menggunakan CultureInfo.InvariantCulture untuk parsing yang konsisten (mengabaikan pengaturan regional)
            if (!decimal.TryParse(txtJumlah.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out jumlah))
            {
                MessageBox.Show("Jumlah pembayaran tidak valid. Masukkan angka yang valid (gunakan titik '.' sebagai pemisah desimal).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string metode = cmbMetodePembayaran.SelectedItem.ToString();
            string status = cmbStatus.SelectedItem.ToString();
            DateTime tanggalPembayaran = dtpTanggalPembayaran.Value; // Ambil nilai dari DateTimePicker

            // REVISI: Validasi Tanggal Pembayaran (tidak boleh di masa lalu)
            // Bandingkan hanya bagian tanggalnya saja (tanpa waktu) untuk akurasi
            if (tanggalPembayaran.Date < DateTime.Today.Date)
            {
                MessageBox.Show("Tanggal pembayaran tidak boleh di masa lalu. Silakan pilih tanggal hari ini atau setelahnya.", "Kesalahan Input Tanggal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Hentikan proses jika tanggal tidak valid
            }

            // --- Konfirmasi Penambahan Data ---
            DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin menambahkan data pembayaran ini?", "Konfirmasi Penambahan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.No)
            {
                return; // Batalkan operasi jika pengguna memilih "Tidak"
            }

            // --- Proses Penambahan Data dengan Transaksi ---
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null; // Deklarasikan transaksi

                try
                {
                    con.Open();
                    transaction = con.BeginTransaction(); // Mulai transaksi

                    // Gunakan Stored Procedure 'sp_TambahPembayaran' untuk menambah data
                    using (SqlCommand cmd = new SqlCommand("sp_TambahPembayaran", con, transaction)) // Kaitkan command dengan transaksi
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Tambah parameter ke stored procedure
                        cmd.Parameters.AddWithValue("@id_peserta", idPeserta);
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);
                        cmd.Parameters.AddWithValue("@metode_pembayaran", metode);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@tanggal_pembayaran", tanggalPembayaran); // Tambahkan parameter tanggal

                        cmd.ExecuteNonQuery(); // Eksekusi stored procedure

                        transaction.Commit(); // Komit transaksi jika semua berhasil
                        MessageBox.Show("Data pembayaran berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm(); // Bersihkan formulir
                        LoadData(); // Muat ulang data ke DataGridView
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback(); // Rollback transaksi jika terjadi error SQL
                    }
                    if (sqlEx.Number == 547) // Foreign Key constraint violation (ID Peserta tidak ditemukan)
                    {
                        MessageBox.Show("Gagal menambahkan pembayaran: ID Peserta tidak ditemukan. Pastikan ID Peserta yang Anda pilih valid dan ada di database.", "Kesalahan Database: ID Peserta Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback(); // Rollback transaksi jika terjadi error umum
                    }
                    MessageBox.Show($"Terjadi kesalahan tidak terduga saat menambahkan data: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- Penghapusan Data ---
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaPembayaran.CurrentRow != null)
            {
                // --- Validasi ID Pembayaran untuk Hapus ---
                if (dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value == null)
                {
                    MessageBox.Show("ID Pembayaran tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idPembayaran;
                if (!int.TryParse(dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value.ToString(), out idPembayaran))
                {
                    MessageBox.Show("Gagal mendapatkan ID Pembayaran. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Konfirmasi Penghapusan ---
                var confirm = MessageBox.Show("Yakin ingin menghapus data pembayaran ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null; // Deklarasikan transaksi
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            // Gunakan Stored Procedure 'sp_DeletePembayaran' untuk menghapus data
                            using (SqlCommand cmd = new SqlCommand("sp_DeletePembayaran", con, transaction)) // Kaitkan dengan transaksi
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaran);

                                // ExecuteNonQuery cocok untuk DELETE karena tidak mengembalikan data, hanya jumlah baris terpengaruh
                                int rowsAffected = cmd.ExecuteNonQuery();

                                transaction.Commit(); // Komit jika sukses
                                MessageBox.Show("Data berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearForm(); // Bersihkan formulir
                                LoadData(); // Muat ulang data ke DataGridView

                            }

                        }
                        catch (SqlException sqlEx)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback(); // Rollback pada error SQL
                            }
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback(); // Rollback pada error umum
                            }
                            MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih baris yang ingin dihapus di tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Pembaharuan Data ---
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaPembayaran.CurrentRow != null)
            {
                // --- Validasi ID Pembayaran untuk Update ---
                if (dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value == null)
                {
                    MessageBox.Show("ID Pembayaran tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid untuk diupdate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idPembayaranToUpdate;
                if (!int.TryParse(dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value.ToString(), out idPembayaranToUpdate))
                {
                    MessageBox.Show("Gagal mendapatkan ID Pembayaran untuk update. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Validasi Input Kosong (termasuk ComboBox) ---
                if (cmbNamaPeserta.SelectedValue == null || (int)cmbNamaPeserta.SelectedValue == -1 || // Pastikan bukan placeholder peserta
                    cmbMetodePembayaran.SelectedIndex == 0 || // Pastikan bukan placeholder metode
                    cmbStatus.SelectedIndex == 0 || // Pastikan bukan placeholder status
                    string.IsNullOrWhiteSpace(txtJumlah.Text))
                {
                    MessageBox.Show("Semua kolom (Nama Peserta, Jumlah, Metode Pembayaran, Status) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validasi dan konversi input dari ComboBoxes dan TextBox
                int idPeserta = (int)cmbNamaPeserta.SelectedValue;
                decimal jumlah;
                if (!decimal.TryParse(txtJumlah.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out jumlah))
                {
                    MessageBox.Show("Jumlah pembayaran tidak valid. Masukkan angka yang valid (gunakan titik '.' sebagai pemisah desimal).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string metodePembayaran = cmbMetodePembayaran.SelectedItem.ToString();
                string status = cmbStatus.SelectedItem.ToString();
                DateTime tanggalPembayaran = dtpTanggalPembayaran.Value;

                // REVISI: Validasi Tanggal Pembayaran (tidak boleh di masa lalu)
                if (tanggalPembayaran.Date < DateTime.Today.Date)
                {
                    MessageBox.Show("Tanggal pembayaran tidak boleh di masa lalu. Silakan pilih tanggal hari ini atau setelahnya.", "Kesalahan Input Tanggal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Konfirmasi Update Data ---
                DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin memperbarui data pembayaran ini?", "Konfirmasi Pembaruan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                {
                    return; // Batalkan operasi jika pengguna memilih "Tidak"
                }

                // --- Proses Update Data dengan Transaksi ---
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlTransaction transaction = null; // Deklarasikan transaksi
                    try
                    {
                        con.Open();
                        transaction = con.BeginTransaction(); // Mulai transaksi

                        // Gunakan Stored Procedure 'sp_UpdatePembayaran' untuk memperbarui data
                        using (SqlCommand cmd = new SqlCommand("sp_UpdatePembayaran", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_pembayaran", idPembayaranToUpdate); // ID pembayaran yang akan diupdate
                            cmd.Parameters.AddWithValue("@id_peserta", idPeserta);
                            cmd.Parameters.AddWithValue("@jumlah", jumlah);
                            cmd.Parameters.AddWithValue("@metode_pembayaran", metodePembayaran);
                            cmd.Parameters.AddWithValue("@status", status);
                            cmd.Parameters.AddWithValue("@tanggal_pembayaran", tanggalPembayaran);

                            // ExecuteNonQuery cocok untuk UPDATE karena tidak mengembalikan data, hanya jumlah baris terpengaruh
                            int rowsAffected = cmd.ExecuteNonQuery();

                            transaction.Commit(); // Komit jika sukses
                            MessageBox.Show("Data pembayaran berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm(); // Bersihkan formulir
                            LoadData(); // Muat ulang data ke DataGridView
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback(); // Rollback pada error SQL
                        }
                        if (sqlEx.Number == 547) // Foreign Key constraint violation (ID Peserta tidak ditemukan)
                        {
                            MessageBox.Show("Gagal memperbarui pembayaran: ID Peserta tidak ditemukan. Pastikan ID Peserta yang Anda pilih valid.", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback(); // Rollback pada error umum
                        }
                        MessageBox.Show($"Terjadi kesalahan tidak terduga saat memperbarui data: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih baris yang ingin diperbarui di tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Klik Sel DataGridView ---
        private void dataGridViewKelolaPembayaran_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Pastikan baris yang diklik valid (bukan header kolom)
            {
                DataGridViewRow row = dataGridViewKelolaPembayaran.Rows[e.RowIndex];

                // Mengatur nilai ComboBox Nama Peserta
                if (row.Cells["id_peserta"].Value != DBNull.Value)
                {
                    int idPesertaFromGrid;
                    if (int.TryParse(row.Cells["id_peserta"].Value.ToString(), out idPesertaFromGrid))
                    {
                        cmbNamaPeserta.SelectedValue = idPesertaFromGrid;
                    }
                    else
                    {
                        cmbNamaPeserta.SelectedIndex = 0; // Pilih placeholder jika gagal parsing
                    }
                }
                else
                {
                    cmbNamaPeserta.SelectedIndex = 0; // Pilih placeholder jika ID Peserta null
                }

                // Mengisi TextBox Jumlah (gunakan null-conditional operator untuk keamanan)
                txtJumlah.Text = row.Cells["jumlah"].Value?.ToString() ?? string.Empty;

                // Mengatur nilai ComboBox Metode Pembayaran
                if (row.Cells["metode_pembayaran"].Value != DBNull.Value)
                {
                    string metode = row.Cells["metode_pembayaran"].Value.ToString();
                    int indexMetode = cmbMetodePembayaran.FindStringExact(metode);
                    if (indexMetode != -1)
                    {
                        cmbMetodePembayaran.SelectedIndex = indexMetode;
                    }
                    else
                    {
                        cmbMetodePembayaran.SelectedIndex = 0; // Pilih placeholder jika tidak ditemukan
                    }
                }
                else
                {
                    cmbMetodePembayaran.SelectedIndex = 0; // Pilih placeholder jika Metode Pembayaran null
                }

                // Mengatur nilai ComboBox Status
                if (row.Cells["status"].Value != DBNull.Value)
                {
                    string status = row.Cells["status"].Value.ToString();
                    int indexStatus = cmbStatus.FindStringExact(status);
                    if (indexStatus != -1)
                    {
                        cmbStatus.SelectedIndex = indexStatus;
                    }
                    else
                    {
                        cmbStatus.SelectedIndex = 0; // Pilih placeholder jika tidak ditemukan
                    }
                }
                else
                {
                    cmbStatus.SelectedIndex = 0; // Pilih placeholder jika Status null
                }

                // Mengatur nilai DateTimePicker
                if (row.Cells["tanggal_pembayaran"].Value != DBNull.Value)
                {
                    DateTime tanggalFromGrid;
                    if (DateTime.TryParse(row.Cells["tanggal_pembayaran"].Value.ToString(), out tanggalFromGrid))
                    {
                        // Tidak perlu memeriksa MinDate di sini karena kita ingin nilai dari grid muncul apa adanya
                        dtpTanggalPembayaran.Value = tanggalFromGrid;
                    }
                    else
                    {
                        dtpTanggalPembayaran.Value = DateTime.Today; // Reset ke tanggal hari ini jika gagal parsing
                    }
                }
                else
                {
                    dtpTanggalPembayaran.Value = DateTime.Today; // Reset ke tanggal hari ini jika Tanggal Pembayaran null
                }
            }
        }

        // --- Metode Umum ---
        // Metode untuk mengosongkan semua input di formulir
        private void ClearForm()
        {
            cmbNamaPeserta.SelectedIndex = 0; // Reset ComboBox ke placeholder
            txtJumlah.Clear();
            cmbMetodePembayaran.SelectedIndex = 0; // Reset ComboBox ke placeholder
            cmbStatus.SelectedIndex = 0; // Reset ComboBox ke placeholder
            dtpTanggalPembayaran.Value = DateTime.Today; // Reset DateTimePicker ke tanggal hari ini
        }

        // Metode untuk memuat data pembayaran dari database ke DataGridView
        private void LoadData()
        {
            try
            {
                // Mulai stopwatch untuk mengukur waktu pemuatan database
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT
                    p.id_pembayaran,
                    p.id_peserta,
                    dp.nama AS nama_peserta, -- Alias kolom nama dari data_peserta
                    p.jumlah,
                    p.metode_pembayaran,
                    p.status,
                    p.tanggal_pembayaran
                FROM
                    pembayaran p
                JOIN
                    data_peserta dp ON p.id_peserta = dp.id_peserta;";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Hentikan stopwatch
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;

                    // Hitung total detik dengan presisi milidetik
                    // Gunakan culture-specific formatting untuk koma sebagai pemisah desimal
                    string elapsedTime = (ts.TotalMilliseconds / 1000.0).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                    // Perbaiki format agar menggunakan koma jika CultureInfo.InvariantCulture menghasilkan titik
                    // Ini akan memastikan selalu menggunakan koma sebagai pemisah desimal
                    elapsedTime = elapsedTime.Replace('.', ',');

                    MessageBox.Show($"Data pembayaran berhasil dimuat dari database dalam waktu {elapsedTime} detik.", "Waktu Pemuatan Data Pembayaran", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dataGridViewKelolaPembayaran.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data pembayaran: " + ex.Message, "Error Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler untuk tombol "Kembali"
        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // Sembunyikan form saat ini
            main mn = new main(); // Buat instance form menu utama Anda (ganti 'main' jika nama form Anda berbeda)
            mn.Show(); // Tampilkan form menu utama
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Event ini mungkin tidak memerlukan implementasi khusus jika tidak ada logika tambahan
            // yang diperlukan saat ComboBox cmbStatus berubah pilihan.
        }
    }
}