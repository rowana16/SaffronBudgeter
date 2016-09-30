using Microsoft.AspNet.Identity;
using Saffron.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saffron.Helpers
{
    public class ControllerHelpers
    {
        // =========================  HelperFunctions ===========================================
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<AccountKey> GetAccountDisplay(ApplicationUser currUser)
        {
            //List<AccountKey> AccountDisplay = currUser.Household.Accounts.ToList();
            List<AccountKey> AccountDisplay = new List<AccountKey>();
            foreach (var account in currUser.Household.Accounts)
            {
                AccountKey currKey = new AccountKey();
                currKey.Id = account.Id;
                currKey.InstitutionName = account.Institution.Name + " " + account.AccountType.Name;
                AccountDisplay.Add(currKey);
            }

            return AccountDisplay;

        }

        public List<Transaction> GetTransactions(ApplicationUser currUser)
        {
            List<Transaction> allTransactions = db.Transaction.Where(i => i.Account.HouseholdId == currUser.HouseholdId).ToList();
            //List<Transaction> currTransactions = new List<Transaction>();
            //foreach (var currTransaction in allTransactions)
            //{

            //    if (currTransaction.Account.HouseholdId == currUser.HouseholdId)
            //    {
            //        Transaction currKey = new Transaction();
            //        currKey.InstitutionName = currTransaction.Account.Institution.Name + " " + currTransaction.Account.AccountType.Name;
            //        currKey.Category.Name = currTransaction.Category.Name;
            //        currKey.Type.Name = currTransaction.Type.Name;
            //        currKey.Date = currTransaction.Date;
            //        currKey.Amount = currTransaction.Amount;

            //        currTransactions.Add(currKey);
            //    }
            //}
            return allTransactions;
        }

        ////================================  Collect Save Errors ==========================================

        //public IdentityResult SaveChangesWithErrors(ApplicationUser user, string password)
        //{
        //    try
        //    {
        //        var result = UserManager.CreateAsync(user, password);
                
        //        return result;
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        foreach (var failure in ex.EntityValidationErrors)
        //        {
        //            sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
        //            foreach (var error in failure.ValidationErrors)
        //            {
        //                sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
        //                sb.AppendLine();
        //            }
        //        }

        //        throw new DbEntityValidationException(
        //            "Entity Validation Failed - errors follow:\n" +
        //            sb.ToString(), ex
        //        ); // Add the original exception as the innerException
        //    }
        //}





    }
}