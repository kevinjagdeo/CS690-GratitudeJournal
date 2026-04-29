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

    [Fact]
    public void ShowSearchFilterMenu_WithKeyword_DisplaysMatchingResult()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Morning tea" }, "", out _);
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Evening walk" }, "", out _);
        var entryUI = new EntryUI(manager);

        using var io = new ConsoleIoScope("tea\n\n\n1\n0\n");
        entryUI.ShowSearchFilterMenu();

        Assert.Contains("[Search Results]", io.Output);
        Assert.Contains("Morning tea", io.Output);
        Assert.DoesNotContain("Evening walk", io.Output);
    }

    [Fact]
    public void ShowHistory_SortChoiceChangesDisplayedOrder()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(new DateOnly(2026, 4, 1), new List<string> { "Old entry" }, "", out _);
        manager.AddEntry(new DateOnly(2026, 4, 20), new List<string> { "New entry" }, "", out _);
        var entryUI = new EntryUI(manager);

        using var newestFirstIo = new ConsoleIoScope("1\n0\n");
        entryUI.ShowHistory();
        string newestFirstOutput = newestFirstIo.Output;

        using var oldestFirstIo = new ConsoleIoScope("2\n0\n");
        entryUI.ShowHistory();
        string oldestFirstOutput = oldestFirstIo.Output;

        int newestIndexInNewestMode = newestFirstOutput.IndexOf("New entry", StringComparison.Ordinal);
        int oldestIndexInNewestMode = newestFirstOutput.IndexOf("Old entry", StringComparison.Ordinal);
        int newestIndexInOldestMode = oldestFirstOutput.IndexOf("New entry", StringComparison.Ordinal);
        int oldestIndexInOldestMode = oldestFirstOutput.IndexOf("Old entry", StringComparison.Ordinal);

        Assert.True(newestIndexInNewestMode >= 0 && oldestIndexInNewestMode >= 0);
        Assert.True(newestIndexInOldestMode >= 0 && oldestIndexInOldestMode >= 0);
        Assert.True(newestIndexInNewestMode < oldestIndexInNewestMode);
        Assert.True(oldestIndexInOldestMode < newestIndexInOldestMode);
    }
}
