using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace Nop.Web.Themes.SaljiDalje.Data


{
    /// <summary>
    /// Represents a address attribute entity builder
    /// </summary>
    public partial class CostumerPictureAttachmentBuilder : NopEntityBuilder<CostumerPictureAttachmentMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CostumerPictureAttachmentMapping.UserId))
                .AsInt32().ForeignKey<Customer>(onDelete: Rule.Cascade);
        }

        #endregion
    }
}