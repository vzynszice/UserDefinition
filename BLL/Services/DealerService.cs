using BLL.interfaces;
using DAL.db;
using DAL.interfaces;
using DAL.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class DealerService : GenericService<Dealer>, IDealerService
    {

        public DealerService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
           
        }
        protected override IGenericRepository<Dealer> GetRepository()
        {
            return _unitOfWork.Dealers;
        }
        public async Task<object> GetDealers(DataSourceLoadOptions loadOptions)
        {
            var dealers = GetRepository().GetQueryable()
                          .Select(d => new
                          {
                            d.ID,
                            d.Name,
                            d.PhoneNumber,
                            d.WebAddress,
                            d.Email,
                            d.OpeningDate,
                            d.AddressLine1,
                            d.PostalCode
                          });

            return await DataSourceLoader.LoadAsync(dealers, loadOptions);
        }

        public async Task<object> GetDealersForDropdown(DataSourceLoadOptions loadOptions)
        {
            var dealers = GetRepository().GetQueryable()
                          .Select(d => new
                          {
                              d.ID,
                              d.Name
                          });

            return await DataSourceLoader.LoadAsync(dealers, loadOptions);
        }

        public override async Task CreateAsync(Dealer entity)
        {
            var count = GetRepository().GetQueryable().Count();
            var lastDealer = await GetRepository().GetQueryable()
                .OrderByDescending(d => d.Code)
                .FirstOrDefaultAsync();

            entity.CreatedOn = DateTime.Now;
            entity.RowGuid = Guid.NewGuid();
            entity.Code = $"DL0{count + 1}";
            entity.IsLocked = false;
            entity.CityID = lastDealer != null ? lastDealer.CityID + 1 : 1;
            entity.TownID = lastDealer != null ? lastDealer.TownID + 1 : 1;
            entity.RegionID = 2;
            entity.TaxNumber = "TXN1313141516";
            entity.PhoneNumber = $"555-01{count + 1}";
            entity.FaxNumber = $"555-02{count + 1}";
            entity.TaxOffice = $"TaxOffice{count + 1}";
            entity.CanOrder = true;
            entity.IsRiskLimitControl = true;
            entity.IsQuotaControl = true;
            entity.DealerClassID = lastDealer != null ? lastDealer.DealerClassID + 1 : 1;
            entity.ClaimCode = $"CLM0{count + 1}";
            entity.CreatedBy = count + 1;
            entity.HostName = $"Host{count + 1}";
            entity.DealerServerIP = $"192.168.1.{count + 1}";
            entity.RealTimeServerPost = lastDealer != null ? lastDealer.RealTimeServerPost + 1 : 1;
            entity.IsSparePartRiskLimitControl = true;
            entity.IsCyprus = false;
            entity.IsEInvoiceDealer = true;
            entity.EInvoiceCounter = lastDealer != null ? lastDealer.EInvoiceCounter + 5 : 5;
            entity.IsEArchiveDealer = false;
            entity.ShipmentRegionID = 1;
            entity.PrintCount = 0;
            entity.IsFactoryPdi = false;
            entity.PartnerCode = $"PC0{count + 1}";
            entity.SecretKey = $"Secret0{count + 1}";
            entity.AllowedOrderDays = 5;
            entity.AllowedOrderHoursBeginTime = "08:00";
            entity.AllowedOrderHoursEndTime = "17:00";
            entity.ClaimCounter = count + 1;
            entity.WholeSaleRiskLimit = 1000;
            entity.WholeSalesUsedRiskLimit = 500;
            entity.SparePartRiskLimit = 1000;
            entity.SparePartUsedRiskLimit = 2500;

            await base.CreateAsync(entity);
        }
        public override async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.ResetIdentityAsync("Dealer");
        }

    }

}
