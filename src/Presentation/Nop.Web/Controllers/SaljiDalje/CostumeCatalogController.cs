using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public class CostumeCatalogController(
    CatalogSettings catalogSettings,
    IAclService aclService,
    ICatalogModelFactory catalogModelFactory,
    ICategoryService categoryService,
    ICustomerActivityService customerActivityService,
    IGenericAttributeService genericAttributeService,
    ILocalizationService localizationService,
    IManufacturerService manufacturerService,
    INopUrlHelper nopUrlHelper,
    IPermissionService permissionService,
    IProductModelFactory productModelFactory,
    IProductService productService,
    IProductTagService productTagService,
    IStoreContext storeContext,
    IStoreMappingService storeMappingService,
    IUrlRecordService urlRecordService,
    IVendorService vendorService,
    IWebHelper webHelper,
    IWorkContext workContext,
    MediaSettings mediaSettings,
    VendorSettings vendorSettings)
    : CatalogController(catalogSettings, aclService, catalogModelFactory, categoryService, customerActivityService,
        genericAttributeService, localizationService, manufacturerService, nopUrlHelper, permissionService,
        productModelFactory, productService, productTagService, storeContext, storeMappingService, urlRecordService,
        vendorService, webHelper, workContext, mediaSettings, vendorSettings)
{
    //ignore SEO friendly URLs checks
    [CheckLanguageSeoCode(ignore: true)]
    public override async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);

        if (!await CheckCategoryAvailabilityAsync(category))
            return NotFound();

        var model = await _catalogModelFactory.PrepareCategoryProductsModelAsync(category, command);

        return PartialView("/Themes/SaljiDalje/Views/Catalog/_ProductsInGridOrLines.cshtml", model);
    }
}