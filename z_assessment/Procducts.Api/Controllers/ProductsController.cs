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
  }
}
