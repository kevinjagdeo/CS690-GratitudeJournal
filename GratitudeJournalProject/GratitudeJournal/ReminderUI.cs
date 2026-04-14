namespace GratitudeJournal;

public class ReminderUI
{
    readonly DataManager dataManager;
    readonly EntryUI entryUI;
    bool reminderCheckedForThisSession;

    public ReminderUI(DataManager dataManager, EntryUI entryUI)
    {
        this.dataManager = dataManager;
        this.entryUI = entryUI;
        reminderCheckedForThisSession = false;
    }

    public void ShowSettings()
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();

        Console.WriteLine("[Reminder Settings]");
        Console.WriteLine("Current status: " + (dataManager.ReminderSettings.Enabled ? "Enabled" : "Disabled"));
        Console.WriteLine("Current time: " + dataManager.ReminderSettings.ReminderTime.ToString("HH:mm"));
        Console.WriteLine();
        Console.WriteLine("1. Configure reminder time");
        Console.WriteLine("2. Disable reminder");
        Console.WriteLine("0. Back");
        string choice = ConsoleHelpers.AskForInput("> ");

        if (choice == "1")
        {
            ConfigureReminder();
        }
        else if (choice == "2")
        {
            DisableReminder();
        }
    }

    public void CheckAndTriggerReminder()
    {
        if (reminderCheckedForThisSession)
        {
            return;
        }

        reminderCheckedForThisSession = true;

        if (!dataManager.ShouldTriggerReminder(DateTime.Now))
        {
            return;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        if (dataManager.HasEntryForDate(today))
        {
            return;
        }

        Console.Clear();
        ConsoleHelpers.PrintHeader();
        Console.WriteLine("[Reminder]");
        Console.WriteLine("Time to write today's gratitude entry.");
        string openNow = ConsoleHelpers.AskForInput("Open today's entry now? [Y/N] > ");
        if (openNow.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            entryUI.ShowCreateEntryForDate(today);
        }
    }

    void ConfigureReminder()
    {
        string timeInput = ConsoleHelpers.AskForInput("Enter reminder time (HH:mm): ");
        bool configured = dataManager.ConfigureReminder(timeInput, out string errorMessage);
        Console.WriteLine();
        if (configured)
        {
            Console.WriteLine("Reminder configured.");
            reminderCheckedForThisSession = false;
        }
        else
        {
            Console.WriteLine(errorMessage);
        }

        ConsoleHelpers.Pause();
    }

    void DisableReminder()
    {
        dataManager.DisableReminder();
        Console.WriteLine();
        Console.WriteLine("Reminder disabled.");
        ConsoleHelpers.Pause();
    }
}

