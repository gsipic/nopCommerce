using Nop.Web.Framework.Models;

namespace Nop.Web.Themes.SaljiDalje.Data;

public record HorsePowerFilterModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the selected price range
    /// </summary>
    public HorsePowerRangeModel SelectedHorsePowerRange { get; set; }

    /// <summary>
    /// Gets or sets the available price range
    /// </summary>
    public IEnumerable<int> AvailableHorsePowerRange { get; set; }

    #endregion

    public HorsePowerFilterModel()
    {
        //HorsePowerRangeModel = new HorsePowerRangeModel();
    }
}

public record HorsePowerRangeModel: BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the "from" price
    /// </summary>
    public int? From { get; set; }

    /// <summary>
    /// Gets or sets the "to" price
    /// </summary>
    public int? To { get; set; }

    #endregion
}