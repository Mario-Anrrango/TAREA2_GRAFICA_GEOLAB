using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    /// <summary>Texto accesible con encabezados entre corchetes y altura medida por su ancho real.</summary>
    public sealed class TextoResultados : Label
    {
        public TextoResultados()
        {
            AutoSize = true;
            Padding = new Padding(4, 6, 4, 8);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private int DibujarOMedir(Graphics graphics, int ancho)
        {
            int y = Padding.Top;
            int disponible = Math.Max(30, ancho - Padding.Horizontal);
            var formato = TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
            foreach (string linea in Text.Replace("\r", "").Split('\n'))
            {
                if (linea.Length == 0) { y += 10; continue; }
                bool titulo = linea.Length > 2 && linea.StartsWith("[") && linea.EndsWith("]") && char.IsLetter(linea[1]);
                string texto = titulo ? linea.Substring(1, linea.Length - 2) : linea;
                Font fuente = titulo ? TemaOscuro.FuenteTitulo : Font;
                if (titulo && y > Padding.Top) y += 6;
                Size medida = TextRenderer.MeasureText(texto, fuente, new Size(disponible, int.MaxValue), formato);
                if (graphics != null)
                    TextRenderer.DrawText(graphics, texto, fuente, new Rectangle(Padding.Left, y, disponible, medida.Height),
                        titulo ? TemaOscuro.Acento : ForeColor, formato);
                y += medida.Height + (titulo ? 7 : 5);
            }
            return y + Padding.Bottom;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Las tablas pueden consultar primero con ancho 1 al medir columnas porcentuales.
            // En esa pasada se usa el ancho ya asignado al bloque, no una columna de un píxel.
            int ancho = proposedSize.Width >= 60 ? proposedSize.Width : MaximumSize.Width > 0 ? MaximumSize.Width : Math.Max(180, Width);
            if (MaximumSize.Width > 0) ancho = Math.Min(ancho, MaximumSize.Width);
            return new Size(ancho, DibujarOMedir(null, ancho));
        }

        protected override void OnPaint(PaintEventArgs e) { DibujarOMedir(e.Graphics, ClientSize.Width); }
    }
}
