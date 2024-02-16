namespace HomeBanking.Models
{
    public class DbInitializer 
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                    new Client { Email = "jcaballero@gmail.com", FirstName="Julian", LastName="Caballero", Password="123456"}
                };

                context.Clients.AddRange(clients);

                //guardamos
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                if (accountVictor != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 0 },
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now.AddDays(-2), Number = "VIN002", Balance = 5 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();

                }

                var accountJulian = context.Clients.FirstOrDefault(c => c.Email == "jcaballero@gmail.com");
                if (accountJulian != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountJulian.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 0 },
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();

                }
            }
        }
    }
}
