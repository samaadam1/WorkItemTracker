using WorkItemTracker.Models;

namespace WorkItemTracker.Services;

// Owns the list of users and the rules that need to see all of them (R1, R2).
// Singleton: there is exactly one users list, reached through UserService.Instance.
public class UserService 
{
    // Private, so nobody can create a second users list with "new UserService()".
    // check how to use singleton without other class.
    public UserService()
    {
        // private constructor, so that only this class can create an instance of itself
    }

    // public static UserService Instance { get; } = new UserService();
    

    // Users by Id, so looking one up is a direct hit instead of a scan.
    private readonly Dictionary<int, User> _users = new();

    // The names already taken. The comparer makes every check ignore case ("Sara" == "sara").
    private readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);
    private int _nextId = 1;

    // R1, R2: add a user with the next Id, rejecting a name that already exists (ignoring case).
    public User AddUser(string name)
    {
        // The constructor validates and trims the name, so the comparison below uses the final name.
        var user = new User(_nextId, name);

        // HashSet.Add returns false when the name is already there, so this checks and reserves in one step.
        if (!_names.Add(user.Name))
        {
            throw new DomainException($"A user named '{user.Name}' already exists.");
        }

        // The Id is only used up once the user is really added, so a rejected name changes nothing (R7).
        _users.Add(user.Id, user);
        _nextId++;
        return user;
    }

    public IReadOnlyList<User> GetAll() => _users.Values.OrderBy(u => u.Id).ToList();

    public User? Find(int id) => _users.GetValueOrDefault(id);

    // Same as Find, but rejects an Id that doesn't exist.
    public User GetRequired(int id) =>
        Find(id) ?? throw new DomainException($"User {id} does not exist.");

    // The name of an existing user; rejects an Id that doesn't exist.
    public string GetName(int id) => GetRequired(id).Name;
}
