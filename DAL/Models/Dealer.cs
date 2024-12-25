using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("Dealer", Schema = "dbo")]
    public class Dealer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(5)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int RegionID { get; set; }

        public int? AuthorizedEmployeeID { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [Required]
        public DateTime OpeningDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        [StringLength(100)]
        public string AddressLine1 { get; set; }

        [StringLength(100)]
        public string? AddressLine2 { get; set; }

        [Required]
        public int CityID { get; set; }

        [Required]
        public int TownID { get; set; }

        [StringLength(6)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string FaxNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(13)]
        public string TaxNumber { get; set; }

        [StringLength(50)]
        public string TaxOffice { get; set; }

        public byte? AllowedOrderDays { get; set; }

        [StringLength(5)]
        public string AllowedOrderHoursBeginTime { get; set; }

        [StringLength(5)]
        public string AllowedOrderHoursEndTime { get; set; }

        [StringLength(50)]
        public string ClaimCode { get; set; }

        public int? ClaimCounter { get; set; }

        public decimal? WholeSaleRiskLimit { get; set; }

        public decimal? WholeSalesUsedRiskLimit { get; set; }

        public decimal? SparePartRiskLimit { get; set; }

        [Required]
        public bool CanOrder { get; set; }

        [Required]
        public bool IsRiskLimitControl { get; set; }

        [Required]
        public bool IsQuotaControl { get; set; }

        [Required]
        public int DealerClassID { get; set; }

        public decimal? SparePartUsedRiskLimit { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RowGuid { get; set; }

        [StringLength(20)]
        public string HostName { get; set; }

        [StringLength(50)]
        public string DealerServerIP { get; set; }

        public int? RealTimeServerPost { get; set; }

        [StringLength(50)]
        public string? DefaultDisplayText { get; set; }

        public bool? IsSparePartRiskLimitControl { get; set; }

        public int? DRPROV { get; set; }

        [StringLength(5)]
        public string? Culture { get; set; }

        public bool? IsCyprus { get; set; }

        public bool? IsEInvoiceDealer { get; set; }

        public int? EInvoiceCounter { get; set; }

        public bool? IsEArchiveDealer { get; set; }

        public int? ShipmentRegionID { get; set; }

        [StringLength(50)]
        public string WebAddress { get; set; }

        public int? OldDealerID { get; set; }

        public int? PrintCount { get; set; }

        public Guid? CrmID { get; set; }

        public bool? IsFactoryPdi { get; set; }

        [StringLength(100)]
        public string? EmailService { get; set; }

        [StringLength(50)]
        public string PartnerCode { get; set; }

        [StringLength(50)]
        public string SecretKey { get; set; }

        [StringLength(250)]
        public string? ShortCutUrl { get; set; }

    }
}
