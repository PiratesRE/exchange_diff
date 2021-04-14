using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory.ExchangeDirectory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TenantPartitionHintAdapter
	{
		public TenantPartitionHintAdapter(Guid externalDirectoryOrganizationId, bool isConsumer)
		{
			this.externalDirectoryOrganizationId = externalDirectoryOrganizationId;
			this.isConsumer = isConsumer;
		}

		public virtual Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return this.externalDirectoryOrganizationId;
			}
		}

		public virtual bool IsConsumer
		{
			get
			{
				return this.isConsumer;
			}
		}

		public static TenantPartitionHintAdapter FromPartitionHint(TenantPartitionHint partitionHint)
		{
			return new TenantPartitionHintAdapter(partitionHint.GetExternalDirectoryOrganizationId(), partitionHint.IsConsumer());
		}

		public static TenantPartitionHintAdapter FromPersistableTenantPartitionHint(byte[] persistedData)
		{
			TenantPartitionHint partitionHint;
			if (persistedData == null)
			{
				partitionHint = TenantPartitionHint.FromOrganizationId(OrganizationId.ForestWideOrgId);
			}
			else
			{
				partitionHint = TenantPartitionHint.FromPersistablePartitionHint(persistedData);
			}
			return TenantPartitionHintAdapter.FromPartitionHint(partitionHint);
		}

		private readonly Guid externalDirectoryOrganizationId;

		private readonly bool isConsumer;
	}
}
