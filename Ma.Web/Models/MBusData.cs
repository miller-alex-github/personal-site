using System.ComponentModel.DataAnnotations;

namespace Ma.Web
{
    public class MBusData
    {
        [Required]                
        public string InputText { get; set; }
        public string OutputText { get; internal set; }
        public string Error { get; internal set; }
        public bool IsExpert { get; set; }
    }
}
