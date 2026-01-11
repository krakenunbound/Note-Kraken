using System.Runtime.InteropServices;

namespace KrakenPad;

public partial class FindReplaceForm : Form
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private readonly RichTextBox _editor;
    private readonly bool _isDarkMode;
    private int _lastFoundIndex = -1;

    public bool ShowReplace
    {
        get => lblReplace.Visible;
        set
        {
            lblReplace.Visible = value;
            txtReplace.Visible = value;
            btnReplace.Visible = value;
            btnReplaceAll.Visible = value;
            Text = value ? "Replace" : "Find";

            // Adjust form height
            Height = value ? 180 : 120;
        }
    }

    public FindReplaceForm(RichTextBox editor, bool isDarkMode)
    {
        _editor = editor;
        _isDarkMode = isDarkMode;
        InitializeComponent();
        SetupTheme();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        int value = _isDarkMode ? 1 : 0;
        DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
    }

    private void SetupTheme()
    {
        if (_isDarkMode)
        {
            var backgroundColor = Color.FromArgb(45, 45, 45);
            var textColor = Color.FromArgb(212, 212, 212);
            var textBoxBack = Color.FromArgb(30, 30, 30);
            var buttonBack = Color.FromArgb(60, 60, 60);

            BackColor = backgroundColor;
            ForeColor = textColor;

            txtFind.BackColor = textBoxBack;
            txtFind.ForeColor = textColor;
            txtReplace.BackColor = textBoxBack;
            txtReplace.ForeColor = textColor;

            foreach (Control c in Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = buttonBack;
                    btn.ForeColor = textColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 80);
                }
                else if (c is CheckBox chk)
                {
                    chk.ForeColor = textColor;
                }
            }

            lblStatus.ForeColor = Color.FromArgb(255, 128, 128);
        }
        // Light mode uses system defaults
    }

    private void FindNext()
    {
        string searchText = txtFind.Text;
        if (string.IsNullOrEmpty(searchText))
        {
            lblStatus.Text = "Enter search text";
            return;
        }

        StringComparison comparison = chkMatchCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        int startIndex = _lastFoundIndex >= 0 ? _lastFoundIndex + 1 : _editor.SelectionStart;
        if (startIndex >= _editor.Text.Length) startIndex = 0;

        int foundIndex = _editor.Text.IndexOf(searchText, startIndex, comparison);

        if (foundIndex < 0 && startIndex > 0)
        {
            // Wrap around
            foundIndex = _editor.Text.IndexOf(searchText, 0, comparison);
        }

        if (foundIndex >= 0)
        {
            _editor.SelectionStart = foundIndex;
            _editor.SelectionLength = searchText.Length;
            _editor.ScrollToCaret();
            _editor.Focus();
            _lastFoundIndex = foundIndex;
            lblStatus.Text = "";
        }
        else
        {
            lblStatus.Text = "Not found";
            _lastFoundIndex = -1;
        }
    }

    private void Replace()
    {
        if (_editor.SelectionLength > 0)
        {
            _editor.SelectedText = txtReplace.Text;
        }
        FindNext();
    }

    private void ReplaceAll()
    {
        string searchText = txtFind.Text;
        string replaceText = txtReplace.Text;

        if (string.IsNullOrEmpty(searchText))
        {
            lblStatus.Text = "Enter search text";
            return;
        }

        StringComparison comparison = chkMatchCase.Checked
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        int count = 0;
        int index = 0;
        string text = _editor.Text;

        while ((index = text.IndexOf(searchText, index, comparison)) >= 0)
        {
            text = text.Remove(index, searchText.Length).Insert(index, replaceText);
            index += replaceText.Length;
            count++;
        }

        if (count > 0)
        {
            _editor.Text = text;
            lblStatus.Text = $"Replaced {count} occurrence(s)";
        }
        else
        {
            lblStatus.Text = "Not found";
        }
    }

    private void btnFindNext_Click(object sender, EventArgs e) => FindNext();
    private void btnReplace_Click(object sender, EventArgs e) => Replace();
    private void btnReplaceAll_Click(object sender, EventArgs e) => ReplaceAll();

    private void txtFind_TextChanged(object sender, EventArgs e)
    {
        _lastFoundIndex = -1;
        lblStatus.Text = "";
    }

    private void FindReplaceForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
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
