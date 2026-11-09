// seperation of concerns: each class has a single responsibility, and the classes are organized into folders by responsibility. The UI layer (Program.cs) does not know how the services work, and the services do not know how the UI works. The models are just data, with no knowledge of the UI or services.
// decoupling: the UI layer calls the services, and the services call the models, but the models do not call the services or the UI. The models are independent of the services and UI, so they can be tested in isolation.
using WorkItemTracker.Data;
using WorkItemTracker.Models;
using WorkItemTracker.Services;
using WorkItemTracker.UI;

namespace WorkItemTracker;
// namespace: a way to organize code and avoid name collisions. All classes in this file are in the WorkItemTracker namespace.

// The menu loop. Each option reads its input with the ConsoleInput helpers, calls a service,
// and prints the result. The rules live in the services and models, and the list formatting in Display (C1).
public class Program
{   // static: one copy shared by everyone, not each object has its own
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
                ListAllWorkItems();
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
        var user = Users.AddUser(name);
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

    // Takes a list of work items to order them.
    // IEnumerable is a more general type of IReadOnlyList, 
    // so it can be used with any collection that can be enumerated (looped over), not just lists. This makes the method more flexible and reusable.
    private static void ListWorkItems(IEnumerable<WorkItem> items)
    {
        Console.WriteLine();
        Console.WriteLine("Choose an order for the work items:");
        Console.WriteLine("1. Priority (default)");
        Console.WriteLine("2. Newest first");
        Console.WriteLine("3. Title A-Z");
        var order = ConsoleInput.ReadText("Order (Enter for Priority)");

        switch (order)
        {
            case "":  // Enter = the default, Priority
            case "1":
                Display.WorkItems(WorkItems.OrderByPriority(items), Users);
                break;
            case "2":
                Display.WorkItems(WorkItems.OrderByNewestFirst(items), Users);
                break;
            case "3":
                Display.WorkItems(WorkItems.OrderByTitle(items), Users);
                break;
            default:
                Console.WriteLine($"'{order}' is not a valid order option.");
                break;
        }
    }

    private static void ListAllWorkItems()
    {
        //  WorkItems is not a list. It's the WorkItemService, 
        // the object that holds and manages the work items
        // we need to return a list of work items from the service, then pass that list to ListWorkItems
        ListWorkItems(WorkItems.GetAll());
    }

    private static void ListWorkItemsByStatus()
    {
        var status = ConsoleInput.ReadChoice<WorkItemStatus>("Status");
        // returns a list of work items filtered by the given status
        var items_by_status = WorkItems.ListByStatus(status);
        // Then chooses ordering method
        ListWorkItems(items_by_status);

    }

    private static void ListWorkItemsByAssignee()
    {
        var assigneeId = ConsoleInput.ReadInt("Assignee user Id");
        // returns a list of work items assigned to the given user
        var items_by_assignee = WorkItems.ListByAssignee(assigneeId);
        // Then chooses ordering method
        ListWorkItems(items_by_assignee);

    }

    private static void AssignWorkItem()
    {
        var workItemId = ConsoleInput.ReadInt("Work item Id");
        var userId = ConsoleInput.ReadInt("User Id");

        var item = WorkItems.Assign(workItemId, userId);
        Console.WriteLine($"Work item {item.Id} assigned to {Display.AssigneeName(item, Users)}.");
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
