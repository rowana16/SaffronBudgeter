using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saffron.Models
{
    public class TransactionViewModel
    {
        //public List<InstitutionKey> AccountDisplay { get; set; };
        public SelectList AccountId { get; set; }
        public SelectList CategoryId { get; set; }
        public SelectList TypeTransactionId { get; set; }
        public List<Transaction> currTransactions { get; set; }
        public Transaction editTransaction { get; set; }
    }

    public class AccountKey
    {
        public string InstitutionName { get; set; }
        public int Id { get; set; }
    }
}