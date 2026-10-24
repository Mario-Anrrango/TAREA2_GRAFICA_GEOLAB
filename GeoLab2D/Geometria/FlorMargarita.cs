using System;
using System.Drawing;

namespace GeoLab2D.Geometria
{
    /// <summary>Cinco pentágonos regulares inscritos en un pentágono mayor.</summary>
    public sealed class FlorMargarita
    {
        public float LadoExterior { get; private set; }
        public double LadoPetalo { get { return LadoExterior / (Phi * Phi); } }
        public double AreaPetalo { get { return 5 * LadoPetalo * LadoPetalo / (4 * Math.Tan(Math.PI / 5)); } }
        public double AreaTotal { get { return 5 * AreaPetalo; } }
        public double PerimetroPetalo { get { return 5 * LadoPetalo; } }
        private static readonly double Phi = (1 + Math.Sqrt(5)) / 2;

        public FlorMargarita(float ladoExterior)
        {
            if (float.IsNaN(ladoExterior) || float.IsInfinity(ladoExterior) || ladoExterior <= 0)
                throw new ArgumentOutOfRangeException("ladoExterior", "El lado exterior debe ser finito y mayor que cero.");
            LadoExterior = ladoExterior;
        }

        public PointF[][] ObtenerPetalos()
        {
            double radio = LadoPetalo / (2 * Math.Sin(Math.PI / 5));
            double apotema = radio * Math.Cos(Math.PI / 5);
            var petalos = new PointF[5][];
            for (int i = 0; i < 5; i++)
            {
                double direccion = Math.PI / 2 + i * 2 * Math.PI / 5;
                double centroX = 2 * apotema * Math.Cos(direccion);
                double centroY = 2 * apotema * Math.Sin(direccion);
                petalos[i] = new PointF[5];
                for (int j = 0; j < 5; j++)
                {
                    double angulo = direccion + j * 2 * Math.PI / 5;
                    petalos[i][j] = new PointF(
                        (float)(centroX + radio * Math.Cos(angulo)),
                        (float)(centroY + radio * Math.Sin(angulo)));
                }
            }
            return petalos;
        }
    }
}
