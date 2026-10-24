using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
    public sealed class PanelAnalisis : UserControl
    {
        private readonly ComboBox figura;
        private readonly NumericUpDown ax, ay, bx, by, px, py;
        private readonly Label resultados;
        public event EventHandler DatosCambiados;
        public int Figura { get { return figura.SelectedIndex; } }
        public PointF A { get { return new PointF((float)ax.Value, (float)ay.Value); } }
        public PointF B { get { return new PointF((float)bx.Value, (float)by.Value); } }
        public PointF P { get { return new PointF((float)px.Value, (float)py.Value); } }

        public PanelAnalisis()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16, 4, 16, 8);
            var tabla = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 4 };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            figura = new ComboBox { Name = "figuraAnalisis", DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
            figura.Items.AddRange(new object[] { "Analizar hexágono", "Analizar margarita", "Analizar estrella" });
            figura.SelectedIndex = 0;
            var entradas = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
            ax = Agregar(entradas, "Ax", -120); ay = Agregar(entradas, "Ay", -60);
            bx = Agregar(entradas, "Bx", 160); by = Agregar(entradas, "By", 40);
            px = Agregar(entradas, "Px", 30); py = Agregar(entradas, "Py", 100);
            resultados = new Label { Name = "resultadosAnalisis", AutoSize = true, Dock = DockStyle.Fill,
                Padding = new Padding(0, 4, 0, 4) };
            tabla.Controls.Add(figura, 0, 0);
            tabla.Controls.Add(entradas, 0, 1);
            tabla.Controls.Add(resultados, 0, 2);
            tabla.Controls.Add(new Label { AutoSize = true, Dock = DockStyle.Fill,
                Text = "AB: azul | P: naranja | Q más cercano: verde | Coordenadas cartesianas en px" }, 0, 3);
            Controls.Add(tabla);
            figura.SelectedIndexChanged += Entrada_Cambiada;
            foreach (NumericUpDown entrada in new[] { ax, ay, bx, by, px, py }) entrada.ValueChanged += Entrada_Cambiada;
        }

        private static NumericUpDown Agregar(FlowLayoutPanel fila, string nombre, decimal valor)
        {
            var grupo = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 8, 0) };
            grupo.Controls.Add(new Label { AutoSize = true, Text = nombre, Margin = new Padding(0, 4, 4, 0) });
            var entrada = new NumericUpDown { Name = "analisis" + nombre, Minimum = -250, Maximum = 250,
                Value = valor, DecimalPlaces = 2, Width = 75 };
            grupo.Controls.Add(entrada);
            fila.Controls.Add(grupo);
            return entrada;
        }

        public void MostrarResultados(PointF[][] contornos, bool esEstrella)
        {
            double area = 0, perimetro = 0;
            foreach (PointF[] contorno in contornos)
            {
                area += GeometriaPlana.AreaFirmada(contorno);
                perimetro += GeometriaPlana.Perimetro(contorno);
            }
            ProyeccionSegmento proyeccion = GeometriaPlana.Proyectar(A, B, P);
            string orientacion = A == B ? "indeterminada (A = B)" : GeometriaPlana.Orientar(A, B, P).ToString();
            resultados.Text = string.Format(
                "{0}: {1:0.00} px² | {2}: {3:0.00} px\r\nP respecto a la figura: {4} | Orientación (A, B, P): {5}\r\nQ = ({6:0.00}; {7:0.00}) | Distancia P–Q: {8:0.00} px | t = {9:0.000}{10}",
                esEstrella ? "Área algebraica (valor absoluto)" : "Área", Math.Abs(area),
                esEstrella ? "Longitud de trazos" : "Perímetro de contornos", perimetro,
                GeometriaPlana.Ubicar(contornos, P), orientacion,
                proyeccion.Punto.X, proyeccion.Punto.Y, proyeccion.Distancia, proyeccion.Parametro,
                proyeccion.SegmentoDegenerado ? " | AB reducido a un punto" : "");
        }

        private void Entrada_Cambiada(object sender, EventArgs e)
        {
            if (DatosCambiados != null) DatosCambiados(this, EventArgs.Empty);
        }
    }
}
