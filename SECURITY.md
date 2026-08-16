# Segurança

## Downloads oficiais

Baixe a tradução somente pelo repositório oficial:

- https://github.com/GabrielMichell/the-survey-ptbr
- https://github.com/GabrielMichell/the-survey-ptbr/releases

Nunca aceite arquivos enviados por mensagens privadas, encurtadores de URL, sites de terceiros ou páginas que imitem este projeto.

## SmartScreen e antivírus

**Nunca é necessário desativar o antivírus, o Microsoft Defender, o SmartScreen ou qualquer outra proteção para instalar esta tradução.**

O instalador não possui assinatura digital comercial. Por isso, o SmartScreen pode mostrar **“O Windows protegeu o computador”** e classificá-lo como **aplicativo não reconhecido**. Esse aviso de reputação, sozinho, não informa que um vírus foi detectado.

Se aparecer somente esse aviso do SmartScreen:

1. não desative a proteção;
2. confirme que o download veio da Release oficial;
3. confira o nome do arquivo e seu hash SHA-256 com `SHA256SUMS.txt`;
4. clique em **Mais informações**;
5. estando tudo correto, escolha **Executar assim mesmo**.

Esse procedimento autoriza apenas o arquivo verificado e não desativa o SmartScreen.

Se o antivírus informar uma ameaça específica, colocar o arquivo em quarentena ou apresentar um nome de detecção, não crie exceções e não force a execução. Use o ZIP manual e abra uma Issue com o nome do antivírus, a mensagem completa e a versão baixada.

## Comportamento do instalador

O instalador não baixa conteúdo da internet, não altera o executável do jogo e não desativa proteções do Windows. Ele modifica cinco arquivos da pasta de dados e cria backup antes da instalação. Seu código-fonte está disponível em [installer-source](installer-source/).

## Relato de vulnerabilidades

Não publique senhas, e-mails pessoais, chaves de acesso ou outros dados sensíveis em Issues. Para um problema que possa colocar usuários em risco, use a opção privada **Report a vulnerability** na aba Security do repositório, quando disponível.

