# CS690-GratitudeJournal

Console-based gratitude journal built for the CS690 final project.

## Project Structure

This project uses a simple console-app class layout:

- `GratitudeJournalProject/GratitudeJournalProject.sln` solution file
- `GratitudeJournalProject/GratitudeJournal/Program.cs` starts the application
- `GratitudeJournalProject/GratitudeJournal/ConsoleUI.cs` handles menus and user interaction
- `GratitudeJournalProject/GratitudeJournal/DataManager.cs` handles validation, storage, and sorting
- `GratitudeJournalProject/GratitudeJournal/Domain.cs` contains domain classes
- `GratitudeJournalProject/GratitudeJournal/FileSaver.cs` handles file read/write

## Scope Implemented

This version implements the following use cases from the project wiki:

- UC1: Create a gratitude entry
  - FR1.1 New entry form
  - FR1.2 Save entry
  - FR1.3 Validate entry
- UC2: View past entries
  - FR2.1 History list
  - FR2.2 Sort entries by date
- UC3: Edit or delete a past entry
  - FR3.1 Open existing entry
  - FR3.2 Edit entry
  - FR3.3 Delete entry
- UC4: Daily reminder to add a journal entry
  - FR4.1 Configure reminder time
  - FR4.2 Trigger notification
  - FR4.3 Open current date on click/tap (console equivalent: prompt to open today's entry)

## Release

- Current iteration release tag: `v1.0.0`
- Expected release asset name: `CS690-GratitudeJournal-v1.0.0.zip`
- Releases page: `https://github.com/kevinjagdeo/CS690-GratitudeJournal/releases`

## Prerequisites

- .NET SDK 10

## Run

```bash
cd GratitudeJournalProject
dotnet restore GratitudeJournalProject.sln
dotnet run --project GratitudeJournal/GratitudeJournal.csproj
```

## Run from v1.0.0 Release Zip

If you downloaded `CS690-GratitudeJournal-v1.0.0.zip` from GitHub Releases:

```bash
cd CS690-GratitudeJournal-v1.0.0/GratitudeJournalProject
dotnet restore GratitudeJournalProject.sln
dotnet run --project GratitudeJournal/GratitudeJournal.csproj
```

## Data Storage

Entries are stored in `GratitudeJournalProject/data/entries.txt`.
