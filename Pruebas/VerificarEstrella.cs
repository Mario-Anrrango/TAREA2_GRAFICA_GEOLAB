using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarEstrella
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

    private static void Rechaza(int n, int k, float radio)
    {
        try { new PoligonoEstrellado(n, k, radio); }
        catch (ArgumentOutOfRangeException) { return; }
        throw new Exception("Parámetros de estrella inválidos aceptados.");
    }

    public static void Ejecutar()
    {
        int casos = 0;
        for (int n = 5; n <= 64; n++)
        for (int k = 2; k <= (n - 1) / 2; k++)
        foreach (float radio in new float[] { 1, 150.5f, 280 })
        {
            var estrella = new PoligonoEstrellado(n, k, radio);
            PointF[] vertices = estrella.ObtenerVertices();
            PointF[][] recorridos = estrella.ObtenerRecorridos();
            if (vertices.Length != n || recorridos.Length != estrella.Componentes)
                throw new Exception("Cantidad incorrecta de vértices o componentes.");
            var visitados = new bool[n];
            int cantidad = 0;
            double total = 0;
            foreach (PointF[] recorrido in recorridos)
            {
                if (recorrido.Length < 3 || recorrido.Length != n / estrella.Componentes)
                    throw new Exception("Recorrido cerrado degenerado.");
                for (int j = 0; j < recorrido.Length; j++)
                {
                    PointF a = recorrido[j], b = recorrido[(j + 1) % recorrido.Length];
                    int indice = Array.IndexOf(vertices, a);
                    if (indice < 0 || visitados[indice]) throw new Exception("Vértice ausente o duplicado.");
                    visitados[indice] = true;
                    cantidad++;
                    if (b != vertices[(indice + k) % n]) throw new Exception("Salto incorrecto al recorrer o cerrar.");
                    Cerca(Distancia(PointF.Empty, a), radio, 0.0001, "Radio circunscrito");
                    total += Distancia(a, b);
                }
            }
            if (cantidad != n) throw new Exception("No se recorrieron todos los vértices.");
            Cerca(total, estrella.LongitudTotal, 0.02, "Suma de longitudes");
            double angulo = 2 * Math.PI / n;
            for (int j = 0; j < n; j++)
            {
                Cerca(vertices[j].X * Math.Cos(angulo) - vertices[j].Y * Math.Sin(angulo),
                    vertices[(j + 1) % n].X, 0.0001, "Simetría X");
                Cerca(vertices[j].X * Math.Sin(angulo) + vertices[j].Y * Math.Cos(angulo),
                    vertices[(j + 1) % n].Y, 0.0001, "Simetría Y");
            }
            casos++;
        }

        Cerca(new PoligonoEstrellado(5, 2, 10).LongitudTotal, 95.105651629515, 0.000001, "Pentagrama conocido");
        Cerca(new PoligonoEstrellado(6, 2, 10).LongitudTotal, 103.923048454133, 0.000001, "Dos triángulos conocidos");
        if (new PoligonoEstrellado(10, 4, 100).Componentes != 2 ||
            new PoligonoEstrellado(16, 6, 100).Componentes != 2)
            throw new Exception("Composición de dos estrellas incorrecta.");
        // Verificar también el límite admitido por el modelo, superior al de la interfaz.
        if (new PoligonoEstrellado(128, 63, 10).ObtenerRecorridos()[0].Length != 128)
            throw new Exception("Límite de puntas del modelo incorrecto.");
        Rechaza(4, 2, 100); Rechaza(129, 2, 100);
        Rechaza(5, 1, 100); Rechaza(5, 3, 100); Rechaza(8, 4, 100);
        foreach (float radio in new float[] { 0, -1, float.NaN, float.PositiveInfinity, float.NegativeInfinity })
            Rechaza(5, 2, radio);
        var modelo = new PoligonoEstrellado(5, 2, 100);
        PointF[][] copia = modelo.ObtenerRecorridos();
        copia[0][0] = PointF.Empty;
        if (modelo.ObtenerRecorridos()[0][0] == PointF.Empty) throw new Exception("Arreglos de estrella compartidos.");

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var entrada = (NumericUpDown)formulario.Controls.Find("lado", true)[0];
            var puntas = (NumericUpDown)formulario.Controls.Find("puntas", true)[0];
            var salto = (NumericUpDown)formulario.Controls.Find("salto", true)[0];
            var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            if (puntas.Enabled || salto.Enabled) throw new Exception("Parámetros de estrella habilitados para hexágono.");
            selector.SelectedIndex = 2;
            if (!puntas.Enabled || !salto.Enabled || lienzo.Hexagono != null || lienzo.Flor != null)
                throw new Exception("Selección de estrella incorrecta.");
            entrada.Value = 220;
            puntas.Value = 8;
            salto.Value = 3;
            Cerca(lienzo.Estrella.Puntas, 8, 0, "Puntas desde interfaz");
            Cerca(lienzo.Estrella.Salto, 3, 0, "Salto desde interfaz");
            Cerca(lienzo.Estrella.RadioExterior, 220, 0, "Radio desde interfaz");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                int pixeles = 0;
                for (int y = 0; y < imagen.Height; y++)
                for (int x = 0; x < imagen.Width; x++)
                    if (imagen.GetPixel(x, y).ToArgb() == Color.BlueViolet.ToArgb()) pixeles++;
                if (pixeles < 1000) throw new Exception("No se renderizaron los trazos de la estrella.");
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "estrella-verificada.png"));
            }
            puntas.Value = 64;
            salto.Value = 31;
            puntas.Value = 5;
            Cerca(salto.Value == 2 ? 2 : -1, 2, 0, "Ajuste de salto al reducir puntas");
            Cerca(lienzo.Estrella.Puntas, 5, 0, "Reducción de puntas");
            selector.SelectedIndex = 1;
            if (lienzo.Estrella != null || lienzo.Flor == null || puntas.Enabled || salto.Enabled)
                throw new Exception("Regreso a margarita incorrecto.");
            selector.SelectedIndex = 0;
            if (lienzo.Estrella != null || lienzo.Hexagono == null) throw new Exception("Regreso a hexágono incorrecto.");
        }
        Console.WriteLine("Correcto: " + casos + " casos de estrellas; ciclos, simetría, longitudes, validación e interfaz.");
    }
}
