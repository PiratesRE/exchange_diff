using System;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal interface IsMemberOfResolverConfig
	{
		bool Enabled { get; }

		long ResolvedGroupsMaxSize { get; }

		TimeSpan ResolvedGroupsExpirationInterval { get; }

		TimeSpan ResolvedGroupsCleanupInterval { get; }

		TimeSpan ResolvedGroupsPurgeInterval { get; }

		TimeSpan ResolvedGroupsRefreshInterval { get; }

		long ExpandedGroupsMaxSize { get; }

		TimeSpan ExpandedGroupsExpirationInterval { get; }

		TimeSpan ExpandedGroupsCleanupInterval { get; }

		TimeSpan ExpandedGroupsPurgeInterval { get; }

		TimeSpan ExpandedGroupsRefreshInterval { get; }
	}
}
