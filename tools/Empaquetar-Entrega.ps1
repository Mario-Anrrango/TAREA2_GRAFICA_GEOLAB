$ErrorActionPreference = 'Stop'
$raizProyecto = Split-Path -Parent $PSScriptRoot
$salida = Join-Path $raizProyecto 'entrega'
$evidencias = Join-Path $salida 'evidencias'
$informe = Join-Path $raizProyecto 'output/pdf/Informe_Tecnico_GeoLab2D.pdf'
$seleccionInforme = Join-Path $salida 'informe-actual.json'
if (Test-Path -LiteralPath $seleccionInforme) {
    $datosInforme = Get-Content -LiteralPath $seleccionInforme -Raw -Encoding UTF8 | ConvertFrom-Json
    $informe = Join-Path $raizProyecto $datosInforme.ruta
}
foreach ($necesario in @($informe, (Join-Path $evidencias 'validacion.json'), (Join-Path $raizProyecto 'GeoLab2D/bin/Release/GeoLab2D.exe'))) {
    if (-not (Test-Path -LiteralPath $necesario)) { throw "Falta un archivo requerido: $necesario" }
}
$validacion = Get-Content -LiteralPath (Join-Path $evidencias 'validacion.json') -Raw | ConvertFrom-Json
if ($validacion.codigoPruebas -ne 0 -or $validacion.codigoCompilacion -ne 0) { throw 'La validacion no esta aprobada.' }
# Directorio unico de archivos temporales, ignorado por Git. No borra archivos existentes.
$temporal = Join-Path $salida ('bin/empaquetado-' + [Guid]::NewGuid().ToString('N'))
$codigo = Join-Path $temporal 'CodigoFuente'
$ejecutable = Join-Path $temporal 'Ejecutable'
$completo = Join-Path $temporal 'Entrega'
New-Item -ItemType Directory -Path $codigo,$ejecutable,$completo -Force | Out-Null
foreach ($carpeta in @('GeoLab2D','Pruebas','docs','tools')) {
    $origen = Join-Path $raizProyecto $carpeta
    Get-ChildItem -LiteralPath $origen -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj|\.vs|\.git)[\\/]' } | ForEach-Object {
        $relativa = $_.FullName.Substring($raizProyecto.Length).TrimStart('\','/')
        $destino = Join-Path $codigo $relativa
        New-Item -ItemType Directory -Path (Split-Path -Parent $destino) -Force | Out-Null
        Copy-Item -LiteralPath $_.FullName -Destination $destino -Force
    }
}
Copy-Item -LiteralPath (Join-Path $raizProyecto 'README.md'),(Join-Path $raizProyecto '.gitignore') -Destination $codigo
Copy-Item -LiteralPath (Join-Path $raizProyecto 'GeoLab2D/bin/Release/GeoLab2D.exe'),(Join-Path $raizProyecto 'GeoLab2D/bin/Release/GeoLab2D.exe.config') -Destination $ejecutable
Copy-Item -LiteralPath (Join-Path $salida 'LEEME_ENTREGA.md') -Destination $ejecutable
$zipCodigo = Join-Path $salida 'GeoLab2D_CodigoFuente.zip'
$zipExe = Join-Path $salida 'GeoLab2D_Ejecutable.zip'
Compress-Archive -Path (Join-Path $codigo '*') -DestinationPath $zipCodigo -Force
Compress-Archive -Path (Join-Path $ejecutable '*') -DestinationPath $zipExe -Force
Copy-Item -LiteralPath $zipCodigo,$zipExe,$informe,(Join-Path $salida 'LEEME_ENTREGA.md'),(Join-Path $raizProyecto 'docs/Guion_Video.md') -Destination $completo
Copy-Item -LiteralPath $evidencias -Destination (Join-Path $completo 'evidencias') -Recurse
$zipCompleto = Join-Path $salida 'GeoLab2D_Entrega.zip'
Compress-Archive -Path (Join-Path $completo '*') -DestinationPath $zipCompleto -Force
Get-FileHash -LiteralPath $zipCodigo,$zipExe,$zipCompleto,$informe -Algorithm SHA256 |
    Select-Object @{Name='Archivo';Expression={Split-Path -Leaf $_.Path}},Hash |
    Export-Csv -LiteralPath (Join-Path $salida 'SHA256.csv') -NoTypeInformation -Encoding UTF8
Write-Output "Entrega empaquetada en $salida"
