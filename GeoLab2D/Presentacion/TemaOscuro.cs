using System.Drawing;
using System.Windows.Forms;

namespace GeoLab2D.Presentacion
{
    public static class TemaOscuro
    {
        public static readonly Color Fondo = Color.FromArgb(18, 23, 29);
        public static readonly Color Panel = Color.FromArgb(30, 37, 45);
        public static readonly Color Campo = Color.FromArgb(40, 49, 59);
        public static readonly Color Texto = Color.FromArgb(229, 236, 243);
        public static readonly Color Secundario = Color.FromArgb(151, 168, 185);
        public static readonly Color Acento = Color.FromArgb(39, 202, 170);
        public static readonly Color Lienzo = Color.FromArgb(24, 32, 40);

        public static void Aplicar(Control control)
        {
            Color previo = control.BackColor;
            control.ForeColor = Texto;
            control.BackColor = Panel;
            var numero = control as NumericUpDown;
            if (numero != null) { numero.BackColor = Campo; numero.BorderStyle = BorderStyle.FixedSingle; }
            var combo = control as ComboBox;
            if (combo != null) { combo.BackColor = Campo; combo.FlatStyle = FlatStyle.Flat; }
            var boton = control as Button;
            if (boton != null)
            {
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderColor = Color.FromArgb(64, 78, 91);
                boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 72, 83);
                boton.Cursor = Cursors.Hand;
                boton.Padding = new Padding(3);
                if (boton.Name == "cambiarColor")
                {
                    boton.BackColor = previo;
                    boton.ForeColor = previo.GetBrightness() < 0.5f ? Color.White : Color.Black;
                }
            }
            var etiqueta = control as Label;
            if (etiqueta != null && etiqueta.AutoSize && etiqueta.MaximumSize.Width == 0)
                etiqueta.MaximumSize = new Size(264, 0);
            foreach (Control hijo in control.Controls) Aplicar(hijo);
        }
    }
}
