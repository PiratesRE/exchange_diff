using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailboxRelocationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "MailboxRelocationSplit")]
	public sealed class NewMailboxRelocationRequest : NewRequest<MailboxRelocationRequest>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public MailboxOrMailUserIdParameter Mailbox
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MailboxRelocationSplit")]
		public DatabaseIdParameter TargetDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["TargetDatabase"];
			}
			set
			{
				base.Fields["TargetDatabase"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "MailboxRelocationJoin")]
		public MailboxOrMailUserIdParameter TargetContainer
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["TargetContainer"];
			}
			set
			{
				base.Fields["TargetContainer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SkippableMoveComponent[] SkipMoving
		{
			get
			{
				return (SkippableMoveComponent[])base.Fields["SkipMoving"];
			}
			set
			{
				base.Fields["SkipMoving"] = value;
			}
		}

		private new Unlimited<int> LargeItemLimit
		{
			get
			{
				return base.LargeItemLimit;
			}
			set
			{
				base.LargeItemLimit = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxRelocation(base.RequestName);
			}
		}

		protected override MailboxRelocationRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries == null || dataObject.IndexEntries.Count < 1)
			{
				base.WriteError(new RequestIndexEntriesAbsentPermanentException(base.RequestName), ErrorCategory.InvalidArgument, this.Mailbox);
			}
			return new MailboxRelocationRequest(dataObject.IndexEntries[0]);
		}

		protected override void CreateIndexEntries(TransactionalRequestJob dataObject)
		{
			RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(dataObject, new RequestIndexId(this.mailbox.Id));
			if (base.ParameterSetName == "MailboxRelocationSplit")
			{
				RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(dataObject, new RequestIndexId(this.sourceContainer.Id));
				return;
			}
			if (base.ParameterSetName == "MailboxRelocationJoin")
			{
				RequestIndexEntryProvider.CreateAndPopulateRequestIndexEntries(dataObject, new RequestIndexId(this.targetContainer.Id));
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (base.WorkloadType == RequestWorkloadType.None)
			{
				base.WorkloadType = RequestWorkloadType.SyncAggregation;
			}
			if (base.ParameterSetName == "MailboxRelocationJoin")
			{
				this.moveFlags |= RequestFlags.Join;
			}
			else if (base.ParameterSetName == "MailboxRelocationSplit")
			{
				this.moveFlags |= RequestFlags.Split;
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalStateReset()
		{
			this.mailbox = null;
			this.targetContainer = null;
			this.targetMailboxDatabase = null;
			this.sourceContainer = null;
			this.sourceDatabaseInformation = null;
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					base.RequestName = base.ParameterSetName;
				}
				this.RetrieveSourceMailboxInformation();
				base.RescopeToOrgId(this.mailbox.OrganizationId);
				string parameterSetName;
				if ((parameterSetName = base.ParameterSetName) != null)
				{
					ADObjectId mdbId;
					ADObjectId mdbServerSite;
					RequestFlags requestFlags;
					if (!(parameterSetName == "MailboxRelocationJoin"))
					{
						if (!(parameterSetName == "MailboxRelocationSplit"))
						{
							goto IL_135;
						}
						if (this.mailbox.UnifiedMailbox == null || !ADRecipient.TryGetFromCrossTenantObjectId(this.mailbox.UnifiedMailbox, out this.sourceContainer).Succeeded)
						{
							base.WriteError(new MailboxRelocationSplitSourceNotInContainerException(this.mailbox.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
						}
						this.RetrieveTargetMailboxInformation();
						requestFlags = this.LocateAndChooseMdb(this.mailbox.Database, this.targetMailboxDatabase.Id, this.Mailbox, this.TargetDatabase, this.TargetDatabase, out mdbId, out mdbServerSite);
					}
					else
					{
						this.RetrieveTargetContainerAndMailboxInformation();
						requestFlags = this.LocateAndChooseMdb(this.mailbox.Database, this.targetContainer.Database, this.Mailbox, this.TargetContainer, this.TargetContainer, out mdbId, out mdbServerSite);
					}
					this.moveFlags |= requestFlags;
					base.MdbId = mdbId;
					base.MdbServerSite = mdbServerSite;
					base.Flags = this.moveFlags;
					RequestTaskHelper.ValidateNotImplicitSplit(this.moveFlags, this.mailbox, new Task.TaskErrorLoggingDelegate(base.WriteError), this.Mailbox);
					this.ValidateNoOtherActiveRequests();
					base.InternalValidate();
					return;
				}
				IL_135:
				throw new NotImplementedException();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void SetRequestProperties(TransactionalRequestJob dataObject)
		{
			base.SetRequestProperties(dataObject);
			dataObject.RequestType = MRSRequestType.MailboxRelocation;
			dataObject.JobType = MRSJobType.RequestJobE15_AutoResume;
			dataObject.UserId = this.mailbox.Id;
			dataObject.ExchangeGuid = this.mailbox.ExchangeGuid;
			RequestTaskHelper.ApplyOrganization(dataObject, this.mailbox.OrganizationId ?? OrganizationId.ForestWideOrgId);
			dataObject.UserOrgName = ((this.mailbox.OrganizationId != null && this.mailbox.OrganizationId.OrganizationalUnit != null) ? this.mailbox.OrganizationId.OrganizationalUnit.Name : this.mailbox.Id.DomainId.ToString());
			dataObject.DistinguishedName = this.mailbox.DistinguishedName;
			dataObject.DisplayName = this.mailbox.DisplayName;
			dataObject.Alias = this.mailbox.Alias;
			dataObject.User = this.mailbox;
			dataObject.DomainControllerToUpdate = this.mailbox.OriginatingServer;
			dataObject.SourceDatabase = ADObjectIdResolutionHelper.ResolveDN(this.mailbox.Database);
			dataObject.SourceVersion = this.sourceDatabaseInformation.Value.ServerVersion;
			this.ChooseTargetMailboxDatabase(dataObject);
			dataObject.PreserveMailboxSignature = false;
			dataObject.IncrementalSyncInterval = NewMailboxRelocationRequest.incrementalSyncInterval;
			dataObject.AllowLargeItems = true;
			RequestTaskHelper.SetSkipMoving(this.SkipMoving, dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), true);
		}

		private void RetrieveSourceMailboxInformation()
		{
			this.mailbox = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.Mailbox, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
			if (this.mailbox.Database == null)
			{
				base.WriteError(new MailboxLacksDatabasePermanentException(this.mailbox.ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
			}
			this.sourceDatabaseInformation = new DatabaseInformation?(MapiUtils.FindServerForMdb(this.mailbox.Database.ObjectGuid, null, null, FindServerFlags.None));
		}

		private void RetrieveTargetContainerAndMailboxInformation()
		{
			this.targetContainer = RequestTaskHelper.ResolveADUser(base.RecipSession, base.GCSession, base.ServerSettings, this.TargetContainer, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), true);
			if (this.targetContainer.UnifiedMailbox != null)
			{
				base.WriteError(new MailboxRelocationJoinTargetNotContainerOwnerException(this.targetContainer.ToString()), ErrorCategory.InvalidArgument, this.TargetContainer);
			}
			if (this.targetContainer.MailboxContainerGuid == null)
			{
				base.WriteError(new MailboxRelocationJoinTargetNotContainerException(this.targetContainer.ToString()), ErrorCategory.InvalidArgument, this.TargetContainer);
			}
			if (this.targetContainer.Database == null)
			{
				base.WriteError(new MailboxLacksDatabasePermanentException(this.targetContainer.ToString()), ErrorCategory.InvalidArgument, this.TargetContainer);
			}
			this.targetDatabaseInformation = new DatabaseInformation?(MapiUtils.FindServerForMdb(this.targetContainer.Database.ObjectGuid, null, null, FindServerFlags.None));
		}

		private void RetrieveTargetMailboxInformation()
		{
			if (base.IsFieldSet("TargetDatabase"))
			{
				this.targetMailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.TargetDatabase, base.ConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.TargetDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.TargetDatabase.ToString())));
				MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.ConfigSession.SessionSettings, this.targetMailboxDatabase, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			else
			{
				this.targetMailboxDatabase = this.ChooseTargetMailboxDatabase(this.mailbox.Database);
			}
			DatabaseInformation value = MapiUtils.FindServerForMdb(this.targetMailboxDatabase.Id.ObjectGuid, null, null, FindServerFlags.None);
			this.targetDatabaseInformation = new DatabaseInformation?(value);
			if (!this.IsSupportedDatabaseVersion(value.ServerVersion, NewRequest<MailboxRelocationRequest>.DatabaseSide.Target))
			{
				base.WriteError(new DatabaseVersionUnsupportedPermanentException(this.targetMailboxDatabase.Identity.ToString(), value.ServerFqdn, new ServerVersion(value.ServerVersion).ToString()), ErrorCategory.InvalidArgument, this.Mailbox);
			}
			if (this.targetMailboxDatabase.Recovery)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTargetDatabaseIsRecovery(this.targetMailboxDatabase.ToString())), ErrorCategory.InvalidArgument, this.Mailbox);
			}
			if (this.mailbox.MailboxProvisioningConstraint != null && this.targetMailboxDatabase.MailboxProvisioningAttributes != null && !this.mailbox.MailboxProvisioningConstraint.IsMatch(this.targetMailboxDatabase.MailboxProvisioningAttributes))
			{
				base.WriteError(new MailboxConstraintsMismatchException(this.mailbox.ToString(), this.targetMailboxDatabase.Name, this.mailbox.MailboxProvisioningConstraint.Value), ErrorCategory.InvalidData, this.Mailbox);
			}
		}

		private void ChooseTargetMailboxDatabase(TransactionalRequestJob dataObject)
		{
			if (this.targetContainer != null)
			{
				dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetContainer.Database);
				dataObject.TargetContainerGuid = this.targetContainer.MailboxContainerGuid;
				dataObject.TargetUnifiedMailboxId = this.targetContainer.GetCrossTenantObjectId();
			}
			else if (this.targetMailboxDatabase != null)
			{
				dataObject.TargetDatabase = ADObjectIdResolutionHelper.ResolveDN(this.targetMailboxDatabase.Id);
				this.targetDatabaseInformation = new DatabaseInformation?(MapiUtils.FindServerForMdb(this.targetMailboxDatabase.Id.ObjectGuid, null, null, FindServerFlags.None));
			}
			dataObject.TargetVersion = this.targetDatabaseInformation.Value.ServerVersion;
		}

		private MailboxDatabase ChooseTargetMailboxDatabase(ADObjectId sourceMailboxDatabase)
		{
			return RequestTaskHelper.ChooseTargetMDB(new ADObjectId[]
			{
				sourceMailboxDatabase
			}, false, this.mailbox, base.DomainController, base.ScopeSet, new Action<LocalizedString>(base.WriteVerbose), new Action<LocalizedException, ExchangeErrorCategory, object>(base.WriteError), new Action<Exception, ErrorCategory, object>(base.WriteError), this.Mailbox);
		}

		private void ValidateNoOtherActiveRequests()
		{
			string otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests(this.mailbox, null);
			if (!string.IsNullOrEmpty(otherRequests))
			{
				base.WriteError(new ObjectInvolvedInMultipleRelocationsPermanentException(MrsStrings.Mailbox, otherRequests), ErrorCategory.InvalidArgument, this.Mailbox);
			}
			ADRecipient adrecipient;
			if (this.mailbox.UnifiedMailbox != null && ADRecipient.TryGetFromCrossTenantObjectId(this.mailbox.UnifiedMailbox, out adrecipient).Succeeded)
			{
				otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests((ADUser)adrecipient, null);
				if (!string.IsNullOrEmpty(otherRequests))
				{
					base.WriteError(new ObjectInvolvedInMultipleRelocationsPermanentException(MrsStrings.SourceContainer, otherRequests), ErrorCategory.InvalidArgument, this.Mailbox);
				}
			}
			if (this.targetContainer != null)
			{
				otherRequests = MailboxRequestIndexEntryHandler.GetOtherRequests(this.targetContainer, null);
				if (!string.IsNullOrEmpty(otherRequests))
				{
					base.WriteError(new ObjectInvolvedInMultipleRelocationsPermanentException(MrsStrings.TargetContainer, otherRequests), ErrorCategory.InvalidArgument, this.Mailbox);
				}
			}
		}

		public const string DefaultMailboxRelocationRequestName = "MailboxRelocation";

		public const string TaskNoun = "MailboxRelocationRequest";

		public const string ParameterSetJoin = "MailboxRelocationJoin";

		public const string ParameterSetSplit = "MailboxRelocationSplit";

		public const string ParameterTargetContainer = "TargetContainer";

		private static readonly TimeSpan incrementalSyncInterval = TimeSpan.FromDays(1.0);

		private ADUser mailbox;

		private ADRecipient sourceContainer;

		private ADUser targetContainer;

		private MailboxDatabase targetMailboxDatabase;

		private DatabaseInformation? sourceDatabaseInformation;

		private DatabaseInformation? targetDatabaseInformation;

		private RequestFlags moveFlags = RequestFlags.IntraOrg;
	}
}
