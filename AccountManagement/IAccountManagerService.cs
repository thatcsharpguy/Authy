using System;
using System.Collections.Generic;

namespace Authy.AccountManagement
{
	public interface IAccountManagerService
	{

		List<Services> Accounts { get; }

        string GetPropertyFromAccount(Services service, string property);

        void EraseAll();

    }


}
