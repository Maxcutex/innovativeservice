using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Bola.MiscHelper;
using Oracle.ManagedDataAccess.Client;

namespace InnovativeService.DataAccess
{
    public   class DataAccessLayer
    {
        public DataAccessLayer()
        {
        }
        public  DataTable SyncDataBatchSql(string tranid, string accountNumber)
        {

            var dt = new DataTable();
            var da = new OracleDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["cnOraConnect"]))
            {
                //(Real Live Query)
                string query = "select stan as transaction_refernce, tran_date as transaction_date, tran_id as transaction_id, " 
                                + "foracid as debit_account, crforacid as credit_account, tran_amt as transaction_amoumt, " 
                                + "fee_amt1 as transaction_fee, tran_particulars as narration from custom.channels_txn "
                                + "where trim(tran_id)='" + tranid + "' and foracid='" + accountNumber + "'";

                try
                {
                    return getData(connection, query, da, ds);
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }

        public DataTable GetCustomerDetails(string CustomerId)
        {

            var dt = new DataTable();
            var da = new OracleDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["cnOraConnect"]))
            {
                //(Real Live Query)
                string query = "select AccountId, bank_id,orgkey,OrgType,cust_First_Name,cust_Middle_Name ,cust_Middle_Name ,salutation ,gender , "
                        + " birth_day,birth_month,birth_year ,cust_dob ,cust_type,cust_language ,phone_cell, "
                        + " (select ref_desc from tbaadm.rct where ref_rec_type='01' and ref_code= city )city , "
                        + " city city_code,relationshipopeningdate ,status ,manager ,maidennameofmother ,uniqueid,uniqueidtype ,staffemployeeid, "
                        + " short_name ,blacklisted ,negated ,suspended ,PreferredName ,RecordStatus ,name ,defaultaddresstype ,PreferredPhone, "
                        + " startdate ,crncy_code ,primary_sol_id ,(select sol_desc from tbaadm.sol where sol_id = primary_sol_id ) PrimaryBranch , "
                        + " core_cust_id , Entity_Cre_Flag , IseBankingEnabled, IsSmsBankingEnabled , SubSegment , IsWapBankingEnabled,  "
                        + " AlreadyCreatedInEbanking , SmsBankingMobileNumber , AccessOwnerSegment , AccessOwnerGroup, PreferredPhoneType , "
                        + " PreferredEmail , Address_Line1, Address_Line2 , Address_Line3 , "
                        + " (select ref_desc from tbaadm.rct where ref_rec_type='02' and ref_code= state ) state , "
                        + " state state_code, country_code ,zip , "
                        + " (select ref_desc from tbaadm.rct where ref_rec_type='02' and ref_code= country )  country "
                        + " crmuser.accounts a where orgkey  ='" + CustomerId + "'";

                try
                {
                    return getData(connection, query, da, ds);
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }
        public DataTable MoneySendCallBack(string tranid, string accountNumber)
        {

            var dt = new DataTable();
            var da = new OracleDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["cnOraConnect"]))
            {
                //(Real Live Query)
                string query = "select stan as transaction_refernce, tran_date as transaction_date, tran_id as transaction_id, "
                               + "foracid as debit_account, crforacid as credit_account, tran_amt as transaction_amoumt, "
                               + "fee_amt1 as transaction_fee, tran_particulars as narration from custom.channels_txn "
                               + "where trim(tran_id)='" + tranid + "' and foracid='" + accountNumber + "'";

                try
                {
                    return getData(connection, query, da, ds);
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }
        public DataTable GetBalance(string accountNumber)
        {

            var dt = new DataTable();
            var da = new OracleDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["cnOraConnect"]))
            {
                //(Real Live Query)
                string query = "select (clr_bal_amt+SANCT_LIM) as ledger_balance,(clr_bal_amt+SANCT_LIM) - (SYSTEM_RESERVED_AMT+lien_amt) as available_balance from tbaadm.gam where foracid='" + accountNumber + "'"
                                + " and acct_cls_flg = 'N' and del_flg = 'N' and entity_cre_flg = 'Y' and acct_ownership != 'O'";
                                //+ " where d.location_id = l.location_id"; ;

                try
                {
                    return getData(connection, query, da, ds);
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }

        public DataTable GetAccountDetails(string accountNumber)
        {

            var dt = new DataTable();
            var da = new OracleDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["cnOraConnect"]))
            {
                //(Real Live Query)
                string query = "select gam.sol_id,acct_name, foracid,gam.schm_code, gam.schm_type,acct_crncy_code, (select open_effective_date from tbaadm.tam where acid = gam.acid) open_effective_date,  "
                        + "ACCT_CLS_DATE,(clr_bal_amt+SANCT_LIM) as ledger_balance,(clr_bal_amt+SANCT_LIM) - (SYSTEM_RESERVED_AMT+lien_amt) as available_balance, "
                        + " (select phoneno from crmuser.phoneemail where orgkey = gam.cif_id and phoneoremail = 'PHONE' and preferredflag = 'Y' and rownum = 1) phone, "
                        + " (select email from crmuser.phoneemail where orgkey = gam.cif_id and phoneoremail = 'EMAIL' and preferredflag = 'Y' and rownum = 1) email, "
                        + " decode(ACCT_CLS_DATE,NULL,'ACCT RUNNING','CLOSED') STATUS , (select maturity_date from tbaadm.tam where acid = gam.acid) maturity_date, cif_id,"
                        + " decode (schm_type,'SBA','SAVINGS ACCOUNT', 'ODA','CURRENT ACCOUNT','CAA','CURRENT ACCOUNT','TDA','TERM DEPOSIT','LAA','TERM LOAN','TERM LOAN')ACCOUNT_TYPE, "
                        + " (select maturity_date from tbaadm.tam where acid = gam.acid) maturity_date, (select deposit_amount from tbaadm.tam where acid = gam.acid) deposit_amount, "
                        + " (case when schm_code = (select schm from custom.atr_schm_tbl where schm = gam.schm_code and tier = 'ONE')then 'Tier One' "
                        + " when schm_code = (select schm from custom.atr_schm_tbl where schm = gam.schm_code and tier = 'TWO')then 'Tier Two' else'Tier Three' end) acct_tier, "
                        + " (select int_pcnt from tbaadm.tdt where substr(flow_code,1,1)='I' and  acid = gam.acid) int_pcnt, decode (acct_status,'A','ACTIVE','D','DORMANT','I','INACTIVE')acct_status "
                        + " from tbaadm.gam gam , (select acid, acct_status, acct_status_date from tbaadm.smt union select acid, acct_status, acct_status_date from tbaadm.cam) s "
                        + " where gam.entity_cre_flg ='Y' and gam.del_flg ='N' and acct_ownership !='O' and s.acid (+) = gam.acid "
                        + " and  foracid = '" + accountNumber + "' ";
                //+ " where d.location_id = l.location_id"; ;

                try
                {
                    return getData(connection, query, da, ds);
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }

        private static DataTable getData(OracleConnection connection, string query, OracleDataAdapter da, DataSet ds)
        {
            DataTable dt;
            OracleCommandBuilder cb = default(OracleCommandBuilder);

            connection.Open();
            using (OracleCommand command = new OracleCommand(query, connection))
            {
                command.CommandType = System.Data.CommandType.Text;
                // command.Parameters.Add("@threadId", SqlDbType.Int);
                //  command.Parameters["@threadId"].Value = threadId;
                da.SelectCommand = command;
                cb = new OracleCommandBuilder(da);
                da.Fill(ds);
                //da.Fill(dt)
                dt = ds.Tables[0];

                return dt;
            }


        }

        public  DataTable GetTrnasactionStatus(string clientRefernce, string debitAccount)
        {

            var dt = new DataTable();
            var da = new SqlDataAdapter();
            var ds = new DataSet();

            //  bool OpnrtnStatus = false;

            using (SqlConnection connection = new SqlConnection(AppConfig.LogTranConnection))
            {
                //(Real Live Query)
                string query = "select * from TransactionLog where client_reference='" + clientRefernce + "'"
                    + " and debit_account='" + debitAccount + "' union " 
                    + "select * from TransactionLogHistory where client_reference='" + clientRefernce + "'"
                    + " and debit_account='" + debitAccount + "'  ";

                try
                {
                    SqlCommandBuilder cb = default(SqlCommandBuilder);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        // command.Parameters.Add("@threadId", SqlDbType.Int);
                        //  command.Parameters["@threadId"].Value = threadId;
                        da.SelectCommand = command;
                        cb = new SqlCommandBuilder(da);
                        da.Fill(ds);
                        //da.Fill(dt)
                        dt = ds.Tables[0];

                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    MyHelperClass.WriteLog(ex);
                }
            }
            return dt;
        }

    }


}