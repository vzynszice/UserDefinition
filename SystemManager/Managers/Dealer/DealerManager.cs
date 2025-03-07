using DAL.Exceptions;
using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;
using SystemManager.Abstractions.Dealer;

namespace SystemManager.Managers.Dealer
{
    public class DealerManager : BaseManager, IDealerManager
    {
        private readonly IDealerService _dealerService;

        public DealerManager(
            IRepositoryManager repositoryManager,
            IDealerService dealerService,
            ILogger<DealerManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _dealerService = dealerService;
        }

        public async Task<object> GetDealers(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Dealers List");
            return await _dealerService.GetDealers(loadOptions);
        }

        public async Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Dealers For Dropdown");
            return await _dealerService.GetDealersForDropdown(loadOptions);
        }

        public async Task<DealerModel> GetByIdAsync(int id)
        {
            await LogActionAsync("Get Dealer By Id", $"DealerId: {id}");
            var dealer = await _dealerService.GetByIdAsync(id);

            if (dealer == null)
            {
                throw new NotFoundException($"Dealer with ID {id} not found");
            }

            return dealer;
        }

        public async Task CreateAsync(DealerModel dealer)
        {
            await LogActionAsync("Create Dealer", $"Name: {dealer.Name}");

            if (string.IsNullOrEmpty(dealer.Name))
            {
                throw new ValidationException("Dealer name cannot be empty");
            }

            var count = await _dealerService.GetRepository().GetQueryable().CountAsync();
            var lastDealer = await _dealerService.GetRepository().GetQueryable()
                .OrderByDescending(d => d.Code)
                .FirstOrDefaultAsync();

            dealer.CreatedOn = DateTime.Now;
            dealer.RowGuid = Guid.NewGuid();
            dealer.Code = $"DL0{count + 1}";
            dealer.IsLocked = false;
            dealer.CityID = lastDealer?.CityID + 1 ?? 1;
            dealer.TownID = lastDealer?.TownID + 1 ?? 1;
            dealer.RegionID = 2;
            dealer.TaxNumber = $"TXN{count + 1}";
            dealer.PhoneNumber = $"555-01{count + 1}";
            dealer.FaxNumber = $"555-02{count + 1}";
            dealer.TaxOffice = $"TaxOffice{count + 1}";
            dealer.CanOrder = true;
            dealer.IsRiskLimitControl = true;
            dealer.IsQuotaControl = true;
            dealer.DealerClassID = lastDealer?.DealerClassID + 1 ?? 1;
            dealer.ClaimCode = $"CLM0{count + 1}";
            dealer.CreatedBy = CurrentUserService.GetCurrentUserId();
            dealer.HostName = $"Host{count + 1}";
            dealer.DealerServerIP = $"192.168.1.{count + 1}";
            dealer.RealTimeServerPost = lastDealer?.RealTimeServerPost + 1 ?? 1;
            dealer.IsSparePartRiskLimitControl = true;
            dealer.IsCyprus = false;
            dealer.IsEInvoiceDealer = true;
            dealer.EInvoiceCounter = lastDealer?.EInvoiceCounter + 5 ?? 5;
            dealer.IsEArchiveDealer = false;
            dealer.ShipmentRegionID = 1;
            dealer.PrintCount = 0;
            dealer.IsFactoryPdi = false;
            dealer.PartnerCode = $"PC0{count + 1}";
            dealer.SecretKey = $"Secret0{count + 1}";
            dealer.AllowedOrderDays = 5;
            dealer.AllowedOrderHoursBeginTime = "08:00";
            dealer.AllowedOrderHoursEndTime = "17:00";
            dealer.ClaimCounter = count + 1;
            dealer.WholeSaleRiskLimit = 1000;
            dealer.WholeSalesUsedRiskLimit = 500;
            dealer.SparePartRiskLimit = 1000;
            dealer.SparePartUsedRiskLimit = 2500;

            await _dealerService.CreateAsync(dealer);
        }

        public async Task UpdateAsync(DealerModel dealer)
        {
            await LogActionAsync("Update Dealer", $"DealerId: {dealer.ID}");

            if (string.IsNullOrEmpty(dealer.Name))
            {
                throw new ValidationException("Dealer name cannot be empty");
            }

            var existingDealer = await GetByIdAsync(dealer.ID);
            dealer.ModifiedOn = DateTime.Now;
            dealer.ModifiedBy = CurrentUserService.GetCurrentUserId();
            dealer.RowGuid = existingDealer.RowGuid;
            dealer.CreatedOn = existingDealer.CreatedOn;
            dealer.CreatedBy = existingDealer.CreatedBy;

            await _dealerService.UpdateAsync(dealer);
        }

        public async Task DeleteAsync(int id)
        {
            await LogActionAsync("Delete Dealer", $"DealerId: {id}");
            await _dealerService.DeleteAsync(id);
            await RepositoryManager.SaveChangesAsync();
            await RepositoryManager.ResetIdentityAsync("Dealer");
        }
    }
}
