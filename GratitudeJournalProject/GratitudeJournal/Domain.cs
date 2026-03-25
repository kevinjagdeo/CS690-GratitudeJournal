namespace GratitudeJournal;

public class GratitudeEntry
{
    public DateOnly EntryDate { get; }
    public List<string> Gratitudes { get; }
    public string Notes { get; }
    public DateTime CreatedAtUtc { get; }

    public GratitudeEntry(DateOnly entryDate, List<string> gratitudes, string notes, DateTime createdAtUtc)
    {
        EntryDate = entryDate;
        Gratitudes = gratitudes;
        Notes = notes;
        CreatedAtUtc = createdAtUtc;
    }
}

