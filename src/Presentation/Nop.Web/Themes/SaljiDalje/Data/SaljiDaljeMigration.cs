using System.Data;
using System.Text;
using FluentMigrator;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Themes.SaljiDalje.Data;

namespace SaljiDalje.Core.Data
{
    [NopMigration("2024-07-22 09:23:00", "SaljiDaljeMigration", MigrationProcessType.Update)]
    public class SaljiDaljeMigration : MigrationBase
    {
        #region Fields

        private readonly INopDataProvider _nopDataProvider;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly CustomerSettings _customerSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IRepository<Setting> _settingRepository;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly INopFileProvider _fileProvider;
        protected readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public SaljiDaljeMigration(
            INopDataProvider nopDataProvider,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService,
            ISpecificationAttributeService specificationAttributeService,
            CustomerSettings customerSettings,
            CatalogSettings catalogSettings,
            CaptchaSettings captchaSettings,
            SecuritySettings securitySettings,
            IAuthenticationPluginManager pluginManager,
            IRepository<Setting> settingRepository,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            INopFileProvider nopFileProvider, IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _nopDataProvider = nopDataProvider;
            _categoryTemplateRepository = categoryTemplateRepository;
            _languageService = languageService;
            _localizationService = localizationService;
            _settingService = settingService;
            _specificationAttributeService = specificationAttributeService;
            _customerSettings = customerSettings;
            _catalogSettings = catalogSettings;
            _captchaSettings = captchaSettings;
            _securitySettings = securitySettings;
            _authenticationPluginManager = pluginManager;
            _settingRepository = settingRepository;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _fileProvider = nopFileProvider;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            /*var method = _authenticationPluginManager.LoadPluginBySystemNameAsync("ExternalAuth.Facebook").GetAwaiter()
                .GetResult();
            if (method.PluginDescriptor.Installed)
            {
                if (!_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Contains(
                        "ExternalAuth.Facebook"))
                {
                    _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add("ExternalAuth.Facebook");
                    _settingService.SaveSettingAsync(_externalAuthenticationSettings).Wait();
                }

                var item = _settingRepository.Table.Single(item =>
                    item.Name == "facebookexternalauthsettings.clientkeyidentifier");
                item.Value = "1554191924993548";
                _settingRepository.UpdateAsync(item).Wait();

                var item2 = _settingRepository.Table.Single(item =>
                    item.Name == "facebookexternalauthsettings.clientsecret");
                item2.Value = "60a581cbf9a9491ee86c9c2fe48606d3";
                _settingRepository.UpdateAsync(item2).Wait();
            }
            else
            {
                throw new NotSupportedException();
            }*/


            var storeInformationSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
            storeInformationSettings.AllowCustomerToSelectTheme = true;
            storeInformationSettings.DefaultStoreTheme = "SaljiDalje";

            _captchaSettings.Enabled = true;
            _captchaSettings.CaptchaType = CaptchaType.CheckBoxReCaptchaV2;
            _captchaSettings.ShowOnRegistrationPage = true;
            _captchaSettings.ReCaptchaPublicKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
            _captchaSettings.ReCaptchaPrivateKey = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe";

            _settingService.SaveSettingAsync(_captchaSettings).Wait();
            _settingService.SaveSettingAsync(storeInformationSettings).Wait();

            _securitySettings.HoneypotEnabled = true;

            _settingService.SaveSettingAsync(_securitySettings).Wait();

            //EngineContext.Current.Resolve<IThemeContext>().SetWorkingThemeNameAsync("SaljiDalje");

            _customerSettings.UsernamesEnabled = true;
            _customerSettings.FirstNameRequired = false;
            _customerSettings.LastNameRequired = false;
            _customerSettings.AllowCustomersToUploadAvatars = true;
            _customerSettings.AvatarMaximumSizeBytes = 2000000;


            _catalogSettings.ShowProductsFromSubcategories = true;
            _catalogSettings.EnablePriceRangeFiltering = true;
            _catalogSettings.ShowLinkToAllResultInSearchAutoComplete = true;
            _catalogSettings.ShowProductImagesInSearchAutoComplete = true;


            _settingService.SaveSettingAsync(_customerSettings).Wait();
            _settingService.SaveSettingAsync(_catalogSettings).Wait();

            _mediaSettings.AutoCompleteSearchThumbPictureSize = 80;
            _settingService.SaveSettingAsync(_mediaSettings).Wait();

            InstallCategories();

            Create.TableFor<CostumerPictureAttachmentMapping>();

            Create.TableFor<ProductExtended>();

            Create.ForeignKey()
                .FromTable(nameof(ProductExtended))
                .ForeignColumn(nameof(ProductExtended.ProductId))
                .ToTable(nameof(Product)).PrimaryColumn(nameof(Product.Id)).OnDelete(Rule.Cascade);

            Create.ForeignKey()
                .FromTable(nameof(ProductExtended))
                .ForeignColumn(nameof(ProductExtended.UserId))
                .ToTable(nameof(Customer)).PrimaryColumn(nameof(Customer.Id)).OnDelete(Rule.Cascade);
        }

        private void InstallCategories()
        {
            var filePath = _fileProvider.MapPath("~/Themes/SaljiDalje/categorydata.json");
            var jsonString = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
            var adsCategory = JsonConvert.DeserializeObject<SaljiDaljeCategory>(jsonString);
            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            var pictureService = EngineContext.Current.Resolve<IPictureService>();


            var prop = adsCategory.GetType().GetProperties();
            for (int i = 0; i < prop.Length; i++)
            {
                var value = prop[i].GetValue(adsCategory, null);

                var categoryName = (string)value
                    .GetType()
                    .GetProperty("CategoryName")
                    .GetValue(value);

                var categoryImagePropertyInfo = value
                    .GetType()
                    .GetProperty("CategoryImage");

                string categoryImage = null;
                int? pictureId = null;
                if (categoryImagePropertyInfo != null && categoryImagePropertyInfo.GetValue(value) != null)
                {
                    categoryImage = (string)categoryImagePropertyInfo.GetValue(value);
                    pictureId = InsertCategoryPicture(categoryName, categoryImage);
                }

                //var categoryImage2PropertyInfo = value
                //    .GetType()
                //    .GetProperty("CategoryImage2");
                //if (categoryImage2PropertyInfo != null)
                //{
                //    categoryImage = (string) categoryImage2PropertyInfo.GetValue(value);
                //    InsertCategoryPicture(categoryName, categoryImage);
                //}


                var category = AddCategories(categoryName, i, default,
                    true, pictureId ?? default);

                List<ChildCategory> type =
                    (List<ChildCategory>)value.GetType().GetProperty("ChildCategory").GetValue(value);
                AddChildCategory(type, category.Id);
            }

            void AddChildCategory(List<ChildCategory> categories, int parentCategoryId)
            {
                for (int i = 0; i < categories.Count; i++)
                {
                    var categoryImage = categories[i].CategoryImage;
                    int? pictureId = null;
                    if (categoryImage != null)
                    {
                        pictureId = InsertCategoryPicture(categories[i].CategoryName, categoryImage);
                    }

                    var category = AddCategories(categories[i].CategoryName, i,
                        parentCategoryId, false, pictureId ?? default);

                    if (categories[i].NestedChildCategory != null)
                    {
                        AddChildCategory(categories[i].NestedChildCategory, category.Id);
                    }
                }
            }

            Category AddCategories(string name, int displayOrder, int parentCategoryId = default,
                bool showOnHomePage = false, int? pictureId = null)
            {
                var categoryTemplateInGridAndLines = _categoryTemplateRepository
                    .Table.FirstOrDefault(pt => pt.Name == "Products in Grid or Lines");
                if (categoryTemplateInGridAndLines == null)
                    throw new Exception("Category template cannot be loaded");

                var category = new Category
                {
                    Name = name,
                    CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                    ParentCategoryId = parentCategoryId,
                    PageSize = 25,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "25,50,100",
                    IncludeInTopMenu = true,
                    PictureId = pictureId ?? default,
                    ShowOnHomepage = showOnHomePage,
                    PriceRangeFiltering = true,
                    Published = true,
                    DisplayOrder = displayOrder,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
                
                var result = _nopDataProvider.InsertEntityAsync(category).Result;

                GenerateSeoName(result);

                return result;
            }

            void GenerateSeoName(Category category)
            {
                var SeName = urlRecordService.ValidateSeNameAsync(category, null, category.Name, true).Result;
                urlRecordService.SaveSlugAsync(category, SeName, _workContext.GetWorkingLanguageAsync().Result.Id).Wait();
            }

            int InsertCategoryPicture(string categoryName, string pictureLocalUri)
            {
                var imagePath = _fileProvider.MapPath(pictureLocalUri);
                return (pictureService.InsertPictureAsync(
                    _fileProvider.ReadAllBytesAsync(imagePath).Result,
                    MimeTypes.ImageJpeg,
                    pictureService.GetPictureSeNameAsync(categoryName).Result)).Id;
            }
        }


        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}