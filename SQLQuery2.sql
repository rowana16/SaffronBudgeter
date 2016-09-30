/****** Script for SelectTopNRows command from SSMS  ******/
SELECT t.Id, t.date, t.CategoryId, t.Amount, t.TypeTransactionId
  FROM [arowan-budgeter].[dbo].[Transactions]	t
  ,[dbo].[Accounts]								a
  ,[dbo].[Households]							h
 where t.AccountId = a.Id
 and a.HouseholdId = h.Id
 and h.Id = 9
 
 and t.Date > CONVERT(date, '09/01/2016')
 /*and t.Date < '09/31/2016'*/
 
 order by CategoryId