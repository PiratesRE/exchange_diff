using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Provisioning.LoadBalancing;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("enable", "Mailbox", SupportsShouldProcess = true, DefaultParameterSetName = "User")]
	public sealed class EnableMailbox : EnableRecipientObjectTask<UserIdParameter, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Linked" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxLinked(this.Identity.ToString(), this.LinkedMasterAccount.ToString(), this.LinkedDomainController.ToString());
				}
				if ("Shared" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxShared(this.Identity.ToString(), this.Shared.ToString());
				}
				if ("Room" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxResource(this.Identity.ToString(), ExchangeResourceType.Room.ToString());
				}
				if ("Equipment" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxResource(this.Identity.ToString(), ExchangeResourceType.Equipment.ToString());
				}
				if ("Arbitration" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxArbitration(this.Identity.ToString());
				}
				if ("PublicFolder" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxPublicFolder(this.Identity.ToString());
				}
				if ("Discovery" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxDiscovery(this.Identity.ToString());
				}
				if ("Archive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxWithArchive(this.Identity.ToString());
				}
				if ("RemoteArchive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxWithRemoteArchive(this.Identity.ToString(), this.ArchiveDomain.ToString());
				}
				if ("LinkedRoomMailbox" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxLinkedAndResource(this.Identity.ToString(), this.LinkedMasterAccount.ToString(), this.LinkedDomainController.ToString(), ExchangeResourceType.Room.ToString());
				}
				if ("AuditLog" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailboxAuditLog(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageEnableMailboxUser(this.Identity.ToString());
			}
		}

		private ActiveManager ActiveManager
		{
			get
			{
				return RecipientTaskHelper.GetActiveManagerInstance();
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override UserIdParameter Identity
		{
			get
			{
				return (UserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "AuditLog")]
		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "Discovery")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AuditLog")]
		[Parameter(Mandatory = false, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = false, ParameterSetName = "Discovery")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public SwitchParameter TargetAllMDBs
		{
			get
			{
				return (SwitchParameter)(base.Fields["TargetAllMDBs"] ?? true);
			}
			set
			{
				base.Fields["TargetAllMDBs"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
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

		[Parameter(Mandatory = true, ParameterSetName = "PublicFolder")]
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

		[Parameter(Mandatory = false, ParameterSetName = "PublicFolder")]
		public SwitchParameter HoldForMigration
		{
			get
			{
				return (SwitchParameter)(base.Fields["HoldForMigration"] ?? false);
			}
			set
			{
				base.Fields["HoldForMigration"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Discovery")]
		public SwitchParameter Discovery
		{
			get
			{
				return (SwitchParameter)(base.Fields["Discovery"] ?? false);
			}
			set
			{
				base.Fields["Discovery"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Room")]
		public SwitchParameter Room
		{
			get
			{
				return (SwitchParameter)base.Fields["Room"];
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		public SwitchParameter LinkedRoom
		{
			get
			{
				return this.Room;
			}
			set
			{
				this.Room = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Equipment")]
		public SwitchParameter Equipment
		{
			get
			{
				return (SwitchParameter)base.Fields["Equipment"];
			}
			set
			{
				base.Fields["Equipment"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Shared")]
		public SwitchParameter Shared
		{
			get
			{
				return (SwitchParameter)base.Fields["Shared"];
			}
			set
			{
				base.Fields["Shared"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields["LinkedMasterAccount"];
			}
			set
			{
				base.Fields["LinkedMasterAccount"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LinkedRoomMailbox")]
		[Parameter(Mandatory = true, ParameterSetName = "Linked")]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields["LinkedDomainController"];
			}
			set
			{
				base.Fields["LinkedDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Linked")]
		[Parameter(Mandatory = false, ParameterSetName = "LinkedRoomMailbox")]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields["LinkedCredential"];
			}
			set
			{
				base.Fields["LinkedCredential"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter RetentionPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[MailboxSchema.RetentionPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RetentionPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? false);
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter ActiveSyncMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["ActiveSyncMailboxPolicy"];
			}
			set
			{
				base.Fields["ActiveSyncMailboxPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter RoleAssignmentPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[MailboxSchema.RoleAssignmentPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RoleAssignmentPolicy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public AddressBookMailboxPolicyIdParameter AddressBookPolicy
		{
			get
			{
				return (AddressBookMailboxPolicyIdParameter)base.Fields[ADRecipientSchema.AddressBookPolicy];
			}
			set
			{
				base.Fields[ADRecipientSchema.AddressBookPolicy] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return (MailboxPlanIdParameter)base.Fields[ADRecipientSchema.MailboxPlan];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxPlan] = value;
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

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		[ValidateNotEmptyGuid]
		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)(base.Fields[ADUserSchema.ArchiveGuid] ?? Guid.Empty);
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveGuid] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public DatabaseIdParameter ArchiveDatabase
		{
			get
			{
				return (DatabaseIdParameter)base.Fields[ADUserSchema.ArchiveDatabase];
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveDatabase] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return base.Fields[ADUserSchema.ArchiveName] as MultiValuedProperty<string>;
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveName] = value;
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

		[Parameter(Mandatory = true, ParameterSetName = "RemoteArchive")]
		public SmtpDomain ArchiveDomain
		{
			get
			{
				return base.Fields[ADUserSchema.ArchiveDomain] as SmtpDomain;
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveDomain] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public override Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				base.SKUCapability = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		public override MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				base.AddOnSKUCapability = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public override bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				base.SKUAssigned = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public SwitchParameter BypassModerationCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassModerationCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassModerationCheck"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public bool? AccountDisabled
		{
			get
			{
				return (bool?)base.Fields["AccountDisabled"];
			}
			set
			{
				base.Fields["AccountDisabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "User")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public override CountryInfo UsageLocation
		{
			get
			{
				return base.UsageLocation;
			}
			set
			{
				base.UsageLocation = value;
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

		[Parameter(Mandatory = false, ParameterSetName = "AuditLog")]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		protected override bool DelayProvisioning
		{
			get
			{
				return true;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			base.CheckExclusiveParameters(new object[]
			{
				ADRecipientSchema.MailboxPlan,
				"SKUCapability"
			});
			if (!("Linked" == base.ParameterSetName))
			{
				if (!("LinkedRoomMailbox" == base.ParameterSetName))
				{
					goto IL_AD;
				}
			}
			try
			{
				NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
				this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			}
			catch (PSArgumentException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
			}
			IL_AD:
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				base.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (this.MailboxPlan != null)
			{
				ADUser dataObject = (ADUser)base.GetDataObject<ADUser>(this.MailboxPlan, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.MailboxPlan.ToString())));
				MailboxTaskHelper.ValidateMailboxPlanRelease(dataObject, new Task.ErrorLoggerDelegate(base.WriteError));
				this.mailboxPlan = new MailboxPlan(dataObject);
			}
			if (this.RetentionPolicy != null)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("RetentionPolicy")), ExchangeErrorCategory.Client, null);
				}
				RetentionPolicy retentionPolicy = (RetentionPolicy)base.GetDataObject<RetentionPolicy>(this.RetentionPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRetentionPolicyNotFound(this.RetentionPolicy.ToString())), new LocalizedString?(Strings.ErrorRetentionPolicyNotUnique(this.RetentionPolicy.ToString())));
				this.retentionPolicyId = retentionPolicy.Id;
			}
			if (this.ActiveSyncMailboxPolicy != null)
			{
				MobileMailboxPolicy mobileMailboxPolicy = (MobileMailboxPolicy)base.GetDataObject<MobileMailboxPolicy>(this.ActiveSyncMailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotFound(this.ActiveSyncMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotUnique(this.ActiveSyncMailboxPolicy.ToString())));
				this.mobileMailboxPolicyId = (ADObjectId)mobileMailboxPolicy.Identity;
			}
			if (this.AddressBookPolicy != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = (AddressBookMailboxPolicy)base.GetDataObject<AddressBookMailboxPolicy>(this.AddressBookPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotFound(this.AddressBookPolicy.ToString())), new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotUnique(this.AddressBookPolicy.ToString())), ExchangeErrorCategory.Client);
				this.addressBookPolicyId = (ADObjectId)addressBookMailboxPolicy.Identity;
			}
		}

		internal override bool SkipPrepareDataObject()
		{
			return "Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName;
		}

		protected override void PrepareRecipientObject(ref ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(ref user);
			if (this.BypassModerationCheck.IsPresent)
			{
				user.BypassModerationCheck = true;
			}
			if (this.AccountDisabled == true)
			{
				user.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
			}
			else if (this.AccountDisabled == false)
			{
				user.UserAccountControl &= ~UserAccountControlFlags.AccountDisabled;
			}
			if (RecipientType.User != user.RecipientType)
			{
				if (("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName) && user.RecipientType == RecipientType.UserMailbox)
				{
					if (user.MailboxMoveStatus != RequestStatus.None && user.MailboxMoveStatus != RequestStatus.Completed && user.MailboxMoveStatus != RequestStatus.CompletedWithWarning)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorMailboxBeingMoved(user.Name, user.MailboxMoveStatus.ToString())), ErrorCategory.InvalidArgument, user);
					}
					if (user.ManagedFolderMailboxPolicy != null)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorNoArchiveWithManagedFolder(user.Name)), ErrorCategory.InvalidData, null);
					}
					if (user.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorNoArchiveForPublicMailbox(user.Name)), ErrorCategory.InvalidArgument, null);
					}
					if ("RemoteArchive" == base.ParameterSetName)
					{
						Database database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(user.Database), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(user.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(user.Database.ToString())));
						if (this.GetDatabaseLocationInfo(database).ServerVersion < Server.E15MinVersion)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(database.ToString())), ExchangeErrorCategory.Client, null);
						}
					}
					if (this.archiveDatabase != null)
					{
						MailboxTaskHelper.BlockLowerMajorVersionArchive(this.archiveDatabaseLocationInfo.ServerVersion, user.Database.DistinguishedName, this.archiveDatabase.DistinguishedName, this.archiveDatabase.ToString(), user.Database, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<MailboxDatabase>), base.GlobalConfigSession, this.ActiveManager, new Task.ErrorLoggerDelegate(base.WriteError));
					}
					this.CreateArchiveIfNecessary(user);
					TaskLogger.LogExit();
					return;
				}
				if (user.RecipientType == RecipientType.MailUser)
				{
					this.originalRecipientType = user.RecipientType;
					base.Alias = user.Alias;
				}
				else
				{
					RecipientIdParameter recipientIdParameter = new RecipientIdParameter((ADObjectId)user.Identity);
					base.WriteError(new RecipientTaskException(Strings.ErrorInvalidRecipientType(recipientIdParameter.ToString(), user.RecipientType.ToString())), ErrorCategory.InvalidArgument, recipientIdParameter);
				}
			}
			if ("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMailboxNotEnabled(this.Identity.ToString())), ErrorCategory.InvalidArgument, user);
			}
			if (!("User" == base.ParameterSetName) && !("WindowsLiveID" == base.ParameterSetName) && (user.UserAccountControl & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.None)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorAccountEnabledForNonUserMailbox), ErrorCategory.InvalidArgument, user);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && this.originalRecipientType == RecipientType.MailUser)
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled && user.WindowsLiveID.Equals(SmtpAddress.Empty))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorWindowsLiveIdRequired(user.Name)), ErrorCategory.InvalidData, null);
				}
				if (!RecipientTaskHelper.SMTPAddressCheckWithAcceptedDomain(this.ConfigurationSession, user.OrganizationId, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache))
				{
					this.StripInvalidSMTPAddresses(user);
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled && !RecipientTaskHelper.IsAcceptedDomain(this.ConfigurationSession, user.OrganizationId, user.WindowsEmailAddress.Domain, base.ProvisioningCache))
					{
						user.WindowsEmailAddress = user.WindowsLiveID;
					}
				}
			}
			if ("PublicFolder" == base.ParameterSetName)
			{
				user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.PublicFolderMailbox, false));
			}
			else if ("Arbitration" == base.ParameterSetName)
			{
				user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.ArbitrationMailbox, false));
			}
			else if ("AuditLog" == base.ParameterSetName)
			{
				user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.AuditLogMailbox, false));
			}
			else
			{
				user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.UserMailbox, false));
			}
			if (user.IsChanged(ADObjectSchema.ExchangeVersion))
			{
				base.WriteVerbose(Strings.VerboseUpdatingVersion(user.Identity.ToString(), user.ExchangeVersion.ToString(), ExchangeObjectVersion.Exchange2010.ToString()));
				base.DataSession.Save(user);
				base.WriteVerbose(Strings.VerboseADOperationSucceeded(user.Identity.ToString()));
				user = (ADUser)base.DataSession.Read<ADUser>(user.Id);
			}
			List<PropertyDefinition> list;
			if (this.originalRecipientType != RecipientType.MailUser)
			{
				list = new List<PropertyDefinition>(RecipientConstants.DisableMailbox_PropertiesToReset);
				MailboxTaskHelper.RemovePersistentProperties(list);
			}
			else
			{
				list = new List<PropertyDefinition>(EnableMailbox.PropertiesToResetForMailUser);
			}
			MailboxTaskHelper.ClearExchangeProperties(user, list);
			if (this.DelayProvisioning && base.IsProvisioningLayerAvailable)
			{
				this.ProvisionDefaultValues(new ADUser
				{
					PersistedCapabilities = user.PersistedCapabilities,
					MailboxProvisioningConstraint = user.MailboxProvisioningConstraint
				}, user);
			}
			if ("RemoteArchive" != base.ParameterSetName && MailboxTaskHelper.SupportsMailboxReleaseVersioning(user))
			{
				if ("Archive" == base.ParameterSetName && this.archiveDatabaseLocationInfo != null)
				{
					user.ArchiveRelease = this.archiveDatabaseLocationInfo.MailboxRelease;
				}
				else
				{
					user.MailboxRelease = this.databaseLocationInfo.MailboxRelease;
				}
			}
			if (AppSettings.Current.DedicatedMailboxPlansEnabled || VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.LegacyRegCodeSupport.Enabled)
			{
				string dedicatedMailboxPlansCustomAttributeName = AppSettings.Current.DedicatedMailboxPlansCustomAttributeName;
				string customAttribute = ADRecipient.GetCustomAttribute(user, dedicatedMailboxPlansCustomAttributeName);
				if (!string.IsNullOrEmpty(customAttribute))
				{
					string text = null;
					MailboxProvisioningConstraint mailboxProvisioningConstraint = null;
					if (!ADRecipient.TryParseMailboxProvisioningData(customAttribute, out text, out mailboxProvisioningConstraint) && user.MailboxProvisioningConstraint == null && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.LegacyRegCodeSupport.Enabled)
					{
						base.WriteError(new RecipientTaskException(Strings.Error_InvalidLegacyRegionCode(customAttribute)), ExchangeErrorCategory.Client, null);
					}
					if (AppSettings.Current.DedicatedMailboxPlansEnabled)
					{
						if (text != null)
						{
							ADUser aduser = this.FindMailboxPlanWithName(text, base.TenantGlobalCatalogSession);
							if (aduser != null)
							{
								user.MailboxPlan = aduser.Id;
								user.MailboxPlanObject = aduser;
							}
							else
							{
								this.WriteWarning(Strings.WarningDedicatedMailboxPlanNotFound(text));
							}
						}
						else
						{
							this.WriteWarning(Strings.WarningInvalidDedicatedMailboxPlanData(customAttribute));
						}
					}
				}
				else if (AppSettings.Current.DedicatedMailboxPlansEnabled)
				{
					this.WriteWarning(Strings.WarningDedicatedMailboxPlanDataNotFound(dedicatedMailboxPlansCustomAttributeName));
				}
			}
			if (user.MailboxPlan != null)
			{
				ADUser dataObject;
				if (user.MailboxPlanObject != null)
				{
					dataObject = user.MailboxPlanObject;
				}
				else
				{
					MailboxPlanIdParameter mailboxPlanIdParameter = new MailboxPlanIdParameter(user.MailboxPlan);
					dataObject = (ADUser)base.GetDataObject<ADUser>(mailboxPlanIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(mailboxPlanIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(mailboxPlanIdParameter.ToString())));
				}
				this.mailboxPlan = new MailboxPlan(dataObject);
			}
			user.MailboxPlanObject = null;
			user.propertyBag.ResetChangeTracking(ADRecipientSchema.MailboxPlanObject);
			if (this.mailboxPlan != null)
			{
				ADUser aduser2 = new ADUser();
				aduser2.StampPersistableDefaultValues();
				aduser2.StampDefaultValues(RecipientType.UserMailbox);
				aduser2.ResetChangeTracking();
				aduser2.EnableSaveCalculatedValues();
				User.FromDataObject(aduser2).ApplyCloneableProperties(User.FromDataObject((ADUser)this.mailboxPlan.DataObject));
				aduser2.PersistedCapabilities = user.PersistedCapabilities;
				Mailbox.FromDataObject(aduser2).ApplyCloneableProperties(Mailbox.FromDataObject((ADUser)this.mailboxPlan.DataObject));
				CASMailbox.FromDataObject(aduser2).ApplyCloneableProperties(CASMailbox.FromDataObject((ADUser)this.mailboxPlan.DataObject));
				UMMailbox.FromDataObject(aduser2).ApplyCloneableProperties(UMMailbox.FromDataObject((ADUser)this.mailboxPlan.DataObject));
				RecipientTaskHelper.UpgradeArchiveQuotaOnArchiveAddOnSKU(aduser2, aduser2.PersistedCapabilities);
				user.CopyChangesFrom(aduser2);
				user.MailboxPlan = this.mailboxPlan.Id;
			}
			else
			{
				user.StampMailboxDefaultValues(false);
			}
			IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(user.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
			if (this.PublicFolder)
			{
				MailboxTaskHelper.ValidatePublicFolderInformationWritable(tenantLocalConfigSession, this.HoldForMigration, new Task.ErrorLoggerDelegate(base.WriteError), this.Force);
			}
			if (this.RoleAssignmentPolicy == null)
			{
				if (!this.Arbitration && !this.PublicFolder)
				{
					RoleAssignmentPolicy roleAssignmentPolicy = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(tenantLocalConfigSession, new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
					if (roleAssignmentPolicy != null)
					{
						this.defaultRoleAssignmentPolicyId = (ADObjectId)roleAssignmentPolicy.Identity;
					}
				}
			}
			else
			{
				RoleAssignmentPolicy roleAssignmentPolicy2 = (RoleAssignmentPolicy)base.GetDataObject<RoleAssignmentPolicy>(this.RoleAssignmentPolicy, tenantLocalConfigSession, null, new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotFound(this.RoleAssignmentPolicy.ToString())), new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotUnique(this.RoleAssignmentPolicy.ToString())));
				this.userSpecifiedRoleAssignmentPolicyId = (ADObjectId)roleAssignmentPolicy2.Identity;
			}
			if (!RecipientTaskHelper.IsE15OrLater(this.databaseLocationInfo.ServerVersion))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(this.database.ToString())), ErrorCategory.InvalidData, null);
			}
			if (user.UseDatabaseQuotaDefaults == null)
			{
				user.UseDatabaseQuotaDefaults = new bool?(VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.UseDatabaseQuotaDefaults.Enabled);
			}
			bool flag = false;
			if ("LinkedRoomMailbox" == base.ParameterSetName)
			{
				user.MasterAccountSid = this.linkedUserSid;
				user.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(this.linkedUserSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Room);
			}
			else if ("Linked" == base.ParameterSetName)
			{
				user.MasterAccountSid = this.linkedUserSid;
				user.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(this.linkedUserSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			else if ("Shared" == base.ParameterSetName)
			{
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			else if ("Room" == base.ParameterSetName)
			{
				user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Room);
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			else if ("Equipment" == base.ParameterSetName)
			{
				user.ResourceType = new ExchangeResourceType?(ExchangeResourceType.Equipment);
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			}
			else if ("AuditLog" == base.ParameterSetName)
			{
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				user.HiddenFromAddressListsEnabled = true;
				user.CalendarVersionStoreDisabled = true;
				user.UseDatabaseQuotaDefaults = new bool?(false);
				user.RecoverableItemsQuota = ByteQuantifiedSize.FromGB(50UL);
			}
			else if ("Discovery" == base.ParameterSetName)
			{
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				user.ProhibitSendReceiveQuota = ByteQuantifiedSize.FromGB(50UL);
				user.ProhibitSendQuota = ByteQuantifiedSize.FromGB(50UL);
				user.CalendarVersionStoreDisabled = true;
				user.UseDatabaseQuotaDefaults = new bool?(false);
				user.HiddenFromAddressListsEnabled = true;
				user.MaxReceiveSize = ByteQuantifiedSize.FromMB(100UL);
				user.MaxSendSize = ByteQuantifiedSize.FromMB(100UL);
			}
			else if ("Arbitration" == base.ParameterSetName)
			{
				if (!user.IsModified(ADRecipientSchema.DisplayName))
				{
					user.DisplayName = Strings.ArbitrationMailboxDefaultDisplayName;
				}
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				user.HiddenFromAddressListsEnabled = true;
				user.RequireAllSendersAreAuthenticated = true;
				user.ProhibitSendQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				user.ProhibitSendReceiveQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				user.IssueWarningQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				user.UseDatabaseQuotaDefaults = new bool?(false);
				ADObjectId childId;
				if (user.OrganizationId.ConfigurationUnit == null)
				{
					childId = this.ConfigurationSession.GetOrgContainerId().GetChildId(ApprovalApplicationContainer.DefaultName);
				}
				else
				{
					childId = user.OrganizationId.ConfigurationUnit.GetChildId(ApprovalApplicationContainer.DefaultName);
				}
				if (this.ConfigurationSession.Read<ApprovalApplicationContainer>(childId) == null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorRootContainerNotExist(childId.ToString())), ErrorCategory.ObjectNotFound, null);
				}
				if (!NewMailboxBase.IsNonApprovalArbitrationMailboxName(user.Name))
				{
					if (user.ManagedFolderMailboxPolicy == null && user.RetentionPolicy == null)
					{
						ADObjectId childId2;
						if (user.OrganizationId.ConfigurationUnit == null)
						{
							childId2 = this.ConfigurationSession.GetOrgContainerId().GetChildId("Retention Policies Container").GetChildId("ArbitrationMailbox");
						}
						else
						{
							childId2 = user.OrganizationId.ConfigurationUnit.GetChildId("Retention Policies Container").GetChildId("ArbitrationMailbox");
						}
						RetentionPolicy retentionPolicy = this.ConfigurationSession.Read<RetentionPolicy>(childId2);
						if (retentionPolicy != null)
						{
							user.RetentionPolicy = retentionPolicy.Id;
							flag = true;
						}
						else
						{
							this.WriteWarning(Strings.WarningArbitrationRetentionPolicyNotAvailable(childId2.ToString()));
						}
					}
					ApprovalApplication[] array = this.ConfigurationSession.Find<ApprovalApplication>(childId, QueryScope.SubTree, null, null, 0);
					List<ADObjectId> list2 = new List<ADObjectId>(array.Length);
					foreach (ApprovalApplication approvalApplication in array)
					{
						list2.Add((ADObjectId)approvalApplication.Identity);
					}
					user[ADUserSchema.ApprovalApplications] = new MultiValuedProperty<ADObjectId>(list2);
				}
			}
			else if ("PublicFolder" == base.ParameterSetName)
			{
				user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				user.HiddenFromAddressListsEnabled = true;
				if (!RecipientTaskHelper.IsE15OrLater(this.databaseLocationInfo.ServerVersion))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(this.database.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			MailboxTaskHelper.StampMailboxRecipientTypes(user, base.ParameterSetName);
			user.Database = this.database.Id;
			user.ServerLegacyDN = this.databaseLocationInfo.ServerLegacyDN;
			if (!flag)
			{
				user.RetentionPolicy = this.retentionPolicyId;
			}
			user.ActiveSyncMailboxPolicy = this.mobileMailboxPolicyId;
			user.AddressBookPolicy = this.addressBookPolicyId;
			if (this.userSpecifiedRoleAssignmentPolicyId != null)
			{
				user.RoleAssignmentPolicy = this.userSpecifiedRoleAssignmentPolicyId;
			}
			else if (user.RoleAssignmentPolicy == null && this.defaultRoleAssignmentPolicyId != null)
			{
				user.RoleAssignmentPolicy = this.defaultRoleAssignmentPolicyId;
			}
			if (this.originalRecipientType != RecipientType.MailUser && user.WindowsLiveID != SmtpAddress.Empty)
			{
				user.EmailAddressPolicyEnabled = false;
				SmtpProxyAddress item = new SmtpProxyAddress(user.WindowsLiveID.ToString(), false);
				if (!user.EmailAddresses.Contains(item))
				{
					user.EmailAddresses.Add(item);
				}
			}
			if (!string.IsNullOrEmpty(base.Alias))
			{
				user.Alias = base.Alias;
			}
			MailboxTaskHelper.WriteWarningWhenMailboxIsUnlicensed(user, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			user.ShouldUseDefaultRetentionPolicy = true;
			user.ElcMailboxFlags |= ElcMailboxFlags.ElcV2;
			TaskLogger.LogExit();
		}

		private void ReloadArchiveMailbox()
		{
			if (this.recoverArchive && this.DataObject.ArchiveDatabase != null)
			{
				MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(new DatabaseIdParameter(this.DataObject.ArchiveDatabase), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.DataObject.ArchiveDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.DataObject.ArchiveDatabase.ToString())));
				using (MapiAdministrationSession adminSession = MapiTaskHelper.GetAdminSession(this.ActiveManager, this.DataObject.ArchiveDatabase.ObjectGuid))
				{
					ConnectMailbox.UpdateSDAndRefreshMailbox(adminSession, this.DataObject, mailboxDatabase, this.DataObject.ArchiveGuid, null, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if ("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				base.InternalProcessRecord();
				this.ReloadArchiveMailbox();
				this.WriteResult();
				TaskLogger.LogExit();
				return;
			}
			if (this.DataObject.UMEnabled)
			{
				Utils.DoUMEnablingSynchronousWork(this.DataObject);
			}
			bool flag = false;
			if ("Linked" == base.ParameterSetName || "Shared" == base.ParameterSetName || "Room" == base.ParameterSetName || "LinkedRoomMailbox" == base.ParameterSetName || "Equipment" == base.ParameterSetName)
			{
				MailboxTaskHelper.GrantPermissionToLinkedUserAccount(this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				flag = true;
			}
			else if ("Arbitration" == base.ParameterSetName)
			{
				MailboxTaskHelper.GrantPermissionToLinkedUserAccount(this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				flag = true;
			}
			else if ("Discovery" == base.ParameterSetName)
			{
				this.DataObject.AcceptMessagesOnlyFrom.Add(this.DataObject.Id);
			}
			else if ("PublicFolder" == base.ParameterSetName && this.PublicFolder)
			{
				IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, false, null, null);
				Organization orgContainer = tenantLocalConfigSession.GetOrgContainer();
				if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty)
				{
					orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
					orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(this.DataObject.ExchangeGuid, this.HoldForMigration ? PublicFolderInformation.HierarchyType.InTransitMailboxGuid : PublicFolderInformation.HierarchyType.MailboxGuid);
					tenantLocalConfigSession.Save(orgContainer);
					MailboxTaskHelper.PrepopulateCacheForMailbox(this.database, this.databaseLocationInfo.ServerFqdn, this.DataObject.OrganizationId, this.DataObject.LegacyExchangeDN, this.DataObject.ExchangeGuid, tenantLocalConfigSession.LastUsedDc, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
			}
			base.InternalProcessRecord();
			if (flag)
			{
				base.WriteVerbose(Strings.VerboseSaveADSecurityDescriptor(this.DataObject.Id.ToString()));
				try
				{
					this.DataObject.SaveSecurityDescriptor(((SecurityDescriptor)this.DataObject[ADObjectSchema.NTSecurityDescriptor]).ToRawSecurityDescriptor());
				}
				catch (ADOperationException ex)
				{
					TaskLogger.Trace("An exception is caught and ignored when enabling the mailbox '{0}'. Exception: {1}", new object[]
					{
						this.DataObject.Id.ToString(),
						ex.Message
					});
					this.WriteWarning(Strings.WarningNTSecurityDescriptorNotUpdated(this.DataObject.Id.ToString(), ex.Message));
				}
			}
			if (this.recoveredMailbox)
			{
				using (MapiAdministrationSession adminSession = MapiTaskHelper.GetAdminSession(this.ActiveManager, this.DataObject.Database.ObjectGuid))
				{
					ConnectMailbox.UpdateSDAndRefreshMailbox(adminSession, this.DataObject, (MailboxDatabase)this.database, this.DataObject.ExchangeGuid, base.ParameterSetName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
				this.ReloadArchiveMailbox();
			}
			this.WriteResult();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if ("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				if (this.ArchiveDatabase != null)
				{
					this.ValidateAndSetArchiveDatabase(this.ArchiveDatabase, true);
				}
				TaskLogger.Trace("EnableMailbox -Archive or -RemoteArchive, skip Database set and validation, Database will not be resolved.", new object[0]);
			}
			else if (this.Database != null)
			{
				this.ValidateAndSetDatabase(this.Database, true);
			}
			else if (!base.IsProvisioningLayerAvailable)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorAutomaticProvisioningFailedToFindDatabase("Database")), ErrorCategory.InvalidData, null);
			}
			base.InternalValidate();
			if (this.AuditLog)
			{
				if (this.DataObject.RecipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
				{
					if (this.DataObject.RecipientTypeDetails != RecipientTypeDetails.UserMailbox)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorNoAuditLogForNonUserMailbox(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
					}
					else if (this.DataObject.ArchiveGuid != Guid.Empty)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorNoAuditLogForArchive(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
					}
				}
			}
			else if (this.DataObject.RecipientTypeDetails == RecipientTypeDetails.AuditLogMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.RecipientNotFoundException(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
			}
			if (this.DataObject.IsModified(ADUserSchema.ArchiveGuid) && this.ArchiveGuid != Guid.Empty)
			{
				RecipientTaskHelper.IsExchangeGuidOrArchiveGuidUnique(this.DataObject, ADUserSchema.ArchiveGuid, this.ArchiveGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (("Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName) && this.DataObject.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNoArchiveForTeamMailbox(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
			}
			if (this.DataObject.MailboxProvisioningConstraint != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(new MailboxProvisioningConstraint[]
				{
					this.DataObject.MailboxProvisioningConstraint
				}, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.DataObject.MailboxProvisioningPreferences != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(this.DataObject.MailboxProvisioningPreferences, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			MailboxTaskHelper.EnsureUserSpecifiedDatabaseMatchesMailboxProvisioningConstraint(this.database, this.archiveDatabase, base.Fields, this.DataObject.MailboxProvisioningConstraint, new Task.ErrorLoggerDelegate(base.WriteError), "Database");
		}

		private void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			Mailbox sendToPipeline = new Mailbox(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Mailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void ValidateProvisionedProperties(IConfigurable dataObject)
		{
			if (this.Database != null || "Archive" == base.ParameterSetName || "RemoteArchive" == base.ParameterSetName)
			{
				return;
			}
			this.recoveredMailbox = false;
			ADUser aduser = dataObject as ADUser;
			if (aduser == null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorParameterRequiredButNotProvisioned("Database")), ErrorCategory.InvalidData, null);
			}
			if (aduser.PreviousDatabase != null && Guid.Empty != aduser.PreviousExchangeGuid && MailboxTaskHelper.FindConnectedMailbox(RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, aduser.Id), aduser.PreviousExchangeGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)) == null)
			{
				ADObjectId deletedObjectsContainer = this.ConfigurationSession.DeletedObjectsContainer;
				ADObjectId adobjectId = ADObjectIdResolutionHelper.ResolveDN(aduser.PreviousDatabase);
				if (adobjectId.Parent == null || adobjectId.Parent.Equals(deletedObjectsContainer))
				{
					aduser.PreviousDatabase = null;
					aduser.PreviousExchangeGuid = Guid.Empty;
				}
				else
				{
					using (MapiAdministrationSession adminSession = MapiTaskHelper.GetAdminSession(this.ActiveManager, adobjectId.ObjectGuid))
					{
						string mailboxLegacyDN = MapiTaskHelper.GetMailboxLegacyDN(adminSession, adobjectId, aduser.PreviousExchangeGuid);
						if (mailboxLegacyDN != null && ConnectMailbox.FindMailboxByLegacyDN(mailboxLegacyDN, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, aduser.Id)) == null)
						{
							this.recoveredMailbox = true;
							aduser.Database = adobjectId;
							aduser.DatabaseAndLocation = null;
							aduser.ExchangeGuid = aduser.PreviousExchangeGuid;
							aduser.LegacyExchangeDN = mailboxLegacyDN;
							aduser.PreviousDatabase = null;
							aduser.PreviousExchangeGuid = Guid.Empty;
						}
					}
					if (this.recoveredMailbox)
					{
						this.recoverArchive = this.IsArchiveRecoverable(aduser);
						if (this.recoverArchive)
						{
							aduser.ArchiveGuid = aduser.DisabledArchiveGuid;
							aduser.ArchiveName = ((this.ArchiveName == null) ? new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + aduser.DisplayName) : this.ArchiveName);
							aduser.ArchiveDatabase = aduser.DisabledArchiveDatabase;
						}
					}
				}
			}
			if (!aduser.IsChanged(IADMailStorageSchema.Database))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorParameterRequiredButNotProvisioned("Database")), ErrorCategory.InvalidData, null);
				return;
			}
			MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = aduser.DatabaseAndLocation as MailboxDatabaseWithLocationInfo;
			if (mailboxDatabaseWithLocationInfo == null)
			{
				this.ValidateAndSetDatabase(new DatabaseIdParameter(aduser.Database), false);
				return;
			}
			this.database = mailboxDatabaseWithLocationInfo.MailboxDatabase;
			this.databaseLocationInfo = mailboxDatabaseWithLocationInfo.DatabaseLocationInfo;
			aduser.DatabaseAndLocation = null;
			aduser.propertyBag.ResetChangeTracking(IADMailStorageSchema.DatabaseAndLocation);
		}

		private void ValidateAndSetDatabase(DatabaseIdParameter databaseId, bool throwOnError)
		{
			this.InternalValidateAndSetArchiveDatabase(databaseId, Server.E15MinVersion, throwOnError, out this.database, out this.databaseLocationInfo);
		}

		private void ValidateAndSetArchiveDatabase(DatabaseIdParameter databaseId, bool throwOnError)
		{
			this.InternalValidateAndSetArchiveDatabase(databaseId, Server.E15MinVersion, throwOnError, out this.archiveDatabase, out this.archiveDatabaseLocationInfo);
		}

		private void InternalValidateAndSetArchiveDatabase(DatabaseIdParameter databaseId, int minServerVersion, bool throwOnError, out Database database, out DatabaseLocationInfo databaseLocationInfo)
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugStartSetDatabase);
			}
			database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(databaseId, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(databaseId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(databaseId.ToString())));
			databaseLocationInfo = this.GetDatabaseLocationInfo(database);
			Exception ex = null;
			if (minServerVersion > databaseLocationInfo.ServerVersion)
			{
				ex = new RecipientTaskException(Strings.ErrorMailboxDatabaseNotOnE15Server(database.ToString()));
			}
			else if (((MailboxDatabase)database).Recovery)
			{
				ex = new RecipientTaskException(Strings.ErrorRecoveryDatabase(database.Name));
			}
			if (ex != null)
			{
				if (throwOnError)
				{
					base.ThrowTerminatingError(ex, (ErrorCategory)1001, null);
				}
				else
				{
					base.WriteError(ex, (ErrorCategory)1001, null);
				}
			}
			MailboxTaskHelper.VerifyDatabaseIsWithinScopeForRecipientCmdlets(base.SessionSettings, database, new Task.ErrorLoggerDelegate(base.WriteError));
			if (base.IsDebugOn)
			{
				base.WriteDebug(Strings.DebugEndSetDatabase);
			}
		}

		private DatabaseLocationInfo GetDatabaseLocationInfo(Database database)
		{
			try
			{
				return this.ActiveManager.GetServerForDatabase(database.Guid);
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, (ErrorCategory)1000, null);
			}
			catch (ServerForDatabaseNotFoundException exception2)
			{
				base.WriteError(exception2, (ErrorCategory)1000, null);
			}
			return null;
		}

		private bool IsArchiveRecoverable(ADUser user)
		{
			bool result = false;
			if (this.archiveDatabase == null && user.DisabledArchiveGuid != Guid.Empty && (this.ArchiveGuid == Guid.Empty || this.ArchiveGuid == user.DisabledArchiveGuid))
			{
				result = ("RemoteArchive" == base.ParameterSetName || MailboxTaskHelper.IsArchiveRecoverable(user, this.ConfigurationSession, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, user.Id)));
			}
			return result;
		}

		private void CreateArchiveIfNecessary(ADUser user)
		{
			if (user.ArchiveGuid == Guid.Empty)
			{
				this.recoverArchive = this.IsArchiveRecoverable(user);
				user.ArchiveGuid = (this.recoverArchive ? user.DisabledArchiveGuid : ((this.ArchiveGuid == Guid.Empty) ? Guid.NewGuid() : this.ArchiveGuid));
				user.ArchiveName = ((this.ArchiveName == null) ? new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + user.DisplayName) : this.ArchiveName);
				if ("RemoteArchive" == base.ParameterSetName)
				{
					user.ArchiveDomain = this.ArchiveDomain;
					user.RemoteRecipientType = ((user.RemoteRecipientType &= ~RemoteRecipientType.DeprovisionArchive) | RemoteRecipientType.ProvisionArchive);
				}
				else
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.SetActiveArchiveStatus.Enabled)
					{
						user.ArchiveStatus |= ArchiveStatusFlags.Active;
					}
					if (this.recoverArchive)
					{
						user.ArchiveDatabase = user.DisabledArchiveDatabase;
					}
					else if (this.archiveDatabase == null)
					{
						user.ArchiveDatabase = user.Database;
					}
					else
					{
						user.ArchiveDatabase = this.archiveDatabase.Id;
					}
				}
				MailboxTaskHelper.ApplyDefaultArchivePolicy(user, this.ConfigurationSession);
				return;
			}
			base.WriteError(new RecipientTaskException(Strings.ErrorArchiveAlreadyPresent(this.Identity.ToString())), (ErrorCategory)1003, null);
		}

		private void StripInvalidSMTPAddresses(ADUser user)
		{
			ProxyAddressCollection emailAddresses = user.EmailAddresses;
			ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
			ProxyAddress proxyAddress = ProxyAddress.Parse(user.WindowsLiveID.ToString());
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (ProxyAddress proxyAddress2 in emailAddresses)
			{
				SmtpAddress smtpAddress = new SmtpAddress(proxyAddress2.AddressString);
				if (!(proxyAddress2 is SmtpProxyAddress) || !smtpAddress.IsValidAddress)
				{
					proxyAddressCollection.Add(proxyAddress2);
				}
				else if (RecipientTaskHelper.IsAcceptedDomain(this.ConfigurationSession, user.OrganizationId, smtpAddress.Domain, base.ProvisioningCache))
				{
					if (proxyAddress2.IsPrimaryAddress)
					{
						flag = true;
					}
					if (string.Compare(proxyAddress2.AddressString, user.WindowsLiveID.ToString(), StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						proxyAddressCollection.Add(proxyAddress2);
						if (proxyAddress2.IsPrimaryAddress)
						{
							flag3 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				if (flag2)
				{
					if (flag3)
					{
						proxyAddressCollection.Add(proxyAddress);
					}
					else
					{
						proxyAddressCollection.Add(proxyAddress.ToPrimary());
					}
				}
			}
			else
			{
				proxyAddressCollection.Add(proxyAddress.ToPrimary());
			}
			user.EmailAddresses = proxyAddressCollection;
		}

		protected override void PrepareRecipientAlias(ADUser dataObject)
		{
			if (!string.IsNullOrEmpty(base.Alias))
			{
				dataObject.Alias = base.Alias;
				return;
			}
			dataObject.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, dataObject.OrganizationId, string.IsNullOrEmpty(dataObject.UserPrincipalName) ? dataObject.SamAccountName : RecipientTaskHelper.GetLocalPartOfUserPrincalName(dataObject.UserPrincipalName), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
		}

		private ADUser FindMailboxPlanWithName(string mailboxPlanName, IRecipientSession session)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				MailboxTaskHelper.mailboxPlanFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, mailboxPlanName)
			});
			bool includeSoftDeletedObjects = session.SessionSettings.IncludeSoftDeletedObjects;
			ADUser[] array = null;
			try
			{
				session.SessionSettings.IncludeSoftDeletedObjects = false;
				array = session.FindADUser(null, QueryScope.SubTree, filter, null, 1);
			}
			finally
			{
				session.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			return null;
		}

		private static readonly PropertyDefinition[] PropertiesToResetForMailUser = new PropertyDefinition[]
		{
			ADMailboxRecipientSchema.ExchangeGuid,
			ADRecipientSchema.RawExternalEmailAddress,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.RecipientLimits,
			ADRecipientSchema.RecipientTypeDetails,
			ADMailboxRecipientSchema.RulesQuota,
			ADUserSchema.MailboxMoveTargetMDB,
			ADUserSchema.MailboxMoveSourceMDB,
			ADUserSchema.MailboxMoveFlags,
			ADUserSchema.MailboxMoveStatus,
			ADUserSchema.MailboxMoveRemoteHostName,
			ADUserSchema.MailboxMoveBatchName
		};

		private Database database;

		private Database archiveDatabase;

		private ADObjectId retentionPolicyId;

		private ADObjectId mobileMailboxPolicyId;

		private ADObjectId userSpecifiedRoleAssignmentPolicyId;

		private ADObjectId defaultRoleAssignmentPolicyId;

		private ADObjectId addressBookPolicyId;

		private SecurityIdentifier linkedUserSid;

		private DatabaseLocationInfo databaseLocationInfo;

		private DatabaseLocationInfo archiveDatabaseLocationInfo;

		private MailboxPlan mailboxPlan;

		private RecipientType originalRecipientType;

		public static readonly string DiscoveryMailboxUniqueName = "DiscoverySearchMailbox {D919BA05-46A6-415f-80AD-7E09334BB852}";

		public static readonly string DiscoveryMailboxDisplayName = Strings.DiscoveryMailboxDisplayName;

		private bool recoveredMailbox;

		private bool recoverArchive;
	}
}
