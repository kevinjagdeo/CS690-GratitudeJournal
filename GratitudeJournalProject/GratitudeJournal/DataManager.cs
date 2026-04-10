namespace GratitudeJournal;

public class DataManager
{
    FileSaver fileSaver;
    FileSaver settingsFileSaver;

    public List<GratitudeEntry> Entries { get; }
    public ReminderSettings ReminderSettings { get; private set; }

    public DataManager()
    {
        string dataFilePath = Path.Combine("data", "entries.txt");
        string settingsFilePath = Path.Combine("data", "settings.txt");
        fileSaver = new FileSaver(dataFilePath);
        settingsFileSaver = new FileSaver(settingsFilePath);
        Entries = new List<GratitudeEntry>();
        ReminderSettings = new ReminderSettings(false, new TimeOnly(20, 0));
        LoadEntries();
        LoadReminderSettings();
    }

    void LoadEntries()
    {
        string[] lines = fileSaver.ReadLines();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split("||");
            if (parts.Length < 4)
            {
                continue;
            }

            DateOnly entryDate;
            DateOnly.TryParse(parts[0], out entryDate);

            List<string> gratitudes = parts[1]
                .Split(";;", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            string notes = parts[2] == "-" ? "" : parts[2];

            DateTime createdAtUtc;
            DateTime.TryParse(parts[3], out createdAtUtc);
            if (createdAtUtc == default)
            {
                createdAtUtc = DateTime.UtcNow;
            }

            Entries.Add(new GratitudeEntry(entryDate, gratitudes, notes, createdAtUtc));
        }
    }

    public bool AddEntry(DateOnly entryDate, List<string> gratitudeItems, string notes, out List<string> errors)
    {
        errors = ValidateEntry(entryDate, gratitudeItems, notes);
        if (errors.Count > 0)
        {
            return false;
        }

        List<string> normalizedItems = gratitudeItems
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        string normalizedNotes = notes?.Trim() ?? "";
        var entry = new GratitudeEntry(entryDate, normalizedItems, normalizedNotes, DateTime.UtcNow);
        Entries.Add(entry);
        SynchronizeEntries();
        return true;
    }

    public bool UpdateEntry(GratitudeEntry entryToUpdate, DateOnly entryDate, List<string> gratitudeItems, string notes, out List<string> errors)
    {
        errors = ValidateEntry(entryDate, gratitudeItems, notes);
        if (errors.Count > 0)
        {
            return false;
        }

        int index = Entries.IndexOf(entryToUpdate);
        if (index < 0)
        {
            errors.Add("Could not find the selected entry.");
            return false;
        }

        List<string> normalizedItems = gratitudeItems
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        string normalizedNotes = notes?.Trim() ?? "";
        DateTime createdAtUtc = entryToUpdate.CreatedAtUtc;
        Entries[index] = new GratitudeEntry(entryDate, normalizedItems, normalizedNotes, createdAtUtc);
        SynchronizeEntries();
        return true;
    }

    public bool DeleteEntry(GratitudeEntry entryToDelete)
    {
        bool removed = Entries.Remove(entryToDelete);
        if (removed)
        {
            SynchronizeEntries();
        }

        return removed;
    }

    public List<GratitudeEntry> GetEntriesSorted(bool newestFirst)
    {
        if (newestFirst)
        {
            return Entries
                .OrderByDescending(x => x.EntryDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToList();
        }

        return Entries
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();
    }

    public bool HasEntryForDate(DateOnly date)
    {
        return Entries.Any(x => x.EntryDate == date);
    }

    public bool ConfigureReminder(string timeInput, out string errorMessage)
    {
        errorMessage = "";

        TimeOnly parsedTime;
        if (!TimeOnly.TryParse(timeInput, out parsedTime))
        {
            errorMessage = "Invalid time format. Please use HH:mm (example: 21:30).";
            return false;
        }

        ReminderSettings = new ReminderSettings(true, parsedTime);
        SynchronizeReminderSettings();
        return true;
    }

    public void DisableReminder()
    {
        ReminderSettings = new ReminderSettings(false, ReminderSettings.ReminderTime);
        SynchronizeReminderSettings();
    }

    public bool ShouldTriggerReminder(DateTime now)
    {
        if (!ReminderSettings.Enabled)
        {
            return false;
        }

        TimeOnly nowTime = TimeOnly.FromDateTime(now);
        return nowTime >= ReminderSettings.ReminderTime;
    }

    List<string> ValidateEntry(DateOnly entryDate, List<string> gratitudeItems, string notes)
    {
        List<string> errors = new List<string>();

        if (entryDate > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Date cannot be in the future.");
        }

        List<string> normalizedItems = gratitudeItems
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (normalizedItems.Count == 0)
        {
            errors.Add("At least one gratitude item is required.");
        }

        if (normalizedItems.Count > 3)
        {
            errors.Add("Maximum three gratitude items are allowed.");
        }

        if (normalizedItems.Any(x => x.Length > 200))
        {
            errors.Add("Each gratitude item must be 200 characters or fewer.");
        }

        if (!string.IsNullOrWhiteSpace(notes) && notes.Trim().Length > 500)
        {
            errors.Add("Notes must be 500 characters or fewer.");
        }

        return errors;
    }

    void SynchronizeEntries()
    {
        List<string> lines = new List<string>();

        foreach (var entry in Entries)
        {
            string datePart = entry.EntryDate.ToString("yyyy-MM-dd");
            string gratitudePart = string.Join(";;", entry.Gratitudes.Select(Sanitize));
            string notesPart = string.IsNullOrWhiteSpace(entry.Notes) ? "-" : Sanitize(entry.Notes);
            string createdPart = entry.CreatedAtUtc.ToString("o");

            lines.Add(datePart + "||" + gratitudePart + "||" + notesPart + "||" + createdPart);
        }

        fileSaver.OverwriteLines(lines);
    }

    void LoadReminderSettings()
    {
        string[] lines = settingsFileSaver.ReadLines();
        if (lines.Length == 0)
        {
            return;
        }

        string line = lines[0];
        string[] parts = line.Split("||");
        if (parts.Length < 2)
        {
            return;
        }

        bool enabled = parts[0] == "1";

        TimeOnly reminderTime;
        if (!TimeOnly.TryParse(parts[1], out reminderTime))
        {
            reminderTime = new TimeOnly(20, 0);
        }

        ReminderSettings = new ReminderSettings(enabled, reminderTime);
    }

    void SynchronizeReminderSettings()
    {
        string enabledPart = ReminderSettings.Enabled ? "1" : "0";
        string timePart = ReminderSettings.ReminderTime.ToString("HH:mm");
        settingsFileSaver.OverwriteLines(new List<string> { enabledPart + "||" + timePart });
    }

    string Sanitize(string text)
    {
        return text.Replace("||", " ").Replace(";;", " ").Trim();
    }
}
