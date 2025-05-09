﻿using System;
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
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_management;Integrated Security=True;";
        public Form1()
        {
            InitializeComponent();
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
    }
}