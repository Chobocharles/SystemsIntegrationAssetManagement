using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Models
{    internal interface IDefaultResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
        string Execution_time { get; set; }
        string Execution_duration { get; set; }
    }
}
