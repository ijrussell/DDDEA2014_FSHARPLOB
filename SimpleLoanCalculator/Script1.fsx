//code copied from Phil Trelford's blog

open System

type Loan = {
   PurchasePrice : decimal
   DownPayment : decimal
   InterestRate : double
   TermMonths : int
   }

let calculateMonthlyPayment (loan:Loan) =
   let monthsPerYear = 12
   let rate = (loan.InterestRate / double monthsPerYear) / 100.0
   let factor = rate + (rate / (Math.Pow(1.+rate, double loan.TermMonths)))
   let amount = loan.PurchasePrice - loan.DownPayment
   let payment = amount * decimal factor
   Math.Round(payment,2)

let loan = {
   PurchasePrice = 50000M
   DownPayment = 0M
   InterestRate = 6.0
   TermMonths = 5 * 12
   }

calculateMonthlyPayment loan

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