using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ucppabdd
{
    public partial class main : Form
    {
        static string connectionString = "Data Source=MSI\\ZAKIMAHOGRA;Initial Catalog=event_managementt;Integrated Security=True;";
        public main()
        {
            InitializeComponent();
        }

        private void btnKelolaAcara_Click(object sender, EventArgs e)
        {
            KelolaAcara ka = new KelolaAcara();
            ka.Show();
            this.Hide();  // Sembunyikan form login setelah login berhasil
        }

        private void btnKelolaTiket_Click(object sender, EventArgs e)
        {
            KelolaTiket kt = new KelolaTiket();
            kt.Show();
            this.Hide();  // Sembunyikan form login setelah login berhasil
        }

        private void btnKelolaDataPeserta_Click(object sender, EventArgs e)
        {
            KelolaDataPeserta kd = new KelolaDataPeserta();
            kd.Show();
            this.Hide();  // Sembunyikan form login setelah login berhasil
        }

        private void btnKelolaPembayaran_Click(object sender, EventArgs e)
        {
            KelolaPembayaran kp = new KelolaPembayaran();
            kp.Show();
            this.Hide();  // Sembunyikan form login setelah login berhasil
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReportAcara reportAcara = new ReportAcara();
            reportAcara.Show();
            this.Hide();
        }
    }
}
