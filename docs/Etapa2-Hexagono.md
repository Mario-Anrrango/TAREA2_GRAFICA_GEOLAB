# Etapa 2: hexágono regular de seis colores

## Construcción matemática

Sea `L > 0` el lado. El radio de la circunferencia circunscrita es `R = L`.
Los seis vértices se obtienen con un ciclo, sin coordenadas precalculadas:

```text
θᵢ = i · π / 3, para i = 0, 1, ..., 5
xᵢ = L · cos(θᵢ)
yᵢ = L · sen(θᵢ)
```

Cada sector usa el origen y dos vértices consecutivos. Su ángulo central es
60° y sus tres lados miden L, por lo que es un triángulo equilátero.
El último sector conecta el vértice 5 con el 0 mediante `(i + 1) % 6`.

```text
P = 6L
A = 6 · (√3 / 4) · L² = (3√3 / 2) · L²
```

El modelo usa coordenadas cartesianas. Al dibujar se convierte cada punto a
pantalla: `X = ancho/2 + x`, `Y = alto/2 - y`. Esta conversión centra la figura
y adapta el eje Y; las transformaciones matriciales serán otra etapa.
Una unidad del modelo equivale a un píxel del lienzo.

## Responsabilidades

- `Geometria/HexagonoRegular.cs`: valida el lado y calcula vértices, área y perímetro.
  No depende de Windows Forms y devuelve un arreglo nuevo de `PointF` en cada consulta.
- `Controles/LienzoGeometrico.cs`: dibuja mediante `Graphics`, `Pen` y `SolidBrush`
  durante `OnPaint`. Libera los pinceles y la pluma con `using` y repinta con `Invalidate`.
- `GeoLab2D.cs`: recibe el lado de la interfaz y actualiza el modelo y los resultados.

El control numérico admite de 1 a 280, con un decimal. El límite de la interfaz
permite mostrar la figura completa en el lienzo mínimo: ancho máximo 560 y alto
aproximado 484.97 píxeles. El modelo admite cualquier lado positivo finito.
Los seis colores son distintos y fijos en esta etapa; su edición queda pendiente.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` con MSBuild o Visual Studio y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. No requiere paquetes externos.
El proceso devuelve 0 si todas las comprobaciones pasan y 1 si alguna falla.

| Caso | Resultado esperado |
| --- | --- |
| L = 1, 10, 150, 280 | Seis vértices; seis lados y radios de longitud L; simetría central |
| L = 10 | Perímetro = 60; área ≈ 259.807621135 |
| Área a partir de vértices | Coincide con la fórmula analítica usando el método del cordón |
| L = 0, -1, NaN, ±infinito | `ArgumentOutOfRangeException` |
| Modificar un arreglo de vértices | Las consultas posteriores permanecen intactas |
| Cambiar el lado en el formulario | Se actualiza el modelo del lienzo |
| Renderizar el lienzo | Se verifican los seis colores en puntos interiores de los sectores |

Las comparaciones admiten tolerancia por el redondeo de `PointF`: 0.0001 para
longitudes individuales, 0.001 para perímetro y 0.02 para área calculada con vértices.
La prueba guarda una imagen en su carpeta de salida, que Git omite.

## Revisión manual

1. Ejecutar con F5: aparece un hexágono de lado 150 con seis sectores de distinto color.
2. Cambiar el lado a 10: comprobar P = 60 y A ≈ 259.81 en el estado.
3. Probar 1, 150.5 y 280: la figura y los resultados se actualizan al confirmar la entrada.
4. Minimizar y restaurar: el dibujo se conserva al repintarse.
5. Reducir la ventana: usar las barras para recorrer el lienzo completo.
6. Abrir el diseñador de Visual Studio y verificar que permite editar el formulario.

## Alcance pendiente

La siguiente etapa es la margarita de cinco pentágonos. Siguen pendientes las
estrellas, operaciones vectoriales, análisis general de polígonos, proyecciones,
matrices, edición de colores/posición/ángulos y entregables finales.
