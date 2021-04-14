using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SyncRequest", DefaultParameterSetName = "Identity")]
	public sealed class GetSyncRequest : GetRequest<SyncRequestIdParameter, SyncRequest>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Filtering", ValueFromPipeline = true)]
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

		protected override RequestIndexId DefaultRequestIndexId
		{
			get
			{
				return new RequestIndexId(this.targetUser.Id);
			}
		}

		protected override MRSRequestType RequestType
		{
			get
			{
				return MRSRequestType.Sync;
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
			requestIndexEntryQueryFilter.IndexId = this.DefaultRequestIndexId;
			requestIndexEntryQueryFilter.TargetMailbox = this.targetUser.Id;
			return requestIndexEntryQueryFilter;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider result;
			using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
			{
				if (this.Mailbox == null)
				{
					ADObjectId adObjectId;
					if (!base.TryGetExecutingUserId(out adObjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					this.Mailbox = new MailboxOrMailUserIdParameter(adObjectId);
				}
				result = base.CreateSession();
			}
			return result;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.targetUser = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.Mailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), false);
			TaskLogger.LogExit();
		}

		private ADUser targetUser;
	}
}
