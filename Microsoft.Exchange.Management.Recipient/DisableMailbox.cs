using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("disable", "Mailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableMailbox : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoteArchive")]
		public SwitchParameter RemoteArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoteArchive"] ?? false);
			}
			set
			{
				base.Fields["RemoteArchive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? false);
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		public SwitchParameter DisableLastArbitrationMailboxAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableLastArbitrationMailboxAllowed"] ?? false);
			}
			set
			{
				base.Fields["DisableLastArbitrationMailboxAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		public SwitchParameter DisableArbitrationMailboxWithOABsAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableArbitrationMailboxWithOABsAllowed"] ?? false);
			}
			set
			{
				base.Fields["DisableArbitrationMailboxWithOABsAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? false);
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreLegalHold
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreLegalHold"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreLegalHold"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeSoftDeletedMailbox"] ?? false);
			}
			set
			{
				base.Fields["IncludeSoftDeletedMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PreserveEmailAddresses
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreserveEmailAddresses"] ?? false);
			}
			set
			{
				base.Fields["PreserveEmailAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PreventRecordingPreviousDatabase
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreventRecordingPreviousDatabase"] ?? false);
			}
			set
			{
				base.Fields["PreventRecordingPreviousDatabase"] = value;
			}
		}

		private IConfigurationSession TenantLocalConfigurationSession
		{
			get
			{
				IConfigurationSession result;
				if ((result = this.tenantLocalConfigurationSession) == null)
				{
					result = (this.tenantLocalConfigurationSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, false, null, null));
				}
				return result;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Archive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageDisableArchive(this.Identity.ToString());
				}
				if ("RemoteArchive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageDisableRemoteArchive(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageDisableMailbox(this.Identity.ToString());
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.IncludeSoftDeletedObjects)
			{
				base.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			MailboxTaskHelper.BlockRemoveOrDisableIfLitigationHoldEnabled((ADUser)adrecipient, new Task.ErrorLoggerDelegate(base.WriteError), true, this.IgnoreLegalHold.ToBool());
			MailboxTaskHelper.BlockRemoveOrDisableIfDiscoveryHoldEnabled((ADUser)adrecipient, new Task.ErrorLoggerDelegate(base.WriteError), true, this.IgnoreLegalHold.ToBool());
			MailboxTaskHelper.BlockRemoveOrDisableIfJournalNDRMailbox((ADUser)adrecipient, this.TenantLocalConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), true);
			MailboxTaskHelper.ValidateNoOABsAssignedToArbitrationMailbox((ADUser)adrecipient, this.DisableArbitrationMailboxWithOABsAllowed.ToBool(), new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDisableArbitrationMailboxWithOABsAssigned(this.Identity.ToString()));
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, this.Arbitration) || MailboxTaskHelper.ExcludePublicFolderMailbox(adrecipient, this.PublicFolder) || MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, this.Identity);
			}
			if (ComplianceConfigImpl.JournalArchivingHardeningEnabled)
			{
				MailboxTaskHelper.BlockRemoveOrDisableMailboxIfJournalArchiveEnabled(base.DataSession as IRecipientSession, this.ConfigurationSession, (ADUser)adrecipient, new Task.ErrorLoggerDelegate(base.WriteError), true);
			}
			return adrecipient;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			this.exchangeGuid = aduser.ExchangeGuid;
			this.mdbId = aduser.Database;
			ProxyAddressCollection emailAddresses = aduser.EmailAddresses;
			if (!aduser.ExchangeVersion.IsOlderThan(ADUserSchema.ArchiveGuid.VersionAdded))
			{
				if (("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName) && aduser.RecipientType == RecipientType.UserMailbox && aduser.MailboxMoveStatus != RequestStatus.None && aduser.MailboxMoveStatus != RequestStatus.Completed && aduser.MailboxMoveStatus != RequestStatus.CompletedWithWarning)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxBeingMoved(this.Identity.ToString(), aduser.MailboxMoveStatus.ToString())), ErrorCategory.InvalidArgument, aduser);
				}
				if (aduser.ArchiveGuid != Guid.Empty)
				{
					if (!this.PreventRecordingPreviousDatabase)
					{
						aduser.DisabledArchiveGuid = aduser.ArchiveGuid;
						aduser.DisabledArchiveDatabase = aduser.ArchiveDatabase;
					}
					else
					{
						aduser.DisabledArchiveGuid = Guid.Empty;
						aduser.DisabledArchiveDatabase = null;
					}
				}
				aduser.ArchiveRelease = MailboxRelease.None;
				aduser.ArchiveGuid = Guid.Empty;
				aduser.ArchiveName = null;
				aduser.ArchiveDatabase = null;
				aduser.ArchiveDomain = null;
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.RecoverMailBox.Enabled && "Archive" == base.ParameterSetName)
				{
					aduser.ArchiveStatus = (aduser.ArchiveStatus &= ~ArchiveStatusFlags.Active);
				}
				if ((aduser.RemoteRecipientType & RemoteRecipientType.ProvisionArchive) == RemoteRecipientType.ProvisionArchive)
				{
					aduser.RemoteRecipientType = ((aduser.RemoteRecipientType &= ~RemoteRecipientType.ProvisionArchive) | RemoteRecipientType.DeprovisionArchive);
				}
			}
			if ("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				TaskLogger.Trace("DisableMailbox -Archive or -RemoteArchive skipping PrepareDataObject", new object[0]);
				TaskLogger.LogExit();
				return aduser;
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.RecoverMailBox.Enabled)
			{
				if (!this.PreventRecordingPreviousDatabase)
				{
					aduser.PreviousDatabase = aduser.Database;
					aduser.PreviousExchangeGuid = aduser.ExchangeGuid;
				}
				else
				{
					aduser.PreviousDatabase = null;
					aduser.PreviousExchangeGuid = Guid.Empty;
				}
			}
			aduser.PreviousRecipientTypeDetails = aduser.RecipientTypeDetails;
			int recipientSoftDeletedStatus = aduser.RecipientSoftDeletedStatus;
			DateTime? whenSoftDeleted = aduser.WhenSoftDeleted;
			Guid disabledArchiveGuid = aduser.DisabledArchiveGuid;
			ADObjectId disabledArchiveDatabase = aduser.DisabledArchiveDatabase;
			MailboxTaskHelper.ClearExchangeProperties(aduser, RecipientConstants.DisableMailbox_PropertiesToReset);
			aduser.SetExchangeVersion(null);
			aduser.MailboxRelease = MailboxRelease.None;
			aduser.OverrideCorruptedValuesWithDefault();
			aduser.propertyBag.SetField(ADRecipientSchema.RecipientSoftDeletedStatus, recipientSoftDeletedStatus);
			aduser.propertyBag.SetField(ADRecipientSchema.WhenSoftDeleted, whenSoftDeleted);
			if (disabledArchiveGuid != Guid.Empty)
			{
				aduser.propertyBag.SetField(ADUserSchema.DisabledArchiveGuid, disabledArchiveGuid);
				aduser.propertyBag.SetField(ADUserSchema.DisabledArchiveDatabase, disabledArchiveDatabase);
			}
			if (this.PreserveEmailAddresses)
			{
				aduser.propertyBag.SetField(ADRecipientSchema.EmailAddresses, emailAddresses);
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (this.PublicFolder)
			{
				MailboxTaskHelper.RemoveOrDisablePublicFolderMailbox(this.DataObject, this.exchangeGuid, this.TenantLocalConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), true, false);
			}
			base.InternalProcessRecord();
			if ("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				TaskLogger.Trace("DisableMailbox -Archive or -RemoteArchive skipping InternalProcessRecord", new object[0]);
				TaskLogger.LogExit();
				return;
			}
			try
			{
				MailboxDatabase mailboxDatabase = null;
				if (this.mdbId != null)
				{
					mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(this.mdbId)
					{
						AllowLegacy = true
					}, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.mdbId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.mdbId.ToString())));
				}
				if (mailboxDatabase != null && this.exchangeGuid != Guid.Empty)
				{
					Server server = mailboxDatabase.GetServer();
					if (server == null)
					{
						this.WriteWarning(Strings.ErrorDBOwningServerNotFound(mailboxDatabase.Identity.ToString()));
					}
					else if (string.IsNullOrEmpty(server.ExchangeLegacyDN))
					{
						this.WriteWarning(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, server.Identity.ToString(), ServerSchema.ExchangeLegacyDN.Name));
					}
					else if (string.IsNullOrEmpty(server.Fqdn))
					{
						this.WriteWarning(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, server.Identity.ToString(), ServerSchema.Fqdn.Name));
					}
					else
					{
						base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(server.Fqdn));
						using (MapiAdministrationSession mapiAdministrationSession = new MapiAdministrationSession(server.ExchangeLegacyDN, Fqdn.Parse(server.Fqdn)))
						{
							base.WriteVerbose(Strings.VerboseSyncMailboxWithDS(this.Identity.ToString(), this.mdbId.ToString(), server.Fqdn));
							mapiAdministrationSession.SyncMailboxWithDS(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(this.mdbId), this.exchangeGuid));
						}
					}
				}
			}
			catch (Microsoft.Exchange.Data.Mapi.Common.MailboxNotFoundException ex)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex
				});
				base.WriteVerbose(ex.LocalizedString);
			}
			catch (DataSourceTransientException ex2)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex2
				});
				this.WriteWarning(ex2.LocalizedString);
			}
			catch (DataSourceOperationException ex3)
			{
				TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
				{
					ex3
				});
				this.WriteWarning(ex3.LocalizedString);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Mailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.Identity != null && this.Arbitration.IsPresent)
			{
				MailboxTaskHelper.ValidateArbitrationMailboxHasNoGroups(this.DataObject, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDisableMailboxWithAssociatedApprovalRecipents(this.Identity.ToString()));
				MailboxTaskHelper.ValidateNotLastArbitrationMailbox(this.DataObject, base.TenantGlobalCatalogSession, base.RootOrgContainerId, this.DisableLastArbitrationMailboxAllowed.IsPresent, new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorCannotDisableLastArbitrationMailboxInOrganization(this.Identity.ToString()));
			}
			if (this.PublicFolder && (this.currentOrganizationId == null || this.currentOrganizationId != this.DataObject.OrganizationId))
			{
				this.currentOrganizationId = this.DataObject.OrganizationId;
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(this.DataObject.OrganizationId);
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException;
		}

		private OrganizationId currentOrganizationId;

		private IConfigurationSession tenantLocalConfigurationSession;

		private Guid exchangeGuid;

		private ADObjectId mdbId;
	}
}
