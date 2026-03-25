namespace GratitudeJournal;

public class FileSaver
{
    string fileName;

    public FileSaver(string fileName)
    {
        this.fileName = fileName;

        string? directory = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(this.fileName))
        {
            File.Create(this.fileName).Close();
        }
    }

    public string[] ReadLines()
    {
        return File.ReadAllLines(fileName);
    }

    public void OverwriteLines(List<string> lines)
    {
        File.WriteAllLines(fileName, lines);
    }
}

