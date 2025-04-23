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

    public partial class KelolaAcara : Form
    {
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_management;Integrated Security=True;";
        public KelolaAcara()
        {
            InitializeComponent();
            LoadData();
            dataGridViewKelolaAcara.CellClick += dataGridViewKelolaAcara_CellContentClick;
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO acara (nama_acara, tanggal, lokasi, deskripsi) VALUES (@namaacara, @tanggal, @lokasi, @deskripsi)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@namaacara", txtNamaAcara.Text);
                cmd.Parameters.AddWithValue("@tanggal", DateTime.Parse(txtTanggal.Text)); // pastikan formatnya sesuai DATE
                cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaAcara.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data acara ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM acara WHERE id_acara = @id";
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
            if (dataGridViewKelolaAcara.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaAcara.CurrentRow.Cells["id_acara"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE acara SET nama_acara=@nama_acara, tanggal=@tanggal, lokasi=@lokasi, deskripsi=@deskripsi WHERE id_acara=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nama_acara", txtNamaAcara.Text);

                    DateTime tanggalAcara;
                    if (!DateTime.TryParse(txtTanggal.Text, out tanggalAcara))
                    {
                        MessageBox.Show("Format tanggal tidak valid. Harap masukkan tanggal yang benar.");
                        return;
                    }
                    cmd.Parameters.AddWithValue("@tanggal", tanggalAcara);

                    cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                    cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);
                    cmd.Parameters.AddWithValue("@id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui");
                    ClearForm();
                    LoadData();
                }
            }
        }

        private void dataGridViewKelolaAcara_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaAcara.Rows[e.RowIndex];

                txtNamaAcara.Text = row.Cells["nama_acara"].Value.ToString();
                txtTanggal.Text = row.Cells["tanggal"].Value.ToString();
                txtLokasi.Text = row.Cells["lokasi"].Value.ToString();
                txtDeskripsi.Text = row.Cells["deskripsi"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtNamaAcara.Clear();
            txtTanggal.Clear();
            txtLokasi.Clear();
            txtDeskripsi.Clear();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM acara";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKelolaAcara.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // sembunyikan form ini
            main mn = new main(); // ganti dengan nama form menu kamu
            mn.Show(); 
        }
    }
}
