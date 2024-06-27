using API.DTOs;
using API.Helpers;
using API.Specifications;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController(
        IRepository<Product> productRepository,
        IRepository<ProductBrand> productBrandRepository,
        IRepository<ProductType> productTypeRepository, IMapper mapper) : ControllerBase
    {

        // GET: api/v1/Products
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<Pagination<ProductDTO>>> GetProducts([FromQuery] ProductParams productParams)
        {
            // Extract the parameters from the productParams object
            var sort = productParams.Sort;
            var productTypeId = productParams.ProductTypeId;
            var productBrandIds = productParams.ProductBrandIds;
            var skip = (productParams.PageIndex - 1) * productParams.PageSize; // Calculate skip based on PageIndex and PageSize
            var take = productParams.PageSize; // Use PageSize for take
            var search = productParams.Search;

            // Create a specification for counting products
            var countSpec = new ProductCountSpecification(productBrandIds, productTypeId, search);

            // Use the specification with the repository to get the total count of products
            var totalCount = await productRepository.CountAsync(countSpec);

            // If totalCount is 0, return an empty result immediately
            if (totalCount == 0)
            {
                return Ok(new Pagination<ProductDTO>(productParams.PageIndex, productParams.PageSize, 0, []));
            }

            // Create a specification for fetching paginated products
            var spec = new ProductWithTypesAndBrandSpecification(sort, productTypeId, productBrandIds, skip, take, search);

            // Use the specification with the repository to get filtered and included results
            var products = await productRepository.ListAsync(spec);

            var productDTOs = mapper.Map<List<ProductDTO>>(products);

            // Create a Pagination object to return both product data and total count
            var pagination = new Pagination<ProductDTO>(productParams.PageIndex, productParams.PageSize, totalCount, productDTOs);

            return Ok(pagination);
        }


        // GET: api/v1/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Create a specification
            var spec = new ProductWithTypesAndBrandSpecification(id);

            // Use the specification with the repository to get filtered and included results
            var retrievedProduct = await productRepository.GetByIdAsync(spec);
            var product = mapper.Map<ProductDTO>(retrievedProduct);
            return Ok(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetProductBrands()
        {
            return Ok(await productBrandRepository.GetProductBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
        {
            return Ok(await productTypeRepository.GetProductTypesAsync());
        }
    }
}
