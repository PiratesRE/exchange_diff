using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public enum ResolverType
	{
		None,
		Recipient,
		OrganizationUnitIdentity,
		SidToRecipient,
		RetentionPolicyTag,
		Server
	}
}
