# GeoLab2D

Aplicación de escritorio para la tarea de Computación Gráfica, desarrollada
en C# con Windows Forms y .NET Framework 4.7.2.

## Avance actual: etapa 7

- Referencias del proyecto y punto de entrada corregidos para el formulario `GeoLab2D`.
- Lienzo de al menos 800 × 600 píxeles, con doble búfer.
- Contenedor con desplazamiento para acceder al lienzo al reducir la ventana.
- Control de dibujo separado del formulario.
- Hexágono generado con trigonometría y dividido en seis triángulos de colores.
- Lado configurable de 1 a 280 píxeles, con un decimal; área y perímetro visibles.
- Modelo matemático independiente de Windows Forms y pruebas reproducibles.
- Margarita de cinco pentágonos rojos inscritos en un pentágono mayor, con hueco central.
- Selector de figura; en la margarita el parámetro es el lado del pentágono exterior.
- Polígonos estrellados con radio, número de puntas y salto configurables.
- Construcción de estrellas simples y composiciones entrelazadas; longitud total de sus trazos.
- Operaciones vectoriales con componentes editables, flechas y ejes cartesianos.
- Magnitudes y direcciones de A y B, producto punto y determinante; manejo del vector cero.
- Análisis de las tres figuras: áreas, longitudes de contornos y pertenencia de un punto.
- Orientación (A, B, P), punto más cercano Q sobre AB y distancia P–Q, con dibujo interactivo.
- Traslación, rotación y escalamiento mediante matrices homogéneas 3 × 3.
- Matriz visible, botón Restablecer y análisis de las figuras transformadas.

Las fórmulas, responsabilidades y casos de prueba están en
[la documentación del hexágono](docs/Etapa2-Hexagono.md) y
[la documentación de la margarita](docs/Etapa3-Margarita.md) y
[la documentación de las estrellas](docs/Etapa4-Estrellas.md) y
[la documentación de vectores](docs/Etapa5-Vectores.md) y
[la documentación del análisis geométrico](docs/Etapa6-Geometria.md) y
[la documentación de matrices](docs/Etapa7-Matrices.md).
La edición de colores, los detalles finales de interacción y los
entregables finales siguen pendientes.

## Ejecutar

1. Abrir `GeoLab2D/GeoLab2D.slnx` en la versión de Visual Studio que creó la solución.
   También se puede abrir directamente `GeoLab2D/GeoLab2D.csproj`.
2. Tener instalado el componente de desarrollo de escritorio con .NET y el
   paquete de destino de .NET Framework 4.7.2.
3. Compilar la solución y ejecutar con F5.

El ejecutable de depuración se genera en `GeoLab2D/bin/Debug/GeoLab2D.exe`.
Git omite `bin`, `obj` y `.vs` porque son archivos generados localmente.

## Trabajo por etapas

Cada fila representa un avance funcional que se revisará antes de pasar al siguiente.
Los commits deben reflejar cambios realmente realizados y comprobados.

| Etapa | Alcance | Mensaje de commit sugerido |
| --- | --- | --- |
| 1 (completada) | Corregir inicio y preparar lienzo | `Prepara formulario y lienzo base de GeoLab2D` |
| 2 (completada) | Hexágono regular, seis triángulos de colores y lado configurable; cálculo separado del dibujo | `Agrega hexágono regular de seis colores` |
| 3 (completada) | Flor simétrica construida con cinco pentágonos regulares | `Agrega flor de margarita con pentagonos` |
| 4 (completada) | Polígono estrellado con número de puntas configurable (máximo actual: 64) | `Agrega construccion de poligonos estrellados` |
| 5 (completada) | Magnitud, dirección, producto punto y determinante de vectores | `Implementa operaciones vectoriales` |
| 6 (completada) | Áreas, perímetros, orientación, pertenencia y proyección sobre segmentos | `Agrega analisis geometrico y proyecciones` |
| 7 (actual) | Traslación, rotación y escalamiento mediante matrices homogéneas | `Implementa transformaciones matriciales 2D` |
| 8 | Integrar edición de posición, colores y ángulos; completar validaciones | `Completa controles interactivos y validaciones` |
| 9 | Consolidar casos de prueba, informe con fórmulas y guion de video de hasta cinco minutos | `Documenta fundamentos y pruebas de GeoLab2D` |

En cada etapa matemática se documentarán las fórmulas y se comprobarán casos
con resultados conocidos; la etapa 9 consolida esas evidencias.

## Comprobación manual de la etapa 1

1. Ejecutar la aplicación: debe abrirse el formulario GeoLab2D sin errores.
2. Comprobar que se muestra la superficie blanca y el estado de la etapa actual.
3. Reducir la ventana: deben aparecer barras de desplazamiento sin reducir
   el lienzo por debajo de 800 × 600.
4. Maximizar, minimizar y restaurar: el lienzo debe conservarse y repintarse.
5. Abrir el formulario en el diseñador de Visual Studio para comprobar su edición.

## Organización prevista

`GeoLab2D.cs` coordina los controles; `Controles/LienzoGeometrico.cs` dibuja.
`Geometria/HexagonoRegular.cs` calcula los vértices `PointF`, el área y el perímetro
sin depender de controles. Las siguientes etapas ampliarán la geometría y las matrices.
`Geometria/FlorMargarita.cs` construye los cinco pétalos y calcula sus propiedades.
`Geometria/PoligonoEstrellado.cs` genera los recorridos por diagonales y calcula sus longitudes.
`Geometria/Vector2D.cs` calcula las operaciones vectoriales y `Controles/PanelVectores.cs`
permite editar las componentes y visualizar los resultados.
`Geometria/GeometriaPlana.cs` implementa los algoritmos generales; `Controles/PanelAnalisis.cs`
permite editar A, B y P y consultar los resultados de la figura seleccionada.
`Geometria/Matriz3x3.cs` calcula las transformaciones y `Controles/PanelTransformaciones.cs`
permite componerlas y restablecerlas desde la interfaz.

## Verificar las etapas 2 a 7

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Se comprueban longitudes, área,
perímetro, entradas inválidas, actualización desde la interfaz y seis colores.
También se verifican la simetría de la flor, la inscripción de los pentágonos,
sus áreas, los cinco rellenos rojos, el hueco y la selección de figura.
Para las estrellas se verifican todos los saltos de 5 a 32 puntas con tres radios,
los recorridos cerrados, las componentes, la simetría, las longitudes y los controles.
Se añaden casos vectoriales conocidos, cuadrantes, entradas inválidas, vector cero,
actualización de los resultados y flechas, y regreso a una estrella de 64 puntas.
Se comprueban áreas firmadas, perímetros, orientación, puntos dentro/fuera/sobre el borde,
polígonos cóncavos y cruzados, huecos y proyecciones sobre segmentos, incluyendo A=B.
Las pruebas de matrices comprueban el orden de composición, invariantes de movimientos
rígidos, escalamiento de áreas, dibujo y análisis transformados y restablecimiento.
La revisión manual está descrita en los documentos de cada etapa.

## Registrar este avance

Después de revisar y compilar, desde la raíz del repositorio:

```powershell
git diff
git add README.md GeoLab2D/GeoLab2D.csproj GeoLab2D/GeoLab2D.cs GeoLab2D/GeoLab2D.Designer.cs GeoLab2D/Controles/LienzoGeometrico.cs GeoLab2D/Controles/PanelTransformaciones.cs GeoLab2D/Geometria/Matriz3x3.cs Pruebas/GeoLab2D.Pruebas.csproj Pruebas/VerificarHexagono.cs Pruebas/VerificarMatrices.cs docs/Etapa7-Matrices.md
git commit -m "Implementa transformaciones matriciales 2D"
git push
```

El comando `git push` supone que la rama actual ya tiene seguimiento de una
rama remota. No se realizan commits ni pushes automáticamente en esta etapa.
