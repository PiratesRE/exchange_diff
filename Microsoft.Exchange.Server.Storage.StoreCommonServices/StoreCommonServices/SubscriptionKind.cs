using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	public enum SubscriptionKind : uint
	{
		PreCommit = 2U,
		PostCommit = 12U
	}
}
