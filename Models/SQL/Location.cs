using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class Location
    {
        public Location()
        {
            Asset = new HashSet<Asset>();
        }

        [Key]
        [Column("LocationID")]
        public int LocationId { get; set; }
        [Column("Location")]
        public string Location1 { get; set; }

        [InverseProperty("Location")]
        public virtual ICollection<Asset> Asset { get; set; }
    }
}
