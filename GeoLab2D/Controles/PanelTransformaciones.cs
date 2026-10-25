using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
    public sealed class PanelTransformaciones : PanelContenidoAjustable
    {
        private readonly NumericUpDown tx, ty, angulo, sx, sy;
        private readonly Label matrizTexto, limites;
        private bool restableciendo;
        public event EventHandler TransformacionCambiada;

        public Matriz3x3 Matriz
        {
            get
            {
                return Matriz3x3.Traslacion((double)tx.Value, (double)ty.Value)
                    .Multiplicar(Matriz3x3.Rotacion((double)angulo.Value))
                    .Multiplicar(Matriz3x3.Escalamiento((double)sx.Value, (double)sy.Value));
            }
        }

        public PanelTransformaciones()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16, 4, 16, 8);
            var tabla = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 1, RowCount = 4 };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var entradas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = true };
            tx = Agregar(entradas, "Tx (px)", "traslacionX", -250, 250, 0, 1);
            ty = Agregar(entradas, "Ty (px)", "traslacionY", -250, 250, 0, 1);
            angulo = Agregar(entradas, "Ángulo (°)", "angulo", -360, 360, 0, 1);
            sx = Agregar(entradas, "Sx", "escalaX", 0.1m, 3, 1, 0.1m);
            sy = Agregar(entradas, "Sy", "escalaY", 0.1m, 3, 1, 0.1m);
            var restablecer = new BotonRedondeado { Name = "restablecerTransformacion", Text = "Restablecer", AutoSize = true };
            restablecer.Click += Restablecer_Click;
            entradas.Controls.Add(restablecer);
            matrizTexto = new TextoResultados { Name = "matrizTransformacion", Dock = DockStyle.Fill };
            limites = new TextoResultados { Name = "limitesTransformacion", Dock = DockStyle.Fill };
            tabla.Controls.Add(entradas, 0, 0);
            tabla.Controls.Add(new TextoResultados { Dock = DockStyle.Fill,
                Text = "[Orden de aplicación]\r\n1. Escalar con Sx y Sy.\r\n2. Rotar alrededor del origen O.\r\n3. Trasladar con Tx y Ty.\r\n\r\nM = T · R · S" }, 0, 1);
            tabla.Controls.Add(matrizTexto, 0, 2);
            tabla.Controls.Add(limites, 0, 3);
            UsarContenido(tabla);
            foreach (NumericUpDown entrada in new[] { tx, ty, angulo, sx, sy }) entrada.ValueChanged += Entrada_Cambiada;
            ActualizarMatriz();
        }

        private static NumericUpDown Agregar(FlowLayoutPanel fila, string texto, string nombre,
            decimal minimo, decimal maximo, decimal valor, decimal incremento)
        {
            var grupo = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false, Margin = new Padding(0, 0, 8, 0) };
            grupo.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(0, 4, 4, 0) });
            var entrada = new EntradaNumericaValidada { Name = nombre, Minimum = minimo, Maximum = maximo,
                Value = valor, DecimalPlaces = 2, Increment = incremento, Width = 80 };
            grupo.Controls.Add(entrada);
            fila.Controls.Add(grupo);
            return entrada;
        }

        public void MostrarLimites(bool fuera)
        {
            limites.Text = "[Visibilidad]\r\n" + (fuera ? "Parte de la figura queda fuera del lienzo.\r\nAjuste la escala o la traslación."
                : "Figura completa dentro del lienzo.");
        }

        public void Desplazar(float x, float y)
        {
            restableciendo = true;
            try
            {
                tx.Value = Math.Max(tx.Minimum, Math.Min(tx.Maximum, tx.Value + (decimal)x));
                ty.Value = Math.Max(ty.Minimum, Math.Min(ty.Maximum, ty.Value + (decimal)y));
            }
            finally { restableciendo = false; }
            Entrada_Cambiada(this, EventArgs.Empty);
        }

        private void ActualizarMatriz()
        {
            Matriz3x3 m = Matriz;
            matrizTexto.Text = string.Format(
                "[Matriz compuesta]\r\n[{0:0.000}; {1:0.000}; {2:0.00}]\r\n[{3:0.000}; {4:0.000}; {5:0.00}]\r\n[0; 0; 1]",
                m[0, 0], m[0, 1], m[0, 2], m[1, 0], m[1, 1], m[1, 2]);
        }

        private void Entrada_Cambiada(object sender, EventArgs e)
        {
            if (restableciendo) return;
            ActualizarMatriz();
            if (TransformacionCambiada != null) TransformacionCambiada(this, EventArgs.Empty);
        }

        private void Restablecer_Click(object sender, EventArgs e)
        {
            restableciendo = true;
            try { tx.Value = 0; ty.Value = 0; angulo.Value = 0; sx.Value = 1; sy.Value = 1; }
            finally { restableciendo = false; }
            Entrada_Cambiada(this, EventArgs.Empty);
        }
    }
}
