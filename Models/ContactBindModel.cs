using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Models
{
    public class ContactBindModel
    {
        [Display(Name = "User ID")]
        [Required, MaxLength(255)]
        [MinLength(1, ErrorMessage = "Please enter at least 1 character")]
        public string UserID { get; set; }        
    }
}
