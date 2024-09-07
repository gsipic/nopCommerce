using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Attributes;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Customer;
using Nop.Web.Themes.SaljiDalje.Data;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Themes.SaljiDalje.Controllers;

public class CustomCustomerController(
    AddressSettings addressSettings,
    CaptchaSettings captchaSettings,
    CustomerSettings customerSettings,
    DateTimeSettings dateTimeSettings,
    ForumSettings forumSettings,
    GdprSettings gdprSettings,
    HtmlEncoder htmlEncoder,
    IAddressModelFactory addressModelFactory,
    IAddressService addressService,
    IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
    IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
    IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
    IAuthenticationService authenticationService,
    ICountryService countryService,
    ICurrencyService currencyService,
    ICustomerActivityService customerActivityService,
    ICustomerModelFactory customerModelFactory,
    ICustomerRegistrationService customerRegistrationService,
    ICustomerService customerService,
    IDownloadService downloadService,
    IEventPublisher eventPublisher,
    IExportManager exportManager,
    IExternalAuthenticationService externalAuthenticationService,
    IGdprService gdprService,
    IGenericAttributeService genericAttributeService,
    IGiftCardService giftCardService,
    ILocalizationService localizationService,
    ILogger logger,
    IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
    INewsLetterSubscriptionService newsLetterSubscriptionService,
    INotificationService notificationService,
    IOrderService orderService,
    IPermissionService permissionService,
    IPictureService pictureService,
    IPriceFormatter priceFormatter,
    IProductService productService,
    IStateProvinceService stateProvinceService,
    IStoreContext storeContext,
    ITaxService taxService,
    IWorkContext workContext,
    IWorkflowMessageService workflowMessageService,
    LocalizationSettings localizationSettings,
    MediaSettings mediaSettings,
    MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
    StoreInformationSettings storeInformationSettings,
    TaxSettings taxSettings, IStoreService storeService)
    : Web.Controllers.CustomerController(addressSettings,
        captchaSettings, customerSettings, dateTimeSettings, forumSettings, gdprSettings, htmlEncoder,
        addressModelFactory, addressService, addressAttributeParser, customerAttributeParser, customerAttributeService,
        authenticationService, countryService, currencyService, customerActivityService, customerModelFactory,
        customerRegistrationService, customerService, downloadService, eventPublisher, exportManager,
        externalAuthenticationService, gdprService, genericAttributeService, giftCardService, localizationService,
        logger, multiFactorAuthenticationPluginManager, newsLetterSubscriptionService, notificationService,
        orderService, permissionService, pictureService, priceFormatter, productService, stateProvinceService,
        storeContext, taxService, workContext, workflowMessageService, localizationSettings, mediaSettings,
        multiFactorAuthenticationSettings, storeInformationSettings, taxSettings)
{
    public override async Task<IActionResult> Info()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var model = new CustomerInfoModel();
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);
        
        var modelAvatar = new CustomerAvatarModel();
        await _customerModelFactory.PrepareCustomerAvatarModelAsync(modelAvatar);
        model.CustomerAvatarModel = modelAvatar;
        model.Customer = customer;
        return View("~/Themes/SaljiDalje/Views/Customer/Info.cshtml",model);
    }

    [HttpPost]
    public override async Task<IActionResult> Info(CustomerInfoModel model, IFormCollection form)
    {
        var result = await base.Info(model, form);
        var customer = await _workContext.GetCurrentCustomerAsync();
        InsertProfilePicture(customer,form);
        return result;
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Delete()
    {
        //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
        //    return AccessDeniedView();

        //try to get a customer with the specified id
        var customer = await workContext.GetCurrentCustomerAsync();
        if (customer == null)
            return RedirectToAction("Info");

        try
        {
            //prevent attempts to delete the user, if it is the last active administrator
            if (await _customerService.IsAdminAsync(customer) && !await SecondAdminAccountExistsAsync(customer))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                return RedirectToAction("Info");
            }

            //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
            if (await _customerService.IsAdminAsync(customer) && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                return RedirectToAction("Info");
            }

            //delete
            await _customerService.DeleteCustomerAsync(customer);
            await authenticationService.SignOutAsync();

            //remove newsletter subscription (if exists)
            foreach (var store in await storeService.GetAllStoresAsync())
            {
                var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                if (subscription != null)
                    await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCustomer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomer"), customer.Id), customer);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Deleted"));

            return RedirectToRoute("Homepage");
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Info");
        }
    }
    
    protected virtual async Task<bool> SecondAdminAccountExistsAsync(Customer customer)
    {
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: [(await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName)).Id]);

        return customers.Any(c => c.Active && c.Id != customer.Id);
    }

    private async void InsertProfilePicture(Customer customer,IFormCollection form)
    {
        IFormFile uploadedFile = null;
        var customerAvatar = await _pictureService.GetPictureByIdAsync(
            await _genericAttributeService.GetAttributeAsync<int>(customer,
                NopCustomerDefaults.AvatarPictureIdAttribute));
        if (customerAvatar != null && StringValues.IsNullOrEmpty(form["filepond"]))
        {
            await _pictureService.DeletePictureAsync(customerAvatar);
            return;
        }
        
        if (customerAvatar == null && StringValues.IsNullOrEmpty(form["filepond"]))
        {
            return;
        }

        if (!StringValues.IsNullOrEmpty(form["filepond"]))
        {
            var formData = form["filepond"];
            FilePond filePond = JsonConvert.DeserializeObject<FilePond>(formData);
            //Console.Write(stepThreeModelFinish.StepThreeModel);
            //Console.Write(filePond.name);
            byte[] byteArray = Convert.FromBase64String(filePond.data);
            var stream = new MemoryStream(byteArray);
            uploadedFile = new FormFile(stream, 0, byteArray.Length, filePond.id, filePond.name)
            {
                Headers = new HeaderDictionary(), ContentType = filePond.type,
            };
        }

        var contentType = uploadedFile.ContentType.ToLowerInvariant();

        if (!contentType.Equals("image/jpeg") && !contentType.Equals("image/gif"))
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Avatar.UploadRules"));

        if (ModelState.IsValid)
        {
            try
            {
                
                if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                {
                    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                    if (uploadedFile.Length > avatarMaxSize)
                        throw new NopException(string.Format(
                            await _localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"),
                            avatarMaxSize));

                    var customerPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                    if (customerAvatar != null)
                        customerAvatar = await _pictureService.UpdatePictureAsync(customerAvatar.Id,
                            customerPictureBinary, contentType, null);
                    else
                        customerAvatar =
                            await _pictureService.InsertPictureAsync(customerPictureBinary, contentType, null);
                }

                var customerAvatarId = 0;
                if (customerAvatar != null)
                    customerAvatarId = customerAvatar.Id;

                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);

                /*model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer,
                        NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize,
                    false);*/

                //return View("~/Themes/SaljiDalje/Views/Customer/Avatar.cshtml", model);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }
        }
    }

    public async Task<IActionResult> MyCars()
    {
        return View("~/Themes/SaljiDalje/Views/Customer/MyCars.cshtml");
    }

    public async Task<IActionResult> WishList()
    {
        return View("~/Themes/SaljiDalje/Views/Customer/WishList.cshtml");
    }

    public async Task<IActionResult> Notifications()
    {
        return View("~/Themes/SaljiDalje/Views/Customer/Notifications.cshtml");
    }
    
    public async Task<IActionResult> HelpCenter()
    {
        return View("~/Themes/SaljiDalje/Views/Customer/HelpCenter.cshtml");
    }
    
}