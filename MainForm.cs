using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NoteKraken;

public partial class MainForm : Form
{
    // Windows dark mode API
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private string? _currentFile;
    private bool _isDirty;
    private FindReplaceForm? _findReplaceForm;

    // Theme colors - set based on system theme
    private Color _backgroundColor;
    private Color _textColor;
    private Color _menuBackColor;
    private Color _menuBorderColor;
    private Color _highlightColor;
    private bool _isDarkMode;

    public MainForm()
    {
        InitializeComponent();
        LoadIcon();
        DetectAndApplyTheme();
        UpdateTitle();
    }

    private void LoadIcon()
    {
        // Load icon from ICO file next to exe (or use embedded icon)
        string iconPath = Path.Combine(AppContext.BaseDirectory, "kraken_transparent.ico");
        if (File.Exists(iconPath))
        {
            Icon = new Icon(iconPath);
        }
    }

    private static bool IsWindowsDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i == 0;
        }
        catch
        {
            return false; // Default to light if we can't read
        }
    }

    private void DetectAndApplyTheme()
    {
        _isDarkMode = IsWindowsDarkMode();

        if (_isDarkMode)
        {
            _backgroundColor = Color.FromArgb(30, 30, 30);
            _textColor = Color.FromArgb(212, 212, 212);
            _menuBackColor = Color.FromArgb(45, 45, 45);
            _menuBorderColor = Color.FromArgb(60, 60, 60);
            _highlightColor = Color.FromArgb(62, 62, 64);
        }
        else
        {
            _backgroundColor = SystemColors.Window;
            _textColor = SystemColors.WindowText;
            _menuBackColor = SystemColors.Menu;
            _menuBorderColor = SystemColors.ControlDark;
            _highlightColor = SystemColors.Highlight;
        }

        ApplyTheme();
    }

    private void ApplyTheme()
    {
        BackColor = _backgroundColor;
        ForeColor = _textColor;

        // Style the text editor
        txtEditor.BackColor = _backgroundColor;
        txtEditor.ForeColor = _textColor;
        txtEditor.Font = new Font("Consolas", 11f);
        txtEditor.BorderStyle = BorderStyle.None;

        if (_isDarkMode)
        {
            // Custom dark renderer
            var renderer = new ThemedMenuRenderer(_menuBackColor, _menuBorderColor, _highlightColor, _textColor);
            menuStrip.Renderer = renderer;
            statusStrip.Renderer = renderer;
        }
        else
        {
            // Use system default for light mode
            menuStrip.RenderMode = ToolStripRenderMode.System;
            statusStrip.RenderMode = ToolStripRenderMode.System;
        }

        menuStrip.BackColor = _menuBackColor;
        menuStrip.ForeColor = _textColor;
        statusStrip.BackColor = _menuBackColor;
        statusStrip.ForeColor = _textColor;
        lblStatus.ForeColor = _textColor;
        lblPosition.ForeColor = _textColor;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // Set title bar to match theme
        int value = _isDarkMode ? 1 : 0;
        DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
    }

    private void UpdateTitle()
    {
        string fileName = string.IsNullOrEmpty(_currentFile) ? "Untitled" : Path.GetFileName(_currentFile);
        string dirty = _isDirty ? " *" : "";
        Text = $"{fileName}{dirty} - Note Kraken";
    }

    private void UpdatePosition()
    {
        int line = txtEditor.GetLineFromCharIndex(txtEditor.SelectionStart) + 1;
        int col = txtEditor.SelectionStart - txtEditor.GetFirstCharIndexOfCurrentLine() + 1;
        lblPosition.Text = $"Ln {line}, Col {col}";
    }

    private void MarkDirty()
    {
        if (!_isDirty)
        {
            _isDirty = true;
            UpdateTitle();
        }
    }

    private void MarkClean()
    {
        _isDirty = false;
        UpdateTitle();
    }

    private bool PromptSaveChanges()
    {
        if (!_isDirty) return true;

        var result = MessageBox.Show(
            "Do you want to save changes?",
            "Note Kraken",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (result == DialogResult.Cancel) return false;
        if (result == DialogResult.Yes) return SaveFile();
        return true;
    }

    // File Operations
    public void NewFile()
    {
        if (!PromptSaveChanges()) return;

        txtEditor.Clear();
        _currentFile = null;
        MarkClean();
    }

    public void OpenFile(string? path = null)
    {
        if (path == null)
        {
            if (!PromptSaveChanges()) return;

            using var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FilterIndex = 2
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;
            path = dialog.FileName;
        }

        try
        {
            txtEditor.Text = File.ReadAllText(path);
            _currentFile = path;
            MarkClean();
            lblStatus.Text = "Opened";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public bool SaveFile()
    {
        if (string.IsNullOrEmpty(_currentFile))
            return SaveFileAs();

        try
        {
            File.WriteAllText(_currentFile, txtEditor.Text);
            MarkClean();
            lblStatus.Text = "Saved";
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public bool SaveFileAs()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            FilterIndex = 1,
            DefaultExt = "txt"
        };

        if (!string.IsNullOrEmpty(_currentFile))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(_currentFile);
            dialog.FileName = Path.GetFileName(_currentFile);
        }

        if (dialog.ShowDialog() != DialogResult.OK) return false;

        _currentFile = dialog.FileName;
        return SaveFile();
    }

    // Edit Operations
    private void ShowFindReplace(bool showReplace = false)
    {
        if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
        {
            _findReplaceForm = new FindReplaceForm(txtEditor, _isDarkMode);
        }
        _findReplaceForm.ShowReplace = showReplace;
        _findReplaceForm.Show();
        _findReplaceForm.BringToFront();
        _findReplaceForm.Focus();
    }

    private void GoToLine()
    {
        int totalLines = txtEditor.Lines.Length;
        string input = Microsoft.VisualBasic.Interaction.InputBox(
            $"Line number (1-{totalLines}):",
            "Go To Line",
            "1");

        if (int.TryParse(input, out int lineNumber) && lineNumber >= 1 && lineNumber <= totalLines)
        {
            int index = txtEditor.GetFirstCharIndexFromLine(lineNumber - 1);
            txtEditor.SelectionStart = index;
            txtEditor.SelectionLength = 0;
            txtEditor.ScrollToCaret();
            txtEditor.Focus();
        }
    }

    // Event Handlers
    private void txtEditor_TextChanged(object sender, EventArgs e)
    {
        MarkDirty();
    }

    private void txtEditor_SelectionChanged(object sender, EventArgs e)
    {
        UpdatePosition();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!PromptSaveChanges())
            e.Cancel = true;
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            e.Effect = DragDropEffects.Copy;
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            OpenFile(files[0]);
    }

    // Menu Handlers
    private void newToolStripMenuItem_Click(object sender, EventArgs e) => NewFile();
    private void openToolStripMenuItem_Click(object sender, EventArgs e) => OpenFile();
    private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();
    private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

    private void undoToolStripMenuItem_Click(object sender, EventArgs e) => txtEditor.Undo();
    private void cutToolStripMenuItem_Click(object sender, EventArgs e) => txtEditor.Cut();
    private void copyToolStripMenuItem_Click(object sender, EventArgs e) => txtEditor.Copy();
    private void pasteToolStripMenuItem_Click(object sender, EventArgs e) => txtEditor.Paste();
    private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => txtEditor.SelectAll();
    private void findToolStripMenuItem_Click(object sender, EventArgs e) => ShowFindReplace(false);
    private void replaceToolStripMenuItem_Click(object sender, EventArgs e) => ShowFindReplace(true);
    private void goToToolStripMenuItem_Click(object sender, EventArgs e) => GoToLine();

    private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
    {
        txtEditor.WordWrap = !txtEditor.WordWrap;
        wordWrapToolStripMenuItem.Checked = txtEditor.WordWrap;
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "Note Kraken\n\nA fast, simple text editor.\nNo bloat, no AI, just text.",
            "About Note Kraken",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}

// Theme-aware menu renderer
public class ThemedMenuRenderer : ToolStripProfessionalRenderer
{
    private readonly Color _menuBack;
    private readonly Color _menuBorder;
    private readonly Color _highlight;
    private readonly Color _textColor;

    public ThemedMenuRenderer(Color menuBack, Color menuBorder, Color highlight, Color textColor)
        : base(new ThemedColorTable(menuBack, menuBorder, highlight))
    {
        _menuBack = menuBack;
        _menuBorder = menuBorder;
        _highlight = highlight;
        _textColor = textColor;
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
        e.Graphics.DrawLine(pen, 0, y, e.Item.Width, y);
    }
}

public class ThemedColorTable : ProfessionalColorTable
{
    private readonly Color _menuBack;
    private readonly Color _menuBorder;
    private readonly Color _highlight;

    public ThemedColorTable(Color menuBack, Color menuBorder, Color highlight)
    {
        _menuBack = menuBack;
        _menuBorder = menuBorder;
        _highlight = highlight;
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
