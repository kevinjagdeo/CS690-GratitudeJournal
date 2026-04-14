namespace GratitudeJournal;

public static class ConsoleHelpers
{
    public static void PrintHeader()
    {
        Console.WriteLine("==================== Gratitude Journal ====================");
        Console.WriteLine();
    }

    public static string AskForInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? "";
    }

    public static void Pause(string message = "Press Enter to continue.")
    {
        Console.WriteLine(message);
        Console.ReadLine();
    }
}

