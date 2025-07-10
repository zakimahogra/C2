using Microsoft.Reporting.WinForms;
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
    public partial class ReportAcara : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";

        public ReportAcara()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
        }

        private void ReportAcara_Load(object sender, EventArgs e)
        {
      
            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
            

            // SQL query to retrieve the required data from the database
            string query = @"
        SELECT        a.nama_acara, SUM(t.jumlah) AS jumlah_tiket_terjual, SUM(t.harga * t.jumlah) AS total_pendapatan
FROM            acara AS a INNER JOIN
                         tiket AS t ON a.id_acara = t.id_acara
GROUP BY a.nama_acara";

            // Create a DataTable to store the data retrieved from the database
            DataTable dt = new DataTable();

            // Use SqlDataAdapter to fill the DataTable with data from the database
            // The 'using' statement ensures that the SqlConnection object is properly disposed of.
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                try
                {
                    conn.Open(); // Open the connection
                    da.Fill(dt); // Fill the DataTable
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during database operations
                    // You might want to log this error or display it to the user in a message box
                    Console.WriteLine("Error filling DataTable: " + ex.Message);
                    // Optionally, display a user-friendly message
                    // MessageBox.Show("Terjadi kesalahan saat memuat data laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Create a ReportDataSource.
            // Make sure "DataSet2" matches the name of your RDLC dataset name.
            ReportDataSource rds = new ReportDataSource("DataSet2", dt); // Changed from "DataSet1" to "DataSet2"

            // Clear any existing data sources and add the new data source
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            // Set the path to the report (.rdlc file)
            // Change this to the actual path of your RDLC file
            reportViewer1.LocalReport.ReportPath = @"D:\PEMROGRAMAN\PABD\ucppabdd\ucppabdd\ReportAcaradanTiket.rdlc";

            // Refresh the ReportViewer to show the updated report
            reportViewer1.RefreshReport();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Hide(); // Sembunyikan form saat ini
            main mn = new main(); // Buat instance form menu utama Anda (ganti 'main' jika nama form Anda berbeda)
            mn.Show(); // Tampilkan form menu utama
        }
    }
}
