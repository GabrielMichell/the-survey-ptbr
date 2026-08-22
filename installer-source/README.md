# Código-fonte do instalador automático

Esta pasta contém o código-fonte do instalador distribuído na Release `v1.1`.

## Arquivos

- `Installer.cs`: interface, localização da instalação Steam, backup, instalação e restauração.
- `app.manifest`: solicita elevação administrativa para gravar na pasta do jogo.
- `BUILD.md`: instruções de compilação.

O instalador não baixa arquivos, não modifica `Visibility03.exe` e não desativa proteções do Windows. O ZIP manual da Release é incorporado ao executável como recurso `TranslationPayload`.
