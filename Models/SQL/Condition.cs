using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class Condition
    {
        public Condition()
        {
            Asset = new HashSet<Asset>();
        }

        [Key]
        [Column("ConditionID")]
        public int ConditionId { get; set; }
        [Column("Condition")]
        [StringLength(20)]
        [Display(Name = "Condition")]
        public string Condition1 { get; set; }

        [InverseProperty("Condition")]
        public virtual ICollection<Asset> Asset { get; set; }
    }
}
