using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var loan = new SimpleLoanCalculator.Loan(50000m, 4000m, 6.0, 60);
            var loan = new SimpleLoanCalculator.Loan
            {
                PurchasePrice = 50000m,
                DownPayment = 0m,
                InterestRate = 6.0,
                TermMonths = 60
            };

            var result = SimpleLoanCalculator.calculateMonthlyPayment(loan);

            Console.WriteLine(result);

            Console.ReadLine();
        }
    }
}
