using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MasterPass.Asmx
{
    public class AuthUser : System.Web.Services.Protocols.SoapHeader
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string AuthenticationToken { get; set; }

        public bool IsValid()
        {
            int count = 0;

            string config = ConfigurationManager.ConnectionStrings["ServiceString"].ConnectionString;

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(config))
            {
                SqlCommand command = new SqlCommand("Select * from AuthUser where UserName =  '" + UserName + "' and password = '" + Password + "' ");
                command.Connection.Open();
                count = Convert.ToInt32(command.ExecuteScalar());
                command.Connection.Close();
            }

            if (count > 0)
                return true;
            else
                return false;
        }

    }
}