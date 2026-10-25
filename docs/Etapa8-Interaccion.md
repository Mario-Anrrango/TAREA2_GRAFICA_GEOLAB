# Etapa 8: colores, interacción y validaciones

## Edición de colores

En los modos de construcción se muestra el panel «Colores»:

1. Seleccionar el sector del hexágono, el pétalo de la margarita o el trazo de la estrella.
2. Pulsar «Cambiar color…» y elegir un color en el diálogo de Windows.
3. Confirmar para actualizar el dibujo; cancelar conserva el color anterior.

El botón refleja el color actual y se muestran sus componentes RGB. El texto
del botón cambia entre blanco y negro según la luminosidad del fondo.
Los colores se conservan al alternar entre figuras durante la sesión.
No se guardan en archivos ni entre ejecuciones de la aplicación.

«Restablecer colores» recupera solamente los colores de la figura seleccionada.
No modifica las otras figuras, las dimensiones ni las transformaciones.

El hexágono debe mantener seis colores distintos: elegir el color de otro sector
se rechaza con un mensaje y se conserva el dibujo anterior. Se comparan valores
ARGB, de modo que un color definido por RGB no evita esta validación.
Los pétalos pueden compartir color; por defecto los cinco son rojos.
Los colores deben ser opacos. Las estrellas conservan su representación sin relleno.

En vectores y análisis geométrico el editor de colores se oculta. Esos modos
conservan los colores de referencia de A, B, P y Q, y el análisis mantiene su
relleno neutro par-impar. Los colores editados se recuperan al regresar al modo
de construcción de cada figura.

## Entradas numéricas

Las entradas usan un control derivado de NumericUpDown que valida el texto
al confirmarlo con Enter, al validarse el control o cuando se consulta su valor
editado. Comprueba número, rango y cantidad de decimales admitidos.
Se usa el separador decimal de la configuración regional de Windows.
No se admiten separadores de miles ni notación científica.

Si el texto no es válido, se recupera el último valor válido y aparece un icono
de error con una explicación del rango, precisión y separador decimal.
El aviso no abre ventanas ni bloquea el foco. Una confirmación válida o un cambio
válido con las flechas limpia el error. Los cuadros desplegables solo admiten
opciones existentes.

Las ayudas al pasar el puntero por las entradas describen el rango y la precisión.
Las ayudas de puntas y salto explican el máximo de 64 y el límite dinámico del salto.
Al reducir las puntas se conserva el ajuste automático del salto a su nuevo máximo.

No se modificaron los rangos establecidos en las fases anteriores:

| Control | Rango | Decimales |
| --- | --- | --- |
| Lado/radio base | 1 a 280 px | 1 |
| Puntas | 5 a 64 | 0 |
| Salto | 2 a floor((N-1)/2) | 0 |
| Componentes de vectores y puntos | -250 a 250 px | 2 |
| Tx y Ty | -250 a 250 px | 2 |
| Ángulo | -360 a 360 grados | 2 |
| Sx y Sy | 0.10 a 3.00 | 2 |

Estas restricciones de interfaz complementan las validaciones de las clases
matemáticas; no sustituyen sus comprobaciones de parámetros.

## Distribución de la ventana

Las herramientas se organizan en filas que se adaptan al ancho disponible.
Su contenedor tiene desplazamiento vertical independiente, y limita su altura
para reservar espacio al área de dibujo al reducir la ventana.
El lienzo conserva su mínimo de 800 × 600 y sus propias barras de desplazamiento.
Se preservan los controles de posición, ángulo, escalas, matriz y restablecimiento
de la fase anterior. El aviso de recorte de la figura sigue activo.

## Separación de responsabilidades

- `Presentacion/PaletaFiguras.cs`: paleta inmutable, validación de colores y
  restablecimiento por figura. No modifica las clases de geometría.
- `Controles/PanelColores.cs`: selección del elemento, diálogo ColorDialog,
  muestra RGB y mensajes de validación.
- `Controles/EntradaNumericaValidada.cs`: validación del texto y conservación
  del último valor correcto.
- `GeoLab2D.cs`: conecta la paleta, ErrorProvider, ToolTip y distribución de herramientas.
- `Controles/LienzoGeometrico.cs`: utiliza los colores seleccionados al repintar.

La paleta devuelve instancias nuevas al editar o restablecer, de modo que el
rechazo de un color no deja cambios parciales. El diálogo se libera al cerrarse.
ErrorProvider y ToolTip pertenecen al contenedor de componentes del formulario
y se liberan con él.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Además de las pruebas anteriores:

- Editar la paleta no modifica una instancia anterior.
- Se rechazan colores repetidos del hexágono, incluso expresados con ARGB.
- Se rechazan colores transparentes o vacíos e índices fuera de rango.
- Un sector y un pétalo editados aparecen con el color nuevo en el renderizado.
- Alternar figuras conserva los colores; restablecer una figura no cambia las otras.
- El control numérico rechaza texto vacío, letras, valores fuera de rango y exceso
  de decimales, conservando el valor anterior y produciendo un aviso.
- Se aceptan límites válidos y decimales con el separador regional, y se rechazan
  separadores de miles. La edición inválida se simula sobre la caja de texto interna,
  como ocurre al escribir; no mediante el setter programático Text de NumericUpDown.
- Se comprueban 64 puntas, salto 31 y su ajuste a 2 al reducir a 5 puntas.
- Una escala cero introducida como texto no altera la matriz.
- En ventana reducida se mantiene un área accesible para el lienzo y su mínimo.

Las imágenes de prueba se generan bajo `Pruebas/bin/Debug`, omitido por Git.
El diálogo nativo de colores, el foco y la edición en el diseñador requieren la
revisión manual siguiente; las pruebas automáticas no interactúan con ese diálogo.

## Revisión manual

1. Ejecutar con F5 y cambiar el color de varios sectores del hexágono.
2. Intentar asignar a otro sector un color ya usado: debe mostrarse el aviso sin cambiar el dibujo.
3. Abrir el diálogo de color y cancelar: no debe cambiar el color anterior.
4. Cambiar pétalos y trazo de estrella; alternar figuras y comprobar que se conservan.
5. Restablecer colores en una figura y verificar que las otras conservan su edición.
6. Escribir letras, un número fuera de rango o un exceso de decimales en una entrada;
   pulsar Enter o salir del control. Verificar recuperación y aviso.
7. Corregir con un valor válido y comprobar que desaparece el aviso.
8. Probar la estrella de 64 puntas y los controles de transformación y análisis.
9. Reducir, maximizar, minimizar y restaurar la ventana; recorrer herramientas
   y lienzo con sus barras y comprobar que el dibujo se repinta.
10. Abrir el formulario en el diseñador de Visual Studio para verificar su edición.

La fase final consolidará el informe técnico, las evidencias de pruebas y el
guion para grabar el video demostrativo de hasta cinco minutos.
