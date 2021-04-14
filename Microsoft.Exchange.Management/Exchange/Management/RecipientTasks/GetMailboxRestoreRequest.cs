using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxRestoreRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxRestoreRequest : GetRequest<MailboxRestoreRequestIdParameter, MailboxRestoreRequest>
	{
		public GetMailboxRestoreRequest()
		{
			this.targetUserId = null;
			this.sourceDatabaseId = null;
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "Filtering")]
		public DatabaseIdParameter SourceDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["SourceDatabase"];
			}
			set
			{
				base.Fields["SourceDatabase"] = value;
			}
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

		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.MailboxRestore;
			}
		}

		protected override void InternalStateReset()
		{
			this.targetUserId = null;
			this.sourceDatabaseId = null;
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
			if (this.targetUserId != null)
			{
				requestIndexEntryQueryFilter.TargetMailbox = this.targetUserId;
			}
			if (this.sourceDatabaseId != null)
			{
				requestIndexEntryQueryFilter.SourceDatabase = this.sourceDatabaseId;
			}
			return requestIndexEntryQueryFilter;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.TargetMailbox != null)
			{
				ADUser aduser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.TargetMailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
				this.targetUserId = aduser.Id;
			}
			if (this.SourceDatabase != null)
			{
				this.SourceDatabase.AllowLegacy = true;
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.SourceDatabase, base.ConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.SourceDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.SourceDatabase.ToString())));
				this.sourceDatabaseId = mailboxDatabase.Id;
			}
			TaskLogger.LogExit();
		}

		public const string ParameterSourceDatabase = "SourceDatabase";

		public const string ParameterTargetMailbox = "TargetMailbox";

		private ADObjectId sourceDatabaseId;

		private ADObjectId targetUserId;
	}
}
