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
    public partial class KelolaDataPeserta : Form
    {
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_management;Integrated Security=True;";
        public KelolaDataPeserta()
        {
            InitializeComponent();
            LoadData();
            dataGridViewKelolaDataPeserta.CellClick += dataGridViewKelolaDataPeserta_CellContentClick;
        }

        private void KelolaDataPeserta_Load(object sender, EventArgs e)
        {

        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO data_peserta (nama, email, no_hp, id_acara, id_tiket) VALUES (@nama, @email, @no_hp, @id_acara, @id_tiket)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text); // pastikan formatnya sesuai DATE
                cmd.Parameters.AddWithValue("@no_hp", txtNoHp.Text);
                cmd.Parameters.AddWithValue("@id_acara", txtIdAcara.Text);
                cmd.Parameters.AddWithValue("@id_tiket", txtIdTiket.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewKelolaDataPeserta.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data peserta ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM data_peserta WHERE id_peserta = @id";
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
            if (dataGridViewKelolaDataPeserta.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewKelolaDataPeserta.CurrentRow.Cells["id_peserta"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE data_peserta SET nama=@nama, email=@email, no_hp=@no_hp, id_acara=@id_acara, id_tiket=@id_tiket WHERE id_peserta=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@no_hp", txtNoHp.Text);
                    cmd.Parameters.AddWithValue("@id_acara", txtIdAcara.Text);
                    cmd.Parameters.AddWithValue("@id_tiket", txtIdTiket.Text);
                    cmd.Parameters.AddWithValue("@id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui");
                    ClearForm();
                    LoadData();
                }
            }
        }

        private void dataGridViewKelolaDataPeserta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewKelolaDataPeserta.Rows[e.RowIndex];

                txtNama.Text = row.Cells["nama"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                txtNoHp.Text = row.Cells["no_hp"].Value.ToString();
                txtIdAcara.Text = row.Cells["id_acara"].Value.ToString();
                txtIdTiket.Text = row.Cells["id_tiket"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtNama.Clear();
            txtEmail.Clear();
            txtNoHp.Clear();
            txtIdAcara.Clear();
            txtIdTiket.Clear();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM data_peserta";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKelolaDataPeserta.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }
    }
}
