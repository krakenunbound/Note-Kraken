namespace NoteKraken;

partial class FindReplaceForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.lblFind = new Label();
        this.txtFind = new TextBox();
        this.lblReplace = new Label();
        this.txtReplace = new TextBox();
        this.btnFindNext = new Button();
        this.btnReplace = new Button();
        this.btnReplaceAll = new Button();
        this.chkMatchCase = new CheckBox();
        this.lblStatus = new Label();
        this.SuspendLayout();

        // lblFind
        this.lblFind.AutoSize = true;
        this.lblFind.Location = new Point(12, 15);
        this.lblFind.Name = "lblFind";
        this.lblFind.Size = new Size(33, 15);
        this.lblFind.TabIndex = 0;
        this.lblFind.Text = "Find:";

        // txtFind
        this.txtFind.Location = new Point(70, 12);
        this.txtFind.Name = "txtFind";
        this.txtFind.Size = new Size(200, 23);
        this.txtFind.TabIndex = 1;
        this.txtFind.TextChanged += new EventHandler(this.txtFind_TextChanged);

        // lblReplace
        this.lblReplace.AutoSize = true;
        this.lblReplace.Location = new Point(12, 44);
        this.lblReplace.Name = "lblReplace";
        this.lblReplace.Size = new Size(51, 15);
        this.lblReplace.TabIndex = 2;
        this.lblReplace.Text = "Replace:";

        // txtReplace
        this.txtReplace.Location = new Point(70, 41);
        this.txtReplace.Name = "txtReplace";
        this.txtReplace.Size = new Size(200, 23);
        this.txtReplace.TabIndex = 3;

        // btnFindNext
        this.btnFindNext.Location = new Point(280, 11);
        this.btnFindNext.Name = "btnFindNext";
        this.btnFindNext.Size = new Size(85, 25);
        this.btnFindNext.TabIndex = 4;
        this.btnFindNext.Text = "Find Next";
        this.btnFindNext.UseVisualStyleBackColor = false;
        this.btnFindNext.Click += new EventHandler(this.btnFindNext_Click);

        // btnReplace
        this.btnReplace.Location = new Point(280, 40);
        this.btnReplace.Name = "btnReplace";
        this.btnReplace.Size = new Size(85, 25);
        this.btnReplace.TabIndex = 5;
        this.btnReplace.Text = "Replace";
        this.btnReplace.UseVisualStyleBackColor = false;
        this.btnReplace.Click += new EventHandler(this.btnReplace_Click);

        // btnReplaceAll
        this.btnReplaceAll.Location = new Point(280, 69);
        this.btnReplaceAll.Name = "btnReplaceAll";
        this.btnReplaceAll.Size = new Size(85, 25);
        this.btnReplaceAll.TabIndex = 6;
        this.btnReplaceAll.Text = "Replace All";
        this.btnReplaceAll.UseVisualStyleBackColor = false;
        this.btnReplaceAll.Click += new EventHandler(this.btnReplaceAll_Click);

        // chkMatchCase
        this.chkMatchCase.AutoSize = true;
        this.chkMatchCase.Location = new Point(70, 70);
        this.chkMatchCase.Name = "chkMatchCase";
        this.chkMatchCase.Size = new Size(86, 19);
        this.chkMatchCase.TabIndex = 7;
        this.chkMatchCase.Text = "Match case";
        this.chkMatchCase.UseVisualStyleBackColor = true;

        // lblStatus
        this.lblStatus.AutoSize = true;
        this.lblStatus.ForeColor = Color.FromArgb(255, 128, 128);
        this.lblStatus.Location = new Point(70, 95);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new Size(0, 15);
        this.lblStatus.TabIndex = 8;

        // FindReplaceForm
        this.AcceptButton = this.btnFindNext;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(377, 120);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.chkMatchCase);
        this.Controls.Add(this.btnReplaceAll);
        this.Controls.Add(this.btnReplace);
        this.Controls.Add(this.btnFindNext);
        this.Controls.Add(this.txtReplace);
        this.Controls.Add(this.lblReplace);
        this.Controls.Add(this.txtFind);
        this.Controls.Add(this.lblFind);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.KeyPreview = true;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FindReplaceForm";
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Find";
        this.KeyDown += new KeyEventHandler(this.FindReplaceForm_KeyDown);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Label lblFind;
    private TextBox txtFind;
    private Label lblReplace;
    private TextBox txtReplace;
    private Button btnFindNext;
    private Button btnReplace;
    private Button btnReplaceAll;
    private CheckBox chkMatchCase;
    private Label lblStatus;
}
