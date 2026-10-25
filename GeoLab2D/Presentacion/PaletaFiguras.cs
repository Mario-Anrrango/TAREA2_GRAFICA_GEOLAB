using System;
using System.Drawing;

namespace GeoLab2D.Presentacion
{
    /// <summary>Paleta inmutable. Cambiar colores no modifica la geometría.</summary>
    public sealed class PaletaFiguras
    {
        private readonly Color[][] colores;

        private PaletaFiguras(Color[][] colores)
        {
            this.colores = new Color[colores.Length][];
            for (int i = 0; i < colores.Length; i++) this.colores[i] = (Color[])colores[i].Clone();
        }

        public static PaletaFiguras Predeterminada()
        {
            return new PaletaFiguras(new[]
            {
                new[] { Color.Coral, Color.Gold, Color.MediumSeaGreen, Color.DeepSkyBlue, Color.MediumPurple, Color.HotPink },
                new[] { Color.Red, Color.Red, Color.Red, Color.Red, Color.Red },
                new[] { Color.BlueViolet }
            });
        }

        public int CantidadElementos(int figura)
        {
            if (figura < 0 || figura >= colores.Length) throw new ArgumentOutOfRangeException("figura");
            return colores[figura].Length;
        }

        public Color ObtenerColor(int figura, int elemento)
        {
            if (elemento < 0 || elemento >= CantidadElementos(figura)) throw new ArgumentOutOfRangeException("elemento");
            return colores[figura][elemento];
        }

        public PaletaFiguras ConColor(int figura, int elemento, Color color)
        {
            ObtenerColor(figura, elemento); // Validar índices antes de copiar.
            if (color.IsEmpty || color.A != 255) throw new ArgumentException("Seleccione un color opaco.", "color");
            if (figura == 0)
                for (int i = 0; i < colores[0].Length; i++)
                    if (i != elemento && colores[0][i].ToArgb() == color.ToArgb())
                        throw new ArgumentException("El hexágono debe conservar seis colores diferentes.", "color");
            var nueva = new PaletaFiguras(colores);
            nueva.colores[figura][elemento] = color;
            return nueva;
        }

        public PaletaFiguras Restablecer(int figura)
        {
            CantidadElementos(figura);
            var nueva = new PaletaFiguras(colores);
            nueva.colores[figura] = (Color[])Predeterminada().colores[figura].Clone();
            return nueva;
        }
    }
}
