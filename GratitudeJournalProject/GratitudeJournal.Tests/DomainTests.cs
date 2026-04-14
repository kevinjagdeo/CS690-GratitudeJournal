namespace GratitudeJournal.Tests;

public class DomainTests
{
    [Fact]
    public void GratitudeEntry_Constructor_AssignsProperties()
    {
        var entryDate = new DateOnly(2026, 4, 14);
        var gratitudes = new List<string> { "Tea", "Family" };
        var notes = "Good day";
        var createdAtUtc = new DateTime(2026, 4, 14, 12, 0, 0, DateTimeKind.Utc);

        var entry = new GratitudeEntry(entryDate, gratitudes, notes, createdAtUtc);

        Assert.Equal(entryDate, entry.EntryDate);
        Assert.Equal(gratitudes, entry.Gratitudes);
        Assert.Equal(notes, entry.Notes);
        Assert.Equal(createdAtUtc, entry.CreatedAtUtc);
    }

    [Fact]
    public void ReminderSettings_Constructor_AssignsProperties()
    {
        var settings = new ReminderSettings(true, new TimeOnly(21, 15));

        Assert.True(settings.Enabled);
        Assert.Equal(new TimeOnly(21, 15), settings.ReminderTime);
    }
}

