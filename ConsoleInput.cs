namespace WorkItemTracker;

/// <summary>
/// Helpers for reading input from the console. They're provided with the task, so you can
/// use them as they are and spend your time on the rest of the app.
/// Each one shows a label, waits for the user to type, and asks again if the input isn't valid.
/// </summary>
public static class ConsoleInput
{
    /// <summary>
    /// Reads one line of text. Returns an empty string if the user just presses Enter.
    /// Example: <c>var name = ConsoleInput.ReadText("Name");</c>
    /// </summary>
    public static string ReadText(string label)
    {
        Console.Write($"{label}: ");
        var input = Console.ReadLine();
        if (input is null)
        {
            // The input was closed (for example with Ctrl+Z), so there's nothing more to read.
            Environment.Exit(0);
        }

        return input;
    }

    /// <summary>
    /// Reads a whole number.
    /// Example: <c>var id = ConsoleInput.ReadInt("Work item Id");</c>
    /// </summary>
    public static int ReadInt(string label)
    {
        while (true)
        {
            if (int.TryParse(ReadText(label), out var number))
            {
                return number;
            }

            Console.WriteLine("Please enter a whole number.");
        }
    }

    /// <summary>
    /// Reads a whole number, or returns null if the user just presses Enter.
    /// Example: <c>int? assigneeId = ConsoleInput.ReadOptionalInt("Assignee user Id (Enter for none)");</c>
    /// </summary>
    public static int? ReadOptionalInt(string label)
    {
        while (true)
        {
            var input = ReadText(label);
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out var number))
            {
                return number;
            }

            Console.WriteLine("Please enter a whole number, or press Enter to skip.");
        }
    }

    /// <summary>
    /// Shows the values of an enum as a numbered list, e.g. "1) High  2) Medium  3) Low",
    /// and returns the value the user picks.
    /// Example: <c>var priority = ConsoleInput.ReadChoice&lt;Priority&gt;("Priority");</c>
    /// </summary>
    public static T ReadChoice<T>(string label) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        var options = string.Join("  ", values.Select((value, index) => $"{index + 1}) {value}"));
        while (true)
        {
            if (int.TryParse(ReadText($"{label} [{options}]"), out var number)
                && number >= 1 && number <= values.Length)
            {
                return values[number - 1];
            }

            Console.WriteLine($"Choose a number from 1 to {values.Length}.");
        }
    }
}
