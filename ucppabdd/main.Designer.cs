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
            this.buttonLaporan = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnKelolaAcara
            // 
            this.btnKelolaAcara.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnKelolaAcara.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKelolaAcara.Location = new System.Drawing.Point(552, 174);
            this.btnKelolaAcara.Name = "btnKelolaAcara";
            this.btnKelolaAcara.Size = new System.Drawing.Size(176, 32);
            this.btnKelolaAcara.TabIndex = 0;
            this.btnKelolaAcara.Text = "Kelola Acara";
            this.btnKelolaAcara.UseVisualStyleBackColor = false;
            this.btnKelolaAcara.Click += new System.EventHandler(this.btnKelolaAcara_Click);
            // 
            // btnKelolaTiket
            // 
            this.btnKelolaTiket.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnKelolaTiket.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKelolaTiket.Location = new System.Drawing.Point(552, 230);
            this.btnKelolaTiket.Name = "btnKelolaTiket";
            this.btnKelolaTiket.Size = new System.Drawing.Size(176, 31);
            this.btnKelolaTiket.TabIndex = 1;
            this.btnKelolaTiket.Text = "Kelola Tiket";
            this.btnKelolaTiket.UseVisualStyleBackColor = false;
            this.btnKelolaTiket.Click += new System.EventHandler(this.btnKelolaTiket_Click);
            // 
            // btnKelolaDataPeserta
            // 
            this.btnKelolaDataPeserta.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnKelolaDataPeserta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKelolaDataPeserta.Location = new System.Drawing.Point(552, 288);
            this.btnKelolaDataPeserta.Name = "btnKelolaDataPeserta";
            this.btnKelolaDataPeserta.Size = new System.Drawing.Size(176, 32);
            this.btnKelolaDataPeserta.TabIndex = 2;
            this.btnKelolaDataPeserta.Text = "Kelola Data Peserta";
            this.btnKelolaDataPeserta.UseVisualStyleBackColor = false;
            this.btnKelolaDataPeserta.Click += new System.EventHandler(this.btnKelolaDataPeserta_Click);
            // 
            // btnKelolaPembayaran
            // 
            this.btnKelolaPembayaran.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnKelolaPembayaran.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKelolaPembayaran.Location = new System.Drawing.Point(552, 345);
            this.btnKelolaPembayaran.Name = "btnKelolaPembayaran";
            this.btnKelolaPembayaran.Size = new System.Drawing.Size(176, 32);
            this.btnKelolaPembayaran.TabIndex = 3;
            this.btnKelolaPembayaran.Text = "Kelola Pembayaran";
            this.btnKelolaPembayaran.UseVisualStyleBackColor = false;
            this.btnKelolaPembayaran.Click += new System.EventHandler(this.btnKelolaPembayaran_Click);
            // 
            // buttonLaporan
            // 
            this.buttonLaporan.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonLaporan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLaporan.Location = new System.Drawing.Point(552, 393);
            this.buttonLaporan.Name = "buttonLaporan";
            this.buttonLaporan.Size = new System.Drawing.Size(176, 31);
            this.buttonLaporan.TabIndex = 4;
            this.buttonLaporan.Text = "Laporan Acara";
            this.buttonLaporan.UseVisualStyleBackColor = false;
            this.buttonLaporan.Click += new System.EventHandler(this.button1_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ucppabdd.Properties.Resources._0166e6f8739a837264dc6dbb9c959de1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1276, 625);
            this.Controls.Add(this.buttonLaporan);
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
        private System.Windows.Forms.Button buttonLaporan;
    }
}