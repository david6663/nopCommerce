﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer model factory implementation
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAffiliateService _affiliateService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerReportService _customerReportService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;        

        #endregion

        #region Ctor

        public CustomerModelFactory(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAffiliateService affiliateService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerReportService customerReportService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IRewardPointService rewardPointService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings)
        {
            this._addressSettings = addressSettings;
            this._customerSettings = customerSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._aclSupportedModelFactory = aclSupportedModelFactory;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._affiliateService = affiliateService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._customerActivityService = customerActivityService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._customerReportService = customerReportService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._externalAuthenticationService = externalAuthenticationService;
            this._genericAttributeService = genericAttributeService;
            this._geoLookupService = geoLookupService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._orderService = orderService;
            this._pictureService = pictureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeFormatter = productAttributeFormatter;
            this._rewardPointService = rewardPointService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._taxService = taxService;
            this._workContext = workContext;
            this._mediaSettings = mediaSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the reward points model to add to the customer
        /// </summary>
        /// <param name="model">Reward points model to add to the customer</param>
        protected virtual void PrepareAddRewardPointsToCustomerModel(AddRewardPointsToCustomerModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Message = _localizationService.GetResource("Admin.Customers.Customers.SomeComment");
            model.ActivatePointsImmediately = true;
            model.StoreId = _storeContext.CurrentStore.Id;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
        }

        /// <summary>
        /// Prepare customer associated external authorization models
        /// </summary>
        /// <param name="models">List of customer associated external authorization models</param>
        /// <param name="customer">Customer</param>
        protected virtual void PrepareAssociatedExternalAuthModels(IList<CustomerAssociatedExternalAuthModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            foreach (var record in customer.ExternalAuthenticationRecords)
            {
                var method = _externalAuthenticationService.LoadExternalAuthenticationMethodBySystemName(record.ProviderSystemName);
                if (method == null)
                    continue;

                models.Add(new CustomerAssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = record.ExternalDisplayIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }
        }

        /// <summary>
        /// Prepare customer attribute models
        /// </summary>
        /// <param name="models">List of customer attribute models</param>
        /// <param name="customer">Customer</param>
        protected virtual void PrepareCustomerAttributeModels(IList<CustomerModel.CustomerAttributeModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available customer attributes
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = customer
                        .GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedCustomerAttributes);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                models.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.FirstNameEnabled = true;
            model.FirstNameRequired = true;
            model.LastNameEnabled = true;
            model.LastNameRequired = true;
            model.EmailEnabled = true;
            model.EmailRequired = true;
            model.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.CompanyRequired = _addressSettings.CompanyRequired;
            model.CountryEnabled = _addressSettings.CountryEnabled;
            model.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.CityEnabled = _addressSettings.CityEnabled;
            model.CityRequired = _addressSettings.CityRequired;
            model.CountyEnabled = _addressSettings.CountyEnabled;
            model.CountyRequired = _addressSettings.CountyRequired;
            model.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.PhoneRequired = _addressSettings.PhoneRequired;
            model.FaxEnabled = _addressSettings.FaxEnabled;
            model.FaxRequired = _addressSettings.FaxRequired;

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);

            //prepare custom address attributes
            model.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);
        }

        /// <summary>
        /// Prepare HTML string address
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareModelAddressHtml(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var addressHtmlSb = new StringBuilder("<div>");

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

            if (_addressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

            if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

            if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

            if (_addressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

            if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

            if (_addressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
                addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

            var customAttributesFormatted = _addressAttributeFormatter.FormatAttributes(address?.CustomAttributes);
            if (!string.IsNullOrEmpty(customAttributesFormatted))
            {
                //already encoded
                addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
            }

            addressHtmlSb.Append("</div>");

            model.AddressHtml = addressHtmlSb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare customer search model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>Customer search model</returns>
        public virtual CustomerSearchModel PrepareCustomerSearchModel(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = _customerSettings.AllowCustomersToUploadAvatars;
            searchModel.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            searchModel.CompanyEnabled = _customerSettings.CompanyEnabled;
            searchModel.PhoneEnabled = _customerSettings.PhoneEnabled;
            searchModel.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;

            //search registered customers by default
            var registeredRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole != null)
                searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

            //prepare available customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer list model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>Customer list model</returns>
        public virtual CustomerListModel PrepareCustomerListModel(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

            //get customers
            var customers = _customerService.GetAllCustomers(loadOnlyWithShoppingCart: false,
                customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                phone: searchModel.SearchPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CustomerListModel
            {
                Data = customers.Select(customer =>
                {
                    //fill in model values from the entity
                    var customerModel = new CustomerModel
                    {
                        Id = customer.Id,
                        Email = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        Username = customer.Username,
                        FullName = customer.GetFullName(),
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        Active = customer.Active
                    };

                    //convert dates to the user time
                    customerModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    customerModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    customerModel.CustomerRoleNames = string.Join(", ", customer.CustomerRoles.Select(role => role.Name));
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        var avatarPictureId = customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId);
                        customerModel.AvatarUrl = _pictureService.GetPictureUrl(avatarPictureId, _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    return customerModel;
                }),
                Total = customers.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare customer model
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer model</returns>
        public virtual CustomerModel PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties = false)
        {
            if (customer != null)
            {
                //fill in model values from the entity
                model = model ?? new CustomerModel();

                model.Id = customer.Id;
                model.DisplayVatNumber = _taxSettings.EuVatEnabled;
                model.AllowSendingOfWelcomeMessage = customer.IsRegistered() &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = customer.IsRegistered() && !customer.Active &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.VendorId = customer.VendorId;
                    model.AdminComment = customer.AdminComment;
                    model.IsTaxExempt = customer.IsTaxExempt;
                    model.Active = customer.Active;
                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                    model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                    model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                    model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                    model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                    model.County = customer.GetAttribute<string>(SystemCustomerAttributeNames.County);
                    model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                    model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);
                    model.TimeZoneId = customer.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId);
                    model.VatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);
                    model.VatNumberStatusNote = ((VatNumberStatus)customer
                        .GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId))
                        .GetLocalizedEnum(_localizationService, _workContext);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);
                    model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(role => role.Id).ToList();
                    model.RegisteredInStore = _storeService.GetAllStores()
                        .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;

                    //prepare model affiliate
                    var affiliate = _affiliateService.GetAffiliateById(customer.AffiliateId);
                    if (affiliate != null)
                    {
                        model.AffiliateId = affiliate.Id;
                        model.AffiliateName = affiliate.GetFullName();
                    }

                    //prepare model newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        model.SelectedNewsletterSubscriptionStoreIds = _storeService.GetAllStores()
                            .Where(store => _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id) != null)
                            .Select(store => store.Id).ToList();
                    }
                }

                //precheck Registered Role as a default role while creating a new customer through admin
                var registeredRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
                if (registeredRole != null)
                    model.SelectedCustomerRoleIds.Add(registeredRole.Id);

                //prepare reward points model
                model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
                if (model.DisplayRewardPointsHistory)
                    PrepareAddRewardPointsToCustomerModel(model.AddRewardPoints);

                //prepare external authentication records
                PrepareAssociatedExternalAuthModels(model.AssociatedExternalAuthRecords, customer);

                //prepare nested search models
                PrepareRewardPointsSearchModel(model.CustomerRewardPointsSearchModel, customer);
                PrepareCustomerAddressSearchModel(model.CustomerAddressSearchModel, customer);
                PrepareCustomerOrderSearchModel(model.CustomerOrderSearchModel, customer);
                PrepareCustomerShoppingCartSearchModel(model.CustomerShoppingCartSearchModel, customer);
                PrepareCustomerActivityLogSearchModel(model.CustomerActivityLogSearchModel, customer);
                PrepareCustomerBackInStockSubscriptionSearchModel(model.CustomerBackInStockSubscriptionSearchModel, customer);
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.FaxEnabled = _customerSettings.FaxEnabled;

            //set default values for the new model
            if (customer == null)
            {
                model.Active = true;
                model.DisplayVatNumber = false;
            }

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors,
                defaultItemText: _localizationService.GetResource("Admin.Customers.Customers.Fields.Vendor.None"));

            //prepare model customer attributes
            PrepareCustomerAttributeModels(model.CustomerAttributes, customer);

            //prepare model stores for newsletter subscriptions
            model.AvailableNewsletterSubscriptionStores = _storeService.GetAllStores().Select(store => new SelectListItem
            {
                Value = store.Id.ToString(),
                Text = store.Name,
                Selected = model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id)
            }).ToList();

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model);

            //prepare available time zones
            _baseAdminModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            //prepare available countries and states
            if (_customerSettings.CountryEnabled)
            {
                _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);
                if (_customerSettings.StateProvinceEnabled)
                    _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
            }

            return model;
        }

        /// <summary>
        /// Prepare reward points search model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points search model</returns>
        public virtual CustomerRewardPointsSearchModel PrepareRewardPointsSearchModel(CustomerRewardPointsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points list model</returns>
        public virtual CustomerRewardPointsListModel PrepareRewardPointsListModel(CustomerRewardPointsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get reward points history
            var rewardPoints = _rewardPointService.GetRewardPointsHistory(customer.Id,
                showNotActivated: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CustomerRewardPointsListModel
            {
                Data = rewardPoints.Select(historyEntry =>
                {
                    //fill in model values from the entity        
                    var rewardPointsHistoryModel = new CustomerRewardPointsModel
                    {
                        Points = historyEntry.Points,
                        Message = historyEntry.Message
                    };

                    //convert dates to the user time
                    var activatingDate = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                    rewardPointsHistoryModel.CreatedOn = activatingDate;
                    rewardPointsHistoryModel.PointsBalance = historyEntry.PointsBalance.HasValue ? historyEntry.PointsBalance.ToString() :
                        string.Format(_localizationService.GetResource("Admin.Customers.Customers.RewardPoints.ActivatedLater"), activatingDate);
                    rewardPointsHistoryModel.EndDate = !historyEntry.EndDateUtc.HasValue ? null :
                        (DateTime?)_dateTimeHelper.ConvertToUserTime(historyEntry.EndDateUtc.Value, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    rewardPointsHistoryModel.StoreName = _storeService.GetStoreById(historyEntry.StoreId)?.Name ?? "Unknown";

                    return rewardPointsHistoryModel;
                }),
                Total = rewardPoints.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare customer address search model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address search model</returns>
        public virtual CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer address list model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address list model</returns>
        public virtual CustomerAddressListModel PrepareCustomerAddressListModel(CustomerAddressSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer addresses
            var addresses = customer.Addresses
                .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList();

            //prepare list model
            var model = new CustomerAddressListModel
            {
                Data = addresses.PaginationByRequestModel(searchModel).Select(address =>
                {
                    //fill in model values from the entity        
                    var addressModel = address.ToModel();

                    //fill in additional values (not existing in the entity)
                    PrepareModelAddressHtml(addressModel, address);

                    return addressModel;
                }),
                Total = addresses.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare customer address model
        /// </summary>
        /// <param name="model">Customer address model</param>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer address model</returns>
        public virtual CustomerAddressModel PrepareCustomerAddressModel(CustomerAddressModel model,
            Customer customer, Address address, bool excludeProperties = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (address != null)
            {
                //fill in model values from the entity
                model = model ?? new CustomerAddressModel();

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.Address = address.ToModel();
            }

            model.CustomerId = customer.Id;

            //prepare address model
            PrepareAddressModel(model.Address, address);

            return model;
        }

        /// <summary>
        /// Prepare customer order search model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order search model</returns>
        public virtual CustomerOrderSearchModel PrepareCustomerOrderSearchModel(CustomerOrderSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer order list model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListModel PrepareCustomerOrderListModel(CustomerOrderSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer orders
            var orders = _orderService.SearchOrders(customerId: customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CustomerOrderListModel
            {
                Data = orders.Select(order =>
                {
                    //fill in model values from the entity
                    var orderModel = new CustomerOrderModel
                    {
                        Id = order.Id,
                        OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false),
                        CustomOrderNumber = order.CustomOrderNumber
                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Unknown";

                    return orderModel;
                }),
                Total = orders.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare customer reports search model
        /// </summary>
        /// <param name="searchModel">Customer reports search model</param>
        /// <returns>Customer reports search model</returns>
        public virtual CustomerReportsSearchModel PrepareCustomerReportsSearchModel(CustomerReportsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search models
            PrepareBestCustomersReportSearchModel(searchModel.BestCustomersByOrderTotal);
            PrepareBestCustomersReportSearchModel(searchModel.BestCustomersByNumberOfOrders);
            PrepareRegisteredCustomersReportSearchModel(searchModel.RegisteredCustomers);

            return searchModel;
        }

        /// <summary>
        /// Prepare best customers report search model
        /// </summary>
        /// <param name="searchModel">Best customers report search model</param>
        /// <returns>Best customers report search model</returns>
        public virtual BestCustomersReportSearchModel PrepareBestCustomersReportSearchModel(BestCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available order, payment and shipping statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);
            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);
            _baseAdminModelFactory.PrepareShippingStatuses(searchModel.AvailableShippingStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged best customers report list model
        /// </summary>
        /// <param name="searchModel">Best customers report search model</param>
        /// <returns>Best customers report list model</returns>
        public virtual BestCustomersReportListModel PrepareBestCustomersReportListModel(BestCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var orderStatus = searchModel.OrderStatusId > 0 ? (OrderStatus?)searchModel.OrderStatusId : null;
            var paymentStatus = searchModel.PaymentStatusId > 0 ? (PaymentStatus?)searchModel.PaymentStatusId : null;
            var shippingStatus = searchModel.ShippingStatusId > 0 ? (ShippingStatus?)searchModel.ShippingStatusId : null;

            //get report items
            var reportItems = _customerReportService.GetBestCustomersReport(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                ss: shippingStatus,
                orderBy: searchModel.OrderBy,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new BestCustomersReportListModel
            {
                Data = reportItems.Select(item =>
                {
                    //fill in model values from the entity
                    var bestCustomersReportModel = new BestCustomersReportModel
                    {
                        CustomerId = item.CustomerId,
                        OrderTotal = _priceFormatter.FormatPrice(item.OrderTotal, true, false),
                        OrderCount = item.OrderCount,
                    };

                    //fill in additional values (not existing in the entity)
                    var customer = _customerService.GetCustomerById(item.CustomerId);
                    if (customer != null)
                    {
                        bestCustomersReportModel.CustomerName = customer.IsRegistered() ? customer.Email :
                            _localizationService.GetResource("Admin.Customers.Guest");
                    }

                    return bestCustomersReportModel;
                }),
                Total = reportItems.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare registered customers report search model
        /// </summary>
        /// <param name="searchModel">Registered customers report search model</param>
        /// <returns>Registered customers report search model</returns>
        public virtual RegisteredCustomersReportSearchModel PrepareRegisteredCustomersReportSearchModel(RegisteredCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged registered customers report list model
        /// </summary>
        /// <param name="searchModel">Registered customers report search model</param>
        /// <returns>Registered customers report list model</returns>
        public virtual RegisteredCustomersReportListModel PrepareRegisteredCustomersReportListModel(RegisteredCustomersReportSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get report items
            var reportItems = new List<RegisteredCustomersReportModel>
            {
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.7days"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(7)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.14days"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(14)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.month"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(30)
                },
                new RegisteredCustomersReportModel
                {
                    Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.year"),
                    Customers = _customerReportService.GetRegisteredCustomersReport(365)
                }
            };

            //prepare list model
            var model = new RegisteredCustomersReportListModel
            {
                Data = reportItems.PaginationByRequestModel(searchModel),
                Total = reportItems.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare customer shopping cart search model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer shopping cart search model</returns>
        public virtual CustomerShoppingCartSearchModel PrepareCustomerShoppingCartSearchModel(CustomerShoppingCartSearchModel searchModel,
            Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare available shopping cart types (search shopping cart by default)
            searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
            _baseAdminModelFactory.PrepareShoppingCartTypes(searchModel.AvailableShoppingCartTypes, false);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer shopping cart list model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer shopping cart list model</returns>
        public virtual CustomerShoppingCartListModel PrepareCustomerShoppingCartListModel(CustomerShoppingCartSearchModel searchModel,
            Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer shopping cart
            var shoppingCart = customer.ShoppingCartItems.Where(item => item.ShoppingCartTypeId == searchModel.ShoppingCartTypeId).ToList();

            //prepare list model
            var model = new CustomerShoppingCartListModel
            {
                Data = shoppingCart.PaginationByRequestModel(searchModel).Select(item =>
                {
                    //fill in model values from the entity
                    var shoppingCartItemModel = new ShoppingCartItemModel
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.Product.Name,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(item.Product, item.AttributesXml),
                        UnitPrice = _priceFormatter
                            .FormatPrice(_taxService.GetProductPrice(item.Product, _priceCalculationService.GetUnitPrice(item), out var _)),
                        Total = _priceFormatter
                            .FormatPrice(_taxService.GetProductPrice(item.Product, _priceCalculationService.GetSubTotal(item), out _))
                    };

                    //convert dates to the user time
                    shoppingCartItemModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(item.UpdatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    shoppingCartItemModel.Store = _storeService.GetStoreById(item.StoreId)?.Name ?? "Unknown";

                    return shoppingCartItemModel;
                }),
                Total = shoppingCart.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare customer activity log search model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log search model</returns>
        public virtual CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer activity log list model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log list model</returns>
        public virtual CustomerActivityLogListModel PrepareCustomerActivityLogListModel(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer activity log
            var activityLog = _customerActivityService.GetAllActivities(customerId: customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CustomerActivityLogListModel
            {
                Data = activityLog.Select(logItem =>
                {
                    //fill in model values from the entity
                    var customerActivityLogModel = new CustomerActivityLogModel
                    {
                        Id = logItem.Id,
                        ActivityLogTypeName = logItem.ActivityLogType.Name,
                        Comment = logItem.Comment,
                        IpAddress = logItem.IpAddress
                    };

                    //convert dates to the user time
                    customerActivityLogModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return customerActivityLogModel;
                }),
                Total = activityLog.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions search model</returns>
        public virtual CustomerBackInStockSubscriptionSearchModel PrepareCustomerBackInStockSubscriptionSearchModel(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions list model</returns>
        public virtual CustomerBackInStockSubscriptionListModel PrepareCustomerBackInStockSubscriptionListModel(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get customer back in stock subscriptions
            var subscriptions = _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CustomerBackInStockSubscriptionListModel
            {
                Data = subscriptions.Select(subscription =>
                {
                    //fill in model values from the entity
                    var subscriptionModel = new CustomerBackInStockSubscriptionModel
                    {
                        Id = subscription.Id,
                        ProductId = subscription.ProductId
                    };

                    //convert dates to the user time
                    subscriptionModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(subscription.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    subscriptionModel.StoreName = _storeService.GetStoreById(subscription.StoreId)?.Name ?? "Unknown";
                    subscriptionModel.ProductName = subscription.Product?.Name ?? "Unknown";

                    return subscriptionModel;
                }),
                Total = subscriptions.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare online customer search model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>Online customer search model</returns>
        public virtual OnlineCustomerSearchModel PrepareOnlineCustomerSearchModel(OnlineCustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged online customer list model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>Online customer list model</returns>
        public virtual OnlineCustomerListModel PrepareOnlineCustomerListModel(OnlineCustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes);

            //get online customers
            var customers = _customerService.GetOnlineCustomers(customerRoleIds: null,
                 lastActivityFromUtc: lastActivityFrom,
                 pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new OnlineCustomerListModel
            {
                Data = customers.Select(customer =>
                {
                    //fill in model values from the entity
                    var customerModel = new OnlineCustomerModel
                    {
                        Id = customer.Id
                    };

                    //convert dates to the user time
                    customerModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    customerModel.CustomerInfo = customer.IsRegistered() 
                        ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    customerModel.LastIpAddress = _customerSettings.StoreIpAddresses
                        ? customer.LastIpAddress : _localizationService.GetResource("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled");
                    customerModel.Location = _geoLookupService.LookupCountryName(customer.LastIpAddress);
                    customerModel.LastVisitedPage = _customerSettings.StoreLastVisitedPage 
                        ? customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage) 
                        : _localizationService.GetResource("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled");

                    return customerModel;
                }),
                Total = customers.TotalCount
            };

            return model;
        }

        #endregion
    }
}