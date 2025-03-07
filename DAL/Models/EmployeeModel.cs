using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("Employee", Schema = "dbo")]
    public class EmployeeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int DealerID { get; set; }

        [StringLength(160)]
        public string Name { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public int TitleID { get; set; }

        public bool? IsTechnician { get; set; }

        [StringLength(16)]
        public string TCIDNO { get; set; }

        [Required]
        public Guid CrmID { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public decimal? HourlyLaborCost { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RowGuid { get; set; }

        public bool? IsRegionManager { get; set; }

        [StringLength(50)]
        public string UserCode { get; set; }

        public int? ServiceID { get; set; }

        [StringLength(5)]
        public string? Culture { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? SparePartMaxDiscountPercent { get; set; }

        public decimal? LaborMaxDiscountPercent { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(60)]
        public string? GsmNumber { get; set; }

        [StringLength(60)]
        public string? Telephone { get; set; }

        [StringLength(60)]
        public string? ExtensionNumber { get; set; }

    }
}
