using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.common
{
    public class auditedColumn
    {
        public int? userid { get; set; }
        public int? userby { get; set; }
        //public int? userid { get; set; }
        public DateTime? entrydate { get; set; }
        public string entrydatestr { get; set; }
        public string entryby { get; set; }
        public string modifyby { get; set; }
        public DateTime? modifydate { get; set; }
        public string modifydatestr { get; set; }
        public bool isdeleted { get; set; } = false;
    }
}
