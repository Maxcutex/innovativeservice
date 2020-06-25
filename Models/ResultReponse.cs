using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MasterPass.Asmx.Models
{
    public class ResultReponse
    {
        public string AvailableBalance { get; set; }
        public string LedgerBalance { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
       
    }

    public class AccountDetailsResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }
        public string AccountTier { get; set; }


    }

    public class ResultReponseTransaction
    {
        public string TransactionReference { get; set; }
        public string TransactionDate { get; set; }

        public string TransactionId { get; set; }

        public string DebitAcount { get; set; }

        public string CreditAccount { get; set; }

        public string TransactionAmount { get; set; }

        public string Transactionfee { get; set; }

        public string TrasactionNarration { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class GetCustomerResponse
    {
        public string AccountId { get; set; }
        public string BankId { get; set; }
        public string OrgKey { get; set; }

        public string OrgType { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }
        public string Gender { get; set; }
        public string DayOfBirth { get; set; }

        public string MonthOfBirth { get; set; }
        public string YearOfBirth { get; set; }

        public string DateOfBirth { get; set; }

        public string CustomerType { get; set; }

        public string Language { get; set; }

        public string CellPhone { get; set; }

        public string City { get; set; }

        public string CityCode { get; set; }
        public string RelationshipOpeningDate { get; set; }
        public string Status { get; set; }

        public string Manager { get; set; }
        public string MotherMaidenName { get; set; }

        public string UniqueId { get; set; }

        public string UniqueIdType { get; set; }

        public string StaffEmployeeId { get; set; }

        public string ShortName { get; set; }

        public string Blacklisted { get; set; }

        public string Negated { get; set; }
        public string Suspended { get; set; }
        public string PreferredName { get; set; }

        public string RecordStatus { get; set; }
        public string Name { get; set; }

        public string DefaultAddressType { get; set; }

        public string PreferredPhone { get; set; }

        public string StartDate { get; set; }

        public string CurrencyCode { get; set; }

        public string PrimarySolId { get; set; }

        public string PrimaryBranch { get; set; }
        public string CustomerId { get; set; }
        public string EntityCreditFlag { get; set; }

        public string IseBankingEnabled { get; set; }
        public string IsSmsBankingEnabled { get; set; }

        public string SubSegment { get; set; }

        public string IsWapBankingEnabled { get; set; }

        public string AlreadyCreatedInEbanking { get; set; }

        public string SmsBankingMobileNumber { get; set; }

        public string AccessOwnerSegment { get; set; }

        public string AccessOwnerGroup { get; set; }
        public string PreferredPhoneType { get; set; }
        public string PreferredEmail { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string State { get; set; }

        public string StateCode { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Zip { get; set; }

        public string CustomerSegment { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class ResultReponseTransactionStatus
    {
        public string TransactionReference { get; set; }

        public string TransactionId { get; set; }

        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

    }
    public class ResultReponseQRCode
    {
        public string QRCode { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class ResultResponseMiniStatementInquiry
    {
        public double AvailableBalance { get; set; }
        public double LedgerBalance { get; set; }

        public List<TransactionHistory> TransactionHistory { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class TransactionHistory
    {
        private string _tranDate;
        public string TransactionDate
        {
            get
            {
                return this._tranDate;
            }

            set
            {
                this._tranDate = DateFormatting.ChangeToDateTime(value).ToString("dd-MM-yyyy");
            }
        }
        public string Narration { get; set; }

        public string TransactionAmount { get; set; }
        public string DebitOrCredit { get; set; }

    }

    public class DateFormatting
    {
        public static DateTime ChangeToDateTime(string dateString)
        {
            if (dateString != null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime newDate = DateTime.ParseExact(dateString, new string[] { "yyyy-MM-ddTHH:mm:ss.fff", "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy-dd-MM", "yyyyMMdd", "yyyyddMM", "ddMMyyyy", "MMddyyyy" }, provider, DateTimeStyles.None);
                return newDate;
            }
            return DateTime.Now;
        }
    }
}