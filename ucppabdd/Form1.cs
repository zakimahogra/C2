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
    public partial class Form1 : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";
        
        public Form1()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            connectionString = kn.connectionString();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;


            try
            {
                // Gunakan parameterized query untuk mencegah SQL Injection
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserName, Password FROM admin WHERE UserName = @username AND Password = @password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    // Buka koneksi ke database
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Cek apakah data ditemukan
                    if (dt.Rows.Count > 0)
                    {
                        // Jika login berhasil, buka form utama
                        main mn = new main();
                        mn.Show();
                        this.Hide();  // Sembunyikan form login setelah login berhasil
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan yang terjadi selama proses login
                MessageBox.Show("Error: " + ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // --- Validasi Input Kosong ---
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong.", "Validasi Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Hentikan proses jika input kosong
            }

            try
            {
                // Gunakan parameterized query untuk mencegah SQL Injection
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Menggunakan COUNT(*) untuk memeriksa keberadaan kredensial
                    string query = "SELECT COUNT(*) FROM admin WHERE UserName = @username AND Password = @password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    con.Open();

                    // ExecuteScalar digunakan untuk mengambil nilai tunggal (dalam kasus ini, jumlah baris)
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // --- Pesan Login Berhasil ---
                        MessageBox.Show("Login Anda berhasil!", "Login Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Jika login berhasil, buka form utama
                        main mn = new main();
                        mn.Show();
                        this.Hide(); // Sembunyikan form login setelah login berhasil
                    }
                    else
                    {
                        MessageBox.Show("Username atau Password salah. Silakan coba lagi.", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Tangani kesalahan khusus SQL Server
                MessageBox.Show($"Terjadi kesalahan database: {sqlEx.Message}\nKode Error: {sqlEx.Number}", "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Tangani kesalahan umum yang mungkin terjadi selama proses login
                MessageBox.Show("Terjadi kesalahan tidak terduga: " + ex.Message, "Kesalahan Aplikasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}