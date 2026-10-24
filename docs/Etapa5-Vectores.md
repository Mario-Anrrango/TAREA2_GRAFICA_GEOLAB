# Etapa 5: operaciones vectoriales

## Uso

Seleccionar «Operaciones vectoriales» y editar A.x, A.y, B.x y B.y.
Los resultados y las flechas se actualizan automáticamente. A se dibuja en azul
y B en naranja, ambos desde el origen situado en el centro del lienzo.
Las componentes positivas de Y apuntan hacia arriba.

Las entradas admiten de -250 a 250 píxeles, con dos decimales. Este rango permite
mostrar ambos extremos dentro del lienzo mínimo, con margen para las flechas.
Una unidad equivale a un píxel. El tamaño del lienzo, el radio máximo de las
figuras y el máximo de 64 puntas de la estrella se conservan.
Los controles de tamaño y parámetros de las figuras se deshabilitan en este modo.
Al volver a los vectores se conservan las componentes introducidas.

## Fundamentos

Para A = (ax, ay) y B = (bx, by):

```text
|A| = √(ax² + ay²)
|B| = √(bx² + by²)
θA = atan2(ay, ax) · 180 / π
θB = atan2(by, bx) · 180 / π
A · B = ax · bx + ay · by
det(A, B) = ax · by - ay · bx
```

Las direcciones se miden desde +X en sentido antihorario. Si `atan2` produce un
ángulo negativo se suman 360°, obteniendo una dirección en [0°, 360°).
La interfaz redondea a dos decimales. Una dirección muy próxima a 360° puede
mostrarse como 360.00° por ese redondeo.

El vector (0, 0) tiene magnitud cero pero no tiene dirección. El modelo devuelve
`null` para su dirección y la interfaz muestra «no definida (vector cero)».
Se representa mediante un punto en el origen.

El producto punto es cero para vectores no nulos perpendiculares, positivo para
un ángulo menor de 90° y negativo para uno mayor de 90°. El determinante es cero
para vectores paralelos, incluyendo el caso de un vector cero. Su signo indica
el sentido del giro de A a B en coordenadas cartesianas, y su valor absoluto
equivale al área del paralelogramo que generan.

Las magnitudes se expresan en px y los productos y determinantes en px².
Estos cálculos utilizan las componentes cartesianas, antes de invertir Y para dibujar.

## Organización y validación

- `Geometria/Vector2D.cs`: vector inmutable con magnitud, dirección, producto punto
  y determinante. No depende de Windows Forms ni del lienzo.
- `Controles/PanelVectores.cs`: entradas y presentación de resultados usando el modelo.
- `GeoLab2D.cs`: selección del modo y coordinación del panel con el lienzo.
- `Controles/LienzoGeometrico.cs`: ejes y flechas con `Graphics`, `Pen`, `Brush` y `PointF`.

El modelo acepta componentes `float` finitas y almacena y opera en `double` para
evitar desbordar al multiplicarlas o elevarlas al cuadrado. Rechaza NaN e infinitos.
Producto punto y determinante rechazan un segundo vector nulo con
`ArgumentNullException`. El rango del modelo es independiente del control de entrada.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Debe terminar con código 0.

| A | B | Resultados esperados |
| --- | --- | --- |
| (3, 4) | (-4, 3) | Ambas magnitudes = 5; A·B = 0; det(A,B) = 25 |
| (3, 4) | (-3, -4) | A·B = -25 |
| (3, 4) | (6, 8) | det(A,B) = 0 |
| (0, 0) | (3, 4) | Magnitud de A = 0, dirección no definida, producto y determinante = 0 |
| (1, 0), (0, 1), (-1, 0), (0, -1) | — | Direcciones 0°, 90°, 180° y 270° |
| (-1, 1), (-1, -1), (1, -1) | — | Direcciones 135°, 225° y 315° |

También se comprueban la antisimetría del determinante, las entradas no finitas,
el segundo vector nulo y la ausencia de desbordamiento con componentes float máximas.
La tolerancia para los casos conocidos es 0.000001.

Las pruebas de interfaz modifican componentes, comprueban los resultados y el
mensaje del vector cero, verifican el dibujo de ambos colores y prueban el cambio
de modo. Se confirma además que la estrella admite 64 puntas. Se guardan los
vectores renderizados en `Pruebas/bin/Debug/vectores-verificados.png`, omitido por Git.
Se ejecutan también las pruebas anteriores de las tres figuras.

## Revisión manual

1. Ejecutar con F5 y seleccionar «Operaciones vectoriales».
2. Introducir A = (3, 4), B = (-4, 3); comprobar magnitudes 5, producto 0 y determinante 25.
3. Cambiar A a (0, 0); comprobar el mensaje de dirección no definida.
4. Probar componentes negativas y decimales, y los extremos -250 y 250.
5. Minimizar y restaurar; comprobar que se conservan las flechas y los valores.
6. Reducir la ventana; comprobar que las entradas se acomodan en filas y que el
   lienzo puede recorrerse con las barras de desplazamiento.
7. Alternar entre las tres figuras y los vectores; comprobar las etiquetas y controles.
8. Abrir el formulario en el diseñador de Visual Studio para verificar su edición.

La siguiente etapa añadirá análisis geométrico y proyección sobre segmentos.
