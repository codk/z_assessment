using Microsoft.AspNetCore.Mvc;
using Products.DTO;
using Products.interfaces;

namespace Products.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Produces("application/json")]
  public class ProductsController(IProductService service) : ControllerBase
  {
    [HttpGet]
    [ProducesResponseType<IEnumerable<ProductResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
      var products = await service.GetAllAsync();
      return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
      var product = await service.GetByIdAsync(id);
      return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    [ProducesResponseType<IEnumerable<ProductResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
      var products = await service.GetByNameAsync(name);
      return Ok(products);
    }

    [HttpGet("stock-level")]
    [ProducesResponseType<IEnumerable<ProductResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStockLevel([FromQuery] int min, [FromQuery] int max)
    {
      var products = await service.GetByStockAsync(min, max);
      return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductDto dto)
    {
      if (!ModelState.IsValid)
        return BadRequest();

      var created = await service.CreateAsync(dto);

      return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
      int id,
      [FromBody] UpdateProductDto dto)
    {
      if (!ModelState.IsValid)
        return BadRequest();

      var updated = await service.UpdateAsync(id, dto);
      return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id}/add-to-stock/{quantity}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToStock(
      int id,
      int quantity)
    {
      if (quantity <= 0)
        return BadRequest(new { error = "Quantity must be greater than zero." });

      var added = await service.StockIncrementAsync(id, quantity);

      return added ? Ok() : BadRequest(new { error = "Failed to add stock." });
    }

    [HttpPost("{id}/decrement-stock/{quantity}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DecrementStock(
  int id,
  int quantity)
    {
      if (quantity <= 0)
        return BadRequest(new { error = "Quantity must be greater than zero." });

      var added = await service.StockDecrementAsync(id, quantity);

      return added ? Ok() : BadRequest(new { error = "Failed to decrement stock." });
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
      var deleted = await service.DeleteAsync(id);
      return deleted ? Ok() : NotFound();
    }
  }
}