param([string]$MSBuildPath)
$ErrorActionPreference = 'Stop'
$raizProyecto = Split-Path -Parent $PSScriptRoot
if (-not $MSBuildPath) {
    $enRuta = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($enRuta) { $MSBuildPath = $enRuta.Source }
    else {
        $instalador = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
        if (Test-Path -LiteralPath $instalador) {
            $MSBuildPath = & $instalador -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        }
    }
}
if (-not $MSBuildPath -or -not (Test-Path -LiteralPath $MSBuildPath)) { throw 'Indique -MSBuildPath con la ruta de MSBuild.exe.' }
$salida = Join-Path $raizProyecto 'entrega/evidencias'
New-Item -ItemType Directory -Path $salida -Force | Out-Null
# Invalidar evidencias anteriores antes de empezar una nueva ejecucion.
# Un fallo no debe permitir empaquetar usando un resultado aprobado antiguo.
@{ codigoCompilacion = -1; codigoPruebas = -1; fechaUtc = [DateTime]::UtcNow.ToString('o') } |
    ConvertTo-Json | Set-Content -LiteralPath (Join-Path $salida 'validacion.json') -Encoding UTF8
Push-Location $raizProyecto
try {
    & $MSBuildPath 'Pruebas/GeoLab2D.Pruebas.csproj' /t:Rebuild /p:Configuration=Release /nologo /verbosity:minimal 2>&1 |
        Tee-Object -FilePath (Join-Path $salida 'compilacion-release.txt')
    $codigoCompilacion = $LASTEXITCODE
    if ($codigoCompilacion -ne 0) { throw "Compilacion fallida: $codigoCompilacion" }
    & './Pruebas/bin/Release/GeoLab2D.Pruebas.exe' 2>&1 | Tee-Object -FilePath (Join-Path $salida 'pruebas-release.txt')
    $codigoPruebas = $LASTEXITCODE
    $codigoPruebas | Set-Content -LiteralPath (Join-Path $salida 'codigo-salida-pruebas.txt') -Encoding UTF8
    if ($codigoPruebas -ne 0) { throw "Pruebas fallidas: $codigoPruebas" }
    Copy-Item -LiteralPath './Pruebas/bin/Release/casos-referencia.csv' -Destination $salida -Force
    $imagenes = Join-Path $salida 'imagenes'
    New-Item -ItemType Directory -Path $imagenes -Force | Out-Null
    Get-ChildItem -LiteralPath './Pruebas/bin/Release' -Filter '*verificad*.png' | Copy-Item -Destination $imagenes -Force
    $info = [ordered]@{
        fechaUtc = [DateTime]::UtcNow.ToString('o')
        framework = '.NET Framework 4.7.2'
        configuracion = 'Release'
        msbuild = $MSBuildPath
        codigoCompilacion = $codigoCompilacion
        codigoPruebas = $codigoPruebas
        casosEstrellas = 2790
        casosReferencia = 16
        revisionGitBase = (& git rev-parse HEAD)
        incluyeCambiosLocales = $true
    }
    $info | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $salida 'validacion.json') -Encoding UTF8
    Write-Output 'Validacion Release completada. Evidencias en entrega/evidencias.'
}
finally { Pop-Location }
