using WorkItemTracker.Models;

namespace WorkItemTracker.Ordering;

// Newest first by created-on date; items created at the same time go highest Id first.
public class NewestFirstOrdering : IWorkItemOrdering
{
    public string Name => "Newest first";

    public IReadOnlyList<WorkItem> Order(IEnumerable<WorkItem> items) =>
        items.OrderByDescending(i => i.CreatedAt).ThenByDescending(i => i.Id).ToList();
}
