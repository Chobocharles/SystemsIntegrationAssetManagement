using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Asset_Management.Models.SQL
{
    public partial class Contact
    {
        public Contact()
        {
            Asset = new HashSet<Asset>();
        }

        [Key]
        [Column("ContactID")]
        public int ContactId { get; set; }
        [StringLength(20)]
        public string Company { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string EmailAddress { get; set; }
        [StringLength(255)]
        public string JobTitle { get; set; }
        [StringLength(20)]
        public string BusinessPhone { get; set; }
        public short? Extension { get; set; }
        [StringLength(20)]
        public string HomePhone { get; set; }
        [StringLength(20)]
        public string MobilePhone { get; set; }
        [StringLength(20)]
        public string FaxNumber { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(20)]
        public string City { get; set; }
        [StringLength(20)]
        public string State { get; set; }
        [StringLength(20)]
        public string Province { get; set; }
        [StringLength(20)]
        public string ZipCode { get; set; }
        [StringLength(20)]
        public string Country { get; set; }
        [StringLength(255)]
        public string WebPage { get; set; }
        public string Notes { get; set; }

        [InverseProperty("Contact")]
        public virtual ICollection<Asset> Asset { get; set; }
    }
}
