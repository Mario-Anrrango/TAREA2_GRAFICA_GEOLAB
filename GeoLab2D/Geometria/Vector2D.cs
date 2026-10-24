using System;

namespace GeoLab2D.Geometria
{
    /// <summary>Vector cartesiano inmutable, independiente de la interfaz gráfica.</summary>
    public sealed class Vector2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Magnitud { get { return Math.Sqrt(X * X + Y * Y); } }
        public double? DireccionGrados
        {
            get
            {
                if (X == 0 && Y == 0) return null;
                double grados = Math.Atan2(Y, X) * 180 / Math.PI;
                return grados < 0 ? grados + 360 : grados;
            }
        }

        public Vector2D(float x, float y)
        {
            if (float.IsNaN(x) || float.IsInfinity(x))
                throw new ArgumentOutOfRangeException("x", "La componente X debe ser finita.");
            if (float.IsNaN(y) || float.IsInfinity(y))
                throw new ArgumentOutOfRangeException("y", "La componente Y debe ser finita.");
            // Operaciones en double para evitar desbordamientos al multiplicar valores float.
            X = x;
            Y = y;
        }

        public double ProductoPunto(Vector2D otro)
        {
            if (otro == null) throw new ArgumentNullException("otro");
            return X * otro.X + Y * otro.Y;
        }

        public double Determinante(Vector2D otro)
        {
            if (otro == null) throw new ArgumentNullException("otro");
            return X * otro.Y - Y * otro.X;
        }
    }
}
