﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Saffron.Models;

namespace Saffron.Models
{
    public class BudgetTransactionViewModel 
    {
        public List<Budget> Budgets { get; set; }
        public List<BudgetItem> BudgetItems { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<BudgetViewItem> GraphData { get; set; }
        
        

    }

    public class BudgetSummaryViewModel  //Used on Overview
    {
        public string Labels { get; set; }
        public string Series { get; set; }
        public string SeriesTotal { get; set; }
        public string Options { get; set; }
        public Dictionary<int,float> dCategorySpending { get; set; }
        
    }

    public class AccountViewModel
    {
        public List<Account> Accounts { get; set; }
        public List<AccountType> Types { get; set; }
    }

    public class AccountDetailViewModel : TransactionViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public Account Account { get; set; }
    }

    public class BudgetViewItem
    {
        public int BudgetItemId { get; set; }
        public int CategoryId { get; set; }
        public float BudgetTotal { get; set; } 
        public string CategoryName { get; set; }
        public string TotalValue { get; set; }
        public string SumValue { get; set; } 
        public int TransactionCount { get; set; }       

    }
}