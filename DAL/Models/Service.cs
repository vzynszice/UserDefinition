using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("Service", Schema = "dbo")]
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int DealerID { get; set; }

        [Required]
        public int ServiceTypeID { get; set; }

        [Required]
        public int RegionID { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string? AddressLine2 { get; set; }

        public int? CityID { get; set; }

        public int? TownID { get; set; }

        [StringLength(6)]
        public string PostalCode { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(20)]
        public string FaxNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public DateTime? OpeningDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        public bool? IsControledForMinimumOrder { get; set; }

        public decimal? LaborPrice { get; set; }

        public decimal? WarrantyDiscountPercent { get; set; }

        public decimal? WarrantyLaborPrice { get; set; }

        public DateTime? WarrantyLaborPriceLastDate { get; set; }

        public decimal? WarrantyLaborNextPrice { get; set; }

        public decimal? NegativeBalanceLimit { get; set; }

        public int? LastWorkOrderNumber { get; set; }

        [Required]
        public int CustomerID { get; set; }

        public int? CampaignExtraDay { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public int? ClaimReferenceNumberGenerationTypeID { get; set; }

        public int? OfferCirculationPeriod { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RowGuid { get; set; }

        [StringLength(20)]
        public string HostName { get; set; }

        [Required]
        public int SparePartReturnSearchPurchaseInvoiceDayCount { get; set; }

        [StringLength(50)]
        public string? DefaultDisplayText { get; set; }

        public bool? IsInvolvedInBalanceProcess { get; set; }

        public bool? IsInBalancedOrderSuppyRiskLimit { get; set; }

        public byte? ActiveWorkOrderReservationStrategy { get; set; }

        public byte? ActiveRetailSalesInvoiceReservationStrategy { get; set; }

        public int? SecondRegionID { get; set; }

        [StringLength(5)]
        public string? Culture { get; set; }

        public bool? StockTakingIncreaseLimitControl { get; set; }

        public bool? BeginningDateControl { get; set; }

        public int? CampaignFixMenuMonthPeriod { get; set; }

        public int? CampaignStrategicFleetMonthPeriod { get; set; }

        public bool? IsVirtualBlueCardGranted { get; set; }

        public Guid? ServiceAppKey { get; set; }

        public int? OldServiceID { get; set; }

        public bool? AllSparePartSalesIncludedForSmart { get; set; }

        public string MapUrl { get; set; } // nvarchar(max) alanı sınırsız uzunluk için string olarak bırakıldı

        [Required]
        public bool IsAuthorizedService { get; set; }

        [StringLength(50)]
        public string Latitude { get; set; }

        [StringLength(50)]
        public string Longitude { get; set; }
    }

}
