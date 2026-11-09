using WorkItemTracker.Models;


namespace WorkItemTracker.Services;

// Owns the list of work items and the rules that need more than one item or the users (R3, R4, Q1-Q3).
// Singleton: there is exactly one work items list, reached through WorkItemService.Instance.
public class WorkItemService
{
    // Work items by Id, so looking one up is a direct hit instead of a scan.
    private readonly Dictionary<int, WorkItem> _items = new();

    // we dont need only one instance for user, we need to call other differences
    private readonly UserService _users = UserService.Instance;
    private int _nextId = 1;

    
    private WorkItemService()
    {
        // Private, so nobody can create a second work items list with "new WorkItemService()".
    }
    // static: ensures there’s only one copy shared across the program.
    public static WorkItemService Instance { get; } = new WorkItemService();

    // R3: create a ToDo item with the next Id. The assignee, if given, must be an existing user.
    public WorkItem Create(string title, Priority priority, int? assigneeId)
    {
        if (assigneeId is not null)
        {
            _users.GetRequired(assigneeId.Value);
        }

        // The constructor validates and trims the title.
        var item = new WorkItem(_nextId, title, priority, assigneeId);

        // The Id is only used up once the item is really added (R7).
        _items.Add(item.Id, item);
        _nextId++;
        return item;
    }

    // Q1, Q2: all items, by status, or by assignee. Always ordered by priority, then Id.
    public IReadOnlyList<WorkItem> ListAll() => OrderByPriorityThenId(_items.Values);

    public IReadOnlyList<WorkItem> ListByStatus(WorkItemStatus status) =>
        OrderByPriorityThenId(_items.Values.Where(i => i.Status == status));

    public IReadOnlyList<WorkItem> ListByAssignee(int userId)
    {
        // An Id that doesn't exist is rejected, rather than returning an empty list.
        _users.GetRequired(userId);

        return OrderByPriorityThenId(_items.Values.Where(i => i.AssigneeId == userId));
    }

    // R4: assign to an existing user. WorkItem.AssignTo rejects a Done item.
    public WorkItem Assign(int workItemId, int userId)
    {
        var item = GetRequired(workItemId);
        _users.GetRequired(userId);

        item.AssignTo(userId);
        return item;
    }

    // R5, R6: WorkItem.MoveToStatus enforces the transition rules.
    public WorkItem ChangeStatus(int workItemId, WorkItemStatus newStatus)
    {
        var item = GetRequired(workItemId);

        item.MoveToStatus(newStatus);
        return item;
    }

    // Q3: a count for every status, including the ones with 0 items.
    public IReadOnlyDictionary<WorkItemStatus, int> GetSummary() =>
        Enum.GetValues<WorkItemStatus>()
            .ToDictionary(status => status, status => _items.Values.Count(i => i.Status == status));

    public WorkItem? Find(int id) => _items.GetValueOrDefault(id);

    private WorkItem GetRequired(int id) =>
        Find(id) ?? throw new DomainException($"Work item {id} does not exist.");

    // Q1-Q3: order by priority, then Id. The caller can filter first, so this is private.
    // IEnum.OrderBy is stable, so the ThenBy preserves the priority order when Ids are compared.
    // IEnumerable<WorkItem> is a sequence you can read through. It has no Count or index, and you can't add to it.    
    private static IReadOnlyList<WorkItem> OrderByPriorityThenId(IEnumerable<WorkItem> items) =>
        items.OrderBy(i => i.Priority).ThenBy(i => i.Id).ToList();
}
