using TransactionService.Models;

namespace TransactionService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        public static void SeedData(AppDbContext context)
        {
            if (!context.Transactions.Any())
            {
                context.AddRange(new Transaction()
                {
                    ClientId = 1,
                    Amount = 120,
                    TransactionDate = DateTime.Now.ToString("dd.MM.yyyy"),
                    PaymentMethod = "card",
                    Status = true,
                    OperationType = "buy",
                    BillId = 1
                });
            }
            context.SaveChanges();
        }
    }
}