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
    public partial class KelolaPembayaran : Form
    {
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_management;Integrated Security=True;";
        public KelolaPembayaran()
        {
            InitializeComponent();
            LoadData();
            dataGridViewKelolaPembayaran.CellClick += dataGridViewKelolaPembayaran_CellContentClick;
        }

        private void KelolaPembayaran_Load(object sender, EventArgs e)
        {

        }
    

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO pembayaran (id_peserta, jumlah, metode_pembayaran, status, tanggal_pembayaran) VALUES (@id_peserta, @jumlah, @metode_pembayaran, @status, @tanggal_pembayaran)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id_peserta", txtIdPeserta.Text);
                cmd.Parameters.AddWithValue("@jumlah", txtJumlah.Text); // pastikan formatnya sesuai DATE
                cmd.Parameters.AddWithValue("@metode_pembayaran", txtMetodePembayaran.Text);
                cmd.Parameters.AddWithValue("@status", txtStatus.Text);
                cmd.Parameters.AddWithValue("@tanggal_pembayaran", txtTanggalPembayaran.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaPembayaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data pembayaran ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM pembayaran WHERE id_pembayaran = @id";
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
            if (dataGridViewKelolaPembayaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaPembayaran.CurrentRow.Cells["id_pembayaran"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE pembayaran SET id_peserta=@id_peserta, jumlah=@jumlah, metode_pembayaran=@metode_pembayaran, status=@status, tanggal_pembayaran=@tanggal_pembayaran WHERE id_pembayaran=@id";

                    SqlCommand cmd = new SqlCommand(query, con);

                    int idPeserta;
                    if (!int.TryParse(txtIdPeserta.Text, out idPeserta))
                    {
                        MessageBox.Show("ID Acara tidak valid.");
                        return;
                    }
                    decimal jumlah;
                    if (!decimal.TryParse(txtJumlah.Text, out jumlah))
                    {
                        MessageBox.Show("Jumlah tidak valid.");
                        return;
                    }


                    cmd.Parameters.AddWithValue("@id_peserta", idPeserta);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@metode_pembayaran", txtMetodePembayaran.Text);
                    cmd.Parameters.AddWithValue("@status", txtStatus.Text);
                    cmd.Parameters.AddWithValue("@tanggal_pembayaran", txtTanggalPembayaran.Text);
                    cmd.Parameters.AddWithValue("@id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui");
                    ClearForm();
                    LoadData();
                }
            }
        }

        private void dataGridViewKelolaPembayaran_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaPembayaran.Rows[e.RowIndex];

                txtIdPeserta.Text = row.Cells["id_peserta"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah"].Value.ToString();
                txtMetodePembayaran.Text = row.Cells["metode_pembayaran"].Value.ToString();
                txtStatus.Text = row.Cells["status"].Value.ToString();
                txtTanggalPembayaran.Text = row.Cells["tanggal_pembayaran"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtIdPeserta.Clear();
            txtJumlah.Clear();
            txtMetodePembayaran.Clear();
            txtStatus.Clear();
            txtTanggalPembayaran.Clear();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM pembayaran";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKelolaPembayaran.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }
    }
}

