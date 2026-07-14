using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using NoteKraken;
using NoteKrakenInstaller;

namespace NoteKraken.ScreenshotTool;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".."));
        string outputDirectory = Path.Combine(projectRoot, "docs", "screenshots");
        Directory.CreateDirectory(outputDirectory);

        string sampleDirectory = Path.Combine(Path.GetTempPath(), $"note-kraken-demo-{Guid.NewGuid():N}");
        Directory.CreateDirectory(sampleDirectory);
        string samplePath = Path.Combine(sampleDirectory, "Note Kraken Demo.txt");
        File.WriteAllText(samplePath,
            "NOTE KRAKEN 1.1\r\n" +
            "A fast, focused Windows text editor.\r\n\r\n" +
            "CLASSIC EDITING\r\n" +
            "  • Cut, copy, paste, undo, and redo\r\n" +
            "  • Find and replace with whole-word and direction controls\r\n" +
            "  • Go to line, time/date, fonts, zoom, page setup, and printing\r\n\r\n" +
            "RELIABLE TEXT FILES\r\n" +
            "  • UTF-8, UTF-8 BOM, UTF-16, UTF-32, and Windows-1252 detection\r\n" +
            "  • Windows, Unix, and classic Mac line-ending preservation\r\n" +
            "  • Unsaved-work protection when opening, dropping, or closing files\r\n\r\n" +
            "No cloud. No accounts. No telemetry. No AI. Just text.\r\n");

        try
        {
            string editorOutput = Path.Combine(outputDirectory, "note-kraken-editor.png");
            using var editor = new MainForm
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(120, 90),
                Size = new Size(1120, 700)
            };
            editor.OpenFile(samplePath);
            RichTextBox textEditor = GetField<RichTextBox>(editor, "txtEditor");
            textEditor.SelectionStart = textEditor.Text.IndexOf("Find and replace", StringComparison.Ordinal);
            textEditor.SelectionLength = "Find and replace".Length;
            editor.Show();
            PumpUi();
            if (editor.Text.Contains(" * - Note Kraken", StringComparison.Ordinal))
                throw new InvalidOperationException("A file opened before first show was incorrectly marked modified.");

            using (Bitmap editorImage = CaptureWindow(editor))
                editorImage.Save(editorOutput, ImageFormat.Png);

            Invoke(editor, "ShowFindReplace", true);
            PumpUi();
            Form findDialog = editor.OwnedForms.Single(form => form.Text == "Replace");
            findDialog.Location = new Point(
                editor.Left + editor.Width - findDialog.Width - 60,
                editor.Top + 90);
            GetField<TextBox>(findDialog, "txtFind").Text = "classic";
            GetField<TextBox>(findDialog, "txtReplace").Text = "focused";
            PumpUi();

            editor.BringToFront();
            findDialog.BringToFront();
            PumpUi();
            using (Bitmap combined = CaptureScreenRegion(editor))
                combined.Save(Path.Combine(outputDirectory, "note-kraken-find-replace.png"), ImageFormat.Png);

            findDialog.Hide();
            editor.Hide();

            using var installer = new InstallerForm
            {
                StartPosition = FormStartPosition.Manual,
                Location = new Point(240, 140)
            };
            GetField<TextBox>(installer, "_pathBox").Text = @"%LOCALAPPDATA%\NoteKraken";
            installer.Show();
            PumpUi();
            using (Bitmap installerImage = CaptureWindow(installer))
                installerImage.Save(Path.Combine(outputDirectory, "note-kraken-installer.png"), ImageFormat.Png);
            installer.Hide();
        }
        finally
        {
            Directory.Delete(sampleDirectory, recursive: true);
        }

        foreach (string file in Directory.GetFiles(outputDirectory, "*.png"))
            Console.WriteLine(file);
    }

    private static void PumpUi()
    {
        for (int i = 0; i < 8; i++)
        {
            Application.DoEvents();
            Thread.Sleep(40);
        }
    }

    private static T GetField<T>(object instance, string name) where T : class =>
        (T)(instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(instance)
            ?? throw new InvalidOperationException($"Field not found: {name}"));

    private static void Invoke(object instance, string name, params object[] arguments)
    {
        MethodInfo method = instance.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Method not found: {name}");
        _ = method.Invoke(instance, arguments);
    }

    private static Bitmap CaptureWindow(Form form)
    {
        Rectangle rectangle = GetWindowRectangle(form.Handle);
        var bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppRgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        IntPtr deviceContext = graphics.GetHdc();
        try
        {
            if (!PrintWindow(form.Handle, deviceContext, 2))
                throw new InvalidOperationException($"PrintWindow failed for {form.Text}.");
        }
        finally
        {
            graphics.ReleaseHdc(deviceContext);
        }
        return bitmap;
    }

    private static Bitmap CaptureScreenRegion(Form form)
    {
        Rectangle rectangle = GetWindowRectangle(form.Handle);
        var bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppRgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(rectangle.Location, Point.Empty, rectangle.Size, CopyPixelOperation.SourceCopy);
        return bitmap;
    }

    private static Rectangle GetWindowRectangle(IntPtr handle)
    {
        if (!GetWindowRect(handle, out NativeRectangle rectangle))
            throw new InvalidOperationException("GetWindowRect failed.");
        return Rectangle.FromLTRB(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PrintWindow(IntPtr window, IntPtr deviceContext, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr window, out NativeRectangle rectangle);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
