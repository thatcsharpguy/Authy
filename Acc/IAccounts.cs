using System;
using System.Collections.Generic;

namespace Authy.Acc
{
	public interface IAccounts
	{

		Dictionary<string, AuthyAccount> Accounts { get; }

	}


}
