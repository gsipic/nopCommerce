using System.Collections;
using Nop.Web.Framework.Models;

namespace Nop.Web.Themes.SaljiDalje.Data;

public record YearRangeFilterModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the selected price range
    /// </summary>
    public YearRangeModel SelectedYearRange { get; set; }

    /// <summary>
    /// Gets or sets the available price range
    /// </summary>
    public IEnumerable<int> AvailableYearRange { get; set; }

    #endregion

    public YearRangeFilterModel()
    {
        SelectedYearRange = new YearRangeModel();
        //AvailableYearRange = new YearRangeModel();
    }
}