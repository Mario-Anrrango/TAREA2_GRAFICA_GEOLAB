using System;
using System.Windows.Forms;
using System.Drawing;
using GeoLab2D.Geometria;

namespace GeoLab2D
{
    public partial class GeoLab2D : Form
    {
        private bool ajustandoHerramientas;

        public GeoLab2D()
        {
            InitializeComponent();
            ConfigurarEntradas(this);
            ayudas.SetToolTip(puntas, "De 5 a 64 puntas. El salto máximo depende de las puntas actuales.");
            ayudas.SetToolTip(salto, "Salto entre vértices: de 2 a la parte entera de (N - 1) / 2.");
            Resize += (sender, e) => AjustarHerramientas();
            tablaHerramientas.SizeChanged += (sender, e) => AjustarHerramientas();
            selectorFigura.SelectedIndex = 0;
        }

        private void ConfigurarEntradas(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                var entrada = control as Controles.EntradaNumericaValidada;
                if (entrada != null)
                {
                    ayudas.SetToolTip(entrada, string.Format("Rango: {0} a {1}. Decimales: {2}. Enter confirma el valor.",
                        entrada.Minimum, entrada.Maximum, entrada.DecimalPlaces));
                    entrada.ValidacionCambiada += (sender, e) =>
                        errores.SetError(entrada, entrada.ErrorEntrada ?? "");
                }
                ConfigurarEntradas(control);
            }
        }

        private void AjustarHerramientas()
        {
            if (ajustandoHerramientas) return;
            ajustandoHerramientas = true;
            try
            {
                int maximo = Math.Max(90, ClientSize.Height / 2 - encabezado.Height);
                contenedorHerramientas.Height = Math.Min(Math.Max(70, tablaHerramientas.PreferredSize.Height), maximo);
            }
            finally { ajustandoHerramientas = false; }
        }

        private void colores_Cambiados(object sender, EventArgs e)
        {
            lienzo.Paleta = panelColores.Paleta;
        }

        private void lado_ValueChanged(object sender, EventArgs e)
        {
            ActualizarFigura();
        }

        private void selectorFigura_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarFigura();
        }

        private void parametrosEstrella_ValueChanged(object sender, EventArgs e)
        {
            ActualizarFigura();
        }

        private void parametrosVectores_Cambiados(object sender, EventArgs e)
        {
            if (selectorFigura.SelectedIndex == 3) ActualizarFigura();
        }

        private void parametrosAnalisis_Cambiados(object sender, EventArgs e)
        {
            if (selectorFigura.SelectedIndex == 4) ActualizarFigura();
        }

        private void parametrosTransformacion_Cambiados(object sender, EventArgs e)
        {
            if (selectorFigura.SelectedIndex != 3) ActualizarFigura();
        }

        private PointF[][] ObtenerContornos(int tipo)
        {
            if (tipo == 0) return new[] { new HexagonoRegular((float)lado.Value).ObtenerVertices() };
            if (tipo == 1) return new FlorMargarita((float)lado.Value).ObtenerPetalos();
            return new PoligonoEstrellado((int)puntas.Value, (int)salto.Value, (float)lado.Value).ObtenerRecorridos();
        }

        private static double AreaContornos(PointF[][] contornos)
        {
            double area = 0;
            foreach (PointF[] contorno in contornos) area += GeometriaPlana.AreaFirmada(contorno);
            return Math.Abs(area);
        }

        private static double LongitudContornos(PointF[][] contornos)
        {
            double longitud = 0;
            foreach (PointF[] contorno in contornos) longitud += GeometriaPlana.Perimetro(contorno);
            return longitud;
        }

        private void ActualizarFigura()
        {
            if (selectorFigura.SelectedIndex < 0) return;
            bool esAnalisis = selectorFigura.SelectedIndex == 4;
            bool esEstrella = selectorFigura.SelectedIndex == 2 || (esAnalisis && panelAnalisis.Figura == 2);
            bool esVectores = selectorFigura.SelectedIndex == 3;
            panelVectores.Visible = esVectores;
            panelAnalisis.Visible = esAnalisis;
            panelTransformaciones.Visible = !esVectores;
            panelColores.Visible = selectorFigura.SelectedIndex <= 2;
            if (selectorFigura.SelectedIndex <= 2) panelColores.SeleccionarFigura(selectorFigura.SelectedIndex);
            lienzo.Paleta = panelColores.Paleta;
            lado.Enabled = !esVectores;
            etiquetaLado.Enabled = !esVectores;
            puntas.Enabled = esEstrella;
            salto.Enabled = esEstrella;
            etiquetaPuntas.Enabled = esEstrella;
            etiquetaSalto.Enabled = esEstrella;
            if (esEstrella)
            {
                int maximo = ((int)puntas.Value - 1) / 2;
                if (salto.Maximum != maximo) salto.Maximum = maximo;
            }
            Matriz3x3 matriz = panelTransformaciones.Matriz;
            lienzo.Transformacion = matriz;
            PointF[][] contornosActuales = null;
            if (!esVectores)
            {
                int tipo = esAnalisis ? panelAnalisis.Figura : selectorFigura.SelectedIndex;
                contornosActuales = matriz.Transformar(ObtenerContornos(tipo));
                bool fuera = false;
                foreach (PointF[] contorno in contornosActuales)
                foreach (PointF punto in contorno)
                    if (Math.Abs(punto.X) > lienzo.ClientSize.Width / 2f - 2 ||
                        Math.Abs(punto.Y) > lienzo.ClientSize.Height / 2f - 2) fuera = true;
                panelTransformaciones.MostrarLimites(fuera);
            }
            if (esAnalisis)
            {
                System.Drawing.PointF[][] contornos;
                if (panelAnalisis.Figura == 0)
                {
                    etiquetaLado.Text = "Lado del hexágono (px):";
                    contornos = contornosActuales;
                }
                else if (panelAnalisis.Figura == 1)
                {
                    etiquetaLado.Text = "Lado exterior (px):";
                    contornos = contornosActuales;
                }
                else
                {
                    etiquetaLado.Text = "Radio exterior (px):";
                    contornos = contornosActuales;
                }
                panelAnalisis.MostrarResultados(contornos, esEstrella);
                lienzo.MostrarAnalisis(contornos, panelAnalisis.A, panelAnalisis.B, panelAnalisis.P);
                estado.Text = esEstrella
                    ? "Pertenencia de estrella: regla par-impar | Área algebraica, no área de relleno\r\nEdite A, B y P para comprobar orientación y proyección"
                    : "Análisis de la figura seleccionada | Borde distinguido del interior\r\nEdite A, B y P para comprobar orientación y proyección";
            }
            else if (selectorFigura.SelectedIndex == 0)
            {
                etiquetaLado.Text = "Lado del hexágono (px):";
                var figura = new HexagonoRegular((float)lado.Value);
                lienzo.Hexagono = figura;
                estado.Text = string.Format(
                    "Lado base: {0:0.0} px | Perímetro actual: {1:0.00} px | Área actual: {2:0.00} px²\r\nLienzo mínimo: 800 × 600 | Hexágono de seis colores",
                    figura.Lado, LongitudContornos(contornosActuales), AreaContornos(contornosActuales));
            }
            else if (selectorFigura.SelectedIndex == 1)
            {
                etiquetaLado.Text = "Lado exterior (px):";
                var figura = new FlorMargarita((float)lado.Value);
                lienzo.Flor = figura;
                estado.Text = string.Format(
                    "Exterior base: {0:0.0} px | Pétalo base: {1:0.00} px | Área de pétalos actual: {2:0.00} px²\r\nMargarita: cinco pentágonos regulares de base | Centro sin relleno",
                    figura.LadoExterior, figura.LadoPetalo, AreaContornos(contornosActuales));
            }
            else if (esEstrella)
            {
                etiquetaLado.Text = "Radio exterior (px):";
                // Maximum ajusta también Value si el salto anterior ya no es válido.
                int maximoSalto = ((int)puntas.Value - 1) / 2;
                if (salto.Maximum != maximoSalto) salto.Maximum = maximoSalto;
                var figura = new PoligonoEstrellado((int)puntas.Value, (int)salto.Value, (float)lado.Value);
                lienzo.Estrella = figura;
                estado.Text = string.Format(
                    "{0} puntas | Salto: {1} | Trazos cerrados: {2} | Longitud actual: {3:0.00} px\r\nRadio base: {4:0.0} px | Polígono estrellado sin relleno",
                    figura.Puntas, figura.Salto, figura.Componentes, LongitudContornos(contornosActuales), figura.RadioExterior);
            }
            else
            {
                lienzo.MostrarVectores(panelVectores.VectorA, panelVectores.VectorB);
                estado.Text = "Vectores desde O(0, 0) | Eje X hacia la derecha, Y hacia arriba\r\nEdite A.x, A.y, B.x y B.y para comprobar las operaciones";
            }
            AjustarHerramientas();
        }

        private void encabezado_Click(object sender, EventArgs e)
        {

        }
    }
}
