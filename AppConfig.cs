using System.Configuration;

namespace InnovativeService
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