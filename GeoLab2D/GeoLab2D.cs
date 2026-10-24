using System;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D
{
    public partial class GeoLab2D : Form
    {
        public GeoLab2D()
        {
            InitializeComponent();
            selectorFigura.SelectedIndex = 0;
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

        private void ActualizarFigura()
        {
            if (selectorFigura.SelectedIndex < 0) return;
            bool esEstrella = selectorFigura.SelectedIndex == 2;
            bool esVectores = selectorFigura.SelectedIndex == 3;
            panelVectores.Visible = esVectores;
            lado.Enabled = !esVectores;
            etiquetaLado.Enabled = !esVectores;
            puntas.Enabled = esEstrella;
            salto.Enabled = esEstrella;
            etiquetaPuntas.Enabled = esEstrella;
            etiquetaSalto.Enabled = esEstrella;
            if (selectorFigura.SelectedIndex == 0)
            {
                etiquetaLado.Text = "Lado del hexágono (px):";
                var figura = new HexagonoRegular((float)lado.Value);
                lienzo.Hexagono = figura;
                estado.Text = string.Format(
                    "Lado: {0:0.0} px | Perímetro: {1:0.00} px | Área: {2:0.00} px²\r\nLienzo mínimo: 800 × 600 | Hexágono de seis colores",
                    figura.Lado, figura.Perimetro, figura.Area);
            }
            else if (selectorFigura.SelectedIndex == 1)
            {
                etiquetaLado.Text = "Lado exterior (px):";
                var figura = new FlorMargarita((float)lado.Value);
                lienzo.Flor = figura;
                estado.Text = string.Format(
                    "Exterior: {0:0.0} px | Lado de pétalo: {1:0.00} px | Área roja: {2:0.00} px²\r\nMargarita: cinco pentágonos regulares | Centro sin relleno",
                    figura.LadoExterior, figura.LadoPetalo, figura.AreaTotal);
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
                    "{0} puntas | Salto: {1} | Trazos cerrados: {2} | Longitud total: {3:0.00} px\r\nRadio exterior: {4:0.0} px | Polígono estrellado sin relleno",
                    figura.Puntas, figura.Salto, figura.Componentes, figura.LongitudTotal, figura.RadioExterior);
            }
            else
            {
                lienzo.MostrarVectores(panelVectores.VectorA, panelVectores.VectorB);
                estado.Text = "Vectores desde O(0, 0) | Eje X hacia la derecha, Y hacia arriba\r\nEdite A.x, A.y, B.x y B.y para comprobar las operaciones";
            }
        }

        private void encabezado_Click(object sender, EventArgs e)
        {

        }
    }
}
