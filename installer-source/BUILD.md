# Como compilar

A compilação requer Windows com o compilador C# do .NET Framework e o ZIP manual oficial da mesma versão.

Coloque o ZIP ao lado dos arquivos-fonte com o nome `Traducao-PTBR-The-Survey-v1.0.zip` e execute:

```powershell
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $csc /nologo /target:winexe /optimize+ /win32manifest:app.manifest `
  /reference:System.dll /reference:System.Core.dll /reference:System.Drawing.dll `
  /reference:System.Windows.Forms.dll /reference:System.IO.Compression.dll `
  /reference:System.IO.Compression.FileSystem.dll `
  /resource:Traducao-PTBR-The-Survey-v1.0.zip,TranslationPayload `
  /out:Traducao-PTBR-The-Survey-Instalador-v1.0.exe Installer.cs
```

Confira o resultado com o hash SHA-256 publicado na Release. Diferenças de compilador, metadados ou ambiente podem produzir um executável funcional com hash diferente.
