using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarMargarita
{
    private static void Cerca(double actual, double esperado, double tolerancia, string caso)
    {
        if (double.IsNaN(actual) || Math.Abs(actual - esperado) > tolerancia)
            throw new Exception(caso + ": esperado " + esperado + ", recibido " + actual);
    }

    private static double Distancia(PointF a, PointF b)
    {
        double x = a.X - b.X, y = a.Y - b.Y;
        return Math.Sqrt(x * x + y * y);
    }

    public static void Ejecutar()
    {
        foreach (float lado in new float[] { 1, 10, 150.5f, 280 })
        {
            var flor = new FlorMargarita(lado);
            PointF[][] petalos = flor.ObtenerPetalos();
            if (petalos.Length != 5) throw new Exception("Se requieren cinco pétalos.");
            double areaTotal = 0;
            double radioExterior = lado / (2 * Math.Sin(Math.PI / 5));
            double paso = 2 * Math.PI / 5;
            for (int i = 0; i < 5; i++)
            {
                if (petalos[i].Length != 5) throw new Exception("Cada pétalo necesita cinco vértices.");
                double cx = 0, cy = 0, areaDoble = 0, perimetro = 0;
                foreach (PointF punto in petalos[i]) { cx += punto.X / 5.0; cy += punto.Y / 5.0; }
                double radioPetalo = flor.LadoPetalo / (2 * Math.Sin(Math.PI / 5));
                for (int j = 0; j < 5; j++)
                {
                    PointF a = petalos[i][j], b = petalos[i][(j + 1) % 5];
                    Cerca(Distancia(a, b), flor.LadoPetalo, 0.0001, "Lados de pétalo iguales");
                    Cerca(Math.Sqrt((a.X - cx) * (a.X - cx) + (a.Y - cy) * (a.Y - cy)),
                        radioPetalo, 0.0001, "Vértices equidistantes del centro");
                    PointF rotado = petalos[(i + 1) % 5][j];
                    Cerca(a.X * Math.Cos(paso) - a.Y * Math.Sin(paso), rotado.X, 0.0001, "Simetría de 72° X");
                    Cerca(a.X * Math.Sin(paso) + a.Y * Math.Cos(paso), rotado.Y, 0.0001, "Simetría de 72° Y");
                    // Cada punto debe quedar dentro de los cinco semiplanos del pentágono exterior.
                    for (int k = 0; k < 5; k++)
                    {
                        double normal = Math.PI / 2 + (k + 0.5) * paso;
                        if (a.X * Math.Cos(normal) + a.Y * Math.Sin(normal) > radioExterior * Math.Cos(Math.PI / 5) + 0.0001)
                            throw new Exception("Pétalo fuera del pentágono exterior.");
                    }
                    perimetro += Distancia(a, b);
                    areaDoble += (double)a.X * b.Y - (double)b.X * a.Y;
                }
                Cerca(Distancia(PointF.Empty, petalos[i][0]), radioExterior, 0.0001, "Punta sobre pentágono exterior");
                Cerca(perimetro, flor.PerimetroPetalo, 0.001, "Perímetro de pétalo");
                Cerca(Math.Abs(areaDoble) / 2, flor.AreaPetalo, 0.02, "Área de pétalo por cordón");
                areaTotal += Math.Abs(areaDoble) / 2;
            }
            Cerca(areaTotal, flor.AreaTotal, 0.05, "Área de cinco pétalos");
            Console.WriteLine("Correcto: margarita de lado exterior " + lado);
        }

        Cerca(new FlorMargarita(10).LadoPetalo, 3.819660112501, 0.000001, "Lado conocido");
        Cerca(new FlorMargarita(10).AreaTotal, 125.507134928892, 0.000001, "Área roja conocida");
        foreach (float lado in new float[] { 0, -1, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            bool rechazado = false;
            try { new FlorMargarita(lado); }
            catch (ArgumentOutOfRangeException) { rechazado = true; }
            if (!rechazado) throw new Exception("Lado exterior inválido aceptado.");
        }

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var entrada = (NumericUpDown)formulario.Controls.Find("lado", true)[0];
            var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            selector.SelectedIndex = 1;
            entrada.Value = 280;
            Cerca(lienzo.Flor.LadoExterior, 280, 0, "Lado de flor desde interfaz");
            if (lienzo.Hexagono != null) throw new Exception("Dos figuras activas simultáneamente.");
            PointF[][] copia = lienzo.Flor.ObtenerPetalos();
            copia[0][0] = PointF.Empty;
            if (lienzo.Flor.ObtenerPetalos()[0][0] == PointF.Empty) throw new Exception("Vértices compartidos con el exterior.");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                int x = lienzo.Width / 2, y = lienzo.Height / 2;
                if (imagen.GetPixel(x, y).ToArgb() == Color.Red.ToArgb()) throw new Exception("El centro no debe tener relleno de pétalo.");
                foreach (PointF[] petalo in lienzo.Flor.ObtenerPetalos())
                {
                    double cx = 0, cy = 0;
                    foreach (PointF p in petalo) { cx += p.X / 5; cy += p.Y / 5; }
                    if (imagen.GetPixel(x + (int)Math.Round(cx), y - (int)Math.Round(cy)).ToArgb() != Color.Red.ToArgb())
                        throw new Exception("Pétalo sin relleno rojo.");
                }
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "margarita-verificada.png"));
            }
            selector.SelectedIndex = 0;
            if (lienzo.Flor != null || lienzo.Hexagono == null) throw new Exception("No se restauró el hexágono.");
            Cerca(lienzo.Hexagono.Lado, 280, 0, "Lado conservado al cambiar de figura");
        }
        Console.WriteLine("Correcto: simetría, pentágonos regulares, límites, área, selección y rellenos de margarita.");
    }
}
