using Core.Interfaces;
using Domin.DTOs;
using Domin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    //[Authorize(Policy = "AdminPolicy")]
    [Route("api/V1.0/[controller]")] // url : localhost:5555/api/V1.0/products
    [ApiController]
    public class ProductsController : ControllerBase
    {
        //[HttpGet]     share data
        // [HttpPost]   add data
        // [HttpDelete] delete data
        // [HttpPut]    update data    (all object)
        // [HttpPatch]  update data    (part of data)  


        private readonly IProductService _productService;
        public ProductsController(IProductService productService) => _productService = productService;


        [HttpGet("GetAll")]  // api/V1.0/Products/getAll
        [Authorize(Policy ="AllPolicy")]
        public async Task<IActionResult> GetAll()
        {
            try
            {

                var products = await _productService.GetAllAsync();
                if (products == null || !products.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No products found 404",
                    }); // 404
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = products
                });
            }
            catch (Exception ex) { 
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }

        [Authorize(Policy = "AllPolicy")]
        [HttpGet("GetById/{id}")]  // api/V1.0/Products/GetById/{id}
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Product not found",
                    });
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }

        [Authorize(Policy = "AllPolicy")]
        [HttpGet("GetProduct/Id=1")]  // api/V1.0/Products/GetProduct/Id=1
        public async Task<IActionResult> GetById1()
        {
            try
            {
                var product = await _productService.GetByIdAsync(1);
                if (product == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Product not found",
                    });
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }
        [Authorize(Policy = "AdminManagerPolicy")]
        [HttpPost("Add")]   // api/V1.0/Products/Add
        public async Task<IActionResult> Add(ProductDTo model)
        {
            try
            {
                if( !ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "ModelStateError",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }
                if(model == null )
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "products can't be null",
                    });
                }
                await _productService.AddAsync(model);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    alert = "Product Added Succesfully!",
                    Data = model
                });
                //return NoContent(); // 204

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }

        }

        [Authorize(Policy = "AdminManagerPolicy")]
        [HttpPut("Update")]  // api/V1.0/Products/Update
        public async Task<IActionResult> Update(ProductDTo model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "ModelStateError",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                await _productService.UpdateAsync(new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    CategoryId = model.CategoryId
                });
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    alert = "Product Updated Succesfully!",
                    Data = model
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("Delete/{id}")]  // api/V1.0/Products/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Product not found",
                    });
                }
                await _productService.DeleteAsync(id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "",
                    alert = "Product Deleted Succesfully!",
                    //Data = product
                });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }
    }
}
