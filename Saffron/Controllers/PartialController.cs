using Microsoft.AspNet.Identity;
using Saffron.Helpers;
using Saffron.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Saffron.Controllers
{
    
    public class PartialController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Partial
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            ViewBag.TestText = "Hello World";
            return PartialView("_PartialTest");

        }
    //====  Build Chartist BudgetSummary Chart with tooltip ==============================================================================

        public ActionResult BudgetSummary()
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            BudgetSummaryViewModel viewModel = new BudgetSummaryViewModel();

            //Initialize Strings
            viewModel.Labels = " labels: [";
            viewModel.Series = " series: [ [";
            viewModel.SeriesTotal = " [";
            viewModel.Options = " seriesBarDistance: 10, reverseData: true,  axisY: { offset: 10 }, plugins: [ Chartist.plugins.tooltip() ]";

            //Get Sum of Current Transactions by CategoryId
            DateTime dateWindow = DateTime.Today.AddDays(-30);
            List<Transaction> transactions = db.Transaction.Where(t => t.Account.HouseholdId == (int)currUser.HouseholdId && t.Date > dateWindow).ToList();
            Dictionary<int, float> transactionSum = new Dictionary<int, float>();
            foreach (Category category in db.Category.ToList())
            {
                float categorySum = 0;
                foreach(Transaction transaction in transactions)
                {
                    if(transaction.CategoryId == category.Id)
                    {
                        if(transaction.TypeTransactionId == 1 || transaction.TypeTransactionId == 4)
                        {
                            categorySum -= transaction.Amount;
                        }
                        if(transaction.TypeTransactionId == 2 || transaction.TypeTransactionId == 3)
                        { 
                            categorySum += transaction.Amount;
                        }
                    }
                   
                }
                transactionSum.Add(category.Id, categorySum);
            }
            
            //Build Strings

            foreach (Budget budget in currUser.Household.Budgets)
            {
                List<BudgetItem> budgetItems = db.BudgetItem.Where(i => i.BudgetId == budget.Id).ToList();

                foreach (BudgetItem budgetItem in  budgetItems )
                {
                    float currSum = transactionSum[budgetItem.CategoryId] / budgetItem.Amount *100;
                    if (currSum > 100) { currSum = 100; }
                    viewModel.Labels += "'" + budgetItem.Category.Name + "',";
                    viewModel.Series += "{meta: '" + budgetItem.Category.Name + "', value: " + currSum + "},";
                    viewModel.SeriesTotal += "{meta: '" + budgetItem.Category.Name + "', value: " + 100 + "},"; ;
                }

            }

            //Finish Strings and ViewModel
            viewModel.Labels = viewModel.Labels.Substring(0, viewModel.Labels.Length - 1) + "]";
            viewModel.SeriesTotal = viewModel.SeriesTotal.Substring(0, viewModel.SeriesTotal.Length - 1) + "]";
            viewModel.Series = viewModel.Series.Substring(0, viewModel.Series.Length - 1) + "]";
            viewModel.dCategorySpending = transactionSum;

            return PartialView("_BudgetSummary",viewModel);
        }

        public ActionResult RecentTransactions()
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            DateTime dateWindow = DateTime.Today.AddDays(-30);
            List<Transaction> transactions = db.Transaction.Where(t => t.Account.HouseholdId == (int)currUser.HouseholdId && t.Date > dateWindow).ToList();

            return PartialView("_RecentTransactions", transactions);
        }

        public ActionResult Edit(int? id)
        {
            ControllerHelpers helper = new ControllerHelpers();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);

            if (transaction == null)
            {
                return HttpNotFound();
            }

            TransactionViewModel viewModel = new TransactionViewModel();
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            if (currUser == null) { return RedirectToAction("Login", "Account"); }

            List<AccountKey> AccountDisplay = helper.GetAccountDisplay(currUser);


            viewModel.editTransaction = transaction;
            //viewModel.AccountId = new SelectList(AccountDisplay, "Id", "InstitutionName");
            //viewModel.CategoryId = new SelectList(db.Category, "Id", "Name");
            //viewModel.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name");
            var model = new Transaction();
            model = transaction;
            ViewBag.AccountId = new SelectList(AccountDisplay, "Id", "InstitutionName", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);
            ViewBag.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name", transaction.TypeTransactionId);
            return PartialView("_EditTransactionPartial",model);
        }
    }
}