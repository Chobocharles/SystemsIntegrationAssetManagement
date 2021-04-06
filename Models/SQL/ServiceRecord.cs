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
        public int ServiceRecordId { get; set; }
        [Column("AssetID")]
        public int AssetId { get; set; }
        [StringLength(255)]
        public string Problem { get; set; }
        [StringLength(255)]
        public string PartsReplaced { get; set; }
        public string DescriptionOfWork { get; set; }
        [StringLength(20)]
        public string DeviceName { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ServiceDate { get; set; }
    }
}
