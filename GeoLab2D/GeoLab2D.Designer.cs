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
            this.lienzo = new global::GeoLab2D.Controles.LienzoGeometrico();
            this.estado = new System.Windows.Forms.Label();
            this.parametros = new System.Windows.Forms.FlowLayoutPanel();
            this.etiquetaLado = new System.Windows.Forms.Label();
            this.lado = new System.Windows.Forms.NumericUpDown();
            this.selectorFigura = new System.Windows.Forms.ComboBox();
            this.puntas = new System.Windows.Forms.NumericUpDown();
            this.salto = new System.Windows.Forms.NumericUpDown();
            this.etiquetaPuntas = new System.Windows.Forms.Label();
            this.etiquetaSalto = new System.Windows.Forms.Label();
            this.panelVectores = new global::GeoLab2D.Controles.PanelVectores();
            this.panelAnalisis = new global::GeoLab2D.Controles.PanelAnalisis();
            this.panelTransformaciones = new global::GeoLab2D.Controles.PanelTransformaciones();
            ((System.ComponentModel.ISupportInitialize)(this.puntas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lado)).BeginInit();
            this.parametros.SuspendLayout();
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
            // parametros
            this.parametros.Dock = System.Windows.Forms.DockStyle.Top;
            this.parametros.Height = 44;
            this.parametros.AutoSize = true;
            this.parametros.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.parametros.Padding = new System.Windows.Forms.Padding(16, 6, 0, 6);
            this.parametros.Controls.Add(this.selectorFigura);
            this.parametros.Controls.Add(this.etiquetaLado);
            this.parametros.Controls.Add(this.lado);
            this.parametros.Controls.Add(this.etiquetaPuntas);
            this.parametros.Controls.Add(this.puntas);
            this.parametros.Controls.Add(this.etiquetaSalto);
            this.parametros.Controls.Add(this.salto);
            this.selectorFigura.Name = "selectorFigura";
            this.selectorFigura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectorFigura.Width = 220;
            this.selectorFigura.Items.AddRange(new object[] { "Hexágono de seis colores", "Flor de margarita", "Polígono estrellado", "Operaciones vectoriales", "Análisis geométrico" });
            this.selectorFigura.SelectedIndexChanged += new System.EventHandler(this.selectorFigura_SelectedIndexChanged);
            this.etiquetaLado.AutoSize = true;
            this.etiquetaLado.Margin = new System.Windows.Forms.Padding(0, 4, 8, 0);
            this.etiquetaLado.Text = "Lado del hexágono (px):";
            this.lado.Name = "lado";
            this.lado.DecimalPlaces = 1;
            this.lado.Minimum = 1;
            this.lado.Maximum = 280;
            this.lado.Value = 150;
            this.lado.Width = 100;
            this.lado.ValueChanged += new System.EventHandler(this.lado_ValueChanged);
            // Parámetros específicos del polígono estrellado
            this.etiquetaPuntas.AutoSize = true;
            this.etiquetaPuntas.Text = "Puntas:";
            this.etiquetaPuntas.Margin = new System.Windows.Forms.Padding(8, 4, 2, 0);
            this.etiquetaSalto.AutoSize = true;
            this.etiquetaSalto.Text = "Salto:";
            this.etiquetaSalto.Margin = new System.Windows.Forms.Padding(8, 4, 2, 0);
            this.puntas.Name = "puntas";
            this.puntas.Minimum = 5;
            this.puntas.Maximum = 64;
            this.puntas.Value = 5;
            this.puntas.Width = 60;
            this.puntas.ValueChanged += new System.EventHandler(this.parametrosEstrella_ValueChanged);
            this.salto.Name = "salto";
            this.salto.Minimum = 2;
            this.salto.Maximum = 2;
            this.salto.Value = 2;
            this.salto.Width = 60;
            this.salto.ValueChanged += new System.EventHandler(this.parametrosEstrella_ValueChanged);
            this.panelVectores.Name = "panelVectores";
            this.panelVectores.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVectores.Visible = false;
            this.panelVectores.VectoresCambiados += new System.EventHandler(this.parametrosVectores_Cambiados);
            this.panelAnalisis.Name = "panelAnalisis";
            this.panelAnalisis.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAnalisis.Visible = false;
            this.panelAnalisis.DatosCambiados += new System.EventHandler(this.parametrosAnalisis_Cambiados);
            this.panelTransformaciones.Name = "panelTransformaciones";
            this.panelTransformaciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTransformaciones.TransformacionCambiada += new System.EventHandler(this.parametrosTransformacion_Cambiados);
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
            this.estado.Size = new System.Drawing.Size(850, 48);
            this.estado.TabIndex = 1;
            this.estado.Text = "GeoLab2D | Construcción de figuras";
            this.estado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GeoLab2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 794);
            this.Controls.Add(this.contenedorLienzo);
            this.Controls.Add(this.estado);
            this.Controls.Add(this.panelVectores);
            this.Controls.Add(this.panelAnalisis);
            this.Controls.Add(this.panelTransformaciones);
            this.Controls.Add(this.parametros);
            this.Controls.Add(this.encabezado);
            this.MinimumSize = new System.Drawing.Size(520, 420);
            this.Name = "GeoLab2D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GeoLab2D - Computación gráfica";
            this.contenedorLienzo.ResumeLayout(false);
            this.parametros.ResumeLayout(false);
            this.parametros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.puntas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label encabezado;
        private System.Windows.Forms.Panel contenedorLienzo;
        private Controles.LienzoGeometrico lienzo;
        private System.Windows.Forms.Label estado;
        private System.Windows.Forms.FlowLayoutPanel parametros;
        private System.Windows.Forms.Label etiquetaLado;
        private System.Windows.Forms.NumericUpDown lado;
        private System.Windows.Forms.ComboBox selectorFigura;
        private System.Windows.Forms.NumericUpDown puntas;
        private System.Windows.Forms.NumericUpDown salto;
        private System.Windows.Forms.Label etiquetaPuntas;
        private System.Windows.Forms.Label etiquetaSalto;
        private Controles.PanelVectores panelVectores;
        private Controles.PanelAnalisis panelAnalisis;
        private Controles.PanelTransformaciones panelTransformaciones;
    }
}
