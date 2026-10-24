# Etapa 3: flor de margarita de cinco pentágonos

## Referencia y parámetro

Se sigue la disposición de la figura 1.2.1 del capítulo proporcionado, página 84:
cinco pentágonos rojos dentro de un pentágono mayor y un pentágono vacío central.
El pentágono exterior es una construcción auxiliar y no se dibuja.
El parámetro de entrada es su lado `L`, no el lado de cada pétalo.

## Fórmulas de construcción

La razón áurea es `φ = (1 + √5) / 2`. En esta disposición, el lado del pétalo es
`s = L / φ²`. Los pétalos y el hueco central son pentágonos regulares de lado s.

```text
r = s / (2 sen(π/5))       radio circunscrito de cada pétalo
a = r cos(π/5)            apotema de cada pétalo
θᵢ = π/2 + i · 2π/5       i = 0, 1, 2, 3, 4
Cᵢ = (2a cos θᵢ, 2a sen θᵢ)
Vᵢⱼ = Cᵢ + r (cos(θᵢ + j·2π/5), sen(θᵢ + j·2π/5))
                           j = 0, 1, 2, 3, 4
```

Cada pétalo comparte un lado con el pentágono central imaginario. Sus centros
están a dos apotemas del origen; los pétalos consecutivos se tocan en un vértice
sin superponer sus interiores. Los centros se distribuyen cada 72° y cada pétalo
tiene cinco lados y cinco ángulos iguales. La primera punta mira hacia arriba.

El radio hasta las cinco puntas exteriores es `R = 2a + r = φ²r`.
Por ello el lado del pentágono exterior es `2R sen(π/5) = φ²s = L`.
Esta relación justifica la escala utilizada y la inscripción de la flor.
Los puntos se calculan con trigonometría, sin listas de coordenadas fijas.
La conversión a pantalla conserva la misma regla de la etapa anterior.

```text
Perímetro de un pétalo = 5s
Área de un pétalo = 5s² / (4 tan(π/5))
Área roja total = 5 · Área de un pétalo
```

El área roja excluye el hueco central. El perímetro de un pétalo no debe
confundirse con el contorno de toda la composición. La interfaz muestra el
lado exterior, el lado derivado de cada pétalo y el área roja total.

## Diseño e interacción

- `FlorMargarita` calcula la geometría sin depender de Windows Forms.
- `ObtenerPetalos()` devuelve cinco arreglos nuevos de cinco `PointF`.
- El lienzo pinta los cinco polígonos en rojo durante `OnPaint`.
- El selector permite alternar entre hexágono y margarita y mantiene el valor de entrada.
- El lado exterior admite de 1 a 280 px con un decimal; el modelo rechaza cero,
  valores negativos, NaN e infinitos.
- Con L = 280, R ≈ 238.18 px: la flor cabe en el lienzo mínimo de 800 × 600.

La selección de figura está separada de las transformaciones matriciales,
que se implementarán en una etapa posterior.

## Pruebas

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar el programa de pruebas.
Se conservan las comprobaciones del hexágono y se añaden:

| Caso | Resultado esperado |
| --- | --- |
| L = 1, 10, 150.5 y 280 | Cinco pentágonos con cinco lados iguales y vértices equidistantes de su centro |
| Simetría | Girar un pétalo 72° alrededor del origen produce el siguiente |
| Inscripción | Todos los vértices quedan dentro del pentágono exterior y las puntas están sobre él |
| Área | La suma por el método del cordón coincide con el área analítica |
| L = 10 | s ≈ 3.819660112501; área roja ≈ 125.507134928892 |
| Valores inválidos | Se rechazan con `ArgumentOutOfRangeException` |
| Selector y lado | Se actualiza la flor y se puede volver al hexágono |
| Renderizado | Cinco centros de pétalos rojos y centro de la flor blanco |
| Copias | Modificar el arreglo devuelto no altera la figura |

Tolerancias por `PointF`: 0.0001 en posiciones y longitudes, 0.001 en perímetros,
0.02 en áreas individuales y 0.05 en el área sumada. Se guarda una imagen de
verificación en la carpeta `bin` del proyecto de pruebas, omitida por Git.

## Revisión manual

1. Ejecutar con F5 y seleccionar «Flor de margarita».
2. Probar lados exteriores 10, 150.5 y 280; comprobar cinco pétalos rojos y hueco central.
3. Con L = 10, comprobar lado de pétalo ≈ 3.82 y área roja ≈ 125.51.
4. Volver al hexágono y verificar que conserva sus seis sectores.
5. Minimizar, restaurar y reducir la ventana; comprobar repintado, controles y desplazamiento.
6. Abrir el diseñador de Visual Studio y verificar la edición del formulario.

La siguiente etapa será el polígono estrellado configurable.
