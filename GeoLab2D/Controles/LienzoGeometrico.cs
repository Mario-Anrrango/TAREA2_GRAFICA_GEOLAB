using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using GeoLab2D.Geometria;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    /// <summary>
    /// Presenta el modelo matemático sin calcular sus vértices en el formulario.
    /// </summary>
    public partial class LienzoGeometrico : Panel
    {
        private HexagonoRegular hexagono;
        private FlorMargarita flor;
        private PoligonoEstrellado estrella;
        private Vector2D vectorA, vectorB;
        private PointF[][] contornosAnalisis;
        private PointF analisisA, analisisB, analisisP, analisisQ;
        private Matriz3x3 transformacion = Matriz3x3.Identidad();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Matriz3x3 Transformacion
        {
            get { return transformacion; }
            set
            {
                if (value == null) throw new System.ArgumentNullException("value");
                transformacion = value;
                Invalidate();
            }
        }
        private PaletaFiguras paleta = PaletaFiguras.Predeterminada();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PaletaFiguras Paleta
        {
            get { return paleta; }
            set
            {
                if (value == null) throw new System.ArgumentNullException("value");
                paleta = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HexagonoRegular Hexagono
        {
            get { return hexagono; }
            set { hexagono = value; flor = null; estrella = null; vectorA = vectorB = null; contornosAnalisis = null; analisisSuperpuesto = false; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FlorMargarita Flor
        {
            get { return flor; }
            set { flor = value; hexagono = null; estrella = null; vectorA = vectorB = null; contornosAnalisis = null; analisisSuperpuesto = false; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PoligonoEstrellado Estrella
        {
            get { return estrella; }
            set { estrella = value; hexagono = null; flor = null; vectorA = vectorB = null; contornosAnalisis = null; analisisSuperpuesto = false; Invalidate(); }
        }

        public void MostrarVectores(Vector2D a, Vector2D b)
        {
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");
            vectorA = a;
            vectorB = b;
            hexagono = null;
            flor = null;
            estrella = null;
            contornosAnalisis = null;
            analisisSuperpuesto = false;
            Invalidate();
        }

        public void MostrarAnalisis(PointF[][] contornos, PointF a, PointF b, PointF p)
        {
            GeometriaPlana.Ubicar(contornos, p); // Validar también las llamadas directas al lienzo.
            ProyeccionSegmento proyeccion = GeometriaPlana.Proyectar(a, b, p);
            var copia = new PointF[contornos.Length][];
            for (int i = 0; i < contornos.Length; i++) copia[i] = (PointF[])contornos[i].Clone();
            contornosAnalisis = copia;
            analisisSuperpuesto = false;
            analisisA = a; analisisB = b; analisisP = p; analisisQ = proyeccion.Punto;
            hexagono = null; flor = null; estrella = null; vectorA = vectorB = null;
            Invalidate();
        }

        public LienzoGeometrico()
        {
            BackColor = Color.White;
            MinimumSize = new Size(800, 600);
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            // El escalado de fuentes del formulario no debe reducir el mínimo en píxeles.
            MinimumSize = new Size(800, 600);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (hexagono == null && flor == null && estrella == null && vectorA == null && contornosAnalisis == null) return;

            GraphicsState estado = e.Graphics.Save();
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                PointF centro = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);
                DibujarReferencia(e.Graphics, centro);
                if (contornosAnalisis != null)
                {
                    DibujarAnalisis(e.Graphics, centro);
                    return;
                }
                if (vectorA != null)
                {
                    DibujarVector(e.Graphics, centro, vectorA, Color.RoyalBlue, "A");
                    DibujarVector(e.Graphics, centro, vectorB, Color.DarkOrange, "B");
                    return;
                }
                if (estrella != null)
                {
                    using (var pluma = new Pen(paleta.ObtenerColor(2, 0), 2f))
                    {
                        foreach (PointF[] recorrido in estrella.ObtenerRecorridos())
                            e.Graphics.DrawPolygon(pluma, ConvertirFiguraAPantalla(recorrido, centro));
                    }
                    return;
                }
                if (flor != null)
                {
                    PointF[][] petalos = flor.ObtenerPetalos();
                    for (int i = 0; i < petalos.Length; i++)
                    {
                        Color color = paleta.ObtenerColor(1, i);
                        using (var pincel = new SolidBrush(color))
                        using (var pluma = new Pen(Color.FromArgb(color.R / 2, color.G / 2, color.B / 2), 1f))
                        {
                            PointF[] puntos = ConvertirFiguraAPantalla(petalos[i], centro);
                            e.Graphics.FillPolygon(pincel, puntos);
                            e.Graphics.DrawPolygon(pluma, puntos);
                        }
                    }
                    return;
                }
                PointF[] vertices = ConvertirFiguraAPantalla(hexagono.ObtenerVertices(), centro);
                PointF centroHexagono = ConvertirFiguraAPantalla(new[] { PointF.Empty }, centro)[0];

                using (var contorno = new Pen(Color.FromArgb(45, 45, 45), 1.5f))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        PointF[] triangulo = { centroHexagono, vertices[i], vertices[(i + 1) % vertices.Length] };
                        using (var pincel = new SolidBrush(paleta.ObtenerColor(0, i)))
                            e.Graphics.FillPolygon(pincel, triangulo);
                    }
                    for (int i = 0; i < vertices.Length; i++)
                        e.Graphics.DrawLine(contorno, centroHexagono, vertices[i]);
                    e.Graphics.DrawPolygon(contorno, vertices);
                }
            }
            finally
            {
                DibujarSeleccion(e.Graphics, new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f));
                if (analisisSuperpuesto)
                    DibujarPuntosAnalisis(e.Graphics, new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f));
                e.Graphics.Restore(estado);
            }
        }

        private static PointF[] ConvertirAPantalla(PointF[] vertices, PointF centro)
        {
            // Conversión de coordenadas cartesianas a pantalla: Y crece hacia abajo.
            var puntos = new PointF[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                puntos[i] = new PointF(centro.X + vertices[i].X, centro.Y - vertices[i].Y);
            return puntos;
        }

        private PointF[] ConvertirFiguraAPantalla(PointF[] vertices, PointF centro)
        {
            return ConvertirAPantalla(transformacion.Transformar(vertices), centro);
        }

        private void DibujarAnalisis(Graphics graphics, PointF centro)
        {
            using (var trayectoria = new GraphicsPath(FillMode.Alternate))
            using (var relleno = new SolidBrush(Color.FromArgb(35, Color.SteelBlue)))
            using (var borde = new Pen(Color.SlateGray, 1.5f))
            {
                foreach (PointF[] contorno in contornosAnalisis)
                    trayectoria.AddPolygon(ConvertirAPantalla(contorno, centro));
                graphics.FillPath(relleno, trayectoria);
                graphics.DrawPath(borde, trayectoria);
            }
            DibujarPuntosAnalisis(graphics, centro);
        }

        private void DibujarPuntosAnalisis(Graphics graphics, PointF centro)
        {
            PointF[] puntos = ConvertirAPantalla(new[] { analisisA, analisisB, analisisP, analisisQ }, centro);
            using (var segmento = new Pen(Color.RoyalBlue, 2.5f))
            using (var distancia = new Pen(Color.ForestGreen, 1.5f))
            {
                distancia.DashStyle = DashStyle.Dash;
                graphics.DrawLine(segmento, puntos[0], puntos[1]);
                graphics.DrawLine(distancia, puntos[2], puntos[3]);
            }
            DibujarPunto(graphics, puntos[0], Color.RoyalBlue, "A", -20, 8);
            DibujarPunto(graphics, puntos[1], Color.RoyalBlue, "B", 8, 8);
            DibujarPunto(graphics, puntos[2], Color.DarkOrange, "P", 8, -20);
            DibujarPunto(graphics, puntos[3], Color.ForestGreen, "Q", 8, 8);
        }

        private void DibujarPunto(Graphics graphics, PointF punto, Color color, string nombre, float dx, float dy)
        {
            using (var pincel = new SolidBrush(color))
            {
                graphics.FillEllipse(pincel, punto.X - 4, punto.Y - 4, 8, 8);
                graphics.DrawString(nombre, Font, pincel, punto.X + dx, punto.Y + dy);
            }
        }

        private void DibujarVector(Graphics graphics, PointF origen, Vector2D vector, Color color, string nombre)
        {
            PointF fin = new PointF(origen.X + (float)vector.X, origen.Y - (float)vector.Y);
            using (var pluma = new Pen(color, 3f))
            using (var pincel = new SolidBrush(color))
            {
                if (vector.Magnitud == 0)
                    graphics.FillEllipse(pincel, origen.X - 4, origen.Y - 4, 8, 8);
                else
                {
                    pluma.EndCap = LineCap.ArrowAnchor;
                    graphics.DrawLine(pluma, origen, fin);
                }
                graphics.DrawString(nombre, Font, pincel, fin.X + 8, fin.Y + (nombre == "A" ? -20 : 8));
            }
        }
    }
}
