using System.Security.Claims;
using CodingJournal.Application.Documents.Actions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodingJournal.API.Controllers;

[ApiController]
[Route("documents")]
public class DocumentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var result = await mediator.Send(new GetDocumentsQuery(userId, page, pageSize, searchTerm, categoryId));
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        var result = await mediator.Send(new GetDocumentByIdQuery(userId, id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { errors = result.Errors, message = "Failed to get document."}); 
    }
}