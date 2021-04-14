using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADRootOrganizationRecipientSession
	{
		SecurityIdentifier GetExchangeServersUsgSid();
	}
}
