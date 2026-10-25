using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
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
        private bool analisisSuperpuesto, arrastrandoFigura;
        private string puntoActivo;
        private Point ultimaPosicion;

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
            puntoActivo = EncontrarPunto(e.Location);
            arrastrandoFigura = puntoActivo == null && TocaFigura(e.Location);
            ultimaPosicion = e.Location;
            if (puntoActivo != null || arrastrandoFigura) { Capture = true; Cursor = Cursors.SizeAll; }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
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
            puntoActivo = null; arrastrandoFigura = false; Capture = false; Cursor = Cursors.Default;
        }
    }
}
