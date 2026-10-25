using System;
using System.Globalization;
using System.Windows.Forms;

namespace GeoLab2D.Controles
{
    /// <summary>Rechaza texto inválido sin perder el último valor válido ni bloquear el foco.</summary>
    public sealed class EntradaNumericaValidada : NumericUpDown
    {
        public string ErrorEntrada { get; private set; }
        public event EventHandler ValidacionCambiada;

        public void ConfirmarEntrada()
        {
            ValidateEditText();
        }

        protected override void ValidateEditText()
        {
            decimal numero;
            NumberStyles estilo = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
            bool valido = decimal.TryParse(Text, estilo, CultureInfo.CurrentCulture, out numero) &&
                numero >= Minimum && numero <= Maximum && decimal.Round(numero, DecimalPlaces) == numero;
            if (!valido)
            {
                // Evitar que consultar Value vuelva a intentar validar el texto inválido.
                UserEdit = false;
                UpdateEditText();
                ErrorEntrada = string.Format("Introduzca un número entre {0} y {1}, con hasta {2} decimales. Separador: {3}",
                    Minimum, Maximum, DecimalPlaces, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
            else
            {
                ErrorEntrada = null;
                base.ValidateEditText();
            }
            NotificarValidacion();
        }

        protected override void OnValueChanged(EventArgs e)
        {
            ErrorEntrada = null;
            base.OnValueChanged(e);
            NotificarValidacion();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConfirmarEntrada();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            base.OnKeyDown(e);
        }

        private void NotificarValidacion()
        {
            if (ValidacionCambiada != null) ValidacionCambiada(this, EventArgs.Empty);
        }
    }
}
