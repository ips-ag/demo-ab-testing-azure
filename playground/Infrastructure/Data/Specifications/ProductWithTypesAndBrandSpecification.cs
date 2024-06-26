using Core.Entities;

namespace Infrastructure.Data.Specifications;

public class ProductWithTypesAndBrandSpecification : BaseSpecification<Product>
{
    public ProductWithTypesAndBrandSpecification(string sort, int? productTypeId, int[]? productBrandIds, int skip, int take, string search)
    {
        AddInclude(p => p.ProductType);
        AddInclude(p => p.ProductBrand);
        // Default sorting by name in ascending order
        ApplyOrderBy(p => p.Name, Core.Enums.OrderBy.Ascending);

        // Parse and apply sorting based on the 'sort' parameter
        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort.ToLower())
            {
                case "priceasc":
                    ApplyOrderBy(p => p.Price, Core.Enums.OrderBy.Ascending);
                    break;
                case "pricedesc":
                    ApplyOrderBy(p => p.Price, Core.Enums.OrderBy.Descending);
                    break;
                case "namedesc":
                    ApplyOrderBy(p => p.Name, Core.Enums.OrderBy.Descending);
                    break;
                // Add more sorting options as needed
                default:
                    ApplyOrderBy(p => p.Name, Core.Enums.OrderBy.Ascending);
                    break;
            }
        }

        // Apply filtering based on product type And product brand
        if (productBrandIds != null || productTypeId.HasValue)
        {
            // Combine the conditions using And operator
            ApplyCriteria(p =>
                (productBrandIds == null || productBrandIds!.Contains(p.ProductBrandId)) &&
                (!productTypeId.HasValue || p.ProductTypeId == productTypeId.Value));
        }

        // Apply Paging
        if (skip >= 0 && take > 0)
        {
            ApplyPaging(skip, take);
        }

        // Apply search criteria
        if (!string.IsNullOrEmpty(search))
        {
            ApplyCriteria(p => p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    public ProductWithTypesAndBrandSpecification(int id) : base(product => product.Id == id)
    {
        AddInclude(p => p.ProductType);
        AddInclude(p => p.ProductBrand);
    }
}