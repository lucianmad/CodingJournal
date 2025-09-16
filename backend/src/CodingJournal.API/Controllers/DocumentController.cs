using CodingJournal.Application.Documents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodingJournal.API.Controllers;

[ApiController]
[Route("/documents")]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public DocumentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetDocumentsQuery());
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _mediator.Send(new GetDocumentQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors); 
    }
}