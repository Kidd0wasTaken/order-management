using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using OrderManagement.Api.DTOs;
using OrderManagement.Api.Mapping;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(orders.Select(o => o.ToResponseDto()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            return NotFound(new { message = $"Comanda cu ID {id} nu a fost găsită." });
        }

        return Ok(order.ToResponseDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponseDto>> Create(
        [FromBody] OrderUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var order = dto.ToEntity();
        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);

        var response = order.ToResponseDto();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> Update(
        int id,
        [FromBody] OrderUpsertDto dto,
        CancellationToken cancellationToken)
    {
        var order = await db.Orders.FindAsync([id], cancellationToken);
        if (order is null)
        {
            return NotFound(new { message = $"Comanda cu ID {id} nu a fost găsită." });
        }

        order.ApplyUpdate(dto);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(order.ToResponseDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var order = await db.Orders.FindAsync([id], cancellationToken);
        if (order is null)
        {
            return NotFound(new { message = $"Comanda cu ID {id} nu a fost găsită." });
        }

        db.Orders.Remove(order);
        await db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
