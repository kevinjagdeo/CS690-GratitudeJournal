using System.Threading;

namespace GratitudeJournal.Tests;

public sealed class TestWorkingDirectoryScope : IDisposable
{
    static readonly object DirectoryLock = new();

    readonly string originalDirectory;
    readonly string tempDirectory;
    bool disposed;

    public TestWorkingDirectoryScope()
    {
        Monitor.Enter(DirectoryLock);

        originalDirectory = Directory.GetCurrentDirectory();
        tempDirectory = Path.Combine(Path.GetTempPath(), "GratitudeJournalTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);
        Directory.SetCurrentDirectory(tempDirectory);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        Directory.SetCurrentDirectory(originalDirectory);
        try
        {
            Directory.Delete(tempDirectory, true);
        }
        catch
        {
            // Best-effort cleanup only.
        }
        Monitor.Exit(DirectoryLock);
    }
}

