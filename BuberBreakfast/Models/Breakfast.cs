using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.ServiceErrors;
using ErrorOr;

namespace BuberBreakfast.Models;

public class Breakfast
{
    // Business rules start.
    public const int MinNameLength = 3;
    public const int MaxNameLength = 50;
    public const int MinDescripLength = 50;
    public const int MaxDescripLength = 100;
    // Business rules end.

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public DateTime StartDateTime { get; }
    public DateTime EndDateTime { get; }
    public DateTime LastModifiedDateTime { get; }
    public List<string> Savory { get; }
    public List<string> Sweet { get; }

    public Breakfast(Guid id, string name, string description, DateTime startDateTime,
                     DateTime endDateTime, DateTime lastModifiedDateTime, List<string> savory,
                     List<string> sweet)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        LastModifiedDateTime = lastModifiedDateTime;
        Savory = savory;
        Sweet = sweet;
    }

    public static ErrorOr<Breakfast> Create(
        string name,
        string description,
        DateTime startDateTime,
        DateTime endDateTime,
        List<string> savory,
        List<string> sweet,
        Guid? id = null)
    {
        List<Error> errors = new();
        // Return error for name if it does not meet its criteria.
        if (name.Length is < MinNameLength or > MaxNameLength)
        {
            errors.Add(Errors.Breakfast.InvalidName);
        }

        // Return error for the description if it does not meet its criteria.
        if (description.Length is < MinDescripLength or > MaxDescripLength)
        {
            errors.Add(Errors.Breakfast.InvalidDescription);
        }

        // Return errors if any are found.
        if (errors.Count > 0)
        {
            return errors;
        }

        // Enforce invariants.
        return new Breakfast(
            id ?? Guid.NewGuid(),
            name,
            description,
            startDateTime,
            endDateTime,
            DateTime.UtcNow,
            savory,
            sweet);
    }

    /// <summary>
    /// Calls Create to create the breakfast. The Craete method has the business rules logic.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static ErrorOr<Breakfast> From(CreateBreakfastRequest request)
    {
        return Create(
            request.Name,
            request.Description,
            request.StartDateTime,
            request.EndDateTime,
            request.Savory,
            request.Sweet);
    }

    /// <summary>
    /// Calls Create to create the breakfast if an id is passed in. The Craete method has the business 
    /// rules logic.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static ErrorOr<Breakfast> From(Guid id, UpsertBreakfastRequest request)
    {
        return Create(
            request.Name,
            request.Description,
            request.StartDateTime,
            request.EndDateTime,
            request.Savory,
            request.Sweet,
            id);
    }
}