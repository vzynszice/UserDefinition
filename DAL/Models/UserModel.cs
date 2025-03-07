using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("User", Schema = "dbo")]
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(250)]
        public string? Password { get; set; } 

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Surname { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string? PasswordQuestion { get; set; }

        [StringLength(250)]
        public string? PasswordAnswer { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsLockedOut { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public DateTime? LastLockOutDate { get; set; }

        public int? FailedPasswordAttemptCount { get; set; }

        public int? FailedPasswordAnswerAttemptCount { get; set; }

        [StringLength(250)]
        public string? Comment { get; set; }

        public int? ServiceID { get; set; }

        public int? DefaultMenuItemID { get; set; }

        public int? LanguageID { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RowGuid { get; set; }

        public int? DealerID { get; set; }

        [StringLength(100)]
        public string? ActiveThemeName { get; set; }

        public bool? DoNotApplyCrmRole { get; set; }

        [StringLength(50)]
        public string? LocalVersion { get; set; }

        [StringLength(255)]
        public string? MachineName { get; set; }

        [StringLength(50)]
        public string? IPAddress { get; set; }

        [StringLength(5)]
        public string? Culture { get; set; }

        [StringLength(200)]
        public string? LastSessionID { get; set; }

        public byte? Active { get; set; }

        [StringLength(100)]
        public string? UserTheme { get; set; }

        public byte? IsSAPUser { get; set; }

        public bool? IsUserForSAP { get; set; }

        public bool? IsUserForDMSWeb { get; set; }

        public bool? IsRequiredMfa { get; set; }

        public DateTime? PasswordExpireDate { get; set; }

        public bool? IsPasswordTemporary { get; set; }
    }
}
