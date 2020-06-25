using System;
using System.Data;
using System.Data.SqlClient;
using Bola.MiscHelper;

namespace InnovativeService.DataAccess
{
    public class LogTransactions
    {
        public static DataTable GetDetailsForReversal(string stan, string tranId)
        {

            var dataTable = new DataTable();
            var dataAccess = new SqlDataAdapter();
            var dataSet = new DataSet();

            //  bool OpnrtnStatus = false;

            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                //(Real Live Query)
                string query = "getDetilsForREversal";

                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@stan", SqlDbType.VarChar);
                        command.Parameters["@stan"].Value = stan;
                        command.Parameters.Add("@tranId", SqlDbType.VarChar);
                        command.Parameters["@tranId"].Value = tranId;
                        dataAccess.SelectCommand = command;
                        cb = new SqlCommandBuilder(dataAccess);
                        dataAccess.Fill(dataSet);
                        //da.Fill(dt)
                        dataTable = dataSet.Tables[0];

                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dataTable;
        }

        public static DataTable GetConfigDetails(string terminalCode)
        {

            var dataTable = new DataTable();
            var dataAccess = new SqlDataAdapter();
            var dataSet = new DataSet();

            //  bool OpnrtnStatus = false;

            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                //(Real Live Query)
                string query = "GetConfigDetails";

                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@terminalCode", SqlDbType.VarChar);
                        command.Parameters["@terminalCode"].Value = terminalCode;
                        dataAccess.SelectCommand = command;
                        cb = new SqlCommandBuilder(dataAccess);
                        dataAccess.Fill(dataSet);
                        //da.Fill(dt)
                        dataTable = dataSet.Tables[0];

                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dataTable;
        }

        public static int StoreTransactions( string stan,  DateTime transactiondate ,  string transactionType , string debitAccount, string creditAccount , 
            double transactionAmount , double fee, DateTime systemDateTime,string terminalId, string solId, string  terminalType, string reference)
        {
            var ret = 0;
            string reservedField1 = "";
            string reservedField2 = "";
            string reservedField3 = "";
            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                //(Real Live Query)
                string query = "StoreTransactions";

                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@stan", SqlDbType.VarChar);
                        command.Parameters["@stan"].Value = stan;
                        command.Parameters.Add("@transactiondate", SqlDbType.Date);
                        command.Parameters["@transactiondate"].Value = transactiondate;
                        command.Parameters.Add("@transactionType", SqlDbType.VarChar);
                        command.Parameters["@transactionType"].Value = transactionType;
                        command.Parameters.Add("@debitAccount", SqlDbType.VarChar);
                        command.Parameters["@debitAccount"].Value = debitAccount;
                        command.Parameters.Add("@creditAccount", SqlDbType.VarChar);
                        command.Parameters["@creditAccount"].Value = creditAccount;
                        command.Parameters.Add("@transactionAmount", SqlDbType.Decimal);
                        command.Parameters["@transactionAmount"].Value = transactionAmount;
                        command.Parameters.Add("@transactionFee", SqlDbType.Decimal);
                        command.Parameters["@transactionFee"].Value = fee;
                        command.Parameters.Add("@systemDateTime", SqlDbType.DateTime);
                        command.Parameters["@systemDateTime"].Value = systemDateTime;
                        command.Parameters.Add("@terminalId", SqlDbType.VarChar);
                        command.Parameters["@terminalId"].Value = terminalId;
                        command.Parameters.Add("@solId", SqlDbType.VarChar);
                        command.Parameters["@solId"].Value = solId;
                        command.Parameters.Add("@terminalType", SqlDbType.VarChar);
                        command.Parameters["@terminalType"].Value = terminalType;
                        command.Parameters.Add("@reservedField1", SqlDbType.VarChar);
                        command.Parameters["@reservedField1"].Value = reservedField1;
                        command.Parameters.Add("@reservedField2", SqlDbType.VarChar);
                        command.Parameters["@reservedField2"].Value = reservedField2;
                        command.Parameters.Add("@reservedField3", SqlDbType.VarChar);
                        command.Parameters["@reservedField3"].Value = reservedField3;
                        command.Parameters.Add("@clientReference", SqlDbType.VarChar);
                        command.Parameters["@clientReference"].Value = reference;


                        var returnPar = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnPar.Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();
                        return Convert.ToInt16(returnPar.Value);
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return ret;
        }

        public static int UpdateAfterPost(string refId,  string tranId, string respCode, string message, char isReversal = 'N',int oldId = 0)
        {
            var ret = 0;
            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                string query = "updateFinalTransactions";
                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@stan", SqlDbType.VarChar);
                        command.Parameters["@stan"].Value = refId;
                        command.Parameters.Add("@tranId", SqlDbType.VarChar);
                        command.Parameters["@tranId"].Value = tranId;
                        command.Parameters.Add("@respCode", SqlDbType.VarChar);
                        command.Parameters["@respCode"].Value = respCode;
                        command.Parameters.Add("@message", SqlDbType.VarChar);
                        command.Parameters["@message"].Value = message;
                        command.Parameters.Add("@isReversal", SqlDbType.Char);
                        command.Parameters["@isReversal"].Value = isReversal; 
                        command.Parameters.Add("@oldId", SqlDbType.VarChar);
                        command.Parameters["@oldId"].Value = @oldId;

                        var returnPar = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnPar.Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();
                        return Convert.ToInt16(returnPar.Value);
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return ret;
        }

        public static int ArchiveTransaction(long refId)
        {
            var ret = 0;
            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                string query = "MAC_sp_updateTransactionHistory";
                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@refId", SqlDbType.BigInt);
                        command.Parameters["@refId"].Value = refId;
                        var returnPar = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnPar.Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();
                        return Convert.ToInt16(returnPar.Value);
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return ret;
        }

        public static DataTable GetTranStatus(string clientReference, string debitAccount)
        {

            var dataTable = new DataTable();
            var dataAccess = new SqlDataAdapter();
            var dataSet = new DataSet();

            //  bool OpnrtnStatus = false;

            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                //(Real Live Query)
                string query = "GetTranStatus";

                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@clientReference", SqlDbType.VarChar);
                        command.Parameters["@clientReference"].Value = clientReference;
                        command.Parameters.Add("@debitAccount", SqlDbType.VarChar);
                        command.Parameters["@debitAccount"].Value = debitAccount;
                        dataAccess.SelectCommand = command;
                        cb = new SqlCommandBuilder(dataAccess);
                        dataAccess.Fill(dataSet);
                        //da.Fill(dt)
                        dataTable = dataSet.Tables[0];

                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dataTable;
        }
    }
}