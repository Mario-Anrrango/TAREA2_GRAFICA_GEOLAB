# Etapa 7: transformaciones mediante matrices homogéneas

## Uso

En los modos hexágono, margarita, estrella y análisis geométrico se muestra el
panel de transformaciones. Editar Tx, Ty, ángulo, Sx y Sy actualiza la figura
automáticamente. «Restablecer» devuelve traslaciones y ángulo a cero y escalas a uno.

Los parámetros se conservan al alternar entre figuras. El modo de vectores
mantiene sus coordenadas originales y oculta este panel.
El tamaño base, el lienzo mínimo de 800 × 600 y el máximo de 64 puntas se conservan.

## Convención matemática

Los puntos son vectores columna homogéneos p = (x, y, 1)ᵀ. Se usan matrices 3 × 3:

```text
Traslación T       Rotación R             Escalamiento S
[1  0  Tx]         [cosθ  -senθ  0]        [Sx  0   0]
[0  1  Ty]         [senθ   cosθ  0]        [0   Sy  0]
[0  0   1]         [0      0     1]        [0   0   1]

M = T · R · S
p' = M · p
```

Por tanto, se aplica primero el escalamiento respecto al origen, luego la rotación
alrededor del origen y finalmente la traslación. Con esta elección la figura gira
alrededor de su centro local antes de colocarse en (Tx, Ty).
El ángulo positivo es antihorario en coordenadas cartesianas; Ty positivo desplaza hacia arriba.
La conversión de Y a coordenadas de pantalla ocurre después de la transformación.

```text
M = [Sx cosθ   -Sy senθ   Tx]
    [Sx senθ    Sy cosθ   Ty]
    [0          0         1 ]
```

El producto se calcula con ciclos sobre filas, columnas y el índice compartido:
`C[i,j] = Σ A[i,k] B[k,j]`. No se delega el cálculo a Graphics.Transform.
La interfaz muestra las tres filas de M, agrupadas entre corchetes.
El redondeo mostrado no modifica los coeficientes utilizados para transformar.

## Orden y ejemplo

Para p = (1,2), Sx = 2, Sy = 3, θ = 90°, Tx = 5 y Ty = -1:

```text
S · p = (2,6)
R · S · p = (-6,2)
T · R · S · p = (-1,1)
```

El orden importa: trasladar (1,0) diez unidades a la derecha después de rotarlo
90° produce (10,1). Trasladarlo antes de rotar produce (0,11).

Los controles describen siempre una transformación desde la figura base. Cambiar
un valor recalcula M y los vértices, sin acumular errores por transformar repetidamente
el resultado anterior. El botón Restablecer recupera la geometría base.

## Medidas y análisis

La traslación y la rotación conservan área y longitud. Un escalamiento multiplica
el área por |Sx · Sy|. Si la escala es uniforme k, las longitudes se multiplican
por |k|. En escalas no uniformes se recalcula cada segmento; no hay un único
factor de longitud válido para todos los segmentos.

Los valores de tamaño del formulario describen la figura base. La zona de estado
muestra el área y las longitudes actuales, calculadas desde los contornos transformados.
Una escala no uniforme puede convertir la figura regular original en una figura
que ya no es regular. Los seis sectores del hexágono se transforman junto con
su centro, y la margarita conserva sus cinco polígonos y su hueco.

En análisis geométrico, pertenencia y área utilizan los contornos transformados.
Los puntos A, B y P permanecen en coordenadas cartesianas del lienzo y no se
transforman automáticamente. Q se recalcula sobre el segmento AB actual.
Así se puede mover una figura respecto a un punto fijo y comprobar su pertenencia.
Los contornos de análisis se transforman una sola vez.
Se conserva la distinción entre área algebraica y área visible de las estrellas.

## Rangos y límites

- Interfaz: Tx y Ty de -250 a 250 px; ángulo de -360° a 360°; Sx y Sy de 0.10 a 3.00.
- Esos rangos son decisiones de la interfaz, no límites matemáticos de las matrices.
- El modelo admite también escalas negativas (reflexiones) y cero (transformaciones singulares).
  La interfaz utiliza escalas positivas para conservar figuras con área no nula.
- Se rechazan coeficientes no finitos, matrices o arreglos nulos y puntos no finitos.
- Se detecta cuando un producto deja de ser finito o un punto excede el rango de PointF.
- Si parte de la figura queda fuera del lienzo, el panel lo indica. El lienzo no cambia
  de tamaño ni se introduce zoom: se puede reducir la escala o ajustar Tx y Ty.

## Organización

- `Geometria/Matriz3x3.cs`: matrices inmutables, fábricas, multiplicación y aplicación
  a puntos y contornos. Opera en double y devuelve nuevos PointF, sin modificar originales.
- `Controles/PanelTransformaciones.cs`: entradas, matriz compuesta, aviso de límites y restablecimiento.
- `GeoLab2D.cs`: conecta parámetros, medidas y análisis con los vértices transformados.
- `Controles/LienzoGeometrico.cs`: dibuja las figuras transformadas y sus colores con Graphics,
  Pen, Brush y PointF. La inversión del eje Y sigue siendo parte de la presentación.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Se ejecutan también las pruebas anteriores.

| Caso | Resultado esperado |
| --- | --- |
| Identidad sobre (1,2) | (1,2) |
| Traslación (5,-1) sobre (1,2) | (6,1) |
| Rotación 90° sobre (1,2) | (-2,1) |
| Rotación -90° sobre (1,2) | (2,-1) |
| Escala (2,3) sobre (1,2) | (2,6) |
| Composición del ejemplo | (-1,1) |
| Rotación 37° seguida de -37° | Punto original |
| Rectángulo 6 × 4 con traslación y rotación | Área 24, perímetro 20 |
| Rectángulo 6 × 4 con Sx=2 y Sy=3 | Área 144, perímetro 48 |
| Escala uniforme 2 sobre ese rectángulo | Perímetro 40 |
| Reflexión con Sx=-1 | Invierte el signo del área firmada |
| Escala singular Sx=0 | Área cero |

Además se verifican coeficientes homogéneos, entradas inválidas, desbordamiento,
arreglos independientes, actualización desde controles, color de un sector
transformado, aviso de recorte, botón Restablecer, pertenencia respecto a la
figura trasladada y área del análisis después de escalar.
Se comprueba la conservación de parámetros y de las estrellas de 64 puntas.
La tolerancia general es 0.001 por el redondeo final a PointF.
La imagen del hexágono transformado se guarda en la carpeta bin de pruebas, omitida por Git.

## Revisión manual

1. Ejecutar con F5 y mantener el hexágono de lado base 150.
2. Probar Tx=60, Ty=-20, ángulo=30°, Sx=1.20 y Sy=0.80; comprobar desplazamiento,
   giro y deformación de todos sus sectores.
3. Pulsar Restablecer y comprobar la figura inicial.
4. Probar la margarita y la estrella con los mismos controles.
5. En análisis geométrico elegir hexágono de lado base 20 y Tx=80. P=(80,0)
   debe estar dentro y P=(0,0) fuera.
6. Con Sx=Sy=2 comprobar que el área se cuadruplica y el perímetro se duplica.
7. Probar escala 3 con una figura grande y comprobar el aviso de recorte.
8. Alternar entre modos, minimizar, restaurar y reducir la ventana; comprobar
   controles, repintado y desplazamiento del lienzo.
9. Abrir el formulario en el diseñador de Visual Studio para verificar su edición.

La siguiente etapa completará la edición de colores, los detalles de interacción
y las validaciones de la interfaz antes de consolidar los entregables finales.
