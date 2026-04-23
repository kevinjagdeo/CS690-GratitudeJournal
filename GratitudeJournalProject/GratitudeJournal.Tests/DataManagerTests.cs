namespace GratitudeJournal.Tests;

public class DataManagerTests
{
    [Fact]
    public void AddEntry_ValidData_SavesEntry()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();

        var success = manager.AddEntry(
            DateOnly.FromDateTime(DateTime.Today),
            new List<string> { "Family dinner", "Sunny weather" },
            "Felt calm today.",
            out var errors);

        Assert.True(success);
        Assert.Empty(errors);
        Assert.Single(manager.Entries);
        Assert.True(File.Exists(Path.Combine("data", "entries.txt")));
    }

    [Fact]
    public void AddEntry_FutureDate_ReturnsValidationError()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();

        var success = manager.AddEntry(
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new List<string> { "Test gratitude" },
            "",
            out var errors);

        Assert.False(success);
        Assert.Contains("Date cannot be in the future.", errors);
        Assert.Empty(manager.Entries);
    }

    [Fact]
    public void UpdateEntry_ExistingEntry_ChangesValues()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Old text" }, "", out _);
        var existingEntry = manager.Entries[0];

        var success = manager.UpdateEntry(
            existingEntry,
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            new List<string> { "Updated gratitude" },
            "Updated notes",
            out var errors);

        Assert.True(success);
        Assert.Empty(errors);
        Assert.Single(manager.Entries);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), manager.Entries[0].EntryDate);
        Assert.Equal("Updated gratitude", manager.Entries[0].Gratitudes[0]);
        Assert.Equal("Updated notes", manager.Entries[0].Notes);
    }

    [Fact]
    public void DeleteEntry_ExistingEntry_RemovesEntry()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "To delete" }, "", out _);
        var entry = manager.Entries[0];

        var deleted = manager.DeleteEntry(entry);

        Assert.True(deleted);
        Assert.Empty(manager.Entries);
    }

    [Fact]
    public void GetEntriesSorted_NewestFirst_ReturnsCorrectOrder()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), new List<string> { "Older" }, "", out _);
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Newest" }, "", out _);

        var sorted = manager.GetEntriesSorted(true);

        Assert.Equal("Newest", sorted[0].Gratitudes[0]);
        Assert.Equal("Older", sorted[1].Gratitudes[0]);
    }

    [Fact]
    public void ReminderSettings_ConfigureAndPersist_Works()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();

        var configured = manager.ConfigureReminder("09:30", out var errorMessage);
        var managerReloaded = new DataManager();

        Assert.True(configured);
        Assert.Equal("", errorMessage);
        Assert.True(managerReloaded.ReminderSettings.Enabled);
        Assert.Equal(new TimeOnly(9, 30), managerReloaded.ReminderSettings.ReminderTime);
    }

    [Fact]
    public void ConfigureReminder_InvalidTime_ReturnsError()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();

        var configured = manager.ConfigureReminder("not-a-time", out var errorMessage);

        Assert.False(configured);
        Assert.Contains("Invalid time format", errorMessage);
    }

    [Fact]
    public void ShouldTriggerReminder_WhenEnabledAndTimePassed_ReturnsTrue()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.ConfigureReminder("08:00", out _);

        var shouldTrigger = manager.ShouldTriggerReminder(DateTime.Today.AddHours(9));

        Assert.True(shouldTrigger);
    }

    [Fact]
    public void HasEntryForDate_WhenEntryExists_ReturnsTrue()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        var today = DateOnly.FromDateTime(DateTime.Today);
        manager.AddEntry(today, new List<string> { "Today item" }, "", out _);

        var hasTodayEntry = manager.HasEntryForDate(today);

        Assert.True(hasTodayEntry);
    }

    [Fact]
    public void SearchEntries_WithKeyword_ReturnsMatchingEntries()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Morning tea" }, "Relaxed", out _);
        manager.AddEntry(DateOnly.FromDateTime(DateTime.Today), new List<string> { "Workout done" }, "Felt strong", out _);

        var results = manager.SearchEntries("tea", null, null, newestFirst: true);

        Assert.Single(results);
        Assert.Contains("Morning tea", results[0].Gratitudes[0]);
    }

    [Fact]
    public void SearchEntries_WithDateRange_ReturnsEntriesWithinRange()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(new DateOnly(2026, 4, 1), new List<string> { "Old one" }, "", out _);
        manager.AddEntry(new DateOnly(2026, 4, 10), new List<string> { "Mid one" }, "", out _);
        manager.AddEntry(new DateOnly(2026, 4, 20), new List<string> { "New one" }, "", out _);

        var results = manager.SearchEntries("", new DateOnly(2026, 4, 5), new DateOnly(2026, 4, 15), newestFirst: true);

        Assert.Single(results);
        Assert.Equal("Mid one", results[0].Gratitudes[0]);
    }

    [Fact]
    public void SearchEntries_WithCombinedFilters_ReturnsIntersection()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.AddEntry(new DateOnly(2026, 4, 10), new List<string> { "Coffee with friend" }, "", out _);
        manager.AddEntry(new DateOnly(2026, 4, 12), new List<string> { "Tea with friend" }, "", out _);
        manager.AddEntry(new DateOnly(2026, 4, 15), new List<string> { "Coffee alone" }, "", out _);

        var results = manager.SearchEntries("coffee", new DateOnly(2026, 4, 11), new DateOnly(2026, 4, 16), newestFirst: true);

        Assert.Single(results);
        Assert.Equal("Coffee alone", results[0].Gratitudes[0]);
    }

    [Fact]
    public void IsValidDateRange_StartAfterEnd_ReturnsFalse()
    {
        using var scope = new TestWorkingDirectoryScope();
        var manager = new DataManager();

        var isValid = manager.IsValidDateRange(new DateOnly(2026, 4, 20), new DateOnly(2026, 4, 10), out string errorMessage);

        Assert.False(isValid);
        Assert.Contains("Start date cannot be later than end date.", errorMessage);
    }
}
