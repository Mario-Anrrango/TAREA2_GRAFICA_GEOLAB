# Selección por clic y coordenadas del cursor

## Uso

- Hacer clic sobre un sector del hexágono o un pétalo de la margarita selecciona
  ese elemento en el panel de colores. Un borde claro identifica la selección.
- Si la sección Colores estaba plegada, el clic la abre para mostrar el elemento elegido.
- Pulsar «Cambiar color…» para editarlo. Seleccionar no cambia el color por sí solo.
- El desplegable y la selección del lienzo se mantienen sincronizados.
- En la estrella se puede seleccionar su trazo único; sus cruces no son elementos de color separados.
- Un clic fuera de la figura o en el hueco central de la margarita conserva la selección anterior.

## Clic y arrastre

Se utiliza el umbral de movimiento del sistema para distinguir un clic de un
arrastre. Un clic no cambia Tx o Ty. Si se arrastra, la selección se conserva
y la figura sigue moviéndose con el mecanismo anterior.
Los marcadores A, B y P y los extremos de los vectores conservan prioridad
cuando el puntero está sobre ellos, incluso si hay una figura debajo.

La selección compara el punto con los triángulos o pentágonos ya transformados,
por lo que funciona después de trasladar, rotar o escalar. Si un clic cae justo
en un borde compartido del hexágono, se elige el primer sector que contiene ese
borde, en el orden de numeración.

## Coordenadas

El indicador al pie muestra X e Y mientras el cursor recorre el lienzo, también
en los modos de vectores y análisis. Al salir se muestran guiones.

```text
x = posición horizontal del ratón - ancho del lienzo / 2
y = alto del lienzo / 2 - posición vertical del ratón
```

El origen está en el centro y +Y apunta hacia arriba. Son coordenadas del lienzo,
no coordenadas locales de la figura antes de transformarse. Una unidad equivale
a un píxel del lienzo. Las barras de desplazamiento no cambian esa referencia.

## Comprobación y demostración

Las pruebas de diseño incluyen clics en sectores, ausencia de traslación por clic,
conservación de selección al arrastrar, sectores y pétalos transformados,
rechazo del hueco y del exterior, actualización y limpieza de coordenadas.

Para el video: señalar el origen, mover el cursor hacia arriba y a la derecha
para mostrar coordenadas positivas, hacer clic en un sector y cambiar su color,
arrastrar la figura y seleccionar otro sector después de rotarla. Repetir con
un pétalo y comprobar que el hueco central no selecciona ninguno.
