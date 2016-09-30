using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Saffron.Models;
using Microsoft.AspNet.Identity;
using Saffron.Helpers;

namespace Saffron.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult DataTables()
        {
            return View();
        }

        // GET: Transactions
        public ActionResult Index()
        {
            ControllerHelpers helper = new ControllerHelpers();
            TransactionViewModel viewModel = new TransactionViewModel();
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            if (currUser == null) { return RedirectToAction("Login", "Account"); }
            List<Transaction> currTransactions = helper.GetTransactions(currUser);
            List<AccountKey> AccountDisplay = helper.GetAccountDisplay(currUser);           


            viewModel.currTransactions = currTransactions;
            viewModel.AccountId = new SelectList(AccountDisplay, "Id", "InstitutionName");
            viewModel.CategoryId = new SelectList(db.Category, "Id", "Name");
            viewModel.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name");
            return View(viewModel);

        }

        

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        //============================================ GET: Transactions/Create ========================================================================

        public ActionResult Create()
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.AccountId = new SelectList(currUser.Household.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name");
            ViewBag.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name");
            return View();
        }

        //============================================POST: Transactions/Create ========================================================================

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,Description,Date,Amount,TypeTransactionId,CategoryId,EnteredById,Reconciled,ReconciledAmount,EnteredBy_Id")] Transaction transaction)
        {
            //Account currAccount = db.Account.Find(transaction.AccountId);
            //if (transaction.TypeTransactionId == 2 || transaction.TypeTransactionId == 3) //Withdrawl or Transfer Action
            //{
            //    if(transaction.Amount > currAccount.Balance)
            //    {
            //        float overdraftAmount = currAccount.Balance - transaction.Amount;
            //        ViewBag.overdraftAmount = overdraftAmount;
            //        return RedirectToAction("OverdraftWarning");
            //    }
            //}


            if (ModelState.IsValid)
            {
                db.Transaction.Add(transaction);
                db.SaveChanges();

                bool overdraft = UpdateBalances(transaction);
                if (overdraft)
                {
                    return RedirectToAction("OverdraftWarning");
                }

                return RedirectToAction("Index");
            }

            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.AccountId = new SelectList(currUser.Household.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);
            ViewBag.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name", transaction.TypeTransactionId);
            return View(transaction);
        }

        //============================================ Overdraft ========================================================================

        public ActionResult OverdraftWarning()
        {
            return View();
        }

        //============================================ GET: Edit ========================================================================

        // GET: Transactions/Edit/5
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
            viewModel.AccountId = new SelectList(AccountDisplay, "Id", "InstitutionName");
            viewModel.CategoryId = new SelectList(db.Category, "Id", "Name");
            viewModel.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name");
            return View(viewModel);
        }

        //============================================ Post: Edit ========================================================================


        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "Id, AccountId, CategoryId, TypeTransactionId, Date, Amount")] Transaction transaction)
        {
            bool overdraft = new bool();

            if (ModelState.IsValid)
            {
                Transaction originalTransaction = db.Transaction.Find(transaction.Id);
                overdraft = BackOutTransaction(originalTransaction);
                overdraft = UpdateBalances(transaction);
                db.Entry(originalTransaction).CurrentValues.SetValues(transaction);
                db.SaveChanges();

                //if (overdraft)
                //{
                return Json(new { overdraft = overdraft });
                //}
                //return RedirectToAction("Index");
            }
            //ViewBag.AccountId = new SelectList(db.Account, "Id", "Name", transaction.AccountId);
            //ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", transaction.CategoryId);
            //ViewBag.TypeTransactionId = new SelectList(db.TypeTransaction, "Id", "Name", transaction.TypeTransactionId);
            return Json(new { overdraft = overdraft });
        }

        

        //============================================ Get: Delete ========================================================================

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        //============================================ Post: Delete ========================================================================

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transaction.Find(id);
            BackOutTransaction(transaction);

            db.Transaction.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        //============================================ Helper Functions ========================================================================

        //============================================ Write Functions ========================================================================
        public bool DepositFunds(float amount, int accountId, bool reconciled)
        {
            Account currAccount = db.Account.Find(accountId);
            currAccount.Balance = currAccount.Balance + amount;
            if (reconciled) { currAccount.ReconciledBalance += amount; }
            db.Entry(currAccount).State = EntityState.Modified;
            db.SaveChanges();

            if(currAccount.Balance < 0) { return true; }
            else { return false; }
        }

        public bool WithdrawFunds(float amount, int accountId, bool reconciled)
        {
            Account currAccount = db.Account.Find(accountId);
            currAccount.Balance = currAccount.Balance - amount;
            if (reconciled) { currAccount.ReconciledBalance -= amount; }
            db.Entry(currAccount).State = EntityState.Modified;
            db.SaveChanges();

            if (currAccount.Balance < 0) { return true; }
            else { return false; }
        }

        //============================================ Sorting Operation ========================================================================

        public bool UpdateBalances(Transaction transaction)
        {
            if (transaction.TypeTransactionId == 1 || transaction.TypeTransactionId == 4)
            {
                bool overdraft = DepositFunds(transaction.Amount, transaction.AccountId, transaction.Reconciled);
                return overdraft;
            }
            if (transaction.TypeTransactionId == 2 || transaction.TypeTransactionId == 3)
            {
                bool overdraft = WithdrawFunds(transaction.Amount, transaction.AccountId, transaction.Reconciled);
                return overdraft;
            }

            return false;
        }

        //============================================ Delete ========================================================================

        public bool BackOutTransaction (Transaction backOutTransaction)
        {
            backOutTransaction.Amount *= -1;
            bool overdraft = UpdateBalances(backOutTransaction);
            backOutTransaction.Amount *= -1;
            return overdraft;
        }

        

        //============================================ Garbage ========================================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
