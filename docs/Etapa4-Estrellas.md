# Etapa 4: polígonos estrellados configurables

## Construcción

Se generan los vértices de un polígono regular y se conectan con un salto K.
Esta construcción por diagonales permite tanto una estrella como varias figuras
entrelazadas, según los parámetros. Sigue el enfoque de polígonos y diagonales
de los ejemplos de la unidad; no reproduce toda la composición fija del decágono.

```text
N = número de puntas o vértices del polígono base
R = radio exterior
K = salto entre índices
θᵢ = π/2 + i · 2π/N
Vᵢ = (R cos θᵢ, R sen θᵢ), i = 0, ..., N-1
Siguiente índice = (índice + K) módulo N
```

La primera punta mira hacia arriba. Las coordenadas se calculan con seno y
coseno, y el lienzo aplica la conversión cartesiana a pantalla usada en las
otras figuras. El parámetro de tamaño de la estrella es el radio, en píxeles.

## Cierre y composición

El máximo común divisor `d = MCD(N,K)` indica cuántos recorridos cerrados existen.
Cada recorrido contiene `N/d` vértices. Los índices iniciales de los recorridos
son `0, ..., d-1`, y sus vértices se obtienen con `(inicio + jK) % N`.
Se cierra cada recorrido conectando su último vértice con el primero.
Así se usan todos los vértices sin unir artificialmente las componentes.

| N | K | Recorridos | Figura |
| --- | --- | --- | --- |
| 5 | 2 | 1 | Pentagrama |
| 6 | 2 | 2 | Dos triángulos entrelazados |
| 8 | 3 | 1 | Estrella de ocho puntas |
| 10 | 4 | 2 | Dos pentagramas entrelazados |
| 16 | 6 | 2 | Dos estrellas de ocho puntas |

N se refiere a las puntas exteriores de la composición. Las intersecciones
interiores de las diagonales no se cuentan como vértices del polígono base.

## Longitud de los trazos

La longitud de cada segmento es la cuerda que subtiende un ángulo de `2πK/N`:

```text
Longitud de un segmento = 2R sen(πK/N)
Longitud total = 2NR sen(πK/N)
```

La estrella se dibuja en azul violeta sin relleno. La longitud mostrada es la
suma de sus segmentos, no el perímetro de la frontera de una región rellena.
El área no se presenta en esta etapa: para polígonos con intersecciones, el área
algebraica y el área de la región visible pueden diferir y requieren una
definición de relleno explícita.

## Parámetros y validación

- Interfaz: de 5 a 32 puntas y radio de 1 a 280 px con un decimal.
- Modelo: de 5 a 128 puntas; radio positivo finito.
- Salto entero desde 2 hasta `floor((N-1)/2)`.
- No se usa K = 1, que dibujaría el polígono convexo, ni K = N/2, que produciría
  recorridos degenerados de dos vértices. Los saltos mayores equivalen a invertir
  el sentido de otros recorridos ya admitidos.
- Al reducir N, el control ajusta automáticamente un salto que exceda su nuevo máximo.
- Los controles de puntas y salto se habilitan solamente al seleccionar la estrella.

R ≤ 280 deja al menos 20 px de margen vertical en el lienzo de 800 × 600.
Cambiar el valor de tamaño actualiza el radio de la estrella, el lado exterior
de la margarita o el lado del hexágono, según el texto de la etiqueta.

## Responsabilidades

`Geometria/PoligonoEstrellado.cs` valida parámetros, genera vértices y recorridos,
calcula el MCD y la longitud total, sin depender de Windows Forms.
Los métodos devuelven arreglos nuevos. El formulario coordina los controles y
el lienzo dibuja cada recorrido con `Graphics.DrawPolygon`, `Pen` y `PointF`.
Las tres figuras siguen disponibles en el selector.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Además de las pruebas anteriores:

- Se recorren todas las combinaciones admitidas por la interfaz de N y K,
  con radios 1, 150.5 y 280.
- Se comprueba que cada vértice se visita una vez y que cada recorrido se cierra
  con el salto correcto, sin segmentos que unan componentes diferentes.
- Se verifica la equidistancia al origen y la simetría de `360°/N`.
- La suma de distancias se compara con la fórmula de longitud total.
- Para N = 5, K = 2 y R = 10, la longitud total es aproximadamente 95.10565163.
- Para N = 6, K = 2 y R = 10, se forman dos triángulos y la longitud es 103.92304845.
- Se rechazan radios no positivos, NaN e infinitos, y números de puntas o saltos inválidos.
- Se comprueba el límite de 128 puntas del modelo y la independencia de los arreglos.
- Se prueba la selección de estrella, la edición de parámetros, el ajuste del salto
  al bajar de 32 a 5 puntas y el regreso a las otras figuras.
- Se renderiza una estrella de 8 puntas, se comprueba la presencia de trazos
  azul violeta y se guarda una imagen en la carpeta `bin` de pruebas, omitida por Git.

Se admite una tolerancia de 0.0001 px en posiciones y radios, y 0.02 px en la suma
de longitudes, por el redondeo de `PointF`.

## Revisión manual

1. Ejecutar con F5 y seleccionar «Polígono estrellado».
2. Probar N = 5, K = 2 y R = 150: debe aparecer un pentagrama.
3. Probar N = 8, K = 3 y N = 10, K = 4; comprobar la forma y los trazos cerrados indicados.
4. Aumentar a N = 32, K = 15 y volver a N = 5: K debe ajustarse a 2 sin errores.
5. Probar radios 1 y 280, minimizar y restaurar; comprobar que se repinta la figura.
6. Alternar entre las tres figuras y comprobar las etiquetas de tamaño y los resultados.
7. Reducir la ventana para revisar el ajuste de filas y el desplazamiento del lienzo.
8. Abrir el diseñador de Visual Studio para verificar su edición.

La siguiente etapa será el cálculo interactivo de operaciones vectoriales.
