namespace KrakenPad;

partial class MainForm
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
        this.menuStrip = new MenuStrip();
        this.fileMenu = new ToolStripMenuItem();
        this.newToolStripMenuItem = new ToolStripMenuItem();
        this.openToolStripMenuItem = new ToolStripMenuItem();
        this.saveToolStripMenuItem = new ToolStripMenuItem();
        this.saveAsToolStripMenuItem = new ToolStripMenuItem();
        this.toolStripSeparator1 = new ToolStripSeparator();
        this.exitToolStripMenuItem = new ToolStripMenuItem();
        this.editMenu = new ToolStripMenuItem();
        this.undoToolStripMenuItem = new ToolStripMenuItem();
        this.toolStripSeparator2 = new ToolStripSeparator();
        this.cutToolStripMenuItem = new ToolStripMenuItem();
        this.copyToolStripMenuItem = new ToolStripMenuItem();
        this.pasteToolStripMenuItem = new ToolStripMenuItem();
        this.toolStripSeparator3 = new ToolStripSeparator();
        this.selectAllToolStripMenuItem = new ToolStripMenuItem();
        this.toolStripSeparator4 = new ToolStripSeparator();
        this.findToolStripMenuItem = new ToolStripMenuItem();
        this.replaceToolStripMenuItem = new ToolStripMenuItem();
        this.goToToolStripMenuItem = new ToolStripMenuItem();
        this.viewMenu = new ToolStripMenuItem();
        this.wordWrapToolStripMenuItem = new ToolStripMenuItem();
        this.helpMenu = new ToolStripMenuItem();
        this.aboutToolStripMenuItem = new ToolStripMenuItem();
        this.statusStrip = new StatusStrip();
        this.lblStatus = new ToolStripStatusLabel();
        this.lblPosition = new ToolStripStatusLabel();
        this.txtEditor = new RichTextBox();
        this.menuStrip.SuspendLayout();
        this.statusStrip.SuspendLayout();
        this.SuspendLayout();

        // menuStrip
        this.menuStrip.Items.AddRange(new ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.viewMenu,
            this.helpMenu});
        this.menuStrip.Location = new Point(0, 0);
        this.menuStrip.Name = "menuStrip";
        this.menuStrip.Size = new Size(800, 24);
        this.menuStrip.TabIndex = 0;

        // fileMenu
        this.fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
        this.fileMenu.Name = "fileMenu";
        this.fileMenu.Size = new Size(37, 20);
        this.fileMenu.Text = "&File";

        // newToolStripMenuItem
        this.newToolStripMenuItem.Name = "newToolStripMenuItem";
        this.newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        this.newToolStripMenuItem.Size = new Size(186, 22);
        this.newToolStripMenuItem.Text = "&New";
        this.newToolStripMenuItem.Click += new EventHandler(this.newToolStripMenuItem_Click);

        // openToolStripMenuItem
        this.openToolStripMenuItem.Name = "openToolStripMenuItem";
        this.openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        this.openToolStripMenuItem.Size = new Size(186, 22);
        this.openToolStripMenuItem.Text = "&Open...";
        this.openToolStripMenuItem.Click += new EventHandler(this.openToolStripMenuItem_Click);

        // saveToolStripMenuItem
        this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        this.saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        this.saveToolStripMenuItem.Size = new Size(186, 22);
        this.saveToolStripMenuItem.Text = "&Save";
        this.saveToolStripMenuItem.Click += new EventHandler(this.saveToolStripMenuItem_Click);

        // saveAsToolStripMenuItem
        this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        this.saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        this.saveAsToolStripMenuItem.Size = new Size(186, 22);
        this.saveAsToolStripMenuItem.Text = "Save &As...";
        this.saveAsToolStripMenuItem.Click += new EventHandler(this.saveAsToolStripMenuItem_Click);

        // toolStripSeparator1
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new Size(183, 6);

        // exitToolStripMenuItem
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        this.exitToolStripMenuItem.Size = new Size(186, 22);
        this.exitToolStripMenuItem.Text = "E&xit";
        this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);

        // editMenu
        this.editMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.toolStripSeparator2,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator3,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator4,
            this.findToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.goToToolStripMenuItem});
        this.editMenu.Name = "editMenu";
        this.editMenu.Size = new Size(39, 20);
        this.editMenu.Text = "&Edit";

        // undoToolStripMenuItem
        this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
        this.undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
        this.undoToolStripMenuItem.Size = new Size(164, 22);
        this.undoToolStripMenuItem.Text = "&Undo";
        this.undoToolStripMenuItem.Click += new EventHandler(this.undoToolStripMenuItem_Click);

        // toolStripSeparator2
        this.toolStripSeparator2.Name = "toolStripSeparator2";
        this.toolStripSeparator2.Size = new Size(161, 6);

        // cutToolStripMenuItem
        this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
        this.cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
        this.cutToolStripMenuItem.Size = new Size(164, 22);
        this.cutToolStripMenuItem.Text = "Cu&t";
        this.cutToolStripMenuItem.Click += new EventHandler(this.cutToolStripMenuItem_Click);

        // copyToolStripMenuItem
        this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        this.copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
        this.copyToolStripMenuItem.Size = new Size(164, 22);
        this.copyToolStripMenuItem.Text = "&Copy";
        this.copyToolStripMenuItem.Click += new EventHandler(this.copyToolStripMenuItem_Click);

        // pasteToolStripMenuItem
        this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        this.pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
        this.pasteToolStripMenuItem.Size = new Size(164, 22);
        this.pasteToolStripMenuItem.Text = "&Paste";
        this.pasteToolStripMenuItem.Click += new EventHandler(this.pasteToolStripMenuItem_Click);

        // toolStripSeparator3
        this.toolStripSeparator3.Name = "toolStripSeparator3";
        this.toolStripSeparator3.Size = new Size(161, 6);

        // selectAllToolStripMenuItem
        this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
        this.selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
        this.selectAllToolStripMenuItem.Size = new Size(164, 22);
        this.selectAllToolStripMenuItem.Text = "Select &All";
        this.selectAllToolStripMenuItem.Click += new EventHandler(this.selectAllToolStripMenuItem_Click);

        // toolStripSeparator4
        this.toolStripSeparator4.Name = "toolStripSeparator4";
        this.toolStripSeparator4.Size = new Size(161, 6);

        // findToolStripMenuItem
        this.findToolStripMenuItem.Name = "findToolStripMenuItem";
        this.findToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
        this.findToolStripMenuItem.Size = new Size(164, 22);
        this.findToolStripMenuItem.Text = "&Find...";
        this.findToolStripMenuItem.Click += new EventHandler(this.findToolStripMenuItem_Click);

        // replaceToolStripMenuItem
        this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
        this.replaceToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
        this.replaceToolStripMenuItem.Size = new Size(164, 22);
        this.replaceToolStripMenuItem.Text = "&Replace...";
        this.replaceToolStripMenuItem.Click += new EventHandler(this.replaceToolStripMenuItem_Click);

        // goToToolStripMenuItem
        this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
        this.goToToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
        this.goToToolStripMenuItem.Size = new Size(164, 22);
        this.goToToolStripMenuItem.Text = "&Go To...";
        this.goToToolStripMenuItem.Click += new EventHandler(this.goToToolStripMenuItem_Click);

        // viewMenu
        this.viewMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.wordWrapToolStripMenuItem});
        this.viewMenu.Name = "viewMenu";
        this.viewMenu.Size = new Size(44, 20);
        this.viewMenu.Text = "&View";

        // wordWrapToolStripMenuItem
        this.wordWrapToolStripMenuItem.Checked = true;
        this.wordWrapToolStripMenuItem.CheckState = CheckState.Checked;
        this.wordWrapToolStripMenuItem.Name = "wordWrapToolStripMenuItem";
        this.wordWrapToolStripMenuItem.Size = new Size(134, 22);
        this.wordWrapToolStripMenuItem.Text = "&Word Wrap";
        this.wordWrapToolStripMenuItem.Click += new EventHandler(this.wordWrapToolStripMenuItem_Click);

        // helpMenu
        this.helpMenu.DropDownItems.AddRange(new ToolStripItem[] {
            this.aboutToolStripMenuItem});
        this.helpMenu.Name = "helpMenu";
        this.helpMenu.Size = new Size(44, 20);
        this.helpMenu.Text = "&Help";

        // aboutToolStripMenuItem
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.Size = new Size(107, 22);
        this.aboutToolStripMenuItem.Text = "&About";
        this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);

        // statusStrip
        this.statusStrip.Items.AddRange(new ToolStripItem[] {
            this.lblStatus,
            this.lblPosition});
        this.statusStrip.Location = new Point(0, 428);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new Size(800, 22);
        this.statusStrip.TabIndex = 1;

        // lblStatus
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new Size(694, 17);
        this.lblStatus.Spring = true;
        this.lblStatus.Text = "Ready";
        this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;

        // lblPosition
        this.lblPosition.Name = "lblPosition";
        this.lblPosition.Size = new Size(91, 17);
        this.lblPosition.Text = "Ln 1, Col 1";

        // txtEditor
        this.txtEditor.AcceptsTab = true;
        this.txtEditor.Dock = DockStyle.Fill;
        this.txtEditor.Location = new Point(0, 24);
        this.txtEditor.Name = "txtEditor";
        this.txtEditor.Size = new Size(800, 404);
        this.txtEditor.TabIndex = 2;
        this.txtEditor.Text = "";
        this.txtEditor.WordWrap = true;
        this.txtEditor.DetectUrls = false;
        this.txtEditor.TextChanged += new EventHandler(this.txtEditor_TextChanged);
        this.txtEditor.SelectionChanged += new EventHandler(this.txtEditor_SelectionChanged);

        // MainForm
        this.AllowDrop = true;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(800, 450);
        this.Controls.Add(this.txtEditor);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.menuStrip);
        this.MainMenuStrip = this.menuStrip;
        this.Name = "MainForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Kraken Pad";
        this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
        this.DragDrop += new DragEventHandler(this.MainForm_DragDrop);
        this.DragEnter += new DragEventHandler(this.MainForm_DragEnter);
        this.menuStrip.ResumeLayout(false);
        this.menuStrip.PerformLayout();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem newToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editMenu;
    private ToolStripMenuItem undoToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem selectAllToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripMenuItem findToolStripMenuItem;
    private ToolStripMenuItem replaceToolStripMenuItem;
    private ToolStripMenuItem goToToolStripMenuItem;
    private ToolStripMenuItem viewMenu;
    private ToolStripMenuItem wordWrapToolStripMenuItem;
    private ToolStripMenuItem helpMenu;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripStatusLabel lblPosition;
    private RichTextBox txtEditor;
}
