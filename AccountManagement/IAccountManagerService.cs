using System;
using System.Collections.Generic;

namespace Authy.AccountManagement
{
	public interface IAccountManagerService
	{

		Dictionary<Services, AuthyAccount> Accounts { get; }

        string GetPropertyFromAccount(Services service, string property);


        void EraseAll();

    }


}
