using Asset_Management.Models;
using Asset_Management.Models.SQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AssetContext _context;

        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, AssetContext context, IConfiguration configuration)
        {
            _context = context;
        
            _logger = logger;

            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Assets()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("Search,SearchType")] SearchBindModel searchData)
        {
            var searchResults = new List<SearchResponse>();

            if (ModelState.IsValid)
            {
                switch (searchData.SearchType)
                {
                    case 0: //Contact
                        var contacts = _context.Contact.Where(a => a.Address.Contains(searchData.Search) ||
                                a.BusinessPhone.Contains(searchData.Search) ||
                                a.City.Contains(searchData.Search) ||
                                a.Company.Contains(searchData.Search) ||
                                a.Country.Contains(searchData.Search) ||
                                a.DisplayName.Contains(searchData.Search) ||
                                a.EmailAddress.Contains(searchData.Search) ||
                                a.FirstName.Contains(searchData.Search) ||
                                a.JobTitle.Contains(searchData.Search) ||
                                a.LastName.Contains(searchData.Search) ||
                                a.Province.Contains(searchData.Search) ||
                                a.State.Contains(searchData.Search) ||
                                a.ZipCode.Contains(searchData.Search) ||
                                a.ContactId.Equals(searchData.Search)
                            )
                            .Select(e => new SearchResponse
                            {
                                Display = e.DisplayName,
                                ID = e.ContactId,
                                Path = "/Contacts/Details/" + e.ContactId
                            })
                            .ToList();

                        searchResults.AddRange(contacts);
                        break;
                    case 1: //Assets
                        var assets = _context.Asset.Where(a => a.AssetId.Equals(searchData.Search) || 
                                a.AssetTagNumber.Equals(searchData.Search) ||
                                a.Brand.Contains(searchData.Search) ||
                                a.Condition.Condition1.Contains(searchData.Search) ||                           
                                a.Contact.DisplayName.Contains(searchData.Search) ||
                                a.DeviceId.Contains(searchData.Search) ||
                                a.Location.Location1.Contains(searchData.Search) ||
                                a.Model.Contains(searchData.Search) ||
                                a.SerialNumber.Contains(searchData.Search) ||
                                a.ServiceTag.Contains(searchData.Search) ||
                                a.WorkCenter.Contains(searchData.Search) ||
                                a.Contact.DisplayName.Contains(searchData.Search)
                            )
                            .Select(e => new SearchResponse
                            {
                                Display = e.Brand + " " + e.Model + " | " + e.AssetTagNumber,
                                ID = e.AssetId,
                                Path = "/Assets/Details/" + e.AssetId
                            })
                            .ToList();
                        searchResults.AddRange(assets);
                        break;
                    case 2: //Asset Types
                        var assetTypes = _context.AssetType.Where(a => a.AssetType1.Contains(searchData.Search) ||
                                a.AssetTypeId.Equals(searchData.Search)
                            )
                            .Select(e => new SearchResponse
                            {
                                Display = e.AssetType1,
                                ID = e.AssetTypeId,
                                Path = "/AssetTypes/Details/" + e.AssetTypeId
                            })
                            .ToList();
                        searchResults.AddRange(assetTypes);
                        break;
                    case 3: //Locations
                        var locations = _context.Location.Where(a => a.Location1.Contains(searchData.Search) ||
                                a.LocationId.Equals(searchData.Search)
                            )
                            .Select(e => new SearchResponse
                            {
                                Display = e.Location1,
                                ID = e.LocationId,
                                Path = "/Locations/Details/" + e.LocationId
                            })
                            .ToList();
                        searchResults.AddRange(locations);
                        break;
                    case 4: //Service Records
                        var serviceRecords = _context.ServiceRecord.Where(a => a.AssetId.Equals(searchData.Search) ||
                                a.DeviceName.Contains(searchData.Search) ||
                                a.PartsReplaced.Contains(searchData.Search) ||
                                a.Problem.Equals(searchData.Search) ||
                                a.ServiceRecordId.Equals(searchData.Search)
                            )
                            .Select(e => new SearchResponse
                            {
                                Display = e.AssetId + " Service",
                                ID = e.ServiceRecordId,
                                Path = "/ServiceRecords/Details/" + e.ServiceRecordId
                            })
                            .ToList();
                        searchResults.AddRange(serviceRecords);
                        break;
                    default:
                        break;
                }

                //var contacts = _context.Contact.Where(x => x.)
                return Ok(searchResults);
            }
            return BadRequest(searchResults);
        }
    }
}
