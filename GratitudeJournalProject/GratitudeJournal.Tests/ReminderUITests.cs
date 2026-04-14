namespace GratitudeJournal.Tests;

public class ReminderUITests
{
    [Fact]
    public void ShowSettings_ConfigureReminder_UpdatesSettings()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        var entryUI = new EntryUI(manager);
        var reminderUI = new ReminderUI(manager, entryUI);

        using var io = new ConsoleIoScope("1\n09:45\n\n");
        reminderUI.ShowSettings();

        Assert.True(manager.ReminderSettings.Enabled);
        Assert.Equal(new TimeOnly(9, 45), manager.ReminderSettings.ReminderTime);
    }

    [Fact]
    public void ShowSettings_DisableReminder_DisablesSettings()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.ConfigureReminder("08:00", out _);
        var entryUI = new EntryUI(manager);
        var reminderUI = new ReminderUI(manager, entryUI);

        using var io = new ConsoleIoScope("2\n\n");
        reminderUI.ShowSettings();

        Assert.False(manager.ReminderSettings.Enabled);
    }

    [Fact]
    public void CheckAndTriggerReminder_OpenNowYes_CreatesTodaysEntry()
    {
        using var workingDir = new TestWorkingDirectoryScope();
        var manager = new DataManager();
        manager.ConfigureReminder("00:00", out _);
        var entryUI = new EntryUI(manager);
        var reminderUI = new ReminderUI(manager, entryUI);

        using var io = new ConsoleIoScope("Y\nReminder gratitude\n\n\n\nY\n\n");
        reminderUI.CheckAndTriggerReminder();

        var today = DateOnly.FromDateTime(DateTime.Today);
        Assert.True(manager.HasEntryForDate(today));
    }
}

