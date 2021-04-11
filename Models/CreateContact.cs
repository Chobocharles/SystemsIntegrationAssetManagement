using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asset_Management.Models.SQL;

namespace Asset_Management.Models
{
    public class CreateContact
    {
        public Contact Contact { get; set; }
        public ContactBindModel Search { get; set; }
    }
}
