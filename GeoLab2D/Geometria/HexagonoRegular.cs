using System;
using System.Drawing;

namespace GeoLab2D.Geometria
{
    /// <summary>Hexágono centrado en el origen, en coordenadas cartesianas.</summary>
    public sealed class HexagonoRegular
    {
        public float Lado { get; private set; }

        public double Perimetro { get { return 6.0 * Lado; } }
        public double Area { get { return 3.0 * Math.Sqrt(3.0) * Lado * Lado / 2.0; } }

        public HexagonoRegular(float lado)
        {
            if (float.IsNaN(lado) || float.IsInfinity(lado) || lado <= 0)
                throw new ArgumentOutOfRangeException("lado", "El lado debe ser un número finito mayor que cero.");

            Lado = lado;
        }

        public PointF[] ObtenerVertices()
        {
            var vertices = new PointF[6];
            // En un hexágono regular el radio circunscrito coincide con el lado.
            for (int i = 0; i < vertices.Length; i++)
            {
                double angulo = i * Math.PI / 3.0;
                vertices[i] = new PointF(
                    (float)(Lado * Math.Cos(angulo)),
                    (float)(Lado * Math.Sin(angulo)));
            }
            return vertices;
        }
    }
}
