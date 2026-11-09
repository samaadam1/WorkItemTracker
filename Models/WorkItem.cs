namespace WorkItemTracker.Models;

public class WorkItem
{
    public const int MaxTitleLength = 100;

    // Throws DomainException if the title is empty or too long, so an invalid WorkItem can't exist.
    public WorkItem(int id, string title, Priority priority, int? assigneeId = null)
    {
        title = (title ?? "").Trim();

        if (title.Length == 0)
        {
            throw new DomainException("Title is required.");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new DomainException($"Title must be at most {MaxTitleLength} characters.");
        }

        Id = id;
        Title = title;
        Priority = priority;
        AssigneeId = assigneeId;
        Status = WorkItemStatus.ToDo;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; }

    public string Title { get; }

    public Priority Priority { get; }

    // Readable by anyone, changed only through MoveToStatus().
    public WorkItemStatus Status { get; private set; }

    // null means unassigned. Stores the user's Id, not the User object.
    // Readable by anyone, changed only through AssignTo().
    public int? AssigneeId { get; private set; }

    public DateTime CreatedAt { get; }

    // R4: assigning again replaces the previous assignee, but a Done item can't be assigned.
    // Checking that the user exists is the caller's job, because this class doesn't know the users.
    public void AssignTo(int userId)
    {
        if (Status == WorkItemStatus.Done)
        {
            throw new DomainException($"Work item {Id} is Done and can't be assigned.");
        }

        AssigneeId = userId;
    }

    // R5: status only moves forward, one step at a time.
    // R6: moving to InProgress requires an assignee.
    public void MoveToStatus(WorkItemStatus newStatus)
    {   // switch case is better for complex rules.

        if (newStatus != Status + 1)
        {
            throw new DomainException($"Cannot change status from {Status} to {newStatus}.");
        }

        if (newStatus == WorkItemStatus.InProgress && AssigneeId is null)
        {
            throw new DomainException($"Work item {Id} needs an assignee before it can be InProgress.");
        }

        Status = newStatus;
    }
}
