using System;
using System.Drawing;
using System.Windows.Forms;

namespace GeoLab2D.Controles
{
    /// <summary>Propaga a la tarjeta la altura completa del contenido, incluidos textos que cambian.</summary>
    public class PanelContenidoAjustable : UserControl
    {
        private TableLayoutPanel contenido;
        private bool ajustando;
        private Size ultimaMedida = new Size(260, 100);

        protected void UsarContenido(TableLayoutPanel tabla)
        {
            contenido = tabla;
            tabla.Dock = DockStyle.None;
            tabla.AutoSize = false;
            tabla.Margin = new Padding(0);
            tabla.RowStyles.Clear();
            for (int i = 0; i < tabla.RowCount; i++) tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(tabla);
            tabla.Layout += (sender, e) => AjustarAltura();
            AjustarAltura();
        }

        private Size Medir(int ancho)
        {
            if (contenido == null || ajustando) return ultimaMedida;
            ajustando = true;
            try
            {
                int interior = Math.Max(120, ancho - Padding.Horizontal);
                contenido.Location = new Point(Padding.Left, Padding.Top);
                contenido.Width = interior;
                foreach (Control control in contenido.Controls)
                {
                    var etiqueta = control as Label;
                    if (etiqueta != null) etiqueta.MaximumSize = new Size(Math.Max(30, interior - etiqueta.Margin.Horizontal), 0);
                }
                Size medida = contenido.GetPreferredSize(new Size(interior, 0));
                contenido.Height = medida.Height;
                ultimaMedida = new Size(ancho, medida.Height + Padding.Vertical);
                return ultimaMedida;
            }
            finally { ajustando = false; }
        }

        private void AjustarAltura()
        {
            if (contenido == null || ajustando) return;
            Size medida = Medir(Math.Max(160, Width));
            if (Height != medida.Height) Height = medida.Height;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Medir no debe cambiar controles desde una consulta del contenedor:
            // eso puede provocar ciclos al combinar varias tablas AutoSize.
            return new Size(proposedSize.Width > 0 ? proposedSize.Width : Math.Max(160, Width), ultimaMedida.Height);
        }

        protected override void OnLayout(LayoutEventArgs e) { base.OnLayout(e); AjustarAltura(); }
    }
}
