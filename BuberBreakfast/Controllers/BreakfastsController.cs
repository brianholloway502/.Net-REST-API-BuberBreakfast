using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BuberBreakfast.Controllers;

public class BreakfastsController : ApiController
{
    private readonly IBreakfastService _breakfastService;

    public BreakfastsController(IBreakfastService breakfastService)
    {
        _breakfastService = breakfastService;
    }

    /// <summary>
    /// Craetes the breakfast item
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        // Map the request item to the breakfast object.
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

        // Errors encountered with passed in request data.
        if (requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        // We get here there were no errors with the breakfast passed in. Add it to the database.
        var breakfast = requestToBreakfastResult.Value;
        ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

        // Return breakfast or error depending on what occurred here.
        return createBreakfastResult.Match(
            craeted => CreatedAtGetBreakfast(breakfast),
            errors => Problem(errors));
    }

    /// <summary>
    /// Get the breakfast from the passed in breakfast id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

        return getBreakfastResult.Match(
            breakfast => Ok(MapBreakfastResponse(breakfast)),
            errors => Problem(errors));
    }

    /// <summary>
    /// Updates the breakfast with data passed in from the request object and the passed in id identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
    {
        ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(id, request);

        // If error, we are done.
        if (requestToBreakfastResult.IsError)
        {
            return Problem(requestToBreakfastResult.Errors);
        }

        // If we are here, we have an error free breakfast and can proceed.
        var breakfast = requestToBreakfastResult.Value;
        ErrorOr<UpsertedBreakfast> upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

        // Update the breakfast or return the errors.
        return upsertBreakfastResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
            errors => Problem(errors)
        );
    }

    /// <summary>
    /// Deletes the breakfast from the passed in breakfast id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteBreakfast(Guid id)
    {
        ErrorOr<Deleted> deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

        // Deletes the breakfast or returns the errors given.
        return deleteBreakfastResult.Match(
            deleted => NoContent(),
            errors => Problem(errors));
    }

    /// <summary>
    /// Maps breakfast item to the response object.
    /// </summary>
    /// <param name="breakfast"></param>
    /// <returns></returns>
    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        // Map the breakfast to the response object.
        return new BreakfastResponse(
            breakfast.Id,
            breakfast.Name,
            breakfast.Description,
            breakfast.StartDateTime,
            breakfast.EndDateTime,
            breakfast.LastModifiedDateTime,
            breakfast.Savory,
            breakfast.Sweet
        );
    }

    /// <summary>
    /// Adds the breakfast.
    /// </summary>
    /// <param name="breakfast"></param>
    /// <returns></returns>
    private CreatedAtActionResult CreatedAtGetBreakfast(Breakfast breakfast)
    {
        // Add the breakfast with this code.
        return CreatedAtAction(
            actionName: nameof(GetBreakfast),
            routeValues: new { id = breakfast.Id },
            value: MapBreakfastResponse(breakfast));
    }
}