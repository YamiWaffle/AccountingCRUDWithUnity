using AccountingApp.Application.AccountEntries.Commands.CreateEntry;
using AccountingApp.Application.AccountEntries.Commands.DeleteEntry;
using AccountingApp.Application.AccountEntries.Commands.UpdateEntry;
using AccountingApp.Application.AccountEntries.Queries.GetEntries;
using AccountingApp.Application.AccountEntries.Queries.GetEntryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountEntriesController : ControllerBase
{
    private readonly ISender _mediator;

    public AccountEntriesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountEntryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entries = await _mediator.Send(new GetEntriesQuery(), cancellationToken);
        return Ok(entries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountEntryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var entry = await _mediator.Send(new GetEntryByIdQuery(id), cancellationToken);
            return Ok(entry);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEntryCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEntryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID in URL must match ID in body.");

        try
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteEntryCommand(id), cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
