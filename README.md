# Work Item Tracker

A C# console application for managing users and work items. It runs on .NET 8, keeps everything in memory, and uses no database and no external packages.

This project is a learning exercise in OOP, data structures, SOLID principles and design patterns. The full assignment is in [requirements.md](requirements.md).

## Contents

- [Features](#features)
- [Getting started](#getting-started)
- [Using the app](#using-the-app)
- [Business rules](#business-rules)
- [Example session](#example-session)
- [Project structure](#project-structure)
- [Design](#design)
- [Limitations](#limitations)

## Features

- Add users and list them.
- Create work items with a title, a priority and an optional assignee.
- Assign a work item to a user.
- Move a work item forward through its statuses.
- List work items: all, by status, or by assignee. Lists are always sorted by priority, then by Id.
- Show a summary with the number of work items in each status.
- Sample data loads at startup, so the app can be tried straight away.

## Getting started

**Requirements:** the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
git clone https://github.com/samaadam1/WorkItemTracker.git
cd WorkItemTracker
dotnet run
```

### Sample data

On startup the app creates:

| Users | Work items |
|---|---|
| Sara | Design homepage (High, Done, Sara) |
| Adel | Fix login bug (High, InProgress, Adel) |
| Lina (no work items) | Set up CI pipeline (Medium, ToDo, Adel) |
| | Write API docs (Low, ToDo, unassigned) |
| | Review pull requests (Low, ToDo, Sara) |

## Using the app

The app shows this menu, runs the chosen option, then shows it again until you pick `0`.

```
=== Work Item Tracker ===
1. Add user
2. List users
3. Create work item
4. List all work items
5. List work items by status
6. List work items by assignee
7. Assign work item
8. Change status
9. Show summary
0. Exit
```

| Option | What it asks for |
|---|---|
| 1. Add user | a name |
| 3. Create work item | a title, a priority (by number), an optional assignee Id (Enter for none) |
| 5. List by status | a status (by number) |
| 6. List by assignee | a user Id |
| 7. Assign work item | a work item Id and a user Id |
| 8. Change status | a work item Id and the new status (by number) |

Invalid input, such as letters where a number is expected or an unknown menu option, shows a message and asks again. The app doesn't crash.

When a rule rejects an operation, nothing changes, the app prints `Error: <reason>`, and the menu comes back.

## Business rules

### Users

- The name is required, trimmed, and at most 50 characters.
- Names are unique, ignoring case. `Sara` and `sara` count as the same name.
- Names can only contain letters and spaces. This rule goes beyond the requirements.
- Ids are assigned automatically: 1, 2, 3, ...

### Work items

- The title is required, trimmed, and at most 100 characters.
- Priority is `High`, `Medium` or `Low`.
- A new item starts as `ToDo`. Ids are assigned automatically, in a sequence separate from users.
- The assignee is optional, but must be an existing user.
- Assigning again replaces the previous assignee. A `Done` item can't be assigned.
- Status only moves forward, one step at a time: `ToDo` -> `InProgress` -> `Done`. Skipping a step, going back, or choosing the current status is rejected.
- An item can only move to `InProgress` if it has an assignee.

### Lists and summary

- Every list is ordered by priority (High, then Medium, then Low), then by Id, lowest first.
- Listing by assignee rejects a user Id that doesn't exist.
- The summary shows all three statuses, including ones with 0 items.

## Example session

```
Name: sara
Error: A user named 'sara' already exists.

Id  Title                 Priority  Status      Assignee  Created
 1  Design homepage       High      Done        Sara      2026-11-04 21:48
 2  Fix login bug         High      InProgress  Adel      2026-11-04 21:48
 4  Set up CI pipeline    Medium    ToDo        Adel      2026-11-04 21:48
 3  Write API docs        Low       ToDo        -         2026-11-04 21:48
 5  Review pull requests  Low       ToDo        Sara      2026-11-04 21:48

Work item Id: 1
User Id: 2
Error: Work item 1 is Done and can't be assigned.

Work item Id: 3
New status [1) ToDo  2) InProgress  3) Done]: 3
Error: Cannot change status from ToDo to Done.

ToDo: 3   InProgress: 1   Done: 1
```

## Project structure

```
Program.cs              Menu loop. Reads input, calls the services, prints results.
ConsoleInput.cs         Helpers for reading numbers, text and enum choices.
Models/
  User.cs               A user, with name validation.
  WorkItem.cs           A work item, with title validation and the assign and status rules.
  Priority.cs           High, Medium, Low.
  WorkItemStatus.cs     ToDo, InProgress, Done.
  DomainException.cs    Thrown when a business rule rejects an operation.
Services/
  UserService.cs        Adds users, rejects duplicate names, finds users.
  WorkItemService.cs    Creates, assigns and changes work items, and runs the lists and summary.
Patterns/
  Singleton.cs          Reusable Singleton<T> base class.
Data/
  SampleData.cs         The sample data loaded at startup.
UI/
  Display.cs            Formats the users list, the work item table and the summary.
```

## Design

### Console and business logic are separate

Models and services never read from or write to the console. Only `Program.cs` and `UI/Display.cs` print, and `ConsoleInput` reads input.

### Models protect themselves

`User` and `WorkItem` validate their fields in the constructor, so an invalid object can't exist. `Status` and `AssigneeId` have private setters and change only through `AssignTo()` and `MoveToStatus()`, which enforce the assignment and status rules.

### Services hold the rules that need more than one object

A single `User` doesn't know about the other users, so name uniqueness is checked in `UserService`. In the same way, `WorkItemService` checks that an assignee exists, because a `WorkItem` doesn't know the users.

### Errors

A rejected operation throws a `DomainException` with a message written for the user. The menu loop catches it in one place, prints `Error: <message>` and shows the menu again. The Id counters only move forward after an operation succeeds, so a rejected operation uses up no Id.

### Singleton pattern

`UserService` and `WorkItemService` inherit from `Singleton<T>` (`Patterns/Singleton.cs`), so there is exactly one users list and one work items list. Both are reached through `.Instance`, and their constructors are private.

### Data structures

- Users and work items are stored in a `Dictionary<int, T>` keyed by Id, for direct lookup.
- A `HashSet<string>` with a case-insensitive comparer checks for duplicate user names.
- Lists are sorted with LINQ: `OrderBy(Priority).ThenBy(Id)`. `Priority` is declared High, Medium, Low, so its natural order is the order the requirements ask for.

## Limitations

- **Data is not saved.** Everything is lost when the app closes. There is no file or database storage.
- **No automated tests.** The assignment leaves them out.
- **Global state.** Because the services are singletons, state is shared across the whole app, which makes a clean reset harder.
- **Not supported:** deleting or editing users and work items, unassigning, due dates, comments and projects.
