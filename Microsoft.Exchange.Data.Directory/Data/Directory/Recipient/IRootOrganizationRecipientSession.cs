using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IRootOrganizationRecipientSession : IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		SecurityIdentifier GetExchangeServersUsgSid();
	}
}
