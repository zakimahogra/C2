using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq; // Penting untuk .AsEnumerable() dan .Where()
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Caching; // Tambahkan namespace ini untuk MemoryCache
using System.Text.RegularExpressions; // Tambahkan namespace ini untuk Regex

namespace ucppabdd
{
    public partial class KelolaDataPeserta : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";
       

        // Kunci unik untuk item cache data peserta, acara, dan tiket
        private const string PesertaCacheKey = "PesertaDataCache";
        private const string AcaraCacheKey = "AcaraDataCache";
        private const string TiketCacheKey = "TiketDataCache";

        // Menggunakan instance MemoryCache default
        private static readonly MemoryCache _cache = MemoryCache.Default;

        public KelolaDataPeserta()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
            // HAPUS BARIS INI JIKA ADA DARI PENGUJIAN SEBELUMNYA.
            // InvalidatePesertaCache(); // Ini hanya untuk diagnosis sementara

            LoadData(); // Memuat data ke DataGridView
            LoadComboBoxData(); // Memuat data ke ComboBoxes
            dataGridViewKelolaDataPeserta.CellClick += dataGridViewKelolaDataPeserta_CellContentClick;

            // --- PENTING: Attach KeyPress event handler untuk txtNoHp ---
            txtNoHp.KeyPress += TxtNoHp_KeyPress;
            // --- NEW: Attach KeyPress event handler untuk txtNama ---
            txtNama.KeyPress += TxtNama_KeyPress;
        }

        private void KelolaDataPeserta_Load(object sender, EventArgs e)
        {
            // Initial data load sudah ditangani di konstruktor, tidak perlu di sini lagi
        }

        // --- NEW METHOD: Event Handler untuk txtNama.KeyPress ---
        private void TxtNama_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Izinkan hanya huruf (a-z, A-Z), spasi, dan tombol kontrol (seperti Backspace, Delete)
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true; // Abaikan input karakter jika bukan huruf atau spasi
                MessageBox.Show("Nama hanya boleh berisi huruf dan spasi.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtNoHp_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Izinkan hanya angka, tombol kontrol (seperti Backspace, Delete), dan tanda plus (+)
            // Tanda plus (+) sering digunakan di awal nomor telepon internasional
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '+'))
            {
                e.Handled = true; // Abaikan input karakter jika bukan angka, kontrol, atau '+'
                MessageBox.Show("Nomor HP hanya boleh berisi angka dan diawali dengan '+'.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Pastikan '+' hanya bisa diinput di awal dan hanya satu kali
            if (e.KeyChar == '+' && (txtNoHp.Text.Length > 0 || txtNoHp.Text.Contains("+")))
            {
                e.Handled = true;
                MessageBox.Show("Tanda '+' hanya boleh di awal dan hanya satu kali.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

            // --- Validasi Nama hanya huruf dan spasi saat button diklik ---
            if (!Regex.IsMatch(txtNama.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nama hanya boleh berisi huruf dan spasi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Validasi Numerik untuk No. HP (saat button diklik) ---
            // Izinkan angka saja atau angka dengan '+' di awal
            if (!Regex.IsMatch(txtNoHp.Text, @"^\+?[0-9]+$"))
            {
                MessageBox.Show("Nomor HP hanya boleh berisi angka atau diawali dengan '+'.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    // --- PENANGANAN DUPLIKASI DATA & FOREIGN KEY ---
                    if (sqlEx.Number == 50000) // RAISERROR kustom dari Stored Procedure
                    {
                        MessageBox.Show(sqlEx.Message, "Duplikasi Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (sqlEx.Number == 2627) // Pelanggaran UNIQUE CONSTRAINT dari Database
                    {
                        if (sqlEx.Message.Contains("UQ_data_peserta_email"))
                        {
                            MessageBox.Show("Email ini sudah terdaftar sebagai peserta.", "Duplikasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (sqlEx.Message.Contains("UQ_data_peserta_no_hp"))
                        {
                            MessageBox.Show("Nomor HP ini sudah terdaftar sebagai peserta.", "Duplikasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (sqlEx.Number == 547) // Foreign Key constraint violation
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
                            if (sqlEx.Number == 547) // Foreign Key constraint violation
                            {
                                MessageBox.Show("Tidak dapat menghapus peserta ini karena data terkait masih ada di tabel lain (misalnya, Pembayaran).", "Kesalahan Integritas Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // --- Validasi Nama hanya huruf dan spasi saat button diklik ---
                if (!Regex.IsMatch(txtNama.Text, @"^[a-zA-Z\s]+$"))
                {
                    MessageBox.Show("Nama hanya boleh berisi huruf dan spasi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- Validasi Numerik untuk No. HP saat button diklik ---
                // Izinkan angka saja atau angka dengan '+' di awal
                if (!Regex.IsMatch(txtNoHp.Text, @"^\+?[0-9]+$"))
                {
                    MessageBox.Show("Nomor HP hanya boleh berisi angka atau diawali dengan '+'.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        // --- PENANGANAN DUPLIKASI DATA & FOREIGN KEY ---
                        if (sqlEx.Number == 50000) // RAISERROR kustom dari Stored Procedure
                        {
                            MessageBox.Show(sqlEx.Message, "Duplikasi Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (sqlEx.Number == 2627) // Pelanggaran UNIQUE CONSTRAINT dari Database
                        {
                            if (sqlEx.Message.Contains("UQ_data_peserta_email"))
                            {
                                MessageBox.Show("Email ini sudah terdaftar untuk peserta lain.", "Duplikasi Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (sqlEx.Message.Contains("UQ_data_peserta_no_hp"))
                            {
                                MessageBox.Show("Nomor HP ini sudah terdaftar untuk peserta lain.", "Duplikasi Nomor HP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (sqlEx.Number == 547) // Foreign Key constraint violation
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
            bool loadedFromCache = false;

            if (dt == null) // Jika data tidak ditemukan di cache
            {
                // Mulai stopwatch untuk mengukur waktu pemuatan database
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

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

                        // Hentikan stopwatch
                        stopwatch.Stop();
                        TimeSpan ts = stopwatch.Elapsed;

                        // Hitung total detik dengan presisi milidetik
                        // Gunakan culture-specific formatting untuk koma sebagai pemisah desimal
                        string elapsedTime = (ts.TotalMilliseconds / 1000.0).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                        // Perbaiki format agar menggunakan koma jika CultureInfo.InvariantCulture menghasilkan titik
                        // Ini akan memastikan selalu menggunakan koma sebagai pemisah desimal
                        elapsedTime = elapsedTime.Replace('.', ',');

                        MessageBox.Show($"Data peserta berhasil dimuat dari database dalam waktu {elapsedTime} detik.", "Waktu Pemuatan Data", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        // --- NEW: Filter out any unwanted placeholder rows before caching ---
                        // Ini adalah langkah defensif untuk memastikan baris -1 tidak pernah masuk ke cache DataGridView.
                        if (dt.Rows.Count > 0 && dt.AsEnumerable().Any(row => row.Field<int>("id_peserta") == -1))
                        {
                            // Membuat DataTable baru tanpa baris placeholder
                            dt = dt.AsEnumerable()
                                         .Where(row => row.Field<int>("id_peserta") != -1)
                                         .CopyToDataTable();
                        }

                        // Tambahkan data ke cache dengan sliding expiration 5 menit
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };
                        _cache.Set(PesertaCacheKey, dt, policy); // Simpan DataTable yang sudah difilter ke cache
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data peserta: " + ex.Message, "Error Pembebanan Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Keluar jika pembebanan data gagal
                }
            }
            else
            {
                loadedFromCache = true;
                MessageBox.Show("Data peserta berhasil dimuat dari cache.", "Pemuatan Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // --- NEW: Filter lagi jika dt diambil dari cache, untuk berjaga-jaga ---
            // Ini diperlukan jika ada skenario di mana cache diisi sebelum filter ini ada
            // atau jika ada mekanisme lain yang bisa memasukkan baris -1 ke cache.
            if (dt != null && dt.Rows.Count > 0 && dt.AsEnumerable().Any(row => row.Field<int>("id_peserta") == -1))
            {
                dt = dt.AsEnumerable()
                                .Where(row => row.Field<int>("id_peserta") != -1)
                                .CopyToDataTable();
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

        private void txtNama_TextChanged(object sender, EventArgs e)
        {
            // You can add additional real-time validation here if needed,
            // but the KeyPress event handles most of it.
        }
    }
}