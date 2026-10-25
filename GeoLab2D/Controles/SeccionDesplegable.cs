using System.Windows.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    public sealed class SeccionDesplegable : UserControl
    {
        private readonly Button titulo;
        private readonly Control contenido;
        private readonly string texto;
        private bool expandida = true;
        public bool Expandida { get { return expandida; } }

        public SeccionDesplegable(string texto, Control contenido)
        {
            this.texto = texto; this.contenido = contenido;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Dock = DockStyle.Top;
            Margin = new Padding(0, 0, 0, 10);
            Padding = new Padding(8);
            titulo = new BotonRedondeado { Dock = DockStyle.Top, Height = 36, Tag = "tituloSeccion", TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 8) };
            contenido.Dock = DockStyle.Top;
            contenido.Margin = new Padding(0);
            var tabla = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(titulo, 0, 0); tabla.Controls.Add(contenido, 0, 1);
            Controls.Add(tabla);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            titulo.Click += (sender, e) => Alternar();
            TemaOscuro.Aplicar(this);
            ActualizarTitulo();
        }

        public void Alternar()
        {
            expandida = !expandida;
            contenido.Visible = expandida;
            ActualizarTitulo();
        }

        public void Mostrar(bool disponible)
        {
            Visible = disponible;
            contenido.Visible = expandida;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width <= 0 || Height <= 0) return;
            using (var forma = TemaOscuro.BordesRedondeados(new RectangleF(0, 0, Width, Height), 12))
            {
                Region anterior = Region;
                Region = new Region(forma);
                if (anterior != null) anterior.Dispose();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var forma = TemaOscuro.BordesRedondeados(new RectangleF(0.5f, 0.5f, Width - 1, Height - 1), 12))
            using (var borde = new Pen(Color.FromArgb(50, 65, 80))) e.Graphics.DrawPath(borde, forma);
        }

        private void ActualizarTitulo()
        {
            titulo.Text = (expandida ? "▾   " : "▸   ") + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower(CultureInfo.CurrentCulture));
        }
    }
}
