using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
    public sealed class ElementoSeleccionadoEventArgs : EventArgs
    {
        public int Figura { get; private set; }
        public int Elemento { get; private set; }
        public ElementoSeleccionadoEventArgs(int figura, int elemento) { Figura = figura; Elemento = elemento; }
    }

    public sealed class PosicionCursorEventArgs : EventArgs
    {
        public PointF? Punto { get; private set; }
        public PosicionCursorEventArgs(PointF? punto) { Punto = punto; }
    }

    public sealed class DesplazamientoEventArgs : EventArgs
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public DesplazamientoEventArgs(float x, float y) { X = x; Y = y; }
    }

    public sealed class PuntoMovidoEventArgs : EventArgs
    {
        public string Nombre { get; private set; }
        public PointF Punto { get; private set; }
        public PuntoMovidoEventArgs(string nombre, PointF punto) { Nombre = nombre; Punto = punto; }
    }

    public partial class LienzoGeometrico
    {
        public bool MostrarCuadricula { get; set; }
        public bool MostrarEjes { get; set; }
        public event EventHandler<DesplazamientoEventArgs> DesplazamientoSolicitado;
        public event EventHandler<PuntoMovidoEventArgs> PuntoMovido;
        public event EventHandler<ElementoSeleccionadoEventArgs> ElementoSeleccionado;
        public event EventHandler<PosicionCursorEventArgs> PosicionCursorCambiada;
        private bool analisisSuperpuesto, arrastrandoFigura;
        private bool movimientoDetectado;
        private Point inicioPulsacion;
        private int figuraSeleccionada = -1, elementoSeleccionado = -1;
        private string puntoActivo;
        private Point ultimaPosicion;

        public void SeleccionarElemento(int figura, int elemento)
        {
            paleta.ObtenerColor(figura, elemento);
            if (figuraSeleccionada == figura && elementoSeleccionado == elemento) return;
            figuraSeleccionada = figura; elementoSeleccionado = elemento;
            Invalidate();
        }

        private PointF[] ContornoElemento(int figura, int elemento)
        {
            if (figura == 0 && hexagono != null)
            {
                PointF[] vertices = hexagono.ObtenerVertices();
                return transformacion.Transformar(new[] { PointF.Empty, vertices[elemento], vertices[(elemento + 1) % 6] });
            }
            if (figura == 1 && flor != null) return transformacion.Transformar(flor.ObtenerPetalos()[elemento]);
            return null;
        }

        private void SeleccionarPorClic(Point punto)
        {
            int figura = hexagono != null ? 0 : flor != null ? 1 : estrella != null ? 2 : -1;
            if (figura < 0) return;
            if (figura == 2)
            {
                if (ElementoSeleccionado != null) ElementoSeleccionado(this, new ElementoSeleccionadoEventArgs(2, 0));
                return;
            }
            PointF posicion = ACoordenadas(punto);
            for (int i = 0; i < paleta.CantidadElementos(figura); i++)
            {
                PointF[] contorno = ContornoElemento(figura, i);
                if (GeometriaPlana.Ubicar(new[] { contorno }, posicion) == UbicacionPunto.Fuera) continue;
                SeleccionarElemento(figura, i);
                if (ElementoSeleccionado != null) ElementoSeleccionado(this, new ElementoSeleccionadoEventArgs(figura, i));
                break;
            }
        }

        private void DibujarSeleccion(Graphics graphics, PointF centro)
        {
            if (figuraSeleccionada < 0 || elementoSeleccionado < 0) return;
            PointF[] contorno = ContornoElemento(figuraSeleccionada, elementoSeleccionado);
            if (contorno == null) return;
            PointF[] pantalla = ConvertirAPantalla(contorno, centro);
            using (var sombra = new Pen(Color.FromArgb(20, 29, 38), 4f))
            using (var borde = new Pen(Color.FromArgb(228, 246, 255), 1.5f))
            {
                graphics.DrawPolygon(sombra, pantalla);
                graphics.DrawPolygon(borde, pantalla);
            }
        }

        public void SuperponerAnalisis(PointF a, PointF b, PointF p)
        {
            ProyeccionSegmento resultado = GeometriaPlana.Proyectar(a, b, p);
            analisisA = a; analisisB = b; analisisP = p; analisisQ = resultado.Punto;
            analisisSuperpuesto = true;
            Invalidate();
        }

        private void DibujarReferencia(Graphics graphics, PointF centro)
        {
            if (MostrarCuadricula)
            using (var menor = new Pen(Color.FromArgb(34, 45, 56)))
            using (var mayor = new Pen(Color.FromArgb(44, 58, 70)))
            {
                for (float x = centro.X % 25; x < Width; x += 25)
                    graphics.DrawLine(Math.Abs((x - centro.X) % 100) < 1 ? mayor : menor, x, 0, x, Height);
                for (float y = centro.Y % 25; y < Height; y += 25)
                    graphics.DrawLine(Math.Abs((y - centro.Y) % 100) < 1 ? mayor : menor, 0, y, Width, y);
            }
            if (!MostrarEjes) return;
            using (var eje = new Pen(Color.FromArgb(145, 166, 185), 1f))
            using (var texto = new SolidBrush(Color.FromArgb(171, 188, 205)))
            {
                eje.EndCap = LineCap.ArrowAnchor;
                graphics.DrawLine(eje, 16, centro.Y, Width - 20, centro.Y);
                graphics.DrawLine(eje, centro.X, Height - 16, centro.X, 20);
                graphics.DrawString("+X", Font, texto, Width - 42, centro.Y + 8);
                graphics.DrawString("+Y", Font, texto, centro.X + 8, 18);
                graphics.DrawString("0", Font, texto, centro.X - 18, centro.Y + 8);
            }
        }

        private PointF ACoordenadas(Point punto)
        {
            return new PointF(punto.X - ClientSize.Width / 2f, ClientSize.Height / 2f - punto.Y);
        }

        private string EncontrarPunto(Point mouse)
        {
            PointF p = ACoordenadas(mouse);
            if (analisisSuperpuesto || contornosAnalisis != null)
            {
                if (GeometriaPlana.Distancia(p, analisisP) <= 11) return "P";
                if (GeometriaPlana.Distancia(p, analisisA) <= 11) return "A";
                if (GeometriaPlana.Distancia(p, analisisB) <= 11) return "B";
            }
            if (vectorA != null)
            {
                if (GeometriaPlana.Distancia(p, new PointF((float)vectorA.X, (float)vectorA.Y)) <= 11) return "A";
                if (GeometriaPlana.Distancia(p, new PointF((float)vectorB.X, (float)vectorB.Y)) <= 11) return "B";
            }
            return null;
        }

        private bool TocaFigura(Point mouse)
        {
            PointF[][] contornos = contornosAnalisis;
            if (hexagono != null) contornos = transformacion.Transformar(new[] { hexagono.ObtenerVertices() });
            else if (flor != null) contornos = transformacion.Transformar(flor.ObtenerPetalos());
            else if (estrella != null) contornos = transformacion.Transformar(estrella.ObtenerRecorridos());
            if (contornos == null) return false;
            PointF p = ACoordenadas(mouse);
            if (GeometriaPlana.Ubicar(contornos, p) != UbicacionPunto.Fuera) return true;
            foreach (PointF[] contorno in contornos)
                for (int i = 0; i < contorno.Length; i++)
                    if (GeometriaPlana.Proyectar(contorno[i], contorno[(i + 1) % contorno.Length], p).Distancia <= 6) return true;
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            inicioPulsacion = e.Location;
            movimientoDetectado = false;
            puntoActivo = EncontrarPunto(e.Location);
            arrastrandoFigura = puntoActivo == null && TocaFigura(e.Location);
            ultimaPosicion = e.Location;
            if (puntoActivo != null || arrastrandoFigura) { Capture = true; Cursor = Cursors.SizeAll; }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (PosicionCursorCambiada != null)
                PosicionCursorCambiada(this, new PosicionCursorEventArgs(ACoordenadas(e.Location)));
            if (e.Button == MouseButtons.Left)
            {
                if (!movimientoDetectado)
                {
                    Size umbral = SystemInformation.DragSize;
                    movimientoDetectado = Math.Abs(e.X - inicioPulsacion.X) >= Math.Max(2, umbral.Width / 2) ||
                        Math.Abs(e.Y - inicioPulsacion.Y) >= Math.Max(2, umbral.Height / 2);
                    if (!movimientoDetectado) return;
                }
                if (puntoActivo != null && PuntoMovido != null)
                    PuntoMovido(this, new PuntoMovidoEventArgs(puntoActivo, ACoordenadas(e.Location)));
                else if (arrastrandoFigura && DesplazamientoSolicitado != null)
                    DesplazamientoSolicitado(this, new DesplazamientoEventArgs(e.X - ultimaPosicion.X, ultimaPosicion.Y - e.Y));
                ultimaPosicion = e.Location;
            }
            else Cursor = EncontrarPunto(e.Location) != null || TocaFigura(e.Location) ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left && !movimientoDetectado && puntoActivo == null && arrastrandoFigura)
                SeleccionarPorClic(e.Location);
            puntoActivo = null; arrastrandoFigura = false; Capture = false; Cursor = Cursors.Default;
            if (!ClientRectangle.Contains(e.Location) && PosicionCursorCambiada != null)
                PosicionCursorCambiada(this, new PosicionCursorEventArgs(null));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!Capture && PosicionCursorCambiada != null) PosicionCursorCambiada(this, new PosicionCursorEventArgs(null));
        }
    }
}
