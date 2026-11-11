using WorkItemTracker.Models;

namespace WorkItemTracker.UI;

// Everything the app prints for lists and summaries (C1, C3). It only reads data and never changes it.
public static class Display
{
    public static void Users(IReadOnlyList<User> users)
    {
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id}  {user.Name}");
        }
    }

    // C3: Id, Title, Priority, Status, Assignee name (or "Unassigned"), CreatedAt.
    public static void WorkItems(IReadOnlyList<WorkItem> items, IReadOnlyDictionary<int, string> assigneeNames)
    {
        Console.WriteLine();

        if (items.Count == 0)
        {
            Console.WriteLine("No matching work items.");
            return;
        }

        // Each column is as wide as its longest value, so the columns line up whatever the data is.
        var rows = items.Select(i => new[]
        {
            i.Id.ToString(),
            i.Title,
            i.Priority.ToString(),
            i.Status.ToString(),
            assigneeNames[i.Id],
            i.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        }).ToList();

        string[] headers = { "Id", "Title", "Priority", "Status", "Assignee", "Created" };
        var widths = headers
            .Select((header, column) => Math.Max(header.Length, rows.Max(row => row[column].Length)))
            .ToArray();

        Console.WriteLine(FormatRow(headers, widths, rightAlignFirst: false));
        foreach (var row in rows)
        {
            Console.WriteLine(FormatRow(row, widths, rightAlignFirst: true));
        }
    }

    // Q3: every status with its count, 0 included.
    public static void Summary(IReadOnlyDictionary<WorkItemStatus, int> counts)
    {
        Console.WriteLine(string.Join("   ", counts.Select(pair => $"{pair.Key}: {pair.Value}")));
    }



    // The Id column is right-aligned like a number, the others are left-aligned.
    private static string FormatRow(string[] cells, int[] widths, bool rightAlignFirst)
    {
        var parts = cells.Select((cell, column) =>
            column == 0 && rightAlignFirst ? cell.PadLeft(widths[column]) : cell.PadRight(widths[column]));
        return string.Join("  ", parts).TrimEnd();
    }
}
