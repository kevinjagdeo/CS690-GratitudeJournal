namespace GratitudeJournal.Tests;

public class FileSaverTests
{
    [Fact]
    public void Constructor_CreatesFileAndDirectory()
    {
        using var scope = new TestWorkingDirectoryScope();
        var path = Path.Combine("data", "sample.txt");

        _ = new FileSaver(path);

        Assert.True(Directory.Exists("data"));
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void OverwriteLines_ThenReadLines_ReturnsWrittenLines()
    {
        using var scope = new TestWorkingDirectoryScope();
        var path = Path.Combine("data", "sample.txt");
        var fileSaver = new FileSaver(path);
        var linesToWrite = new List<string> { "line1", "line2", "line3" };

        fileSaver.OverwriteLines(linesToWrite);
        var readLines = fileSaver.ReadLines();

        Assert.Equal(linesToWrite, readLines);
    }
}

