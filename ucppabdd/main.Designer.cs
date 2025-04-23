namespace ucppabdd
{
    partial class main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnKelolaAcara = new System.Windows.Forms.Button();
            this.btnKelolaTiket = new System.Windows.Forms.Button();
            this.btnKelolaDataPeserta = new System.Windows.Forms.Button();
            this.btnKelolaPembayaran = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnKelolaAcara
            // 
            this.btnKelolaAcara.Location = new System.Drawing.Point(309, 79);
            this.btnKelolaAcara.Name = "btnKelolaAcara";
            this.btnKelolaAcara.Size = new System.Drawing.Size(151, 23);
            this.btnKelolaAcara.TabIndex = 0;
            this.btnKelolaAcara.Text = "Kelola Acara";
            this.btnKelolaAcara.UseVisualStyleBackColor = true;
            this.btnKelolaAcara.Click += new System.EventHandler(this.btnKelolaAcara_Click);
            // 
            // btnKelolaTiket
            // 
            this.btnKelolaTiket.Location = new System.Drawing.Point(309, 135);
            this.btnKelolaTiket.Name = "btnKelolaTiket";
            this.btnKelolaTiket.Size = new System.Drawing.Size(151, 23);
            this.btnKelolaTiket.TabIndex = 1;
            this.btnKelolaTiket.Text = "Kelola Tiket";
            this.btnKelolaTiket.UseVisualStyleBackColor = true;
            this.btnKelolaTiket.Click += new System.EventHandler(this.btnKelolaTiket_Click);
            // 
            // btnKelolaDataPeserta
            // 
            this.btnKelolaDataPeserta.Location = new System.Drawing.Point(309, 193);
            this.btnKelolaDataPeserta.Name = "btnKelolaDataPeserta";
            this.btnKelolaDataPeserta.Size = new System.Drawing.Size(151, 23);
            this.btnKelolaDataPeserta.TabIndex = 2;
            this.btnKelolaDataPeserta.Text = "Kelola Data Peserta";
            this.btnKelolaDataPeserta.UseVisualStyleBackColor = true;
            this.btnKelolaDataPeserta.Click += new System.EventHandler(this.btnKelolaDataPeserta_Click);
            // 
            // btnKelolaPembayaran
            // 
            this.btnKelolaPembayaran.Location = new System.Drawing.Point(309, 254);
            this.btnKelolaPembayaran.Name = "btnKelolaPembayaran";
            this.btnKelolaPembayaran.Size = new System.Drawing.Size(151, 23);
            this.btnKelolaPembayaran.TabIndex = 3;
            this.btnKelolaPembayaran.Text = "Kelola Pembayaran";
            this.btnKelolaPembayaran.UseVisualStyleBackColor = true;
            this.btnKelolaPembayaran.Click += new System.EventHandler(this.btnKelolaPembayaran_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnKelolaPembayaran);
            this.Controls.Add(this.btnKelolaDataPeserta);
            this.Controls.Add(this.btnKelolaTiket);
            this.Controls.Add(this.btnKelolaAcara);
            this.Name = "main";
            this.Text = "main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnKelolaAcara;
        private System.Windows.Forms.Button btnKelolaTiket;
        private System.Windows.Forms.Button btnKelolaDataPeserta;
        private System.Windows.Forms.Button btnKelolaPembayaran;
    }
}