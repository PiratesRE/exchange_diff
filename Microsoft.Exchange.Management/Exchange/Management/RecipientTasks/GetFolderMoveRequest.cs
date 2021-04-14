using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "FolderMoveRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetFolderMoveRequest : GetRequest<FolderMoveRequestIdParameter, FolderMoveRequest>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[ValidateNotNull]
		public MailboxIdParameter SourceMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["SourceMailbox"];
			}
			set
			{
				base.Fields["SourceMailbox"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public MailboxIdParameter TargetMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.FolderMove;
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceMailboxUser = null;
			this.targetMailboxUser = null;
			base.InternalStateReset();
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
			if (this.sourceMailboxUser != null)
			{
				requestIndexEntryQueryFilter.SourceMailbox = this.sourceMailboxUser.Id;
			}
			if (this.targetMailboxUser != null)
			{
				requestIndexEntryQueryFilter.TargetMailbox = this.targetMailboxUser.Id;
			}
			return requestIndexEntryQueryFilter;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.SourceMailbox != null)
				{
					this.sourceMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.SourceMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.SourceMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.SourceMailbox.ToString())), ExchangeErrorCategory.Client);
				}
				if (this.TargetMailbox != null)
				{
					this.targetMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), ExchangeErrorCategory.Client);
				}
				base.InternalValidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		public const string ParameterSourceMailbox = "SourceMailbox";

		public const string ParameterTargetMailbox = "TargetMailbox";

		private ADUser sourceMailboxUser;

		private ADUser targetMailboxUser;
	}
}
