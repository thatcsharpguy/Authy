using System;
using System.Linq;
using System.Collections.Generic;
using Authy.Acc;
using Authy.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AccountsImplementation))]

namespace Authy.Droid
{
	public class AccountsImplementation : IAccounts
	{
		public Dictionary<string,AuthyAccount> Accounts
		{
			get
			{
				var accounts = new Dictionary<string, AuthyAccount>();
				var accountStore = Xamarin.Auth.AccountStore.Create();
				foreach (var service in Enum.GetNames(typeof(Service)))
				{
					var acc = accountStore.FindAccountsForService(service).FirstOrDefault();
					if (acc != null)
					{
						accounts.Add(service, new AuthyAccount( acc.ToString()));
					}
				}
				return accounts;
			}
		}
	}
}
