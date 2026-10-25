# Guion del video demostrativo de GeoLab2D

Duración objetivo: **4 minutos y 40 segundos**. Quedan 20 segundos de margen frente al máximo de 5 minutos.
La grabación debe mostrar la aplicación en funcionamiento y puede acompañarse de esta narración.

## Preparación antes de grabar

1. Abrir el ejecutable Release, ampliar la ventana y volver al inicio de la barra lateral.
2. Elegir hexágono de lado 150, pulsar Restablecer en transformaciones y restablecer sus colores.
3. Mantener cuadrícula y ejes visibles. Tener el proyecto y el registro de pruebas abiertos para el cierre.
4. Preparar los valores de cada segmento para evitar tiempo de búsqueda. No acelerar la reproducción para ocultar errores.
5. Hacer una grabación de ensayo, comprobar que los números se leen y recortar pausas sin superar 5:00.

| Tiempo | Acción en pantalla | Narración sugerida |
| --- | --- | --- |
| 0:00-0:20 | Mostrar la ventana y señalar panel lateral y lienzo | «GeoLab2D es una aplicación en C# con Windows Forms y .NET Framework 4.7.2. Construye figuras por algoritmos, permite transformarlas y comprobar sus propiedades. El lienzo tiene un mínimo de 800 por 600 píxeles». |
| 0:20-0:55 | Hexágono: cambiar lado a 10, mostrar perímetro 60 y área 259.81; volver a 150, seleccionar un sector y cambiar su color | «Los seis vértices se calculan con seno y coseno. Cada sector es un triángulo equilátero. Con lado 10 se obtiene perímetro 60 y área aproximada de 259.81. Puedo seleccionar un sector por clic y editar su color; el programa mantiene seis colores distintos». |
| 0:55-1:15 | Seleccionar margarita, lado exterior 150, señalar cinco pétalos y hueco | «La margarita está formada por cinco pentágonos regulares separados 72 grados. El lado exterior determina el lado de los pétalos mediante la proporción áurea. El centro queda vacío y no se suma al área». |
| 1:15-1:40 | Estrella N=5 K=2 y luego N=10 K=4; probar N=64 sin detenerse en cada salto | «La estrella conecta vértices de un polígono regular con un salto. Cinco vértices con salto dos forman un pentagrama. Diez con salto cuatro producen dos recorridos entrelazados. La interfaz admite hasta 64 puntas». |
| 1:40-2:10 | Volver al hexágono, lado 150; Tx=60, Ty=-20, ángulo=30, Sx=1.2, Sy=0.8; mostrar matriz y Restablecer | «Las transformaciones se realizan con matrices homogéneas 3 por 3. La composición T por R por S aplica primero escala, luego rotación y finalmente traslación. Las medidas se recalculan desde los puntos transformados. Restablecer recupera la figura base». |
| 2:10-2:45 | Modo vectores: A=(3,4), B=(-4,3), señalar magnitudes 5, producto 0 y determinante 25; probar A=(0,0) | «La magnitud de cada vector es cinco. El producto punto es cero porque son perpendiculares y el determinante es 25. Cuando un vector es cero, su dirección aparece como no definida. Los extremos también se pueden arrastrar». |
| 2:45-3:30 | Análisis: hexágono lado 150, identidad; A=(-100,0), B=(100,0), P=(30,80). Luego P=(150,0) y P=(250,250) | «El punto inicial está dentro. Q es el punto más cercano sobre AB: aquí Q es 30, cero y la distancia es 80. El parámetro se limita entre cero y uno para proyectar sobre el segmento. Ahora P está en el borde y después fuera. La orientación indica el sentido de A, B y P». |
| 3:30-3:55 | Volver al hexágono y activar Analizar figura; arrastrar figura y P; mover cursor y abrir Cómo interactuar | «Puedo analizar la figura coloreada, arrastrarla y mover los puntos. El pie muestra las coordenadas cartesianas del cursor, con el origen en el centro. La ayuda se consulta desde el botón superior sin ocupar el panel lateral». |
| 3:55-4:20 | Mostrar carpetas Geometria, Controles y Presentacion en Visual Studio | «El proyecto aplica POO. Las clases matemáticas construyen puntos y calculan resultados; LienzoGeometrico dibuja y los paneles gestionan la interacción. Hay encapsulación, objetos inmutables y herencia en los controles personalizados». |
| 4:20-4:40 | Mostrar final aprobado del registro y CSV de referencias | «La entrega incluye pruebas reproducibles en Release, 2790 combinaciones de estrellas y 16 referencias numéricas exportadas. El informe explica las fórmulas, los algoritmos, sus tolerancias y los casos especiales». |

## Si el tiempo se aproxima a cinco minutos

Reducir la edición de colores a una sola selección y omitir la exploración de 64 puntas, que puede mencionarse verbalmente. Conservar las tres figuras, una composición matricial, el ejemplo vectorial, una proyección, la arquitectura y el resultado de pruebas.

## Comprobación antes de entregar

- El archivo reproduce correctamente y dura como máximo 5:00.
- Los números y nombres de controles se leen; el audio explica lo que se está mostrando.
- No aparecen notificaciones personales ni credenciales en otras ventanas.
- El video está grabado: este guion no sustituye el archivo de video solicitado.
