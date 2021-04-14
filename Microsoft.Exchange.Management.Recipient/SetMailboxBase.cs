using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class SetMailboxBase<TIdentity, TPublicObject> : SetUserBase<TIdentity, TPublicObject> where TIdentity : IIdentityParameter, new() where TPublicObject : Mailbox, new()
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject.IsModified(MailboxSchema.WindowsLiveID) && this.DataObject.WindowsLiveID != SmtpAddress.Empty)
				{
					TIdentity identity = this.Identity;
					return Strings.ConfirmationMessageSetMailboxWithWindowsLiveID(identity.ToString(), this.DataObject.WindowsLiveID.ToString());
				}
				TIdentity identity2 = this.Identity;
				return Strings.ConfirmationMessageSetMailbox(identity2.ToString());
			}
		}

		protected bool NeedChangeMailboxSubtype
		{
			get
			{
				return this.needChangeMailboxSubtype;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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
		public RecipientIdParameter ForwardingAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields[MailboxSchema.ForwardingAddress];
			}
			set
			{
				base.Fields[MailboxSchema.ForwardingAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OfflineAddressBookIdParameter OfflineAddressBook
		{
			get
			{
				return (OfflineAddressBookIdParameter)base.Fields[MailboxSchema.OfflineAddressBook];
			}
			set
			{
				base.Fields[MailboxSchema.OfflineAddressBook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields[MailboxSchema.LinkedMasterAccount];
			}
			set
			{
				base.Fields[MailboxSchema.LinkedMasterAccount] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
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
		public ConvertibleMailboxSubType Type
		{
			get
			{
				return (ConvertibleMailboxSubType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ApplyMandatoryProperties
		{
			get
			{
				return (SwitchParameter)(base.Fields["ApplyMandatoryProperties"] ?? false);
			}
			set
			{
				base.Fields["ApplyMandatoryProperties"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoveManagedFolderAndPolicy
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveManagedFolderAndPolicy"] ?? false);
			}
			set
			{
				base.Fields["RemoveManagedFolderAndPolicy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public ThrottlingPolicyIdParameter ThrottlingPolicy
		{
			get
			{
				return (ThrottlingPolicyIdParameter)base.Fields[MailboxSchema.ThrottlingPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.ThrottlingPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SharingPolicyIdParameter SharingPolicy
		{
			get
			{
				return (SharingPolicyIdParameter)base.Fields[MailboxSchema.SharingPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.SharingPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteAccountPolicyIdParameter RemoteAccountPolicy
		{
			get
			{
				return (RemoteAccountPolicyIdParameter)base.Fields[MailboxSchema.RemoteAccountPolicy];
			}
			set
			{
				base.Fields[MailboxSchema.RemoteAccountPolicy] = value;
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

		protected MailboxPlanIdParameter MailboxPlan
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

		[Parameter(Mandatory = false)]
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

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			if (base.ShouldUpgradeExchangeVersion(adObject))
			{
				return true;
			}
			if (this.ApplyMandatoryProperties.IsPresent)
			{
				this.VerifyMandatoryPropertiesAppliable(null);
			}
			return this.needApplyMandatoryProperties;
		}

		private void VerifyMandatoryPropertiesAppliable(ADUser mailbox)
		{
			if (this.needApplyMandatoryProperties)
			{
				return;
			}
			string serverLegacyDN;
			if (mailbox != null)
			{
				serverLegacyDN = mailbox.ServerLegacyDN;
			}
			else
			{
				TPublicObject instance = this.Instance;
				serverLegacyDN = instance.ServerLegacyDN;
			}
			if (!string.IsNullOrEmpty(serverLegacyDN))
			{
				AdministrativeGroup administrativeGroup = base.GlobalConfigSession.GetAdministrativeGroup();
				if (serverLegacyDN.StartsWith(administrativeGroup.LegacyExchangeDN + "/"))
				{
					this.needApplyMandatoryProperties = true;
					return;
				}
				TIdentity identity = this.Identity;
				LocalizedException exception = new TaskInvalidOperationException(Strings.ErrorNoNeedApplyMandatoryProperties(identity.ToString()));
				ExchangeErrorCategory category = ExchangeErrorCategory.Client;
				TPublicObject instance2 = this.Instance;
				base.WriteError(exception, category, instance2.Id);
			}
		}

		protected override void UpgradeExchangeVersion(ADObject adObject)
		{
			string text = (string)adObject[ADMailboxRecipientSchema.ServerLegacyDN];
			if (!string.IsNullOrEmpty(text))
			{
				Server server = base.GlobalConfigSession.FindServerByLegacyDN(text);
				if (server == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorServerNotFound(text)), ExchangeErrorCategory.Client, null);
				}
				ExchangeObjectVersion exchangeVersion = ExchangeObjectVersion.Exchange2003;
				if (server.IsE14OrLater)
				{
					ADRecipient adrecipient = adObject as ADRecipient;
					MailEnabledRecipient mailEnabledRecipient = adObject as MailEnabledRecipient;
					RecipientTypeDetails recipientTypeDetails = (adrecipient != null) ? adrecipient.RecipientTypeDetails : ((mailEnabledRecipient != null) ? mailEnabledRecipient.RecipientTypeDetails : RecipientTypeDetails.None);
					exchangeVersion = ADUser.GetMaximumSupportedExchangeObjectVersion(recipientTypeDetails, false);
				}
				else if (server.IsExchange2007OrLater)
				{
					exchangeVersion = ExchangeObjectVersion.Exchange2007;
				}
				adObject.SetExchangeVersion(exchangeVersion);
			}
		}

		internal static void StampMailboxTypeDetails(ADRecipient recipient, bool e2k7Mailbox)
		{
			if (!e2k7Mailbox && recipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007) && RecipientTypeDetails.LegacyMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.LegacyMailbox;
				return;
			}
			if (recipient.IsResource)
			{
				if (recipient.ResourceType == ExchangeResourceType.Room)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.RoomMailbox;
					return;
				}
				if (recipient.ResourceType == ExchangeResourceType.Equipment)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.EquipmentMailbox;
					return;
				}
			}
			else if (recipient.IsLinked)
			{
				if (recipient.ResourceType == ExchangeResourceType.Room)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.LinkedRoomMailbox;
					return;
				}
				recipient.RecipientTypeDetails = RecipientTypeDetails.LinkedMailbox;
				return;
			}
			else if (recipient.IsShared)
			{
				if (RecipientTypeDetails.ArbitrationMailbox != recipient.RecipientTypeDetails && RecipientTypeDetails.DiscoveryMailbox != recipient.RecipientTypeDetails && RecipientTypeDetails.TeamMailbox != recipient.RecipientTypeDetails && RecipientTypeDetails.PublicFolderMailbox != recipient.RecipientTypeDetails && RecipientTypeDetails.AuditLogMailbox != recipient.RecipientTypeDetails)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.SharedMailbox;
					return;
				}
			}
			else
			{
				if (RecipientTypeDetails.MailboxPlan == recipient.RecipientTypeDetails)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.MailboxPlan;
					return;
				}
				if (RecipientTypeDetails.MonitoringMailbox == recipient.RecipientTypeDetails)
				{
					recipient.RecipientTypeDetails = RecipientTypeDetails.MonitoringMailbox;
					return;
				}
				recipient.RecipientTypeDetails = RecipientTypeDetails.UserMailbox;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.RemoveManagedFolderAndPolicy)
			{
				this.RetentionPolicy = null;
			}
			base.InternalBeginProcessing();
			base.CheckExclusiveParameters(new object[]
			{
				ADRecipientSchema.MailboxPlan,
				"SKUCapability"
			});
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			this.needChangeMailboxSubtype = base.Fields.IsModified("Type");
			if (this.needChangeMailboxSubtype)
			{
				if (ConvertibleMailboxSubType.Equipment != this.Type && ConvertibleMailboxSubType.Room != this.Type && (mailbox.ResourceCustom.Changed || mailbox.ResourceCapacity != null))
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorCannotChangeResourceProperties, null), ExchangeErrorCategory.Client, null);
				}
				if (base.Fields.IsModified("LinkedCredential") || base.Fields.IsModified("LinkedDomainController") || base.Fields.IsModified(MailboxSchema.LinkedMasterAccount))
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorCannotChangeLinkedProperty, null), ExchangeErrorCategory.Client, null);
				}
			}
			if (mailbox.IsModified(MailboxSchema.UseDatabaseRetentionDefaults) && mailbox.IsModified(MailboxSchema.RetainDeletedItemsUntilBackup) && mailbox.UseDatabaseRetentionDefaults && mailbox.RetainDeletedItemsUntilBackup)
			{
				base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorOnlyOneParameterMayBeUsed), ExchangeErrorCategory.Client, this);
			}
			if (mailbox.IsModified(MailboxSchema.AuditOwner))
			{
				MultiValuedProperty<MailboxAuditOperations> auditOwner = mailbox.AuditOwner;
				foreach (MailboxAuditOperations mailboxAuditOperations in auditOwner)
				{
					if ((mailboxAuditOperations & SetMailboxBase<TIdentity, TPublicObject>.UnsupportedOwnerOperations) != MailboxAuditOperations.None)
					{
						base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorInvalidAuditOwnerOperationType), ExchangeErrorCategory.Client, this);
					}
				}
			}
			if (mailbox.IsModified(MailboxSchema.AuditDelegate))
			{
				MultiValuedProperty<MailboxAuditOperations> auditDelegate = mailbox.AuditDelegate;
				foreach (MailboxAuditOperations mailboxAuditOperations2 in auditDelegate)
				{
					if ((mailboxAuditOperations2 & SetMailboxBase<TIdentity, TPublicObject>.UnsupportedDelegateOperations) != MailboxAuditOperations.None)
					{
						base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorInvalidAuditDelegateOperationType), ExchangeErrorCategory.Client, this);
					}
				}
			}
			if (mailbox.IsModified(MailboxSchema.AuditAdmin))
			{
				MultiValuedProperty<MailboxAuditOperations> auditAdmin = mailbox.AuditAdmin;
				foreach (MailboxAuditOperations mailboxAuditOperations3 in auditAdmin)
				{
					if ((mailboxAuditOperations3 & SetMailboxBase<TIdentity, TPublicObject>.UnsupportedAdminOperations) != MailboxAuditOperations.None)
					{
						base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorInvalidAuditAdminOperationType), ExchangeErrorCategory.Client, this);
					}
				}
			}
			if (this.LinkedMasterAccount != null)
			{
				if (string.IsNullOrEmpty(this.LinkedDomainController))
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorMissLinkedDomainController), ExchangeErrorCategory.Client, this);
				}
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					mailbox[ADRecipientSchema.MasterAccountSid] = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
				}
				catch (PSArgumentException exception)
				{
					base.ThrowNonLocalizedTerminatingError(exception, ExchangeErrorCategory.Client, this.LinkedCredential);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			if (this.RemoveManagedFolderAndPolicy)
			{
				mailbox.ManagedFolderMailboxPolicy = null;
			}
			if (base.Fields.IsModified(MailboxSchema.RetentionPolicy))
			{
				if (this.RetentionPolicy != null)
				{
					RetentionPolicy retentionPolicy = (RetentionPolicy)base.GetDataObject<RetentionPolicy>(this.RetentionPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRetentionPolicyNotFound(this.RetentionPolicy.ToString())), new LocalizedString?(Strings.ErrorRetentionPolicyNotUnique(this.RetentionPolicy.ToString())), ExchangeErrorCategory.Client);
					mailbox.RetentionPolicy = (ADObjectId)retentionPolicy.Identity;
					mailbox.ManagedFolderMailboxPolicy = null;
				}
				else
				{
					mailbox.RetentionPolicy = null;
				}
			}
			base.SetReferenceParameter<RecipientIdParameter>(MailboxSchema.ForwardingAddress, this.ForwardingAddress, mailbox, new GetRecipientDelegate<RecipientIdParameter>(this.GetRecipient));
			if (base.Fields.IsModified(MailboxSchema.OfflineAddressBook))
			{
				if (this.OfflineAddressBook != null)
				{
					OfflineAddressBook offlineAddressBook = (OfflineAddressBook)base.GetDataObject<OfflineAddressBook>(this.OfflineAddressBook, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOfflineAddressBookNotFound(this.OfflineAddressBook.ToString())), new LocalizedString?(Strings.ErrorOfflineAddressBookNotUnique(this.OfflineAddressBook.ToString())), ExchangeErrorCategory.Client);
					mailbox.OfflineAddressBook = (ADObjectId)offlineAddressBook.Identity;
				}
				else
				{
					mailbox.OfflineAddressBook = null;
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.AddressBookPolicy))
			{
				AddressBookMailboxPolicyIdParameter addressBookMailboxPolicyIdParameter = (AddressBookMailboxPolicyIdParameter)base.Fields[ADRecipientSchema.AddressBookPolicy];
				if (addressBookMailboxPolicyIdParameter != null)
				{
					AddressBookMailboxPolicy addressBookMailboxPolicy = (AddressBookMailboxPolicy)base.GetDataObject<AddressBookMailboxPolicy>(addressBookMailboxPolicyIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotFound(addressBookMailboxPolicyIdParameter.ToString())), new LocalizedString?(Strings.ErrorAddressBookMailboxPolicyNotUnique(addressBookMailboxPolicyIdParameter.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADRecipientSchema.AddressBookPolicy] = (ADObjectId)addressBookMailboxPolicy.Identity;
				}
				else
				{
					mailbox[ADRecipientSchema.AddressBookPolicy] = null;
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.ThrottlingPolicy))
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new TaskInvalidOperationException(Strings.ErrorLinkOpOnDehydratedTenant("ThrottlingPolicy")), ExchangeErrorCategory.Context, this.DataObject.Identity);
				}
				ThrottlingPolicyIdParameter throttlingPolicyIdParameter = (ThrottlingPolicyIdParameter)base.Fields[ADRecipientSchema.ThrottlingPolicy];
				if (throttlingPolicyIdParameter != null)
				{
					ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)base.GetDataObject<ThrottlingPolicy>(throttlingPolicyIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorThrottlingPolicyNotFound(throttlingPolicyIdParameter.ToString())), new LocalizedString?(Strings.ErrorThrottlingPolicyNotUnique(throttlingPolicyIdParameter.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADRecipientSchema.ThrottlingPolicy] = (ADObjectId)throttlingPolicy.Identity;
				}
				else
				{
					mailbox[ADRecipientSchema.ThrottlingPolicy] = null;
				}
			}
			if (base.Fields.IsModified(ADUserSchema.SharingPolicy))
			{
				if (SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
				{
					base.WriteError(new LocalizedException(Strings.ErrorLinkOpOnDehydratedTenant("SharingPolicy")), ExchangeErrorCategory.Client, this.DataObject);
				}
				SharingPolicyIdParameter sharingPolicyIdParameter = (SharingPolicyIdParameter)base.Fields[ADUserSchema.SharingPolicy];
				if (sharingPolicyIdParameter != null)
				{
					SharingPolicy sharingPolicy = (SharingPolicy)base.GetDataObject<SharingPolicy>(sharingPolicyIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorSharingPolicyNotFound(sharingPolicyIdParameter.ToString())), new LocalizedString?(Strings.ErrorSharingPolicyNotUnique(sharingPolicyIdParameter.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADUserSchema.SharingPolicy] = (ADObjectId)sharingPolicy.Identity;
				}
				else
				{
					mailbox[ADUserSchema.SharingPolicy] = null;
				}
			}
			if (base.Fields.IsModified(ADUserSchema.RemoteAccountPolicy))
			{
				RemoteAccountPolicyIdParameter remoteAccountPolicyIdParameter = (RemoteAccountPolicyIdParameter)base.Fields[ADUserSchema.RemoteAccountPolicy];
				if (remoteAccountPolicyIdParameter != null)
				{
					RemoteAccountPolicy remoteAccountPolicy = (RemoteAccountPolicy)base.GetDataObject<RemoteAccountPolicy>(remoteAccountPolicyIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRemoteAccountPolicyNotFound(remoteAccountPolicyIdParameter.ToString())), new LocalizedString?(Strings.ErrorRemoteAccountPolicyNotUnique(remoteAccountPolicyIdParameter.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADUserSchema.RemoteAccountPolicy] = (ADObjectId)remoteAccountPolicy.Identity;
				}
				else
				{
					mailbox[ADUserSchema.RemoteAccountPolicy] = null;
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.RoleAssignmentPolicy))
			{
				MailboxPolicyIdParameter mailboxPolicyIdParameter = (MailboxPolicyIdParameter)base.Fields[ADRecipientSchema.RoleAssignmentPolicy];
				if (mailboxPolicyIdParameter != null)
				{
					IConfigurationSession tenantLocalConfigSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
					RoleAssignmentPolicy roleAssignmentPolicy = (RoleAssignmentPolicy)base.GetDataObject<RoleAssignmentPolicy>(mailboxPolicyIdParameter, tenantLocalConfigSession, null, new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotFound(mailboxPolicyIdParameter.ToString())), new LocalizedString?(Strings.ErrorRoleAssignmentPolicyNotUnique(mailboxPolicyIdParameter.ToString())), ExchangeErrorCategory.Client);
					mailbox[ADRecipientSchema.RoleAssignmentPolicy] = (ADObjectId)roleAssignmentPolicy.Identity;
				}
				else
				{
					mailbox[ADRecipientSchema.RoleAssignmentPolicy] = null;
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.MailboxPlan))
			{
				if (this.MailboxPlan != null)
				{
					ADUser aduser = base.ProvisioningCache.TryAddAndGetOrganizationDictionaryValue<ADUser, string>(CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId, base.CurrentOrganizationId, this.MailboxPlan.RawIdentity, () => (ADUser)base.GetDataObject<ADUser>(this.MailboxPlan, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.MailboxPlan.ToString())), ExchangeErrorCategory.Client));
					MailboxTaskHelper.ValidateMailboxPlanRelease(aduser, new Task.ErrorLoggerDelegate(base.WriteError));
					mailbox[ADRecipientSchema.MailboxPlan] = (ADObjectId)aduser.Identity;
					return;
				}
				mailbox[ADRecipientSchema.MailboxPlan] = null;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.needApplyMandatoryProperties = false;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADUser aduser = (ADUser)dataObject;
			this.originalForwardingAddress = aduser.ForwardingAddress;
			this.originalForwardingSmtpAddress = aduser.ForwardingSmtpAddress;
			Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
			bool flag = aduser.IsResource;
			if (this.ApplyMandatoryProperties.IsPresent)
			{
				this.VerifyMandatoryPropertiesAppliable(aduser);
				SetMailboxBase<TIdentity, TPublicObject>.StampMailboxTypeDetails(aduser, this.needApplyMandatoryProperties);
			}
			this.needChangeMailboxSubtype = base.Fields.IsModified("Type");
			if (this.needChangeMailboxSubtype)
			{
				this.originalRecipientTypeDetails = aduser.RecipientTypeDetails;
				this.targetRecipientTypeDetails = (RecipientTypeDetails)this.Type;
				this.needChangeMailboxSubtype = (this.originalRecipientTypeDetails != this.targetRecipientTypeDetails);
				if (!this.needChangeMailboxSubtype)
				{
					TIdentity identity = this.Identity;
					this.WriteWarning(Strings.WarningNoNeedToConvertMailboxType(identity.ToString(), this.Type.ToString()));
				}
				else
				{
					if (!Enum.IsDefined(typeof(ConvertibleMailboxSubType), (long)this.originalRecipientTypeDetails) || !Enum.IsDefined(typeof(ConvertibleMailboxSubType), (long)this.targetRecipientTypeDetails))
					{
						TIdentity identity2 = this.Identity;
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorInvalidMailboxTypeConversion(identity2.ToString(), this.originalRecipientTypeDetails.ToString(), this.targetRecipientTypeDetails.ToString())), ExchangeErrorCategory.Client, aduser.Id);
					}
					RecipientTypeDetails recipientTypeDetails = this.targetRecipientTypeDetails;
					if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.UserMailbox)
						{
							if (recipientTypeDetails == RecipientTypeDetails.SharedMailbox)
							{
								aduser.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
								TPublicObject instance = this.Instance;
								instance[MailboxSchema.MasterAccountSid] = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
								TPublicObject instance2 = this.Instance;
								instance2[MailboxSchema.ResourceType] = null;
								TPublicObject instance3 = this.Instance;
								instance3.ResourceCapacity = null;
								flag = false;
							}
						}
						else
						{
							aduser.UserAccountControl &= ~UserAccountControlFlags.AccountDisabled;
							aduser.UserAccountControl |= UserAccountControlFlags.NormalAccount;
							TPublicObject instance4 = this.Instance;
							instance4[MailboxSchema.MasterAccountSid] = null;
							TPublicObject instance5 = this.Instance;
							instance5[MailboxSchema.ResourceType] = null;
							TPublicObject instance6 = this.Instance;
							instance6.ResourceCapacity = null;
							flag = false;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox)
					{
						if (recipientTypeDetails == RecipientTypeDetails.EquipmentMailbox)
						{
							aduser.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
							TPublicObject instance7 = this.Instance;
							instance7[MailboxSchema.MasterAccountSid] = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
							TPublicObject instance8 = this.Instance;
							instance8[MailboxSchema.ResourceType] = ExchangeResourceType.Equipment;
							TPublicObject instance9 = this.Instance;
							instance9.ResourceCapacity = mailbox.ResourceCapacity;
							flag = true;
						}
					}
					else
					{
						aduser.UserAccountControl |= UserAccountControlFlags.AccountDisabled;
						TPublicObject instance10 = this.Instance;
						instance10[MailboxSchema.MasterAccountSid] = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
						TPublicObject instance11 = this.Instance;
						instance11[MailboxSchema.ResourceType] = ExchangeResourceType.Room;
						TPublicObject instance12 = this.Instance;
						instance12.ResourceCapacity = mailbox.ResourceCapacity;
						flag = true;
					}
					TPublicObject instance13 = this.Instance;
					instance13[ADRecipientSchema.RecipientTypeDetails] = this.targetRecipientTypeDetails;
				}
			}
			if (!this.needChangeMailboxSubtype)
			{
				if (!flag)
				{
					TPublicObject instance14 = this.Instance;
					if (!instance14.IsModified(ADRecipientSchema.ResourceMetaData))
					{
						TPublicObject instance15 = this.Instance;
						if (!instance15.IsModified(ADRecipientSchema.ResourceSearchProperties))
						{
							TPublicObject instance16 = this.Instance;
							if (!instance16.IsModified(ADRecipientSchema.ResourcePropertiesDisplay))
							{
								TPublicObject instance17 = this.Instance;
								if (!instance17.IsModified(ADRecipientSchema.ResourceType))
								{
									TPublicObject instance18 = this.Instance;
									if (!instance18.IsModified(ADRecipientSchema.ResourceCustom))
									{
										TPublicObject instance19 = this.Instance;
										if (!instance19.IsModified(ADRecipientSchema.ResourceCapacity))
										{
											goto IL_46A;
										}
									}
								}
							}
						}
					}
					base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotChangeResourceProperties), ExchangeErrorCategory.Client, aduser);
				}
				IL_46A:
				if (!aduser.IsLinked)
				{
					TPublicObject instance20 = this.Instance;
					if (instance20.IsModified(ADRecipientSchema.MasterAccountSid) && aduser.RecipientTypeDetails != RecipientTypeDetails.RoomMailbox)
					{
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotChangeLinkedProperty), ExchangeErrorCategory.Client, aduser);
					}
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.MailboxPlan) || (base.Fields.IsModified("SKUCapability") && aduser.RecipientTypeDetails != RecipientTypeDetails.MailboxPlan))
			{
				if (base.Fields.IsModified(ADRecipientSchema.MailboxPlan) && this.MailboxPlan == null)
				{
					if (aduser.OrganizationId != null && !aduser.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
					{
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotUnlinkMailboxPlanForTenant), ExchangeErrorCategory.Client, aduser);
					}
				}
				else
				{
					ADUser aduser2 = null;
					if (base.Fields.IsModified(ADRecipientSchema.MailboxPlan))
					{
						ADObjectId adobjectId = (ADObjectId)mailbox[ADRecipientSchema.MailboxPlan];
						if (!adobjectId.Equals(aduser.MailboxPlan))
						{
							aduser2 = (ADUser)base.GetDataObject<ADUser>(new MailboxPlanIdParameter(adobjectId), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.MailboxPlan.ToString())), ExchangeErrorCategory.Client);
						}
					}
					else if (base.Fields.IsModified("SKUCapability"))
					{
						bool checkCurrentReleasePlanFirst = RecipientTaskHelper.IsOrganizationInPilot(this.ConfigurationSession, aduser.OrganizationId);
						LocalizedString message;
						aduser2 = MailboxTaskHelper.FindMailboxPlanWithSKUCapability(base.SKUCapability, (IRecipientSession)base.DataSession, out message, checkCurrentReleasePlanFirst);
						if (aduser2 == null && aduser.RecipientTypeDetails != RecipientTypeDetails.MailboxPlan)
						{
							base.WriteError(new RecipientTaskException(message), ExchangeErrorCategory.Client, aduser.Id);
						}
					}
					MailboxTaskHelper.UpdateMailboxPlan(aduser, aduser2, (ADObjectId mbxPlanId) => (ADUser)base.GetDataObject<ADUser>(new MailboxPlanIdParameter(mbxPlanId), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(mbxPlanId.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(mbxPlanId.ToString())), ExchangeErrorCategory.Client));
				}
			}
			base.StampChangesOn(dataObject);
			if (this.needChangeMailboxSubtype && flag && mailbox.ResourceCustom.Count != 0)
			{
				aduser.ResourceCustom = mailbox.ResourceCustom;
			}
			bool flag2 = false;
			if (aduser.IsLinked && aduser.RecipientTypeDetails == RecipientTypeDetails.RoomMailbox)
			{
				flag2 = true;
			}
			if (flag && !aduser.IsResource && !flag2)
			{
				base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotChangeResourceMailboxToUserMailbox(aduser.Identity.ToString())), ExchangeErrorCategory.Client, aduser);
			}
			SetMailboxBase<TIdentity, TPublicObject>.StampMailboxTypeDetails(aduser, this.needApplyMandatoryProperties);
			MailboxTaskHelper.StampMailboxRecipientDisplayType(aduser);
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject = (ADUser)base.PrepareDataObject();
			if (this.DataObject.IsChanged(MailboxSchema.WindowsLiveID) && this.DataObject.WindowsLiveID != SmtpAddress.Empty)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
				SmtpAddress value = (SmtpAddress)this.DataObject.GetOriginalObject()[MailboxSchema.WindowsLiveID];
				if (value == SmtpAddress.Empty || value == this.DataObject.PrimarySmtpAddress)
				{
					this.DataObject.PrimarySmtpAddress = this.DataObject.WindowsLiveID;
				}
			}
			bool flag = false;
			ADUser dataObject = this.DataObject;
			if (this.needChangeMailboxSubtype)
			{
				if (this.originalRecipientTypeDetails == RecipientTypeDetails.UserMailbox || this.targetRecipientTypeDetails == RecipientTypeDetails.UserMailbox)
				{
					flag = true;
				}
			}
			else if (dataObject.IsChanged(ADRecipientSchema.MasterAccountSid))
			{
				flag = true;
			}
			if (!flag && (this.RemoveManagedFolderAndPolicy || base.Fields.IsModified(ADUserSchema.SharingPolicy)))
			{
				flag = true;
			}
			if (flag)
			{
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, dataObject, false, this.ConfirmationMessage, new CmdletProxyInfo.ChangeCmdletProxyParametersDelegate(CmdletProxy.AppendForceToProxyCmdlet));
			}
			if ((dataObject.IsChanged(ADUserSchema.LitigationHoldEnabled) && dataObject.LitigationHoldEnabled) || (dataObject.IsChanged(ADRecipientSchema.InPlaceHoldsRaw) && dataObject.IsInLitigationHoldOrInplaceHold))
			{
				RecoverableItemsQuotaHelper.IncreaseRecoverableItemsQuotaIfNeeded(dataObject);
			}
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.Fields.IsModified(ADRecipientSchema.RoleAssignmentPolicy) && (MailboxPolicyIdParameter)base.Fields[ADRecipientSchema.RoleAssignmentPolicy] == null && !OrganizationId.ForestWideOrgId.Equals(this.DataObject.OrganizationId))
			{
				base.WriteError(new TaskInvalidOperationException(Strings.ResettingPolicyIsNotSupported("RoleAssignmentPolicy")), ExchangeErrorCategory.Client, this.Identity);
			}
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(base.CurrentOrganizationId);
			if (sharedConfiguration != null)
			{
				this.DataObject.SharedConfiguration = sharedConfiguration.SharedConfigId.ConfigurationUnit;
			}
			if (this.DataObject.IsResource)
			{
				this.CheckResourceProperties();
			}
			else if (this.DataObject.RecipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
			{
				if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom) || base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers))
				{
					base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotChangeAcceptMessagesFrom), ExchangeErrorCategory.Client, this.DataObject);
				}
				MailboxTaskHelper.ValidateMaximumDiscoveryMailboxQuota(this.DataObject, this.ConfigurationSession, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			else if (this.PublicFolder && base.UserSpecifiedParameters.Contains(MailEnabledRecipientSchema.HiddenFromAddressListsEnabled.Name))
			{
				base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotChangeHiddenFromAddressListsEnabled), ExchangeErrorCategory.Client, this.DataObject);
			}
			if (this.RetentionPolicy != null && this.DataObject.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNoRetentionPolicyForTeamMailbox(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
			}
			if (this.AuditLog)
			{
				Mailbox mailbox = (Mailbox)this.GetDynamicParameters();
				foreach (object obj in SetMailboxBase<MailboxIdParameter, Mailbox>.InvalidAuditLogParameters)
				{
					ProviderPropertyDefinition providerPropertyDefinition = obj as ProviderPropertyDefinition;
					if (base.Fields.IsModified(obj) || (providerPropertyDefinition != null && mailbox.IsModified(providerPropertyDefinition)))
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorInvalidParameterForAuditLog((providerPropertyDefinition == null) ? obj.ToString() : providerPropertyDefinition.Name, "AuditLog")), ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
			else if (this.DataObject.RecipientTypeDetails == RecipientTypeDetails.AuditLogMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.RecipientNotFoundException(this.DataObject.Name)), ExchangeErrorCategory.Client, this.DataObject);
			}
			base.VerifyIsWithinScopes((IRecipientSession)base.DataSession, this.DataObject, true, new DataAccessTask<ADUser>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			bool flag = false;
			if (false == this.Force && this.Arbitration)
			{
				TIdentity identity = this.Identity;
				if (!base.ShouldContinue(Strings.SetArbitrationMailboxConfirmationMessage(identity.ToString())))
				{
					TaskLogger.LogExit();
					return;
				}
			}
			if (false == this.Force && this.originalForwardingAddress == null && this.DataObject.ForwardingAddress != null && this.DataObject.ForwardingSmtpAddress != null)
			{
				LocalizedString message = (this.originalForwardingSmtpAddress != null) ? Strings.SetMailboxForwardingAddressConfirmationMessage : Strings.SetBothForwardingAddressConfirmationMessage;
				if (!base.ShouldContinue(message))
				{
					TaskLogger.LogExit();
					return;
				}
			}
			if (this.DataObject.IsModified(MailboxSchema.ForwardingSmtpAddress) && this.DataObject.ForwardingSmtpAddress != null && this.DataObject.ForwardingAddress != null && !base.Fields.IsModified(MailboxSchema.ForwardingAddress))
			{
				this.WriteWarning(Strings.ContactAdminForForwardingWarning);
			}
			if (false == this.Force && this.DataObject.IsModified(ADRecipientSchema.AuditLogAgeLimit))
			{
				EnhancedTimeSpan t;
				if (this.DataObject.MailboxAuditLogAgeLimit == EnhancedTimeSpan.Zero)
				{
					TIdentity identity2 = this.Identity;
					if (!base.ShouldContinue(Strings.ConfirmationMessageSetMailboxAuditLogAgeLimitZero(identity2.ToString())))
					{
						TaskLogger.LogExit();
						return;
					}
				}
				else if (this.DataObject.TryGetOriginalValue<EnhancedTimeSpan>(ADRecipientSchema.AuditLogAgeLimit, out t))
				{
					EnhancedTimeSpan mailboxAuditLogAgeLimit = this.DataObject.MailboxAuditLogAgeLimit;
					if (t > mailboxAuditLogAgeLimit)
					{
						TIdentity identity3 = this.Identity;
						if (!base.ShouldContinue(Strings.ConfirmationMessageSetMailboxAuditLogAgeLimitSmaller(identity3.ToString(), mailboxAuditLogAgeLimit.ToString())))
						{
							TaskLogger.LogExit();
							return;
						}
					}
				}
			}
			bool flag2 = false;
			bool flag3 = false;
			MapiMessageStoreSession mapiMessageStoreSession = null;
			try
			{
				if (this.needChangeMailboxSubtype)
				{
					if (this.originalRecipientTypeDetails == RecipientTypeDetails.UserMailbox)
					{
						MailboxTaskHelper.GrantPermissionToLinkedUserAccount(this.DataObject, PermissionTaskHelper.GetReadOnlySession(null), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
						flag2 = true;
						flag3 = true;
					}
					else if (this.targetRecipientTypeDetails == RecipientTypeDetails.UserMailbox)
					{
						MailboxTaskHelper.ClearExternalAssociatedAccountPermission(this.DataObject, PermissionTaskHelper.GetReadOnlySession(null), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
						flag = true;
						flag3 = true;
					}
				}
				else if (this.DataObject.IsChanged(ADRecipientSchema.MasterAccountSid))
				{
					MailboxTaskHelper.GrantPermissionToLinkedUserAccount(this.DataObject, PermissionTaskHelper.GetReadOnlySession(null), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
					flag2 = true;
					flag3 = true;
				}
				base.InternalProcessRecord();
				if (flag3)
				{
					PermissionTaskHelper.SaveMailboxSecurityDescriptor(this.DataObject, SecurityDescriptorConverter.ConvertToActiveDirectorySecurity(this.DataObject.ExchangeSecurityDescriptor), (IRecipientSession)base.DataSession, ref mapiMessageStoreSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			finally
			{
				if (mapiMessageStoreSession != null)
				{
					mapiMessageStoreSession.Dispose();
				}
			}
			if (flag2)
			{
				base.WriteVerbose(Strings.VerboseSaveADSecurityDescriptor(this.DataObject.Id.ToString()));
				this.DataObject.SaveSecurityDescriptor(((SecurityDescriptor)this.DataObject[ADObjectSchema.NTSecurityDescriptor]).ToRawSecurityDescriptor());
			}
			bool flag4 = base.Fields.IsModified(ADUserSchema.SharingPolicy);
			if (this.RemoveManagedFolderAndPolicy || flag || flag4)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 4021, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\SetMailbox.cs");
				if (!tenantOrRootOrgRecipientSession.IsReadConnectionAvailable())
				{
					tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 4030, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\SetMailbox.cs");
				}
				MailboxSession mailboxSession = this.OpenMailboxSession(tenantOrRootOrgRecipientSession, this.DataObject);
				if (mailboxSession == null)
				{
					base.WriteError(new RecipientTaskException(Strings.LogonFailure), ExchangeErrorCategory.ServerOperation, null);
					return;
				}
				using (mailboxSession)
				{
					if (this.RemoveManagedFolderAndPolicy && !ElcMailboxHelper.RemoveElcInMailbox(mailboxSession))
					{
						this.WriteWarning(Strings.WarningNonemptyManagedFolderNotDeleted);
					}
					if (flag)
					{
						using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(mailboxSession))
						{
							CalendarConfiguration calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
							calendarConfiguration.AutomateProcessing = CalendarProcessingFlags.None;
							try
							{
								calendarConfigurationDataProvider.Save(calendarConfiguration);
							}
							catch (LocalizedException exception)
							{
								base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
							}
						}
					}
					if (flag4)
					{
						mailboxSession.Mailbox.Delete(MailboxSchema.LastSharingPolicyAppliedId);
						mailboxSession.Mailbox.Delete(MailboxSchema.LastSharingPolicyAppliedHash);
						mailboxSession.Mailbox.Delete(MailboxSchema.LastSharingPolicyAppliedTime);
						mailboxSession.Mailbox.Save();
					}
				}
			}
			if (base.IsSetRandomPassword)
			{
				MailboxTaskHelper.SetMailboxPassword((IRecipientSession)base.DataSession, this.DataObject, null, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		protected override bool IsObjectStateChanged()
		{
			return this.RemoveManagedFolderAndPolicy || base.IsObjectStateChanged();
		}

		private MailboxSession OpenMailboxSession(IRecipientSession recipientSession, ADRecipient recipient)
		{
			MailboxSession result;
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromDirectoryObjectId(recipientSession, recipient.Id, RemotingOptions.AllowCrossSite);
				if (exchangePrincipal == null)
				{
					result = null;
				}
				else
				{
					result = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Set-Mailbox");
				}
			}
			catch (StorageTransientException ex)
			{
				base.WriteError(new RecipientTaskException(Strings.ExceptionStorageOther(ex.ErrorCode, ex.Message)), ExchangeErrorCategory.ServerTransient, null);
				result = null;
			}
			catch (StoragePermanentException ex2)
			{
				LocalizedString message;
				if (ex2 is AccessDeniedException)
				{
					message = Strings.ExceptionStorageAccessDenied(ex2.ErrorCode, ex2.Message);
				}
				else
				{
					message = Strings.ExceptionStorageOther(ex2.ErrorCode, ex2.Message);
				}
				base.WriteError(new RecipientTaskException(message), ExchangeErrorCategory.ServerOperation, null);
				result = null;
			}
			return result;
		}

		private void CheckResourceProperties()
		{
			if (this.DataObject.IsChanged(ADRecipientSchema.ResourceSearchProperties) || this.DataObject.IsChanged(ADRecipientSchema.ResourceMetaData) || this.DataObject.IsChanged(ADRecipientSchema.ResourcePropertiesDisplay))
			{
				SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(base.CurrentOrganizationId);
				IConfigurationSession configurationSession = this.ConfigurationSession;
				if (sharedConfiguration != null)
				{
					configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.ConfigurationSession.ConsistencyMode, sharedConfiguration.GetSharedConfigurationSessionSettings(), 4180, "CheckResourceProperties", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\SetMailbox.cs");
				}
				configurationSession.SessionSettings.IsSharedConfigChecked = true;
				configurationSession.SessionSettings.IsRedirectedToSharedConfig = false;
				ResourceBookingConfig resourceBookingConfig = configurationSession.Read<ResourceBookingConfig>(ResourceBookingConfig.GetWellKnownLocation(configurationSession.GetOrgContainerId()));
				List<string> list = new List<string>(this.DataObject.ResourceCustom);
				list.Remove(this.DataObject.ResourceType.Value.ToString());
				foreach (string text in list)
				{
					if (!resourceBookingConfig.IsPropAllowedOnResourceType(this.DataObject.ResourceType.Value.ToString(), text))
					{
						this.WriteError(new RecipientTaskException(Strings.ErrorResourceSearchPropertyInvalid(text)), ExchangeErrorCategory.Client, this.DataObject, false);
					}
				}
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Mailbox.FromDataObject((ADUser)dataObject);
		}

		private static readonly MailboxAuditOperations UnsupportedOwnerOperations = MailboxAuditOperations.Copy | MailboxAuditOperations.SendAs | MailboxAuditOperations.SendOnBehalf | MailboxAuditOperations.MailboxLogin;

		private static readonly MailboxAuditOperations UnsupportedDelegateOperations = MailboxAuditOperations.Copy | MailboxAuditOperations.MessageBind | MailboxAuditOperations.MailboxLogin;

		private static readonly MailboxAuditOperations UnsupportedAdminOperations = MailboxAuditOperations.MailboxLogin;

		private bool needChangeMailboxSubtype;

		private RecipientTypeDetails originalRecipientTypeDetails;

		private RecipientTypeDetails targetRecipientTypeDetails;

		private bool needApplyMandatoryProperties;

		private ADObjectId originalForwardingAddress;

		private ProxyAddress originalForwardingSmtpAddress;

		private static readonly object[] InvalidAuditLogParameters = new object[]
		{
			"Arbitration",
			"PublicFolder"
		};
	}
}
