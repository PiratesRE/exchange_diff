using System;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Set", "PublicFolder", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetPublicFolder : SetTenantADTaskBase<PublicFolderIdParameter, PublicFolder, PublicFolder>
	{
		private IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					ADSessionSettings sessionSettings;
					if (MapiTaskHelper.IsDatacenter)
					{
						sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
					}
					else
					{
						sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					}
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 75, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\PublicFolder\\SetPublicFolder.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
					this.recipientSession = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSession;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override PublicFolderIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PublicFolderIdParameter Path { get; set; }

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public MailboxIdParameter OverrideContentMailbox { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false)]
		public bool? MailEnabled { get; set; }

		[Parameter(Mandatory = false)]
		public Guid? MailRecipientGuid { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPublicFolderIdentity(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				if (this.Path != null)
				{
					PublicFolder publicFolder = (PublicFolder)base.GetDataObject<PublicFolder>(this.Path, base.DataSession, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.Path.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.Path.ToString())));
					using (this.publicFolderDataProvider.PublicFolderSession.GetRestrictedOperationToken())
					{
						using (CoreFolder coreFolder = CoreFolder.Bind(this.publicFolderDataProvider.PublicFolderSession, publicFolder.InternalFolderIdentity.ObjectId))
						{
							coreFolder.MoveFolder(coreFolder, this.DataObject.InternalFolderIdentity.ObjectId);
							this.folderUpdated = true;
						}
					}
				}
				if (this.OverrideContentMailbox != null && (this.Force || base.ShouldContinue(Strings.ConfirmationMessageOverrideContentMailbox)))
				{
					PublicFolder publicFolder2 = (PublicFolder)base.GetDataObject<PublicFolder>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.Identity.ToString())));
					using (this.publicFolderDataProvider.PublicFolderSession.GetRestrictedOperationToken())
					{
						using (CoreFolder coreFolder2 = CoreFolder.Bind(this.publicFolderDataProvider.PublicFolderSession, publicFolder2.InternalFolderIdentity.ObjectId))
						{
							coreFolder2.PropertyBag.SetProperty(CoreFolderSchema.ReplicaList, new string[]
							{
								this.contentMailboxGuid.ToString()
							});
							coreFolder2.PropertyBag.SetProperty(CoreFolderSchema.LastMovedTimeStamp, ExDateTime.UtcNow);
							coreFolder2.Save(SaveMode.NoConflictResolution);
							this.folderUpdated = true;
						}
					}
				}
				base.InternalProcessRecord();
			}
			catch (NotSupportedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidType, this.Identity);
			}
		}

		protected override bool IsObjectStateChanged()
		{
			return this.folderUpdated || base.IsObjectStateChanged();
		}

		protected override void InternalValidate()
		{
			try
			{
				base.InternalValidate();
			}
			catch (NotSupportedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidType, this.DataObject.Identity);
			}
			if (this.DataObject.FolderPath.IsSubtreeRoot)
			{
				base.WriteError(new ModificationDisallowedException(Strings.ExceptionModifyIpmSubtree), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.IsChanged(PublicFolderSchema.Name))
			{
				PropertyValidationError propertyValidationError = PublicFolderSchema.Name.ValidateValue(this.DataObject.Name, false);
				if (propertyValidationError != null)
				{
					base.WriteError(new DataValidationException(propertyValidationError), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
			}
			ValidationError[] array = Database.ValidateAscendingQuotas(this.DataObject.propertyBag, new ProviderPropertyDefinition[]
			{
				PublicFolderSchema.IssueWarningQuota,
				PublicFolderSchema.ProhibitPostQuota
			}, this.DataObject.Identity);
			if (array.Length > 0)
			{
				base.WriteError(new DataValidationException(array[0]), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			this.ValidateAndUpdateMailPublicFolderParameters();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject(this.Identity);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.publicFolderDataProvider != null)
			{
				this.publicFolderDataProvider.Dispose();
				this.publicFolderDataProvider = null;
			}
		}

		protected override void InternalStateReset()
		{
			this.folderUpdated = false;
			base.InternalStateReset();
		}

		protected override IConfigDataProvider CreateSession()
		{
			OrganizationIdParameter organization = null;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(null, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			if (this.publicFolderDataProvider == null || base.CurrentOrganizationId != this.publicFolderDataProvider.CurrentOrganizationId)
			{
				if (this.OverrideContentMailbox != null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 337, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\PublicFolder\\SetPublicFolder.cs");
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.OverrideContentMailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.OverrideContentMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.OverrideContentMailbox.ToString())));
					if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorInvalidContentMailbox(this.OverrideContentMailbox.ToString())), ErrorCategory.InvalidArgument, aduser);
					}
					if (TenantPublicFolderConfigurationCache.Instance.GetValue(base.CurrentOrganizationId).GetLocalMailboxRecipient(aduser.ExchangeGuid) == null)
					{
						TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.CurrentOrganizationId);
					}
					this.contentMailboxGuid = aduser.ExchangeGuid;
				}
				if (this.publicFolderDataProvider != null)
				{
					this.publicFolderDataProvider.Dispose();
					this.publicFolderDataProvider = null;
				}
				try
				{
					this.publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "Set-PublicFolder", Guid.Empty);
				}
				catch (AccessDeniedException exception)
				{
					base.WriteError(exception, ErrorCategory.PermissionDenied, this.Identity);
				}
			}
			return this.publicFolderDataProvider;
		}

		private void ValidateAndUpdateMailPublicFolderParameters()
		{
			if (this.MailRecipientGuid == null && this.MailEnabled == null)
			{
				return;
			}
			Guid? mailRecipientGuid = this.MailRecipientGuid;
			Guid? guid = (mailRecipientGuid != null) ? new Guid?(mailRecipientGuid.GetValueOrDefault()) : this.DataObject.MailRecipientGuid;
			bool flag = this.MailEnabled ?? this.DataObject.MailEnabled;
			if (this.MailRecipientGuid != null && !flag)
			{
				base.WriteError(new ArgumentException(Strings.ErrorSetPublicFolderMailMailEnabledFalse), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.MailEnabled != null && this.MailEnabled.Value && guid == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorSetPublicFolderMailMailRecipientGuidNull), ErrorCategory.InvalidArgument, this.Identity);
			}
			if (this.MailEnabled != null && !this.MailEnabled.Value)
			{
				this.DataObject.MailEnabled = false;
				this.DataObject.ProxyGuid = null;
				return;
			}
			if (this.MailRecipientGuid != null || (this.MailEnabled != null && this.MailEnabled.Value))
			{
				ADObjectId identity = new ADObjectId(guid.Value);
				ADPublicFolder adpublicFolder = this.RecipientSession.Read<ADPublicFolder>(identity) as ADPublicFolder;
				if (adpublicFolder == null)
				{
					base.WriteError(new ObjectNotFoundException(Strings.ErrorSetPublicFolderMailRecipientGuidNotFoundInAd(guid.Value.ToString())), ErrorCategory.InvalidData, this.Identity);
				}
				StoreObjectId storeObjectId = StoreObjectId.FromHexEntryId(adpublicFolder.EntryId);
				StoreObjectId storeObjectId2 = StoreObjectId.FromHexEntryId(this.DataObject.EntryId);
				if (!ArrayComparer<byte>.Comparer.Equals(storeObjectId.LongTermFolderId, storeObjectId2.LongTermFolderId))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSetPublicFolderMailRecipientGuidLongTermIdNotMatch(adpublicFolder.Id.ToString())), ErrorCategory.InvalidData, this.Identity);
				}
				if (this.DataObject.MailRecipientGuid == null || this.DataObject.MailRecipientGuid.Value != guid.Value)
				{
					this.DataObject.MailRecipientGuid = new Guid?(guid.Value);
				}
				if (!this.DataObject.MailEnabled)
				{
					this.DataObject.MailEnabled = true;
				}
			}
		}

		private PublicFolderDataProvider publicFolderDataProvider;

		private bool folderUpdated;

		private Guid contentMailboxGuid = Guid.Empty;

		private IRecipientSession recipientSession;
	}
}
