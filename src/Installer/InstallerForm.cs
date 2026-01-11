using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NoteKrakenInstaller;

public class InstallerForm : Form
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private readonly string _defaultPath;
    private TextBox _pathBox = null!;
    private CheckBox _desktopShortcut = null!;
    private CheckBox _startMenuShortcut = null!;
    private CheckBox _associateTxt = null!;
    private CheckBox _associateLog = null!;
    private CheckBox _associateIni = null!;
    private CheckBox _associateMd = null!;
    private Button _installBtn = null!;
    private Button _browseBtn = null!;
    private ProgressBar _progress = null!;
    private Label _statusLabel = null!;
    private bool _isDarkMode;

    private static readonly string[] EmbeddedFiles = {
        "NoteKraken.exe",
        "NoteKraken.dll",
        "NoteKraken.deps.json",
        "NoteKraken.runtimeconfig.json",
        "kraken_transparent.ico"
    };

    public InstallerForm()
    {
        _defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoteKraken");
        _isDarkMode = IsWindowsDarkMode();
        InitializeComponent();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int value = _isDarkMode ? 1 : 0;
        DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
    }

    private static bool IsWindowsDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i == 0;
        }
        catch { return false; }
    }

    private void InitializeComponent()
    {
        Text = "Note Kraken Setup";
        Size = new Size(500, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var bgColor = _isDarkMode ? Color.FromArgb(30, 30, 30) : SystemColors.Control;
        var fgColor = _isDarkMode ? Color.FromArgb(212, 212, 212) : SystemColors.ControlText;
        var boxBg = _isDarkMode ? Color.FromArgb(45, 45, 45) : SystemColors.Window;

        BackColor = bgColor;
        ForeColor = fgColor;

        // Title
        var title = new Label
        {
            Text = "Note Kraken",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            Location = new Point(20, 15),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(title);

        var subtitle = new Label
        {
            Text = "A fast, simple text editor. No bloat, no AI, just text.",
            Location = new Point(22, 50),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(subtitle);

        // Install location
        var pathLabel = new Label
        {
            Text = "Install location:",
            Location = new Point(20, 90),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(pathLabel);

        _pathBox = new TextBox
        {
            Text = _defaultPath,
            Location = new Point(20, 110),
            Size = new Size(350, 23),
            BackColor = boxBg,
            ForeColor = fgColor
        };
        Controls.Add(_pathBox);

        _browseBtn = new Button
        {
            Text = "Browse...",
            Location = new Point(380, 109),
            Size = new Size(80, 25),
            FlatStyle = _isDarkMode ? FlatStyle.Flat : FlatStyle.Standard,
            BackColor = _isDarkMode ? Color.FromArgb(60, 60, 60) : SystemColors.Control,
            ForeColor = fgColor
        };
        _browseBtn.Click += BrowseBtn_Click;
        Controls.Add(_browseBtn);

        // Options
        var optionsLabel = new Label
        {
            Text = "Options:",
            Location = new Point(20, 150),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(optionsLabel);

        _desktopShortcut = new CheckBox
        {
            Text = "Create desktop shortcut",
            Checked = true,
            Location = new Point(30, 170),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_desktopShortcut);

        _startMenuShortcut = new CheckBox
        {
            Text = "Create Start Menu shortcut",
            Checked = true,
            Location = new Point(30, 195),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_startMenuShortcut);

        // File associations
        var assocLabel = new Label
        {
            Text = "File associations:",
            Location = new Point(20, 230),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(assocLabel);

        _associateTxt = new CheckBox
        {
            Text = ".txt (Text files)",
            Checked = true,
            Location = new Point(30, 250),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_associateTxt);

        _associateLog = new CheckBox
        {
            Text = ".log (Log files)",
            Checked = false,
            Location = new Point(180, 250),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_associateLog);

        _associateIni = new CheckBox
        {
            Text = ".ini (Config files)",
            Checked = false,
            Location = new Point(30, 275),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_associateIni);

        _associateMd = new CheckBox
        {
            Text = ".md (Markdown)",
            Checked = false,
            Location = new Point(180, 275),
            AutoSize = true,
            ForeColor = fgColor
        };
        Controls.Add(_associateMd);

        // Progress
        _progress = new ProgressBar
        {
            Location = new Point(20, 315),
            Size = new Size(445, 23),
            Visible = false
        };
        Controls.Add(_progress);

        _statusLabel = new Label
        {
            Text = "",
            Location = new Point(20, 315),
            Size = new Size(445, 23),
            ForeColor = fgColor
        };
        Controls.Add(_statusLabel);

        // Install button
        _installBtn = new Button
        {
            Text = "Install",
            Location = new Point(380, 345),
            Size = new Size(85, 30),
            FlatStyle = _isDarkMode ? FlatStyle.Flat : FlatStyle.Standard,
            BackColor = _isDarkMode ? Color.FromArgb(60, 60, 60) : SystemColors.Control,
            ForeColor = fgColor
        };
        _installBtn.Click += InstallBtn_Click;
        Controls.Add(_installBtn);

        var cancelBtn = new Button
        {
            Text = "Cancel",
            Location = new Point(285, 345),
            Size = new Size(85, 30),
            FlatStyle = _isDarkMode ? FlatStyle.Flat : FlatStyle.Standard,
            BackColor = _isDarkMode ? Color.FromArgb(60, 60, 60) : SystemColors.Control,
            ForeColor = fgColor
        };
        cancelBtn.Click += (s, e) => Close();
        Controls.Add(cancelBtn);
    }

    private void BrowseBtn_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select installation folder",
            InitialDirectory = _pathBox.Text
        };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _pathBox.Text = dialog.SelectedPath;
        }
    }

    private async void InstallBtn_Click(object? sender, EventArgs e)
    {
        _installBtn.Enabled = false;
        _browseBtn.Enabled = false;
        _progress.Visible = true;
        _statusLabel.Visible = false;
        _progress.Value = 0;

        try
        {
            string installPath = _pathBox.Text;
            Directory.CreateDirectory(installPath);

            // Extract files
            _progress.Value = 10;
            await Task.Run(() => ExtractFiles(installPath));
            _progress.Value = 50;

            // Register application
            await Task.Run(() => RegisterApplication(installPath));
            _progress.Value = 70;

            string exePath = Path.Combine(installPath, "NoteKraken.exe");
            string iconPath = Path.Combine(installPath, "kraken_transparent.ico");

            // Create shortcuts
            if (_desktopShortcut.Checked)
            {
                string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Note Kraken.lnk");
                CreateShortcut(desktopPath, exePath, iconPath);
            }
            _progress.Value = 80;

            if (_startMenuShortcut.Checked)
            {
                string startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "Note Kraken"
                );
                Directory.CreateDirectory(startMenuPath);
                CreateShortcut(Path.Combine(startMenuPath, "Note Kraken.lnk"), exePath, iconPath);
            }
            _progress.Value = 90;

            // File associations
            RegisterFileAssociations(installPath);
            _progress.Value = 100;

            // Create uninstaller info
            CreateUninstallEntry(installPath);

            MessageBox.Show(
                "Note Kraken has been installed successfully!\n\n" +
                $"Location: {installPath}\n\n" +
                "You can set it as your default text editor in:\n" +
                "Settings → Apps → Default apps",
                "Installation Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Offer to launch
            if (MessageBox.Show("Would you like to launch Note Kraken now?", "Launch", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true
                });
            }

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Installation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _installBtn.Enabled = true;
            _browseBtn.Enabled = true;
            _progress.Visible = false;
        }
    }

    private void ExtractFiles(string installPath)
    {
        var assembly = typeof(InstallerForm).Assembly;
        foreach (var fileName in EmbeddedFiles)
        {
            using var stream = assembly.GetManifestResourceStream(fileName);
            if (stream == null) continue;

            string destPath = Path.Combine(installPath, fileName);
            using var fileStream = File.Create(destPath);
            stream.CopyTo(fileStream);
        }
    }

    private void RegisterApplication(string installPath)
    {
        string exePath = Path.Combine(installPath, "NoteKraken.exe");
        string iconPath = Path.Combine(installPath, "kraken_transparent.ico");

        // Register capabilities
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\NoteKraken\Capabilities"))
        {
            key.SetValue("ApplicationName", "Note Kraken");
            key.SetValue("ApplicationDescription", "A fast, simple text editor. No bloat, no AI, just text.");
        }

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\NoteKraken\Capabilities\FileAssociations"))
        {
            key.SetValue(".txt", "NoteKraken.TextFile");
            key.SetValue(".log", "NoteKraken.TextFile");
            key.SetValue(".ini", "NoteKraken.TextFile");
            key.SetValue(".cfg", "NoteKraken.TextFile");
            key.SetValue(".md", "NoteKraken.TextFile");
        }

        // Register with Windows
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications"))
        {
            key.SetValue("NoteKraken", @"Software\NoteKraken\Capabilities");
        }

        // Register file type
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile"))
        {
            key.SetValue("", "Text File");
            key.SetValue("FriendlyTypeName", "Text File");
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile\DefaultIcon"))
        {
            key.SetValue("", iconPath);
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile\shell\open\command"))
        {
            key.SetValue("", $"\"{exePath}\" \"%1\"");
        }

        // Register in Applications
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe"))
        {
            key.SetValue("FriendlyAppName", "Note Kraken");
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe\shell\open\command"))
        {
            key.SetValue("", $"\"{exePath}\" \"%1\"");
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe\SupportedTypes"))
        {
            key.SetValue(".txt", "");
            key.SetValue(".log", "");
            key.SetValue(".ini", "");
            key.SetValue(".cfg", "");
            key.SetValue(".md", "");
        }
    }

    private void RegisterFileAssociations(string installPath)
    {
        var associations = new List<string>();
        if (_associateTxt.Checked) associations.Add(".txt");
        if (_associateLog.Checked) associations.Add(".log");
        if (_associateIni.Checked) associations.Add(".ini");
        if (_associateMd.Checked) associations.Add(".md");

        foreach (var ext in associations)
        {
            using var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{ext}\OpenWithProgids");
            key.SetValue("NoteKraken.TextFile", "");
        }
    }

    private void CreateUninstallEntry(string installPath)
    {
        string exePath = Path.Combine(installPath, "NoteKraken.exe");
        string iconPath = Path.Combine(installPath, "kraken_transparent.ico");

        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\NoteKraken");
        key.SetValue("DisplayName", "Note Kraken");
        key.SetValue("DisplayIcon", iconPath);
        key.SetValue("DisplayVersion", "1.0");
        key.SetValue("Publisher", "Kraken Software");
        key.SetValue("InstallLocation", installPath);
        key.SetValue("UninstallString", $"powershell -ExecutionPolicy Bypass -Command \"Remove-Item -Recurse -Force '{installPath}'; Remove-Item 'HKCU:\\Software\\NoteKraken' -Recurse -ErrorAction SilentlyContinue; Remove-Item 'HKCU:\\Software\\Classes\\NoteKraken.TextFile' -Recurse -ErrorAction SilentlyContinue; Remove-Item 'HKCU:\\Software\\Classes\\Applications\\NoteKraken.exe' -Recurse -ErrorAction SilentlyContinue; Remove-ItemProperty 'HKCU:\\Software\\RegisteredApplications' -Name 'NoteKraken' -ErrorAction SilentlyContinue; Remove-Item 'HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\NoteKraken' -Recurse\"");
        key.SetValue("NoModify", 1);
        key.SetValue("NoRepair", 1);
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string iconPath)
    {
        // Use PowerShell to create shortcut (avoids COM interop)
        string script = $@"
$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut('{shortcutPath.Replace("'", "''")}')
$Shortcut.TargetPath = '{targetPath.Replace("'", "''")}'
$Shortcut.IconLocation = '{iconPath.Replace("'", "''")}'
$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath)?.Replace("'", "''")}'
$Shortcut.Save()
";
        var psi = new ProcessStartInfo
        {
            FileName = "powershell",
            Arguments = $"-ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}
