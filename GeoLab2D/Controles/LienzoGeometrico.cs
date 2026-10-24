using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using GeoLab2D.Geometria;

namespace GeoLab2D.Controles
{
    /// <summary>
    /// Presenta el modelo matemático sin calcular sus vértices en el formulario.
    /// </summary>
    public class LienzoGeometrico : Panel
    {
        private HexagonoRegular hexagono;
        private FlorMargarita flor;
        private PoligonoEstrellado estrella;
        private static readonly Color[] Colores =
        {
            Color.Coral, Color.Gold, Color.MediumSeaGreen,
            Color.DeepSkyBlue, Color.MediumPurple, Color.HotPink
        };

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HexagonoRegular Hexagono
        {
            get { return hexagono; }
            set { hexagono = value; flor = null; estrella = null; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FlorMargarita Flor
        {
            get { return flor; }
            set { flor = value; hexagono = null; estrella = null; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PoligonoEstrellado Estrella
        {
            get { return estrella; }
            set { estrella = value; hexagono = null; flor = null; Invalidate(); }
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
            if (hexagono == null && flor == null && estrella == null) return;

            GraphicsState estado = e.Graphics.Save();
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                PointF centro = new PointF(ClientSize.Width / 2f, ClientSize.Height / 2f);
                if (estrella != null)
                {
                    using (var pluma = new Pen(Color.BlueViolet, 2f))
                    {
                        foreach (PointF[] recorrido in estrella.ObtenerRecorridos())
                            e.Graphics.DrawPolygon(pluma, ConvertirAPantalla(recorrido, centro));
                    }
                    return;
                }
                if (flor != null)
                {
                    using (var pincel = new SolidBrush(Color.Red))
                    using (var pluma = new Pen(Color.DarkRed, 1f))
                    {
                        foreach (PointF[] petalo in flor.ObtenerPetalos())
                        {
                            PointF[] puntos = ConvertirAPantalla(petalo, centro);
                            e.Graphics.FillPolygon(pincel, puntos);
                            e.Graphics.DrawPolygon(pluma, puntos);
                        }
                    }
                    return;
                }
                PointF[] vertices = ConvertirAPantalla(hexagono.ObtenerVertices(), centro);

                using (var contorno = new Pen(Color.FromArgb(45, 45, 45), 1.5f))
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        PointF[] triangulo = { centro, vertices[i], vertices[(i + 1) % vertices.Length] };
                        using (var pincel = new SolidBrush(Colores[i]))
                            e.Graphics.FillPolygon(pincel, triangulo);
                    }
                    for (int i = 0; i < vertices.Length; i++)
                        e.Graphics.DrawLine(contorno, centro, vertices[i]);
                    e.Graphics.DrawPolygon(contorno, vertices);
                }
            }
            finally
            {
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
    }
}
