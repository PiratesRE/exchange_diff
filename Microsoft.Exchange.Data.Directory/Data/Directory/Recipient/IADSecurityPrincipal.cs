using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADSecurityPrincipal
	{
		bool IsSecurityPrincipal { get; }

		string SamAccountName { get; set; }

		SecurityIdentifier Sid { get; }

		SecurityIdentifier MasterAccountSid { get; }

		MultiValuedProperty<SecurityIdentifier> SidHistory { get; }
	}
}
