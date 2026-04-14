namespace GratitudeJournal.Tests;

public sealed class ConsoleIoScope : IDisposable
{
    readonly TextReader originalIn;
    readonly TextWriter originalOut;
    readonly StringWriter capturedOutput;
    bool disposed;

    public ConsoleIoScope(string input)
    {
        originalIn = Console.In;
        originalOut = Console.Out;
        capturedOutput = new StringWriter();

        Console.SetIn(new StringReader(input));
        Console.SetOut(capturedOutput);
    }

    public string Output => capturedOutput.ToString();

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        Console.SetIn(originalIn);
        Console.SetOut(originalOut);
        capturedOutput.Dispose();
    }
}

