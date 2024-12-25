using BLL.interfaces;
using DAL.db;
using DAL.interfaces;
using DAL.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ServiceService : GenericService<Service>, IServiceService
    {
        
        public ServiceService(IUnitOfWork unitOfWork) :base(unitOfWork)
        {
           
        }
        protected override IGenericRepository<Service> GetRepository()
        {
            return _unitOfWork.Services;
        }

        public async Task<object> GetServicesForDropdown(DataSourceLoadOptions loadOptions)
        {
            var services = GetRepository().GetQueryable()
            .Select(s => new
            {
                id = s.ID,
                name = s.Name,
                dealerID = s.DealerID
            });

            return await DataSourceLoader.LoadAsync(services, loadOptions);
        }

        public async Task<object> GetServices(DataSourceLoadOptions loadOptions)
        {
            var services = GetRepository().GetQueryable().Select(s => new
            {
                s.ID,
                s.Name,
                s.PhoneNumber,
                s.MapUrl,
                s.Email,
                s.OpeningDate,
                s.AddressLine1,
                s.PostalCode,
                s.DealerID
            });

            return await DataSourceLoader.LoadAsync(services, loadOptions);
        }
        public override async Task CreateAsync(Service entity)
        {
            var count = GetRepository().GetQueryable().Count();
            var lastService = GetRepository().GetQueryable()
                .OrderByDescending(s => s.Code)
                .FirstOrDefault();

            entity.ServiceTypeID = 3;
            entity.RegionID = 2;
            entity.Code = $"SV0{count + 1}";
            entity.IsLocked = false;
            entity.CityID = lastService?.CityID + 1 ?? 1;
            entity.TownID = lastService?.TownID + 1 ?? 1;
            entity.PhoneNumber = $"555-03{count + 1}";
            entity.FaxNumber = $"555-04{count + 1}";
            entity.IsControledForMinimumOrder = false;
            entity.LaborPrice = 100;
            entity.WarrantyDiscountPercent = lastService?.WarrantyDiscountPercent + 1 ?? 1;
            entity.WarrantyLaborPrice = lastService?.WarrantyLaborPrice + 10 ?? 100;
            entity.WarrantyLaborPriceLastDate = DateTime.Now;
            entity.WarrantyLaborNextPrice = lastService?.WarrantyLaborNextPrice + 10 ?? 100;
            entity.NegativeBalanceLimit = lastService?.NegativeBalanceLimit + 1000 ?? 1000;
            entity.LastWorkOrderNumber = lastService?.LastWorkOrderNumber + 50 ?? 50;
            entity.CustomerID = lastService?.CustomerID + 1 ?? 1;
            entity.CampaignExtraDay = lastService?.CampaignExtraDay + 5 ?? 5;
            entity.CreatedOn = DateTime.Now;
            entity.CreatedBy = lastService?.CreatedBy + 1 ?? 1;
            entity.ClaimReferenceNumberGenerationTypeID = 1;
            entity.OfferCirculationPeriod = lastService?.OfferCirculationPeriod + 15 ?? 15;
            entity.RowGuid = Guid.NewGuid();
            entity.HostName = $"HostService{count + 1}";
            entity.SparePartReturnSearchPurchaseInvoiceDayCount = lastService?.SparePartReturnSearchPurchaseInvoiceDayCount + 15 ?? 15;
            entity.IsInvolvedInBalanceProcess = true;
            entity.IsInBalancedOrderSuppyRiskLimit = true;
            entity.ActiveWorkOrderReservationStrategy = (byte?)(lastService?.ActiveWorkOrderReservationStrategy + 1 ?? 1);
            entity.ActiveRetailSalesInvoiceReservationStrategy = (byte?)(lastService?.ActiveRetailSalesInvoiceReservationStrategy + 1 ?? 1);
            entity.SecondRegionID = lastService?.SecondRegionID + 1 ?? 1;
            entity.StockTakingIncreaseLimitControl = true;
            entity.BeginningDateControl = true;
            entity.CampaignFixMenuMonthPeriod = lastService?.CampaignFixMenuMonthPeriod + 1 ?? 1;
            entity.CampaignStrategicFleetMonthPeriod = lastService?.CampaignFixMenuMonthPeriod + 2 ?? 2;
            entity.IsVirtualBlueCardGranted = false;
            entity.AllSparePartSalesIncludedForSmart = true;
            entity.IsAuthorizedService = true;
            entity.Latitude = "39.5834° N";
            entity.Longitude = "32.1624° W";

            await base.CreateAsync(entity);
        }
        public override async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.ResetIdentityAsync("Service");
        }
    }
}
