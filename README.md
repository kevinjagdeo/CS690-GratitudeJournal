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
