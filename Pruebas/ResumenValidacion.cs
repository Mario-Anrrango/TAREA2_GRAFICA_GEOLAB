using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using GeoLab2D.Geometria;

internal static class ResumenValidacion
{
    public static void Guardar()
    {
        var filas = new List<string> { "Caso;Esperado;Obtenido;Tolerancia;Resultado" };
        var hexagono = new HexagonoRegular(10);
        Agregar(filas, "Hexagono L10 perimetro", 60, hexagono.Perimetro);
        Agregar(filas, "Hexagono L10 area", 259.807621135332, hexagono.Area);
        var flor = new FlorMargarita(10);
        Agregar(filas, "Margarita L10 lado petalo", 3.81966011250105, flor.LadoPetalo);
        Agregar(filas, "Margarita L10 area total", 125.507134928892, flor.AreaTotal);
        Agregar(filas, "Estrella N5 K2 R10 longitud", 95.1056516295154, new PoligonoEstrellado(5, 2, 10).LongitudTotal);
        Agregar(filas, "Estrella N10 K4 componentes", 2, new PoligonoEstrellado(10, 4, 10).Componentes);
        var a = new Vector2D(3, 4); var b = new Vector2D(-4, 3);
        Agregar(filas, "Vector A3 4 magnitud", 5, a.Magnitud);
        Agregar(filas, "Vector A3 4 direccion", 53.130102354156, a.DireccionGrados.Value);
        Agregar(filas, "Producto punto perpendiculares", 0, a.ProductoPunto(b));
        Agregar(filas, "Determinante A3 4 B-4 3", 25, a.Determinante(b));
        var q = GeometriaPlana.Proyectar(PointF.Empty, new PointF(10, 0), new PointF(3, 4));
        Agregar(filas, "Proyeccion Qx", 3, q.Punto.X);
        Agregar(filas, "Proyeccion Qy", 0, q.Punto.Y);
        Agregar(filas, "Proyeccion distancia", 4, q.Distancia);
        Agregar(filas, "Proyeccion parametro", 0.3, q.Parametro);
        var matriz = Matriz3x3.Traslacion(5, -1).Multiplicar(Matriz3x3.Rotacion(90))
            .Multiplicar(Matriz3x3.Escalamiento(2, 3));
        PointF transformado = matriz.Transformar(new PointF(1, 2));
        Agregar(filas, "Composicion TRS x", -1, transformado.X);
        Agregar(filas, "Composicion TRS y", 1, transformado.Y);
        File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "casos-referencia.csv"), filas, new UTF8Encoding(true));
        Console.WriteLine("Correcto: 16 casos de referencia exportados a CSV.");
    }

    private static void Agregar(List<string> filas, string nombre, double esperado, double obtenido)
    {
        const double tolerancia = 0.000001;
        bool correcto = !double.IsNaN(obtenido) && Math.Abs(esperado - obtenido) <= tolerancia;
        if (!correcto) throw new Exception("Fallo de referencia: " + nombre);
        filas.Add(string.Join(";", nombre, esperado.ToString("G17", CultureInfo.InvariantCulture),
            obtenido.ToString("G17", CultureInfo.InvariantCulture), tolerancia.ToString("G", CultureInfo.InvariantCulture), "APROBADO"));
    }
}
