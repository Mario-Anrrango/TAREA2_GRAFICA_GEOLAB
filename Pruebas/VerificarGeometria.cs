using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarGeometria
{
    private static void Cerca(double actual, double esperado, string caso, double tolerancia = 0.0001)
    {
        if (double.IsNaN(actual) || Math.Abs(actual - esperado) > tolerancia)
            throw new Exception(caso + ": esperado " + esperado + ", recibido " + actual);
    }

    private static void Ubicacion(PointF[][] contornos, PointF p, UbicacionPunto esperado)
    {
        if (GeometriaPlana.Ubicar(contornos, p) != esperado) throw new Exception("Pertenencia incorrecta en " + p);
    }

    private static void Rechaza(Action accion)
    {
        try { accion(); }
        catch (ArgumentException) { return; }
        throw new Exception("Entrada geométrica inválida aceptada.");
    }

    private static void Proyeccion(PointF a, PointF b, PointF p, PointF esperado, double distancia, double t)
    {
        ProyeccionSegmento resultado = GeometriaPlana.Proyectar(a, b, p);
        Cerca(resultado.Punto.X, esperado.X, "Proyección X");
        Cerca(resultado.Punto.Y, esperado.Y, "Proyección Y");
        Cerca(resultado.Distancia, distancia, "Distancia mínima");
        Cerca(resultado.Parametro, t, "Parámetro limitado");
        ProyeccionSegmento invertido = GeometriaPlana.Proyectar(b, a, p);
        Cerca(invertido.Punto.X, esperado.X, "Proyección con AB invertido X");
        Cerca(invertido.Punto.Y, esperado.Y, "Proyección con AB invertido Y");
    }

    public static void Ejecutar()
    {
        PointF[] cuadrado = { new PointF(0, 0), new PointF(10, 0), new PointF(10, 10), new PointF(0, 10) };
        Cerca(GeometriaPlana.AreaFirmada(cuadrado), 100, "Área de cuadrado");
        Cerca(GeometriaPlana.Perimetro(cuadrado), 40, "Perímetro de cuadrado");
        var reverso = (PointF[])cuadrado.Clone(); Array.Reverse(reverso);
        Cerca(GeometriaPlana.AreaFirmada(reverso), -100, "Área con orientación inversa");
        Cerca(GeometriaPlana.Perimetro(reverso), 40, "Perímetro con orientación inversa");
        PointF[] trasladado = { new PointF(10000, -10000), new PointF(10010, -10000),
            new PointF(10010, -9990), new PointF(10000, -9990) };
        Cerca(GeometriaPlana.AreaFirmada(trasladado), 100, "Área invariante bajo traslación");
        var contornos = new[] { cuadrado };
        Ubicacion(contornos, new PointF(5, 5), UbicacionPunto.Dentro);
        Ubicacion(contornos, new PointF(15, 5), UbicacionPunto.Fuera);
        Ubicacion(contornos, new PointF(10, 5), UbicacionPunto.Borde);
        Ubicacion(contornos, new PointF(0, 0), UbicacionPunto.Borde);
        Ubicacion(new[] { reverso }, new PointF(5, 5), UbicacionPunto.Dentro);
        PointF[] interior = { new PointF(3, 3), new PointF(7, 3), new PointF(7, 7), new PointF(3, 7) };
        Ubicacion(new[] { cuadrado, interior }, new PointF(5, 5), UbicacionPunto.Fuera);
        Ubicacion(new[] { cuadrado, interior }, new PointF(2, 5), UbicacionPunto.Dentro);
        Ubicacion(new[] { cuadrado, interior }, new PointF(3, 5), UbicacionPunto.Borde);
        PointF[] concavo = { new PointF(0, 0), new PointF(4, 0), new PointF(4, 1),
            new PointF(1, 1), new PointF(1, 4), new PointF(0, 4) };
        Cerca(GeometriaPlana.AreaFirmada(concavo), 7, "Área de polígono cóncavo");
        Cerca(GeometriaPlana.Perimetro(concavo), 16, "Perímetro de polígono cóncavo");
        Ubicacion(new[] { concavo }, new PointF(2, 2), UbicacionPunto.Fuera);
        Ubicacion(new[] { concavo }, new PointF(0.5f, 3), UbicacionPunto.Dentro);
        PointF[] cruzado = { new PointF(0, 0), new PointF(4, 4), new PointF(0, 4), new PointF(4, 0) };
        Cerca(GeometriaPlana.AreaFirmada(cruzado), 0, "Área algebraica con cancelación");
        Ubicacion(new[] { cruzado }, new PointF(2, 2), UbicacionPunto.Borde);
        Ubicacion(new[] { cruzado }, new PointF(2, 3.5f), UbicacionPunto.Dentro);
        var flor = new FlorMargarita(150);
        Ubicacion(flor.ObtenerPetalos(), PointF.Empty, UbicacionPunto.Fuera);
        double areaFlor = 0;
        foreach (PointF[] petalo in flor.ObtenerPetalos()) areaFlor += GeometriaPlana.AreaFirmada(petalo);
        Cerca(areaFlor, flor.AreaTotal, "Área de los cinco pétalos", 0.02);
        var estrella = new PoligonoEstrellado(5, 2, 10);
        Ubicacion(estrella.ObtenerRecorridos(), PointF.Empty, UbicacionPunto.Fuera);
        Cerca(GeometriaPlana.AreaFirmada(estrella.ObtenerRecorridos()[0]),
            146.946313073118, "Área algebraica de pentagrama");

        PointF origen = PointF.Empty, derecha = new PointF(10, 0);
        if (GeometriaPlana.Orientar(origen, derecha, new PointF(3, 4)) != Orientacion.Antihoraria ||
            GeometriaPlana.Orientar(origen, derecha, new PointF(3, -4)) != Orientacion.Horaria ||
            GeometriaPlana.Orientar(origen, derecha, new PointF(20, 0)) != Orientacion.Colineal)
            throw new Exception("Orientación incorrecta.");
        if (GeometriaPlana.Orientar(origen, derecha, new PointF(3, 0.000001f)) != Orientacion.Colineal)
            throw new Exception("Tolerancia de colinealidad incorrecta.");
        Proyeccion(origen, derecha, new PointF(3, 4), new PointF(3, 0), 4, 0.3);
        Proyeccion(origen, derecha, new PointF(-3, 4), origen, 5, 0);
        Proyeccion(origen, derecha, new PointF(13, 4), derecha, 5, 1);
        Proyeccion(new PointF(2, -2), new PointF(2, 8), new PointF(-2, 3), new PointF(2, 3), 4, 0.5);
        Proyeccion(origen, new PointF(10, 10), new PointF(10, 0), new PointF(5, 5), Math.Sqrt(50), 0.5);
        Proyeccion(new PointF(1, 2), new PointF(1, 2), new PointF(4, 6), new PointF(1, 2), 5, 0);
        if (!GeometriaPlana.Proyectar(origen, origen, derecha).SegmentoDegenerado)
            throw new Exception("Segmento degenerado no detectado.");
        Rechaza(() => GeometriaPlana.AreaFirmada(null));
        Rechaza(() => GeometriaPlana.Perimetro(new PointF[2]));
        Rechaza(() => GeometriaPlana.Ubicar(null, origen));
        Rechaza(() => GeometriaPlana.Ubicar(new PointF[0][], origen));
        Rechaza(() => GeometriaPlana.Ubicar(new[] { cuadrado, (PointF[])null }, origen));
        Rechaza(() => GeometriaPlana.Proyectar(origen, derecha, new PointF(float.NaN, 0)));
        Rechaza(() => GeometriaPlana.Orientar(origen, new PointF(0, float.PositiveInfinity), derecha));

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var lado = (NumericUpDown)formulario.Controls.Find("lado", true)[0];
            var puntas = (NumericUpDown)formulario.Controls.Find("puntas", true)[0];
            var salto = (NumericUpDown)formulario.Controls.Find("salto", true)[0];
            var fuente = (ComboBox)formulario.Controls.Find("figuraAnalisis", true)[0];
            var resultados = (Label)formulario.Controls.Find("resultadosAnalisis", true)[0];
            var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            var ax = (NumericUpDown)formulario.Controls.Find("analisisAx", true)[0];
            var ay = (NumericUpDown)formulario.Controls.Find("analisisAy", true)[0];
            var bx = (NumericUpDown)formulario.Controls.Find("analisisBx", true)[0];
            var by = (NumericUpDown)formulario.Controls.Find("analisisBy", true)[0];
            var px = (NumericUpDown)formulario.Controls.Find("analisisPx", true)[0];
            var py = (NumericUpDown)formulario.Controls.Find("analisisPy", true)[0];
            selector.SelectedIndex = 4;
            lado.Value = 150;
            ax.Value = -100; ay.Value = 0; bx.Value = 100; by.Value = 0; px.Value = 30; py.Value = 80;
            if (!resultados.Text.Contains("Dentro") || !resultados.Text.Contains("Antihoraria") ||
                !resultados.Text.Contains(string.Format("Distancia P–Q: {0:0.00}", 80)))
                throw new Exception("Resultados de análisis no actualizados.");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                int x = lienzo.Width / 2, y = lienzo.Height / 2;
                if (imagen.GetPixel(x + 30, y).ToArgb() != Color.ForestGreen.ToArgb() ||
                    imagen.GetPixel(x + 30, y - 80).ToArgb() != Color.DarkOrange.ToArgb())
                    throw new Exception("Marcadores de proyección incorrectos.");
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "analisis-verificado.png"));
            }
            px.Value = 150; py.Value = 0;
            if (!resultados.Text.Contains("Borde")) throw new Exception("Borde no mostrado.");
            px.Value = 250; py.Value = 250;
            if (!resultados.Text.Contains("Fuera")) throw new Exception("Exterior no mostrado.");
            bx.Value = ax.Value;
            if (!resultados.Text.Contains("AB reducido a un punto") || !resultados.Text.Contains("indeterminada"))
                throw new Exception("Segmento degenerado no presentado.");
            fuente.SelectedIndex = 1; px.Value = 0; py.Value = 0;
            if (!resultados.Text.Contains("Fuera")) throw new Exception("Hueco de margarita mal clasificado.");
            fuente.SelectedIndex = 2; puntas.Value = 5; salto.Value = 2;
            if (!puntas.Enabled || !resultados.Text.Contains("Área algebraica") || !resultados.Text.Contains("Fuera"))
                throw new Exception("Análisis de estrella incorrecto.");
            puntas.Value = 64;
            selector.SelectedIndex = 0;
            if (lienzo.Hexagono == null || !lado.Enabled) throw new Exception("Regreso a figuras incorrecto.");
            selector.SelectedIndex = 3;
            if (lado.Enabled) throw new Exception("Modo de vectores alterado.");
            selector.SelectedIndex = 4;
            if (fuente.SelectedIndex != 2 || puntas.Value != 64) throw new Exception("Análisis no conserva parámetros.");
        }
        Console.WriteLine("Correcto: áreas, perímetros, orientación, pertenencia par-impar, proyecciones e interfaz.");
    }
}
