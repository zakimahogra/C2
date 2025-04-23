namespace ucppabdd
{
    partial class KelolaPembayaran
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
            this.txtIdPeserta = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridViewKelolaPembayaran = new System.Windows.Forms.DataGridView();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            this.btnTambah = new System.Windows.Forms.Button();
            this.txtJumlah = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtMetodePembayaran = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTanggalPembayaran = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKelolaPembayaran)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIdPeserta
            // 
            this.txtIdPeserta.Location = new System.Drawing.Point(265, 23);
            this.txtIdPeserta.Name = "txtIdPeserta";
            this.txtIdPeserta.Size = new System.Drawing.Size(244, 20);
            this.txtIdPeserta.TabIndex = 40;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "id_peserta";
            // 
            // dataGridViewKelolaPembayaran
            // 
            this.dataGridViewKelolaPembayaran.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKelolaPembayaran.Location = new System.Drawing.Point(73, 264);
            this.dataGridViewKelolaPembayaran.Name = "dataGridViewKelolaPembayaran";
            this.dataGridViewKelolaPembayaran.Size = new System.Drawing.Size(655, 150);
            this.dataGridViewKelolaPembayaran.TabIndex = 38;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(432, 219);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 37;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnHapus
            // 
            this.btnHapus.Location = new System.Drawing.Point(290, 219);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(75, 23);
            this.btnHapus.TabIndex = 36;
            this.btnHapus.Text = "Delete";
            this.btnHapus.UseVisualStyleBackColor = true;
            this.btnHapus.Click += new System.EventHandler(this.btnHapus_Click);
            // 
            // btnTambah
            // 
            this.btnTambah.Location = new System.Drawing.Point(139, 219);
            this.btnTambah.Name = "btnTambah";
            this.btnTambah.Size = new System.Drawing.Size(75, 23);
            this.btnTambah.TabIndex = 35;
            this.btnTambah.Text = "Add";
            this.btnTambah.UseVisualStyleBackColor = true;
            this.btnTambah.Click += new System.EventHandler(this.btnTambah_Click);
            // 
            // txtJumlah
            // 
            this.txtJumlah.Location = new System.Drawing.Point(265, 62);
            this.txtJumlah.Name = "txtJumlah";
            this.txtJumlah.Size = new System.Drawing.Size(244, 20);
            this.txtJumlah.TabIndex = 34;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(265, 151);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(244, 20);
            this.txtStatus.TabIndex = 33;
            // 
            // txtMetodePembayaran
            // 
            this.txtMetodePembayaran.Location = new System.Drawing.Point(265, 108);
            this.txtMetodePembayaran.Name = "txtMetodePembayaran";
            this.txtMetodePembayaran.Size = new System.Drawing.Size(244, 20);
            this.txtMetodePembayaran.TabIndex = 32;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(410, 186);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Jumlah";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Metode_Pembayaran";
            // 
            // txtTanggalPembayaran
            // 
            this.txtTanggalPembayaran.Location = new System.Drawing.Point(265, 186);
            this.txtTanggalPembayaran.Name = "txtTanggalPembayaran";
            this.txtTanggalPembayaran.Size = new System.Drawing.Size(244, 20);
            this.txtTanggalPembayaran.TabIndex = 42;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(118, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Tanggal_Pembayaran";
            // 
            // KelolaPembayaran
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtTanggalPembayaran);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtIdPeserta);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridViewKelolaPembayaran);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnTambah);
            this.Controls.Add(this.txtJumlah);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtMetodePembayaran);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "KelolaPembayaran";
            this.Text = "KelolaPembayaran";
            this.Load += new System.EventHandler(this.KelolaPembayaran_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKelolaPembayaran)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIdPeserta;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridViewKelolaPembayaran;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Button btnTambah;
        private System.Windows.Forms.TextBox txtJumlah;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtMetodePembayaran;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTanggalPembayaran;
        private System.Windows.Forms.Label label6;
    }
}