using System;
using System.Drawing;

namespace GeoLab2D.Geometria
{
    /// <summary>Matriz homogénea inmutable para vectores columna (x, y, 1).</summary>
    public sealed class Matriz3x3
    {
        private readonly double[,] valores;
        public double this[int fila, int columna] { get { return valores[fila, columna]; } }

        private Matriz3x3(double[,] datos)
        {
            foreach (double valor in datos) ValidarFinito(valor);
            valores = (double[,])datos.Clone();
        }

        private static void ValidarFinito(double valor)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                throw new ArgumentOutOfRangeException("valor", "La matriz requiere valores finitos.");
        }

        public static Matriz3x3 Identidad()
        {
            return new Matriz3x3(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
        }

        public static Matriz3x3 Traslacion(double x, double y)
        {
            return new Matriz3x3(new double[,] { { 1, 0, x }, { 0, 1, y }, { 0, 0, 1 } });
        }

        public static Matriz3x3 Rotacion(double grados)
        {
            ValidarFinito(grados);
            double radianes = (grados % 360) * Math.PI / 180;
            double c = Math.Cos(radianes), s = Math.Sin(radianes);
            return new Matriz3x3(new double[,] { { c, -s, 0 }, { s, c, 0 }, { 0, 0, 1 } });
        }

        public static Matriz3x3 Escalamiento(double x, double y)
        {
            return new Matriz3x3(new double[,] { { x, 0, 0 }, { 0, y, 0 }, { 0, 0, 1 } });
        }

        /// <summary>Devuelve this * derecha: la matriz de la derecha actúa primero.</summary>
        public Matriz3x3 Multiplicar(Matriz3x3 derecha)
        {
            if (derecha == null) throw new ArgumentNullException("derecha");
            var producto = new double[3, 3];
            for (int fila = 0; fila < 3; fila++)
            for (int columna = 0; columna < 3; columna++)
            for (int k = 0; k < 3; k++)
                producto[fila, columna] += valores[fila, k] * derecha[k, columna];
            return new Matriz3x3(producto);
        }

        public PointF Transformar(PointF punto)
        {
            ValidarFinito(punto.X); ValidarFinito(punto.Y);
            double x = valores[0, 0] * punto.X + valores[0, 1] * punto.Y + valores[0, 2];
            double y = valores[1, 0] * punto.X + valores[1, 1] * punto.Y + valores[1, 2];
            ValidarFinito(x); ValidarFinito(y);
            if (Math.Abs(x) > float.MaxValue || Math.Abs(y) > float.MaxValue)
                throw new OverflowException("El punto transformado no se puede representar con PointF.");
            return new PointF((float)x, (float)y);
        }

        public PointF[] Transformar(PointF[] puntos)
        {
            if (puntos == null) throw new ArgumentNullException("puntos");
            var resultado = new PointF[puntos.Length];
            for (int i = 0; i < puntos.Length; i++) resultado[i] = Transformar(puntos[i]);
            return resultado;
        }

        public PointF[][] Transformar(PointF[][] contornos)
        {
            if (contornos == null) throw new ArgumentNullException("contornos");
            var resultado = new PointF[contornos.Length][];
            for (int i = 0; i < contornos.Length; i++) resultado[i] = Transformar(contornos[i]);
            return resultado;
        }
    }
}
