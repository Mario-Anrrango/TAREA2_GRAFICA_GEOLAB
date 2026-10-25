# Entrega de GeoLab2D

## Archivos

- `GeoLab2D_Ejecutable.zip`: aplicación Release y configuración.
- `GeoLab2D_CodigoFuente.zip`: proyecto, pruebas, documentación y scripts, sin archivos temporales de Visual Studio.
- `GeoLab2D_Entrega.zip`: ambos paquetes, informe PDF, guion y evidencias.
- `evidencias`: registro de compilación, pruebas, 16 referencias numéricas e imágenes renderizadas.
- `SHA256.csv`: comprobación de integridad de los paquetes y el informe.

El informe editable está en `docs/InformeTecnico.md` y el procedimiento ilustrado en `docs/Procedimiento_Paso_a_Paso.md`.
La versión final es `output/pdf/Informe_Tecnico_GeoLab2D_Anrrango_Benavides_Mario_Santiago.pdf`: incluye la carátula proporcionada y siete capturas del desarrollo anterior al rediseño visual.

## Ejecutar

1. Extraer completamente `GeoLab2D_Ejecutable.zip` en una carpeta de Windows.
2. Tener instalado .NET Framework 4.7.2 o una versión compatible posterior de .NET Framework 4.x.
3. Mantener `GeoLab2D.exe` y `GeoLab2D.exe.config` juntos y abrir el ejecutable.

No requiere servidor, conexión a Internet ni paquetes NuGet. La paleta se conserva durante la sesión, no entre ejecuciones.

## Abrir el código

Abrir `GeoLab2D/GeoLab2D.slnx` en la versión de Visual Studio que creó la solución.
También se puede abrir `GeoLab2D/GeoLab2D.csproj`. Se requiere la carga de desarrollo de escritorio con .NET y el paquete de destino 4.7.2.
Para repetir la validación ejecutar `tools/Validar-Entrega.ps1` desde PowerShell. Si no detecta MSBuild, proporcionar `-MSBuildPath`.
Las pruebas devuelven 0 si pasan y 1 si falla alguna comprobación.

## Revisar antes de enviar

- Revisar los datos académicos de la carátula proporcionada.
- Revisar apertura/cancelación del diálogo de colores, la guía, el foco, el desplazamiento y el diseñador en el equipo de entrega.
- Grabar el video siguiendo `Guion_Video.md`, comprobar audio y legibilidad y confirmar duración máxima de 5:00.
- Adjuntar el video o su enlace según la modalidad solicitada por el docente. El paquete contiene el guion, no una grabación.
- Revisar los cambios de Git y realizar el commit y push de esta última etapa. Los scripts no hacen commits ni pushes.
