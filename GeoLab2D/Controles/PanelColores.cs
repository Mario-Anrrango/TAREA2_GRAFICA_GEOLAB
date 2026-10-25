using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Presentacion;

namespace GeoLab2D.Controles
{
    public sealed class PanelColores : UserControl
    {
        private PaletaFiguras paleta = PaletaFiguras.Predeterminada();
        private int figura = -1;
        private readonly ComboBox elemento;
        private readonly Button cambiar;
        private readonly Label mensaje;
        public event EventHandler PaletaCambiada;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PaletaFiguras Paleta { get { return paleta; } }

        public PanelColores()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16, 4, 16, 4);
            var tabla = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 1, RowCount = 2 };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var fila = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = true };
            fila.Controls.Add(new Label { Text = "Colores:", AutoSize = true, Margin = new Padding(0, 5, 6, 0) });
            elemento = new ComboBox { Name = "elementoColor", DropDownStyle = ComboBoxStyle.DropDownList, Width = 135 };
            cambiar = new BotonRedondeado { Name = "cambiarColor", Text = "Cambiar color…", AutoSize = true, UseVisualStyleBackColor = false };
            var restablecer = new BotonRedondeado { Name = "restablecerColores", Text = "Restablecer colores", AutoSize = true };
            mensaje = new Label { Name = "mensajeColor", AutoSize = true, Dock = DockStyle.Fill };
            fila.Controls.Add(elemento); fila.Controls.Add(cambiar); fila.Controls.Add(restablecer);
            tabla.Controls.Add(fila, 0, 0); tabla.Controls.Add(mensaje, 0, 1);
            Controls.Add(tabla);
            elemento.SelectedIndexChanged += (sender, e) => ActualizarMuestra();
            cambiar.Click += Cambiar_Click;
            restablecer.Click += (sender, e) => RestablecerColores();
            SeleccionarFigura(0);
        }

        public void SeleccionarFigura(int tipo)
        {
            int cantidad = paleta.CantidadElementos(tipo);
            if (figura == tipo) return;
            figura = tipo;
            elemento.Items.Clear();
            for (int i = 0; i < cantidad; i++)
                elemento.Items.Add(tipo == 0 ? "Sector " + (i + 1) : tipo == 1 ? "Pétalo " + (i + 1) : "Trazo de estrella");
            elemento.SelectedIndex = 0;
        }

        public bool AplicarColor(Color color)
        {
            try { paleta = paleta.ConColor(figura, elemento.SelectedIndex, color); }
            catch (ArgumentException error)
            {
                mensaje.Text = error.Message;
                mensaje.ForeColor = Color.Salmon;
                return false;
            }
            ActualizarMuestra();
            if (PaletaCambiada != null) PaletaCambiada(this, EventArgs.Empty);
            return true;
        }

        public void RestablecerColores()
        {
            paleta = paleta.Restablecer(figura);
            ActualizarMuestra();
            if (PaletaCambiada != null) PaletaCambiada(this, EventArgs.Empty);
        }

        private void ActualizarMuestra()
        {
            if (elemento.SelectedIndex < 0) return;
            Color color = paleta.ObtenerColor(figura, elemento.SelectedIndex);
            cambiar.BackColor = color;
            cambiar.ForeColor = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B < 140 ? Color.White : Color.Black;
            mensaje.ForeColor = TemaOscuro.Secundario;
            mensaje.Text = string.Format("RGB ({0}; {1}; {2}) | Los colores se conservan durante esta sesión.", color.R, color.G, color.B);
        }

        private void Cambiar_Click(object sender, EventArgs e)
        {
            using (var dialogo = new ColorDialog { Color = paleta.ObtenerColor(figura, elemento.SelectedIndex), FullOpen = true })
                if (dialogo.ShowDialog(this) == DialogResult.OK) AplicarColor(dialogo.Color);
        }
    }
}
