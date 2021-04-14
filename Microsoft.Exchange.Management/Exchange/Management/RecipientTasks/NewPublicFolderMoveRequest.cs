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
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "PublicFolderMoveRequest", SupportsShouldProcess = true)]
	public sealed class NewPublicFolderMoveRequest : NewRequest<PublicFolderMoveRequest>
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true)]
		public PublicFolderIdParameter[] Folders
		{
			get
			{
				return (PublicFolderIdParameter[])base.Fields["SourceFolder"];
			}
			set
			{
				base.Fields["SourceFolder"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true)]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPublicFolderMoveRequest((this.DataObject == null) ? base.RequestName : this.DataObject.ToString());
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (MapiTaskHelper.IsDatacenter)
				{
					base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, this.Organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
					base.RescopeToOrgId(base.CurrentOrganizationId);
					base.OrganizationId = base.CurrentOrganizationId;
				}
				else
				{
					base.OrganizationId = OrganizationId.ForestWideOrgId;
				}
				this.DisallowPublicFolderMoveDuringFinalization();
				this.targetMailboxUser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.TargetMailbox.ToString())), ExchangeErrorCategory.Client);
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.CurrentOrganizationId);
				TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(base.CurrentOrganizationId);
				if (value.GetLocalMailboxRecipient(this.targetMailboxUser.ExchangeGuid) == null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotMovePublicFolderIntoNonPublicFolderMailbox), ErrorCategory.InvalidArgument, this.targetMailboxUser);
				}
				string text = this.GetSourceMailboxGuid().ToString();
				this.sourceMailboxUser = (ADUser)base.GetDataObject<ADUser>(MailboxIdParameter.Parse(text), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(text)), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(text)), ExchangeErrorCategory.Client);
				if (this.sourceMailboxUser.ExchangeGuid == this.targetMailboxUser.ExchangeGuid)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorCannotMovePublicFolderIntoSameMailbox), ErrorCategory.InvalidArgument, this.targetMailboxUser);
				}
				if (!string.IsNullOrEmpty(base.Name))
				{
					base.ValidateName();
					base.RequestName = base.Name;
				}
				else
				{
					base.RequestName = "PublicFolderMove";
				}
				ADObjectId mdbId = null;
				ADObjectId mdbServerSite = null;
				base.Flags = (RequestFlags.IntraOrg | this.LocateAndChooseMdb(value.GetLocalMailboxRecipient(this.sourceMailboxUser.ExchangeGuid).Database, value.GetLocalMailboxRecipient(this.targetMailboxUser.ExchangeGuid).Database, this.sourceMailboxUser, this.targetMailboxUser, this.targetMailboxUser, out mdbId, out mdbServerSite));
				if (base.WorkloadType == RequestWorkloadType.None)
				{
					base.WorkloadType = RequestWorkloadType.Local;
				}
				base.MdbId = mdbId;
				base.MdbServerSite = mdbServerSite;
				base.WriteVerbose(Strings.RequestQueueIdentified(base.MdbId.Name));
				this.CheckRequestNameAvailability(null, null, false, MRSRequestType.PublicFolderMove, this.Organization, false);
				base.WriteVerbose(Strings.RequestNameAvailabilityComplete);
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
			dataObject.RequestType = MRSRequestType.PublicFolderMove;
			dataObject.WorkloadType = base.WorkloadType;
			RequestTaskHelper.ApplyOrganization(dataObject, base.OrganizationId ?? OrganizationId.ForestWideOrgId);
			dataObject.FolderList = this.folderList;
			dataObject.SourceUserId = this.sourceMailboxUser.Id;
			dataObject.TargetUserId = this.targetMailboxUser.Id;
			dataObject.SourceExchangeGuid = this.sourceMailboxUser.ExchangeGuid;
			dataObject.TargetExchangeGuid = this.targetMailboxUser.ExchangeGuid;
			dataObject.ExchangeGuid = this.targetMailboxUser.ExchangeGuid;
			dataObject.SourceDatabase = this.sourceMailboxUser.Database;
			dataObject.TargetDatabase = this.targetMailboxUser.Database;
			dataObject.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
			dataObject.AllowLargeItems = this.AllowLargeItems;
			dataObject.PreserveMailboxSignature = false;
		}

		protected override PublicFolderMoveRequest ConvertToPresentationObject(TransactionalRequestJob dataObject)
		{
			if (dataObject.IndexEntries != null && dataObject.IndexEntries.Count >= 1)
			{
				return new PublicFolderMoveRequest(dataObject.IndexEntries[0]);
			}
			base.WriteError(new RequestIndexEntriesAbsentPermanentException(dataObject.ToString()), ErrorCategory.InvalidArgument, this.Organization);
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException;
		}

		private void DisallowPublicFolderMoveDuringFinalization()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.PublicFolderMailboxesMigration.Enabled && CommonUtils.IsPublicFolderMailboxesLockedForNewConnectionsFlagSet(base.CurrentOrganizationId))
			{
				base.WriteError(new RecipientTaskException(new LocalizedString(ServerStrings.PublicFoldersCannotBeMovedDuringMigration)), ErrorCategory.InvalidOperation, this.targetMailboxUser);
			}
		}

		private Guid GetSourceMailboxGuid()
		{
			Guid guid = Guid.Empty;
			base.WriteVerbose(Strings.DetermineSourceMailbox);
			using (PublicFolderDataProvider publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "New-PublicFolderMoveRequest", Guid.Empty))
			{
				foreach (PublicFolderIdParameter publicFolderIdParameter in this.Folders)
				{
					PublicFolder publicFolder = (PublicFolder)base.GetDataObject<PublicFolder>(publicFolderIdParameter, publicFolderDataProvider, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(publicFolderIdParameter.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(publicFolderIdParameter.ToString())));
					if (publicFolderDataProvider.PublicFolderSession.IsWellKnownFolder(publicFolder.InternalFolderIdentity.ObjectId))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotMoveWellKnownPublicFolders), ErrorCategory.InvalidArgument, publicFolderIdParameter);
					}
					if (guid == Guid.Empty)
					{
						guid = publicFolder.ContentMailboxGuid;
					}
					else if (guid != publicFolder.ContentMailboxGuid)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotMovePublicFoldersFromDifferentSourceMailbox), ErrorCategory.InvalidArgument, publicFolderIdParameter);
					}
					if (publicFolder.EntryId == null || publicFolder.DumpsterEntryId == null)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorCannotMovePublicFoldersWithNullEntryId), ErrorCategory.InvalidData, publicFolderIdParameter);
					}
					this.folderList.Add(new MoveFolderInfo(publicFolder.EntryId, false));
					this.folderList.Add(new MoveFolderInfo(publicFolder.DumpsterEntryId, false));
				}
			}
			return guid;
		}

		private const string DefaultPublicFolderMoveRequestName = "PublicFolderMove";

		internal const string TaskNoun = "PublicFolderMoveRequest";

		internal const string ParameterOrganization = "Organization";

		internal const string ParameterTargetMailbox = "TargetMailbox";

		internal const string ParameterSourceFolder = "SourceFolder";

		internal const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		internal const string ParameterAllowLargeItems = "AllowLargeItems";

		private ADUser sourceMailboxUser;

		private ADUser targetMailboxUser;

		private List<MoveFolderInfo> folderList = new List<MoveFolderInfo>();
	}
}
