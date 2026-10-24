namespace GeoLab2D
{
    partial class GeoLab2D
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
            this.encabezado = new System.Windows.Forms.Label();
            this.contenedorLienzo = new System.Windows.Forms.Panel();
            this.lienzo = new GeoLab2D.Controles.LienzoGeometrico();
            this.estado = new System.Windows.Forms.Label();
            this.contenedorLienzo.SuspendLayout();
            this.SuspendLayout();
            // 
            // encabezado
            // 
            this.encabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.encabezado.Location = new System.Drawing.Point(0, 0);
            this.encabezado.Name = "encabezado";
            this.encabezado.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.encabezado.Size = new System.Drawing.Size(850, 60);
            this.encabezado.TabIndex = 2;
            this.encabezado.Text = "GeoLab2D | Laboratorio de geometría bidimensional\r\n";
            this.encabezado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.encabezado.Click += new System.EventHandler(this.encabezado_Click);
            // 
            // contenedorLienzo
            // 
            this.contenedorLienzo.AutoScroll = true;
            this.contenedorLienzo.AutoScrollMinSize = new System.Drawing.Size(832, 632);
            this.contenedorLienzo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.contenedorLienzo.Controls.Add(this.lienzo);
            this.contenedorLienzo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contenedorLienzo.Location = new System.Drawing.Point(0, 60);
            this.contenedorLienzo.Name = "contenedorLienzo";
            this.contenedorLienzo.Size = new System.Drawing.Size(850, 642);
            this.contenedorLienzo.TabIndex = 0;
            // 
            // lienzo
            // 
            this.lienzo.BackColor = System.Drawing.Color.White;
            this.lienzo.Location = new System.Drawing.Point(16, 16);
            this.lienzo.MinimumSize = new System.Drawing.Size(800, 600);
            this.lienzo.Name = "lienzo";
            this.lienzo.Size = new System.Drawing.Size(800, 600);
            this.lienzo.TabIndex = 0;
            // 
            // estado
            // 
            this.estado.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.estado.Location = new System.Drawing.Point(0, 702);
            this.estado.Name = "estado";
            this.estado.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.estado.Size = new System.Drawing.Size(850, 28);
            this.estado.TabIndex = 1;
            this.estado.Text = "Lienzo: 800 × 600 píxeles | Próxima etapa: hexágono de seis colores";
            this.estado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GeoLab2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 730);
            this.Controls.Add(this.contenedorLienzo);
            this.Controls.Add(this.estado);
            this.Controls.Add(this.encabezado);
            this.MinimumSize = new System.Drawing.Size(520, 420);
            this.Name = "GeoLab2D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GeoLab2D - Computación gráfica";
            this.contenedorLienzo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label encabezado;
        private System.Windows.Forms.Panel contenedorLienzo;
        private Controles.LienzoGeometrico lienzo;
        private System.Windows.Forms.Label estado;
    }
}
