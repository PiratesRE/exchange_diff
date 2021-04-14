using System;

namespace Microsoft.Exchange.Common.Reputation
{
	public enum ReputationEntityType : byte
	{
		None,
		IP,
		Domain,
		Tenant,
		Max
	}
}
