namespace WorkItemTracker;

// Starter menu loop. Replace each "Not implemented yet" with your own code.
// Some options already read their input with the ConsoleInput helpers, to show how they're used.
public class Program
{
    public static void Main()
    {
        while (true)
        {
            ShowMenu();

            var choice = ConsoleInput.ReadText("Choose an option");
            switch (choice)
            {
                case "0":
                    return;

                case "1":
                    // ReadText: reads a line of text.
                    var name = ConsoleInput.ReadText("Name");
                    Console.WriteLine($"Not implemented yet. You typed '{name}'.");
                    break;

                case "3":
                    var title = ConsoleInput.ReadText("Title");

                    // ReadChoice: once you've created a Priority enum, this shows
                    // "1) High  2) Medium  3) Low" and returns the value picked:
                    // var priority = ConsoleInput.ReadChoice<Priority>("Priority");

                    // ReadOptionalInt: returns null when the user just presses Enter.
                    int? assigneeId = ConsoleInput.ReadOptionalInt("Assignee user Id (Enter for none)");
                    Console.WriteLine(assigneeId is null
                        ? $"Not implemented yet. Title '{title}', no assignee."
                        : $"Not implemented yet. Title '{title}', assignee {assigneeId}.");
                    break;

                case "7":
                    // ReadInt: asks again until the user types a whole number.
                    var workItemId = ConsoleInput.ReadInt("Work item Id");
                    var userId = ConsoleInput.ReadInt("User Id");
                    Console.WriteLine($"Not implemented yet. Work item {workItemId}, user {userId}.");
                    break;

                case "2":
                case "4":
                case "5":
                case "6":
                case "8":
                case "9":
                    Console.WriteLine("Not implemented yet.");
                    break;

                default:
                    Console.WriteLine($"'{choice}' is not a menu option.");
                    break;
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Work Item Tracker ===");
        Console.WriteLine("1. Add user");
        Console.WriteLine("2. List users");
        Console.WriteLine("3. Create work item");
        Console.WriteLine("4. List all work items");
        Console.WriteLine("5. List work items by status");
        Console.WriteLine("6. List work items by assignee");
        Console.WriteLine("7. Assign work item");
        Console.WriteLine("8. Change status");
        Console.WriteLine("9. Show summary");
        Console.WriteLine("0. Exit");
    }
}
