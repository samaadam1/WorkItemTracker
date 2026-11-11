using WorkItemTracker.Models;

namespace WorkItemTracker.Ordering;

// Strategy pattern: each way of ordering work items is its own class behind this interface.
// To add a new ordering, add a class that implements it and add one line to the list in Program.cs.
public interface IWorkItemOrdering
{
    // Shown in the order menu.
    string Name { get; }

    // IEnumerable<WorkItem> is a sequence you can read through. The caller can filter first.
    IReadOnlyList<WorkItem> Order(IEnumerable<WorkItem> items);
}
