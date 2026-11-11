using WorkItemTracker.Models;


namespace WorkItemTracker.Services;

// Owns the list of work items and the rules that need more than one item or the users (R3, R4, Q1-Q3).
// Singleton: there is exactly one work items list, reached through WorkItemService.Instance.
public class WorkItemService
{
    // Work items by Id, so looking one up is a direct hit instead of a scan.
    private readonly Dictionary<int, WorkItem> _items = new();

    // // we dont need only one instance for user, we need to call other differences
    private readonly UserService _userService;
    private int _nextId = 1;


    public WorkItemService(UserService userService)
    {
        _userService = userService;
    }
    // static: ensures there’s only one copy shared across the program.
    // public static WorkItemService Instance { get; } = new WorkItemService();

    // R3: create a ToDo item with the next Id. The assignee, if given, must be an existing user.
    public WorkItem Create(string title, Priority priority, int? assigneeId)
    {
        if (assigneeId is not null)
        {
            _userService.GetRequired(assigneeId.Value);
        }

        // The constructor validates and trims the title.
        var item = new WorkItem(_nextId, title, priority, assigneeId);

        // The Id is only used up once the item is really added (R7).
        _items.Add(item.Id, item);
        _nextId++;
        return item;
    }

    // Q1, Q2: all items, by status, or by assignee. The Program then picks the ordering.
    public IReadOnlyList<WorkItem> GetAll() => _items.Values.ToList();

    // Priority (default): priority, then Id
    public IReadOnlyList<WorkItem> OrderByPriority(IEnumerable<WorkItem> items) =>
        OrderByPriorityThenId(items);

    // Newest first by created-on date; items created at the same time go highest Id first.
    public IReadOnlyList<WorkItem> OrderByNewestFirst(IEnumerable<WorkItem> items) =>
        items.OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Id).ToList();

    // Title: A to Z ignoring case, then Id
    // dont forget the ignore case
    // strategy pattern 
    public IReadOnlyList<WorkItem> OrderByTitle(IEnumerable<WorkItem> items) =>
        items.OrderBy(i => i.Title, StringComparer.OrdinalIgnoreCase).ThenBy(i => i.Id).ToList();

    public IReadOnlyList<WorkItem> ListByStatus(WorkItemStatus status) =>
        _items.Values.Where(i => i.Status == status).ToList();

    public IReadOnlyList<WorkItem> ListByAssignee(int userId)
    {
        // An Id that doesn't exist is rejected, rather than returning an empty list.
        _userService.GetRequired(userId);

        return _items.Values.Where(i => i.AssigneeId == userId).ToList();
    }

    // R4: assign to an existing user. WorkItem.AssignTo rejects a Done item.
    public WorkItem Assign(int workItemId, int userId)
    {
        var item = GetRequired(workItemId);
        _userService.GetRequired(userId);

        item.AssignTo(userId);
        return item;
    }

    // The assignee's name for a work item, or "Unassigned" if it has none.
    // Lives here (not on WorkItem) because only the services know the users.
    public string GetAssigneeName(int workItemId)
    {
        var item = GetRequired(workItemId);

        return item.AssigneeId is int userId
            ? _userService.GetName(userId)
            : "-";
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
