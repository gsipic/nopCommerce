using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public class CostumeProductService(
    CatalogSettings catalogSettings,
    CommonSettings commonSettings,
    IAclService aclService,
    ICustomerService customerService,
    IDateRangeService dateRangeService,
    ILanguageService languageService,
    ILocalizationService localizationService,
    IProductAttributeParser productAttributeParser,
    IProductAttributeService productAttributeService,
    IRepository<Category> categoryRepository,
    IRepository<CrossSellProduct> crossSellProductRepository,
    IRepository<DiscountProductMapping> discountProductMappingRepository,
    IRepository<LocalizedProperty> localizedPropertyRepository,
    IRepository<Manufacturer> manufacturerRepository,
    IRepository<Product> productRepository,
    IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
    IRepository<ProductAttributeMapping> productAttributeMappingRepository,
    IRepository<ProductCategory> productCategoryRepository,
    IRepository<ProductManufacturer> productManufacturerRepository,
    IRepository<ProductPicture> productPictureRepository,
    IRepository<ProductProductTagMapping> productTagMappingRepository,
    IRepository<ProductReview> productReviewRepository,
    IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
    IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
    IRepository<ProductTag> productTagRepository,
    IRepository<ProductVideo> productVideoRepository,
    IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
    IRepository<RelatedProduct> relatedProductRepository,
    IRepository<Shipment> shipmentRepository,
    IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
    IRepository<TierPrice> tierPriceRepository,
    ISearchPluginManager searchPluginManager,
    IStaticCacheManager staticCacheManager,
    IStoreService storeService,
    IStoreMappingService storeMappingService,
    IWorkContext workContext,
    LocalizationSettings localizationSettings)
    : ProductService(catalogSettings, commonSettings, aclService, customerService, dateRangeService, languageService,
        localizationService, productAttributeParser, productAttributeService, categoryRepository,
        crossSellProductRepository, discountProductMappingRepository, localizedPropertyRepository,
        manufacturerRepository, productRepository, productAttributeCombinationRepository,
        productAttributeMappingRepository, productCategoryRepository, productManufacturerRepository,
        productPictureRepository, productTagMappingRepository, productReviewRepository,
        productReviewHelpfulnessRepository, productSpecificationAttributeRepository, productTagRepository,
        productVideoRepository, productWarehouseInventoryRepository, relatedProductRepository, shipmentRepository,
        stockQuantityHistoryRepository, tierPriceRepository, searchPluginManager, staticCacheManager, storeService,
        storeMappingService, workContext, localizationSettings)
{
    public async Task<IEnumerable<Product>> GetProductsByVendorId(int vendorId)
    {
        var items = await _productRepository.Table
            .Where(p => p.VendorId == vendorId && !p.Deleted).ToListAsync();
        return items;
    }
}