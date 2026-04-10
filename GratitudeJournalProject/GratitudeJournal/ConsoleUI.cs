namespace GratitudeJournal;

public class ConsoleUI
{
    DataManager dataManager;
    bool reminderCheckedForThisSession;

    public ConsoleUI()
    {
        dataManager = new DataManager();
        reminderCheckedForThisSession = false;
    }

    public void Show()
    {
        string command;

        do
        {
            CheckAndTriggerReminder();
            Console.Clear();
            PrintHeader();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. New Entry   2. View Entries   3. Edit/Delete Entry   4. Settings   0. Exit");
            Console.WriteLine();
            command = AskForInput("> ");

            if (command == "1")
            {
                ShowCreateEntry();
            }
            else if (command == "2")
            {
                ShowHistory();
            }
            else if (command == "3")
            {
                ShowEditDeleteMenu();
            }
            else if (command == "4")
            {
                ShowReminderSettings();
            }
            else if (command != "0")
            {
                Console.WriteLine("Invalid option. Press Enter to continue.");
                Console.ReadLine();
            }
        } while (command != "0");
    }

    void ShowCreateEntry()
    {
        Console.Clear();
        PrintHeader();

        DateOnly entryDate = AskForDate();

        Console.WriteLine();
        Console.WriteLine("[New Entry - " + entryDate.ToString("yyyy-MM-dd") + "]");
        Console.WriteLine();
        Console.WriteLine("What are you grateful for today?");

        List<string> gratitudeItems = AskForGratitudeItems();
        string notes = AskForInput("Optional notes (press Enter to skip): ");

        string saveChoice = AskForInput("[Save? Y/N] > ");
        if (!saveChoice.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            Console.WriteLine("Entry was not saved. Press Enter to go back to the main menu.");
            Console.ReadLine();
            return;
        }

        List<string> errors;
        bool wasSaved = dataManager.AddEntry(entryDate, gratitudeItems, notes, out errors);

        Console.WriteLine();
        if (wasSaved)
        {
            Console.WriteLine("Entry saved! Press Enter to go back to the main menu.");
        }
        else
        {
            Console.WriteLine("Entry not saved. Please fix:");
            foreach (var error in errors)
            {
                Console.WriteLine("- " + error);
            }
            Console.WriteLine("Press Enter to go back to the main menu.");
        }

        Console.ReadLine();
    }

    void ShowHistory()
    {
        Console.Clear();
        PrintHeader();

        if (dataManager.Entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        string sortChoice = AskForInput("Sort by date [1=Newest, 2=Oldest] (default 1): ");
        bool newestFirst = sortChoice != "2";

        List<GratitudeEntry> entries = dataManager.GetEntriesSorted(newestFirst);
        Dictionary<int, GratitudeEntry> entryMap = new Dictionary<int, GratitudeEntry>();

        Console.WriteLine();
        if (newestFirst)
        {
            Console.WriteLine("[Recent Entries - Newest to Oldest]");
        }
        else
        {
            Console.WriteLine("[Recent Entries - Oldest to Newest]");
        }
        Console.WriteLine();

        for (int i = 0; i < entries.Count; i++)
        {
            GratitudeEntry entry = entries[i];
            int displayNumber = newestFirst ? entries.Count - i : i + 1;
            string summary = entry.Gratitudes.Count > 0 ? entry.Gratitudes[0] : "(No gratitude text)";
            Console.WriteLine("# " + displayNumber.ToString("D3") + " | " + entry.EntryDate.ToString("yyyy-MM-dd") + " | \"" + summary + "\"");
            entryMap[displayNumber] = entry;
        }

        while (true)
        {
            Console.WriteLine();
            string input = AskForInput("[Enter # to open entry, 0 to go back] > ");

            int selectedNumber;
            if (!int.TryParse(input, out selectedNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            if (selectedNumber == 0)
            {
                break;
            }

            if (!entryMap.ContainsKey(selectedNumber))
            {
                Console.WriteLine("Entry number not found.");
                continue;
            }

            ShowSingleEntry(entryMap[selectedNumber], selectedNumber);
        }
    }

    void ShowEditDeleteMenu()
    {
        Console.Clear();
        PrintHeader();

        if (dataManager.Entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        List<GratitudeEntry> entries = dataManager.GetEntriesSorted(true);
        Dictionary<int, GratitudeEntry> entryMap = new Dictionary<int, GratitudeEntry>();

        Console.WriteLine("[Select Entry to Edit/Delete]");
        Console.WriteLine();
        for (int i = 0; i < entries.Count; i++)
        {
            GratitudeEntry entry = entries[i];
            int displayNumber = entries.Count - i;
            string summary = entry.Gratitudes.Count > 0 ? entry.Gratitudes[0] : "(No gratitude text)";
            Console.WriteLine("# " + displayNumber.ToString("D3") + " | " + entry.EntryDate.ToString("yyyy-MM-dd") + " | \"" + summary + "\"");
            entryMap[displayNumber] = entry;
        }

        while (true)
        {
            Console.WriteLine();
            string input = AskForInput("[Enter # to open entry, 0 to go back] > ");

            int selectedNumber;
            if (!int.TryParse(input, out selectedNumber))
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

    void ShowEditDeleteActions(GratitudeEntry entry, int entryNumber)
    {
        Console.Clear();
        PrintHeader();
        ShowSingleEntry(entry, entryNumber);
        Console.WriteLine();
        Console.WriteLine("1. Edit entry");
        Console.WriteLine("2. Delete entry");
        Console.WriteLine("0. Back");
        string action = AskForInput("> ");

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
        string newNotes = AskForInput("Notes (current: " + (string.IsNullOrWhiteSpace(entry.Notes) ? "-" : entry.Notes) + "): ");
        if (string.IsNullOrWhiteSpace(newNotes))
        {
            newNotes = entry.Notes;
        }

        List<string> errors;
        bool success = dataManager.UpdateEntry(entry, newDate, newItems, newNotes, out errors);
        Console.WriteLine();
        if (success)
        {
            Console.WriteLine("Entry updated successfully.");
        }
        else
        {
            Console.WriteLine("Entry update failed:");
            foreach (var error in errors)
            {
                Console.WriteLine("- " + error);
            }
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    void DeleteEntry(GratitudeEntry entry)
    {
        Console.WriteLine();
        string confirm = AskForInput("Are you sure you want to delete this entry? [Y/N] > ");
        if (!confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Delete cancelled. Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        bool deleted = dataManager.DeleteEntry(entry);
        Console.WriteLine(deleted ? "Entry deleted." : "Could not delete entry.");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    void ShowReminderSettings()
    {
        Console.Clear();
        PrintHeader();

        Console.WriteLine("[Reminder Settings]");
        Console.WriteLine("Current status: " + (dataManager.ReminderSettings.Enabled ? "Enabled" : "Disabled"));
        Console.WriteLine("Current time: " + dataManager.ReminderSettings.ReminderTime.ToString("HH:mm"));
        Console.WriteLine();
        Console.WriteLine("1. Configure reminder time");
        Console.WriteLine("2. Disable reminder");
        Console.WriteLine("0. Back");
        string choice = AskForInput("> ");

        if (choice == "1")
        {
            string timeInput = AskForInput("Enter reminder time (HH:mm): ");
            string errorMessage;
            bool configured = dataManager.ConfigureReminder(timeInput, out errorMessage);
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
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
        else if (choice == "2")
        {
            dataManager.DisableReminder();
            Console.WriteLine();
            Console.WriteLine("Reminder disabled.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
    }

    void CheckAndTriggerReminder()
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
        PrintHeader();
        Console.WriteLine("[Reminder]");
        Console.WriteLine("Time to write today's gratitude entry.");
        string openNow = AskForInput("Open today's entry now? [Y/N] > ");
        if (openNow.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            ShowCreateEntryForDate(today);
        }
    }

    void ShowCreateEntryForDate(DateOnly entryDate)
    {
        Console.Clear();
        PrintHeader();

        Console.WriteLine("[New Entry - " + entryDate.ToString("yyyy-MM-dd") + "]");
        Console.WriteLine();
        Console.WriteLine("What are you grateful for today?");

        List<string> gratitudeItems = AskForGratitudeItems();
        string notes = AskForInput("Optional notes (press Enter to skip): ");
        string saveChoice = AskForInput("[Save? Y/N] > ");
        if (!saveChoice.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            Console.WriteLine("Entry was not saved. Press Enter to go back to the main menu.");
            Console.ReadLine();
            return;
        }

        List<string> errors;
        bool wasSaved = dataManager.AddEntry(entryDate, gratitudeItems, notes, out errors);
        Console.WriteLine();
        if (wasSaved)
        {
            Console.WriteLine("Entry saved! Press Enter to go back to the main menu.");
        }
        else
        {
            Console.WriteLine("Entry not saved. Please fix:");
            foreach (var error in errors)
            {
                Console.WriteLine("- " + error);
            }
            Console.WriteLine("Press Enter to go back to the main menu.");
        }

        Console.ReadLine();
    }

    DateOnly AskForDate()
    {
        while (true)
        {
            string input = AskForInput("Entry date (yyyy-MM-dd, default today): ");
            if (string.IsNullOrWhiteSpace(input))
            {
                return DateOnly.FromDateTime(DateTime.Today);
            }

            DateOnly date;
            if (DateOnly.TryParse(input, out date))
            {
                return date;
            }

            Console.WriteLine("Invalid date format.");
        }
    }

    List<string> AskForGratitudeItems()
    {
        List<string> items = new List<string>();

        for (int i = 1; i <= 3; i++)
        {
            string item = AskForInput("> ");
            if (!string.IsNullOrWhiteSpace(item))
            {
                items.Add(item);
            }
        }

        return items;
    }

    DateOnly AskForOptionalDate(DateOnly currentDate)
    {
        while (true)
        {
            string input = AskForInput("Entry date (current: " + currentDate.ToString("yyyy-MM-dd") + "): ");
            if (string.IsNullOrWhiteSpace(input))
            {
                return currentDate;
            }

            DateOnly parsed;
            if (DateOnly.TryParse(input, out parsed))
            {
                return parsed;
            }

            Console.WriteLine("Invalid date format.");
        }
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
            string item = AskForInput("> ");
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

    void PrintHeader()
    {
        Console.WriteLine("==================== Gratitude Journal ====================");
        Console.WriteLine();
    }

    public static string AskForInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? "";
    }
}
