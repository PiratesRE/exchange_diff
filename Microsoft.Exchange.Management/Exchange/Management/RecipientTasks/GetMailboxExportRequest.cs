using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxExportRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxExportRequest : GetRequest<MailboxExportRequestIdParameter, MailboxExportRequest>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[ValidateNotNull]
		public MailboxOrMailUserIdParameter Mailbox
		{
			get
			{
				return base.InternalMailbox;
			}
			set
			{
				base.InternalMailbox = value;
			}
		}

		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.MailboxExport;
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
