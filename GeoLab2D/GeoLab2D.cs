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

        private void ActualizarFigura()
        {
            if (selectorFigura.SelectedIndex < 0) return;
            if (selectorFigura.SelectedIndex == 0)
            {
                etiquetaLado.Text = "Lado del hexágono (px):";
                var figura = new HexagonoRegular((float)lado.Value);
                lienzo.Hexagono = figura;
                estado.Text = string.Format(
                    "Lado: {0:0.0} px | Perímetro: {1:0.00} px | Área: {2:0.00} px²\r\nLienzo mínimo: 800 × 600 | Hexágono de seis colores",
                    figura.Lado, figura.Perimetro, figura.Area);
            }
            else
            {
                etiquetaLado.Text = "Lado exterior (px):";
                var figura = new FlorMargarita((float)lado.Value);
                lienzo.Flor = figura;
                estado.Text = string.Format(
                    "Exterior: {0:0.0} px | Lado de pétalo: {1:0.00} px | Área roja: {2:0.00} px²\r\nMargarita: cinco pentágonos regulares | Centro sin relleno",
                    figura.LadoExterior, figura.LadoPetalo, figura.AreaTotal);
            }
        }

        private void encabezado_Click(object sender, EventArgs e)
        {

        }
    }
}
