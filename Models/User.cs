namespace WorkItemTracker.Models;

public class User
{
    public const int MaxNameLength = 50;

    // Throws DomainException if the name is empty or too long, so an invalid User can't exist.
    // Name uniqueness (R2) is checked by the service, because one User doesn't know the others.
    public User(int id, string name)
    {
        name = (name ?? "").Trim();

        if (name.Length == 0)
        {
            throw new DomainException("Name is required.");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException($"Name must be at most {MaxNameLength} characters.");
        }

        // Letters (from any language) and spaces only, so "1" or "###" is not a name.
        if (!name.All(c => char.IsLetter(c) || c == ' '))
        {
            throw new DomainException("Name can only contain letters and spaces.");
        }

        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }
}
