using Nop.Web.Themes.SaljiDalje.Data;

namespace Nop.Web.Models.Catalog;

public partial record CatalogProductsModel 
{
    public YearRangeFilterModel YearRangeFilter { get; set; }
    public MileageRangeModel MileageRangeModel { get; set; }
}