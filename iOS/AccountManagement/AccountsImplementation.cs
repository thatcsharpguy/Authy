using System;
using System.Linq;
using System.Collections.Generic;
using Authy.AccountManagement;
using Authy.iOS.AccountManagement;
using Authy.AccountManagement;

[assembly: Xamarin.Forms.Dependency(typeof(AccountsImplementation))]

namespace Authy.iOS.AccountManagement
{
	public class AccountsImplementation : IAccountManagerService
	{
		public Dictionary<Services, AuthyAccount> Accounts
		{
			get
			{
				var accounts = new Dictionary<Services, AuthyAccount>();
				var accountStore = Xamarin.Auth.AccountStore.Create();
				foreach (var service in Enum.GetNames(typeof(Services)))
				{
					var acc = accountStore.FindAccountsForService(service).FirstOrDefault();
					if (acc != null)
                    {
                        accounts.Add((Services)Enum.Parse(typeof(Services), service), new AuthyAccount(acc.ToString()));
                    }
				}
				return accounts;
			}
		}

        public void EraseAll()
        {
            var accountStore = Xamarin.Auth.AccountStore.Create();
            foreach (var service in Enum.GetNames(typeof(Services)))
            {
                var acc = accountStore.FindAccountsForService(service).FirstOrDefault();
                if (acc != null)
                {
                    accountStore.Delete(acc, service);
                }
            }
        }

        public string GetPropertyFromAccount(Services service, string property)
        {
			var accountStore = Xamarin.Auth.AccountStore.Create();
			var account = accountStore.FindAccountsForService(service.ToString()).FirstOrDefault();
			if (account != null)
			{
				return account.Properties[property];
			}
			return null;
        }
    }
}
