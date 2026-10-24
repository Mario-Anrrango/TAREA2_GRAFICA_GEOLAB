using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Geometria;

internal static class VerificarVectores
{
    private static void Cerca(double actual, double esperado, string caso)
    {
        if (double.IsNaN(actual) || Math.Abs(actual - esperado) > 0.000001)
            throw new Exception(caso + ": esperado " + esperado + ", recibido " + actual);
    }

    private static void Rechaza(Action accion)
    {
        try { accion(); }
        catch (ArgumentException) { return; }
        throw new Exception("Parámetro inválido de vector aceptado.");
    }

    public static void Ejecutar()
    {
        var a = new Vector2D(3, 4);
        var b = new Vector2D(-4, 3);
        Cerca(a.Magnitud, 5, "Magnitud 3-4-5");
        Cerca(b.Magnitud, 5, "Magnitud perpendicular");
        Cerca(a.DireccionGrados.Value, 53.130102354156, "Dirección primer cuadrante");
        Cerca(a.ProductoPunto(b), 0, "Vectores perpendiculares");
        Cerca(a.Determinante(b), 25, "Determinante positivo");
        Cerca(b.Determinante(a), -25, "Determinante antisimétrico");
        Cerca(a.ProductoPunto(new Vector2D(-3, -4)), -25, "Vectores opuestos");
        Cerca(a.Determinante(new Vector2D(6, 8)), 0, "Vectores paralelos");
        float[,] ejes = { { 1, 0, 0 }, { 0, 1, 90 }, { -1, 0, 180 }, { 0, -1, 270 },
            { -1, 1, 135 }, { -1, -1, 225 }, { 1, -1, 315 } };
        for (int i = 0; i < ejes.GetLength(0); i++)
            Cerca(new Vector2D(ejes[i, 0], ejes[i, 1]).DireccionGrados.Value, ejes[i, 2], "Dirección en ejes y cuadrantes");
        var cero = new Vector2D(0, 0);
        Cerca(cero.Magnitud, 0, "Magnitud cero");
        if (cero.DireccionGrados.HasValue) throw new Exception("El vector cero no tiene dirección.");
        Cerca(cero.ProductoPunto(a), 0, "Producto con cero");
        Cerca(cero.Determinante(a), 0, "Determinante con cero");
        foreach (float invalido in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity })
        {
            Rechaza(() => new Vector2D(invalido, 0));
            Rechaza(() => new Vector2D(0, invalido));
        }
        Rechaza(() => a.ProductoPunto(null));
        Rechaza(() => a.Determinante(null));
        var grande = new Vector2D(float.MaxValue, float.MaxValue);
        if (double.IsInfinity(grande.Magnitud) || double.IsInfinity(grande.ProductoPunto(grande)))
            throw new Exception("Desbordamiento al operar con componentes float finitas.");

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var entradaLado = (NumericUpDown)formulario.Controls.Find("lado", true)[0];
            var puntas = (NumericUpDown)formulario.Controls.Find("puntas", true)[0];
            var panel = (GeoLab2D.Controles.PanelVectores)formulario.Controls.Find("panelVectores", true)[0];
            var lienzo = (GeoLab2D.Controles.LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            var ax = (NumericUpDown)panel.Controls.Find("ax", true)[0];
            var ay = (NumericUpDown)panel.Controls.Find("ay", true)[0];
            var bx = (NumericUpDown)panel.Controls.Find("bx", true)[0];
            var by = (NumericUpDown)panel.Controls.Find("by", true)[0];
            var resultados = (Label)panel.Controls.Find("resultadosVectores", true)[0];
            selector.SelectedIndex = 3;
            if (entradaLado.Enabled || puntas.Enabled || lienzo.Hexagono != null || lienzo.Flor != null || lienzo.Estrella != null)
                throw new Exception("Modo de vectores incorrecto.");
            ax.Value = 3; ay.Value = 4; bx.Value = -4; by.Value = 3;
            Cerca(panel.VectorA.Magnitud, 5, "Edición de componentes");
            Cerca(panel.VectorA.ProductoPunto(panel.VectorB), 0, "Producto desde interfaz");
            if (!resultados.Text.Contains(string.Format("det(A, B) = {0:0.00}", 25)))
                throw new Exception("Los resultados no se actualizan.");
            ax.Value = 0; ay.Value = 0;
            if (!resultados.Text.Contains("no definida (vector cero)")) throw new Exception("Dirección nula mal presentada.");
            ax.Value = 120; ay.Value = 80; bx.Value = -60; by.Value = 140;
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                int azules = 0, naranjas = 0;
                for (int y = 0; y < imagen.Height; y++)
                for (int x = 0; x < imagen.Width; x++)
                {
                    int color = imagen.GetPixel(x, y).ToArgb();
                    if (color == Color.RoyalBlue.ToArgb()) azules++;
                    if (color == Color.DarkOrange.ToArgb()) naranjas++;
                }
                if (azules < 100 || naranjas < 100) throw new Exception("No se dibujaron ambos vectores.");
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vectores-verificados.png"));
            }
            ax.Value = -250; ay.Value = 250; bx.Value = 250; by.Value = -250;
            Cerca(panel.VectorA.Determinante(panel.VectorB), 0, "Extremos de controles");
            selector.SelectedIndex = 2;
            puntas.Value = 64;
            if (lienzo.Estrella == null || lienzo.Estrella.Puntas != 64) throw new Exception("No se conservó el máximo de 64 puntas.");
            selector.SelectedIndex = 0;
            if (lienzo.Hexagono == null || !entradaLado.Enabled) throw new Exception("No se restauró el modo de figuras.");
            selector.SelectedIndex = 3;
            Cerca(panel.VectorA.X, -250, "Componentes conservadas al alternar modos");
        }
        Console.WriteLine("Correcto: magnitudes, direcciones, producto punto, determinantes, vector cero, validación e interfaz.");
    }
}
