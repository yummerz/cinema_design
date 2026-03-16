using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLoginApp.Models
{
    public class LostItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LocationLost { get; set; } = string.Empty;
        public string DateReported { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
