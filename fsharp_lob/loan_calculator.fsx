open System

/// Loan record
type Loan = {
   /// The total purchase price of the item being paid for.
   PurchasePrice : decimal
   /// The total down payment towards the item being purchased.
   DownPayment : decimal
   /// The annual interest rate to be charged on the loan
   InterestRate : double
   /// The term of the loan in months. This is the number of months
   /// that payments will be made.
   TermMonths : int
   }

/// Calculates montly payment amount
let calculateMonthlyPayment (loan:Loan) =
   let monthsPerYear = 12
   let rate = (loan.InterestRate / double monthsPerYear) / 100.0
   let factor = rate + (rate / (Math.Pow(rate+1., double loan.TermMonths)))
   let amount = loan.PurchasePrice - loan.DownPayment
   let payment = amount * decimal factor
   Math.Round(payment,2)
 
///We can test the function immediately in F# interactive
let loan = {
   PurchasePrice = 50000M
   DownPayment = 0M
   InterestRate = 6.0
   TermMonths = 5 * 12
   }

calculateMonthlyPayment loan
 
///Then a test run (which produces the same results as the original code):
let displayLoanInformation (loan:Loan) =
   printfn "Purchase Price: %M" loan.PurchasePrice
   printfn "Down Payment: %M" loan.DownPayment
   printfn "Loan Amount: %M" (loan.PurchasePrice - loan.DownPayment)
   printfn "Annual Interest Rate: %f%%" loan.InterestRate
   printfn "Term: %d months" loan.TermMonths
   printfn "Monthly Payment: %f" (calculateMonthlyPayment loan)
   printfn ""

for i in 0M .. 1000M .. 10000M do
   let loan = { loan with DownPayment = i }
   displayLoanInformation loan
