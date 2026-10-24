using System;
using System.Drawing;

namespace GeoLab2D.Geometria
{
    public enum Orientacion { Colineal, Antihoraria, Horaria }
    public enum UbicacionPunto { Fuera, Dentro, Borde }

    public sealed class ProyeccionSegmento
    {
        public PointF Punto { get; private set; }
        public double Parametro { get; private set; }
        public double Distancia { get; private set; }
        public bool SegmentoDegenerado { get; private set; }

        internal ProyeccionSegmento(PointF punto, double parametro, double distancia, bool degenerado)
        {
            Punto = punto; Parametro = parametro; Distancia = distancia; SegmentoDegenerado = degenerado;
        }
    }

    /// <summary>Algoritmos cartesianos reutilizables. No dependen de controles ni de Graphics.</summary>
    public static class GeometriaPlana
    {
        // Tolerancia de distancia en unidades del modelo (píxeles en la interfaz).
        public const double Tolerancia = 0.00001;

        private static void ValidarPunto(PointF punto)
        {
            if (float.IsNaN(punto.X) || float.IsInfinity(punto.X) ||
                float.IsNaN(punto.Y) || float.IsInfinity(punto.Y))
                throw new ArgumentException("Las coordenadas deben ser finitas.");
        }

        private static void ValidarPoligono(PointF[] vertices)
        {
            if (vertices == null) throw new ArgumentNullException("vertices");
            if (vertices.Length < 3) throw new ArgumentException("Se requieren al menos tres vértices.", "vertices");
            foreach (PointF punto in vertices) ValidarPunto(punto);
        }

        public static double Distancia(PointF a, PointF b)
        {
            ValidarPunto(a); ValidarPunto(b);
            double dx = (double)b.X - a.X, dy = (double)b.Y - a.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double AreaFirmada(PointF[] vertices)
        {
            ValidarPoligono(vertices);
            // Trasladar al primer vértice reduce la cancelación con coordenadas grandes.
            PointF origen = vertices[0];
            double dobleArea = 0;
            for (int i = 1; i < vertices.Length - 1; i++)
            {
                double ax = (double)vertices[i].X - origen.X, ay = (double)vertices[i].Y - origen.Y;
                double bx = (double)vertices[i + 1].X - origen.X, by = (double)vertices[i + 1].Y - origen.Y;
                dobleArea += ax * by - ay * bx;
            }
            return dobleArea / 2;
        }

        public static double Perimetro(PointF[] vertices)
        {
            ValidarPoligono(vertices);
            double total = 0;
            for (int i = 0; i < vertices.Length; i++)
                total += Distancia(vertices[i], vertices[(i + 1) % vertices.Length]);
            return total;
        }

        public static Orientacion Orientar(PointF a, PointF b, PointF p)
        {
            ValidarPunto(a); ValidarPunto(b); ValidarPunto(p);
            double dx = (double)b.X - a.X, dy = (double)b.Y - a.Y;
            double determinante = dx * ((double)p.Y - a.Y) - dy * ((double)p.X - a.X);
            double longitud = Math.Sqrt(dx * dx + dy * dy);
            if (Math.Abs(determinante) <= Tolerancia * longitud) return Orientacion.Colineal;
            return determinante > 0 ? Orientacion.Antihoraria : Orientacion.Horaria;
        }

        public static ProyeccionSegmento Proyectar(PointF a, PointF b, PointF p)
        {
            ValidarPunto(a); ValidarPunto(b); ValidarPunto(p);
            double dx = (double)b.X - a.X, dy = (double)b.Y - a.Y;
            double longitud2 = dx * dx + dy * dy;
            if (longitud2 == 0) return new ProyeccionSegmento(a, 0, Distancia(a, p), true);
            double t = (((double)p.X - a.X) * dx + ((double)p.Y - a.Y) * dy) / longitud2;
            t = Math.Max(0, Math.Min(1, t));
            double x = a.X + t * dx, y = a.Y + t * dy;
            double px = p.X - x, py = p.Y - y;
            return new ProyeccionSegmento(new PointF((float)x, (float)y), t, Math.Sqrt(px * px + py * py), false);
        }

        /// <summary>Regla par-impar para todos los contornos; separa explícitamente el borde.</summary>
        public static UbicacionPunto Ubicar(PointF[][] contornos, PointF p)
        {
            if (contornos == null) throw new ArgumentNullException("contornos");
            if (contornos.Length == 0) throw new ArgumentException("Se requiere al menos un contorno.", "contornos");
            ValidarPunto(p);
            bool dentro = false, borde = false;
            foreach (PointF[] vertices in contornos)
            {
                ValidarPoligono(vertices);
                for (int i = 0; i < vertices.Length; i++)
                {
                    PointF a = vertices[i], b = vertices[(i + 1) % vertices.Length];
                    if (Proyectar(a, b, p).Distancia <= Tolerancia) borde = true;
                    // Semiabierto en Y: un vértice no debe contarse dos veces.
                    if ((a.Y > p.Y) != (b.Y > p.Y))
                    {
                        double interseccionX = a.X + ((double)p.Y - a.Y) * ((double)b.X - a.X) / ((double)b.Y - a.Y);
                        if (p.X < interseccionX) dentro = !dentro;
                    }
                }
            }
            return borde ? UbicacionPunto.Borde : dentro ? UbicacionPunto.Dentro : UbicacionPunto.Fuera;
        }
    }
}
