using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record CustomerInfoModel : BaseNopModel
{
    public CustomerAvatarModel CustomerAvatarModel { get; set; }
    public Core.Domain.Customers.Customer Customer { get; set; }
}