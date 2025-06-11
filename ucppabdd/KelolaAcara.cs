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
using System.Runtime.Caching; // Tambahkan namespace ini

namespace ucppabdd
{
    public partial class KelolaAcara : Form
    {
        // Connection string ke database Anda
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_managementt;Integrated Security=True;";

        // Kunci unik untuk item cache data acara
        private const string CacheKey = "AcaraDataCache";

        // Menggunakan instance MemoryCache default
        private static readonly MemoryCache _cache = MemoryCache.Default;

        public KelolaAcara()
        {
            InitializeComponent();

            // Mengatur rentang tanggal yang diizinkan untuk DatePicker
            dtpTanggal.MinDate = new DateTime(2025, 1, 1);
            dtpTanggal.MaxDate = new DateTime(2025, 12, 31);

            // Memuat data saat form pertama kali dibuka
            LoadData();

            // Menghubungkan event CellClick ke handler untuk mengisi form
            dataGridViewKelolaAcara.CellClick += dataGridViewKelolaAcara_CellContentClick;
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            // Validasi input: pastikan semua kolom terisi
            if (string.IsNullOrWhiteSpace(txtNamaAcara.Text) ||
                string.IsNullOrWhiteSpace(txtLokasi.Text) ||
                string.IsNullOrWhiteSpace(txtDeskripsi.Text))
            {
                MessageBox.Show("Semua kolom wajib diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Batalkan proses tambah data
            }

            // Konfirmasi dari pengguna sebelum menambah data
            DialogResult result = MessageBox.Show("Apakah Anda ingin menambahkan data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlTransaction transaction = null; // Deklarasi objek transaksi

                    try
                    {
                        conn.Open(); // Buka koneksi database
                        transaction = conn.BeginTransaction(); // Mulai transaksi

                        // Menggunakan Stored Procedure untuk menambah data acara
                        SqlCommand cmd = new SqlCommand("sp_TambahAcara", conn, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Menambahkan parameter ke command
                        cmd.Parameters.AddWithValue("@nama_acara", txtNamaAcara.Text);
                        cmd.Parameters.AddWithValue("@tanggal", dtpTanggal.Value.Date);
                        cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                        cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                        cmd.ExecuteNonQuery(); // Eksekusi perintah

                        transaction.Commit(); // Komit transaksi jika berhasil

                        MessageBox.Show("Data berhasil ditambahkan melalui stored procedure.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm(); // Bersihkan form input
                        InvalidateCache(); // Invalidate cache agar data terbaru dimuat
                        LoadData(); // Muat ulang data ke DataGridView
                    }
                    catch (SqlException ex)
                    {
                        // Rollback transaksi jika terjadi kesalahan SQL
                        if (transaction != null)
                            transaction.Rollback();
                        MessageBox.Show("Gagal menambahkan data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaksi jika terjadi kesalahan umum
                        if (transaction != null)
                            transaction.Rollback();
                        MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            // Pastikan ada baris yang dipilih di DataGridView
            if (dataGridViewKelolaAcara.CurrentRow != null)
            {
                // Validasi jika sel 'id_acara' memiliki nilai
                if (dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value == null)
                {
                    MessageBox.Show("ID acara tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int id;
                // Coba konversi nilai sel ke integer
                if (!int.TryParse(dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value.ToString(), out id))
                {
                    MessageBox.Show("Gagal mendapatkan ID acara. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Konfirmasi pengguna sebelum menghapus
                var confirm = MessageBox.Show("Yakin ingin menghapus data acara ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open(); // Buka koneksi
                        SqlTransaction transaction = null; // Deklarasi objek transaksi

                        try
                        {
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            // Menggunakan Stored Procedure untuk menghapus data acara
                            using (SqlCommand cmd = new SqlCommand("sp_DeleteAcara", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_acara", id);

                                int rowsAffected = cmd.ExecuteNonQuery(); // Eksekusi perintah

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Komit transaksi jika berhasil
                                    MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm(); // Bersihkan form
                                    InvalidateCache(); // Invalidate cache
                                    LoadData(); // Muat ulang data
                                }
                                else
                                {
                                    transaction.Rollback(); // Batalkan transaksi
                                    MessageBox.Show("Data tidak ditemukan atau gagal dihapus. Perubahan dibatalkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            // Tangani kesalahan spesifik SQL
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            // Tangani kesalahan umum lainnya
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show($"Terjadi kesalahan tidak terduga: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Pastikan ada baris yang dipilih di DataGridView
            if (dataGridViewKelolaAcara.CurrentRow != null)
            {
                // Validasi Input Form
                if (string.IsNullOrWhiteSpace(txtNamaAcara.Text) ||
                    string.IsNullOrWhiteSpace(txtLokasi.Text) ||
                    string.IsNullOrWhiteSpace(txtDeskripsi.Text))
                {
                    MessageBox.Show("Mohon lengkapi semua data acara (Nama Acara, Lokasi, Deskripsi).", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ambil ID Acara dari DataGridView
                int id;
                if (dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value == null ||
                    !int.TryParse(dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value.ToString(), out id))
                {
                    MessageBox.Show("Gagal mendapatkan ID acara dari baris yang dipilih. Pastikan ID valid.", "Kesalahan ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validasi dan Parsing Tanggal
                DateTime tanggalAcara;
                if (!DateTime.TryParse(dtpTanggal.Text, out tanggalAcara))
                {
                    MessageBox.Show("Format tanggal tidak valid. Harap masukkan tanggal yang benar (contoh: DD/MM/YYYY).", "Kesalahan Format Tanggal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Konfirmasi Pembaruan
                var konfirmasi = MessageBox.Show("Yakin ingin memperbarui data acara ini?", "Konfirmasi Pembaruan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (konfirmasi == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null; // Deklarasi objek transaksi

                        try
                        {
                            con.Open(); // Buka koneksi database
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            // Menggunakan Stored Procedure untuk memperbarui data acara
                            using (SqlCommand cmd = new SqlCommand("sp_UpdateAcara", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Tambahkan parameter ke stored procedure
                                cmd.Parameters.AddWithValue("@id_acara", id);
                                cmd.Parameters.AddWithValue("@nama_acara", txtNamaAcara.Text);
                                cmd.Parameters.AddWithValue("@tanggal", tanggalAcara);
                                cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                                cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                                int rowsAffected = cmd.ExecuteNonQuery(); // Eksekusi perintah

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Komit transaksi jika berhasil
                                    MessageBox.Show("Data acara berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm(); // Bersihkan formulir
                                    InvalidateCache(); // Invalidate cache
                                    LoadData(); // Muat ulang data ke DataGridView
                                }
                                else
                                {
                                    transaction.Rollback(); // Batalkan transaksi
                                    MessageBox.Show("Data acara gagal diperbarui. ID acara mungkin tidak ditemukan atau tidak ada perubahan yang dibuat.", "Gagal Pembaruan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            // Tangani kesalahan khusus SQL Server
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            // Tangani kesalahan umum lainnya
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show($"Terjadi kesalahan tidak terduga saat memperbarui data: {ex.Message}", "Kesalahan Umum", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                // Pesan jika tidak ada baris yang dipilih di DataGridView
                MessageBox.Show("Silakan pilih baris acara yang ingin diperbarui di tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridViewKelolaAcara_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaAcara.Rows[e.RowIndex];

                // Mengisi TextBoxes dan DatePicker dengan data dari baris yang dipilih
                // Menggunakan null-conditional operator (?.) dan null-coalescing operator (??) untuk menghindari NullReferenceException
                txtNamaAcara.Text = row.Cells["nama_acara"].Value?.ToString() ?? string.Empty;
                // Pastikan kolom 'tanggal' ada dan berisi data tanggal yang valid
                if (row.Cells["tanggal"].Value != DBNull.Value && row.Cells["tanggal"].Value != null)
                {
                    dtpTanggal.Value = Convert.ToDateTime(row.Cells["tanggal"].Value);
                }
                txtLokasi.Text = row.Cells["lokasi"].Value?.ToString() ?? string.Empty;
                txtDeskripsi.Text = row.Cells["deskripsi"].Value?.ToString() ?? string.Empty;
            }
        }

        // Metode untuk membersihkan input form
        private void ClearForm()
        {
            txtNamaAcara.Clear();
            dtpTanggal.Value = DateTime.Today; // Mengatur tanggal ke hari ini
            txtLokasi.Clear();
            txtDeskripsi.Clear();
        }

        // Metode untuk memuat data ke DataGridView, dengan optimasi cache
        private void LoadData()
        {
            // Coba ambil data dari cache terlebih dahulu
            DataTable dt = _cache.Get(CacheKey) as DataTable;

            if (dt == null) // Jika data tidak ditemukan di cache
            {
                // Muat data dari database
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "SELECT id_acara, nama_acara, tanggal, lokasi, deskripsi FROM acara"; // Pilih kolom yang dibutuhkan
                        SqlDataAdapter da = new SqlDataAdapter(query, con);
                        dt = new DataTable();
                        da.Fill(dt);

                        // Tambahkan data ke cache dengan sliding expiration 5 menit
                        // Data akan kedaluwarsa 5 menit setelah akses terakhir
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _cache.Set(CacheKey, dt, policy); // Simpan DataTable ke cache
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data: " + ex.Message, "Error Pembebanan Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Keluar jika pembebanan data gagal
                }
            }
            // Ikatan data ke DataGridView, baik dari cache maupun dari database
            dataGridViewKelolaAcara.DataSource = dt;
        }

        // Metode untuk menghapus data acara dari cache
        private void InvalidateCache()
        {
            // Hapus item cache agar LoadData berikutnya akan memuat data segar dari database
            _cache.Remove(CacheKey);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // Sembunyikan form ini
            main mn = new main(); // Buat instance form menu utama Anda
            mn.Show(); // Tampilkan form menu utama
        }

        private void KelolaAcara_Load(object sender, EventArgs e)
        {
            // Initial data load sudah ditangani di konstruktor, tidak perlu di sini lagi
        }
    }
}