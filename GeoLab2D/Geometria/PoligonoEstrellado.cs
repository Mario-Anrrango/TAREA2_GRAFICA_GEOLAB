using System;
using System.Drawing;

namespace GeoLab2D.Geometria
{
    /// <summary>Recorre N vértices equidistantes con un salto K, incluyendo composiciones.</summary>
    public sealed class PoligonoEstrellado
    {
        public int Puntas { get; private set; }
        public int Salto { get; private set; }
        public float RadioExterior { get; private set; }
        public int Componentes
        {
            get
            {
                int a = Puntas, b = Salto;
                while (b != 0) { int resto = a % b; a = b; b = resto; }
                return a;
            }
        }
        // Suma de las longitudes de todos los segmentos, no perímetro de una región rellena.
        public double LongitudTotal { get { return 2.0 * Puntas * RadioExterior * Math.Sin(Math.PI * Salto / Puntas); } }

        public PoligonoEstrellado(int puntas, int salto, float radioExterior)
        {
            if (puntas < 5 || puntas > 128)
                throw new ArgumentOutOfRangeException("puntas", "El número de puntas debe estar entre 5 y 128.");
            if (salto < 2 || salto > (puntas - 1) / 2)
                throw new ArgumentOutOfRangeException("salto", "El salto debe estar entre 2 y la parte entera de (N - 1) / 2.");
            if (float.IsNaN(radioExterior) || float.IsInfinity(radioExterior) || radioExterior <= 0)
                throw new ArgumentOutOfRangeException("radioExterior", "El radio debe ser finito y mayor que cero.");
            Puntas = puntas;
            Salto = salto;
            RadioExterior = radioExterior;
        }

        public PointF[] ObtenerVertices()
        {
            var vertices = new PointF[Puntas];
            for (int i = 0; i < Puntas; i++)
            {
                double angulo = Math.PI / 2 + i * 2 * Math.PI / Puntas;
                vertices[i] = new PointF((float)(RadioExterior * Math.Cos(angulo)),
                    (float)(RadioExterior * Math.Sin(angulo)));
            }
            return vertices;
        }

        public PointF[][] ObtenerRecorridos()
        {
            PointF[] vertices = ObtenerVertices();
            int componentes = Componentes;
            int longitud = Puntas / componentes;
            var recorridos = new PointF[componentes][];
            for (int inicio = 0; inicio < componentes; inicio++)
            {
                recorridos[inicio] = new PointF[longitud];
                for (int j = 0; j < longitud; j++)
                    recorridos[inicio][j] = vertices[(inicio + j * Salto) % Puntas];
            }
            return recorridos;
        }
    }
}
