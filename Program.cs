namespace NoteKraken;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var mainForm = new MainForm();

        // Handle command line argument (file to open)
        if (args.Length > 0 && File.Exists(args[0]))
        {
            mainForm.OpenFile(args[0]);
        }

        Application.Run(mainForm);
    }
}
