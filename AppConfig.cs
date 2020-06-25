using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MasterPass.Asmx
{
    public class AppConfig
    {
        public static string LogTranConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ServiceString"].ConnectionString;
            }
        }
    }
}