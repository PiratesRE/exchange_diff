using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncArbitrationMailboxADObjectCache : LazyLookupTimeoutCache<Guid, ADUser>
	{
		public SyncArbitrationMailboxADObjectCache() : base(10, 1000, false, TimeSpan.FromHours(24.0))
		{
		}

		protected override ADUser CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromExternalDirectoryOrganizationId(key), 42, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\UnifiedPolicy\\SyncArbitrationMailboxADObjectCache.cs");
			return MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession);
		}
	}
}
