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
            ActualizarHexagono();
        }

        private void lado_ValueChanged(object sender, EventArgs e)
        {
            ActualizarHexagono();
        }

        private void ActualizarHexagono()
        {
            var figura = new HexagonoRegular((float)lado.Value);
            lienzo.Hexagono = figura;
            estado.Text = string.Format(
                "Lado: {0:0.0} px | Perímetro: {1:0.00} px | Área: {2:0.00} px²\r\nLienzo mínimo: 800 × 600 | Etapa 2: hexágono regular de seis colores",
                figura.Lado, figura.Perimetro, figura.Area);
        }

        private void encabezado_Click(object sender, EventArgs e)
        {

        }
    }
}
