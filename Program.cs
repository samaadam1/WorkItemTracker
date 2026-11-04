using WorkItemTracker.Data;
using WorkItemTracker.Models;
using WorkItemTracker.Services;
using WorkItemTracker.UI;

namespace WorkItemTracker;
// namespace: a way to organize code and avoid name collisions. All classes in this file are in the WorkItemTracker namespace.

// The menu loop. Each option reads its input with the ConsoleInput helpers, calls a service,
// and prints the result. The rules live in the services and models, and the list formatting in Display (C1).
public class Program
{
    private static readonly UserService Users = UserService.Instance;
    private static readonly WorkItemService WorkItems = WorkItemService.Instance;

    public static void Main()
    {
        // C5: sample data so the app can be tried immediately.
        SampleData.Load();

        while (true)
        {
            ShowMenu();

            var choice = ConsoleInput.ReadText("Choose an option");

            if (choice == "0")
            {
                return;
            }

            try
            {
                RunOption(choice);
            }
            catch (DomainException ex)
            {
                // R7: a rejected operation changed nothing. Show why and go back to the menu.
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void RunOption(string choice)
    {
        switch (choice)
        {
            case "1":
                AddUser();
                break;

            case "2":
                Display.Users(Users.GetAll());
                break;

            case "3":
                CreateWorkItem();
                break;

            case "4":
                Display.WorkItems(WorkItems.ListAll());
                break;

            case "5":
                ListWorkItemsByStatus();
                break;

            case "6":
                ListWorkItemsByAssignee();
                break;

            case "7":
                AssignWorkItem();
                break;

            case "8":
                ChangeStatus();
                break;

            case "9":
                Display.Summary(WorkItems.GetSummary());
                break;

            default:
                Console.WriteLine($"'{choice}' is not a menu option.");
                break;
        }
    }

    private static void AddUser()
    {
        var name = ConsoleInput.ReadText("Name");
        var user = Users.Add(name);
        Console.WriteLine($"User {user.Id} '{user.Name}' added.");
    }

    private static void CreateWorkItem()
    {
        var title = ConsoleInput.ReadText("Title");
        var priority = ConsoleInput.ReadChoice<Priority>("Priority");
        var assigneeId = ConsoleInput.ReadOptionalInt("Assignee user Id (Enter for none)");

        var item = WorkItems.Create(title, priority, assigneeId);
        Console.WriteLine($"Work item {item.Id} created.");
    }

    private static void ListWorkItemsByStatus()
    {
        var status = ConsoleInput.ReadChoice<WorkItemStatus>("Status");
        Display.WorkItems(WorkItems.ListByStatus(status));
    }

    private static void ListWorkItemsByAssignee()
    {
        var assigneeId = ConsoleInput.ReadInt("Assignee user Id");
        Display.WorkItems(WorkItems.ListByAssignee(assigneeId));
    }

    private static void AssignWorkItem()
    {
        var workItemId = ConsoleInput.ReadInt("Work item Id");
        var userId = ConsoleInput.ReadInt("User Id");

        var item = WorkItems.Assign(workItemId, userId);
        Console.WriteLine($"Work item {item.Id} assigned to {Display.AssigneeName(item)}.");
    }

    private static void ChangeStatus()
    {
        var workItemId = ConsoleInput.ReadInt("Work item Id");
        var newStatus = ConsoleInput.ReadChoice<WorkItemStatus>("New status");

        var item = WorkItems.ChangeStatus(workItemId, newStatus);
        Console.WriteLine($"Work item {item.Id} is now {item.Status}.");
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
