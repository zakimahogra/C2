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

namespace ucppabdd
{
    public partial class KelolaTiket : Form
    {
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_management;Integrated Security=True;";
        public KelolaTiket()
        {
            InitializeComponent();
            LoadData();
            dataGridViewKelolaTiket.CellClick += dataGridViewKelolaTiket_CellContentClick;
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO tiket (id_acara, kategori, harga, jumlah) VALUES (@id_acara, @kategori, @harga, @jumlah)";
                SqlCommand cmd = new SqlCommand(query, con);

                // Validasi dan konversi input
                if (!int.TryParse(txtIdAcara.Text, out int idAcara))
                {
                    MessageBox.Show("ID Acara harus berupa angka.");
                    return;
                }

                if (!decimal.TryParse(txtHarga.Text, out decimal harga))
                {
                    MessageBox.Show("Harga harus berupa angka desimal (contoh: 50000.00).");
                    return;
                }

                if (!int.TryParse(txtJumlah.Text, out int jumlah))
                {
                    MessageBox.Show("Jumlah harus berupa angka.");
                    return;
                }

                cmd.Parameters.AddWithValue("@id_acara", idAcara);
                cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@jumlah", jumlah);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data tiket ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM tiket WHERE id_tiket = @id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadData();
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            if (dataGridViewKelolaTiket.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaTiket.CurrentRow.Cells["id_tiket"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tiket SET id_acara=@id_acara, kategori=@kategori, harga=@harga, jumlah=@jumlah WHERE id_tiket=@id";

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Konversi nilai input ke tipe data yang sesuai
                    int idAcara;
                    if (!int.TryParse(txtIdAcara.Text, out idAcara))
                    {
                        MessageBox.Show("ID Acara tidak valid.");
                        return;
                    }

                    decimal harga;
                    if (!decimal.TryParse(txtHarga.Text, out harga))
                    {
                        MessageBox.Show("Harga tidak valid.");
                        return;
                    }

                    int jumlah;
                    if (!int.TryParse(txtJumlah.Text, out jumlah))
                    {
                        MessageBox.Show("Jumlah tidak valid.");
                        return;
                    }

                    // Menambahkan parameter yang sudah terkonversi
                    cmd.Parameters.AddWithValue("@id_acara", idAcara);
                    cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                    cmd.Parameters.AddWithValue("@harga", harga);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui");
                    ClearForm();
                    LoadData();
                }
            }
        }

        private void dataGridViewKelolaTiket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaTiket.Rows[e.RowIndex];

                txtIdAcara.Text = row.Cells["id_acara"].Value.ToString();
                txtKategori.Text = row.Cells["kategori"].Value.ToString();
                txtHarga.Text = row.Cells["harga"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtIdAcara.Clear();
            txtKategori.Clear();
            txtHarga.Clear();
            txtJumlah.Clear();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM tiket";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKelolaTiket.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void KelolaTiket_Load(object sender, EventArgs e)
        {

        }
    }
}
