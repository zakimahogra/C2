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
        public ReportAcara()
        {
            InitializeComponent();
        }

        private void ReportAcara_Load(object sender, EventArgs e)
        {
      
            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
            string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_managementt;Integrated Security=True;";

            // SQL query to retrieve the required data from the database
            string query = @"
                SELECT        acara.nama_acara, acara.id_acara, acara.tanggal, acara.lokasi, tiket.id_tiket, tiket.kategori, tiket.harga
FROM            acara INNER JOIN
                         tiket ON acara.id_acara = tiket.id_acara";

            // Create a DataTable to store the data
            DataTable dt = new DataTable();

            // Use SqlDataAdapter to fill the DataTable with data from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            // Create a ReportDataSource
            ReportDataSource rds = new ReportDataSource("DataSet1", dt); // Make sure "DataSet1" matches your RDLC dataset name

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
