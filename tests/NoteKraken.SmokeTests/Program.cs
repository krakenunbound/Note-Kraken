using System.Reflection;
using System.Text;
using NoteKraken;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

MethodInfo readDocument = typeof(MainForm).GetMethod("ReadDocument", BindingFlags.NonPublic | BindingFlags.Static)
    ?? throw new InvalidOperationException("ReadDocument was not found.");
MethodInfo normalizeLineEndings = typeof(MainForm).GetMethod("NormalizeLineEndings", BindingFlags.NonPublic | BindingFlags.Static)
    ?? throw new InvalidOperationException("NormalizeLineEndings was not found.");

string temp = Path.Combine(Path.GetTempPath(), $"note-kraken-tests-{Guid.NewGuid():N}");
Directory.CreateDirectory(temp);

try
{
    CheckDocument("utf8-bom.txt", "alpha\r\nbeta", new UTF8Encoding(true), "UTF-8 BOM", "Windows (CRLF)");
    CheckDocument("utf8.txt", "alpha\nbeta", new UTF8Encoding(false), "UTF-8", "Unix (LF)");
    CheckDocument("utf16.txt", "alpha\nbeta", new UnicodeEncoding(false, true), "UTF-16 LE", "Unix (LF)");
    CheckDocument("ansi.txt", "café\r\nmenu", Encoding.GetEncoding(1252), "ANSI (1252)", "Windows (CRLF)");

    string normalized = (string)(normalizeLineEndings.Invoke(null, new object[] { "a\r\nb\nc\rd", "\n" })
        ?? throw new InvalidOperationException("NormalizeLineEndings returned null."));
    AssertEqual("a\nb\nc\nd", normalized, "mixed line endings normalize to LF");

    Console.WriteLine("All Note Kraken codec smoke tests passed.");
}
finally
{
    Directory.Delete(temp, recursive: true);
}

void CheckDocument(string fileName, string text, Encoding encoding, string expectedEncoding, string expectedEnding)
{
    string path = Path.Combine(temp, fileName);
    File.WriteAllText(path, text, encoding);
    object result = readDocument.Invoke(null, new object[] { path })
        ?? throw new InvalidOperationException("ReadDocument returned null.");
    Type resultType = result.GetType();
    string actualText = (string)(resultType.GetProperty("Text")?.GetValue(result)
        ?? throw new InvalidOperationException("Document text was unavailable."));
    string actualEncoding = (string)(resultType.GetProperty("EncodingLabel")?.GetValue(result)
        ?? throw new InvalidOperationException("Encoding label was unavailable."));
    string actualEnding = (string)(resultType.GetProperty("LineEndingLabel")?.GetValue(result)
        ?? throw new InvalidOperationException("Line-ending label was unavailable."));

    AssertEqual(text, actualText, $"{fileName} text round-trip");
    AssertEqual(expectedEncoding, actualEncoding, $"{fileName} encoding detection");
    AssertEqual(expectedEnding, actualEnding, $"{fileName} line-ending detection");
}

static void AssertEqual(string expected, string actual, string name)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
        throw new InvalidOperationException($"Failed: {name}. Expected '{Escape(expected)}', got '{Escape(actual)}'.");
    Console.WriteLine($"Passed: {name}");
}

static string Escape(string value) => value.Replace("\r", "\\r").Replace("\n", "\\n");
