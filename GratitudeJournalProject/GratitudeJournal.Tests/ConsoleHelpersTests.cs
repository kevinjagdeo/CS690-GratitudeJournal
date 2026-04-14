namespace GratitudeJournal.Tests;

public class ConsoleHelpersTests
{
    [Fact]
    public void AskForInput_ReturnsTypedValue()
    {
        using var io = new ConsoleIoScope("hello\n");

        var result = ConsoleHelpers.AskForInput("Prompt: ");

        Assert.Equal("hello", result);
    }
}

