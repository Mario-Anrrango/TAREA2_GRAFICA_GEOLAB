using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using GeoLab2D.Controles;

internal static class VerificarDiseno
{
    private static Control Buscar(Form ventana, string nombre) { return ventana.Controls.Find(nombre, true)[0]; }
    private static void Exigir(bool condicion, string mensaje) { if (!condicion) throw new Exception(mensaje); }

    private static void Arrastrar(LienzoGeometrico lienzo, PointF origen, PointF destino)
    {
        int cx = lienzo.Width / 2, cy = lienzo.Height / 2;
        string[] metodos = { "OnMouseDown", "OnMouseMove", "OnMouseUp" };
        PointF[] puntos = { origen, destino, destino };
        for (int i = 0; i < metodos.Length; i++)
            typeof(LienzoGeometrico).GetMethod(metodos[i], BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(lienzo, new object[] { new MouseEventArgs(MouseButtons.Left, 1,
                    cx + (int)puntos[i].X, cy - (int)puntos[i].Y, 0) });
    }

    public static void Ejecutar()
    {
        using (var ventana = new GeoLab2D.GeoLab2D())
        {
            var lienzo = (LienzoGeometrico)Buscar(ventana, "lienzo");
            var selector = (ComboBox)Buscar(ventana, "selectorFigura");
            var tx = (NumericUpDown)Buscar(ventana, "traslacionX");
            var ty = (NumericUpDown)Buscar(ventana, "traslacionY");
            Exigir(lienzo.MostrarCuadricula && lienzo.MostrarEjes, "No se activaron cuadrícula y ejes.");
            Exigir(Buscar(ventana, "contenedorHerramientas").Dock == DockStyle.Left, "No se configuró la barra lateral.");
            Arrastrar(lienzo, new PointF(40, -20), new PointF(70, 0));
            Exigir(tx.Value == 30 && ty.Value == 20, "Arrastrar la figura no actualizó Tx y Ty.");
            var superponer = (CheckBox)Buscar(ventana, "mostrarAnalisis");
            superponer.Checked = true;
            Exigir(lienzo.Hexagono != null, "Superponer análisis eliminó la figura coloreada.");
            Arrastrar(lienzo, new PointF(30, 100), new PointF(70, 120));
            Exigir(((NumericUpDown)Buscar(ventana, "analisisPx")).Value == 70 &&
                ((NumericUpDown)Buscar(ventana, "analisisPy")).Value == 120, "Arrastrar P no actualizó sus coordenadas.");
            selector.SelectedIndex = 1;
            Exigir(((ComboBox)Buscar(ventana, "figuraAnalisis")).SelectedIndex == 1 && lienzo.Flor != null,
                "La superposición no siguió a la margarita.");
            selector.SelectedIndex = 3;
            Arrastrar(lienzo, new PointF(120, 80), new PointF(90, 60));
            Exigir(((NumericUpDown)Buscar(ventana, "ax")).Value == 90 &&
                ((NumericUpDown)Buscar(ventana, "ay")).Value == 60, "El extremo del vector no se puede arrastrar.");
            using (var contenido = new Label { Text = "Prueba", AutoSize = true })
            using (var seccion = new SeccionDesplegable("PRUEBA", contenido))
            {
                seccion.Alternar();
                Exigir(!seccion.Expandida, "No se plegó la sección.");
                seccion.Mostrar(true);
                Exigir(!seccion.Expandida, "Actualizar la disponibilidad abrió una sección plegada.");
                seccion.Alternar();
                Exigir(seccion.Expandida, "No se expandió la sección.");
            }
            selector.SelectedIndex = 2;
            ((NumericUpDown)Buscar(ventana, "puntas")).Value = 64;
            Exigir(lienzo.Estrella.Puntas == 64, "Se alteró el máximo de puntas.");
            Exigir(lienzo.Width >= 800 && lienzo.Height >= 600, "El lienzo es inferior al mínimo.");
        }
        Console.WriteLine("Correcto: barra lateral, cuadrícula, secciones, arrastre de figuras, puntos y vectores.");
    }
}
