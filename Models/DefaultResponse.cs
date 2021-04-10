using System.ComponentModel.DataAnnotations;

namespace Asset_Management.Models
{
    public class DefaultResponse : IDefaultResponse
    {
        [Required]
        public bool Success { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string Execution_time { get; set; }

        [Required]
        public string Execution_duration { get; set; }
    }
}
