using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace NoteKrakenInstaller;

public sealed class InstallerForm : Form
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(uint eventId, uint flags, IntPtr item1, IntPtr item2);

    private const int DwmwaUseImmersiveDarkMode = 20;
    private const uint ShcneAssocChanged = 0x08000000;
    private const uint ShcnfIdList = 0x0000;
    private const string AppVersion = "1.1.0";

    private readonly string _defaultPath;
    private readonly bool _isDarkMode;
    private TextBox _pathBox = null!;
    private CheckBox _desktopShortcut = null!;
    private CheckBox _startMenuShortcut = null!;
    private CheckBox _associateTxt = null!;
    private CheckBox _associateLog = null!;
    private CheckBox _associateIni = null!;
    private CheckBox _associateMd = null!;
    private Button _installButton = null!;
    private Button _browseButton = null!;
    private ProgressBar _progress = null!;
    private Label _statusLabel = null!;

    private static readonly string[] EmbeddedFiles = { "NoteKraken.exe", "note-kraken.ico" };

    public InstallerForm()
    {
        _defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NoteKraken");
        _isDarkMode = IsWindowsDarkMode();
        InitializeComponent();
        LoadInstallerIcon();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int value = _isDarkMode ? 1 : 0;
        _ = DwmSetWindowAttribute(Handle, DwmwaUseImmersiveDarkMode, ref value, sizeof(int));
    }

    private static bool IsWindowsDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch
        {
            return false;
        }
    }

    private void LoadInstallerIcon()
    {
        try
        {
            using Stream? stream = typeof(InstallerForm).Assembly.GetManifestResourceStream("note-kraken.ico");
            if (stream != null)
                Icon = new Icon(stream);
        }
        catch
        {
            // The executable still has the compiled application icon.
        }
    }

    private void InitializeComponent()
    {
        Text = $"Note Kraken {AppVersion} Setup";
        ClientSize = new Size(520, 430);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Color background = _isDarkMode ? Color.FromArgb(31, 27, 23) : Color.FromArgb(255, 253, 247);
        Color foreground = _isDarkMode ? Color.FromArgb(244, 235, 221) : Color.FromArgb(43, 36, 29);
        Color panel = _isDarkMode ? Color.FromArgb(43, 37, 31) : Color.FromArgb(246, 237, 221);
        Color accent = _isDarkMode ? Color.FromArgb(229, 163, 59) : Color.FromArgb(185, 101, 24);

        BackColor = background;
        ForeColor = foreground;

        var title = new Label
        {
            Text = "Note Kraken",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = accent,
            Location = new Point(22, 16),
            AutoSize = true
        };
        var subtitle = new Label
        {
            Text = "A fast, focused text editor. No bloat. No AI. Just text.",
            ForeColor = foreground,
            Location = new Point(25, 56),
            AutoSize = true
        };
        var pathLabel = new Label
        {
            Text = "Install location:",
            ForeColor = foreground,
            Location = new Point(22, 94),
            AutoSize = true
        };

        _pathBox = new TextBox
        {
            Text = _defaultPath,
            Location = new Point(22, 116),
            Size = new Size(374, 23),
            BackColor = panel,
            ForeColor = foreground,
            BorderStyle = BorderStyle.FixedSingle
        };
        _browseButton = MakeButton("Browse...", new Point(407, 114), new Size(88, 28), panel, foreground, accent);
        _browseButton.Click += BrowseButton_Click;

        var optionsLabel = new Label
        {
            Text = "Shortcuts",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = accent,
            Location = new Point(22, 160),
            AutoSize = true
        };
        _desktopShortcut = MakeCheckBox("Desktop shortcut", true, new Point(32, 184), foreground);
        _startMenuShortcut = MakeCheckBox("Start Menu shortcut", true, new Point(210, 184), foreground);

        var associationsLabel = new Label
        {
            Text = "Available in Open With / Default Apps",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = accent,
            Location = new Point(22, 222),
            AutoSize = true
        };
        _associateTxt = MakeCheckBox(".txt  Text", true, new Point(32, 248), foreground);
        _associateLog = MakeCheckBox(".log  Logs", false, new Point(210, 248), foreground);
        _associateIni = MakeCheckBox(".ini / .cfg  Configuration", false, new Point(32, 276), foreground);
        _associateMd = MakeCheckBox(".md  Markdown", false, new Point(270, 276), foreground);

        var associationNote = new Label
        {
            Text = "Windows may ask you to confirm the default app the first time each type is opened.",
            ForeColor = foreground,
            Location = new Point(32, 306),
            AutoSize = true
        };

        _progress = new ProgressBar
        {
            Location = new Point(22, 340),
            Size = new Size(473, 20),
            Visible = false
        };
        _statusLabel = new Label
        {
            Location = new Point(22, 342),
            Size = new Size(473, 20),
            ForeColor = foreground
        };

        _installButton = MakeButton("Install", new Point(407, 378), new Size(88, 32), accent, Color.FromArgb(31, 27, 23), accent);
        _installButton.Click += InstallButton_Click;
        var cancelButton = MakeButton("Cancel", new Point(309, 378), new Size(88, 32), panel, foreground, accent);
        cancelButton.Click += (_, _) => Close();

        AcceptButton = _installButton;
        CancelButton = cancelButton;
        Controls.AddRange(new Control[]
        {
            title, subtitle, pathLabel, _pathBox, _browseButton, optionsLabel,
            _desktopShortcut, _startMenuShortcut, associationsLabel, _associateTxt,
            _associateLog, _associateIni, _associateMd, associationNote,
            _progress, _statusLabel, _installButton, cancelButton
        });
    }

    private static Button MakeButton(string text, Point location, Size size, Color back, Color fore, Color border)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = back,
            ForeColor = fore,
            FlatStyle = FlatStyle.Flat
        };
        button.FlatAppearance.BorderColor = border;
        return button;
    }

    private static CheckBox MakeCheckBox(string text, bool value, Point location, Color foreground) => new()
    {
        Text = text,
        Checked = value,
        Location = location,
        AutoSize = true,
        ForeColor = foreground
    };

    private void BrowseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Choose where Note Kraken will be installed",
            InitialDirectory = Directory.Exists(_pathBox.Text) ? _pathBox.Text : _defaultPath,
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _pathBox.Text = dialog.SelectedPath;
    }

    private async void InstallButton_Click(object? sender, EventArgs e)
    {
        string installPath;
        try
        {
            if (string.IsNullOrWhiteSpace(_pathBox.Text))
                throw new InvalidOperationException("Choose an installation folder.");
            installPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(_pathBox.Text.Trim()));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Invalid install location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetBusy(true);
        try
        {
            Directory.CreateDirectory(installPath);
            await Task.Run(() => ExtractFiles(installPath));
            _progress.Value = 35;

            await Task.Run(() => RegisterApplication(installPath));
            _progress.Value = 55;

            string executable = Path.Combine(installPath, "NoteKraken.exe");
            string icon = Path.Combine(installPath, "note-kraken.ico");
            if (!File.Exists(executable))
                throw new FileNotFoundException("The editor executable was not installed.", executable);

            if (_desktopShortcut.Checked)
            {
                string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Note Kraken.lnk");
                CreateShortcut(desktop, executable, icon);
            }
            _progress.Value = 70;

            if (_startMenuShortcut.Checked)
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    "Note Kraken");
                Directory.CreateDirectory(folder);
                CreateShortcut(Path.Combine(folder, "Note Kraken.lnk"), executable, icon);
            }
            _progress.Value = 82;

            RegisterSelectedAssociations();
            CreateUninstallEntry(installPath);
            SHChangeNotify(ShcneAssocChanged, ShcnfIdList, IntPtr.Zero, IntPtr.Zero);
            _progress.Value = 100;

            DialogResult launch = MessageBox.Show(
                $"Note Kraken {AppVersion} was installed successfully.\n\n" +
                $"Location: {installPath}\n\n" +
                "Selected file types are now available under Open With and Windows Default Apps.\n\n" +
                "Launch Note Kraken now?",
                "Installation complete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (launch == DialogResult.Yes)
                Process.Start(new ProcessStartInfo(executable) { UseShellExecute = true });
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Installation failed: {ex.Message}", "Note Kraken Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        _installButton.Enabled = !busy;
        _browseButton.Enabled = !busy;
        _progress.Visible = busy;
        _statusLabel.Visible = !busy;
        if (busy)
            _progress.Value = 5;
    }

    private static void ExtractFiles(string installPath)
    {
        var assembly = typeof(InstallerForm).Assembly;
        foreach (string fileName in EmbeddedFiles)
        {
            using Stream stream = assembly.GetManifestResourceStream(fileName)
                ?? throw new InvalidOperationException($"Installer resource is missing: {fileName}");
            string destination = Path.Combine(installPath, fileName);
            using var file = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.CopyTo(file);
        }
    }

    private static void RegisterApplication(string installPath)
    {
        string executable = Path.Combine(installPath, "NoteKraken.exe");
        string icon = Path.Combine(installPath, "note-kraken.ico");

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\NoteKraken\Capabilities"))
        {
            key.SetValue("ApplicationName", "Note Kraken");
            key.SetValue("ApplicationDescription", "A fast, focused text editor for Windows.");
            key.SetValue("ApplicationIcon", icon);
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\NoteKraken\Capabilities\FileAssociations"))
        {
            foreach (string extension in SupportedExtensions())
                key.SetValue(extension, "NoteKraken.TextFile");
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications"))
            key.SetValue("NoteKraken", @"Software\NoteKraken\Capabilities");

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile"))
        {
            key.SetValue(string.Empty, "Text Document");
            key.SetValue("FriendlyTypeName", "Text Document");
        }
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile\DefaultIcon"))
            key.SetValue(string.Empty, $"\"{icon}\"");
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\NoteKraken.TextFile\shell\open\command"))
            key.SetValue(string.Empty, $"\"{executable}\" \"%1\"");

        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe"))
            key.SetValue("FriendlyAppName", "Note Kraken");
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe\DefaultIcon"))
            key.SetValue(string.Empty, $"\"{icon}\"");
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe\shell\open\command"))
            key.SetValue(string.Empty, $"\"{executable}\" \"%1\"");
        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\NoteKraken.exe\SupportedTypes"))
        {
            foreach (string extension in SupportedExtensions())
                key.SetValue(extension, string.Empty);
        }
    }

    private void RegisterSelectedAssociations()
    {
        var selected = new List<string>();
        if (_associateTxt.Checked) selected.Add(".txt");
        if (_associateLog.Checked) selected.Add(".log");
        if (_associateIni.Checked) selected.AddRange(new[] { ".ini", ".cfg" });
        if (_associateMd.Checked) selected.Add(".md");

        foreach (string extension in selected)
        {
            using var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{extension}\OpenWithProgids");
            key.SetValue("NoteKraken.TextFile", Array.Empty<byte>(), RegistryValueKind.None);
        }
    }

    private static string[] SupportedExtensions() => new[] { ".txt", ".log", ".ini", ".cfg", ".md" };

    private static void CreateUninstallEntry(string installPath)
    {
        string icon = Path.Combine(installPath, "note-kraken.ico");
        string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Note Kraken.lnk");
        string startMenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Note Kraken");
        string extensions = string.Join(",", SupportedExtensions().Select(extension => $"'{extension}'"));
        string script = $@"
$ErrorActionPreference = 'SilentlyContinue'
Get-Process -Name 'NoteKraken' | Stop-Process -Force
Remove-Item -LiteralPath '{EscapePowerShellLiteral(desktop)}' -Force
Remove-Item -LiteralPath '{EscapePowerShellLiteral(startMenu)}' -Recurse -Force
foreach ($ext in @({extensions})) {{ Remove-ItemProperty -LiteralPath ('HKCU:\Software\Classes\' + $ext + '\OpenWithProgids') -Name 'NoteKraken.TextFile' }}
Remove-Item -LiteralPath 'HKCU:\Software\NoteKraken' -Recurse -Force
Remove-Item -LiteralPath 'HKCU:\Software\Classes\NoteKraken.TextFile' -Recurse -Force
Remove-Item -LiteralPath 'HKCU:\Software\Classes\Applications\NoteKraken.exe' -Recurse -Force
Remove-ItemProperty -LiteralPath 'HKCU:\Software\RegisteredApplications' -Name 'NoteKraken'
Remove-Item -LiteralPath '{EscapePowerShellLiteral(installPath)}' -Recurse -Force
Remove-Item -LiteralPath 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\NoteKraken' -Recurse -Force
";
        string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        string uninstall = $"powershell.exe -NoProfile -ExecutionPolicy Bypass -EncodedCommand {encoded}";

        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\NoteKraken");
        key.SetValue("DisplayName", "Note Kraken");
        key.SetValue("DisplayIcon", icon);
        key.SetValue("DisplayVersion", AppVersion);
        key.SetValue("Publisher", "Kraken Unbound");
        key.SetValue("InstallLocation", installPath);
        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
        key.SetValue("UninstallString", uninstall);
        key.SetValue("QuietUninstallString", uninstall.Replace("powershell.exe ", "powershell.exe -WindowStyle Hidden "));
        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string iconPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath)!);
        string script = $@"
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut('{EscapePowerShellLiteral(shortcutPath)}')
$shortcut.TargetPath = '{EscapePowerShellLiteral(targetPath)}'
$shortcut.IconLocation = '{EscapePowerShellLiteral(iconPath)}'
$shortcut.WorkingDirectory = '{EscapePowerShellLiteral(Path.GetDirectoryName(targetPath)!)}'
$shortcut.Save()
";
        string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand {encoded}",
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException("Could not create a Windows shortcut.");
        process.WaitForExit();
        if (process.ExitCode != 0 || !File.Exists(shortcutPath))
            throw new InvalidOperationException($"Could not create shortcut: {shortcutPath}");
    }

    private static string EscapePowerShellLiteral(string value) => value.Replace("'", "''");
}
