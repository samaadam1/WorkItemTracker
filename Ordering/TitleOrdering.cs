using WorkItemTracker.Models;

namespace WorkItemTracker.Ordering;

// Title: A to Z ignoring case, then Id.
public class TitleOrdering : IWorkItemOrdering
{
    public string Name => "Title A-Z";

    public IReadOnlyList<WorkItem> Order(IEnumerable<WorkItem> items) =>
        items.OrderBy(i => i.Title, StringComparer.OrdinalIgnoreCase).ThenBy(i => i.Id).ToList();
}
