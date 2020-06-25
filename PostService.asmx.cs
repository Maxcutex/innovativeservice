using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Services;
using Bola.MiscHelper;
using ISO8583Connect;
using NanoService.Asmx.DataAccess;
using NanoService.Asmx.Models;
using MasterPass.Asmx.DataAccess;
using MasterPass.Asmx.Models;
using MasterPass.Asmx.Helpers;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MasterPass.Asmx
{
    /// <summary>
    /// Summary description for PostService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PostService : System.Web.Services.WebService
    {

        public AuthUser User;
        public SecuredTokenWebservice SoapHeader;

        //[WebMethod]
        //[SoapHeader("User", Required = true)]
        public string ValidateUser()
        {
            if (User != null)
            {
                if (User.IsValid())
                {
                    return "Hello " + User.UserName;
                }
                else
                    return "Invalid credentials";
            }
            else
            {
                return "Please provide user details";
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string AuthenticateUser()
        {
            if (SoapHeader == null)
                return "Please provide Username and Password";
            if (string.IsNullOrEmpty(SoapHeader.UserName) || string.IsNullOrEmpty(SoapHeader.Password))
                return "Please provide Username and Password";

            //Check is User credentials Valid
            if (!SoapHeader.IsUserCredentialsValid(SoapHeader.UserName, SoapHeader.Password))
                return "Invalid Username or Password";

            // Create and store the AuthenticatedToken before returning it
            string token = Guid.NewGuid().ToString();
            HttpRuntime.Cache.Add(
                token,
                SoapHeader.UserName,
                null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(30),
                System.Web.Caching.CacheItemPriority.NotRemovable,
                null
            );

            return token;
        }

        private ResultReponseTransactionStatus result;

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponseTransactionStatus TransactionStatusInquiry(string clientReference, string debitAccount)
        {
            var resp = new ResultReponseTransactionStatus();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            var dataRow = new DataResponse();
            var dal = new DataAccessLayer();
            
            try
            {
                var dataTable = dal.GetTrnasactionStatus(clientReference, debitAccount);
           
                resp.ResponseCode = dataTable.Rows[0]["response_code"].ToString(); ;
                resp.ResponseMessage = dataTable.Rows[0]["response_message"].ToString();
                resp.TransactionReference = dataTable.Rows[0]["stan"].ToString();
                resp.TransactionId = dataTable.Rows[0]["transaction_id"].ToString();
            }
            catch (Exception e)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = e.Message;
            }
             return resp;
            
            ///return JsonConvert.SerializeObject(resp);

        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponseQRCode GenerateQrCode(string qrcode)
        {
            var resp = new ResultReponseQRCode();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        string generatedCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        resp.ResponseCode = "0";
                        resp.ResponseMessage = "QRCode generated successfully";
                        resp.QRCode = generatedCode;
                    }
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = e.Message;
            }
            return resp;

            ///return JsonConvert.SerializeObject(resp);

        }


        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponseTransaction GetTransaction(string tranid, string accountNumber)
        {
            var resp = new ResultReponseTransaction();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }
            var dataRow = new DataResponse();

            var dal = new DataAccessLayer();

            try
            {
                var  dataTable = dal.SyncDataBatchSql(tranid, accountNumber);
                resp.TransactionAmount = dataTable.Rows[0]["TRANSACTION_AMOUMT"].ToString();
                resp.TransactionDate = dataTable.Rows[0]["TRANSACTION_DATE"].ToString();
                resp.Transactionfee = dataTable.Rows[0]["TRANSACTION_FEE"].ToString();
                resp.TransactionId = dataTable.Rows[0]["TRANSACTION_ID"].ToString();
                resp.DebitAcount = dataTable.Rows[0]["DEBIT_ACCOUNT"].ToString();
                resp.CreditAccount = dataTable.Rows[0]["CREDIT_ACCOUNT"].ToString();
                resp.TransactionReference = dataTable.Rows[0]["TRANSACTION_REFERNCE"].ToString();
                resp.TrasactionNarration = dataTable.Rows[0]["NARRATION"].ToString();
                resp.ResponseCode = "0";
                resp.ResponseMessage = "Successful";
            }
            catch (Exception e)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = e.Message;
            }
            return resp;
            ///return JsonConvert.SerializeObject(resp);

        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public GetCustomerResponse SearchCustomer(string CustomerId)
        {
            var resp = new GetCustomerResponse();

                //var dataRow = new DataResponse();

            var dal = new DataAccessLayer();

            try
            {
                var dataTable = dal.GetCustomerDetails(CustomerId);
                resp.AccountId = dataTable.Rows[0]["ACCOUNTID"].ToString();
                resp.BankId = dataTable.Rows[0]["BANK_ID"].ToString();
                resp.OrgKey = dataTable.Rows[0]["ORGKEY"].ToString();
                resp.OrgType = dataTable.Rows[0]["ORGTYPE"].ToString();
                resp.FirstName = dataTable.Rows[0]["CUST_FIRST_NAME"].ToString();
                resp.MiddleName = dataTable.Rows[0]["CUST_MIDDLE_NAME"].ToString();
                resp.LastName = dataTable.Rows[0]["CUST_LAST_NAME"].ToString();
                resp.Title = dataTable.Rows[0]["SALUTATION"].ToString();

                resp.DayOfBirth = dataTable.Rows[0]["BIRTH_DAY"].ToString();
                resp.MonthOfBirth = dataTable.Rows[0]["BIRTH_MONTH"].ToString();
                resp.YearOfBirth = dataTable.Rows[0]["BIRTH_YEAR"].ToString();
                resp.DateOfBirth = dataTable.Rows[0]["CUST_DOB"].ToString();
                resp.CustomerType = dataTable.Rows[0]["CUST_TYPE"].ToString();
                resp.Language = dataTable.Rows[0]["CUST_LANGUAGE"].ToString();
                resp.CellPhone = dataTable.Rows[0]["PHONE_CELL"].ToString();
                resp.City = dataTable.Rows[0]["CITY_CODE"].ToString();


                resp.CityCode = dataTable.Rows[0]["CITY"].ToString();
                resp.RelationshipOpeningDate = dataTable.Rows[0]["RELATIONSHIPOPENINGDATE"].ToString();
                resp.Status = dataTable.Rows[0]["STATUS"].ToString();
                resp.Manager = dataTable.Rows[0]["MANAGER"].ToString();
                resp.MotherMaidenName = dataTable.Rows[0]["MAIDENNAMEOFMOTHER"].ToString();
                resp.UniqueId = dataTable.Rows[0]["UNIQUEID"].ToString();
                resp.UniqueIdType = dataTable.Rows[0]["UNIQUEIDTYPE"].ToString();
                resp.StaffEmployeeId = dataTable.Rows[0]["STAFFEMPLOYEEID"].ToString();


                resp.ShortName = dataTable.Rows[0]["SHORT_NAME"].ToString();
                resp.Blacklisted = dataTable.Rows[0]["BLACKLISTED"].ToString();
                resp.Negated = dataTable.Rows[0]["NEGATED"].ToString();
                resp.Suspended = dataTable.Rows[0]["SUSPENDED"].ToString();
                resp.PreferredName = dataTable.Rows[0]["PREFERREDNAME"].ToString();
                resp.RecordStatus = dataTable.Rows[0]["RECORDSTATUS"].ToString();
                resp.Name = dataTable.Rows[0]["NAME"].ToString();
                resp.CustomerSegment = dataTable.Rows[0]["SEGMENTATION_CLASS"].ToString();

                resp.DefaultAddressType = dataTable.Rows[0]["DEFAULTADDRESSTYPE"].ToString();
                resp.PreferredPhone = dataTable.Rows[0]["PREFERREDPHONE"].ToString();
                resp.StartDate = dataTable.Rows[0]["STARTDATE"].ToString();
                resp.CurrencyCode = dataTable.Rows[0]["CRNCY_CODE"].ToString();
                resp.PrimarySolId = dataTable.Rows[0]["PRIMARY_SOL_ID"].ToString();
                resp.PrimaryBranch = dataTable.Rows[0]["PRIMARY_SOL_ID"].ToString();
                resp.CustomerId = dataTable.Rows[0]["CORE_CUST_ID"].ToString();
                resp.EntityCreditFlag = dataTable.Rows[0]["ENTITY_CRE_FLAG"].ToString();

                resp.IseBankingEnabled = dataTable.Rows[0]["ISEBANKINGENABLED"].ToString();
                resp.IsSmsBankingEnabled = dataTable.Rows[0]["ISSMSBANKINGENABLED"].ToString();
                resp.IsWapBankingEnabled = dataTable.Rows[0]["ISWAPBANKINGENABLED"].ToString();
                resp.PreferredPhoneType = dataTable.Rows[0]["PREFERREDPHONETYPE"].ToString();
                resp.PreferredEmail = dataTable.Rows[0]["PREFERREDEMAIL"].ToString();
                resp.AddressLine1 = dataTable.Rows[0]["ADDRESS_LINE1"].ToString();
                resp.AddressLine2 = dataTable.Rows[0]["ADDRESS_LINE2"].ToString();
                resp.AddressLine3 = dataTable.Rows[0]["ADDRESS_LINE3"].ToString();

                resp.State = dataTable.Rows[0]["STATE_CODE"].ToString();
                resp.StateCode = dataTable.Rows[0]["STATE"].ToString();
                resp.Country = dataTable.Rows[0]["COUNTRY_CODE"].ToString();
                resp.CountryCode = dataTable.Rows[0]["COUNTRY"].ToString();
                resp.Zip = dataTable.Rows[0]["ZIP"].ToString();
                resp.AccessOwnerSegment = dataTable.Rows[0]["ACCESSOWNERSEGMENT"].ToString();
                resp.AccessOwnerGroup = dataTable.Rows[0]["ACCESSOWNERGROUP"].ToString();
                resp.SmsBankingMobileNumber = dataTable.Rows[0]["SMSBANKINGMOBILENUMBER"].ToString();

                resp.AlreadyCreatedInEbanking = dataTable.Rows[0]["ALREADYCREATEDINEBANKING"].ToString();
                resp.SubSegment = dataTable.Rows[0]["SUBSEGMENT"].ToString();


            }
            catch (Exception e)
            {
                resp.ResponseMessage = e.Message;
            }
            return resp;
            ///return JsonConvert.SerializeObject(resp);

        }


        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponse BalanceInquiry(string accountNumber)
        {
            var resp = new ResultReponse();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }
            //var dataRow = new DataResponse();

            var dal = new DataAccessLayer();
            try
            {
                var dataTable = dal.GetBalance(accountNumber);
                resp.AvailableBalance = dataTable.Rows[0]["AVAILABLE_BALANCE"].ToString();
                resp.LedgerBalance = dataTable.Rows[0]["LEDGER_BALANCE"].ToString();
                resp.ResponseCode = "0";
                resp.ResponseMessage = "Successful";
            }
            catch (Exception e)
            {
               
                resp.ResponseCode = "1";
                resp.ResponseMessage = e.Message;
            }
            var stan = MyHelperClass.GenerateRandomString(12);
            var transValueDate = DateTime.Now;
            var tranType = "BLIR";
            var get = LogTransactions.StoreTransactions(stan, transValueDate, tranType, accountNumber, "", 0, 0, transValueDate, "" , "", "", "");
            return resp;
            //return   JsonConvert.SerializeObject(resp);

        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public AccountDetailsResponse NameInquiry(string accountNumber)
        {
            var resp = new AccountDetailsResponse();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }
            //var dataRow = new DataResponse();

            var dal = new DataAccessLayer();
            try
            {
                var dataTable = dal.GetAccountDetails(accountNumber);
                resp.AccountNumber = dataTable.Rows[0]["foracid"].ToString();
                resp.AccountName = dataTable.Rows[0]["acct_name"].ToString();
                resp.AccountType = dataTable.Rows[0]["account_type"].ToString();
                resp.AccountStatus = dataTable.Rows[0]["acct_status"].ToString();
                resp.AccountTier = dataTable.Rows[0]["acct_tier"].ToString();

                resp.ResponseCode = "0";
                resp.ResponseMessage = "Successful";
            }
            catch (Exception e)
            {

                resp.ResponseCode = "1";
                resp.ResponseMessage = e.Message;
            }
            var stan = MyHelperClass.GenerateRandomString(12);
            var transValueDate = DateTime.Now;
            var tranType = "NAIR";
            var get = LogTransactions.StoreTransactions(stan, transValueDate, tranType, accountNumber, "", 0, 0, transValueDate, "", "", "", "");
            return resp;
            //return   JsonConvert.SerializeObject(resp);

        }


        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponseTransactionStatus PostTransaction(string debitAccount, string creditAccount, double amount, string narration, string valueDate, double fee, string reference, string currency = "NGN", string terminalCode = "INO")
        {

            if (terminalCode == "")
                terminalCode = "INO";
            if (currency == "")
                currency = "NGN";


            var resp = new ResultReponseTransactionStatus();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }
            var nowDate = Convert.ToDateTime(valueDate);

            try
            {
                var configData = LogTransactions.GetConfigDetails(terminalCode);
                var col124 = configData.Rows[0]["terminalCode"].ToString();
                var appCode = configData.Rows[0]["terminalDescription"].ToString();
                var defaultNarration = configData.Rows[0]["defaultNarration"].ToString();

                var solId = "999";

                ResultReponseTransactionStatus postConnect = PostTransaction(debitAccount, creditAccount, currency, Convert.ToDouble(amount), nowDate,
                                    narration,col124,fee ,solId,reference,appCode);
                resp = postConnect;
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);

                MyHelperClass.WriteLog(ex);
                resp.ResponseCode = "99";
                resp.ResponseMessage = "Application Error";
                return resp;
            }

            

            return resp;

        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultResponseMiniStatementInquiry MiniStatementInquiry(string accountNumber)
        {
            var terminalCode = "INO";
            var currency = "NGN";


            var resp = new ResultResponseMiniStatementInquiry();
            if (SoapHeader == null)
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }

            if (!SoapHeader.IsUserCredentialsValid(SoapHeader))
            {
                resp.ResponseCode = "1";
                resp.ResponseMessage = "Invalid Token";
                return resp;
            }
            var nowDate = DateTime.Now;

            try
            {
                var configData = LogTransactions.GetConfigDetails(terminalCode);


                var solId = "999";

                ResultResponseMiniStatementInquiry postConnect = MiniStatement(accountNumber, currency,  solId);
                resp = postConnect;
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);

                MyHelperClass.WriteLog(ex);
                resp.ResponseCode = "99";
                resp.ResponseMessage = "Application Error";
                return resp;
            }


            return resp;

        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public ResultReponseTransactionStatus ReverseTransaction(string debitAccount, string creditAccount, string narration, string refNo, string tranId, string reference, string terminalCode = "INO")
        {

            var resp = new ResultReponseTransactionStatus();
            if (terminalCode == "")
                terminalCode = "INO";
            try
            {
                var configData = LogTransactions.GetConfigDetails(terminalCode);
                var appCode = configData.Rows[0]["terminalDescription"].ToString();
                var defaultNarration = configData.Rows[0]["defaultNarration"].ToString();
                var dataTable = LogTransactions.GetDetailsForReversal(refNo, tranId); 
                var valueDate = dataTable.Rows[0]["system_date_time"];                
                var nowDate = Convert.ToDateTime(valueDate);
                var col124 = dataTable.Rows[0]["terminal_type"].ToString();
                var fee = dataTable.Rows[0]["transaction_fee"];
                var amount = dataTable.Rows[0]["transaction_amount"];
                var oldId = dataTable.Rows[0]["id"].ToString();
                var solId = dataTable.Rows[0]["sol_id"].ToString();

                result = ReverseTransaction(debitAccount, creditAccount,
                "NGN", Convert.ToDouble(amount), nowDate, refNo, narration, col124, Convert.ToDouble(fee), solId,Convert.ToInt32(oldId),reference,appCode);
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);
                resp.ResponseCode = "99";
                resp.ResponseMessage = "Application Error";
                return resp;
            }

            return result;
        }


        [WebMethod]
        public string EncryptDecryptString(string text, string indicator)
        {
            if (indicator == "E")
            {
                return GenericFunctions.Encrypt(text);
            }
            return GenericFunctions.Decrypt(text); ;
        }
        public string Enquiry(string accountNum)
        {
            var bankId = "030";

            var ipAddress = ConfigurationManager.AppSettings["finacleIp"];
            var controllerId = ConfigurationManager.AppSettings["controllerId"];
            // live "10.0.0.133"; // test "10.0.33.13"; 
            

            // transValueDate; //Convert.ToDateTime("2015-07-27 00:00:00");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["finaclePort"]);
            // live 47002; // test 48027; //
            Response resCharge;
             
            var xFTran = new BalanceInquiry(controllerId, ipAddress, port);

            try
            {
                
                resCharge =  xFTran.Commit(accountNum);

            }
            catch (Exception ex)
            {
                 MyHelperClass.WriteLog(ex);

                return "-99|Error: Application Error"  ;
            }
            if (resCharge != Response.TransactionApproved)
            {
                return "-1|" + resCharge ;
            }
            return "0|" + xFTran.Diagnosis.Trim() + "|" + resCharge ;
        }

        private ResultReponseTransactionStatus PostTransaction(string debitAccount, string creditAccount, string currency,
            double amount, DateTime transValueDate,    string appDesc, string col124, double fee , string solId, string refernce, string appCode)
        {
            var bankId = "030";
            
            var ipAddress = ConfigurationManager.AppSettings["finacleIp"];
            var controllerId = ConfigurationManager.AppSettings["controllerId"];
            // live "10.0.0.133"; // test "10.0.33.13"; 
            var ntranDate = transValueDate;
            // var te = amount.ToString();
            //amount = amount * 100;
            if (ConfigurationManager.AppSettings["Environment"] == "Test")
            {
                ntranDate = Convert.ToDateTime(ConfigurationManager.AppSettings["TestDate"]);
            }

            // transValueDate; //Convert.ToDateTime("2015-07-27 00:00:00");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["finaclePort"]);
            // live 47002; // test 48027; //
            Response resCharge;

            var stan = MyHelperClass.GenerateRandomString(12) ;
            
            var uniqueId = stan;
            var response = new ResultReponseTransactionStatus();

            var xFTran = new FundTransfer(controllerId, ipAddress, port);
            var tranType = "TRTR";
            try
            {
                //xFTran.ValueDate = transValueDate;
                xFTran.ValueDate = ntranDate;
                xFTran.Field41CardAcceptorTerminalId = "000000003"+ col124 + solId + "1";
                var terminalId =  xFTran.Field41CardAcceptorTerminalId;

                xFTran.Field2PAN = "9990190000000000000";
                switch (currency)
                {
                    case "NGN":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.NGN);
                        break;
                    case "USD":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.USD);
                        break;
                    case "GBP":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.GBP);
                        break;
                    case "EUR":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.EUR);
                        break;
                }
                tranType = "TRTR";
                var tranFee = (fee * 100).ToString();
                tranFee = tranFee.PadLeft(16, '0');
                xFTran.OptionalFieldData = "70" + currency + "D" + tranFee + "00000001D0000000000000000" + currency; //Field46

                xFTran.Field37RetrievalReferenceNumber = stan;
                xFTran.Field42CardAcceptorTerminalCode = appCode;
                xFTran.Field124InfoText = col124;
                var get = LogTransactions.StoreTransactions(stan,transValueDate,tranType,debitAccount,creditAccount,amount,fee,transValueDate,terminalId,solId,col124,refernce);

                resCharge = xFTran.Commit(amount, debitAccount, creditAccount, appDesc,
                    uniqueId);
                MyHelperClass.WriteLog(xFTran.Diagnosis.ToString());
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);
                response.ResponseCode = "-99";
                response.ResponseMessage = "Application Error, contact support";
                response.TransactionId = "";
                response.TransactionReference = uniqueId;
                return response;
            }
           
            if (resCharge != Response.TransactionApproved)
            {
               
                var messageFail = resCharge.ToString();
                var tranIdFail = "";
                LogTransactions.UpdateAfterPost(stan, tranIdFail, "1", messageFail);
                response.ResponseCode =  "1";
                response.ResponseMessage =  messageFail;
                response.TransactionId =  "";
                response.TransactionReference  = uniqueId;
                ///return "-1|" + resCharge + "|" + uniqueId;
                //return JsonConvert.SerializeObject(response);
                return response;
            }
            //return "0|" + Right(xFTran.Diagnosis.Trim(), 9) + "|" + uniqueId;
            

            var tranId = Right(xFTran.Diagnosis.Trim(), 9);
            tranId = tranId.Replace("::", "");
            tranId = tranId.Replace("::", "").TrimStart('7');
            var message = "Approved";
            
            response.ResponseCode = "0";
            response.ResponseMessage = message;
            response.TransactionId = tranId;
            response.TransactionReference = uniqueId;
            var up = LogTransactions.UpdateAfterPost(stan, tranId, "0", message);
            return response;
        }


        private ResultResponseMiniStatementInquiry MiniStatement(string debitAccount, string currency, string solId)
        {
            var col124 = "INO";

            var ipAddress = ConfigurationManager.AppSettings["finacleIp"];
            var controllerId = ConfigurationManager.AppSettings["controllerId"];
            // live "10.0.0.133"; // test "10.0.33.13"; 
            var ntranDate = DateTime.Now;
            // var te = amount.ToString();
            //amount = amount * 100;
            if (ConfigurationManager.AppSettings["Environment"] == "Test")
            {
                ntranDate = Convert.ToDateTime(ConfigurationManager.AppSettings["TestDate"]);
            }

            // transValueDate; //Convert.ToDateTime("2015-07-27 00:00:00");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["finaclePort"]);
            // live 47002; // test 48027; //
            Response resCharge;

            var stan = MyHelperClass.GenerateRandomString(12);

            var response = new ResultResponseMiniStatementInquiry();
            var tranFee = (0 * 100).ToString();
            tranFee = tranFee.PadLeft(16, '0');

            var xFTran = new SendMessage(controllerId, ipAddress, port)
            {
                Field37RetrievalReferenceNumber = stan,
                Field42CardAcceptorTerminalCode = "INNOVECTIVEMINI",
                Field124InfoText = col124,
                Field41CardAcceptorTerminalId = "000000003" + col124 + solId + "1",
                Field2PAN = "9990190000000000000",
                Field46AmountFees = "70" + currency + "D" + tranFee + "00000001D0000000000000000" + currency,
            };

            var balance = 0.00;
            var tranType = "MSIR";
            try
            {
                //xFTran.ValueDate = transValueDate;
                xFTran.ValueDate = ntranDate;
                var terminalId = xFTran.Field41CardAcceptorTerminalId;

                switch (currency)
                {
                    case "NGN":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.NGN);
                        break;
                    case "USD":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.USD);
                        break;
                    case "GBP":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.GBP);
                        break;
                    case "EUR":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.EUR);
                        break;
                }


                var get = LogTransactions.StoreTransactions(stan, ntranDate, tranType, debitAccount, "", 0, 0, ntranDate, terminalId, solId, col124, stan);
                resCharge = xFTran.MiniStatement(debitAccount);
                

                MyHelperClass.WriteLog(xFTran.Diagnosis.ToString());
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);
                response.ResponseCode = "-99";
                response.ResponseMessage = "Application Error, contact support";
                return response;
            }

            if (resCharge != Response.TransactionApproved)
            {

                var messageFail = resCharge.ToString();
                var tranIdFail = "";
                LogTransactions.UpdateAfterPost(stan, tranIdFail, "1", messageFail);
                response.ResponseCode = "1";
                response.ResponseMessage = messageFail;
                ///return "-1|" + resCharge + "|" + uniqueId;
                //return JsonConvert.SerializeObject(response);
                return response;
            }
            //return "0|" + Right(xFTran.Diagnosis.Trim(), 9) + "|" + uniqueId;


            var tranId = Right(xFTran.Diagnosis.Trim(), 9);
            tranId = tranId.Replace("::", "");
            tranId = tranId.Replace("::", "").TrimStart('7');
            var message = "Approved";

            response.ResponseCode = "0";
            response.ResponseMessage = message;

            response.ResponseCode = xFTran.ResponseCode;
            response.ResponseMessage = message;
            response.AvailableBalance = xFTran.Balance.FirstAccAvail;
            response.LedgerBalance = xFTran.Balance.FirstAccLedger;
            var listResult = GenericFunctions.SplitStringMiniStatement(xFTran.Field125OptionalField1);
            List<TransactionHistory> transactionHistory = new List<TransactionHistory>();
            foreach (string s in listResult)
            {
                var res = GenericFunctions.SplitStringMiniStatementDet(s);
                var tranHis = new TransactionHistory
                {
                    TransactionDate = res[0],
                    TransactionAmount = res[3],
                    DebitOrCredit = res[2],
                    Narration = res[1]

                };
                transactionHistory.Add
                    (tranHis);
            }
            response.TransactionHistory = transactionHistory;

            var up = LogTransactions.UpdateAfterPost(stan, tranId, "0", message);
            return response;
        }

        public ResultReponseTransactionStatus ReverseTransaction(string debitAccount, string creditAccount, string currency,double amount, 
            DateTime transValueDate, string stan, string appDesc, string col124, double fee, string solId, int oldId, string reference, string appCode )
        {
            var bankId = "030";
            var controllerId = ConfigurationManager.AppSettings["controllerId"];
            var ipAddress = ConfigurationManager.AppSettings["finacleIp"];
            // live "10.0.0.133"; // test "10.0.33.13"; 
            var ntranDate = transValueDate;
            // var te = amount.ToString();
            //amount = amount * 100;
            if (ConfigurationManager.AppSettings["Environment"] == "Test")
            {
                ntranDate = Convert.ToDateTime(ConfigurationManager.AppSettings["TestDate"]);
            }

            // transValueDate; //Convert.ToDateTime("2015-07-27 00:00:00");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["finaclePort"]);
            // live 47002; // test 48027; //
            Response resCharge;
            var uniqueId = stan; //MyHelperClass.GenerateRandomString(8) + DateTime.Now.Date.Second + rnd.Next();
            var xFTran = new FundTransfer(controllerId, ipAddress, port);
            var tranType = "TRRR";
            var response = new ResultReponseTransactionStatus();

            try
            {
                //xFTran.ValueDate = transValueDate;
                xFTran.ValueDate = ntranDate;
                xFTran.Field41CardAcceptorTerminalId = "000000003" + col124 +  solId + "1";
                var terminalId = xFTran.Field41CardAcceptorTerminalId;

                xFTran.Field2PAN = "9990190000000000000";
                switch (currency)
                {
                    case "NGN":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.NGN);
                        break;
                    case "USD":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.USD);
                        break;
                    case "GBP":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.GBP);
                        break;
                    case "EUR":
                        xFTran.Currency = new Currency(Currency.CURRENCY_REF.EUR);
                        break;
                }

                var tranFee = (fee * 100).ToString();
                tranFee = tranFee.PadLeft(16, '0');
                xFTran.OptionalFieldData = "70" + currency + "C" + tranFee + "00000001C0000000000000000" + currency; //Field46

                xFTran.Field37RetrievalReferenceNumber = stan;
                xFTran.Field42CardAcceptorTerminalCode = appCode;
                xFTran.Field124InfoText = col124;
                var get = LogTransactions.StoreTransactions(stan, transValueDate, tranType, debitAccount, creditAccount, amount, fee, transValueDate, terminalId, solId, col124,reference);
                resCharge = xFTran.Reverse(amount, debitAccount, creditAccount, appDesc,
                    uniqueId,ntranDate);
                MyHelperClass.WriteLog(xFTran.Diagnosis.ToString());
            }
            catch (Exception ex)
            {
                MyHelperClass.WriteLog(ex);

                response.ResponseCode = "-99";
                response.ResponseMessage = "Application error, contact support";
                response.TransactionId = "";
                response.TransactionReference = uniqueId;
                return response;
            }

            if (resCharge != Response.TransactionApproved)
            {
                var messageFail = resCharge.ToString();
                var tranIdFail = "";
                LogTransactions.UpdateAfterPost(stan, tranIdFail, "1", messageFail);
                response.ResponseCode = "1";
                response.ResponseMessage = messageFail;
                response.TransactionId = "";
                response.TransactionReference = uniqueId;
                //return "-1|" + resCharge + "|" + uniqueId;
                ///return JsonConvert.SerializeObject(response);
                return response;
            }
            var tranId = Right(xFTran.Diagnosis.Trim(), 9);
            tranId = tranId.Replace("::", "");
            tranId = tranId.Replace("::", "").TrimStart('7');
            var message = "Approved";
            response.ResponseCode = "0";
            response.ResponseMessage = message;
            response.TransactionId = tranId;
            response.TransactionReference = uniqueId;
            var up = LogTransactions.UpdateAfterPost(stan, tranId, "0", message,'Y',oldId);
            // var result =  "0|" + Right(xFTran.Diagnosis.Trim(), 9) + "|" + uniqueId;
            //return JsonConvert.SerializeObject(response);
            return response;
        }
        public static string Right(string value, int length)
        {
            return value.Substring(value.Length - length);
        }

      

    }
}
