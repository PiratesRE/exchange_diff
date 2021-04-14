using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class PerTenantOrganizationMailboxDatabases : TenantConfigurationCacheableItemBase
	{
		public IList<ADObjectId> Databases
		{
			get
			{
				return this.databases;
			}
		}

		public override long ItemSize
		{
			get
			{
				if (this.databases == null)
				{
					throw new InvalidOperationException("ItemSize is invokes before the object is initialized");
				}
				return (long)(IntPtr.Size + 50 * this.databases.Count);
			}
		}

		public override bool InitializeWithoutRegistration(IConfigurationSession session, bool allowExceptions)
		{
			throw new NotSupportedException("InitializeWithoutRegistration is not supported in PerTenantOrganizationMailboxDatabases");
		}

		public override bool TryInitialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.Initialize(organizationId, cacheNotificationHandler, state);
			}, 0);
			if (adoperationResult.Succeeded)
			{
				return true;
			}
			ProxyHubSelectorComponent.Tracer.TraceError<OrganizationId, object>(0L, "Failed to read organization mailboxes for organization <{0}>; exception details: {1}", organizationId, adoperationResult.Exception ?? "<none>");
			return false;
		}

		public override bool Initialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			ADUser[] array = OrganizationMailbox.FindByOrganizationId(organizationId, OrganizationCapability.MailRouting);
			if (array != null && array.Length > 0)
			{
				HashSet<ADObjectId> source = new HashSet<ADObjectId>(from mailbox in array
				select mailbox.Database into databaseId
				where databaseId != null
				select databaseId);
				this.databases = Array.AsReadOnly<ADObjectId>(source.ToArray<ADObjectId>());
			}
			else
			{
				this.databases = PerTenantOrganizationMailboxDatabases.NoDatabases;
			}
			if (this.databases.Count == 0)
			{
				ProxyHubSelectorComponent.Tracer.TraceError<OrganizationId>(0L, "Failed to find any organization mailboxes with non-null databases for organization <{0}>", organizationId);
			}
			return true;
		}

		private const int EstimatedADObjectIdSize = 50;

		private static readonly IList<ADObjectId> NoDatabases = Array.AsReadOnly<ADObjectId>(new ADObjectId[0]);

		private IList<ADObjectId> databases;
	}
}
