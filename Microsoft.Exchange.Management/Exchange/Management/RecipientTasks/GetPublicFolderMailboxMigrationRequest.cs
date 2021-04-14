using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "PublicFolderMailboxMigrationRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderMailboxMigrationRequest : GetRequest<PublicFolderMailboxMigrationRequestIdParameter, PublicFolderMailboxMigrationRequest>
	{
		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.PublicFolderMailboxMigration;
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

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";
	}
}
