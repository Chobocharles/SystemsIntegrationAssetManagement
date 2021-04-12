using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Models
{
    public class SearchBindModel
    {
        [BindProperty]
        [Display(Name = "Search Value")]
        [Required, MaxLength(255)]
        [MinLength(1, ErrorMessage = "Please enter at least 1 character")]
        public string Search { get; set; }

        [BindProperty]
        [Display(Name = "Type")]
        public int SearchType { get; set; }

        public List<SelectListItem> SearchTypes { get; } = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Contacts", Value = "0" },
                new SelectListItem() { Text = "Assets", Value = "1" },
                new SelectListItem() { Text = "Asset Types", Value = "2" },
                new SelectListItem() { Text = "Locations", Value = "3" },
                new SelectListItem() { Text = "Service Records", Value = "4" }
            };
    }
}
