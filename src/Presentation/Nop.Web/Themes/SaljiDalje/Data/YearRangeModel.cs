using Nop.Web.Framework.Models;

namespace Nop.Web.Themes.SaljiDalje.Data;

public record YearRangeModel: BaseNopModel
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