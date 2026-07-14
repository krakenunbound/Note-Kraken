using System.Runtime.InteropServices;
using System.Text;

namespace NoteKraken;

public partial class FindReplaceForm : Form
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    private const int DwmwaUseImmersiveDarkMode = 20;
    private readonly RichTextBox _editor;
    private readonly ThemeColors _theme;
    private bool _showReplace;

    public string SearchText => txtFind.Text;

    public bool ShowReplace
    {
        get => _showReplace;
        set
        {
            _showReplace = value;
            ApplyModeLayout();
        }
    }

    internal FindReplaceForm(RichTextBox editor, ThemeColors theme)
    {
        _editor = editor;
        _theme = theme;
        InitializeComponent();
        ApplyTheme();
        ApplyModeLayout();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int value = _theme.IsDark ? 1 : 0;
        _ = DwmSetWindowAttribute(Handle, DwmwaUseImmersiveDarkMode, ref value, sizeof(int));
    }

    public void FocusSearch()
    {
        txtFind.Focus();
        txtFind.SelectAll();
    }

    private void ApplyModeLayout()
    {
        if (lblReplace == null)
            return;

        lblReplace.Visible = _showReplace;
        txtReplace.Visible = _showReplace;
        btnReplace.Visible = _showReplace;
        btnReplaceAll.Visible = _showReplace;
        Text = _showReplace ? "Replace" : "Find";

        int offset = _showReplace ? 0 : -30;
        chkMatchCase.Location = new Point(70, 76 + offset);
        chkWholeWord.Location = new Point(180, 76 + offset);
        lblDirection.Location = new Point(12, 106 + offset);
        radioDown.Location = new Point(70, 104 + offset);
        radioUp.Location = new Point(135, 104 + offset);
        chkWrapAround.Location = new Point(220, 104 + offset);
        lblStatus.Location = new Point(70, 134 + offset);
        ClientSize = new Size(425, 165 + offset);
    }

    private void ApplyTheme()
    {
        BackColor = _theme.MenuBackground;
        ForeColor = _theme.Text;
        foreach (Control control in Controls)
        {
            control.ForeColor = _theme.Text;
            if (control is TextBox box)
            {
                box.BackColor = _theme.Background;
                box.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is Button button)
            {
                button.BackColor = _theme.Highlight;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = _theme.Accent;
            }
        }
        lblStatus.ForeColor = _theme.Accent;
    }

    public void FindNext()
    {
        string searchText = txtFind.Text;
        if (string.IsNullOrEmpty(searchText))
        {
            lblStatus.Text = "Enter search text";
            FocusSearch();
            return;
        }

        bool forward = radioDown.Checked;
        int start = forward
            ? _editor.SelectionStart + _editor.SelectionLength
            : _editor.SelectionStart - 1;
        int found = FindIndex(_editor.Text, searchText, start, forward, allowWrap: chkWrapAround.Checked);

        if (found < 0)
        {
            lblStatus.Text = $"Cannot find “{searchText}”";
            return;
        }

        _editor.SelectionStart = found;
        _editor.SelectionLength = searchText.Length;
        _editor.ScrollToCaret();
        _editor.Focus();
        lblStatus.Text = string.Empty;
    }

    private int FindIndex(string text, string search, int start, bool forward, bool allowWrap)
    {
        StringComparison comparison = chkMatchCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        if (forward)
        {
            int index = FindForward(text, search, Math.Clamp(start, 0, text.Length), comparison, text.Length);
            if (index < 0 && allowWrap && start > 0)
                index = FindForward(text, search, 0, comparison, Math.Min(start, text.Length));
            return index;
        }

        int backwardStart = Math.Min(start, text.Length - 1);
        int result = FindBackward(text, search, backwardStart, comparison, 0);
        if (result < 0 && allowWrap && start < text.Length - 1)
            result = FindBackward(text, search, text.Length - 1, comparison, Math.Max(0, start + 1));
        return result;
    }

    private int FindForward(string text, string search, int start, StringComparison comparison, int limit)
    {
        int index = start;
        while (index <= text.Length - search.Length)
        {
            index = text.IndexOf(search, index, comparison);
            if (index < 0 || index >= limit)
                return -1;
            if (!chkWholeWord.Checked || IsWholeWord(text, index, search.Length))
                return index;
            index++;
        }
        return -1;
    }

    private int FindBackward(string text, string search, int start, StringComparison comparison, int lowerLimit)
    {
        if (text.Length == 0 || start < 0)
            return -1;

        int index = Math.Min(start, text.Length - 1);
        while (index >= lowerLimit)
        {
            index = text.LastIndexOf(search, index, comparison);
            if (index < lowerLimit)
                return -1;
            if (!chkWholeWord.Checked || IsWholeWord(text, index, search.Length))
                return index;
            index--;
        }
        return -1;
    }

    private static bool IsWholeWord(string text, int index, int length)
    {
        bool leftBoundary = index == 0 || !IsWordCharacter(text[index - 1]);
        int after = index + length;
        bool rightBoundary = after >= text.Length || !IsWordCharacter(text[after]);
        return leftBoundary && rightBoundary;
    }

    private static bool IsWordCharacter(char c) => char.IsLetterOrDigit(c) || c == '_';

    private bool SelectionMatchesSearch()
    {
        if (_editor.SelectionLength != txtFind.Text.Length)
            return false;

        StringComparison comparison = chkMatchCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        return string.Equals(_editor.SelectedText, txtFind.Text, comparison)
            && (!chkWholeWord.Checked || IsWholeWord(_editor.Text, _editor.SelectionStart, _editor.SelectionLength));
    }

    private void Replace()
    {
        if (string.IsNullOrEmpty(txtFind.Text))
        {
            lblStatus.Text = "Enter search text";
            return;
        }

        if (!SelectionMatchesSearch())
            FindNext();

        if (SelectionMatchesSearch())
            _editor.SelectedText = txtReplace.Text;

        FindNext();
    }

    private void ReplaceAll()
    {
        string search = txtFind.Text;
        if (string.IsNullOrEmpty(search))
        {
            lblStatus.Text = "Enter search text";
            return;
        }

        string source = _editor.Text;
        string replacement = txtReplace.Text;
        StringComparison comparison = chkMatchCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        var output = new StringBuilder(source.Length);
        int position = 0;
        int count = 0;

        while (position <= source.Length - search.Length)
        {
            int found = source.IndexOf(search, position, comparison);
            if (found < 0)
                break;
            if (chkWholeWord.Checked && !IsWholeWord(source, found, search.Length))
            {
                output.Append(source, position, found - position + 1);
                position = found + 1;
                continue;
            }

            output.Append(source, position, found - position);
            output.Append(replacement);
            position = found + search.Length;
            count++;
        }
        output.Append(source, position, source.Length - position);

        if (count == 0)
        {
            lblStatus.Text = "Not found";
            return;
        }

        int caret = _editor.SelectionStart;
        _editor.Text = output.ToString();
        _editor.SelectionStart = Math.Min(caret, _editor.TextLength);
        _editor.SelectionLength = 0;
        lblStatus.Text = $"Replaced {count} occurrence{(count == 1 ? string.Empty : "s")}";
    }

    private void btnFindNext_Click(object? sender, EventArgs e) => FindNext();
    private void btnReplace_Click(object? sender, EventArgs e) => Replace();
    private void btnReplaceAll_Click(object? sender, EventArgs e) => ReplaceAll();
    private void txtFind_TextChanged(object? sender, EventArgs e) => lblStatus.Text = string.Empty;

    private void FindReplaceForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Hide();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            FindNext();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnFormClosing(e);
    }
}
