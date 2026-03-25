namespace GratitudeJournal;

public class DataManager
{
    FileSaver fileSaver;

    public List<GratitudeEntry> Entries { get; }

    public DataManager()
    {
        string dataFilePath = Path.Combine("data", "entries.txt");
        fileSaver = new FileSaver(dataFilePath);
        Entries = new List<GratitudeEntry>();
        LoadEntries();
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

    string Sanitize(string text)
    {
        return text.Replace("||", " ").Replace(";;", " ").Trim();
    }
}

