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
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            AccountViewModel viewModel = new AccountViewModel();
            if(currUser != null)
            {
                viewModel.Accounts = currUser.Household.Accounts.ToList();
                viewModel.Types = db.AccountType.ToList();
                return View(viewModel);
            }



            return RedirectToAction("index", "Budgets");
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            AccountDetailViewModel viewModel = new AccountDetailViewModel();
            ControllerHelpers helper = new ControllerHelpers();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            viewModel.Account = db.Account.Find(id);
            viewModel.Transactions = db.Transaction.Where(db => db.AccountId == viewModel.Account.Id).ToList();

            if (viewModel.Account == null)
            {
                return HttpNotFound();
            }

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

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name");
            ViewBag.AccountType = new SelectList(db.AccountType, "Id", "Name");
            ViewBag.InstitutionName = new SelectList(db.Institution, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int AccountType, int Institution, float Balance, Account newAccount)
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            newAccount.HouseholdId = (int)currUser.HouseholdId;
            newAccount.AccountTypeId = AccountType;
            newAccount.InstitutionId = Institution;
            newAccount.Name = db.Institution.Find(Institution).Name + " " + db.AccountType.Find(AccountType).Name;
            newAccount.Balance = Balance;



            
            db.Account.Add(newAccount);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = newAccount.Id });

            //ViewBag.AccountType = new SelectList(db.AccountType, "Id", "Name");
            //ViewBag.InstitutionName = new SelectList(db.Institution, "Id", "Name");
            //return View();
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name");
            ViewBag.AccountType = new SelectList(db.AccountType, "Id", "Name");
            ViewBag.InstitutionName = new SelectList(db.Institution, "Id", "Name");
            
            return View(account);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,ReconciledBalance,HouseholdId,InstitutionId,AccountTypeId")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details",new { id = account.Id } );
            }
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Account.Find(id);
            db.Account.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
