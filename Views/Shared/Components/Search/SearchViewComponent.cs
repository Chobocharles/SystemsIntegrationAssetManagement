using Asset_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Views.Shared.Components.Search
{
    public class SearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new SearchBindModel();
            return View(model);
        }
    }
}
