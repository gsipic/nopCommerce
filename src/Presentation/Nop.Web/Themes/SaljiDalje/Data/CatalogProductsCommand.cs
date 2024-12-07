namespace Nop.Web.Models.Catalog;

public partial record CatalogProductsCommand
{
    /// <summary>
    /// Gets or sets the year ('min-max' format)
    /// </summary>
    public string Year { get; set; }
    
    /// <summary>
    /// Gets or sets the milage ('min-max' format)
    /// </summary>
    public string Mileage { get; set; }
}