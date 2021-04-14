using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MergeRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetMergeRequest : GetRequest<MergeRequestIdParameter, MergeRequest>
	{
		public GetMergeRequest()
		{
			this.sourceUserId = null;
			this.targetUserId = null;
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public MailboxOrMailUserIdParameter TargetMailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		[ValidateNotNull]
		public MailboxOrMailUserIdParameter SourceMailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["SourceMailbox"];
			}
			set
			{
				base.Fields["SourceMailbox"] = value;
			}
		}

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
				return MRSRequestType.Merge;
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceUserId = null;
			this.targetUserId = null;
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
			if (this.sourceUserId != null)
			{
				requestIndexEntryQueryFilter.SourceMailbox = this.sourceUserId;
			}
			if (this.targetUserId != null)
			{
				requestIndexEntryQueryFilter.TargetMailbox = this.targetUserId;
			}
			return requestIndexEntryQueryFilter;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.SourceMailbox != null)
			{
				ADUser aduser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.SourceMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
				this.sourceUserId = aduser.Id;
			}
			if (this.TargetMailbox != null)
			{
				ADUser aduser2 = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.TargetMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
				this.targetUserId = aduser2.Id;
			}
			TaskLogger.LogExit();
		}

		public const string ParameterTargetMailbox = "TargetMailbox";

		public const string ParameterSourceMailbox = "SourceMailbox";

		private ADObjectId sourceUserId;

		private ADObjectId targetUserId;
	}
}
