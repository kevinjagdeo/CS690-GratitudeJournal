namespace GratitudeJournal.Tests;

public class EntryUITests
{
    [Fact]
    public void ShowCreateEntryForDate_SaveYes_AddsEntry()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        var entryUI = new EntryUI(manager);

        using var io = new ConsoleIoScope("My first gratitude\n\n\n\nY\n\n");
        entryUI.ShowCreateEntryForDate(DateOnly.FromDateTime(DateTime.Today));

        Assert.Single(manager.Entries);
        Assert.Equal("My first gratitude", manager.Entries[0].Gratitudes[0]);
    }

    [Fact]
    public void ShowEditDeleteMenu_DeleteFlow_RemovesEntry()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "To delete" }, "", out _);
        var entryUI = new EntryUI(manager);

        using var io = new ConsoleIoScope("1\n2\nY\n\n");
        entryUI.ShowEditDeleteMenu();

        Assert.Empty(manager.Entries);
    }
}

