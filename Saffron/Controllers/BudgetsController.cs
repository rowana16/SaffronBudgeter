using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Saffron.Models
{
   

    public class BudgetsController : Controller
    {
        

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index(DateTime? start, DateTime? end)
        {
            if (start == null)
            {
                start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (end == null)
            {
                end = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);
            }

            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            if (currUser == null) { return RedirectToAction("Login", "Account"); }
            BudgetTransactionViewModel viewModel = new BudgetTransactionViewModel();
            DateTime currMonth = DateTime.Today.AddDays(-30);

            //viewModel.Budgets = currUser.Household.Budgets.ToList();
            viewModel.BudgetItems = db.BudgetItem.Where(o => o.Budget.HouseholdId == currUser.HouseholdId).OrderBy(o => o.Category.Name).ToList();
            viewModel.Transactions = db.Transaction.Where(o => o.Account.HouseholdId == currUser.HouseholdId && o.Date < end && o.Date > start).ToList();
            viewModel.GraphData = Stringify(viewModel.BudgetItems);
            viewModel.GraphData = AddTransactions(viewModel.GraphData, viewModel.Transactions);
           
            return View(viewModel);
        }


        //========================== Graph Helper ========================================

        private List<BudgetViewItem> Stringify(List<BudgetItem> items)
        {
            List<BudgetViewItem> returnObj = new List<BudgetViewItem>();
           
            
            foreach (var item in items)
            {
                BudgetViewItem itemList = CloneObject(item);         
                
                returnObj.Add(itemList);
               
            }

            return returnObj;
        }

        private BudgetViewItem CloneObject(BudgetItem item)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            BudgetViewItem itemList = new BudgetViewItem();
            itemList.BudgetItemId = item.Id;
            itemList.CategoryName = item.Category.Name;
            itemList.CategoryId = item.CategoryId;
            itemList.BudgetTotal = item.Amount;
            itemList.TotalValue = javaScriptSerializer.Serialize(item.Amount);
            return itemList;
        }

        private List<BudgetViewItem> AddTransactions(List<BudgetViewItem> items, List<Transaction> transactions)
        {

            for (int i = 0; i < items.Count; i++)
            {
                items[i] = LinkTransactionsItem(items[i], transactions);
            }   
                
            

            return items;
        }

        private BudgetViewItem LinkTransactionsItem(BudgetViewItem item, List<Transaction> transactions)
        {
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            float sum = 0;
            int count = 0;

            foreach (var iTransaction in transactions)
            {


                if (item.CategoryId == iTransaction.CategoryId)
                {
                    if (iTransaction.TypeTransactionId == 1 || iTransaction.TypeTransactionId == 4)
                    {
                        sum -= iTransaction.Amount;  //  Deposits are negative spending
                        count++;
                    }

                    if (iTransaction.TypeTransactionId == 2 || iTransaction.TypeTransactionId == 3)
                    {
                        sum += iTransaction.Amount;  //  Withdrawls are positive spending
                        count++;
                    }
                }
            }

            sum = (sum / item.BudgetTotal) * 100;
            item.SumValue = javaScriptSerializer.Serialize(sum);
            item.TransactionCount = count;
            item.TotalValue = javaScriptSerializer.Serialize(100);
            return item;
        }

        public ActionResult GraphTest()
        {
            return View();
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BudgetItem budgetItem = db.BudgetItem.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            BudgetViewItem viewItem = CloneObject(budgetItem);
            //viewItem.CategoryName = budgetItem.Category.Name;
            //viewItem.BudgetItemId = budgetItem.Id;
            //viewItem.BudgetTotal = budgetItem.Amount;         
            ViewBag.Transactions = db.Transaction.Where(o => o.Account.HouseholdId == currUser.HouseholdId && o.CategoryId == budgetItem.CategoryId).ToList();
            viewItem = LinkTransactionsItem(viewItem, ViewBag.Transactions);  

            return View(viewItem);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.HouseholdId = currUser.HouseholdId;
            if(currUser.Household.Budgets.Count() == 0)
            {
                Budget newBudget = new Budget();
                newBudget.HouseholdId = (int)currUser.HouseholdId;
                newBudget.Name = currUser.LastName;
                db.Budget.Add(newBudget);
                db.SaveChanges();
            }
            ViewBag.BudgetId = currUser.Household.Budgets.First().Id;
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name");
            ViewBag.RepeatFrequencyId = new SelectList(db.RepeatFrequency, "Id", "Frequency");
            
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime DayOfMonth, int RepeatFrequencyId, int CategoryId, float Amount, int BudgetId)
        {
            // check for existing budget for set category
            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            int categorySum = 0;
            int budgetItemId = 0;
            foreach (Budget userBudget in currUser.Household.Budgets)
            {
                int count = 0;
                count = db.BudgetItem.Where(i => i.CategoryId == CategoryId && userBudget.Id == i.BudgetId).Count();
                categorySum += count;
                if (count > 0)
                {
                    budgetItemId = db.BudgetItem.Where(i => i.CategoryId == CategoryId && userBudget.Id == i.BudgetId).FirstOrDefault().Id;
                }
            }

            if (categorySum > 0)
            {
                return RedirectToAction("CreateError", new { id = budgetItemId });
            }
            BudgetItem budget = new BudgetItem();

            budget.DayOfMonth = DayOfMonth.Day;
            budget.BudgetId = BudgetId;
            budget.RepeatFrequencyId = RepeatFrequencyId;
            budget.CategoryId = CategoryId;
            budget.Amount = Amount;

            if (ModelState.IsValid)
            {
                db.BudgetItem.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           
            ViewBag.HouseholdId = currUser.HouseholdId;
            ViewBag.BudgetId = currUser.Household.Budgets.First().Id;
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name");
            ViewBag.RepeatFrequencyId = new SelectList(db.RepeatFrequency, "Id", "Frequency");
            return View();
        }

        public ActionResult CreateError(int? id)
        {
            ViewBag.Id = id;
            return View();
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }

            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.HouseholdId = currUser.HouseholdId;
            ViewBag.BudgetId = currUser.Household.Budgets.First().Id;
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", budgetItem.CategoryId);
            ViewBag.RepeatFrequencyId = new SelectList(db.RepeatFrequency, "Id", "Frequency", budgetItem.RepeatFrequencyId);
            return View(budgetItem);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( int CategoryId, float Amount, int RepeatFrequencyId, int DayOfMonth, int BudgetId, int Id)
        {
            BudgetItem budgetItem = new BudgetItem();
            budgetItem.Amount = Amount;
            budgetItem.CategoryId = CategoryId;
            budgetItem.RepeatFrequencyId = RepeatFrequencyId;
            budgetItem.DayOfMonth = DayOfMonth;
            budgetItem.BudgetId = BudgetId;
            budgetItem.Id = Id;

            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ApplicationUser currUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.HouseholdId = currUser.HouseholdId;
            ViewBag.BudgetId = currUser.Household.Budgets.First().Id;
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "Name", budgetItem.CategoryId);
            ViewBag.RepeatFrequencyId = new SelectList(db.RepeatFrequency, "Id", "Frequency", budgetItem.RepeatFrequencyId);
            return View(budgetItem);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budget = db.BudgetItem.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            db.BudgetItem.Remove(budgetItem);
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
