using System.Drawing;
using System.Windows.Forms;

namespace GeoLab2D.Controles
{
    /// <summary>
    /// Superficie de presentación. Los cálculos de las figuras se añadirán
    /// en clases independientes del formulario y de este control.
    /// </summary>
    public class LienzoGeometrico : Panel
    {
        public LienzoGeometrico()
        {
            BackColor = Color.White;
            MinimumSize = new Size(800, 600);
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }
}
