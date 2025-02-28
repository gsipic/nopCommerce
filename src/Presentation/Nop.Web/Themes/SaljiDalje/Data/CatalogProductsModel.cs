using BlazorApp1.Pages;
using Nop.Core.Domain.Catalog;
using Nop.Web.Themes.SaljiDalje.Data;

namespace Nop.Web.Models.Catalog;

public partial record CatalogProductsModel 
{
    public YearRangeFilterModel YearRangeFilter { get; set; }
    public MileageRangeModel MileageRangeModel { get; set; }
    
    public IList<SpecificationOption> Make { get; set; }
    public IList<SpecificationOption> Model { get; set; }
    
    public string ChildCategory { get; set; }
}