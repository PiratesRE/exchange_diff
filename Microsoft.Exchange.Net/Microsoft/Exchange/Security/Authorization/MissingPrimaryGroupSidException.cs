using System;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class MissingPrimaryGroupSidException : AuthzException
	{
		public MissingPrimaryGroupSidException(string message) : base(message)
		{
		}
	}
}
