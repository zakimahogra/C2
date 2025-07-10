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
using System.Runtime.Caching;
using System.Globalization; // Pastikan namespace ini ada!

namespace ucppabdd
{
    public partial class KelolaTiket : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";
        
        public int SelectedAcaraId { get; set; } = -1;

        public KelolaTiket()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
            InitializeCustomComponents();
        }

        public KelolaTiket(int idAcara) : this()
        {
            this.SelectedAcaraId = idAcara;
        }

        private void InitializeCustomComponents()
        {
            LoadData();
            FillIdAcaraComboBox();
            dataGridViewKelolaTiket.CellClick += dataGridViewKelolaTiket_CellContentClick;
        }

        private void FillIdAcaraComboBox()
        {
            cmbIdAcara.DataSource = null;
            cmbIdAcara.Items.Clear();

            DataTable dtAcara = null;
            MemoryCache cache = MemoryCache.Default;
            string cacheKey = "AcaraDataCache";

            dtAcara = cache.Get(cacheKey) as DataTable;

            if (dtAcara == null)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT id_acara, nama_acara FROM acara ORDER BY nama_acara";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        dtAcara = new DataTable();
                        da.Fill(dtAcara);

                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                        };
                        cache.Set(cacheKey, dtAcara, policy);

                        Console.WriteLine("Data acara dimuat dari database dan disimpan ke cache.");
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Terjadi kesalahan database saat memuat daftar acara: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Terjadi kesalahan tidak terduga saat memuat daftar acara: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Data acara dimuat dari cache.");
            }

            if (dtAcara != null && dtAcara.Rows.Count > 0)
            {
                DataTable displayDt = dtAcara.Copy();
                displayDt.Columns.Add("DisplayMember", typeof(string));
                foreach (DataRow row in displayDt.Rows)
                {
                    row["DisplayMember"] = $"{row["id_acara"]} - {row["nama_acara"]}";
                }

                cmbIdAcara.DisplayMember = "DisplayMember";
                cmbIdAcara.ValueMember = "id_acara";
                cmbIdAcara.DataSource = displayDt;

                if (SelectedAcaraId != -1)
                {
                    DataRow[] foundRows = displayDt.Select($"id_acara = {SelectedAcaraId}");
                    if (foundRows.Length > 0)
                    {
                        cmbIdAcara.SelectedValue = SelectedAcaraId;
                    }
                    else
                    {
                        cmbIdAcara.SelectedIndex = -1;
                    }
                }
                else
                {
                    cmbIdAcara.SelectedIndex = -1;
                }
            }
            else
            {
                cmbIdAcara.Items.Clear();
                MessageBox.Show("Tidak ada data acara yang tersedia untuk dipilih.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Tidak ada implementasi khusus untuk saat ini
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (cmbIdAcara.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtKategori.Text) ||
                string.IsNullOrWhiteSpace(txtHarga.Text) ||
                string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("Semua kolom (ID Acara, Kategori, Harga, Jumlah) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idAcara;
            if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
            {
                MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal harga;
            if (!decimal.TryParse(txtHarga.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out harga))
            {
                MessageBox.Show("Harga harus berupa angka desimal yang valid sesuai pengaturan regional Anda (gunakan koma ',' atau titik '.' sebagai pemisah desimal yang benar).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int jumlah;
            if (!int.TryParse(txtJumlah.Text, out jumlah))
            {
                MessageBox.Show("Jumlah harus berupa angka bulat yang valid.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // *** AWAL PERUBAHAN: Memeriksa duplikasi tiket sebelum menambahkan ***
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM tiket WHERE id_acara = @id_acara AND kategori = @kategori";
                    using (SqlCommand checkCmd = new SqlCommand(checkDuplicateQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@id_acara", idAcara);
                        checkCmd.Parameters.AddWithValue("@kategori", txtKategori.Text);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            MessageBox.Show("Tiket dengan ID Acara dan Kategori yang sama sudah ada. Mohon gunakan kategori atau ID Acara yang berbeda, atau perbarui tiket yang sudah ada.", "Duplikasi Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Hentikan proses jika ditemukan duplikasi
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Terjadi kesalahan database saat memeriksa duplikasi: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Terjadi kesalahan tidak terduga saat memeriksa duplikasi: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // *** AKHIR PERUBAHAN ***

            DialogResult confirmResult = MessageBox.Show("Apakah Anda yakin ingin menambahkan data tiket ini?", "Konfirmasi Penambahan Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();

                    using (SqlCommand cmd = new SqlCommand("sp_TambahTiket", con, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_acara", idAcara);
                        cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                        cmd.Parameters.AddWithValue("@harga", harga);
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);

                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Data tiket berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadData();
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    if (sqlEx.Number == 547)
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
                        transaction.Rollback();
                    }
                    MessageBox.Show($"Terjadi kesalahan tidak terduga saat menambahkan data: {ex.Message}", "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
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

                var confirm = MessageBox.Show("Yakin ingin menghapus data tiket ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction();

                            using (SqlCommand cmd = new SqlCommand("sp_DeleteTiket", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@id_tiket", idTiket);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm();
                                    LoadData();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Data tidak ditemukan atau gagal dihapus. Perubahan dibatalkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
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
            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
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

                if (cmbIdAcara.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(txtKategori.Text) ||
                    string.IsNullOrWhiteSpace(txtHarga.Text) ||
                    string.IsNullOrWhiteSpace(txtJumlah.Text))
                {
                    MessageBox.Show("Semua kolom (ID Acara, Kategori, Harga, Jumlah) harus diisi.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idAcara;
                if (!int.TryParse(cmbIdAcara.SelectedValue.ToString(), out idAcara))
                {
                    MessageBox.Show("ID Acara yang dipilih tidak valid. Silakan pilih kembali dari daftar.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal harga;
                if (!decimal.TryParse(txtHarga.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out harga))
                {
                    MessageBox.Show("Harga harus berupa angka desimal yang valid sesuai pengaturan regional Anda (gunakan koma ',' atau titik '.' sebagai pemisah desimal yang benar).", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int jumlah;
                if (!int.TryParse(txtJumlah.Text, out jumlah))
                {
                    MessageBox.Show("Jumlah harus berupa angka bulat yang valid.", "Kesalahan Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                var konfirmasi = MessageBox.Show("Yakin ingin memperbarui data tiket ini?", "Konfirmasi Pembaruan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (konfirmasi == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            con.Open();
                            transaction = con.BeginTransaction();

                            using (SqlCommand cmd = new SqlCommand("sp_UpdateTiket", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@id_tiket", idTiket);
                                cmd.Parameters.AddWithValue("@id_acara", idAcara);
                                cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                                cmd.Parameters.AddWithValue("@harga", harga); // Mengirim nilai decimal yang sudah benar ke SP
                                cmd.Parameters.AddWithValue("@jumlah", jumlah);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Data berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm();
                                    LoadData();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Data tidak ditemukan atau gagal diperbarui. Perubahan dibatalkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (SqlException sqlEx)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            if (sqlEx.Number == 547)
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
                                transaction.Rollback();
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

                if (row.Cells["id_acara"].Value != DBNull.Value)
                {
                    int idAcaraFromGrid;
                    if (int.TryParse(row.Cells["id_acara"].Value.ToString(), out idAcaraFromGrid))
                    {
                        SelectedAcaraId = idAcaraFromGrid;
                        FillIdAcaraComboBox();
                    }
                }
                else
                {
                    cmbIdAcara.SelectedIndex = -1;
                }

                txtKategori.Text = row.Cells["kategori"].Value?.ToString() ?? string.Empty;

                if (row.Cells["harga"].Value != DBNull.Value)
                {
                    decimal hargaValueFromCell;

                    if (row.Cells["harga"].Value is decimal)
                    {
                        hargaValueFromCell = (decimal)row.Cells["harga"].Value;
                    }
                    else if (decimal.TryParse(row.Cells["harga"].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out hargaValueFromCell))
                    {
                        // Berhasil parsing
                    }
                    else
                    {
                        txtHarga.Clear();
                        MessageBox.Show("Peringatan: Gagal membaca atau mengonversi format harga dari database untuk tiket ini. Nilai default 0 akan digunakan.", "Error Data Harga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        hargaValueFromCell = 0m;
                    }

                    txtHarga.Text = hargaValueFromCell.ToString("N2", CultureInfo.CurrentCulture);
                }
                else
                {
                    txtHarga.Clear();
                }

                txtJumlah.Text = row.Cells["jumlah"].Value?.ToString() ?? string.Empty;
            }
        }

        private void ClearForm()
        {
            cmbIdAcara.SelectedIndex = -1;
            txtKategori.Clear();
            txtHarga.Clear();
            txtJumlah.Clear();
            SelectedAcaraId = -1;
        }

        private void LoadData()
        {
            try
            {
                // Mulai stopwatch untuk mengukur waktu pemuatan database
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT t.id_tiket, t.id_acara, a.nama_acara, t.kategori, t.harga, t.jumlah " +
                                   "FROM tiket t JOIN acara a ON t.id_acara = a.id_acara";
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

                    MessageBox.Show($"Data tiket berhasil dimuat dari database dalam waktu {elapsedTime} detik.", "Waktu Pemuatan Data Tiket", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dataGridViewKelolaTiket.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data tiket: " + ex.Message, "Error Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KelolaTiket_Load(object sender, EventArgs e)
        {
            // Tidak ada perubahan di sini
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide();
            main mn = new main();
            mn.Show();
        }
    }
}