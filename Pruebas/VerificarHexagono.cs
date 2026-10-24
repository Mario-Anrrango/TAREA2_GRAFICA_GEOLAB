using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarHexagono
{
    private static void Cerca(double actual, double esperado, double tolerancia, string caso)
    {
        if (Math.Abs(actual - esperado) > tolerancia)
            throw new Exception(caso + ": esperado " + esperado + ", recibido " + actual);
    }

    private static double Distancia(PointF a, PointF b)
    {
        double dx = a.X - b.X, dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            foreach (float lado in new float[] { 1f, 10f, 150f, 280f })
            {
                var figura = new HexagonoRegular(lado);
                PointF[] vertices = figura.ObtenerVertices();
                if (vertices.Length != 6) throw new Exception("Se requieren seis vértices.");
                double areaDoble = 0, perimetro = 0;
                for (int i = 0; i < 6; i++)
                {
                    PointF a = vertices[i], b = vertices[(i + 1) % 6];
                    // Cada triángulo tiene sus tres lados iguales al lado de entrada.
                    Cerca(Distancia(a, b), lado, 0.0001, "Lado exterior");
                    Cerca(Distancia(PointF.Empty, a), lado, 0.0001, "Radio");
                    Cerca(a.X + vertices[(i + 3) % 6].X, 0, 0.0001, "Simetría X");
                    Cerca(a.Y + vertices[(i + 3) % 6].Y, 0, 0.0001, "Simetría Y");
                    perimetro += Distancia(a, b);
                    areaDoble += (double)a.X * b.Y - (double)b.X * a.Y;
                }
                Cerca(perimetro, figura.Perimetro, 0.001, "Perímetro por distancias");
                Cerca(Math.Abs(areaDoble) / 2, figura.Area, 0.02, "Área por fórmula del cordón");
                Console.WriteLine("Correcto: lado " + lado);
            }

            var conocido = new HexagonoRegular(10);
            Cerca(conocido.Perimetro, 60, 0.000001, "Caso conocido: perímetro");
            Cerca(conocido.Area, 259.807621135, 0.000001, "Caso conocido: área");
            PointF[] copia = conocido.ObtenerVertices();
            copia[0] = PointF.Empty;
            Cerca(conocido.ObtenerVertices()[0].X, 10, 0.000001, "Vértices independientes");

            foreach (float invalido in new float[] { 0, -1, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
            {
                bool rechazado = false;
                try { new HexagonoRegular(invalido); }
                catch (ArgumentOutOfRangeException) { rechazado = true; }
                if (!rechazado) throw new Exception("Lado inválido aceptado: " + invalido);
            }

            // Comprobar la integración real sin abrir una ventana al usuario.
            using (var formulario = new GeoLab2D.GeoLab2D())
            {
                formulario.PerformLayout();
                var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
                var entrada = (NumericUpDown)formulario.Controls.Find("lado", true)[0];
                entrada.Value = 10;
                Cerca(lienzo.Hexagono.Lado, 10, 0, "Cambio desde la interfaz");
                entrada.Value = 150;
                using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
                {
                    lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                    int x = lienzo.Width / 2, y = lienzo.Height / 2;
                    Point[] muestras = { new Point(60, -30), new Point(0, -60), new Point(-60, -30),
                        new Point(-60, 30), new Point(0, 60), new Point(60, 30) };
                    Color[] colores = { Color.Coral, Color.Gold, Color.MediumSeaGreen,
                        Color.DeepSkyBlue, Color.MediumPurple, Color.HotPink };
                    for (int i = 0; i < muestras.Length; i++)
                        if (imagen.GetPixel(x + muestras[i].X, y + muestras[i].Y).ToArgb() != colores[i].ToArgb())
                            throw new Exception("Relleno incorrecto del sector " + i);
                    imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hexagono-verificado.png"));
                }
                entrada.Value = 280;
                if (lienzo.Width < 800 || lienzo.Height < 600) throw new Exception("Lienzo inferior al mínimo.");
            }
            Console.WriteLine("Correcto: entradas inválidas, área conocida, actualización y seis colores.");
            VerificarMargarita.Ejecutar();
            VerificarEstrella.Ejecutar();
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }
}
