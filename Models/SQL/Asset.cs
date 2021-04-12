using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class Asset
    {
        public Asset()
        {
            ServiceRecord = new HashSet<ServiceRecord>();
        }

        [Key]
        [Column("AssetID")]
        public int AssetId { get; set; }
        [Required]
        [Column("AssetTypeID")]
        [Display(Name = "Asset Type*")]
        public int AssetTypeId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Required]
        [Column("ConditionID")]
        [Display(Name = "Condition*")]
        public int ConditionId { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Aquired Date")]
        public DateTime? AcquiredDate { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        [Display(Name = "Current Value")]
        public decimal CurrentValue { get; set; }
        [Required]
        [Column("LocationID")]
        [Display(Name = "Location*")]
        public int LocationId { get; set; }
        [StringLength(20)]
        public string Brand { get; set; }
        [StringLength(255)]
        public string Model { get; set; }
        public string Comments { get; set; }
        [Required]
        [Column("ContactID")]
        [Display(Name = "Contact*")]
        public int ContactId { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Retired Date")]
        public DateTime? RetiredDate { get; set; }
        [Display(Name = "Asset Tag Number")]
        public int? AssetTagNumber { get; set; }
        [StringLength(255)]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }
        [StringLength(255)]
        [Display(Name = "Service Tag")]
        public string ServiceTag { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Warranty Expiration Date")]
        public DateTime? WarrantyExpires { get; set; }
        [Column("DeviceID")]
        [StringLength(255)]
        [Display(Name = "Device ID")]
        public string DeviceId { get; set; }
        public bool Verified { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Verified Date")]
        public DateTime? DateVerified { get; set; }
        [StringLength(255)]
        [Display(Name = "Work Center")]
        public string WorkCenter { get; set; }
        [Display(Name = "Device Image")]
        public byte[] Picture { get; set; }
        public string PictureSourceFileName { get; set; }
        public string PictureContentType { get; set; }

        [ForeignKey(nameof(AssetTypeId))]
        [InverseProperty("Asset")]
        public virtual AssetType AssetType { get; set; }
        [ForeignKey(nameof(ConditionId))]
        [InverseProperty("Asset")]
        public virtual Condition Condition { get; set; }
        [ForeignKey(nameof(ContactId))]
        [InverseProperty("Asset")]
        public virtual Contact Contact { get; set; }
        [ForeignKey(nameof(LocationId))]
        [InverseProperty("Asset")]
        public virtual Location Location { get; set; }
        [InverseProperty("Asset")]
        public virtual ICollection<ServiceRecord> ServiceRecord { get; set; }
    }
}
