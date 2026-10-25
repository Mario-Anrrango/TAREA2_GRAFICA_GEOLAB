using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
    /// <summary>Entrada y resultados de dos vectores; la aritmética pertenece a Vector2D.</summary>
    public sealed class PanelVectores : PanelContenidoAjustable
    {
        private readonly NumericUpDown ax, ay, bx, by;
        private readonly Label resultados;
        public event EventHandler VectoresCambiados;

        public Vector2D VectorA { get { return new Vector2D((float)ax.Value, (float)ay.Value); } }
        public Vector2D VectorB { get { return new Vector2D((float)bx.Value, (float)by.Value); } }
        private bool moviendo;

        public void MoverVector(string nombre, PointF punto)
        {
            NumericUpDown x = nombre == "A" ? ax : bx, y = nombre == "A" ? ay : by;
            moviendo = true;
            try
            {
                x.Value = Math.Max(x.Minimum, Math.Min(x.Maximum, (decimal)punto.X));
                y.Value = Math.Max(y.Minimum, Math.Min(y.Maximum, (decimal)punto.Y));
            }
            finally { moviendo = false; }
            Entrada_ValueChanged(this, EventArgs.Empty);
        }

        public PanelVectores()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16, 4, 16, 8);
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 1, RowCount = 3
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var entradas = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = true };
            ax = CrearEntrada("ax", 120);
            ay = CrearEntrada("ay", 80);
            bx = CrearEntrada("bx", -60);
            by = CrearEntrada("by", 140);
            AgregarComponente(entradas, "A.x", ax);
            AgregarComponente(entradas, "A.y", ay);
            AgregarComponente(entradas, "B.x", bx);
            AgregarComponente(entradas, "B.y", by);
            resultados = new TextoResultados { Name = "resultadosVectores", Dock = DockStyle.Fill };
            var ayuda = new TextoResultados { Dock = DockStyle.Fill,
                Text = "[Cómo interpretar]\r\nA: flecha azul.\r\nB: flecha naranja.\r\n\r\nLa dirección se mide desde +X, en sentido antihorario.\r\nLas componentes se expresan en píxeles." };
            tabla.Controls.Add(entradas, 0, 0);
            tabla.Controls.Add(resultados, 0, 1);
            tabla.Controls.Add(ayuda, 0, 2);
            UsarContenido(tabla);
            foreach (NumericUpDown entrada in new[] { ax, ay, bx, by })
                entrada.ValueChanged += Entrada_ValueChanged;
            ActualizarResultados();
        }

        private static NumericUpDown CrearEntrada(string nombre, decimal valor)
        {
            return new EntradaNumericaValidada { Name = nombre, Minimum = -250, Maximum = 250,
                DecimalPlaces = 2, Increment = 1, Value = valor, Width = 85 };
        }

        private static void AgregarComponente(FlowLayoutPanel fila, string texto, NumericUpDown entrada)
        {
            var grupo = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false, Margin = new Padding(0, 0, 10, 0) };
            grupo.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(0, 4, 4, 0) });
            grupo.Controls.Add(entrada);
            fila.Controls.Add(grupo);
        }

        private static string Direccion(Vector2D vector)
        {
            return vector.DireccionGrados.HasValue
                ? string.Format("{0:0.00}°", vector.DireccionGrados.Value)
                : "no definida (vector cero)";
        }

        private void ActualizarResultados()
        {
            Vector2D a = VectorA, b = VectorB;
            resultados.Text = string.Format(
                "[Vector A]\r\n|A| = {0:0.00} px\r\nDirección A = {1}\r\n\r\n[Vector B]\r\n|B| = {2:0.00} px\r\nDirección B = {3}\r\n\r\n[Operaciones entre A y B]\r\nA · B = {4:0.00} px²\r\ndet(A, B) = {5:0.00} px²",
                a.Magnitud, Direccion(a), b.Magnitud, Direccion(b), a.ProductoPunto(b), a.Determinante(b));
        }

        private void Entrada_ValueChanged(object sender, EventArgs e)
        {
            if (moviendo) return;
            ActualizarResultados();
            if (VectoresCambiados != null) VectoresCambiados(this, EventArgs.Empty);
        }
    }
}
