using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.common
{
    public class JsonResponse
    {
        public JsonResponse()
        {
            success = 0;
        }
        //public int totalrecords { get; set; }
        public int success { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
        //public PaginationModel paginationModel { get; set; }
    }
    public class genericResponse
    {
        public int success { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }

}


