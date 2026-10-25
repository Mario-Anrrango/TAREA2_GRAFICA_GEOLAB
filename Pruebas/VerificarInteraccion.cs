using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using GeoLab2D.Controles;
using GeoLab2D.Presentacion;

internal static class VerificarInteraccion
{
    private static void Exigir(bool condicion, string mensaje)
    {
        if (!condicion) throw new Exception(mensaje);
    }

    private static void Rechaza(Action accion)
    {
        try { accion(); }
        catch (ArgumentException) { return; }
        throw new Exception("Color o índice inválido aceptado.");
    }

    private static void RechazarTexto(EntradaNumericaValidada entrada, string texto)
    {
        decimal anterior = entrada.Value;
        // Escribir en la caja interna simula la edición del usuario. El setter Text
        // de NumericUpDown aplica por sí solo su validación programática y recorta valores.
        bool escrito = false;
        foreach (Control control in entrada.Controls)
        {
            var caja = control as TextBox;
            if (caja == null) continue;
            caja.Text = texto;
            escrito = true;
            break;
        }
        Exigir(escrito, "No se encontró la caja de edición numérica.");
        entrada.ConfirmarEntrada();
        Exigir(entrada.Value == anterior && !string.IsNullOrEmpty(entrada.ErrorEntrada),
            "No se conservó el valor anterior al rechazar: " + texto + "; valor=" + entrada.Value + "; aviso=" + entrada.ErrorEntrada);
    }

    public static void Ejecutar()
    {
        PaletaFiguras original = PaletaFiguras.Predeterminada();
        PaletaFiguras editada = original.ConColor(0, 0, Color.Navy);
        Exigir(original.ObtenerColor(0, 0) == Color.Coral && editada.ObtenerColor(0, 0) == Color.Navy,
            "La paleta original fue modificada.");
        Rechaza(() => original.ConColor(0, 0, Color.Gold));
        Rechaza(() => original.ConColor(0, 0, Color.FromArgb(Color.Gold.ToArgb())));
        Rechaza(() => original.ConColor(1, 0, Color.Transparent));
        Rechaza(() => original.ConColor(1, 0, Color.Empty));
        Rechaza(() => original.ObtenerColor(3, 0));
        Rechaza(() => original.ConColor(0, 6, Color.Navy));
        using (var entrada = new EntradaNumericaValidada { Minimum = 1, Maximum = 64, Value = 5, DecimalPlaces = 0 })
        {
            RechazarTexto(entrada, "999"); RechazarTexto(entrada, "-1");
            RechazarTexto(entrada, ""); RechazarTexto(entrada, "abc");
            RechazarTexto(entrada, 5.5m.ToString(CultureInfo.CurrentCulture));
            entrada.Text = "64"; entrada.ConfirmarEntrada();
            Exigir(entrada.Value == 64 && entrada.ErrorEntrada == null, "No se aceptó el límite válido.");
            entrada.DecimalPlaces = 2;
            entrada.Text = 12.25m.ToString(CultureInfo.CurrentCulture); entrada.ConfirmarEntrada();
            Exigir(entrada.Value == 12.25m, "Separador decimal local no aceptado.");
            RechazarTexto(entrada, 12.345m.ToString(CultureInfo.CurrentCulture));
            string separador = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            if (!string.IsNullOrEmpty(separador)) RechazarTexto(entrada, "1" + separador + "5");
        }

        using (var formulario = new GeoLab2D.GeoLab2D())
        {
            var selector = (ComboBox)formulario.Controls.Find("selectorFigura", true)[0];
            var panel = (PanelColores)formulario.Controls.Find("panelColores", true)[0];
            var elemento = (ComboBox)formulario.Controls.Find("elementoColor", true)[0];
            var lienzo = (LienzoGeometrico)formulario.Controls.Find("lienzo", true)[0];
            var puntas = (EntradaNumericaValidada)formulario.Controls.Find("puntas", true)[0];
            var salto = (EntradaNumericaValidada)formulario.Controls.Find("salto", true)[0];
            Exigir(panel.AplicarColor(Color.Navy), "Edición de color válida rechazada.");
            elemento.SelectedIndex = 1;
            Exigir(!panel.AplicarColor(Color.Navy), "Se permitió un color repetido.");
            Exigir(lienzo.Paleta.ObtenerColor(0, 1) == Color.Gold, "El color inválido cambió el dibujo.");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                Exigir(imagen.GetPixel(lienzo.Width / 2 + 60, lienzo.Height / 2 - 30).ToArgb() == Color.Navy.ToArgb(),
                    "No se renderizó el sector editado.");
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "colores-verificados.png"));
            }
            selector.SelectedIndex = 1;
            Exigir(elemento.Items.Count == 5 && panel.AplicarColor(Color.LimeGreen), "No se puede editar el pétalo.");
            using (var imagen = new Bitmap(lienzo.Width, lienzo.Height))
            {
                PointF[] petalo = lienzo.Flor.ObtenerPetalos()[0];
                double cx = 0, cy = 0;
                foreach (PointF p in petalo) { cx += p.X / 5.0; cy += p.Y / 5.0; }
                lienzo.DrawToBitmap(imagen, lienzo.ClientRectangle);
                Exigir(imagen.GetPixel(lienzo.Width / 2 + (int)Math.Round(cx), lienzo.Height / 2 - (int)Math.Round(cy)).ToArgb()
                    == Color.LimeGreen.ToArgb(), "No se renderizó el pétalo editado.");
            }
            selector.SelectedIndex = 2;
            Exigir(elemento.Items.Count == 1 && panel.AplicarColor(Color.Crimson), "Edición de estrella fallida.");
            puntas.Text = "64"; puntas.ConfirmarEntrada();
            salto.Value = 31;
            RechazarTexto(salto, "32");
            puntas.Text = "5"; puntas.ConfirmarEntrada();
            Exigir(salto.Value == 2 && salto.Maximum == 2 && salto.ErrorEntrada == null, "Salto dinámico no validado.");
            Exigir(lienzo.Estrella.Puntas == 5, "El texto confirmado no actualizó la figura.");
            selector.SelectedIndex = 0;
            Exigir(lienzo.Paleta.ObtenerColor(0, 0) == Color.Navy, "Color perdido al alternar.");
            panel.RestablecerColores();
            Exigir(lienzo.Paleta.ObtenerColor(0, 0) == Color.Coral &&
                lienzo.Paleta.ObtenerColor(1, 0) == Color.LimeGreen && lienzo.Paleta.ObtenerColor(2, 0) == Color.Crimson,
                "Restablecer afectó a otra figura.");
            var escala = (EntradaNumericaValidada)formulario.Controls.Find("escalaX", true)[0];
            RechazarTexto(escala, "0");
            Exigir(lienzo.Transformacion[0, 0] == 1, "La escala inválida afectó la matriz.");
            selector.SelectedIndex = 4;
            formulario.ClientSize = new Size(520, 420);
            formulario.PerformLayout();
            var herramientas = formulario.Controls.Find("contenedorHerramientas", true)[0];
            var vista = formulario.Controls.Find("contenedorLienzo", true)[0];
            Exigir(herramientas.Height <= formulario.ClientSize.Height / 2 && vista.Height > 50,
                "Las herramientas ocultan completamente el lienzo.");
            Exigir(lienzo.Width >= 800 && lienzo.Height >= 600, "Se redujo el lienzo mínimo.");
            selector.SelectedIndex = 0;
            formulario.ClientSize = new Size(850, 794);
            GuardarVistaSinVentana(formulario);
        }
        Console.WriteLine("Correcto: paletas, seis colores distintos, texto inválido, precisión, límites y herramientas desplazables.");
    }

    private static void GuardarVistaSinVentana(Form formulario)
    {
        // Un panel independiente permite renderizar los controles sin abrir ventanas.
        using (var host = new Panel { Size = formulario.ClientSize })
        {
            var controles = new Control[formulario.Controls.Count];
            formulario.Controls.CopyTo(controles, 0);
            host.Controls.AddRange(controles);
            host.CreateControl();
            host.PerformLayout();
            using (var imagen = new Bitmap(host.Width, host.Height))
            {
                host.DrawToBitmap(imagen, host.ClientRectangle);
                imagen.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "interfaz-verificada.png"));
            }
        }
    }
}
