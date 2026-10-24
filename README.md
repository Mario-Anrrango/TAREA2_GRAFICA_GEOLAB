# GeoLab2D

Aplicación de escritorio para la tarea de Computación Gráfica, desarrollada
en C# con Windows Forms y .NET Framework 4.7.2.

## Avance actual: etapa 3

- Referencias del proyecto y punto de entrada corregidos para el formulario `GeoLab2D`.
- Lienzo de al menos 800 × 600 píxeles, con doble búfer.
- Contenedor con desplazamiento para acceder al lienzo al reducir la ventana.
- Control de dibujo separado del formulario.
- Hexágono generado con trigonometría y dividido en seis triángulos de colores.
- Lado configurable de 1 a 280 píxeles, con un decimal; área y perímetro visibles.
- Modelo matemático independiente de Windows Forms y pruebas reproducibles.
- Margarita de cinco pentágonos rojos inscritos en un pentágono mayor, con hueco central.
- Selector de figura; en la margarita el parámetro es el lado del pentágono exterior.

Las fórmulas, responsabilidades y casos de prueba están en
[la documentación del hexágono](docs/Etapa2-Hexagono.md) y
[la documentación de la margarita](docs/Etapa3-Margarita.md).
Las estrellas y las operaciones generales siguen pendientes.

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
| 3 (actual) | Flor simétrica construida con cinco pentágonos regulares | `Agrega flor de margarita con pentagonos` |
| 4 | Polígono estrellado con número de puntas configurable | `Agrega construccion de poligonos estrellados` |
| 5 | Magnitud, dirección, producto punto y determinante de vectores | `Implementa operaciones vectoriales` |
| 6 | Áreas, perímetros, orientación, pertenencia y proyección sobre segmentos | `Agrega analisis geometrico y proyecciones` |
| 7 | Traslación, rotación y escalamiento mediante matrices homogéneas | `Implementa transformaciones matriciales 2D` |
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

## Verificar las etapas 2 y 3

Compilar `Pruebas/GeoLab2D.Pruebas.csproj` y ejecutar
`Pruebas/bin/Debug/GeoLab2D.Pruebas.exe`. Se comprueban longitudes, área,
perímetro, entradas inválidas, actualización desde la interfaz y seis colores.
También se verifican la simetría de la flor, la inscripción de los pentágonos,
sus áreas, los cinco rellenos rojos, el hueco y la selección de figura.
La revisión manual está descrita en los documentos de cada etapa.

## Registrar este avance

Después de revisar y compilar, desde la raíz del repositorio:

```powershell
git diff
git add README.md GeoLab2D/GeoLab2D.csproj GeoLab2D/GeoLab2D.cs GeoLab2D/GeoLab2D.Designer.cs GeoLab2D/Controles/LienzoGeometrico.cs GeoLab2D/Geometria/FlorMargarita.cs Pruebas/GeoLab2D.Pruebas.csproj Pruebas/VerificarHexagono.cs Pruebas/VerificarMargarita.cs docs/Etapa3-Margarita.md
git commit -m "Agrega flor de margarita con pentagonos"
git push
```

El comando `git push` supone que la rama actual ya tiene seguimiento de una
rama remota. No se realizan commits ni pushes automáticamente en esta etapa.
