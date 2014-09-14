module SimpleLoanCalculator

open System

[<CLIMutable>]
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