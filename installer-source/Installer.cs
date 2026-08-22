using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TheSurveyPTBRInstaller
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }
    }

    internal sealed class InstallerForm : Form
    {
        private const string GameExe = "Visibility03.exe";
        private const string DataFolder = "Visibility03_Data";
        private const string BackupFolder = "_Backup_Traducao_PTBR";
        private readonly string[] files = {
            "level0", "level1", "sharedassets0.assets", "sharedassets1.assets",
            Path.Combine("Managed", "Assembly-CSharp.dll")
        };

        private TextBox pathBox;
        private Label statusLabel;
        private ProgressBar progress;
        private Button installButton;
        private Button removeButton;

        public InstallerForm()
        {
            Text = "Tradução PT-BR — The Survey v1.1";
            ClientSize = new Size(640, 475);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 245, 245);
            Font = new Font("Segoe UI", 10F);

            Label title = new Label();
            title.Text = "THE SURVEY — TRADUÇÃO PT-BR";
            title.Font = new Font("Segoe UI Semibold", 19F, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(32, 25);
            Controls.Add(title);

            Label credits = new Label();
            credits.Text = "Tradução: Gabriel Michel   •   Assistência: OpenAI Codex";
            credits.AutoSize = true;
            credits.ForeColor = Color.DimGray;
            credits.Location = new Point(35, 69);
            Controls.Add(credits);

            Label pathLabel = new Label();
            pathLabel.Text = "Pasta de instalação do jogo:";
            pathLabel.AutoSize = true;
            pathLabel.Location = new Point(35, 112);
            Controls.Add(pathLabel);

            pathBox = new TextBox();
            pathBox.Location = new Point(38, 139);
            pathBox.Size = new Size(474, 30);
            Controls.Add(pathBox);

            Button browseButton = new Button();
            browseButton.Text = "Procurar...";
            browseButton.Location = new Point(520, 137);
            browseButton.Size = new Size(90, 34);
            browseButton.Click += BrowseClicked;
            Controls.Add(browseButton);

            Panel warningPanel = new Panel();
            warningPanel.Location = new Point(38, 185);
            warningPanel.Size = new Size(572, 66);
            warningPanel.BackColor = Color.FromArgb(255, 247, 224);
            warningPanel.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(warningPanel);

            Label warningTitle = new Label();
            warningTitle.Text = "AVISO — TRADUÇÃO NÃO OFICIAL";
            warningTitle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            warningTitle.ForeColor = Color.FromArgb(121, 75, 0);
            warningTitle.AutoSize = true;
            warningTitle.Location = new Point(12, 8);
            warningPanel.Controls.Add(warningTitle);

            Label warningText = new Label();
            warningText.Text = "Atualização v1.1: corrige a entrada numérica e a tela de dicas do celular.\nFeche o jogo antes de instalar; 5 arquivos serão alterados e um backup será criado.";
            warningText.Font = new Font("Segoe UI", 8.5F);
            warningText.ForeColor = Color.FromArgb(80, 65, 38);
            warningText.AutoSize = true;
            warningText.Location = new Point(12, 29);
            warningPanel.Controls.Add(warningText);

            installButton = new Button();
            installButton.Text = "Instalar tradução";
            installButton.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            installButton.BackColor = Color.FromArgb(37, 125, 76);
            installButton.ForeColor = Color.White;
            installButton.FlatStyle = FlatStyle.Flat;
            installButton.Location = new Point(38, 270);
            installButton.Size = new Size(270, 54);
            installButton.Click += InstallClicked;
            Controls.Add(installButton);

            removeButton = new Button();
            removeButton.Text = "Remover tradução";
            removeButton.Location = new Point(320, 270);
            removeButton.Size = new Size(290, 54);
            removeButton.Click += RemoveClicked;
            Controls.Add(removeButton);

            progress = new ProgressBar();
            progress.Location = new Point(38, 345);
            progress.Size = new Size(572, 18);
            progress.Minimum = 0;
            progress.Maximum = files.Length;
            Controls.Add(progress);

            statusLabel = new Label();
            statusLabel.Text = "Localizando o jogo pela Steam...";
            statusLabel.AutoSize = false;
            statusLabel.Size = new Size(572, 48);
            statusLabel.Location = new Point(38, 377);
            statusLabel.ForeColor = Color.FromArgb(70, 70, 70);
            Controls.Add(statusLabel);

            Label safety = new Label();
            safety.Text = "O instalador altera somente 5 arquivos e cria backup antes da instalação.";
            safety.AutoSize = true;
            safety.Font = new Font("Segoe UI", 8.5F);
            safety.ForeColor = Color.Gray;
            safety.Location = new Point(38, 447);
            Controls.Add(safety);

            Shown += delegate {
                string detected = DetectGameFolder();
                pathBox.Text = detected ?? @"C:\Program Files (x86)\Steam\steamapps\common\The Survey";
                statusLabel.Text = detected != null ? "Jogo encontrado. Pronto para instalar." : "Selecione a pasta onde o jogo está instalado.";
            };
        }

        private void BrowseClicked(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecione a pasta principal de The Survey";
                dialog.SelectedPath = Directory.Exists(pathBox.Text) ? pathBox.Text : "";
                if (dialog.ShowDialog(this) == DialogResult.OK) pathBox.Text = dialog.SelectedPath;
            }
        }

        private bool IsValidGameFolder(string folder)
        {
            return Directory.Exists(folder) && File.Exists(Path.Combine(folder, GameExe)) &&
                   Directory.Exists(Path.Combine(folder, DataFolder));
        }

        private void SetBusy(bool busy)
        {
            installButton.Enabled = !busy;
            removeButton.Enabled = !busy;
            UseWaitCursor = busy;
            Application.DoEvents();
        }

        private void InstallClicked(object sender, EventArgs e)
        {
            string game = pathBox.Text.Trim();
            if (!IsValidGameFolder(game))
            {
                MessageBox.Show(this, "A pasta selecionada não contém uma instalação válida de The Survey.", "Pasta incorreta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(this, "Instalar a tradução PT-BR nesta pasta?\n\n" + game, "Confirmar instalação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string temp = Path.Combine(Path.GetTempPath(), "TheSurveyPTBR_" + Guid.NewGuid().ToString("N"));
            try
            {
                SetBusy(true); progress.Value = 0; statusLabel.Text = "Preparando arquivos...";
                Directory.CreateDirectory(temp);
                string payload = Path.Combine(temp, "traducao.zip");
                using (Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream("TranslationPayload"))
                using (FileStream target = File.Create(payload)) source.CopyTo(target);
                ZipFile.ExtractToDirectory(payload, temp);
                string sourceData = FindPayloadDataFolder(temp);
                string targetData = Path.Combine(game, DataFolder);
                string backupData = Path.Combine(game, BackupFolder, DataFolder);

                for (int i = 0; i < files.Length; i++)
                {
                    string relative = files[i];
                    statusLabel.Text = "Instalando: " + relative;
                    string original = Path.Combine(targetData, relative);
                    string backup = Path.Combine(backupData, relative);
                    string translated = Path.Combine(sourceData, relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(backup));
                    Directory.CreateDirectory(Path.GetDirectoryName(original));
                    if (!File.Exists(backup)) File.Copy(original, backup, false);
                    File.Copy(translated, original, true);
                    progress.Value = i + 1;
                    Application.DoEvents();
                }
                statusLabel.Text = "Tradução instalada com sucesso.";
                MessageBox.Show(this, "Tradução PT-BR instalada com sucesso!", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                ShowAdminError();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Não foi possível concluir a instalação.";
                MessageBox.Show(this, "Erro durante a instalação:\n\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
                try { if (Directory.Exists(temp)) Directory.Delete(temp, true); } catch { }
            }
        }

        private void RemoveClicked(object sender, EventArgs e)
        {
            string game = pathBox.Text.Trim();
            if (!IsValidGameFolder(game))
            {
                MessageBox.Show(this, "Selecione primeiro a pasta correta do jogo.", "Pasta incorreta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string backupData = Path.Combine(game, BackupFolder, DataFolder);
            if (!Directory.Exists(backupData))
            {
                MessageBox.Show(this, "Nenhum backup deste instalador foi encontrado. Para restaurar o jogo, use a verificação de integridade dos arquivos na Steam.", "Backup não encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show(this, "Restaurar os arquivos originais salvos pelo instalador?", "Remover tradução", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                SetBusy(true); progress.Value = 0;
                string targetData = Path.Combine(game, DataFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    string relative = files[i];
                    statusLabel.Text = "Restaurando: " + relative;
                    File.Copy(Path.Combine(backupData, relative), Path.Combine(targetData, relative), true);
                    progress.Value = i + 1;
                    Application.DoEvents();
                }
                statusLabel.Text = "Arquivos originais restaurados.";
                MessageBox.Show(this, "Tradução removida e arquivos originais restaurados.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException) { ShowAdminError(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { SetBusy(false); }
        }

        private void ShowAdminError()
        {
            statusLabel.Text = "O Windows bloqueou a gravação na pasta do jogo.";
            MessageBox.Show(this, "Não foi possível alterar a pasta do jogo. Feche o jogo e abra este instalador como administrador.", "Permissão necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private string FindPayloadDataFolder(string root)
        {
            foreach (string dir in Directory.GetDirectories(root, DataFolder, SearchOption.AllDirectories))
                if (File.Exists(Path.Combine(dir, "level0"))) return dir;
            throw new DirectoryNotFoundException("A pasta de tradução não foi encontrada dentro do instalador.");
        }

        private string DetectGameFolder()
        {
            foreach (string steam in SteamPaths())
            {
                string direct = Path.Combine(steam, "steamapps", "common", "The Survey");
                if (IsValidGameFolder(direct)) return direct;
                string libraries = Path.Combine(steam, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(libraries)) continue;
                string text = File.ReadAllText(libraries);
                foreach (Match match in Regex.Matches(text, "\\\"path\\\"\\s+\\\"([^\\\"]+)\\\""))
                {
                    string library = match.Groups[1].Value.Replace("\\\\", "\\");
                    string candidate = Path.Combine(library, "steamapps", "common", "The Survey");
                    if (IsValidGameFolder(candidate)) return candidate;
                }
            }
            return null;
        }

        private IEnumerable<string> SteamPaths()
        {
            string[] keys = { @"SOFTWARE\WOW6432Node\Valve\Steam", @"SOFTWARE\Valve\Steam" };
            foreach (string keyName in keys)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
                {
                    if (key != null)
                    {
                        string value = key.GetValue("InstallPath") as string;
                        if (!String.IsNullOrEmpty(value)) yield return value;
                    }
                }
            }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
            {
                if (key != null)
                {
                    string value = key.GetValue("SteamPath") as string;
                    if (!String.IsNullOrEmpty(value)) yield return value.Replace('/', '\\');
                }
            }
        }
    }
}
