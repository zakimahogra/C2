﻿namespace ucppabdd
{
    partial class KelolaTiket
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
            this.dataGridViewKelolaTiket = new System.Windows.Forms.DataGridView();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            this.btnTambah = new System.Windows.Forms.Button();
            this.txtJumlah = new System.Windows.Forms.TextBox();
            this.txtHarga = new System.Windows.Forms.TextBox();
            this.txtKategori = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIdAcara = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnKembali = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKelolaTiket)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewKelolaTiket
            // 
            this.dataGridViewKelolaTiket.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKelolaTiket.Location = new System.Drawing.Point(73, 262);
            this.dataGridViewKelolaTiket.Name = "dataGridViewKelolaTiket";
            this.dataGridViewKelolaTiket.Size = new System.Drawing.Size(655, 150);
            this.dataGridViewKelolaTiket.TabIndex = 25;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(432, 217);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 24;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnHapus
            // 
            this.btnHapus.Location = new System.Drawing.Point(290, 217);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(75, 23);
            this.btnHapus.TabIndex = 23;
            this.btnHapus.Text = "Delete";
            this.btnHapus.UseVisualStyleBackColor = true;
            this.btnHapus.Click += new System.EventHandler(this.btnHapus_Click);
            // 
            // btnTambah
            // 
            this.btnTambah.Location = new System.Drawing.Point(139, 217);
            this.btnTambah.Name = "btnTambah";
            this.btnTambah.Size = new System.Drawing.Size(75, 23);
            this.btnTambah.TabIndex = 22;
            this.btnTambah.Text = "Add";
            this.btnTambah.UseVisualStyleBackColor = true;
            this.btnTambah.Click += new System.EventHandler(this.btnTambah_Click);
            // 
            // txtJumlah
            // 
            this.txtJumlah.Location = new System.Drawing.Point(265, 157);
            this.txtJumlah.Name = "txtJumlah";
            this.txtJumlah.Size = new System.Drawing.Size(244, 20);
            this.txtJumlah.TabIndex = 20;
            // 
            // txtHarga
            // 
            this.txtHarga.Location = new System.Drawing.Point(265, 113);
            this.txtHarga.Name = "txtHarga";
            this.txtHarga.Size = new System.Drawing.Size(244, 20);
            this.txtHarga.TabIndex = 19;
            // 
            // txtKategori
            // 
            this.txtKategori.Location = new System.Drawing.Point(265, 70);
            this.txtKategori.Name = "txtKategori";
            this.txtKategori.Size = new System.Drawing.Size(244, 20);
            this.txtKategori.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(410, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Jumlah";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Harga";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(118, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Kategori";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtIdAcara
            // 
            this.txtIdAcara.Location = new System.Drawing.Point(265, 34);
            this.txtIdAcara.Name = "txtIdAcara";
            this.txtIdAcara.Size = new System.Drawing.Size(244, 20);
            this.txtIdAcara.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "id_acara";
            // 
            // btnKembali
            // 
            this.btnKembali.Location = new System.Drawing.Point(587, 102);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(75, 23);
            this.btnKembali.TabIndex = 28;
            this.btnKembali.Text = "Back";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click);
            // 
            // KelolaTiket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnKembali);
            this.Controls.Add(this.txtIdAcara);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridViewKelolaTiket);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnTambah);
            this.Controls.Add(this.txtJumlah);
            this.Controls.Add(this.txtHarga);
            this.Controls.Add(this.txtKategori);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "KelolaTiket";
            this.Text = "KelolaTiket";
            this.Load += new System.EventHandler(this.KelolaTiket_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKelolaTiket)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewKelolaTiket;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Button btnTambah;
        private System.Windows.Forms.TextBox txtJumlah;
        private System.Windows.Forms.TextBox txtHarga;
        private System.Windows.Forms.TextBox txtKategori;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIdAcara;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnKembali;
    }
}