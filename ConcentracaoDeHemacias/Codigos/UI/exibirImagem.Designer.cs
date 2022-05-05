namespace ConcentracaoDeHemacias.Codigos.UI
{
    partial class exibirImagem
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
            this.ExportarButton = new System.Windows.Forms.Button();
            this.imagem = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imagem)).BeginInit();
            this.SuspendLayout();
            // 
            // ExportarButton
            // 
            this.ExportarButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExportarButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExportarButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.ExportarButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportarButton.ForeColor = System.Drawing.Color.White;
            this.ExportarButton.Location = new System.Drawing.Point(0, 205);
            this.ExportarButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ExportarButton.Name = "ExportarButton";
            this.ExportarButton.Size = new System.Drawing.Size(235, 25);
            this.ExportarButton.TabIndex = 1;
            this.ExportarButton.Text = "Exportar";
            this.ExportarButton.UseVisualStyleBackColor = true;
            this.ExportarButton.Click += new System.EventHandler(this.ExportarButton_Click);
            // 
            // imagem
            // 
            this.imagem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.imagem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagem.Location = new System.Drawing.Point(0, 0);
            this.imagem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.imagem.Name = "imagem";
            this.imagem.Size = new System.Drawing.Size(235, 230);
            this.imagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imagem.TabIndex = 0;
            this.imagem.TabStop = false;
            // 
            // exibirImagem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(235, 230);
            this.Controls.Add(this.ExportarButton);
            this.Controls.Add(this.imagem);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "exibirImagem";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "exibirImagem";
            ((System.ComponentModel.ISupportInitialize)(this.imagem)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imagem;
        private System.Windows.Forms.Button ExportarButton;
    }
}