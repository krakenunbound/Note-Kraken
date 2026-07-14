namespace NoteKraken;

partial class FindReplaceForm
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblFind = new Label { AutoSize = true, Location = new Point(12, 15), Text = "Find:" };
        txtFind = new TextBox { Location = new Point(70, 12), Size = new Size(235, 23) };
        lblReplace = new Label { AutoSize = true, Location = new Point(12, 46), Text = "Replace:" };
        txtReplace = new TextBox { Location = new Point(70, 43), Size = new Size(235, 23) };
        btnFindNext = new Button { Location = new Point(320, 11), Size = new Size(92, 25), Text = "Find Next" };
        btnReplace = new Button { Location = new Point(320, 42), Size = new Size(92, 25), Text = "Replace" };
        btnReplaceAll = new Button { Location = new Point(320, 73), Size = new Size(92, 25), Text = "Replace All" };
        chkMatchCase = new CheckBox { AutoSize = true, Text = "Match case" };
        chkWholeWord = new CheckBox { AutoSize = true, Text = "Whole word" };
        lblDirection = new Label { AutoSize = true, Text = "Direction:" };
        radioDown = new RadioButton { AutoSize = true, Checked = true, Text = "Down" };
        radioUp = new RadioButton { AutoSize = true, Text = "Up" };
        chkWrapAround = new CheckBox { AutoSize = true, Checked = true, Text = "Wrap around" };
        lblStatus = new Label { AutoSize = true };

        txtFind.TextChanged += txtFind_TextChanged;
        btnFindNext.Click += btnFindNext_Click;
        btnReplace.Click += btnReplace_Click;
        btnReplaceAll.Click += btnReplaceAll_Click;

        SuspendLayout();
        Controls.AddRange(new Control[]
        {
            lblFind, txtFind, lblReplace, txtReplace, btnFindNext, btnReplace, btnReplaceAll,
            chkMatchCase, chkWholeWord, lblDirection, radioDown, radioUp, chkWrapAround, lblStatus
        });
        AcceptButton = btnFindNext;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(425, 165);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FindReplaceForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Find";
        KeyDown += FindReplaceForm_KeyDown;
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblFind = null!;
    private TextBox txtFind = null!;
    private Label lblReplace = null!;
    private TextBox txtReplace = null!;
    private Button btnFindNext = null!;
    private Button btnReplace = null!;
    private Button btnReplaceAll = null!;
    private CheckBox chkMatchCase = null!;
    private CheckBox chkWholeWord = null!;
    private Label lblDirection = null!;
    private RadioButton radioDown = null!;
    private RadioButton radioUp = null!;
    private CheckBox chkWrapAround = null!;
    private Label lblStatus = null!;
}
