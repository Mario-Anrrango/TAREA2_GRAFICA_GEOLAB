using System;
using System.Drawing;
using System.Windows.Forms;
using GeoLab2D.Controles;
using GeoLab2D.Presentacion;

namespace GeoLab2D
{
    public partial class GeoLab2D
    {
        private Label resumenFigura;
        private CheckBox mostrarAnalisis;
        private SeccionDesplegable seccionColores, seccionTransformacion, seccionVectores, seccionAnalisis;
        private bool configurandoDiseno;

        private void ConfigurarDiseno()
        {
            configurandoDiseno = true;
            SuspendLayout();
            Font = TemaOscuro.FuenteTexto;
            BackColor = TemaOscuro.Fondo;
            ForeColor = TemaOscuro.Texto;
            ClientSize = new Size(1240, 800);
            MinimumSize = new Size(920, 660);
            encabezado.Height = 48;
            encabezado.Text = "GEOLAB 2D    /    Laboratorio de geometría";
            estado.Height = 44;
            contenedorHerramientas.Dock = DockStyle.Left;
            contenedorHerramientas.Width = 326;
            contenedorHerramientas.Padding = new Padding(12, 8, 12, 8);
            tablaHerramientas.SuspendLayout();
            tablaHerramientas.Controls.Clear();
            tablaHerramientas.RowStyles.Clear();
            tablaHerramientas.RowCount = 7;
            for (int i = 0; i < 7; i++) tablaHerramientas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            parametros.Controls.Clear();
            parametros.Padding = new Padding(10, 6, 10, 10);
            parametros.FlowDirection = FlowDirection.TopDown;
            parametros.WrapContents = false;
            selectorFigura.Width = 264;
            parametros.Controls.Add(selectorFigura);
            parametros.Controls.Add(FilaParametro(etiquetaLado, lado));
            var filaEstrella = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false };
            filaEstrella.Controls.Add(etiquetaPuntas); filaEstrella.Controls.Add(puntas);
            filaEstrella.Controls.Add(etiquetaSalto); filaEstrella.Controls.Add(salto);
            parametros.Controls.Add(filaEstrella);
            tablaHerramientas.Controls.Add(new SeccionDesplegable("GEOMETRÍA", parametros), 0, 0);
            seccionColores = new SeccionDesplegable("COLORES", panelColores);
            seccionTransformacion = new SeccionDesplegable("TRANSFORMACIONES", panelTransformaciones);
            seccionVectores = new SeccionDesplegable("VECTORES A / B", panelVectores);
            seccionAnalisis = new SeccionDesplegable("PUNTOS Y PROYECCIÓN", panelAnalisis);
            tablaHerramientas.Controls.Add(seccionColores, 0, 1);
            tablaHerramientas.Controls.Add(seccionTransformacion, 0, 2);
            tablaHerramientas.Controls.Add(seccionVectores, 0, 3);
            tablaHerramientas.Controls.Add(seccionAnalisis, 0, 4);
            resumenFigura = new Label { Name = "resumenFigura", AutoSize = true, Dock = DockStyle.Top,
                Padding = new Padding(10), MaximumSize = new Size(284, 0), MinimumSize = new Size(260, 0) };
            tablaHerramientas.Controls.Add(new SeccionDesplegable("RESULTADOS", resumenFigura), 0, 5);
            tablaHerramientas.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(278, 0),
                Padding = new Padding(8), Text = "Arrastra una figura para moverla. En análisis, arrastra A, B o P; en vectores, sus extremos." }, 0, 6);
            tablaHerramientas.ResumeLayout(true);

            var trabajo = new Panel { Name = "areaTrabajo", Dock = DockStyle.Fill, Padding = new Padding(0, 8, 12, 8) };
            var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 4, 0, 0) };
            barra.Controls.Add(new Label { Text = "LIENZO 2D", AutoSize = true, Margin = new Padding(0, 7, 24, 0) });
            var cuadricula = new CheckBox { Text = "Cuadrícula", Checked = true, AutoSize = true, Margin = new Padding(4, 6, 12, 0) };
            var ejes = new CheckBox { Text = "Ejes", Checked = true, AutoSize = true, Margin = new Padding(4, 6, 12, 0) };
            mostrarAnalisis = new CheckBox { Name = "mostrarAnalisis", Text = "Analizar figura", AutoSize = true, Margin = new Padding(4, 6, 12, 0) };
            barra.Controls.Add(cuadricula); barra.Controls.Add(ejes); barra.Controls.Add(mostrarAnalisis);
            contenedorLienzo.Parent = trabajo;
            trabajo.Controls.Add(barra);
            Controls.Add(trabajo);
            contenedorHerramientas.BringToFront();
            trabajo.BringToFront();
            TemaOscuro.Aplicar(this);
            BackColor = TemaOscuro.Fondo;
            encabezado.BackColor = TemaOscuro.Fondo;
            encabezado.ForeColor = TemaOscuro.Acento;
            encabezado.Font = TemaOscuro.FuenteCabecera;
            estado.BackColor = TemaOscuro.Fondo;
            estado.ForeColor = TemaOscuro.Secundario;
            contenedorLienzo.BackColor = TemaOscuro.Fondo;
            lienzo.BackColor = TemaOscuro.Lienzo;
            lienzo.ForeColor = TemaOscuro.Texto;
            lienzo.MostrarCuadricula = true; lienzo.MostrarEjes = true;
            cuadricula.CheckedChanged += (sender, e) => { lienzo.MostrarCuadricula = cuadricula.Checked; lienzo.Invalidate(); };
            ejes.CheckedChanged += (sender, e) => { lienzo.MostrarEjes = ejes.Checked; lienzo.Invalidate(); };
            mostrarAnalisis.CheckedChanged += (sender, e) => ActualizarFigura();
            contenedorLienzo.Resize += (sender, e) => AjustarLienzo();
            lienzo.DesplazamientoSolicitado += (sender, e) => panelTransformaciones.Desplazar(e.X, e.Y);
            lienzo.PuntoMovido += (sender, e) =>
            {
                if (selectorFigura.SelectedIndex == 3) panelVectores.MoverVector(e.Nombre, e.Punto);
                else panelAnalisis.MoverPunto(e.Nombre, e.Punto);
            };
            configurandoDiseno = false;
            ResumeLayout(true);
            AjustarLienzo();
        }

        private static Control FilaParametro(Label etiqueta, NumericUpDown numero)
        {
            var fila = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false };
            etiqueta.AutoSize = false; etiqueta.Width = 170; etiqueta.Height = 25;
            numero.Width = 86;
            fila.Controls.Add(etiqueta); fila.Controls.Add(numero);
            return fila;
        }

        private void AjustarLienzo()
        {
            if (configurandoDiseno) return;
            lienzo.Size = new Size(Math.Max(800, contenedorLienzo.ClientSize.Width - 32),
                Math.Max(600, contenedorLienzo.ClientSize.Height - 32));
        }
    }
}
