# Work Item Tracker: Project Requirements

In this task you'll apply what you've studied about OOP, data structures, SOLID and design patterns in a small **C# console application** that manages users and work items **in memory**. It's the same domain as the SQL assessment and the API we'll build later.

If something isn't listed here, you don't need to build it. If anything is unclear, ask me on Slack.

The requirements are numbered so we can refer to them easily when we talk:

- **R** = business rules (what the app allows and rejects)
- **Q** = queries (listing and summarizing work items)
- **C** = console rules (how the app talks to the user)

---

## 1. C# topics to know first

If C# is new to you, get comfortable with these before you start. Any source is fine.

- **Project basics:** creating and running a console app with `dotnet new console` and `dotnet run`, namespaces, one class per file.
- **Classes:** properties (`get` / `set`), constructors, access modifiers (`public`, `private`), `enum`, and nullable types such as `int?`.
- **Collections:** `List<T>`, `Dictionary<TKey, TValue>` and `HashSet<T>`, and comparing strings while ignoring case.
- **LINQ basics:** LINQ (Language Integrated Query) is a set of methods for querying collections in C#, a bit like SQL for in-memory lists. Learn `Where` (filter), `OrderBy` / `ThenBy` (sort), `Count` and `ToList`.
- **Exceptions:** `throw`, `try` / `catch`, and writing a small custom exception class.
- **Strings and dates:** string interpolation (`$"Hello {name}"`), `Trim`, `string.IsNullOrWhiteSpace`, `DateTime.Now`.

---

## 2. Technical setup

- **Language / runtime:** C#, **.NET 8**
- **Project:** one console application named `WorkItemTracker`
- **Packages:** no external NuGet packages
- **Repository:** a new repository on **your GitHub account**. Send me the link when you create it. If it's private, add me as a collaborator.
- **Commits:** commit as you go with clear messages (at least 5), rather than one big commit at the end.

There's no database, no web API and no UI framework. Everything lives in memory while the app runs.

### Starter files

I've attached two starter files so you don't have to spend time on console plumbing:

- **`Program.cs`**: a `Program` class whose `Main` method runs the menu loop. It shows the menu, reads the choice, and prints "Not implemented yet" for every option. Options 1, 3 and 7 already read their input with the helpers below, as examples of how to use them. Replace the `Program.cs` that `dotnet new console` creates with this one.
- **`ConsoleInput.cs`**: helpers for reading input. They show a prompt and ask again when the input isn't valid:
  - `ReadText("Name")` reads a line of text,
  - `ReadInt("Work item Id")` reads a whole number,
  - `ReadOptionalInt("Assignee user Id (Enter for none)")` reads a whole number, or returns `null` when the user presses Enter,
  - `ReadChoice<Priority>("Priority")` shows an enum's values as a numbered list and returns the one picked.

You can use them as they are, and change them if you want to.

---

## 3. The model

### User

| Field | Rules |
|---|---|
| Id | Whole number, assigned automatically: 1, 2, 3, … |
| Name | Required. Leading/trailing spaces are removed. At most 50 characters. **Unique, ignoring case** ("Sara" and "sara" count as the same name). |

### Work item

| Field | Rules |
|---|---|
| Id | Whole number, assigned automatically: 1, 2, 3, … (a separate sequence from users) |
| Title | Required. Leading/trailing spaces are removed. At most 100 characters. |
| Priority | `High`, `Medium` or `Low` (High is the most urgent) |
| Status | `ToDo`, `InProgress` or `Done` |
| Assignee | Optional: one existing user, or nobody |
| CreatedAt | Set automatically to the current date and time when the item is created. Never changes. |

---

## 4. Business rules (R)

- **R1**: A user can be added with a valid name. The new user gets the next Id.
- **R2**: Adding a user whose name already exists (ignoring case) is rejected.
- **R3**: A work item can be created with a valid title, a priority and, optionally, an assignee who must be an existing user. It starts as **`ToDo`** and gets the next Id.
- **R4**: A work item can be assigned to an existing user. Assigning again replaces the previous assignee. A `Done` work item can't be assigned.
- **R5**: Status only moves **forward, one step at a time**: `ToDo` → `InProgress` → `Done`. Anything else is rejected, for example `ToDo` → `Done`, going back, or changing to the status the item already has.
- **R6**: A work item can only move to `InProgress` if it has an assignee.
- **R7**: When an operation is rejected:
  - **nothing changes**,
  - the user sees a clear message saying what's wrong, for example `User 7 does not exist.` or `Cannot change status from ToDo to Done.`,
  - the app continues and shows the menu again.

---

## 5. Queries (Q)

- **Q1, listing:** work items can be listed in three ways: **all** of them, **by status**, or **by assignee** (a user Id; an Id that doesn't exist is rejected).
- **Q2, ordering:** every list of work items is ordered by **priority** (High, then Medium, then Low), then by **Id** (lowest first).
- **Q3, summary:** the number of work items for **each of the three statuses, including statuses with 0 items**.

---

## 6. Console application (C)

- **C1**: Keep console input and output separate from the business logic.
- **C2**: Invalid input (letters where a number is expected, an unknown menu option, a number outside a list) shows a clear message, and the app never crashes. The starter helpers already handle this for input.
- **C3**: Work item lists show, for each item: **Id, Title, Priority, Status, Assignee name (or "-"), CreatedAt**.
- **C4**: After every successful action, a short confirmation is printed, e.g. `Work item 4 assigned to Khaled.`
- **C5**: When the app starts, it loads sample data so it can be tried immediately:
  - at least **3 users**, including **one user with no work items**,
  - at least **5 work items** covering **every status** and **every priority**, with **at least one unassigned** item.

### The menu

The app shows this menu, runs the chosen action, then shows the menu again until the user picks `0`:

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

### What each option does

The examples below show what each option should produce. The data is made up, and the exact wording and layout are up to you.

**1. Add user**: asks for a name and confirms the new user (R1, R2).

```
Name: Khaled
User 4 'Khaled' added.
```

```
Name: sara
Error: A user named 'sara' already exists.
```

**2. List users**: shows every user's Id and name.

```
1  Sara
2  Adel
3  Lina
4  Khaled
```

**3. Create work item**: asks for a title, a priority (by number) and an optional assignee (Enter for none), then confirms the new item (R3).

```
Title: Deploy app
Priority [1) High  2) Medium  3) Low]: 1
Assignee user Id (Enter for none): 2
Work item 6 created.
```

```
Title: Deploy app
Priority [1) High  2) Medium  3) Low]: 1
Assignee user Id (Enter for none): 99
Error: User 99 does not exist.
```

**4. List all work items**: shows every work item in the order from Q2 (Q1, Q2, C3).

```
Id  Title               Priority  Status      Assignee  Created
 1  Design homepage     High      Done        Sara      2026-10-26 09:15
 2  Fix login bug       High      InProgress  Adel      2026-10-26 09:15
 4  Set up CI pipeline  Medium    ToDo        Adel      2026-10-26 09:15
 3  Write API docs      Low       ToDo        -         2026-10-26 09:15
```

**5. List work items by status**: asks for a status (by number) and shows the matching items.

```
Status [1) ToDo  2) InProgress  3) Done]: 1

Id  Title               Priority  Status  Assignee  Created
 4  Set up CI pipeline  Medium    ToDo    Adel      2026-10-26 09:15
 3  Write API docs      Low       ToDo    -         2026-10-26 09:15
```

**6. List work items by assignee**: asks for a user Id and shows that user's items.

```
Assignee user Id: 2

Id  Title               Priority  Status      Assignee  Created
 2  Fix login bug       High      InProgress  Adel      2026-10-26 09:15
 4  Set up CI pipeline  Medium    ToDo        Adel      2026-10-26 09:15
```

If a list has no items: `No matching work items.`

**7. Assign work item**: asks for a work item Id and a user Id (R4).

```
Work item Id: 3
User Id: 1
Work item 3 assigned to Sara.
```

```
Work item Id: 1
User Id: 2
Error: Work item 1 is Done and can't be assigned.
```

**8. Change status**: asks for a work item Id and the new status (by number) (R5, R6).

```
Work item Id: 4
New status [1) ToDo  2) InProgress  3) Done]: 2
Work item 4 is now InProgress.
```

```
Work item Id: 3
New status [1) ToDo  2) InProgress  3) Done]: 3
Error: Cannot change status from ToDo to Done.
```

**9. Show summary**: shows the count for each status (Q3). A status with no items shows 0.

```
ToDo: 2   InProgress: 1   Done: 1
```

**0. Exit**: closes the app.

**Invalid input** anywhere (C2):

```
Choose an option: abc
'abc' is not a menu option.
```

```
Work item Id: x
Please enter a whole number.
Work item Id:
```

---

## 7. What to deliver

Push the project to your GitHub repository. It should build and run with `dotnet run`. When it's ready, send me the link and we'll go through it together.

---

## 8. Out of scope

These aren't part of this project, so you can keep things simple:

- Saving to files or a database
- Automated tests (we'll cover testing later)
- Deleting users or work items, unassigning, or editing titles
- Due dates, comments, projects
- Login, colors or UI libraries
- `async`/`await`, dependency injection containers.
