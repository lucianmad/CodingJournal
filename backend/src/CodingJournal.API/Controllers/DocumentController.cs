using System.Security.Claims;
using CodingJournal.Application.Features.Documents.Actions;
using CodingJournal.Application.Features.Documents.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodingJournal.API.Controllers;

[ApiController]
[Route("documents")]
[Authorize]
public class DocumentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null)
    {
        var result = await mediator.Send(new GetDocumentsQuery(page, pageSize, searchTerm, categoryId));
        return Ok(result.Value);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new GetDocumentByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) 
            : BadRequest(new { errors = result.Errors, message = "Failed to get document."}); 
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateDocumentCommand command)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value }, new { id = result.Value }) 
            : BadRequest(new { errors = result.Errors, message = "Failed to create document."}); 
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDocumentRequest request)
    {
        var command = new UpdateDocumentCommand(id, request.Title, request.Content, request.CategoryId);
        var result = await mediator.Send(command);
        return result.IsSuccess ? NoContent() 
            : BadRequest(new { errors = result.Errors, message = "Failed to update document."}); 
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteDocumentCommand(id));
        return result.IsSuccess ? NoContent() 
            : BadRequest(new { errors = result.Errors, message = "Failed to delete document."}); 
    }
}