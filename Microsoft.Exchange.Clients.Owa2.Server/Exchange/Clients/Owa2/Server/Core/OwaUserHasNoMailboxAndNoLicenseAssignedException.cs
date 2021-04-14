using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaUserHasNoMailboxAndNoLicenseAssignedException : OwaPermanentException
	{
		public OwaUserHasNoMailboxAndNoLicenseAssignedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
