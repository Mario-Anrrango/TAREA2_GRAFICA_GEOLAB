# Etapa 6: análisis geométrico y proyección sobre segmentos

## Uso

Seleccionar «Análisis geométrico» y elegir hexágono, margarita o estrella en el
segundo selector. El control de tamaño conserva su significado según la figura:
lado del hexágono, lado exterior de la margarita o radio exterior de la estrella.
Puntas y salto se habilitan al analizar una estrella, con hasta 64 puntas.

Editar las coordenadas cartesianas de A, B y P, entre -250 y 250 px y con dos
decimales. Se muestran área, longitud de contornos, pertenencia de P, orientación
del triple (A, B, P), punto Q más cercano sobre AB, distancia P–Q y parámetro t.
AB se dibuja en azul, P en naranja y Q en verde. Una línea verde discontinua une P y Q.
El lienzo conserva su mínimo de 800 × 600 y las otras figuras y vectores siguen disponibles.

## Áreas y perímetros

Para un contorno ordenado V₀, ..., Vₙ₋₁, con Vₙ = V₀:

```text
Área firmada = (1/2) Σ (xᵢ yᵢ₊₁ - xᵢ₊₁ yᵢ)
Perímetro = Σ √((xᵢ₊₁ - xᵢ)² + (yᵢ₊₁ - yᵢ)²)
```

La implementación calcula el área mediante una suma equivalente de determinantes
con origen en el primer vértice, para reducir cancelaciones con coordenadas grandes.
En un polígono simple, el signo del área indica su orientación y el valor absoluto
es su área geométrica. Invertir el orden cambia el signo, pero conserva el perímetro.

En la margarita se suman las áreas de los cinco pétalos, cuyos interiores no se
superponen. La longitud de contornos incluye los cinco pentágonos completos,
también sus lados que limitan con el hueco central.

En las estrellas con cruces se muestra explícitamente el valor absoluto de la
suma de áreas algebraicas de los recorridos. Puede incluir multiplicidades y
cancelaciones; no es el área del relleno visible. La longitud mostrada suma todos
los segmentos de los recorridos. No se presenta como perímetro de una región simple.

## Orientación de tres puntos

```text
D = (Bx - Ax)(Py - Ay) - (By - Ay)(Px - Ax)
D > 0: antihoraria (P a la izquierda del segmento dirigido AB)
D < 0: horaria (P a la derecha)
D ≈ 0: colineal
```

Se considera colineal si `|D| ≤ ε · |AB|`, con ε = 0.00001 unidades de distancia.
Esto evita usar la misma tolerancia de área para segmentos de longitudes diferentes.
Si A = B, el modelo devuelve colineal por determinante nulo, pero la interfaz
explica «indeterminada (A = B)», porque AB no determina una dirección.

## Punto más cercano sobre un segmento

```text
v = B - A
t₀ = ((P - A) · v) / (v · v)
t = max(0, min(1, t₀))
Q = A + t v
Distancia mínima = |P - Q|
```

Limitar t a [0,1] es esencial: se proyecta sobre el segmento, no sobre toda su
recta. Si t₀ < 0 se obtiene A y si t₀ > 1 se obtiene B. Si A = B, Q = A y t = 0
por convenio; se evita dividir por cero y se indica que AB se reduce a un punto.

Los cálculos se realizan en double. Q se devuelve como PointF para el dibujo;
su distancia se calcula antes del redondeo a float.

## Pertenencia y borde

Primero se calcula la distancia de P a cada segmento. Si alguna es menor o igual
que ε, el resultado es «Borde». En otro caso se lanza un rayo horizontal hacia +X:
un número impar de cruces significa «Dentro» y uno par significa «Fuera».
La prueba de cruce es semiabierta en Y para no contar dos veces un vértice.

La paridad se acumula sobre todos los contornos. Esto permite representar huecos.
En la margarita el centro queda fuera; en un pentagrama, la zona central también
queda fuera bajo la regla par-impar. Las estrellas entrelazadas usan esta misma
regla global, por lo que una zona cubierta dos veces puede quedar vacía.
El relleno del lienzo utiliza GraphicsPath con FillMode.Alternate, consistente
con esa clasificación. Los bordes de todos los trazos se consideran «Borde».

## Organización y validación

- `Geometria/GeometriaPlana.cs`: área firmada, perímetro, distancia, orientación,
  proyección y pertenencia. Solo depende de System y PointF.
- `ProyeccionSegmento`: resultado inmutable con Q, t, distancia y detección de AB degenerado.
- `Controles/PanelAnalisis.cs`: entradas y presentación de los resultados.
- `GeoLab2D.cs`: genera los contornos de la figura elegida y coordina el análisis.
- `Controles/LienzoGeometrico.cs`: copia los contornos y dibuja figura, puntos y segmento.

Se rechazan arreglos nulos, contornos de menos de tres vértices, conjuntos vacíos
y coordenadas NaN o infinitas. Se admiten contornos degenerados y autointersectados:
el área conserva su definición algebraica y la pertenencia su definición par-impar.
Los segmentos de longitud cero y los vértices consecutivos repetidos no causan división por cero.

## Pruebas reproducibles

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Se conservan las pruebas de etapas anteriores.

| Caso | Resultado esperado |
| --- | --- |
| Cuadrado de lado 10 | Área 100, perímetro 40 |
| Cuadrado con orden inverso | Área firmada -100, perímetro 40 |
| Cuadrado trasladado | Misma área |
| Polígono cóncavo en L | Área 7, perímetro 16; punto (2,2) fuera |
| Contorno cruzado en forma de lazo | Área algebraica 0; punto de cruce sobre el borde |
| Cuadrado con contorno interior | El centro está fuera bajo par-impar |
| Centro de margarita y pentagrama | Fuera |
| Pentagrama N=5, K=2, R=10 | Área algebraica ≈ 146.946313073118 |
| A=(0,0), B=(10,0), P=(3,4) | Q=(3,0), t=0.3, distancia 4 |
| Mismo AB, P=(-3,4) | Q=A, t=0, distancia 5 |
| Mismo AB, P=(13,4) | Q=B, t=1, distancia 5 |
| A=(2,-2), B=(2,8), P=(-2,3) | Q=(2,3), t=0.5, distancia 4 |
| A=(0,0), B=(10,10), P=(10,0) | Q=(5,5), t=0.5, distancia √50 |
| A=B=(1,2), P=(4,6) | Q=(1,2), distancia 5 y segmento degenerado |

También se verifican orientación, tolerancia de colinealidad, proyección con AB
invertido, entradas inválidas y área de los cinco pétalos. Las pruebas de interfaz
comprueban resultados al editar puntos, representación de P y Q, selección de las
tres figuras, segmento degenerado y regreso a los demás modos. La imagen de
verificación se guarda en `Pruebas/bin/Debug/analisis-verificado.png`, omitida por Git.

La tolerancia general de las comparaciones de prueba es 0.0001; la del área de
la margarita es 0.02 por la suma de coordenadas PointF. No debe confundirse esta
tolerancia de prueba con ε, usado por los algoritmos de borde y orientación.

## Revisión manual

1. Ejecutar con F5 y abrir «Análisis geométrico».
2. Elegir hexágono de lado 150, A=(-100,0), B=(100,0), P=(30,80).
   Comprobar P dentro, orientación antihoraria, Q=(30,0) y distancia 80.
3. Cambiar P a (150,0) y luego a (250,250): comprobar borde y exterior.
4. Probar A=B; comprobar el mensaje de degeneración y que Q coincide con ese punto.
5. Analizar la margarita con P=(0,0); comprobar que su centro está fuera.
6. Analizar estrella N=5, K=2 con P=(0,0); comprobar hueco central y etiqueta de área algebraica.
7. Cambiar tamaño, puntas y salto; comprobar la actualización de resultados y dibujo.
8. Reducir, minimizar y restaurar la ventana; revisar las filas de entradas y el desplazamiento.
9. Volver a las figuras y vectores y abrir el formulario en el diseñador de Visual Studio.

La siguiente etapa implementará las transformaciones mediante matrices homogéneas.
