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


    [HttpGet("id:{id:int}")]
    [ProducesResponseType<ProductResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
      var product = await service.GetByIdAsync(id);
      return product is null ? NotFound() : Ok(product);
    
    }

    [HttpGet("name:{name}")]
    [ProducesResponseType<IEnumerable<ProductResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
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
        [FromBody] CreateProductDto dto,
        CancellationToken ct)
    {

      if (!ModelState.IsValid)
        return BadRequest();

      var created = await service.CreateAsync(dto);
      
      return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
    }



  }
}

