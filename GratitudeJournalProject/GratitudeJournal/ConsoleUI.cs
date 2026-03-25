namespace GratitudeJournal;

public class ConsoleUI
{
    DataManager dataManager;

    public ConsoleUI()
    {
        dataManager = new DataManager();
    }

    public void Show()
    {
        string command;

        do
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. New Entry   2. View Entries   0. Exit");
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

