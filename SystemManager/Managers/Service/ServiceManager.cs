using DAL.Exceptions;
using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Service;

namespace SystemManager.Managers.Service
{
    public class ServiceManager : BaseManager, IServiceManager
    {
        private readonly IServiceService _serviceService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IServiceService serviceService,
            ILogger<ServiceManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _serviceService = serviceService;
        }

        public async Task<object> GetServices(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Services List");
            return await _serviceService.GetServices(loadOptions);
        }

        public async Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Services For Dropdown");
            return await _serviceService.GetServicesForDropdown(loadOptions);
        }

        public async Task<ServiceModel> GetByIdAsync(int id)
        {
            await LogActionAsync("Get Service By Id", $"ServiceId: {id}");
            var service = await _serviceService.GetByIdAsync(id);

            if (service == null)
            {
                throw new NotFoundException($"Service with ID {id} not found");
            }

            return service;
        }

        public async Task CreateAsync(ServiceModel service)
        {
            await LogActionAsync("Create Service", $"Name: {service.Name}");

            if (string.IsNullOrEmpty(service.Name))
            {
                throw new ValidationException("Service name cannot be empty");
            }

            var count = await _serviceService.GetRepository().GetQueryable().CountAsync();
            var lastService = await _serviceService.GetRepository().GetQueryable()
                .OrderByDescending(s => s.Code)
                .FirstOrDefaultAsync();

            service.ServiceTypeID = 3;
            service.RegionID = 2;
            service.Code = $"SV0{count + 1}";
            service.IsLocked = false;
            service.CityID = lastService?.CityID + 1 ?? 1;
            service.TownID = lastService?.TownID + 1 ?? 1;
            service.PhoneNumber = $"555-03{count + 1}";
            service.FaxNumber = $"555-04{count + 1}";
            service.IsControledForMinimumOrder = false;
            service.LaborPrice = 100;
            service.WarrantyDiscountPercent = lastService?.WarrantyDiscountPercent + 1 ?? 1;
            service.WarrantyLaborPrice = lastService?.WarrantyLaborPrice + 10 ?? 100;
            service.WarrantyLaborPriceLastDate = DateTime.Now;
            service.WarrantyLaborNextPrice = lastService?.WarrantyLaborNextPrice + 10 ?? 100;
            service.NegativeBalanceLimit = lastService?.NegativeBalanceLimit + 1000 ?? 1000;
            service.LastWorkOrderNumber = lastService?.LastWorkOrderNumber + 50 ?? 50;
            service.CustomerID = lastService?.CustomerID + 1 ?? 1;
            service.CampaignExtraDay = lastService?.CampaignExtraDay + 5 ?? 5;
            service.CreatedOn = DateTime.Now;
            service.CreatedBy = CurrentUserService.GetCurrentUserId();
            service.ClaimReferenceNumberGenerationTypeID = 1;
            service.OfferCirculationPeriod = lastService?.OfferCirculationPeriod + 15 ?? 15;
            service.RowGuid = Guid.NewGuid();
            service.HostName = $"HostService{count + 1}";
            service.SparePartReturnSearchPurchaseInvoiceDayCount = lastService?.SparePartReturnSearchPurchaseInvoiceDayCount + 15 ?? 15;
            service.IsInvolvedInBalanceProcess = true;
            service.IsInBalancedOrderSuppyRiskLimit = true;
            service.ActiveWorkOrderReservationStrategy = (byte?)(lastService?.ActiveWorkOrderReservationStrategy + 1 ?? 1);
            service.ActiveRetailSalesInvoiceReservationStrategy = (byte?)(lastService?.ActiveRetailSalesInvoiceReservationStrategy + 1 ?? 1);
            service.SecondRegionID = lastService?.SecondRegionID + 1 ?? 1;
            service.StockTakingIncreaseLimitControl = true;
            service.BeginningDateControl = true;
            service.CampaignFixMenuMonthPeriod = lastService?.CampaignFixMenuMonthPeriod + 1 ?? 1;
            service.CampaignStrategicFleetMonthPeriod = lastService?.CampaignFixMenuMonthPeriod + 2 ?? 2;
            service.IsVirtualBlueCardGranted = false;
            service.AllSparePartSalesIncludedForSmart = true;
            service.IsAuthorizedService = true;
            service.Latitude = "39.5834° N";
            service.Longitude = "32.1624° W";

            await _serviceService.CreateAsync(service);
        }

        public async Task UpdateAsync(ServiceModel service)
        {
            await LogActionAsync("Update Service", $"ServiceId: {service.ID}");

            if (string.IsNullOrEmpty(service.Name))
            {
                throw new ValidationException("Service name cannot be empty");
            }

            var existingService = await GetByIdAsync(service.ID);
            service.ModifiedOn = DateTime.Now;
            service.ModifiedBy = CurrentUserService.GetCurrentUserId();
            service.CreatedOn = existingService.CreatedOn;
            service.CreatedBy = existingService.CreatedBy;
            service.RowGuid = existingService.RowGuid;

            await _serviceService.UpdateAsync(service);
        }

        public async Task DeleteAsync(int id)
        {
            await LogActionAsync("Delete Service", $"ServiceId: {id}");

            var service = await GetByIdAsync(id);

            if (service.IsLocked)
            {
                throw new BusinessException("Locked services cannot be deleted");
            }

            await _serviceService.DeleteAsync(id);
            await RepositoryManager.SaveChangesAsync();
            await RepositoryManager.ResetIdentityAsync("Service");
        }
    }
}
