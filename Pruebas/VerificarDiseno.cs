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
            var elementoColor = (ComboBox)Buscar(ventana, "elementoColor");
            var coordenadas = (Label)Buscar(ventana, "coordenadasCursor");
            PointF? posicionCursor = null;
            lienzo.PosicionCursorCambiada += (sender, e) => posicionCursor = e.Punto;
            Exigir(lienzo.MostrarCuadricula && lienzo.MostrarEjes, "No se activaron cuadrícula y ejes.");
            Exigir(Buscar(ventana, "contenedorHerramientas").Dock == DockStyle.Left, "No se configuró la barra lateral.");
            Arrastrar(lienzo, new PointF(-70, 30), new PointF(-70, 30));
            Exigir(elementoColor.SelectedIndex == 2, "El clic no seleccionó el sector geométrico correcto.");
            Exigir(tx.Value == 0 && ty.Value == 0, "Un clic trasladó la figura.");
            Exigir(posicionCursor.HasValue && coordenadas.Text.Contains(string.Format("X: {0:0.00}", posicionCursor.Value.X)) &&
                coordenadas.Text.Contains(string.Format("Y: {0:0.00}", posicionCursor.Value.Y)), "Las coordenadas del cursor no se presentan.");
            typeof(LienzoGeometrico).GetMethod("OnMouseLeave", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(lienzo, new object[] { EventArgs.Empty });
            Exigir(!posicionCursor.HasValue && coordenadas.Text.Contains("—"), "Las coordenadas no se limpian al salir.");
            Arrastrar(lienzo, new PointF(40, -20), new PointF(70, 0));
            Exigir(tx.Value == 30 && ty.Value == 20, "Arrastrar la figura no actualizó Tx y Ty.");
            Exigir(elementoColor.SelectedIndex == 2, "Arrastrar cambió la selección como si fuera un clic.");
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
            superponer.Checked = false;
            selector.SelectedIndex = 0;
            tx.Value = 70; ty.Value = -30;
            ((NumericUpDown)Buscar(ventana, "angulo")).Value = 45;
            ((NumericUpDown)Buscar(ventana, "escalaX")).Value = 1.2m;
            ((NumericUpDown)Buscar(ventana, "escalaY")).Value = 0.8m;
            PointF sectorTransformado = lienzo.Transformacion.Transformar(new PointF(60, 30));
            Arrastrar(lienzo, sectorTransformado, sectorTransformado);
            Exigir(elementoColor.SelectedIndex == 0 && tx.Value == 70 && ty.Value == -30,
                "La selección del sector no respeta la transformación.");
            selector.SelectedIndex = 1;
            PointF[] petalo = lienzo.Flor.ObtenerPetalos()[4];
            float x = 0, y = 0;
            foreach (PointF punto in petalo) { x += punto.X / 5; y += punto.Y / 5; }
            PointF centroPetalo = lienzo.Transformacion.Transformar(new PointF(x, y));
            Arrastrar(lienzo, centroPetalo, centroPetalo);
            Exigir(elementoColor.SelectedIndex == 4, "El clic no seleccionó el pétalo transformado.");
            PointF centroFlor = lienzo.Transformacion.Transformar(PointF.Empty);
            Arrastrar(lienzo, centroFlor, centroFlor);
            Exigir(elementoColor.SelectedIndex == 4, "El hueco de la margarita seleccionó un pétalo.");
            Arrastrar(lienzo, new PointF(-350, -280), new PointF(-350, -280));
            Exigir(elementoColor.SelectedIndex == 4, "Un clic fuera de la figura cambió la selección.");
        }
        Console.WriteLine("Correcto: arrastre, selección por clic, figuras transformadas y coordenadas del cursor.");
    }
}
