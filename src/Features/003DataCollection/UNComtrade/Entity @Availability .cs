using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.UNComtrade
{
    public class DataAvailability
    {
        public string? type { get; set; }
        public string? freq { get; set; }
        public string? px { get; set; }
        public string? r { get; set; }
        public string? rDesc { get; set; }
        public string? ps { get; set; }
        public int TotalRecords { get; set; }
        public int isOriginal { get; set; }
        public DateTime publicationDate { get; set; }
        public int isPartnerDetail { get; set; }
    }
}
