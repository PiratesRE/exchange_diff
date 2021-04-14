using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "FolderMoveRequest", SupportsShouldProcess = true)]
	public sealed class NewFolderMoveRequest : NewRequest<FolderMoveRequest>
	{
		[Parameter(Mandatory = true)]
		[ValidateNotNull]
		public MailboxFolderIdParameter[] Folders
		{
			get
			{
				return (MailboxFolderIdParameter[])base.Fields["Folders"];
			}
			set
			{
				base.Fields["Folders"] = value;
			}
		}

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = true)]
		[ValidateNotNull]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter SuspendWhenReadyToComplete
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuspendWhenReadyToComplete"] ?? false);
			}
			set
			{
				base.Fields["SuspendWhenReadyToComplete"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowLargeItems
		{
			get
			{
				return (SwitchParameter)(base.Fields["AllowLargeItems"] ?? false);
			}
			set
			{
				base.Fields["AllowLargeItems"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime CompleteAfter
		{
			get
			{
				return (DateTime)base.Fields["CompleteAfter"];
			}
			set
			{
				base.Fields["CompleteAfter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan IncrementalSyncInterval
		{
			get
			{
				return (TimeSpan)(base.Fields["IncrementalSyncInterval"] ?? NewFolderMoveRequest.defaultIncrementalSyncIntervalForMove);
			}
			set
			{
				base.Fields["IncrementalSyncInterval"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewFolderMoveRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
			}
		}

		private new SkippableMergeComponent[] SkipMerging
		{
			get
			{
				return base.SkipMerging;
			}
			set
			{
				base.SkipMerging = value;
			}
		}

		private new string BatchName
		{
			get
			{
				return base.BatchName;
			}
			set
			{
				base.BatchName = value;
			}
		}

		protected override void InternalStateReset()
		{
			this.sourceMailboxUser = null;
			this.targetMailboxUser = null;
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.IsFieldSet("CompleteAfter") && base.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				DateTime? completeAfter = base.IsFieldSet("CompleteAfter") ? new DateTime?(this.CompleteAfter) : null;
				RequestTaskHelper.ValidateStartAfterCompleteAfterWithSuspendWhenReadyToComplete(null, completeAfter, this.SuspendWhenReadyToComplete.ToBool(), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.IsFieldSet("IncrementalSyncInterval") && base.IsFieldSet("SuspendWhenReadyToComplete") && this.SuspendWhenReadyToComplete.ToBool())
			{
				base.WriteError(new SuspendWhenReadyToCompleteCannotBeSetWithIncrementalSyncIntervalException(), ErrorCategory.InvalidArgument, this.SuspendWhenReadyToComplete);
			}
			if (base.IsFieldSet("IncrementalSyncInterval"))
			{
				RequestTaskHelper.ValidateIncrementalSyncInterval(this.IncrementalSyncInterval, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.sourceMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.SourceMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.SourceMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.SourceMailbox.ToString())), ExchangeErrorCategory.Client);
				this.targetMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), ExchangeErrorCategory.Client);
				if (!object.Equals(this.sourceMailboxUser.OrganizationId, this.targetMailboxUser.OrganizationId))
				{
					base.WriteError(new UsersNotInSameOrganizationPermanentException(this.sourceMailboxUser.ToString(), this.targetMailboxUser.ToString()), ErrorCategory.InvalidArgument, this.TargetMailbox);
				}
				base.RescopeToOrgId(this.sourceMailboxUser.OrganizationId);
				using (MailboxFolderDataProvider mailboxFolderDataProvider = new MailboxFolderDataProvider(base.OrgWideSessionSettings, this.sourceMailboxUser, "New-FolderMoveRequest"))
				{
					foreach (MailboxFolderIdParameter mailboxFolderIdParameter in this.Folders)
					{
						mailboxFolderIdParameter.InternalMailboxFolderId = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(this.sourceMailboxUser.Id, mailboxFolderIdParameter.RawFolderStoreId, mailboxFolderIdParameter.RawFolderPath);
						MailboxFolder mailboxFolder = (MailboxFolder)base.GetDataObject<MailboxFolder>(mailboxFolderIdParameter, mailboxFolderDataProvider, null, new LocalizedString?(Strings.ErrorMailboxFolderNotFound(mailboxFolderIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxFolderNotUnique(mailboxFolderIdParameter.ToString())));
						string entryId = string.Empty;
						if (mailboxFolder.InternalFolderIdentity != null && mailboxFolder.InternalFolderIdentity.ObjectId != null)
						{
							entryId = mailboxFolder.InternalFolderIdentity.ObjectId.ToHexEntryId();
						}
						this.folderList.Add(new MoveFolderInfo(entryId, false));
					}
				}
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					base.RequestName = "FolderMove";
				}
				ADObjectId mdbId = null;
				ADObjectId mdbServerSite = null;
				base.Flags = (RequestFlags.IntraOrg | this.LocateAndChooseMdb(this.sourceMailboxUser.Database, this.targetMailboxUser.Database, this.sourceMailboxUser, this.targetMailboxUser, this.targetMailboxUser, out mdbId, out mdbServerSite));
				if (base.WorkloadType == RequestWorkloadType.None)
				{
					base.WorkloadType = RequestWorkloadType.Local;
				}
				base.MdbId = mdbId;
				base.MdbServerSite = mdbServerSite;
				base.WriteVerbose(Strings.RequestQueueIdentified(base.MdbId.Name));
				this.CheckRequestNameAvailability(base.RequestName, null, false, MRSRequestType.FolderMove, this.TargetMailbox, false);
				base.WriteVerbose(Strings.FolderMoveRequestCheckComplete);
				base.InternalValidate();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void SetRequestProperties(TransactionalRequestJob dataObject)
		{
			base.SetRequestProperties(dataObject);
			dataObject.RequestType = MRSRequestType.FolderMove;
			dataObject.WorkloadType = base.WorkloadType;
			dataObject.FolderList = this.folderList;
			dataObject.SourceUser = this.sourceMailboxUser;
			dataObject.SourceUserId = this.sourceMailboxUser.Id;
			dataObject.TargetUser = this.targetMailboxUser;
			dataObject.TargetUserId = this.targetMailboxUser.Id;
			dataObject.SourceExchangeGuid = this.sourceMailboxUser.ExchangeGuid;
			dataObject.TargetExchangeGuid = this.targetMailboxUser.ExchangeGuid;
			dataObject.ExchangeGuid = this.targetMailboxUser.ExchangeGuid;
			dataObject.SourceDatabase = this.sourceMailboxUser.Database;
			dataObject.TargetDatabase = this.targetMailboxUser.Database;
			dataObject.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
			dataObject.AllowLargeItems = this.AllowLargeItems;
			if (base.IsFieldSet("CompleteAfter"))
			{
				RequestTaskHelper.SetCompleteAfter(new DateTime?(this.CompleteAfter), dataObject, null);
			}
			dataObject.IncrementalSyncInterval = this.IncrementalSyncInterval;
			dataObject.PreserveMailboxSignature = false;
		}

		protected override FolderMoveRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new FolderMoveRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(dataObject.ToString()), ErrorCategory.InvalidArgument, null);
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException;
		}

		protected override string CheckRequestNameAvailability(string name, ADObjectId identifyingObjectId, bool objectIsMailbox, MRSRequestType requestType, object errorObject, bool wildcardedSearch)
		{
			string text = string.Format("{0}*", name);
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = new RequestIndexEntryQueryFilter(wildcardedSearch ? text : name, identifyingObjectId, requestType, new RequestIndexId(RequestIndexLocation.AD), objectIsMailbox);
			requestIndexEntryQueryFilter.WildcardedNameSearch = wildcardedSearch;
			ObjectId rootId = ADHandler.GetRootId(base.CurrentOrgConfigSession, requestType);
			IEnumerable<FolderMoveRequest> enumerable = ((RequestJobProvider)base.DataSession).IndexProvider.FindPaged<FolderMoveRequest>(requestIndexEntryQueryFilter, rootId, rootId == null, null, 10);
			string result;
			using (IEnumerator<FolderMoveRequest> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					bool flag = true;
					while (this.IsNewRequestAllowed(enumerator.Current))
					{
						if (!enumerator.MoveNext())
						{
							IL_93:
							if (!flag)
							{
								base.WriteError(new NameMustBeUniquePermanentException(name, (identifyingObjectId == null) ? string.Empty : identifyingObjectId.ToString()), ErrorCategory.InvalidArgument, errorObject);
								return null;
							}
							return name;
						}
					}
					flag = false;
					goto IL_93;
				}
				result = name;
			}
			return result;
		}

		private bool IsNewRequestAllowed(FolderMoveRequest request)
		{
			return !request.Name.Equals(base.Name, StringComparison.OrdinalIgnoreCase) && (((request.SourceMailbox == null || !request.SourceMailbox.Equals(this.sourceMailboxUser.Id)) && (request.TargetMailbox == null || !request.TargetMailbox.Equals(this.targetMailboxUser.Id))) || request.Status == RequestStatus.Completed || request.Status == RequestStatus.CompletedWithWarning || request.Status == RequestStatus.Failed);
		}

		private const string DefaultFolderMoveRequestName = "FolderMove";

		internal const string TaskNoun = "FolderMoveRequest";

		internal const string ParameterSourceMailbox = "SourceMailbox";

		internal const string ParameterTargetMailbox = "TargetMailbox";

		internal const string ParameterFolders = "Folders";

		internal const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		internal const string ParameterAllowLargeItems = "AllowLargeItems";

		private static TimeSpan defaultIncrementalSyncIntervalForMove = TimeSpan.FromDays(1.0);

		private ADUser sourceMailboxUser;

		private ADUser targetMailboxUser;

		private List<MoveFolderInfo> folderList = new List<MoveFolderInfo>();
	}
}
