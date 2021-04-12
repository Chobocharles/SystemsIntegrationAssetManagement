using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class ServiceRecord
    {
        [Key]
        [Column("ServiceRecordID")]
        [Display(Name = "Service Record ID")]
        public int ServiceRecordId { get; set; }
        [Column("AssetID")]
        [Display(Name = "Asset ID")]
        public int AssetId { get; set; }
        [StringLength(255)]
        public string Problem { get; set; }
        [StringLength(255)]
        [Display(Name = "Parts Replaced")]
        public string PartsReplaced { get; set; }
        [Display(Name = "Description of Work")]
        public string DescriptionOfWork { get; set; }
        [StringLength(20)]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Service Date")]
        public DateTime? ServiceDate { get; set; }

        [ForeignKey(nameof(AssetId))]
        [InverseProperty("ServiceRecord")]
        public virtual Asset Asset { get; set; }
    }
}
