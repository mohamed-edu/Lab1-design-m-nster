// Använda designmönster:
// 1. Factory Method - Skapar olika typer av konton
// 2. Singleton - För att hantera bankomatens centrala logik (ATMController)
// 3. Strategy - För att hantera olika uttagsstrategier
// ---
// 

using System;
using System.Collections.Generic;

// Singleton - ATMController hanterar alla transaktioner och användarhantering
public class ATMController
{
	private static ATMController _instance;
	private Dictionary<string, Account> accounts = new Dictionary<string, Account>();

	private ATMController() { }

	public static ATMController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ATMController();
			}
			return _instance;
		}
	}

	public void RegisterAccount(string username, Account account)
	{
		accounts[username] = account;
	}

	public Account Login(string username)
	{
		return accounts.ContainsKey(username) ? accounts[username] : null;
	}
}

// Strategy - Olika strategier för uttag
public interface IWithdrawStrategy
{
	void Withdraw(Account account, decimal amount);
}

public class NormalWithdrawStrategy : IWithdrawStrategy
{
	public void Withdraw(Account account, decimal amount)
	{
		if (account.Balance >= amount)
		{
			account.Balance -= amount;
			Console.WriteLine($"Du har tagit ut {amount}. Ny balans: {account.Balance}");
		}
		else
		{
			Console.WriteLine("Otillräcklig balans.");
		}
	}
}

public class FastWithdrawStrategy : IWithdrawStrategy
{
	public void Withdraw(Account account, decimal amount)
	{
		const decimal fastWithdrawLimit = 500;
		if (amount > fastWithdrawLimit)
		{
			Console.WriteLine($"Snabbuttag är begränsat till {fastWithdrawLimit}.");
		}
		else if (account.Balance >= amount)
		{
			account.Balance -= amount;
			Console.WriteLine($"Du har snabbt tagit ut {amount}. Ny balans: {account.Balance}");
		}
		else
		{
			Console.WriteLine("Otillräcklig balans.");
		}
	}
}

// Factory Method - Skapa olika konton
public abstract class Account
{
	public decimal Balance { get; set; }
}

public class SavingsAccount : Account
{
	public SavingsAccount()
	{
		Balance = 1000; // Exempel på startbalans
	}
}

public class CheckingAccount : Account
{
	public CheckingAccount()
	{
		Balance = 500; // Exempel på startbalans
	}
}

public class AccountFactory
{
	public static Account CreateAccount(string accountType)
	{
		switch (accountType)
		{
			case "Savings":
				return new SavingsAccount();
			case "Checking":
				return new CheckingAccount();
			default:
				throw new ArgumentException("Okänt kontotyp.");
		}
	}
}

class Program
{
	static void Main(string[] args)
	{
		// Skapa Singleton-instans av ATMController
		var atmController = ATMController.Instance;

		// Skapa konton via Factory Method
		Account savings = AccountFactory.CreateAccount("Savings");
		Account checking = AccountFactory.CreateAccount("Checking");

		// Registrera konton
		atmController.RegisterAccount("user1", savings);
		atmController.RegisterAccount("user2", checking);

		// Logga in och välj konto
		Console.WriteLine("Ange användarnamn user1 eller user2:");
		string username = Console.ReadLine();
		Account userAccount = atmController.Login(username);

		if (userAccount == null)
		{
			Console.WriteLine("Fel användarnamn.");
			return;
		}

		// Välj strategi
		Console.WriteLine("Välj uttagsstrategi: 1 för Normal Uttag, 2 för Snabb Uttag");
		int strategyChoice = int.Parse(Console.ReadLine());

		IWithdrawStrategy withdrawStrategy = strategyChoice == 1
			? new NormalWithdrawStrategy()
			: (IWithdrawStrategy)new FastWithdrawStrategy();

		// Gör uttag
		Console.WriteLine("Ange belopp att ta ut:");
		decimal amount = decimal.Parse(Console.ReadLine());
		withdrawStrategy.Withdraw(userAccount, amount);
	}
}

