namespace NoteKraken;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _printDocument.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem("&File");
        newToolStripMenuItem = new ToolStripMenuItem("&New", null, newToolStripMenuItem_Click, Keys.Control | Keys.N);
        newWindowToolStripMenuItem = new ToolStripMenuItem("New &Window", null, newWindowToolStripMenuItem_Click, Keys.Control | Keys.Shift | Keys.N);
        openToolStripMenuItem = new ToolStripMenuItem("&Open...", null, openToolStripMenuItem_Click, Keys.Control | Keys.O);
        saveToolStripMenuItem = new ToolStripMenuItem("&Save", null, saveToolStripMenuItem_Click, Keys.Control | Keys.S);
        saveAsToolStripMenuItem = new ToolStripMenuItem("Save &As...", null, saveAsToolStripMenuItem_Click, Keys.Control | Keys.Shift | Keys.S);
        pageSetupToolStripMenuItem = new ToolStripMenuItem("Page Set&up...", null, pageSetupToolStripMenuItem_Click);
        printToolStripMenuItem = new ToolStripMenuItem("&Print...", null, printToolStripMenuItem_Click, Keys.Control | Keys.P);
        exitToolStripMenuItem = new ToolStripMenuItem("E&xit", null, exitToolStripMenuItem_Click, Keys.Alt | Keys.F4);

        editMenu = new ToolStripMenuItem("&Edit");
        undoToolStripMenuItem = new ToolStripMenuItem("&Undo", null, undoToolStripMenuItem_Click, Keys.Control | Keys.Z);
        redoToolStripMenuItem = new ToolStripMenuItem("&Redo", null, redoToolStripMenuItem_Click, Keys.Control | Keys.Y);
        cutToolStripMenuItem = new ToolStripMenuItem("Cu&t", null, cutToolStripMenuItem_Click, Keys.Control | Keys.X);
        copyToolStripMenuItem = new ToolStripMenuItem("&Copy", null, copyToolStripMenuItem_Click, Keys.Control | Keys.C);
        pasteToolStripMenuItem = new ToolStripMenuItem("&Paste", null, pasteToolStripMenuItem_Click, Keys.Control | Keys.V);
        deleteToolStripMenuItem = new ToolStripMenuItem("&Delete", null, deleteToolStripMenuItem_Click, Keys.Delete);
        findToolStripMenuItem = new ToolStripMenuItem("&Find...", null, findToolStripMenuItem_Click, Keys.Control | Keys.F);
        findNextToolStripMenuItem = new ToolStripMenuItem("Find &Next", null, findNextToolStripMenuItem_Click, Keys.F3);
        replaceToolStripMenuItem = new ToolStripMenuItem("&Replace...", null, replaceToolStripMenuItem_Click, Keys.Control | Keys.H);
        goToToolStripMenuItem = new ToolStripMenuItem("&Go To...", null, goToToolStripMenuItem_Click, Keys.Control | Keys.G);
        selectAllToolStripMenuItem = new ToolStripMenuItem("Select &All", null, selectAllToolStripMenuItem_Click, Keys.Control | Keys.A);
        timeDateToolStripMenuItem = new ToolStripMenuItem("Time/&Date", null, timeDateToolStripMenuItem_Click, Keys.F5);

        formatMenu = new ToolStripMenuItem("F&ormat");
        wordWrapToolStripMenuItem = new ToolStripMenuItem("&Word Wrap", null, wordWrapToolStripMenuItem_Click) { Checked = true, CheckOnClick = false };
        fontToolStripMenuItem = new ToolStripMenuItem("&Font...", null, fontToolStripMenuItem_Click);
        encodingMenu = new ToolStripMenuItem("&Encoding");
        utf8ToolStripMenuItem = new ToolStripMenuItem("UTF-&8", null, utf8ToolStripMenuItem_Click) { Checked = true };
        utf8BomToolStripMenuItem = new ToolStripMenuItem("UTF-8 with &BOM", null, utf8BomToolStripMenuItem_Click);
        utf16ToolStripMenuItem = new ToolStripMenuItem("UTF-1&6 LE", null, utf16ToolStripMenuItem_Click);
        ansiToolStripMenuItem = new ToolStripMenuItem("&ANSI (Windows-1252)", null, ansiToolStripMenuItem_Click);
        lineEndingsMenu = new ToolStripMenuItem("&Line Endings");
        windowsLineEndingsToolStripMenuItem = new ToolStripMenuItem("&Windows (CRLF)", null, windowsLineEndingsToolStripMenuItem_Click) { Checked = true };
        unixLineEndingsToolStripMenuItem = new ToolStripMenuItem("&Unix (LF)", null, unixLineEndingsToolStripMenuItem_Click);
        macLineEndingsToolStripMenuItem = new ToolStripMenuItem("Classic &Mac (CR)", null, macLineEndingsToolStripMenuItem_Click);

        viewMenu = new ToolStripMenuItem("&View");
        zoomMenu = new ToolStripMenuItem("&Zoom");
        zoomInToolStripMenuItem = new ToolStripMenuItem("Zoom &In", null, zoomInToolStripMenuItem_Click, Keys.Control | Keys.Oemplus);
        zoomOutToolStripMenuItem = new ToolStripMenuItem("Zoom &Out", null, zoomOutToolStripMenuItem_Click, Keys.Control | Keys.OemMinus);
        restoreZoomToolStripMenuItem = new ToolStripMenuItem("Restore Default Zoom", null, restoreZoomToolStripMenuItem_Click, Keys.Control | Keys.D0);
        statusBarToolStripMenuItem = new ToolStripMenuItem("&Status Bar", null, statusBarToolStripMenuItem_Click) { Checked = true };

        helpMenu = new ToolStripMenuItem("&Help");
        aboutToolStripMenuItem = new ToolStripMenuItem("&About Note Kraken", null, aboutToolStripMenuItem_Click);

        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel("Ready") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
        lblEncoding = new ToolStripStatusLabel("UTF-8") { BorderSides = ToolStripStatusLabelBorderSides.Left, Padding = new Padding(8, 0, 8, 0) };
        lblLineEnding = new ToolStripStatusLabel("Windows (CRLF)") { BorderSides = ToolStripStatusLabelBorderSides.Left, Padding = new Padding(8, 0, 8, 0) };
        lblZoom = new ToolStripStatusLabel("100%") { BorderSides = ToolStripStatusLabelBorderSides.Left, Padding = new Padding(8, 0, 8, 0) };
        lblPosition = new ToolStripStatusLabel("Ln 1, Col 1") { BorderSides = ToolStripStatusLabelBorderSides.Left, Padding = new Padding(8, 0, 4, 0) };

        editorContextMenu = new ContextMenuStrip(components);
        ctxUndo = new ToolStripMenuItem("&Undo", null, undoToolStripMenuItem_Click);
        ctxRedo = new ToolStripMenuItem("&Redo", null, redoToolStripMenuItem_Click);
        ctxCut = new ToolStripMenuItem("Cu&t", null, cutToolStripMenuItem_Click);
        ctxCopy = new ToolStripMenuItem("&Copy", null, copyToolStripMenuItem_Click);
        ctxPaste = new ToolStripMenuItem("&Paste", null, pasteToolStripMenuItem_Click);
        ctxDelete = new ToolStripMenuItem("&Delete", null, deleteToolStripMenuItem_Click);
        ctxSelectAll = new ToolStripMenuItem("Select &All", null, selectAllToolStripMenuItem_Click);

        txtEditor = new RichTextBox();

        SuspendLayout();
        menuStrip.SuspendLayout();
        statusStrip.SuspendLayout();

        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            newToolStripMenuItem,
            newWindowToolStripMenuItem,
            openToolStripMenuItem,
            saveToolStripMenuItem,
            saveAsToolStripMenuItem,
            new ToolStripSeparator(),
            pageSetupToolStripMenuItem,
            printToolStripMenuItem,
            new ToolStripSeparator(),
            exitToolStripMenuItem
        });

        editMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            undoToolStripMenuItem,
            redoToolStripMenuItem,
            new ToolStripSeparator(),
            cutToolStripMenuItem,
            copyToolStripMenuItem,
            pasteToolStripMenuItem,
            deleteToolStripMenuItem,
            new ToolStripSeparator(),
            findToolStripMenuItem,
            findNextToolStripMenuItem,
            replaceToolStripMenuItem,
            goToToolStripMenuItem,
            new ToolStripSeparator(),
            selectAllToolStripMenuItem,
            timeDateToolStripMenuItem
        });
        editMenu.DropDownOpening += editMenu_DropDownOpening;

        encodingMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            utf8ToolStripMenuItem,
            utf8BomToolStripMenuItem,
            utf16ToolStripMenuItem,
            ansiToolStripMenuItem
        });
        lineEndingsMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            windowsLineEndingsToolStripMenuItem,
            unixLineEndingsToolStripMenuItem,
            macLineEndingsToolStripMenuItem
        });
        formatMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            wordWrapToolStripMenuItem,
            fontToolStripMenuItem,
            new ToolStripSeparator(),
            encodingMenu,
            lineEndingsMenu
        });

        zoomMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            zoomInToolStripMenuItem,
            zoomOutToolStripMenuItem,
            restoreZoomToolStripMenuItem
        });
        viewMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            zoomMenu,
            statusBarToolStripMenuItem
        });
        helpMenu.DropDownItems.Add(aboutToolStripMenuItem);

        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, formatMenu, viewMenu, helpMenu });
        menuStrip.Dock = DockStyle.Top;
        menuStrip.Name = "menuStrip";
        menuStrip.TabIndex = 0;

        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblEncoding, lblLineEnding, lblZoom, lblPosition });
        statusStrip.Dock = DockStyle.Bottom;
        statusStrip.Name = "statusStrip";
        statusStrip.SizingGrip = true;

        editorContextMenu.Items.AddRange(new ToolStripItem[]
        {
            ctxUndo,
            ctxRedo,
            new ToolStripSeparator(),
            ctxCut,
            ctxCopy,
            ctxPaste,
            ctxDelete,
            new ToolStripSeparator(),
            ctxSelectAll
        });
        editorContextMenu.Opening += editorContextMenu_Opening;

        txtEditor.AcceptsTab = true;
        txtEditor.ContextMenuStrip = editorContextMenu;
        txtEditor.DetectUrls = false;
        txtEditor.Dock = DockStyle.Fill;
        txtEditor.HideSelection = false;
        txtEditor.Name = "txtEditor";
        txtEditor.ScrollBars = RichTextBoxScrollBars.Both;
        txtEditor.WordWrap = true;
        txtEditor.TextChanged += txtEditor_TextChanged;
        txtEditor.SelectionChanged += txtEditor_SelectionChanged;

        AllowDrop = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        Controls.Add(txtEditor);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(420, 280);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Untitled - Note Kraken";
        FormClosing += MainForm_FormClosing;
        DragEnter += MainForm_DragEnter;
        DragDrop += MainForm_DragDrop;

        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem fileMenu = null!;
    private ToolStripMenuItem newToolStripMenuItem = null!;
    private ToolStripMenuItem newWindowToolStripMenuItem = null!;
    private ToolStripMenuItem openToolStripMenuItem = null!;
    private ToolStripMenuItem saveToolStripMenuItem = null!;
    private ToolStripMenuItem saveAsToolStripMenuItem = null!;
    private ToolStripMenuItem pageSetupToolStripMenuItem = null!;
    private ToolStripMenuItem printToolStripMenuItem = null!;
    private ToolStripMenuItem exitToolStripMenuItem = null!;
    private ToolStripMenuItem editMenu = null!;
    private ToolStripMenuItem undoToolStripMenuItem = null!;
    private ToolStripMenuItem redoToolStripMenuItem = null!;
    private ToolStripMenuItem cutToolStripMenuItem = null!;
    private ToolStripMenuItem copyToolStripMenuItem = null!;
    private ToolStripMenuItem pasteToolStripMenuItem = null!;
    private ToolStripMenuItem deleteToolStripMenuItem = null!;
    private ToolStripMenuItem findToolStripMenuItem = null!;
    private ToolStripMenuItem findNextToolStripMenuItem = null!;
    private ToolStripMenuItem replaceToolStripMenuItem = null!;
    private ToolStripMenuItem goToToolStripMenuItem = null!;
    private ToolStripMenuItem selectAllToolStripMenuItem = null!;
    private ToolStripMenuItem timeDateToolStripMenuItem = null!;
    private ToolStripMenuItem formatMenu = null!;
    private ToolStripMenuItem wordWrapToolStripMenuItem = null!;
    private ToolStripMenuItem fontToolStripMenuItem = null!;
    private ToolStripMenuItem encodingMenu = null!;
    private ToolStripMenuItem utf8ToolStripMenuItem = null!;
    private ToolStripMenuItem utf8BomToolStripMenuItem = null!;
    private ToolStripMenuItem utf16ToolStripMenuItem = null!;
    private ToolStripMenuItem ansiToolStripMenuItem = null!;
    private ToolStripMenuItem lineEndingsMenu = null!;
    private ToolStripMenuItem windowsLineEndingsToolStripMenuItem = null!;
    private ToolStripMenuItem unixLineEndingsToolStripMenuItem = null!;
    private ToolStripMenuItem macLineEndingsToolStripMenuItem = null!;
    private ToolStripMenuItem viewMenu = null!;
    private ToolStripMenuItem zoomMenu = null!;
    private ToolStripMenuItem zoomInToolStripMenuItem = null!;
    private ToolStripMenuItem zoomOutToolStripMenuItem = null!;
    private ToolStripMenuItem restoreZoomToolStripMenuItem = null!;
    private ToolStripMenuItem statusBarToolStripMenuItem = null!;
    private ToolStripMenuItem helpMenu = null!;
    private ToolStripMenuItem aboutToolStripMenuItem = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblStatus = null!;
    private ToolStripStatusLabel lblEncoding = null!;
    private ToolStripStatusLabel lblLineEnding = null!;
    private ToolStripStatusLabel lblZoom = null!;
    private ToolStripStatusLabel lblPosition = null!;
    private RichTextBox txtEditor = null!;
    private ContextMenuStrip editorContextMenu = null!;
    private ToolStripMenuItem ctxUndo = null!;
    private ToolStripMenuItem ctxRedo = null!;
    private ToolStripMenuItem ctxCut = null!;
    private ToolStripMenuItem ctxCopy = null!;
    private ToolStripMenuItem ctxPaste = null!;
    private ToolStripMenuItem ctxDelete = null!;
    private ToolStripMenuItem ctxSelectAll = null!;
}
