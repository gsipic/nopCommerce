﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using BlazorApp1.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Themes.SaljiDalje.Data;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

[Authorize]
public partial class SellCarController(
    ICatalogModelFactory catalogModelFactory,
    ICategoryService categoryService,
    IProductService productService,
    IUrlRecordService urlRecordService,
    IPictureService pictureService,
    INotificationService notificationService,
    ISpecificationAttributeService specificationAttributeService,
    IRepository<ProductExtended> productExtendedRepository,
    IRepository<Product> productRepository,
    IRepository<SpecificationAttribute> specificationAttributeRepository,
    IRepository<CostumerPictureAttachmentMapping> costumerPictureAttachmentMappingRepository,
    IWorkContext workContext)
    : BasePublicController
{
    public async Task<IActionResult> Index(bool isEditMode = false, int productId = 0, int? categoryId = 10)
    {
        Product productToEdit = null;
        StepTwoModel stepTwoModel = null;

        Dictionary<string, IList<SpecificationOption>> specificationAttributeOptions = new();

        var StanjeOption = await getSpecificationOptions("BasicInfo","Stanje vozila");

        var adType = await getSpecificationOptions("BasicInfo","Tip prodavaca");

        //Location
        var location = await getSpecificationOptions("Location","Županija");

        var negotiateForPrice = await getSpecificationOptions("Price","Pregovaranje za cijenu");
        
        var bodyType = await getSpecificationOptions("Vehicle information","BodyType");
        
        var fuelType = await getSpecificationOptions("Vehicle information","FuelType");
        
        var transmission = await getSpecificationOptions("Vehicle information","Tranmission");
        
        var driveTrain = await getSpecificationOptions("Vehicle information","DriveTrain");
        
        var color = await getSpecificationOptions("Vehicle information","Color");
        
        var sustavPomoći = await getSpecificationOptionsWithValue("Features","Sustav pomoći");
        
        var sigurnostPutnika = await getSpecificationOptionsWithValue("Features","Sigurnost putnika");
        
        var udobnostPutnika = await getSpecificationOptionsWithValue("Features","Udobnost putnika");
        
        var svjetlaifarovi = await getSpecificationOptionsWithValue("Features","Svjetla i farovi");
        
        var Zaštitaodkrađe = await getSpecificationOptionsWithValue("Features","Zaštita od krađe");
        
        var Multimedia = await getSpecificationOptionsWithValue("Features","Multimedia");
        
        var Gumeinaplatci = await getSpecificationOptionsWithValue("Features","Gume i naplatci");
        
        var Ostalidodaci = await getSpecificationOptionsWithValue("Features","Ostali dodaci");
        
        var županije = await getSpecificationOptions("Location","Županija");
        

        specificationAttributeOptions["PregovaranjeZaCijenu"] = negotiateForPrice;

        specificationAttributeOptions["StanjeOption"] = StanjeOption;

        specificationAttributeOptions["Županije"] = location;


        if (isEditMode)
        {
            var itemsOfValue = StanjeOption;
            var itemsOfValueZupanije = location.Select(itemA => int.Parse(itemA.Value));

            var currentUser = await workContext.GetCurrentCustomerAsync();

            var productSpecification = await specificationAttributeService
                .GetProductSpecificationAttributesAsync(productId);

            var productSpecificationOptionZupanije = productSpecification.FirstOrDefault(itemSpecification =>
                itemsOfValueZupanije.Any(itemB => itemSpecification.SpecificationAttributeOptionId == itemB));

            var productSpecificationPrice = negotiateForPrice;

            categoryId = (await categoryService
                .GetProductCategoriesByProductIdAsync(productId))[0].CategoryId;

            productToEdit =
                await (from p in productRepository.Table
                    join pe in productExtendedRepository.Table on p.Id equals pe.ProductId
                    where pe.UserId == currentUser.Id && p.Id == productId
                    select p).SingleAsync();

            if (productToEdit == null)
            {
                return RedirectToRoute("Homepage");
            }
        }
        else
        {
            if (categoryId == null)
            {
                return RedirectToRoute("Homepage");
            }
            else
            {
            }
        }

        var allCategories = await categoryService.GetAllCategoriesByParentCategoryIdAsync(1);
        var Make = (allCategories).Select(option =>
            new SpecificationOption { Text = option.Name, Value = option.Id.ToString() }).ToList();
        var Model = (await categoryService.GetAllCategoriesByParentCategoryIdAsync(Int32.Parse(Make.First().Value)))
            .Select(option => new SpecificationOption { Text = option.Name, Value = option.Id.ToString() }).ToList();
        var listYears = Enumerable.Range(1971, DateTime.Now.Year - 1971 + 1).Select(year => new SpecificationOption {
            Text = year.ToString(),
            Value = year.ToString(),
        }).ToList();

        return View("~/Themes/SaljiDalje/Views/SellCar/Index.cshtml",
            new SellCarModel
            {
                BasicInfo = new BasicInfo
                {
                    Title = "",
                    VehicleCondition = StanjeOption,
                    SpecificationAttributeOptionIdStateOptionId = StanjeOption.Select(option => option.Value).First(),
                    SpecificationAttributeAdTypeIdStateOptionId = adType.First().Value,
                    AdType = adType,
                },
                Price = new Price { SellCarPrice = 200, NegotiatedPrice = false },
                VehicleInformation = new VehicleInformation
                {
                    Make = Make,
                    Model = Model,
                    Year = Int32.Parse(listYears.Last().Value),
                    YearOptions = listYears,
                    Mileage = null,
                    VIN = string.Empty,
                    BodyType = bodyType,
                    FuelType = fuelType,
                    Tranmission = transmission,
                    DriveTrain = driveTrain,
                    Color = color
                },
                Features = new Features
                {
                    SustavPomoći = sustavPomoći,
                    SigurnostPutnika = sigurnostPutnika,
                    UdobnostPutnika = udobnostPutnika,
                    SvjetlaiFarovi = svjetlaifarovi,
                    ZaštitaOdKrađe = Zaštitaodkrađe,
                    Multimedia = Multimedia,
                    Gumeinaplatci = Gumeinaplatci,
                    Ostalidodaci = Ostalidodaci,
                    
                },
                Location = new Location
                {
                    Županije = županije
                },
                Contacts = new Contacts()
            });
    }

    private async Task<IList<SpecificationOption>> getSpecificationOptions(string specificationGroup, string specificationOption)
    {
        var specificationAttributeGroup = (await specificationAttributeService
                .GetSpecificationAttributeGroupsAsync())
            .First(item => item.Name == specificationGroup);

        var specificationAttributeBasicAds = await specificationAttributeService
            .GetSpecificationAttributesByGroupIdAsync(specificationAttributeGroup.Id);

        var specificationAttributeState = specificationAttributeBasicAds
            .Single(item => item.Name == specificationOption);
        
        var StanjeOption = (await specificationAttributeService
            .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                specificationAttributeState.Id)).Select( option => new SpecificationOption
        {
            Text = option.Name,
            Value = option.Id.ToString()
        }).ToList();
        return StanjeOption;
    }
    
    private async Task<IList<SpecificationOptionValue>> getSpecificationOptionsWithValue(string specificationGroup, string specificationOption)
    {
        var specificationAttributeGroup = (await specificationAttributeService
                .GetSpecificationAttributeGroupsAsync())
            .First(item => item.Name == specificationGroup);

        var specificationAttributeBasicAds = await specificationAttributeService
            .GetSpecificationAttributesByGroupIdAsync(specificationAttributeGroup.Id);

        var specificationAttributeState = specificationAttributeBasicAds
            .Single(item => item.Name == specificationOption);
        
        var StanjeOption = (await specificationAttributeService
            .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                specificationAttributeState.Id)).Select( option => new SpecificationOptionValue
        {
            Text = option.Name,
            Value = option.Id.ToString(),
            Data = false
        }).ToList();
        return StanjeOption;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ChildrenCategories([FromBody] PostExample test)
    {
        Console.WriteLine(test);
        var model = (await categoryService.GetAllCategoriesByParentCategoryIdAsync(test.id))
            .Select(option => new SpecificationOption { Text = option.Name, Value = option.Id.ToString() }).ToList();
        if (model.Count == 0)
        {
            return NoContent();
        }

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> Test(BlazorApp1.Pages.SellCarModel test)
    {
        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> StepTwoFinish(StepTwoModel stepTwoModel)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var product = new Product
            {
                Name = stepTwoModel.Title,
                Price = stepTwoModel.Cijena ?? 0,
                FullDescription = stepTwoModel.Description,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Published = true,
                VisibleIndividually = true
            };
            await productService.InsertProductAsync(product);

            //search engine name
            var SeName = await urlRecordService.ValidateSeNameAsync(product, null, product.Name, true);
            await urlRecordService.SaveSlugAsync(product, SeName, 0);

            //locales
            //await UpdateLocalesAsync(product, model);

            //categories
            await SaveCategoryMappingsAsync(product, stepTwoModel);

            await Insertpictures(stepTwoModel, product);

            //var psa = model.ToEntity<ProductSpecificationAttribute>();
            //psa.CustomValue = model.ValueRaw;

            //var sawp = await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync();
            //sawp.First(item => item.Id == 1 )
            async Task InsertProductAttribute(string s)
            {
                int numericValue;
                if (int.TryParse(s, out numericValue))
                    await specificationAttributeService.InsertProductSpecificationAttributeAsync(
                        new ProductSpecificationAttribute()
                        {
                            ProductId = product.Id,
                            AttributeType = SpecificationAttributeType.Option,
                            AllowFiltering = true,
                            ShowOnProductPage = true,
                            SpecificationAttributeOptionId = numericValue
                        });
            }

            foreach (var mogućaDostavaItem in stepTwoModel.MogucaDostavaList)
            {
                await InsertProductAttribute(mogućaDostavaItem);
            }

            foreach (var mogućnostPlaćanjaItem in stepTwoModel.MogućnostPlaćanjaList)
            {
                await InsertProductAttribute(mogućnostPlaćanjaItem);
            }

            var psa = new ProductSpecificationAttribute
            {
                ProductId = product.Id,
                AttributeType = SpecificationAttributeType.Option,
                AllowFiltering = true,
                ShowOnProductPage = true,
                SpecificationAttributeOptionId = stepTwoModel.SpecificationAttributeOptionIdStateOptionId
            };
            var psaZupanija = new ProductSpecificationAttribute
            {
                ProductId = product.Id,
                AttributeType = SpecificationAttributeType.Option,
                AllowFiltering = true,
                ShowOnProductPage = true,
                SpecificationAttributeOptionId = (int)stepTwoModel.OdabaranaZupanija
            };

            await specificationAttributeService.InsertProductSpecificationAttributeAsync(psa);
            await specificationAttributeService
                .InsertProductSpecificationAttributeAsync(
                    psaZupanija); //_specificationAttributeService.GetProductSpecificationAttributesAsync(1);

            //await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(Convert.ToInt32(attributeId));

            await productExtendedRepository.InsertAsync(new ProductExtended
            {
                ProductId = product.Id, UserId = (await workContext.GetCurrentCustomerAsync()).Id
            });

            notificationService.SuccessNotification("Uspješno dodan oglas!");

            transaction.Complete();
            transaction.Dispose();

            return RedirectToRoute("Homepage");
        }
        catch (Exception exception)
        {
            transaction.Dispose();
            throw;
        }
    }

    private async Task Insertpictures(StepTwoModel stepTwoModel, Product product)
    {
        foreach (var base64EncodedPictures in stepTwoModel.ImageFile)
        {
            if (base64EncodedPictures == null)
            {
                return;
            }
        }

        foreach (var base64EncodedPictures in stepTwoModel.ImageFile)
        {
            var filePond = await costumerPictureAttachmentMappingRepository.Table
                .FirstOrDefaultAsync(item => item.Guid == base64EncodedPictures);
            //Console.Write(stepThreeModelFinish.StepThreeModel);
            //Console.Write(filePond.name);
            byte[] byteArray = filePond.PictureData;
            var stream = new MemoryStream(byteArray);
            IFormFile file = new FormFile(stream, 0, byteArray.Length, filePond.Guid, filePond.FileName)
            {
                Headers = new HeaderDictionary(), ContentType = filePond.FileType,
            };

            var picture = await pictureService.InsertPictureAsync(file, file.Name);
            await productService.InsertProductPictureAsync(new ProductPicture
            {
                PictureId = picture.Id, ProductId = product.Id, DisplayOrder = 1
            });

            await costumerPictureAttachmentMappingRepository.DeleteAsync(filePond);
        }


        //Console.Write(picture.VirtualPath);
    }

    protected virtual async Task SaveCategoryMappingsAsync(Product product, StepTwoModel model)
    {
        var existingProductCategories =
            await categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

        //delete categories
        foreach (var existingProductCategory in existingProductCategories)
            if (model.categoryId != existingProductCategory.CategoryId)
                await categoryService.DeleteProductCategoryAsync(existingProductCategory);

        //add categories

        if (categoryService.FindProductCategory(existingProductCategories, product.Id, model.categoryId) == null)
        {
            //find next display order
            var displayOrder = 1;
            var existingCategoryMapping =
                await categoryService.GetProductCategoriesByCategoryIdAsync(model.categoryId, showHidden: true);
            if (existingCategoryMapping.Any())
                displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
            await categoryService.InsertProductCategoryAsync(new ProductCategory
            {
                ProductId = product.Id, CategoryId = model.categoryId, DisplayOrder = displayOrder
            });
        }
    }
}

public record StepTwoModel
{
    public Dictionary<string, IList<SpecificationAttributeOption>> SpecificationAttributeOptions = new();
    public int categoryId { get; set; }
    [Microsoft.Build.Framework.Required] public string Title { get; set; }
    [Microsoft.Build.Framework.Required] public IList<SelectListItem> Stanje { get; set; }

    [DisplayName("Upload File")] public string[] ImageFile { get; set; }

    [Microsoft.Build.Framework.Required] public IList<GenericItem> GenericOptionsList { get; set; }


    [Microsoft.Build.Framework.Required] public IList<SpecificationAttributeOption> Dostava { get; set; }

    public IList<SpecificationAttributeOption> MogućnostPlaćanja { get; set; }

    [Microsoft.Build.Framework.Required] public IList<SelectListItem> Zupanije { get; set; }
    [Microsoft.Build.Framework.Required] public int? OdabaranaZupanija { get; set; }

    [Microsoft.Build.Framework.Required] public string Description { get; set; }

    public int SpecificationAttributeOptionIdStateOptionId { get; set; }
    public string MogucaDostava { get; set; }
    [Microsoft.Build.Framework.Required] public IList<string> MogucaDostavaList { get; set; }
    [Microsoft.Build.Framework.Required] public IList<string> MogućnostPlaćanjaList { get; set; }

    public string RazgledavanjePutemPoziva { get; set; }

    [Microsoft.Build.Framework.Required] public Valuta Valuta { get; set; }

    public int? Cijena { get; set; }

    [Display(Name = "Pregovaranje za cijenu")]
    public Boolean NegotatiedPrice { get; set; }

    // Mogućnost plaćanja
    [Display(Name = "Gotovina")] public string Gotovina { get; set; }
    [Display(Name = "Kredit")] public string Kredit { get; set; }
    [Display(Name = "Leasing")] public string Leasing { get; set; }

    [Display(Name = "Obročno Bankovnim Karticama")]
    public string ObrocnoBankovnimKarticama { get; set; }

    [Display(Name = "Preuzimanje Leasinga")]
    public string PreuzimanjeLeasinga { get; set; }

    [Display(Name = "Staro za Novo")] public string StaroZaNovo { get; set; }
    [Display(Name = "Zamjena")] public string Zamjena { get; set; }
}

public enum Valuta
{
    // [Display(Name = "$")] USD,
    [Display(Name = "€")] EURO,
}

public record GenericItem
{
    [Required] public string item { get; set; }
    public string itemBool { get; set; }
}