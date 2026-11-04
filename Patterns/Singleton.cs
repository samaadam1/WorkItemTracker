namespace WorkItemTracker.Patterns;

// Singleton pattern: a class that has exactly one instance, shared by the whole app.
//
// Usage: class UserService : Singleton<UserService> { private UserService() { } }
// then use UserService.Instance everywhere.
//
// The subclass must keep its constructor private, otherwise anyone could still write "new UserService()".
public abstract class Singleton<T> where T : class
{
    // "static": one field for the whole class, not one per object. This is what makes the instance shared.
    // "readonly": the field can't be pointed at a different object later.
    //
    // Lazy<T> is a built-in .NET type. "Lazy" means the object is not created when the app starts,
    // only the first time someone asks for it. It also makes sure that happens once, even with several threads.
    //
    // The code inside "() => ..." is what Lazy runs to create the object:
    //  - typeof(T) is the subclass, for example UserService.
    //  - Activator.CreateInstance builds an object of that type by calling its constructor.
    //  - nonPublic: true lets it use the private constructor. "new T()" couldn't, because it needs a public one.
    //  - (T) converts the result to the subclass type.
    //  - "!" tells the compiler the result is never null.
    private static readonly Lazy<T> _instance =
        new(() => (T)Activator.CreateInstance(typeof(T), nonPublic: true)!);

    // The only way to get the object. The first read creates it, and every later read returns the same one.
    public static T Instance => _instance.Value;
}
