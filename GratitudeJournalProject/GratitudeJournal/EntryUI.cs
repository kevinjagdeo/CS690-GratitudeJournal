namespace GratitudeJournal;

public class EntryUI
{
    readonly DataManager dataManager;

    public EntryUI(DataManager dataManager)
    {
        this.dataManager = dataManager;
    }

    public void ShowCreateEntry()
    {
        DateOnly entryDate = AskForDate();
        ShowCreateEntryForDate(entryDate);
    }

    public void ShowCreateEntryForDate(DateOnly entryDate)
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();

        Console.WriteLine("[New Entry - " + entryDate.ToString("yyyy-MM-dd") + "]");
        Console.WriteLine();
        Console.WriteLine("What are you grateful for today?");

        List<string> gratitudeItems = AskForGratitudeItems();
        string notes = ConsoleHelpers.AskForInput("Optional notes (press Enter to skip): ");

        string saveChoice = ConsoleHelpers.AskForInput("[Save? Y/N] > ");
        if (!saveChoice.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            ConsoleHelpers.Pause("Entry was not saved. Press Enter to go back to the main menu.");
            return;
        }

        bool wasSaved = dataManager.AddEntry(entryDate, gratitudeItems, notes, out var errors);

        Console.WriteLine();
        if (wasSaved)
        {
            ConsoleHelpers.Pause("Entry saved! Press Enter to go back to the main menu.");
            return;
        }

        Console.WriteLine("Entry not saved. Please fix:");
        foreach (var error in errors)
        {
            Console.WriteLine("- " + error);
        }

        ConsoleHelpers.Pause("Press Enter to go back to the main menu.");
    }

    public void ShowHistory()
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();

        if (dataManager.Entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            Console.WriteLine();
            ConsoleHelpers.Pause();
            return;
        }

        string sortChoice = ConsoleHelpers.AskForInput("Sort by date [1=Newest, 2=Oldest] (default 1): ");
        bool newestFirst = sortChoice != "2";

        List<GratitudeEntry> entries = dataManager.GetEntriesSorted(newestFirst);
        Dictionary<int, GratitudeEntry> entryMap = BuildEntryMap(entries, newestFirst);

        Console.WriteLine();
        Console.WriteLine(newestFirst
            ? "[Recent Entries - Newest to Oldest]"
            : "[Recent Entries - Oldest to Newest]");
        Console.WriteLine();

        foreach (var pair in entryMap.OrderBy(x => x.Key))
        {
            string summary = pair.Value.Gratitudes.Count > 0 ? pair.Value.Gratitudes[0] : "(No gratitude text)";
            Console.WriteLine("# " + pair.Key.ToString("D3") + " | " + pair.Value.EntryDate.ToString("yyyy-MM-dd") + " | \"" + summary + "\"");
        }

        while (true)
        {
            Console.WriteLine();
            string input = ConsoleHelpers.AskForInput("[Enter # to open entry, 0 to go back] > ");

            if (!int.TryParse(input, out int selectedNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            if (selectedNumber == 0)
            {
                return;
            }

            if (!entryMap.ContainsKey(selectedNumber))
            {
                Console.WriteLine("Entry number not found.");
                continue;
            }

            ShowSingleEntry(entryMap[selectedNumber], selectedNumber);
        }
    }

    public void ShowEditDeleteMenu()
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();

        if (dataManager.Entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            Console.WriteLine();
            ConsoleHelpers.Pause();
            return;
        }

        List<GratitudeEntry> entries = dataManager.GetEntriesSorted(true);
        Dictionary<int, GratitudeEntry> entryMap = BuildEntryMap(entries, true);

        Console.WriteLine("[Select Entry to Edit/Delete]");
        Console.WriteLine();
        foreach (var pair in entryMap.OrderBy(x => x.Key))
        {
            string summary = pair.Value.Gratitudes.Count > 0 ? pair.Value.Gratitudes[0] : "(No gratitude text)";
            Console.WriteLine("# " + pair.Key.ToString("D3") + " | " + pair.Value.EntryDate.ToString("yyyy-MM-dd") + " | \"" + summary + "\"");
        }

        while (true)
        {
            Console.WriteLine();
            string input = ConsoleHelpers.AskForInput("[Enter # to open entry, 0 to go back] > ");

            if (!int.TryParse(input, out int selectedNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            if (selectedNumber == 0)
            {
                return;
            }

            if (!entryMap.ContainsKey(selectedNumber))
            {
                Console.WriteLine("Entry number not found.");
                continue;
            }

            ShowEditDeleteActions(entryMap[selectedNumber], selectedNumber);
            return;
        }
    }

    public void ShowSearchFilterMenu()
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();

        if (dataManager.Entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            Console.WriteLine();
            ConsoleHelpers.Pause();
            return;
        }

        Console.WriteLine("[Search / Filter Entries]");
        Console.WriteLine("Leave any field blank to skip that filter.");
        Console.WriteLine();

        string keyword = ConsoleHelpers.AskForInput("Keyword: ");
        DateOnly? startDate = AskForOptionalFilterDate("Start date (yyyy-MM-dd): ");
        DateOnly? endDate = AskForOptionalFilterDate("End date (yyyy-MM-dd): ");

        if (!dataManager.IsValidDateRange(startDate, endDate, out string rangeError))
        {
            Console.WriteLine();
            Console.WriteLine(rangeError);
            ConsoleHelpers.Pause();
            return;
        }

        string sortChoice = ConsoleHelpers.AskForInput("Sort by date [1=Newest, 2=Oldest] (default 1): ");
        bool newestFirst = sortChoice != "2";

        List<GratitudeEntry> results = dataManager.SearchEntries(keyword, startDate, endDate, newestFirst);
        Console.WriteLine();
        Console.WriteLine("[Search Results]");

        if (results.Count == 0)
        {
            Console.WriteLine("No matching entries found.");
            ConsoleHelpers.Pause();
            return;
        }

        Dictionary<int, GratitudeEntry> entryMap = BuildEntryMap(results, newestFirst);
        Console.WriteLine();
        foreach (var pair in entryMap.OrderBy(x => x.Key))
        {
            string summary = pair.Value.Gratitudes.Count > 0 ? pair.Value.Gratitudes[0] : "(No gratitude text)";
            Console.WriteLine("# " + pair.Key.ToString("D3") + " | " + pair.Value.EntryDate.ToString("yyyy-MM-dd") + " | \"" + summary + "\"");
        }

        while (true)
        {
            Console.WriteLine();
            string input = ConsoleHelpers.AskForInput("[Enter # to open entry, 0 to go back] > ");

            if (!int.TryParse(input, out int selectedNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            if (selectedNumber == 0)
            {
                return;
            }

            if (!entryMap.ContainsKey(selectedNumber))
            {
                Console.WriteLine("Entry number not found.");
                continue;
            }

            ShowSingleEntry(entryMap[selectedNumber], selectedNumber);
        }
    }

    void ShowEditDeleteActions(GratitudeEntry entry, int entryNumber)
    {
        Console.Clear();
        ConsoleHelpers.PrintHeader();
        ShowSingleEntry(entry, entryNumber);
        Console.WriteLine();
        Console.WriteLine("1. Edit entry");
        Console.WriteLine("2. Delete entry");
        Console.WriteLine("0. Back");
        string action = ConsoleHelpers.AskForInput("> ");

        if (action == "1")
        {
            EditEntry(entry);
        }
        else if (action == "2")
        {
            DeleteEntry(entry);
        }
    }

    void EditEntry(GratitudeEntry entry)
    {
        Console.WriteLine();
        Console.WriteLine("[Edit Entry]");
        Console.WriteLine("Press Enter to keep current value.");

        DateOnly newDate = AskForOptionalDate(entry.EntryDate);
        List<string> newItems = AskForOptionalGratitudeItems(entry.Gratitudes);
        string newNotes = ConsoleHelpers.AskForInput("Notes (current: " + (string.IsNullOrWhiteSpace(entry.Notes) ? "-" : entry.Notes) + "): ");
        if (string.IsNullOrWhiteSpace(newNotes))
        {
            newNotes = entry.Notes;
        }

        bool success = dataManager.UpdateEntry(entry, newDate, newItems, newNotes, out var errors);
        Console.WriteLine();
        if (success)
        {
            Console.WriteLine("Entry updated successfully.");
            ConsoleHelpers.Pause();
            return;
        }

        Console.WriteLine("Entry update failed:");
        foreach (var error in errors)
        {
            Console.WriteLine("- " + error);
        }

        ConsoleHelpers.Pause();
    }

    void DeleteEntry(GratitudeEntry entry)
    {
        Console.WriteLine();
        string confirm = ConsoleHelpers.AskForInput("Are you sure you want to delete this entry? [Y/N] > ");
        if (!confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            ConsoleHelpers.Pause("Delete cancelled. Press Enter to continue.");
            return;
        }

        bool deleted = dataManager.DeleteEntry(entry);
        Console.WriteLine(deleted ? "Entry deleted." : "Could not delete entry.");
        ConsoleHelpers.Pause();
    }

    void ShowSingleEntry(GratitudeEntry entry, int entryNumber)
    {
        Console.WriteLine();
        Console.WriteLine("[Entry #" + entryNumber.ToString("D3") + "]");
        Console.WriteLine("Date: " + entry.EntryDate.ToString("yyyy-MM-dd"));

        for (int i = 0; i < entry.Gratitudes.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + entry.Gratitudes[i]);
        }

        if (!string.IsNullOrWhiteSpace(entry.Notes))
        {
            Console.WriteLine("Notes: " + entry.Notes);
        }

        Console.WriteLine("Saved At (UTC): " + entry.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    DateOnly AskForDate()
    {
        while (true)
        {
            string input = ConsoleHelpers.AskForInput("Entry date (yyyy-MM-dd, default today): ");
            if (string.IsNullOrWhiteSpace(input))
            {
                return DateOnly.FromDateTime(DateTime.Today);
            }

            if (DateOnly.TryParse(input, out DateOnly date))
            {
                return date;
            }

            Console.WriteLine("Invalid date format.");
        }
    }

    DateOnly AskForOptionalDate(DateOnly currentDate)
    {
        while (true)
        {
            string input = ConsoleHelpers.AskForInput("Entry date (current: " + currentDate.ToString("yyyy-MM-dd") + "): ");
            if (string.IsNullOrWhiteSpace(input))
            {
                return currentDate;
            }

            if (DateOnly.TryParse(input, out DateOnly parsed))
            {
                return parsed;
            }

            Console.WriteLine("Invalid date format.");
        }
    }

    DateOnly? AskForOptionalFilterDate(string prompt)
    {
        while (true)
        {
            string input = ConsoleHelpers.AskForInput(prompt);
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (DateOnly.TryParse(input, out DateOnly parsed))
            {
                return parsed;
            }

            Console.WriteLine("Invalid date format.");
        }
    }

    List<string> AskForGratitudeItems()
    {
        List<string> items = new List<string>();

        for (int i = 1; i <= 3; i++)
        {
            string item = ConsoleHelpers.AskForInput("> ");
            if (!string.IsNullOrWhiteSpace(item))
            {
                items.Add(item);
            }
        }

        return items;
    }

    List<string> AskForOptionalGratitudeItems(List<string> currentItems)
    {
        Console.WriteLine("Current gratitude items:");
        for (int i = 0; i < currentItems.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + currentItems[i]);
        }
        Console.WriteLine("Enter up to 3 new items. Press Enter for each to keep current items.");

        List<string> items = new List<string>();
        bool enteredAny = false;
        for (int i = 1; i <= 3; i++)
        {
            string item = ConsoleHelpers.AskForInput("> ");
            if (!string.IsNullOrWhiteSpace(item))
            {
                items.Add(item);
                enteredAny = true;
            }
        }

        if (!enteredAny)
        {
            return new List<string>(currentItems);
        }

        return items;
    }

    Dictionary<int, GratitudeEntry> BuildEntryMap(List<GratitudeEntry> entries, bool newestFirst)
    {
        Dictionary<int, GratitudeEntry> entryMap = new Dictionary<int, GratitudeEntry>();

        for (int i = 0; i < entries.Count; i++)
        {
            int displayNumber = i + 1;
            entryMap[displayNumber] = entries[i];
        }

        return entryMap;
    }
}
