$ErrorActionPreference = 'SilentlyContinue'

$projectProcessIds = @()

$projectProcessIds += @(Get-Process -Name 'Presentation' -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Id)

$dotnetProcessIds = @(Get-CimInstance Win32_Process -Filter "Name = 'dotnet.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -like '*Presentation.dll*' } |
    Select-Object -ExpandProperty ProcessId)

$projectProcessIds += $dotnetProcessIds

$projectProcessIds |
    Where-Object { $_ } |
    Select-Object -Unique |
    ForEach-Object { Stop-Process -Id $_ -Force -ErrorAction SilentlyContinue }
