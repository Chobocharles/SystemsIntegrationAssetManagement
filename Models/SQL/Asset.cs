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
        [Key]
        [Column("AssetID")]
        public int AssetId { get; set; }
        [Column("AssetTypeID")]
        public int AssetTypeId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Column("ConditionID")]
        public int ConditionId { get; set; }
        [Column(TypeName = "date")]
        public DateTime? AcquiredDate { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        public decimal PurchasePrice { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        public decimal CurrentValue { get; set; }
        [Column("LocationID")]
        public int LocationId { get; set; }
        [StringLength(20)]
        public string Brand { get; set; }
        [StringLength(255)]
        public string Model { get; set; }
        public string Comments { get; set; }
        [Column("ContactID")]
        public int ContactId { get; set; }
        [Column(TypeName = "date")]
        public DateTime? RetiredDate { get; set; }
        public int? AssetTagNumber { get; set; }
        [StringLength(255)]
        public string SerialNumber { get; set; }
        [StringLength(255)]
        public string ServiceTag { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WarrantyExpires { get; set; }
        [Column("DeviceID")]
        [StringLength(255)]
        public string DeviceId { get; set; }
        public bool Verified { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateVerified { get; set; }
        [StringLength(255)]
        public string WorkCenter { get; set; }

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
    }
}
