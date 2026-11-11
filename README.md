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
- List work items: all, by status, or by assignee. After choosing what to list, you pick the order: priority (default), newest first, or title A-Z. The orderings use the **Strategy design pattern**, so a new ordering is easy to add (see [Strategy pattern](#strategy-pattern)).
- Show a summary with the number of work items in each status.
- Sample data loads at startup, so the app can be tried straight away.

## Getting started

**Requirements:** the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
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

```text
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
| 4. List all work items | an order: 1 priority (Enter for the default), 2 newest first, 3 title A-Z |
| 5. List by status | a status (by number), then an order |
| 6. List by assignee | a user Id, then an order |
| 7. Assign work item | a work item Id and a user Id |
| 8. Change status | a work item Id and the new status (by number) |

Invalid input, such as letters where a number is expected or an unknown menu option, shows a message and asks again. The app doesn't crash.

When a rule rejects an operation, nothing changes, the app prints `Error: <reason>`, and the menu comes back.

## Business rules

### Users

- The name is required, trimmed, and at most 50 characters.
- Names are unique, ignoring case. `Sara` and `sara` count as the same name.
- Names can only contain letters and spaces.
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

- You choose the order after choosing what to list (options 4, 5 and 6). Pressing Enter gives the default.
- **Priority** (default): High, then Medium, then Low, then by Id, lowest first.
- **Newest first**: by created date, newest first; items created at the same time go highest Id first.
- **Title A-Z**: alphabetical by title.
- Listing by assignee rejects a user Id that doesn't exist.
- The summary shows all three statuses, including ones with 0 items.

## Example session

```text
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

```text
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
  WorkItemService.cs    Creates, assigns and changes work items, and runs the filters and summary.
Ordering/
  IWorkItemOrdering.cs  The strategy interface: a Name and an Order method.
  PriorityOrdering.cs   Priority, then Id.
  NewestFirstOrdering.cs  Newest first, then highest Id first.
  TitleOrdering.cs      Title A-Z, ignoring case, then Id.
Data/
  SampleData.cs         The sample data loaded at startup. Receives the services from Program.cs.
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

A rejected operation throws a custom `DomainException` containing a user-friendly message. The menu loop in `Program.cs` catches this exception in one central place, displays `Error: <message>`, and prompts the user again. ID counters only increment after an operation succeeds, ensuring that rejected operations do not consume IDs.

#### Why a Custom Exception?

Catch blocks filter errors by exception type. If business logic threw a generic `Exception`, the menu loop would need to catch all `Exception` types—inadvertently masking critical bugs as ordinary user errors. Using a dedicated `DomainException` allows the application to handle domain rule violations specifically:

```csharp
// In service or model:
throw new DomainException("A user named 'Sara' already exists.");

// In main menu loop:
catch (DomainException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

- **Expected Rule Rejections (Bad input or business rules):** Caught gracefully by `DomainException`, showing a friendly `Error: <reason>` message and returning the user to the menu.
- **Unexpected System Errors (Bugs like `NullReferenceException`):** Allowed to bypass the catch block, intentionally crashing the application to output a stack trace. This prevents silent failures and makes root causes easy to locate and fix.

**Key Design Benefits:**

- **Safe Error Messages:** Every `DomainException` message is explicitly written for end-user display, allowing the console layer to print `ex.Message` directly.
- **Centralized Handling:** Models and services only throw exceptions; they never interact with the console. `Program.cs` is the sole place that catches and displays domain errors, preserving clean separation of concerns.
- **State Consistency:** Throwing an exception instantly halts execution before state changes are applied, ensuring operations fail atomically without leaving half-modified data.

### One instance of each service

`Program.cs` creates one `UserService` and one `WorkItemService` (which receives the `UserService` in its constructor). It passes them to `SampleData.Load`, so nothing else creates or looks up a service on its own.

### Strategy pattern

The Strategy design pattern puts each version of an algorithm in its own class behind a shared interface, so the caller can pick one at run time without knowing how it works. Here the algorithm is "how to order work items".

Each way of ordering work items is its own class that implements `IWorkItemOrdering` (`PriorityOrdering`, `NewestFirstOrdering`, `TitleOrdering`). `Program.cs` keeps them in one `orderings` list. The order menu and the input check are built from that list, and the first entry is the default.

To add a new ordering:

1. Create a class in `Ordering/` that implements `IWorkItemOrdering`.
2. Add one line for it to the `orderings` list in `Program.cs`.

The menu, the numbering and the validation update by themselves, and `WorkItemService` doesn't change.

### Data structures

- Users and work items are stored in a `Dictionary<int, T>` keyed by Id, for direct lookup.
- A `HashSet<string>` with a case-insensitive comparer checks for duplicate user names.
- Lists are sorted with LINQ in the classes in `Ordering/`, which work on any list, so they can be applied after filtering. `Priority` is declared High, Medium, Low, so its natural order is the order the requirements ask for.

## Limitations

- **Data is not saved.** Everything is lost when the app closes. There is no file or database storage.
- **No automated tests.** The assignment leaves them out.
- **Not supported:** deleting or editing users and work items, unassigning, due dates, comments and projects.