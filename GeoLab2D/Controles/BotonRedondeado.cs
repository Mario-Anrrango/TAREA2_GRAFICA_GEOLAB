using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    /// <summary>Conserva eventos y accesibilidad de Button con dibujo adaptado al tema oscuro.</summary>
    public sealed class BotonRedondeado : Button
    {
        private bool encima;
        public BotonRedondeado()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Padding = new Padding(8, 5, 8, 5);
        }

        protected override void OnMouseEnter(EventArgs e) { encima = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { encima = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color fondo = encima && Enabled ? ControlPaint.Light(BackColor, 0.12f) : BackColor;
            using (var forma = TemaOscuro.BordesRedondeados(new RectangleF(0.5f, 0.5f, Width - 1, Height - 1), 8))
            using (var relleno = new SolidBrush(fondo))
            using (var borde = new Pen(Focused ? TemaOscuro.Acento : Color.FromArgb(70, 86, 103)))
            {
                e.Graphics.FillPath(relleno, forma);
                e.Graphics.DrawPath(borde, forma);
            }
            TextFormatFlags formato = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
            formato |= TextAlign == ContentAlignment.MiddleLeft ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter;
            Rectangle texto = new Rectangle(Padding.Left + 3, Padding.Top, Math.Max(1, Width - Padding.Horizontal - 6), Math.Max(1, Height - Padding.Vertical));
            TextRenderer.DrawText(e.Graphics, Text, Font, texto, Enabled ? ForeColor : TemaOscuro.Secundario, formato);
        }
    }
}
