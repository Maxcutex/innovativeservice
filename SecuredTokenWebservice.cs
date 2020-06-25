using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Bola.MiscHelper;
using InnovativeService.Helpers;

namespace InnovativeService
{
    public class SecuredTokenWebservice : System.Web.Services.Protocols.SoapHeader
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string AuthenticationToken { get; set; }
        public bool IsUserCredentialsValid(string UserName, string Password)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                var dataAdaptera = new SqlDataAdapter();
                 var dataSets = new DataSet();
                var dt = new DataTable();
                //string query = "select * from AuthUser where UserName ='" + UserName + "'"
                             //  + " and Password='" + GenericFunctions.Encrypt(Password) + "' ";

                string query = "select * from AuthUser where UserName = @UserName "
                               + " and Password='" + GenericFunctions.Encrypt(Password) + "' ";

                try
                {
                    SqlCommandBuilder commandBuilderb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UserName", SqlDbType.Int);
                        command.Parameters["@UserName"].Value = UserName;

                        // Use AddWithValue to assign Demographics. 
                        // SQL Server will implicitly convert strings into XML.
                       // command.Parameters.AddWithValue("@demographics", demoXml);
                        command.CommandType = System.Data.CommandType.Text;
                        dataAdaptera.SelectCommand = command;
                        commandBuilderb = new SqlCommandBuilder(dataAdaptera);
                        dataAdaptera.Fill(dataSets);
                        //da.Fill(dt)
                        dt = dataSets.Tables[0];
                        count = dt.Rows.Count;


                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }

            if (count > 0)
                return true;
            else
                return false;
        }

        public bool IsUserCredentialsValid(SecuredTokenWebservice SoapHeader)
        {
            if (SoapHeader == null)
                return false;

            // check the token exists in Cache
            if (!string.IsNullOrEmpty(SoapHeader.AuthenticationToken))
                return (HttpRuntime.Cache[SoapHeader.AuthenticationToken] != null);

            return false;
        }

    }
}