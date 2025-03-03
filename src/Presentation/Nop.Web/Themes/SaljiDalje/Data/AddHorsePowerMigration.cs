using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Migrations;

namespace Nop.Web.Themes.SaljiDalje.Data;

[NopMigration("2025-03-03 07:15:00", "AddHorsePowerMigration", MigrationProcessType.Update)]
public class AddHorsePowerMigration : MigrationBase
{
    public override void Up()
    {
        var categoryTableName = nameof(Product);
        if (!Schema.Table(categoryTableName).Column(nameof(Product.CostumeHorsePower)).Exists())
            Alter.Table(categoryTableName)
                .AddColumn(nameof(Product.CostumeHorsePower)).AsInt32().SetExistingRowsTo(0);
    }
    
    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}