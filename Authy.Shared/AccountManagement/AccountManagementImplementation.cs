using System;
using System.Linq;
using System.Collections.Generic;
using Authy.AccountManagement;

#if __IOS__
[assembly: Xamarin.Forms.Dependency(typeof(Authy.iOS.AccountManagement.AccountManagementImplementation))]
namespace Authy.iOS.AccountManagement
#elif __ANDROID__
[assembly: Xamarin.Forms.Dependency(typeof(Authy.Droid.AccountManagement.AccountManagementImplementation))]
namespace Authy.Droid.AccountManagement
#endif
{
	public class AccountManagementImplementation : IAccountManagerService
	{
		public static void Init() { var now = DateTime.Now; }

		public List<Services> Accounts
		{
			get
			{
				var accounts = new List<Services>();
				var accountStore = Xamarin.Auth.AccountStore.Create();
				foreach (var service in Enum.GetNames(typeof(Services)))
				{
					var acc = accountStore.FindAccountsForService(service).FirstOrDefault();
					if (acc != null)
					{
						accounts.Add((Services)Enum.Parse(typeof(Services), service));
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

