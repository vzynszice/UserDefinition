using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    [Table("MenuItem", Schema = "dbo")]
    public class MenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int? ViewTypeID { get; set; }

        public int? TargetFormID { get; set; }

        [StringLength(50)]
        public string ItemPath { get; set; }

        [StringLength(200)]
        public string DefaultDisplayText { get; set; }

        [StringLength(50)]
        public string IconPath { get; set; }

        [StringLength(500)]
        public string ContentParameter { get; set; }

        [StringLength(50)]
        public string ShortcutKey { get; set; }

        [StringLength(200)]
        public string DisplayText_EN { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RowGuid { get; set; }

        public byte? ShowInMode { get; set; }

        public int? ParentMenuId { get; set; }

        public string DisplayText_KO { get; set; } // nvarchar(max) alanları sınırlama olmaksızın tanımlanır

        public string MenuItemURL { get; set; } // nvarchar(max)

        public int? ViewOrder { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(30)]
        public string ImageClass { get; set; }

        public int? SequenceNumber { get; set; }
    }
}
