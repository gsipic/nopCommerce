using Nop.Data.Mapping;

namespace Nop.Web.Themes.SaljiDalje.Data;

public partial class TablesOrColumnsNames : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        { typeof(CostumerPictureAttachmentMapping), "Costumer_PictureAttachment_Mapping" },
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}