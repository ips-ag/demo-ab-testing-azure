using Core.Entities;
using Infrastructure.Data.Specifications;

namespace API.Specifications
{
    public class ProductCountSpecification(int? productBrandId, int? productTypeId, string search) : BaseSpecification<Product>(
            x =>
                    (string.IsNullOrEmpty(search) || x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)) &&
                    (!productBrandId.HasValue || x.ProductBrandId == productBrandId.Value) &&
                    (!productTypeId.HasValue || x.ProductTypeId == productTypeId.Value)
            )
    {
    }
}