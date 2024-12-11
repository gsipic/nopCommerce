using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Nop.Web.Themes.SaljiDalje.Data;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public class CostumeCatalogModelFactory(
    BlogSettings blogSettings,
    CatalogSettings catalogSettings,
    DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
    ForumSettings forumSettings,
    ICategoryService categoryService,
    ICategoryTemplateService categoryTemplateService,
    ICurrencyService currencyService,
    ICustomerService customerService,
    IEventPublisher eventPublisher,
    IHttpContextAccessor httpContextAccessor,
    IJsonLdModelFactory jsonLdModelFactory,
    ILocalizationService localizationService,
    IManufacturerService manufacturerService,
    IManufacturerTemplateService manufacturerTemplateService,
    INopUrlHelper nopUrlHelper,
    IPictureService pictureService,
    IProductModelFactory productModelFactory,
    IProductService productService,
    IProductTagService productTagService,
    ISearchTermService searchTermService,
    ISpecificationAttributeService specificationAttributeService,
    IStaticCacheManager staticCacheManager,
    IStoreContext storeContext,
    ITopicService topicService,
    IUrlRecordService urlRecordService,
    IVendorService vendorService,
    IWebHelper webHelper,
    IWorkContext workContext,
    MediaSettings mediaSettings,
    SeoSettings seoSettings,
    VendorSettings vendorSettings)
    : CatalogModelFactory(blogSettings, catalogSettings, displayDefaultMenuItemSettings, forumSettings, categoryService,
        categoryTemplateService, currencyService, customerService, eventPublisher, httpContextAccessor,
        jsonLdModelFactory, localizationService, manufacturerService, manufacturerTemplateService, nopUrlHelper,
        pictureService, productModelFactory, productService, productTagService, searchTermService,
        specificationAttributeService, staticCacheManager, storeContext, topicService, urlRecordService, vendorService,
        webHelper, workContext, mediaSettings, seoSettings, vendorSettings)
{
    public override async Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category,
        CatalogProductsCommand command)
    {
        ArgumentNullException.ThrowIfNull(category);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel { UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading };

        var currentStore = await _storeContext.GetCurrentStoreAsync();

        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, category.AllowCustomersToSelectPageSize,
            category.PageSizeOptions, category.PageSize);

        var categoryIds = new List<int> { category.Id };

        //include subcategories
        if (_catalogSettings.ShowProductsFromSubcategories)
            categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, currentStore.Id));

        //price range
        PriceRangeModel selectedPriceRange = null;
        if (_catalogSettings.EnablePriceRangeFiltering && category.PriceRangeFiltering)
        {
            selectedPriceRange = await GetConvertedPriceRangeAsync(command);

            PriceRangeModel availablePriceRange = null;
            if (!category.ManuallyPriceRange)
            {
                async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                {
                    var products = await _productService.SearchProductsAsync(0, 1,
                        categoryIds: categoryIds,
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts &&
                                                 !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                        orderBy: orderBy);

                    return products?.FirstOrDefault()?.Price ?? 0;
                }

                availablePriceRange = new PriceRangeModel
                {
                    From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                    To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                };
            }
            else
            {
                availablePriceRange = new PriceRangeModel { From = category.PriceFrom, To = category.PriceTo };
            }

            model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
        }
        
        //year range
        YearRangeModel selectedYearRange = null;
        
        selectedYearRange = GetConvertedYearRangeAsync(command);
        
        YearRangeModel availableYearRange = null;
        if (!category.ManuallyPriceRange)
        {
            async Task<IEnumerable<int>> getProductYearAsync(ProductSortingEnum orderBy)
            {
                var products = await (_productService as CostumeProductService).SearchProductsAsync(
                    categoryIds: categoryIds,
                    storeId: currentStore.Id,
                    visibleIndividuallyOnly: true,
                    excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts &&
                                             !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                    orderBy: orderBy);
                return products?.DistinctBy( t=> t.CostumeYear).OrderByDescending(t=> t.CostumeYear).Select(t=> t.CostumeYear);
            }

            var foo = await getProductYearAsync(ProductSortingEnum.Position);
            model.YearRangeFilter = new YearRangeFilterModel
            {
                SelectedYearRange = selectedYearRange, AvailableYearRange = foo
            };
        }
        
        //mileage range
        MileageRangeModel selectedMileageRange = null;
        
        selectedMileageRange = GetConvertedMileageRangeAsync(command);

        model.MileageRangeModel = selectedMileageRange;
        
        //filterable options
        var filterableOptions = await _specificationAttributeService
            .GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(category.Id);

        if (_catalogSettings.EnableSpecificationAttributeFiltering)
        {
            model.SpecificationFilter =
                await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);
        }

        //filterable manufacturers
        if (_catalogSettings.EnableManufacturerFiltering)
        {
            var manufacturers = await _manufacturerService.GetManufacturersByCategoryIdAsync(category.Id);

            model.ManufacturerFilter = await PrepareManufacturerFilterModel(command.ManufacturerIds, manufacturers);
        }

        var filteredSpecs = command.SpecificationOptionIds is null
            ? null
            : filterableOptions.Where(fo => command.SpecificationOptionIds.Contains(fo.Id)).ToList();

        //products
        var products = await (_productService as CostumeProductService).SearchProductsAsync(
            command.PageNumber - 1,
            command.PageSize,
            categoryIds: categoryIds,
            storeId: currentStore.Id,
            visibleIndividuallyOnly: true,
            excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts &&
                                     !_catalogSettings.IncludeFeaturedProductsInNormalLists,
            priceMin: selectedPriceRange?.From,
            priceMax: selectedPriceRange?.To,
            yearMin: selectedYearRange?.From,
            yearMax: selectedYearRange?.To,
            mileageMin: selectedMileageRange?.From,
            mileageMax: selectedMileageRange?.To,
            manufacturerIds: command.ManufacturerIds,
            filteredSpecOptions: filteredSpecs,
            orderBy: (ProductSortingEnum)command.OrderBy);

        var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
        await PrepareCatalogProductsAsync(model, products, isFiltering);

        return model;
    }
    
    protected YearRangeModel GetConvertedYearRangeAsync(CatalogProductsCommand command)
    {
        var result = new YearRangeModel();

        if (string.IsNullOrWhiteSpace(command.Year))
            return result;

        var fromTo = command.Year.Trim().Split(['-']);
        if (fromTo.Length == 2)
        {
            var rawFrom = fromTo[0]?.Trim();
            if (!string.IsNullOrEmpty(rawFrom) && int.TryParse(rawFrom, out var from))
                result.From = from;

            var rawTo = fromTo[1]?.Trim();
            if (!string.IsNullOrEmpty(rawTo) && int.TryParse(rawTo, out var to))
                result.To = to;

            if (result.From > result.To)
                result.From = result.To;
        }

        return result;
    }
    
    protected MileageRangeModel GetConvertedMileageRangeAsync(CatalogProductsCommand command)
    {
        var result = new MileageRangeModel();

        if (string.IsNullOrWhiteSpace(command.Mileage))
            return result;

        var fromTo = command.Mileage.Trim().Split(['-']);
        if (fromTo.Length == 2)
        {
            var rawFrom = fromTo[0]?.Trim();
            if (!string.IsNullOrEmpty(rawFrom) && int.TryParse(rawFrom, out var from))
                result.From = from;

            var rawTo = fromTo[1]?.Trim();
            if (!string.IsNullOrEmpty(rawTo) && int.TryParse(rawTo, out var to))
                result.To = to;

            if (result.From > result.To)
                result.From = result.To;
        }

        return result;
    }
}