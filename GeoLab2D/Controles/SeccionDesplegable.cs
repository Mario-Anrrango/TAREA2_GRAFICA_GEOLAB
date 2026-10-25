using System.Windows.Forms;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    public sealed class SeccionDesplegable : UserControl
    {
        private readonly Button titulo;
        private readonly Control contenido;
        private readonly string texto;
        private bool expandida = true;
        public bool Expandida { get { return expandida; } }

        public SeccionDesplegable(string texto, Control contenido)
        {
            this.texto = texto; this.contenido = contenido;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Dock = DockStyle.Top;
            Margin = new Padding(0, 0, 0, 10);
            Padding = new Padding(1);
            titulo = new Button { Dock = DockStyle.Top, Height = 34, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            contenido.Dock = DockStyle.Top;
            Controls.Add(contenido); Controls.Add(titulo);
            titulo.Click += (sender, e) => Alternar();
            TemaOscuro.Aplicar(this);
            ActualizarTitulo();
        }

        public void Alternar()
        {
            expandida = !expandida;
            contenido.Visible = expandida;
            ActualizarTitulo();
        }

        public void Mostrar(bool disponible)
        {
            Visible = disponible;
            contenido.Visible = expandida;
        }

        private void ActualizarTitulo() { titulo.Text = (expandida ? "▾   " : "▸   ") + texto; }
    }
}
