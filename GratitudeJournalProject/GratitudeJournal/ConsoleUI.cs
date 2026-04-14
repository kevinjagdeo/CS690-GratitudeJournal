namespace GratitudeJournal;

public class ConsoleUI
{
    readonly EntryUI entryUI;
    readonly ReminderUI reminderUI;

    public ConsoleUI()
    {
        var dataManager = new DataManager();
        entryUI = new EntryUI(dataManager);
        reminderUI = new ReminderUI(dataManager, entryUI);
    }

    public void Show()
    {
        string command;

        do
        {
            reminderUI.CheckAndTriggerReminder();
            Console.Clear();
            ConsoleHelpers.PrintHeader();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. New Entry   2. View Entries   3. Edit/Delete Entry   4. Settings   0. Exit");
            Console.WriteLine();
            command = ConsoleHelpers.AskForInput("> ");

            if (command == "1")
            {
                entryUI.ShowCreateEntry();
            }
            else if (command == "2")
            {
                entryUI.ShowHistory();
            }
            else if (command == "3")
            {
                entryUI.ShowEditDeleteMenu();
            }
            else if (command == "4")
            {
                reminderUI.ShowSettings();
            }
            else if (command != "0")
            {
                ConsoleHelpers.Pause("Invalid option. Press Enter to continue.");
            }
        } while (command != "0");
    }
}

