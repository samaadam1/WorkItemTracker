namespace WorkItemTracker.Models;

// Thrown when a business rule rejects an operation (R7).
// The message is written for the user, so the console can print it as is.
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
