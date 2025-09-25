using CodingJournal.Application.Features.Categories.Actions;
using CodingJournal.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodingJournal.API.Controllers;

[ApiController]
[Route("categories")]
[Authorize]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var result = await mediator.Send(new GetCategoriesQuery(page, pageSize, searchTerm));
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) 
            : BadRequest(new { errors = result.Errors, message = "Failed to get category."});
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand command)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value }, new { id = result.Value }) 
            : BadRequest(new { errors = result.Errors, message = "Failed to create category."});
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand(id, request.Name);
        var result = await mediator.Send(command);
        return result.IsSuccess ? NoContent() 
            : BadRequest(new { errors = result.Errors, message = "Failed to update category."});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(id));
        return result.IsSuccess ? NoContent() 
            : BadRequest(new { errors = result.Errors, message = "Failed to delete category."});
    }
}