namespace WorkItemTracker.Models;

// Declared in workflow order: ToDo -> InProgress -> Done.
public enum WorkItemStatus
{
    ToDo = 1,
    InProgress = 2,
    Done
}
