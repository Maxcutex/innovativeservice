using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NanoService.Asmx.Models
{
    public class DataResponse
    {
        public DataTable Records { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}