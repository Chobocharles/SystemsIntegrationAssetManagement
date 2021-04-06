using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class AssetType
    {
        public AssetType()
        {
            Asset = new HashSet<Asset>();
        }

        [Key]
        [Column("AssetTypeID")]
        public int AssetTypeId { get; set; }
        [Column("AssetType")]
        [StringLength(20)]
        public string AssetType1 { get; set; }

        [InverseProperty("AssetType")]
        public virtual ICollection<Asset> Asset { get; set; }
    }
}
