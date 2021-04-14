using System;

namespace Microsoft.Exchange.Security.Authentication
{
	public interface IAccountValidationContext
	{
		AccountState CheckAccount();
	}
}
