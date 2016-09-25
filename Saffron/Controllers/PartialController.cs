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