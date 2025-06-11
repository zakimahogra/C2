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
    public partial class KelolaTiket : Form
    {
        // String koneksi ke database SQL Server Anda.
        // Dalam aplikasi sungguhan, disarankan untuk menyimpan ini di App.config.
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_managementt;Integrated Security=True;";

        // Properti untuk menerima ID acara dari form sebelumnya
        public int SelectedAcaraId { get; set; } = -1; // Default -1 menunjukkan tidak ada ID yang dipilih

        // Konstruktor default
        public KelolaTiket()
        {
            InitializeComponent();
            InitializeCustomComponents(); // Panggil metode inisialisasi kustom
        }

        // Overload constructor untuk menerima ID acara dari form sebelumnya
        public KelolaTiket(int idAcara) : this() // Panggil constructor default terlebih dahulu
        {
            this.SelectedAcaraId = idAcara;
            // Catatan: Pemilihan ComboBox berdasarkan SelectedAcaraId akan dilakukan di FillIdAcaraComboBox
            // setelah data ComboBox dimuat.
        }

        // Metode untuk inisialisasi komponen kustom, termasuk memuat data dan mengatur event
        private void InitializeCustomComponents()
        {
            LoadData(); // Muat data tiket ke DataGridView
            FillIdAcaraComboBox(); // Isi ComboBox ID Acara
            // Menghubungkan event CellClick pada DataGridView ke fungsi
            // Ketika sel di DataGridView diklik, data akan mengisi kolom input.
            dataGridViewKelolaTiket.CellClick += dataGridViewKelolaTiket_CellContentClick;
        }

        // Metode untuk mengisi ComboBox ID Acara dengan data dari database (dengan MemoryCache)
        private void FillIdAcaraComboBox()
        {
            cmbIdAcara.DataSource = null; // Hapus sumber data lama
            cmbIdAcara.Items.Clear(); // Bersihkan item lama

            DataTable dtAcara = null;
            // Dapatkan instance cache default
            MemoryCache cache = MemoryCache.Default;
            string cacheKey = "AcaraDataCache"; // Kunci unik untuk data acara di cache

            // Coba ambil data dari cache
            dtAcara = cache.Get(cacheKey) as DataTable;

            if (dtAcara == null) // Jika data tidak ada di cache, muat dari database
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        // Query untuk mengambil id_acara dan nama_acara dari tabel acara
                        string query = "SELECT id_acara, nama_acara FROM acara ORDER BY nama_acara";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        dtAcara = new DataTable();
                        da.Fill(dtAcara);

                        // Tambahkan data ke cache dengan kebijakan kadaluarsa (misal: 5 menit)
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) // Data akan kadaluarsa setelah 5 menit
                        };
                        cache.Set(cacheKey, dtAcara, policy);

                        Console.WriteLine("Data acara dimuat dari database dan disimpan ke cache."); // Untuk debug
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Terjadi kesalahan database saat memuat daftar acara: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Keluar dari fungsi jika ada kesalahan database
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Terjadi kesalahan tidak terduga saat memuat daftar acara: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Keluar dari fungsi jika ada kesalahan umum
                    }
                }
            }
            else
            {
                Console.WriteLine("Data acara dimuat dari cache."); // Untuk debug
            }

            // Pastikan dtAcara tidak null sebelum digunakan
            if (dtAcara != null && dtAcara.Rows.Count > 0)
            {
                // Clone DataTable untuk menambahkan kolom DisplayMember tanpa memengaruhi data di cache asli
                DataTable displayDt = dtAcara.Copy();
                displayDt.Columns.Add("DisplayMember", typeof(string));
                foreach (DataRow row in displayDt.Rows)
                {
                    row["DisplayMember"] = $"{row["id_acara"]} - {row["nama_acara"]}";
                }

                cmbIdAcara.DisplayMember = "DisplayMember"; // Kolom yang akan ditampilkan
                cmbIdAcara.ValueMember = "id_acara";        // Kolom yang akan menjadi nilai sebenarnya
                cmbIdAcara.DataSource = displayDt; // Set DataTable sebagai sumber data

                // Coba pilih ID acara jika ada yang dikirim dari form sebelumnya atau dari dataGridView click
                if (SelectedAcaraId != -1)
                {
                    // Pastikan SelectedValue ada di DataSource
                    DataRow[] foundRows = displayDt.Select($"id_acara = {SelectedAcaraId}");
                    if (foundRows.Length > 0)
                    {
                        cmbIdAcara.SelectedValue = SelectedAcaraId;
                    }
                    else
                    {
                        cmbIdAcara.SelectedIndex = -1; // Jika tidak ditemukan, tidak ada yang terpilih
                    }
                }
                else
                {
                    cmbIdAcara.SelectedIndex = -1; // Tidak ada yang terpilih secara default
                }
            }
            else
            {
                // Jika tidak ada data acara, kosongkan ComboBox dan beri tahu pengguna
                cmbIdAcara.Items.Clear();
                MessageBox.Show("Tidak ada data acara yang tersedia untuk dipilih.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Event handler untuk label (kosong, bisa ditambahkan logika jika diperlukan)
        private void label1_Click(object sender, EventArgs e)
        {
            // Tidak ada implementasi khusus untuk saat ini
        }

        // Event handler untuk tombol "Tambah"
        private void btnTambah_Click(object sender, EventArgs e)
        {
            // --- Validasi Input Kosong ---
            if (cmbIdAcara.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtKategori.Text) ||
                string.IsNullOrWhiteSpace(txtHarga.Text) ||
                string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("Semua kolom (ID Acara, Kategori, Harga, Jumlah) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Validasi dan Parsing Input Numerik ---
            int idAcara;
            if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
            {
                MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal harga;
            // Menggunakan CultureInfo.InvariantCulture untuk parsing yang konsisten (mengabaikan pengaturan regional)
            // Atau Anda bisa menggunakan CultureInfo.CurrentCulture jika ingin sesuai regional PC
            if (!decimal.TryParse(txtHarga.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out harga))
            {
                MessageBox.Show("Harga harus berupa angka desimal yang valid (gunakan titik '.' sebagai pemisah desimal).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int jumlah;
            if (!int.TryParse(txtJumlah.Text, out jumlah))
            {
                MessageBox.Show("Jumlah harus berupa angka bulat yang valid.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- Konfirmasi Penambahan Data ---
            DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin menambahkan data tiket ini?", "Konfirmasi Penambahan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

                    using (SqlCommand cmd = new SqlCommand("sp_TambahTiket", con, transaction)) // Kaitkan command dengan transaksi
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Tambah parameter ke stored procedure
                        cmd.Parameters.AddWithValue("@id_acara", idAcara);
                        cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                        cmd.Parameters.AddWithValue("@harga", harga);
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);

                        cmd.ExecuteNonQuery(); // Eksekusi stored procedure

                        transaction.Commit(); // Komit transaksi jika semua berhasil
                        MessageBox.Show("Data tiket berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                    if (sqlEx.Number == 547) // Foreign Key constraint violation (ID Acara tidak ditemukan)
                    {
                        MessageBox.Show("Gagal menambahkan tiket: ID Acara tidak ditemukan. Pastikan ID Acara yang Anda pilih valid dan ada di database.", "Kesalahan Database: ID Acara Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Event handler untuk tombol "Hapus"
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
                // --- Validasi ID Tiket untuk Hapus ---
                if (dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value == null)
                {
                    MessageBox.Show("ID Tiket tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idTiket;
                if (!int.TryParse(dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value.ToString(), out idTiket))
                {
                    MessageBox.Show("Gagal mendapatkan ID Tiket. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Konfirmasi Penghapusan ---
                var confirm = MessageBox.Show("Yakin ingin menghapus data tiket ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null; // Deklarasikan transaksi
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            using (SqlCommand cmd = new SqlCommand("sp_DeleteTiket", con, transaction)) // Kaitkan dengan transaksi
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_tiket", idTiket);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Komit jika sukses
                                    MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm(); // Bersihkan formulir
                                    LoadData(); // Muat ulang data ke DataGridView
                                }
                                else
                                {
                                    transaction.Rollback(); // Rollback jika tidak ada baris terpengaruh
                                    MessageBox.Show("Data tidak ditemukan atau gagal dihapus. Perubahan dibatalkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
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

        // Event handler untuk tombol "Update"
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
                // --- Validasi ID Tiket untuk Update ---
                if (dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value == null)
                {
                    MessageBox.Show("ID Tiket tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int idTiket;
                if (!int.TryParse(dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value.ToString(), out idTiket))
                {
                    MessageBox.Show("Gagal mendapatkan ID Tiket. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Validasi Input Kosong (termasuk ComboBox) ---
                if (cmbIdAcara.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(txtKategori.Text) ||
                    string.IsNullOrWhiteSpace(txtHarga.Text) ||
                    string.IsNullOrWhiteSpace(txtJumlah.Text))
                {
                    MessageBox.Show("Semua kolom (ID Acara, Kategori, Harga, Jumlah) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- Validasi dan Parsing Input Numerik ---
                int idAcara;
                if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
                {
                    MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal harga;
                if (!decimal.TryParse(txtHarga.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out harga))
                {
                    MessageBox.Show("Harga harus berupa angka desimal yang valid (gunakan titik '.' sebagai pemisah desimal).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int jumlah;
                if (!int.TryParse(txtJumlah.Text, out jumlah))
                {
                    MessageBox.Show("Jumlah harus berupa angka bulat yang valid.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Konfirmasi Pembaruan ---
                var konfirmasi = MessageBox.Show("Yakin ingin memperbarui data tiket ini?", "Konfirmasi Pembaruan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (konfirmasi == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null; // Deklarasikan transaksi
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            using (SqlCommand cmd = new SqlCommand("sp_UpdateTiket", con, transaction)) // Kaitkan dengan transaksi
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Tambah parameter ke command
                                cmd.Parameters.AddWithValue("@id_tiket", idTiket); // ID tiket yang akan diupdate
                                cmd.Parameters.AddWithValue("@id_acara", idAcara); // ID acara dari ComboBox
                                cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                                cmd.Parameters.AddWithValue("@harga", harga);
                                cmd.Parameters.AddWithValue("@jumlah", jumlah);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Komit jika sukses
                                    MessageBox.Show("Data berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm(); // Bersihkan formulir
                                    LoadData(); // Muat ulang data ke DataGridView
                                }
                                else
                                {
                                    transaction.Rollback(); // Rollback jika tidak ada baris terpengaruh
                                    MessageBox.Show("Data tidak ditemukan atau gagal diperbarui. Perubahan dibatalkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback(); // Rollback pada error SQL
                            }
                            if (sqlEx.Number == 547) // Foreign Key constraint violation (ID Acara tidak ditemukan)
                            {
                                MessageBox.Show("Gagal memperbarui tiket: ID Acara tidak ditemukan. Pastikan ID Acara yang Anda pilih valid.", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih baris yang ingin diperbarui di tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Event handler saat mengklik sel di DataGridView
        private void dataGridViewKelolaTiket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaTiket.Rows[e.RowIndex];

                // Pilih item di ComboBox berdasarkan id_acara dari DataGridView
                if (row.Cells["id_acara"].Value != DBNull.Value)
                {
                    int idAcaraFromGrid;
                    if (int.TryParse(row.Cells["id_acara"].Value.ToString(), out idAcaraFromGrid))
                    {
                        // Set SelectedAcaraId agar ComboBox terpilih
                        SelectedAcaraId = idAcaraFromGrid;
                        // Panggil lagi FillIdAcaraComboBox untuk memilih nilai yang sesuai
                        FillIdAcaraComboBox();
                    }
                }
                else
                {
                    cmbIdAcara.SelectedIndex = -1; // Jika ID Acara kosong di grid, reset ComboBox
                }

                // Isi TextBox dengan data dari baris yang diklik
                txtKategori.Text = row.Cells["kategori"].Value?.ToString() ?? string.Empty; // Gunakan null-conditional operator untuk menghindari NullReferenceException
                txtHarga.Text = row.Cells["harga"].Value?.ToString() ?? string.Empty;
                txtJumlah.Text = row.Cells["jumlah"].Value?.ToString() ?? string.Empty;
            }
        }

        // Metode untuk mengosongkan semua input di formulir
        private void ClearForm()
        {
            cmbIdAcara.SelectedIndex = -1; // Reset ComboBox
            txtKategori.Clear();
            txtHarga.Clear();
            txtJumlah.Clear();
            // Reset SelectedAcaraId agar tidak memengaruhi pemilihan ComboBox selanjutnya
            SelectedAcaraId = -1;
        }

        // Metode untuk memuat data tiket dari database ke DataGridView
        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Query untuk mengambil semua kolom dari tabel tiket
                    // Anda bisa JOIN dengan tabel 'acara' di sini jika ingin menampilkan nama_acara
                    string query = "SELECT t.id_tiket, t.id_acara, a.nama_acara, t.kategori, t.harga, t.jumlah " +
                                   "FROM tiket t JOIN acara a ON t.id_acara = a.id_acara";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKelolaTiket.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data tiket: " + ex.Message, "Error Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler untuk event Load formulir (dipanggil saat formulir pertama kali dimuat)
        private void KelolaTiket_Load(object sender, EventArgs e)
        {
            // Karena InitializeCustomComponents() sudah dipanggil di konstruktor,
            // Anda tidak perlu memanggil LoadData() atau FillIdAcaraComboBox() di sini lagi.
            // Namun, jika Anda ingin logika yang dieksekusi hanya saat form benar-benar selesai dimuat
            // (misalnya setelah semua kontrol di-render), Anda bisa tambahkan di sini.
        }

        // Event handler untuk tombol "Kembali"
        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // Sembunyikan form saat ini
            main mn = new main(); // Buat instance form menu utama Anda (ganti 'main' jika nama form Anda berbeda)
            mn.Show(); // Tampilkan form menu utama
        }
    }
}