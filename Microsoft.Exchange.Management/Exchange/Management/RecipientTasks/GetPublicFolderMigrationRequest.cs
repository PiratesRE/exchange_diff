using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "PublicFolderMigrationRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderMigrationRequest : GetRequest<PublicFolderMigrationRequestIdParameter, PublicFolderMigrationRequest>
	{
		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.PublicFolderMigration;
			}
		}

		protected override RequestIndexEntryQueryFilter InternalFilterBuilder()
		{
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = base.InternalFilterBuilder();
			if (requestIndexEntryQueryFilter == null)
			{
				requestIndexEntryQueryFilter = new RequestIndexEntryQueryFilter();
			}
			requestIndexEntryQueryFilter.RequestType = this.RequestType;
			requestIndexEntryQueryFilter.IndexId = new RequestIndexId(RequestIndexLocation.AD);
			return requestIndexEntryQueryFilter;
		}
	}
}
