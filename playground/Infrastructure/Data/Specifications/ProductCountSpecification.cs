using Core.Entities;
using Infrastructure.Data.Specifications;

namespace API.Specifications
{
    public class ProductCountSpecification(int[]? productBrandIds, int? productTypeId, string search) : BaseSpecification<Product>(
            x =>
                    (string.IsNullOrEmpty(search) || x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)) &&
                    (productBrandIds == null || productBrandIds!.Contains(x.ProductBrandId)) &&
                    (!productTypeId.HasValue || x.ProductTypeId == productTypeId.Value)
            )
    {
    }
}