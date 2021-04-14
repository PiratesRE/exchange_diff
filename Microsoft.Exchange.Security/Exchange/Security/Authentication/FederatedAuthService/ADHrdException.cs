using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class ADHrdException : Exception
	{
		public ADHrdException(string message) : base(message)
		{
		}
	}
}
