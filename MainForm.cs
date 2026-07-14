using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace NoteKraken;

public partial class MainForm : Form
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    private const int DwmwaUseImmersiveDarkMode = 20;
    private const string AppName = "Note Kraken";

    private string? _currentFile;
    private bool _isDirty;
    private bool _suppressDirty;
    private bool _markCleanWhenShown;
    private FindReplaceForm? _findReplaceForm;
    private Encoding _encoding = new UTF8Encoding(false);
    private string _encodingLabel = "UTF-8";
    private string _lineEnding = "\r\n";
    private string _lineEndingLabel = "Windows (CRLF)";
    private readonly PrintDocument _printDocument = new();
    private string _printText = string.Empty;
    private int _printOffset;

    private Color _backgroundColor;
    private Color _textColor;
    private Color _menuBackColor;
    private Color _menuBorderColor;
    private Color _highlightColor;
    private Color _accentColor;
    private bool _isDarkMode;

    public MainForm()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        InitializeComponent();
        LoadIcon();
        DetectAndApplyTheme();
        ConfigurePrinting();
        goToToolStripMenuItem.Enabled = !txtEditor.WordWrap;
        UpdateTitle();
        UpdateStatus();
    }

    private void LoadIcon()
    {
        string iconPath = Path.Combine(AppContext.BaseDirectory, "note-kraken.ico");
        if (File.Exists(iconPath))
        {
            Icon = new Icon(iconPath);
            return;
        }

        using Stream? embedded = typeof(MainForm).Assembly.GetManifestResourceStream("note-kraken.ico");
        if (embedded != null)
        {
            using var loaded = new Icon(embedded);
            Icon = (Icon)loaded.Clone();
        }
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

    private void DetectAndApplyTheme()
    {
        _isDarkMode = IsWindowsDarkMode();

        if (_isDarkMode)
        {
            _backgroundColor = Color.FromArgb(31, 27, 23);
            _textColor = Color.FromArgb(244, 235, 221);
            _menuBackColor = Color.FromArgb(43, 37, 31);
            _menuBorderColor = Color.FromArgb(79, 63, 47);
            _highlightColor = Color.FromArgb(91, 64, 34);
            _accentColor = Color.FromArgb(229, 163, 59);
        }
        else
        {
            _backgroundColor = Color.FromArgb(255, 253, 247);
            _textColor = Color.FromArgb(43, 36, 29);
            _menuBackColor = Color.FromArgb(246, 237, 221);
            _menuBorderColor = Color.FromArgb(213, 194, 166);
            _highlightColor = Color.FromArgb(247, 216, 158);
            _accentColor = Color.FromArgb(185, 101, 24);
        }

        ApplyTheme();
    }

    private void ApplyTheme()
    {
        BackColor = _backgroundColor;
        ForeColor = _textColor;
        txtEditor.BackColor = _backgroundColor;
        txtEditor.ForeColor = _textColor;
        txtEditor.Font = new Font("Cascadia Mono", 11f);
        txtEditor.BorderStyle = BorderStyle.None;

        var renderer = new ThemedMenuRenderer(
            _menuBackColor,
            _menuBorderColor,
            _highlightColor,
            _textColor,
            _accentColor);
        menuStrip.Renderer = renderer;
        statusStrip.Renderer = renderer;
        editorContextMenu.Renderer = renderer;

        menuStrip.BackColor = _menuBackColor;
        menuStrip.ForeColor = _textColor;
        statusStrip.BackColor = _menuBackColor;
        statusStrip.ForeColor = _textColor;
        foreach (ToolStripItem item in statusStrip.Items)
            item.ForeColor = _textColor;
    }

    internal ThemeColors GetThemeColors() => new(
        _isDarkMode,
        _backgroundColor,
        _textColor,
        _menuBackColor,
        _menuBorderColor,
        _highlightColor,
        _accentColor);

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int value = _isDarkMode ? 1 : 0;
        _ = DwmSetWindowAttribute(Handle, DwmwaUseImmersiveDarkMode, ref value, sizeof(int));
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_markCleanWhenShown)
        {
            _markCleanWhenShown = false;
            MarkClean("Opened");
        }
    }

    private void UpdateTitle()
    {
        string fileName = string.IsNullOrEmpty(_currentFile) ? "Untitled" : Path.GetFileName(_currentFile);
        Text = $"{fileName}{(_isDirty ? " *" : string.Empty)} - {AppName}";
    }

    private void UpdateStatus(string? message = null)
    {
        int line = txtEditor.GetLineFromCharIndex(txtEditor.SelectionStart) + 1;
        int lineStart = txtEditor.GetFirstCharIndexOfCurrentLine();
        int column = txtEditor.SelectionStart - Math.Max(0, lineStart) + 1;
        lblStatus.Text = message ?? (_isDirty ? "Modified" : "Ready");
        lblPosition.Text = $"Ln {line}, Col {column}";
        lblZoom.Text = $"{Math.Round(txtEditor.ZoomFactor * 100)}%";
        lblEncoding.Text = _encodingLabel;
        lblLineEnding.Text = _lineEndingLabel;
    }

    private void MarkDirty()
    {
        if (_suppressDirty || _isDirty)
            return;

        _isDirty = true;
        UpdateTitle();
        UpdateStatus();
    }

    private void MarkClean(string? message = null)
    {
        _isDirty = false;
        UpdateTitle();
        UpdateStatus(message);
    }

    private bool PromptSaveChanges()
    {
        if (!_isDirty)
            return true;

        string name = string.IsNullOrEmpty(_currentFile) ? "Untitled" : Path.GetFileName(_currentFile);
        DialogResult result = MessageBox.Show(
            $"Save changes to {name}?",
            AppName,
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        return result switch
        {
            DialogResult.Cancel => false,
            DialogResult.Yes => SaveFile(),
            _ => true
        };
    }

    public void NewFile()
    {
        if (!PromptSaveChanges())
            return;

        _suppressDirty = true;
        txtEditor.Clear();
        _suppressDirty = false;
        _currentFile = null;
        SetEncoding(new UTF8Encoding(false), "UTF-8", markDirty: false);
        SetLineEnding("\r\n", "Windows (CRLF)", markDirty: false);
        MarkClean("New file");
        txtEditor.Focus();
    }

    private void NewWindow()
    {
        try
        {
            string? executable = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(executable))
                Process.Start(new ProcessStartInfo(executable) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            ShowError("Could not open a new window", ex);
        }
    }

    public void OpenFile(string? path = null)
    {
        if (path == null)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Text files|*.txt;*.log;*.ini;*.cfg;*.md|All files|*.*",
                FilterIndex = 1,
                CheckFileExists = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;
            path = dialog.FileName;
        }

        if (!PromptSaveChanges())
            return;

        try
        {
            DocumentData document = ReadDocument(path);
            _suppressDirty = true;
            txtEditor.Text = document.Text;
            txtEditor.SelectionStart = 0;
            txtEditor.SelectionLength = 0;
            _suppressDirty = false;
            _currentFile = Path.GetFullPath(path);
            SetEncoding(document.Encoding, document.EncodingLabel, markDirty: false);
            SetLineEnding(document.LineEnding, document.LineEndingLabel, markDirty: false);
            MarkClean("Opened");
            _markCleanWhenShown = !Visible;
            txtEditor.Focus();
        }
        catch (Exception ex)
        {
            _suppressDirty = false;
            ShowError("Could not open file", ex);
        }
    }

    public bool SaveFile()
    {
        if (string.IsNullOrEmpty(_currentFile))
            return SaveFileAs();

        try
        {
            string text = NormalizeLineEndings(txtEditor.Text, _lineEnding);
            File.WriteAllText(_currentFile, text, _encoding);
            MarkClean("Saved");
            return true;
        }
        catch (Exception ex)
        {
            ShowError("Could not save file", ex);
            return false;
        }
    }

    public bool SaveFileAs()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Text file (*.txt)|*.txt|Log file (*.log)|*.log|Markdown (*.md)|*.md|Configuration (*.ini;*.cfg)|*.ini;*.cfg|All files|*.*",
            FilterIndex = 1,
            DefaultExt = "txt",
            AddExtension = true,
            OverwritePrompt = true
        };

        if (!string.IsNullOrEmpty(_currentFile))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(_currentFile);
            dialog.FileName = Path.GetFileName(_currentFile);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return false;

        string? previousPath = _currentFile;
        _currentFile = dialog.FileName;
        if (SaveFile())
            return true;

        _currentFile = previousPath;
        UpdateTitle();
        return false;
    }

    private static DocumentData ReadDocument(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Encoding encoding;
        string label;
        int preambleLength;

        if (bytes.AsSpan().StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }))
        {
            encoding = new UTF8Encoding(true);
            label = "UTF-8 BOM";
            preambleLength = 3;
        }
        else if (bytes.AsSpan().StartsWith(new byte[] { 0xFF, 0xFE, 0x00, 0x00 }))
        {
            encoding = new UTF32Encoding(false, true);
            label = "UTF-32 LE";
            preambleLength = 4;
        }
        else if (bytes.AsSpan().StartsWith(new byte[] { 0x00, 0x00, 0xFE, 0xFF }))
        {
            encoding = new UTF32Encoding(true, true);
            label = "UTF-32 BE";
            preambleLength = 4;
        }
        else if (bytes.AsSpan().StartsWith(new byte[] { 0xFF, 0xFE }))
        {
            encoding = new UnicodeEncoding(false, true);
            label = "UTF-16 LE";
            preambleLength = 2;
        }
        else if (bytes.AsSpan().StartsWith(new byte[] { 0xFE, 0xFF }))
        {
            encoding = new UnicodeEncoding(true, true);
            label = "UTF-16 BE";
            preambleLength = 2;
        }
        else
        {
            preambleLength = 0;
            try
            {
                var strictUtf8 = new UTF8Encoding(false, true);
                _ = strictUtf8.GetString(bytes);
                encoding = new UTF8Encoding(false);
                label = "UTF-8";
            }
            catch (DecoderFallbackException)
            {
                encoding = Encoding.GetEncoding(1252);
                label = "ANSI (1252)";
            }
        }

        string text = encoding.GetString(bytes, preambleLength, bytes.Length - preambleLength);
        (string ending, string endingLabel) = DetectLineEnding(text);
        return new DocumentData(text, encoding, label, ending, endingLabel);
    }

    private static (string Ending, string Label) DetectLineEnding(string text)
    {
        int crlf = 0;
        int lf = 0;
        int cr = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    crlf++;
                    i++;
                }
                else
                {
                    cr++;
                }
            }
            else if (text[i] == '\n')
            {
                lf++;
            }
        }

        if (lf > crlf && lf >= cr)
            return ("\n", "Unix (LF)");
        if (cr > crlf && cr > lf)
            return ("\r", "Mac (CR)");
        return ("\r\n", "Windows (CRLF)");
    }

    private static string NormalizeLineEndings(string text, string ending) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", ending);

    private void SetEncoding(Encoding encoding, string label, bool markDirty = true)
    {
        bool changed = _encoding.CodePage != encoding.CodePage || _encoding.GetPreamble().Length != encoding.GetPreamble().Length;
        _encoding = encoding;
        _encodingLabel = label;
        utf8ToolStripMenuItem.Checked = label == "UTF-8";
        utf8BomToolStripMenuItem.Checked = label == "UTF-8 BOM";
        utf16ToolStripMenuItem.Checked = label == "UTF-16 LE";
        ansiToolStripMenuItem.Checked = label == "ANSI (1252)";
        if (changed && markDirty)
            MarkDirty();
        UpdateStatus();
    }

    private void SetLineEnding(string ending, string label, bool markDirty = true)
    {
        bool changed = _lineEnding != ending;
        _lineEnding = ending;
        _lineEndingLabel = label;
        windowsLineEndingsToolStripMenuItem.Checked = ending == "\r\n";
        unixLineEndingsToolStripMenuItem.Checked = ending == "\n";
        macLineEndingsToolStripMenuItem.Checked = ending == "\r";
        if (changed && markDirty)
            MarkDirty();
        UpdateStatus();
    }

    private void ShowFindReplace(bool replace)
    {
        if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            _findReplaceForm = new FindReplaceForm(txtEditor, GetThemeColors());

        _findReplaceForm.ShowReplace = replace;
        if (!_findReplaceForm.Visible)
            _findReplaceForm.Show(this);
        _findReplaceForm.BringToFront();
        _findReplaceForm.FocusSearch();
    }

    private void FindNext()
    {
        if (_findReplaceForm == null || string.IsNullOrEmpty(_findReplaceForm.SearchText))
            ShowFindReplace(false);
        else
            _findReplaceForm.FindNext();
    }

    private void GoToLine()
    {
        int totalLines = Math.Max(1, txtEditor.Lines.Length);
        string input = Microsoft.VisualBasic.Interaction.InputBox(
            $"Line number (1-{totalLines}):", "Go To Line", "1");

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (!int.TryParse(input, out int lineNumber) || lineNumber < 1 || lineNumber > totalLines)
        {
            MessageBox.Show($"Enter a line number from 1 to {totalLines}.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int index = txtEditor.GetFirstCharIndexFromLine(lineNumber - 1);
        txtEditor.SelectionStart = Math.Max(0, index);
        txtEditor.SelectionLength = 0;
        txtEditor.ScrollToCaret();
        txtEditor.Focus();
    }

    private void ChooseFont()
    {
        using var dialog = new FontDialog
        {
            Font = txtEditor.Font,
            FontMustExist = true,
            ShowEffects = false
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            txtEditor.Font = dialog.Font;
    }

    private void InsertTimeAndDate()
    {
        txtEditor.SelectedText = DateTime.Now.ToString("g");
        txtEditor.Focus();
    }

    private void SetZoom(float factor)
    {
        txtEditor.ZoomFactor = Math.Clamp(factor, 0.5f, 5f);
        UpdateStatus();
    }

    private void ConfigurePrinting()
    {
        _printDocument.DocumentName = "Note Kraken Document";
        _printDocument.BeginPrint += (_, _) =>
        {
            _printText = txtEditor.Text;
            _printOffset = 0;
        };
        _printDocument.PrintPage += PrintDocument_PrintPage;
    }

    private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
    {
        if (e.Graphics == null)
            return;

        string remaining = _printText[_printOffset..];
        if (remaining.Length == 0)
        {
            e.HasMorePages = false;
            return;
        }

        using var brush = new SolidBrush(Color.Black);
        using var format = new StringFormat(StringFormatFlags.LineLimit)
        {
            Trimming = StringTrimming.Word
        };
        SizeF layout = new(e.MarginBounds.Width, e.MarginBounds.Height);
        e.Graphics.MeasureString(remaining, txtEditor.Font, layout, format, out int charactersFitted, out _);
        if (charactersFitted <= 0)
            charactersFitted = Math.Min(1, remaining.Length);

        e.Graphics.DrawString(remaining[..charactersFitted], txtEditor.Font, brush, e.MarginBounds, format);
        _printOffset += charactersFitted;
        e.HasMorePages = _printOffset < _printText.Length;
    }

    private void PageSetup()
    {
        using var dialog = new PageSetupDialog
        {
            Document = _printDocument,
            EnableMetric = true
        };
        _ = dialog.ShowDialog(this);
    }

    private void Print()
    {
        using var dialog = new PrintDialog
        {
            Document = _printDocument,
            UseEXDialog = true
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _printDocument.Print();
    }

    private static void ShowError(string action, Exception ex) =>
        MessageBox.Show($"{action}: {ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private void UpdateEditCommands()
    {
        bool selection = txtEditor.SelectionLength > 0;
        bool clipboardText = Clipboard.ContainsText();
        undoToolStripMenuItem.Enabled = txtEditor.CanUndo;
        redoToolStripMenuItem.Enabled = txtEditor.CanRedo;
        cutToolStripMenuItem.Enabled = selection;
        copyToolStripMenuItem.Enabled = selection;
        deleteToolStripMenuItem.Enabled = selection;
        pasteToolStripMenuItem.Enabled = clipboardText;
        ctxUndo.Enabled = txtEditor.CanUndo;
        ctxRedo.Enabled = txtEditor.CanRedo;
        ctxCut.Enabled = selection;
        ctxCopy.Enabled = selection;
        ctxPaste.Enabled = clipboardText;
        ctxDelete.Enabled = selection;
    }

    private void txtEditor_TextChanged(object? sender, EventArgs e) => MarkDirty();
    private void txtEditor_SelectionChanged(object? sender, EventArgs e) => UpdateStatus();

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!PromptSaveChanges())
            e.Cancel = true;
    }

    private void MainForm_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            e.Effect = DragDropEffects.Copy;
    }

    private void MainForm_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            OpenFile(files[0]);
    }

    private void newToolStripMenuItem_Click(object? sender, EventArgs e) => NewFile();
    private void newWindowToolStripMenuItem_Click(object? sender, EventArgs e) => NewWindow();
    private void openToolStripMenuItem_Click(object? sender, EventArgs e) => OpenFile();
    private void saveToolStripMenuItem_Click(object? sender, EventArgs e) => SaveFile();
    private void saveAsToolStripMenuItem_Click(object? sender, EventArgs e) => SaveFileAs();
    private void pageSetupToolStripMenuItem_Click(object? sender, EventArgs e) => PageSetup();
    private void printToolStripMenuItem_Click(object? sender, EventArgs e) => Print();
    private void exitToolStripMenuItem_Click(object? sender, EventArgs e) => Close();
    private void undoToolStripMenuItem_Click(object? sender, EventArgs e) { if (txtEditor.CanUndo) txtEditor.Undo(); }
    private void redoToolStripMenuItem_Click(object? sender, EventArgs e) { if (txtEditor.CanRedo) txtEditor.Redo(); }
    private void cutToolStripMenuItem_Click(object? sender, EventArgs e) => txtEditor.Cut();
    private void copyToolStripMenuItem_Click(object? sender, EventArgs e) => txtEditor.Copy();
    private void pasteToolStripMenuItem_Click(object? sender, EventArgs e) => txtEditor.Paste();
    private void deleteToolStripMenuItem_Click(object? sender, EventArgs e) { if (txtEditor.SelectionLength > 0) txtEditor.SelectedText = string.Empty; }
    private void selectAllToolStripMenuItem_Click(object? sender, EventArgs e) => txtEditor.SelectAll();
    private void findToolStripMenuItem_Click(object? sender, EventArgs e) => ShowFindReplace(false);
    private void findNextToolStripMenuItem_Click(object? sender, EventArgs e) => FindNext();
    private void replaceToolStripMenuItem_Click(object? sender, EventArgs e) => ShowFindReplace(true);
    private void goToToolStripMenuItem_Click(object? sender, EventArgs e) => GoToLine();
    private void timeDateToolStripMenuItem_Click(object? sender, EventArgs e) => InsertTimeAndDate();
    private void editMenu_DropDownOpening(object? sender, EventArgs e) => UpdateEditCommands();
    private void editorContextMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e) => UpdateEditCommands();

    private void wordWrapToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        txtEditor.WordWrap = !txtEditor.WordWrap;
        wordWrapToolStripMenuItem.Checked = txtEditor.WordWrap;
        goToToolStripMenuItem.Enabled = !txtEditor.WordWrap;
    }

    private void fontToolStripMenuItem_Click(object? sender, EventArgs e) => ChooseFont();
    private void utf8ToolStripMenuItem_Click(object? sender, EventArgs e) => SetEncoding(new UTF8Encoding(false), "UTF-8");
    private void utf8BomToolStripMenuItem_Click(object? sender, EventArgs e) => SetEncoding(new UTF8Encoding(true), "UTF-8 BOM");
    private void utf16ToolStripMenuItem_Click(object? sender, EventArgs e) => SetEncoding(new UnicodeEncoding(false, true), "UTF-16 LE");
    private void ansiToolStripMenuItem_Click(object? sender, EventArgs e) => SetEncoding(Encoding.GetEncoding(1252), "ANSI (1252)");
    private void windowsLineEndingsToolStripMenuItem_Click(object? sender, EventArgs e) => SetLineEnding("\r\n", "Windows (CRLF)");
    private void unixLineEndingsToolStripMenuItem_Click(object? sender, EventArgs e) => SetLineEnding("\n", "Unix (LF)");
    private void macLineEndingsToolStripMenuItem_Click(object? sender, EventArgs e) => SetLineEnding("\r", "Mac (CR)");
    private void zoomInToolStripMenuItem_Click(object? sender, EventArgs e) => SetZoom(txtEditor.ZoomFactor + 0.1f);
    private void zoomOutToolStripMenuItem_Click(object? sender, EventArgs e) => SetZoom(txtEditor.ZoomFactor - 0.1f);
    private void restoreZoomToolStripMenuItem_Click(object? sender, EventArgs e) => SetZoom(1f);

    private void statusBarToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        statusStrip.Visible = !statusStrip.Visible;
        statusBarToolStripMenuItem.Checked = statusStrip.Visible;
    }

    private void aboutToolStripMenuItem_Click(object? sender, EventArgs e) =>
        MessageBox.Show(
            "Note Kraken 1.1\n\nA fast, focused text editor for Windows.\nNo bloat. No AI. Just text.",
            $"About {AppName}",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    private sealed record DocumentData(
        string Text,
        Encoding Encoding,
        string EncodingLabel,
        string LineEnding,
        string LineEndingLabel);
}

internal sealed record ThemeColors(
    bool IsDark,
    Color Background,
    Color Text,
    Color MenuBackground,
    Color Border,
    Color Highlight,
    Color Accent);

public sealed class ThemedMenuRenderer : ToolStripProfessionalRenderer
{
    private readonly Color _menuBack;
    private readonly Color _menuBorder;
    private readonly Color _highlight;
    private readonly Color _textColor;
    private readonly Color _accent;

    public ThemedMenuRenderer(Color menuBack, Color menuBorder, Color highlight, Color textColor, Color accent)
        : base(new ThemedColorTable(menuBack, menuBorder, highlight))
    {
        _menuBack = menuBack;
        _menuBorder = menuBorder;
        _highlight = highlight;
        _textColor = textColor;
        _accent = accent;
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        using var brush = new SolidBrush(_menuBack);
        e.Graphics.FillRectangle(brush, e.AffectedBounds);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected || e.Item.Pressed)
        {
            using var brush = new SolidBrush(_highlight);
            e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.TextColor = _textColor;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        e.ArrowColor = _textColor;
        base.OnRenderArrow(e);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        using var pen = new Pen(_menuBorder);
        int y = e.Item.Height / 2;
        e.Graphics.DrawLine(pen, 4, y, Math.Max(4, e.Item.Width - 4), y);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        using var pen = new Pen(_accent);
        e.Graphics.DrawLine(pen, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
    }
}

public sealed class ThemedColorTable : ProfessionalColorTable
{
    private readonly Color _menuBack;
    private readonly Color _menuBorder;
    private readonly Color _highlight;

    public ThemedColorTable(Color menuBack, Color menuBorder, Color highlight)
    {
        _menuBack = menuBack;
        _menuBorder = menuBorder;
        _highlight = highlight;
        UseSystemColors = false;
    }

    public override Color MenuStripGradientBegin => _menuBack;
    public override Color MenuStripGradientEnd => _menuBack;
    public override Color ToolStripGradientBegin => _menuBack;
    public override Color ToolStripGradientMiddle => _menuBack;
    public override Color ToolStripGradientEnd => _menuBack;
    public override Color MenuItemSelected => _highlight;
    public override Color MenuItemSelectedGradientBegin => _highlight;
    public override Color MenuItemSelectedGradientEnd => _highlight;
    public override Color MenuItemPressedGradientBegin => _menuBack;
    public override Color MenuItemPressedGradientEnd => _menuBack;
    public override Color MenuBorder => _menuBorder;
    public override Color MenuItemBorder => _menuBorder;
    public override Color ToolStripDropDownBackground => _menuBack;
    public override Color ImageMarginGradientBegin => _menuBack;
    public override Color ImageMarginGradientMiddle => _menuBack;
    public override Color ImageMarginGradientEnd => _menuBack;
    public override Color SeparatorDark => _menuBorder;
    public override Color SeparatorLight => _menuBack;
    public override Color StatusStripGradientBegin => _menuBack;
    public override Color StatusStripGradientEnd => _menuBack;
}
