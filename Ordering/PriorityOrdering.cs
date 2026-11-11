using WorkItemTracker.Models;

namespace WorkItemTracker.Ordering;

// Q1-Q3: order by priority, then Id.
// OrderBy is stable, so the ThenBy keeps the priority order when Ids are compared.
public class PriorityOrdering : IWorkItemOrdering
{
    public string Name => "Priority";

    public IReadOnlyList<WorkItem> Order(IEnumerable<WorkItem> items) =>
        items.OrderBy(i => i.Priority).ThenBy(i => i.Id).ToList();
}
