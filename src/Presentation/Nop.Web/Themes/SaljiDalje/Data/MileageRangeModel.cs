using Nop.Web.Framework.Models;

namespace Nop.Web.Themes.SaljiDalje.Data;

public record MileageRangeModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the "from" mileage
    /// </summary>
    public int? From { get; set; }

    /// <summary>
    /// Gets or sets the "to" mileage
    /// </summary>
    public int? To { get; set; }

    #endregion
}