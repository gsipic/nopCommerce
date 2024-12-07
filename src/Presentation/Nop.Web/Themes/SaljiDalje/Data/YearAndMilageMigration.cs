using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations;

[NopSchemaMigration("2024-11-28 00:00:00", "Category. Add Year and Mileage")]
public class AddSomeNewProperty : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var categoryTableName = nameof(Product);
        if (!Schema.Table(categoryTableName).Column(nameof(Product.CostumeYear)).Exists())
            Alter.Table(categoryTableName)
                .AddColumn(nameof(Product.CostumeYear)).AsInt32().SetExistingRowsTo(0);

        if (!Schema.Table(categoryTableName).Column(nameof(Product.CostumeMileage)).Exists())
            Alter.Table(categoryTableName)
                .AddColumn(nameof(Product.CostumeMileage)).AsInt32().SetExistingRowsTo(0);
    }
}