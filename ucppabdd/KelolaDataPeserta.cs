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
    public partial class KelolaDataPeserta : Form
    {
        // Connection string ke database Anda
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_managementt;Integrated Security=True;";

        // Kunci unik untuk item cache data peserta, acara, dan tiket
        private const string PesertaCacheKey = "PesertaDataCache";
        private const string AcaraCacheKey = "AcaraDataCache";
        private const string TiketCacheKey = "TiketDataCache";

        // Menggunakan instance MemoryCache default
        private static readonly MemoryCache _cache = MemoryCache.Default;

        public KelolaDataPeserta()
        {
            InitializeComponent();
            LoadData(); // Memuat data ke DataGridView
            LoadComboBoxData(); // Memuat data ke ComboBoxes
            dataGridViewKelolaDataPeserta.CellClick += dataGridViewKelolaDataPeserta_CellContentClick;
        }

        private void KelolaDataPeserta_Load(object sender, EventArgs e)
        {
            // Initial data load sudah ditangani di konstruktor, tidak perlu di sini lagi
        }

        // --- Metode untuk memuat data ke ComboBoxes dengan Cache ---
        private void LoadComboBoxData()
        {
            // Memuat data ID Acara ke cmbIdAcara
            try
            {
                DataTable dtAcara = _cache.Get(AcaraCacheKey) as DataTable;
                if (dtAcara == null)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string queryAcara = "SELECT id_acara, nama_acara FROM Acara ORDER BY nama_acara";
                        SqlDataAdapter daAcara = new SqlDataAdapter(queryAcara, con);
                        dtAcara = new DataTable();
                        daAcara.Fill(dtAcara);

                        // Tambahkan placeholder
                        DataRow placeholderAcara = dtAcara.NewRow();
                        placeholderAcara["id_acara"] = -1;
                        placeholderAcara["nama_acara"] = "-- Pilih Acara --";
                        dtAcara.Rows.InsertAt(placeholderAcara, 0);

                        // Tambahkan ke cache dengan sliding expiration 5 menit
                        CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };
                        _cache.Set(AcaraCacheKey, dtAcara, policy);
                    }
                }

                cmbIdAcara.DataSource = dtAcara;
                cmbIdAcara.DisplayMember = "nama_acara";
                cmbIdAcara.ValueMember = "id_acara";
                cmbIdAcara.SelectedIndex = 0; // Pilih item placeholder secara default
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data Acara: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Memuat data ID Tiket ke cmbIdTiket
            try
            {
                DataTable dtTiket = _cache.Get(TiketCacheKey) as DataTable;
                if (dtTiket == null)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string queryTiket = "SELECT id_tiket, kategori FROM Tiket ORDER BY kategori";
                        SqlDataAdapter daTiket = new SqlDataAdapter(queryTiket, con);
                        dtTiket = new DataTable();
                        daTiket.Fill(dtTiket);

                        // Tambahkan placeholder
                        DataRow placeholderTiket = dtTiket.NewRow();
                        placeholderTiket["id_tiket"] = -1;
                        placeholderTiket["kategori"] = "-- Pilih Kategori --";
                        dtTiket.Rows.InsertAt(placeholderTiket, 0);

                        // Tambahkan ke cache dengan sliding expiration 5 menit
                        CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };
                        _cache.Set(TiketCacheKey, dtTiket, policy);
                    }
                }

                cmbIdTiket.DataSource = dtTiket;
                cmbIdTiket.DisplayMember = "kategori";
                cmbIdTiket.ValueMember = "id_tiket";
                cmbIdTiket.SelectedIndex = 0; // Pilih item placeholder secara default
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data Tiket: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            // --- Validasi Input Kosong (termasuk ComboBox) ---
            if (cmbIdAcara.SelectedValue == null || (int)cmbIdAcara.SelectedValue == -1 ||
                cmbIdTiket.SelectedValue == null || (int)cmbIdTiket.SelectedValue == -1 ||
                string.IsNullOrWhiteSpace(txtNama.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtNoHp.Text))
            {
                MessageBox.Show("Semua kolom (Nama, Email, No. HP, ID Acara, ID Tiket) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Validasi Format Email ---
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.EndsWith(".com"))
            {
                MessageBox.Show("Format email tidak valid. Email harus mengandung '@' dan diakhiri dengan '.com'.", "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Parsing Input Numerik dari ComboBox ---
            int idAcara;
            if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
            {
                MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idTiket;
            if (!int.TryParse(cmbIdTiket.SelectedValue.ToString(), out idTiket))
            {
                MessageBox.Show("ID Tiket yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- Konfirmasi Penambahan Data ---
            DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin menambahkan data ini?", "Konfirmasi Penambahan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

                    using (SqlCommand cmd = new SqlCommand("sp_TambahPeserta", con, transaction)) // Kaitkan dengan transaksi
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Isi parameter ke stored procedure
                        cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@no_hp", txtNoHp.Text);
                        cmd.Parameters.AddWithValue("@id_acara", idAcara);
                        cmd.Parameters.AddWithValue("@id_tiket", idTiket);

                        cmd.ExecuteNonQuery();

                        transaction.Commit(); // Komit jika sukses
                        MessageBox.Show("Data peserta berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        InvalidatePesertaCache(); // Invalidate cache peserta
                        LoadData(); // Muat ulang data peserta
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback(); // Rollback pada error SQL
                    }
                    if (sqlEx.Number == 547) // Foreign Key constraint violation
                    {
                        MessageBox.Show("Gagal menambahkan peserta: ID Acara atau ID Tiket tidak ditemukan. Pastikan ID Acara dan ID Tiket yang Anda pilih valid dan ada di database.", "Kesalahan Database: ID Acara/Tiket Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show($"Terjadi kesalahan tidak terduga saat menambahkan data: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaDataPeserta.CurrentRow != null)
            {
                // --- Validasi ID Peserta untuk Hapus ---
                if (dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value == null)
                {
                    MessageBox.Show("ID Peserta tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idPeserta;
                if (!int.TryParse(dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value.ToString(), out idPeserta))
                {
                    MessageBox.Show("Gagal mendapatkan ID Peserta. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var confirm = MessageBox.Show("Yakin ingin menghapus data peserta ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null; // Deklarasikan transaksi
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction(); // Mulai transaksi

                            using (SqlCommand cmd = new SqlCommand("sp_DeletePeserta", con, transaction)) // Kaitkan dengan transaksi
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_peserta", idPeserta); // Gunakan idPeserta yang sudah divalidasi

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit(); // Komit jika sukses
                                    MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm();
                                    InvalidatePesertaCache(); // Invalidate cache peserta
                                    LoadData(); // Muat ulang data peserta
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaDataPeserta.CurrentRow != null)
            {
                // --- Validasi ID Peserta untuk Update ---
                if (dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value == null)
                {
                    MessageBox.Show("ID Peserta tidak ditemukan pada baris yang dipilih. Silakan pilih baris yang valid untuk diupdate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idPesertaToUpdate;
                if (!int.TryParse(dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value.ToString(), out idPesertaToUpdate))
                {
                    MessageBox.Show("Gagal mendapatkan ID Peserta untuk update. Format ID tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Validasi Input Kosong (termasuk ComboBox) ---
                if (cmbIdAcara.SelectedValue == null || (int)cmbIdAcara.SelectedValue == -1 ||
                    cmbIdTiket.SelectedValue == null || (int)cmbIdTiket.SelectedValue == -1 ||
                    string.IsNullOrWhiteSpace(txtNama.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtNoHp.Text))
                {
                    MessageBox.Show("Semua kolom (Nama, Email, No. HP, ID Acara, ID Tiket) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- Validasi Format Email ---
                if (!txtEmail.Text.Contains("@") || !txtEmail.Text.EndsWith(".com"))
                {
                    MessageBox.Show("Format email tidak valid. Email harus mengandung '@' dan diakhiri dengan '.com'.", "Validasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validasi dan konversi input dari ComboBoxes
                int idAcara;
                if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
                {
                    MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idTiket;
                if (!int.TryParse(cmbIdTiket.SelectedValue.ToString(), out idTiket))
                {
                    MessageBox.Show("ID Tiket yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- Konfirmasi Update Data ---
                DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin memperbarui data ini?", "Konfirmasi Pembaruan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

                        using (SqlCommand cmd = new SqlCommand("sp_UpdatePeserta", con, transaction)) // Kaitkan dengan transaksi
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Tambah parameter ke stored procedure
                            cmd.Parameters.AddWithValue("@id_peserta", idPesertaToUpdate);
                            cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@no_hp", txtNoHp.Text);
                            cmd.Parameters.AddWithValue("@id_acara", idAcara);
                            cmd.Parameters.AddWithValue("@id_tiket", idTiket);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                transaction.Commit(); // Komit jika sukses
                                MessageBox.Show("Data peserta berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearForm();
                                InvalidatePesertaCache(); // Invalidate cache peserta
                                LoadData(); // Muat ulang data peserta
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
                        if (sqlEx.Number == 547) // Foreign Key constraint violation
                        {
                            MessageBox.Show("Gagal memperbarui peserta: ID Acara atau ID Tiket tidak ditemukan. Pastikan ID Acara dan ID Tiket yang Anda pilih valid dan ada di database.", "Kesalahan Database: ID Acara/Tiket Tidak Ditemukan", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dataGridViewKelolaDataPeserta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaDataPeserta.Rows[e.RowIndex];

                txtNama.Text = row.Cells["nama"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["email"].Value?.ToString() ?? string.Empty;
                txtNoHp.Text = row.Cells["no_hp"].Value?.ToString() ?? string.Empty;

                // Set nilai ComboBox berdasarkan data dari DataGridView
                // Pastikan nilai di kolom DataGridView sesuai dengan ValueMember di ComboBox
                if (row.Cells["id_acara"].Value != DBNull.Value && row.Cells["id_acara"].Value != null)
                {
                    // Coba temukan item dengan ValueMember yang sesuai
                    int idAcara = Convert.ToInt32(row.Cells["id_acara"].Value);
                    cmbIdAcara.SelectedValue = idAcara;
                }
                else
                {
                    cmbIdAcara.SelectedValue = -1; // Set ke nilai placeholder
                }

                if (row.Cells["id_tiket"].Value != DBNull.Value && row.Cells["id_tiket"].Value != null)
                {
                    // Coba temukan item dengan ValueMember yang sesuai
                    int idTiket = Convert.ToInt32(row.Cells["id_tiket"].Value);
                    cmbIdTiket.SelectedValue = idTiket;
                }
                else
                {
                    cmbIdTiket.SelectedValue = -1; // Set ke nilai placeholder
                }
            }
        }

        // Metode untuk membersihkan input form
        private void ClearForm()
        {
            txtNama.Clear();
            txtEmail.Clear();
            txtNoHp.Clear();
            // Reset ComboBox ke placeholder (index 0 karena kita sisipkan di awal)
            if (cmbIdAcara.Items.Count > 0) cmbIdAcara.SelectedIndex = 0;
            if (cmbIdTiket.Items.Count > 0) cmbIdTiket.SelectedIndex = 0;
        }

        // Metode untuk memuat data ke DataGridView, dengan optimasi cache
        private void LoadData()
        {
            // Coba ambil data dari cache terlebih dahulu
            DataTable dt = _cache.Get(PesertaCacheKey) as DataTable;

            if (dt == null) // Jika data tidak ditemukan di cache
            {
                // Muat data dari database
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        // Query ini akan mengambil semua kolom yang relevan dan JOIN tabel
                        // agar Anda bisa menampilkan nama acara dan kategori tiket di DataGridView
                        string query = @"
                            SELECT
                                dp.id_peserta,
                                dp.nama,
                                dp.email,
                                dp.no_hp,
                                dp.id_acara,
                                a.nama_acara, -- Tambahkan kolom nama_acara dari tabel Acara
                                dp.id_tiket,
                                t.kategori    -- Tambahkan kolom kategori dari tabel Tiket
                            FROM
                                data_peserta dp
                            JOIN
                                Acara a ON dp.id_acara = a.id_acara
                            JOIN
                                Tiket t ON dp.id_tiket = t.id_tiket;";
                        SqlDataAdapter da = new SqlDataAdapter(query, con);
                        dt = new DataTable();
                        da.Fill(dt);

                        // Tambahkan data ke cache dengan sliding expiration 5 menit
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _cache.Set(PesertaCacheKey, dt, policy); // Simpan DataTable ke cache
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data peserta: " + ex.Message, "Error Pembebanan Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Keluar jika pembebanan data gagal
                }
            }
            // Ikatan data ke DataGridView, baik dari cache maupun dari database
            dataGridViewKelolaDataPeserta.DataSource = dt;
        }

        // Metode untuk menghapus data peserta dari cache
        private void InvalidatePesertaCache()
        {
            // Hapus item cache peserta agar LoadData berikutnya akan memuat data segar dari database
            _cache.Remove(PesertaCacheKey);
        }

        // Invalidate cache untuk ComboBoxes juga jika data master berubah
        private void InvalidateAcaraCache()
        {
            _cache.Remove(AcaraCacheKey);
        }

        private void InvalidateTiketCache()
        {
            _cache.Remove(TiketCacheKey);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // Sembunyikan form ini
            main mn = new main(); // Buat instance form menu utama Anda
            mn.Show(); // Tampilkan form menu utama
        }

        private void cmbIdTiket_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Event ini tidak perlu diisi jika hanya untuk memuat data saat form dibuka
        }
    }
}