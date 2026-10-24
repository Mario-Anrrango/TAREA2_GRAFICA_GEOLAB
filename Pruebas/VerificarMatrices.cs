using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarMatrices
{
    private static void Cerca(double actual, double esperado, string caso, double tolerancia = 0.001)
    {
        if (double.IsNaN(actual) || Math.Abs(actual - esperado) > tolerancia)
            throw new Exception(caso + ": esperado " + esperado + ", recibido " + actual);
    }

    private static void Punto(PointF actual, float x, float y, string caso)
    {
        Cerca(actual.X, x, caso + " X"); Cerca(actual.Y, y, caso + " Y");
    }

    private static void Rechaza(Action accion)
    {
        try { accion(); }
        catch (ArgumentException) { return; }
        catch (OverflowException) { return; }
        throw new Exception("Matriz o punto inválido aceptado.");
    }

    private static NumericUpDown Entrada(Form formulario, string nombre)
    {
        return (NumericUpDown)formulario.Controls.Find(nombre, true)[0];
    }

    public static void Ejecutar()
    {
        PointF p = new PointF(1, 2);
        Punto(Matriz3x3.Identidad().Transformar(p), 1, 2, "Identidad");
        Punto(Matriz3x3.Traslacion(5, -1).Transformar(p), 6, 1, "Traslación");
        Punto(Matriz3x3.Rotacion(90).Transformar(p), -2, 1, "Rotación positiva de 90°");
        Punto(Matriz3x3.Rotacion(-90).Transformar(p), 2, -1, "Rotación negativa");
        Punto(Matriz3x3.Rotacion(720).Transformar(p), 1, 2, "Vueltas completas");
        Punto(Matriz3x3.Escalamiento(2, 3).Transformar(p), 2, 6, "Escalamiento no uniforme");
        Matriz3x3 compuesta = Matriz3x3.Traslacion(5, -1).Multiplicar(Matriz3x3.Rotacion(90))
            .Multiplicar(Matriz3x3.Escalamiento(2, 3));
        Punto(compuesta.Transformar(p), -1, 1, "Orden T R S");
        Punto(Matriz3x3.Traslacion(10, 0).Multiplicar(Matriz3x3.Rotacion(90)).Transformar(new PointF(1, 0)),
            10, 1, "Trasladar después de rotar");
        Punto(Matriz3x3.Rotacion(90).Multiplicar(Matriz3x3.Traslacion(10, 0)).Transformar(new PointF(1, 0)),
            0, 11, "Rotar después de trasladar");
        Punto(Matriz3x3.Rotacion(-37).Multiplicar(Matriz3x3.Rotacion(37)).Transformar(p), 1, 2, "Rotación inversa");
        Cerca(compuesta[2, 0], 0, "Fila homogénea X");
        Cerca(compuesta[2, 1], 0, "Fila homogénea Y");
        Cerca(compuesta[2, 2], 1, "Fila homogénea W");
        PointF[] rectangulo = { new PointF(0, 0), new PointF(6, 0), new PointF(6, 4), new PointF(0, 4) };
        PointF[] rigido = Matriz3x3.Traslacion(25, -10).Multiplicar(Matriz3x3.Rotacion(37)).Transformar(rectangulo);
        Cerca(GeometriaPlana.AreaFirmada(rigido), 24, "Área invariante en movimiento rígido");
        Cerca(GeometriaPlana.Perimetro(rigido), 20, "Perímetro invariante en movimiento rígido");
        PointF[] escalado = Matriz3x3.Escalamiento(2, 3).Transformar(rectangulo);
        Cerca(GeometriaPlana.AreaFirmada(escalado), 144, "Área por determinante de escala");
        Cerca(GeometriaPlana.Perimetro(escalado), 48, "Perímetro no uniforme recalculado");
        Cerca(GeometriaPlana.Perimetro(Matriz3x3.Escalamiento(2, 2).Transformar(rectangulo)), 40, "Perímetro uniforme");
        Cerca(GeometriaPlana.AreaFirmada(Matriz3x3.Escalamiento(-1, 1).Transformar(rectangulo)), -24, "Reflexión invierte orientación");
        Cerca(GeometriaPlana.AreaFirmada(Matriz3x3.Escalamiento(0, 1).Transformar(rectangulo)), 0, "Escala singular admitida por modelo");
        var copia = Matriz3x3.Identidad().Transformar(new[] { rectangulo });
        copia[0][0] = new PointF(99, 99);
        Punto(rectangulo[0], 0, 0, "No modificar vértices originales");
        Rechaza(() => Matriz3x3.Traslacion(double.NaN, 0));
        Rechaza(() => Matriz3x3.Rotacion(double.PositiveInfinity));
        Rechaza(() => Matriz3x3.Escalamiento(1, double.NegativeInfinity));
        Rechaza(() => Matriz3x3.Identidad().Multiplicar(null));
        Rechaza(() => Matriz3x3.Identidad().Transformar((PointF[])null));
        Rechaza(() => Matriz3x3.Identidad().Transformar(new PointF(float.NaN, 0)));
        Rechaza(() => Matriz3x3.Traslacion(double.MaxValue, 0).Transformar(p));
        Rechaza(() => Matriz3x3.Escalamiento(double.MaxValue, 1).Multiplicar(Matriz3x3.Escalamiento(2, 1)));

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            var panel = (GeoLab2D.Controles.PanelTransformaciones)formulario.Controls.Find("panelTransformaciones", true)[0];
            var tx = Entrada(formulario, "traslacionX"); var ty = Entrada(formulario, "traslacionY");
            var angulo = Entrada(formulario, "angulo");
            var sx = Entrada(formulario, "escalaX"); var sy = Entrada(formulario, "escalaY");
            var limites = (Label)formulario.Controls.Find("limitesTransformacion", true)[0];
            var reiniciar = (Button)formulario.Controls.Find("restablecerTransformacion", true)[0];
            tx.Value = 60; ty.Value = -20; angulo.Value = 30; sx.Value = 1.2m; sy.Value = 0.8m;
            PointF esperado = panel.Matriz.Transformar(new PointF(1, 2));
            Punto(lienzo.Transformacion.Transformar(p), esperado.X, esperado.Y, "Matriz enlazada al lienzo");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                PointF interior = panel.Matriz.Transformar(new PointF(60, 30));
                if (imagen.GetPixel(lienzo.Width / 2 + (int)Math.Round(interior.X),
                    lienzo.Height / 2 - (int)Math.Round(interior.Y)).ToArgb() != Color.Coral.ToArgb())
                    throw new Exception("El sector del hexágono no se transformó con su figura.");
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transformacion-verificada.png"));
            }
            sx.Value = 3; sy.Value = 3;
            if (!limites.Text.Contains("fuera")) throw new Exception("No se indicó el recorte del lienzo.");
            // PerformClick puede omitir un botón de un formulario oculto; invocar su evento de clic.
            typeof(Button).GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(reiniciar, new object[] { EventArgs.Empty });
            Punto(panel.Matriz.Transformar(p), 1, 2, "Restablecer controles");
            if (tx.Value != 0 || ty.Value != 0 || angulo.Value != 0 || sx.Value != 1 || sy.Value != 1)
                throw new Exception("Restablecimiento incompleto.");
            selector.SelectedIndex = 4;
            var fuente = (ComboBox)formulario.Controls.Find("figuraAnalisis", true)[0];
            fuente.SelectedIndex = 0;
            Entrada(formulario, "lado").Value = 20;
            tx.Value = 80;
            Entrada(formulario, "analisisPx").Value = 80; Entrada(formulario, "analisisPy").Value = 0;
            var resultados = (Label)formulario.Controls.Find("resultadosAnalisis", true)[0];
            if (!resultados.Text.Contains("Dentro")) throw new Exception("Pertenencia no usa la figura trasladada.");
            Entrada(formulario, "analisisPx").Value = 0;
            if (!resultados.Text.Contains("Fuera")) throw new Exception("Pertenencia todavía usa la figura original.");
            sx.Value = 2; sy.Value = 2;
            double area = new HexagonoRegular(20).Area * 4;
            if (!resultados.Text.Contains(string.Format("Área: {0:0.00}", area)))
                throw new Exception("Área de análisis no usa la escala.");
            fuente.SelectedIndex = 1;
            if (lienzo.Transformacion[0, 2] != 80) throw new Exception("Transformación no conservada en margarita.");
            fuente.SelectedIndex = 2; Entrada(formulario, "puntas").Value = 64;
            if (!resultados.Text.Contains("Área algebraica")) throw new Exception("Se perdió la definición de área de estrella.");
            selector.SelectedIndex = 3;
            selector.SelectedIndex = 2;
            if (lienzo.Estrella.Puntas != 64 || tx.Value != 80) throw new Exception("Parámetros no conservados al alternar modos.");
        }
        Console.WriteLine("Correcto: matrices, orden de composición, invariantes, dibujo, análisis transformado y restablecimiento.");
    }
}
