using WorkItemTracker.Models;
using WorkItemTracker.Services;

namespace WorkItemTracker.Data;

// C5: sample data loaded when the app starts, so it can be tried immediately.
// It goes through the services, so the sample items follow the same rules as real ones.
public static class SampleData
{
    public static void Load()
    {
        var users = UserService.Instance;
        var workItems = WorkItemService.Instance;

        var sara = users.Add("Sara");
        var adel = users.Add("Adel");
        users.Add("Lina"); // no work items assigned to her

        // Done: it has to go ToDo -> InProgress -> Done, one step at a time (R5), and needs an assignee (R6).
        var homepage = workItems.Create("Design homepage", Priority.High, sara.Id);
        workItems.ChangeStatus(homepage.Id, WorkItemStatus.InProgress);
        workItems.ChangeStatus(homepage.Id, WorkItemStatus.Done);

        // InProgress
        var loginBug = workItems.Create("Fix login bug", Priority.High, adel.Id);
        workItems.ChangeStatus(loginBug.Id, WorkItemStatus.InProgress);

        // ToDo, unassigned
        workItems.Create("Write API docs", Priority.Low, null);

        // ToDo, assigned
        workItems.Create("Set up CI pipeline", Priority.Medium, adel.Id);
        workItems.Create("Review pull requests", Priority.Low, sara.Id);
    }
}
