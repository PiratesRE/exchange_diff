using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SyncMailUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSyncMailUser : SetMailUserBase<MailUserIdParameter, SyncMailUser>
	{
		[Parameter(Mandatory = false)]
		public RecipientIdParameter ForwardingAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields[MailUserSchema.ForwardingAddress];
			}
			set
			{
				base.Fields[MailUserSchema.ForwardingAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UserContactIdParameter Manager
		{
			get
			{
				return (UserContactIdParameter)base.Fields[SyncMailUserSchema.Manager];
			}
			set
			{
				base.Fields[SyncMailUserSchema.Manager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPlanIdParameter IntendedMailboxPlanName
		{
			get
			{
				return (MailboxPlanIdParameter)base.Fields["IntendedMailboxPlan"];
			}
			set
			{
				base.Fields["IntendedMailboxPlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> BypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[MailEnabledRecipientSchema.BypassModerationFrom];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> BypassModerationFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>> RawAcceptMessagesOnlyFrom
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>)base.Fields["RawAcceptMessagesOnlyFrom"];
			}
			set
			{
				base.Fields["RawAcceptMessagesOnlyFrom"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>> RawBypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>)base.Fields["RawBypassModerationFrom"];
			}
			set
			{
				base.Fields["RawBypassModerationFrom"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>> RawRejectMessagesFrom
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>)base.Fields["RawRejectMessagesFrom"];
			}
			set
			{
				base.Fields["RawRejectMessagesFrom"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> RawGrantSendOnBehalfTo
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>>)base.Fields["RawGrantSendOnBehalfTo"];
			}
			set
			{
				base.Fields["RawGrantSendOnBehalfTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<ModeratorIDParameter>> RawModeratedBy
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<ModeratorIDParameter>>)base.Fields["RawModeratedBy"];
			}
			set
			{
				base.Fields["RawModeratedBy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientWithAdUserIdParameter<RecipientIdParameter> RawForwardingAddress
		{
			get
			{
				return (RecipientWithAdUserIdParameter<RecipientIdParameter>)base.Fields["RawForwardingAddress"];
			}
			set
			{
				base.Fields["RawForwardingAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string C
		{
			get
			{
				return ((SyncMailUser)this.GetDynamicParameters()).C;
			}
			set
			{
				((SyncMailUser)this.GetDynamicParameters()).C = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Co
		{
			get
			{
				return ((SyncMailUser)this.GetDynamicParameters()).Co;
			}
			set
			{
				((SyncMailUser)this.GetDynamicParameters()).Co = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DoNotCheckAcceptedDomains
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotCheckAcceptedDomains"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DoNotCheckAcceptedDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection SmtpAndX500Addresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields["SmtpAndX500Addresses"];
			}
			set
			{
				base.Fields["SmtpAndX500Addresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection SipAddresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields["SipAddresses"];
			}
			set
			{
				base.Fields["SipAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)base.Fields["ReleaseTrack"];
			}
			set
			{
				base.Fields["ReleaseTrack"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SoftDeletedMailUser
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedObject"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> RawSiteMailboxOwners
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>>)base.Fields["RawSiteMailboxOwners"];
			}
			set
			{
				base.Fields["RawSiteMailboxOwners"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> RawSiteMailboxUsers
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>>)base.Fields["RawSiteMailboxUsers"];
			}
			set
			{
				base.Fields["RawSiteMailboxUsers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientIdParameter> SiteMailboxOwners
		{
			get
			{
				return (MultiValuedProperty<RecipientIdParameter>)base.Fields[SyncMailUserSchema.SiteMailboxOwners];
			}
			set
			{
				base.Fields[SyncMailUserSchema.SiteMailboxOwners] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientIdParameter> SiteMailboxUsers
		{
			get
			{
				return (MultiValuedProperty<RecipientIdParameter>)base.Fields[SyncMailUserSchema.SiteMailboxUsers];
			}
			set
			{
				base.Fields[SyncMailUserSchema.SiteMailboxUsers] = value;
			}
		}

		internal static MultiValuedProperty<ADObjectId> ResolveSiteMailboxOwnersReferenceParameter(IList<RecipientIdParameter> recipientIdParameters, IRecipientSession recipientSession, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, WriteWarningDelegate writeWarning)
		{
			if (recipientIdParameters == null || recipientIdParameters.Count == 0)
			{
				return null;
			}
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			foreach (RecipientIdParameter recipientIdParameter in recipientIdParameters)
			{
				ADRecipient adrecipient = null;
				try
				{
					adrecipient = (ADRecipient)getDataObject(recipientIdParameter, recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
				}
				catch (ManagementObjectNotFoundException ex)
				{
					writeWarning(new LocalizedString(ex.Message));
					continue;
				}
				catch (ManagementObjectAmbiguousException ex2)
				{
					writeWarning(new LocalizedString(ex2.Message));
					continue;
				}
				if (adrecipient != null && (adrecipient.RecipientType == RecipientType.User || TeamMailboxMembershipHelper.IsUserQualifiedType(adrecipient)))
				{
					multiValuedProperty.Add((ADObjectId)adrecipient.Identity);
				}
			}
			return multiValuedProperty;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.SoftDeletedMailUser.IsPresent)
			{
				recipientSession = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(recipientSession, null);
			}
			return recipientSession;
		}

		internal override IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			sessionSettings.IncludeSoftDeletedObjects = this.SoftDeletedMailUser;
			return base.CreateTenantGlobalCatalogSession(sessionSettings);
		}

		protected override bool ShouldCheckAcceptedDomains()
		{
			return !this.DoNotCheckAcceptedDomains;
		}

		internal override IReferenceErrorReporter ReferenceErrorReporter
		{
			get
			{
				if (this.batchReferenceErrorReporter == null)
				{
					this.batchReferenceErrorReporter = new BatchReferenceErrorReporter();
				}
				return this.batchReferenceErrorReporter;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			MultiLinkSyncHelper.ValidateIncompatibleParameters(base.Fields, this.GetIncompatibleParametersDictionary(), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			SyncMailUser syncMailUser = (SyncMailUser)this.GetDynamicParameters();
			if (syncMailUser.IsModified(SyncMailUserSchema.CountryOrRegion) && (syncMailUser.IsModified(SyncMailUserSchema.C) || syncMailUser.IsModified(SyncMailUserSchema.Co) || syncMailUser.IsModified(SyncMailUserSchema.CountryCode)))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorConflictCountryOrRegion), ExchangeErrorCategory.Client, null);
			}
			if (syncMailUser.IsModified(SyncMailUserSchema.ResourceCustom) && (syncMailUser.IsModified(SyncMailUserSchema.ResourcePropertiesDisplay) || syncMailUser.IsModified(SyncMailUserSchema.ResourceSearchProperties)))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorConflictResourceCustom), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			bool includeSoftDeletedObjects = base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects;
			try
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = this.SoftDeletedMailUser;
				base.ResolveLocalSecondaryIdentities();
				this.InternalResolveLocalSecondaryIdentities();
			}
			finally
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
			}
		}

		private void InternalResolveLocalSecondaryIdentities()
		{
			SyncMailUser syncMailUser = (SyncMailUser)this.GetDynamicParameters();
			base.SetReferenceParameter<UserContactIdParameter>(SyncMailUserSchema.Manager, this.Manager, syncMailUser, new GetRecipientDelegate<UserContactIdParameter>(this.GetRecipient));
			base.SetReferenceParameter<RecipientIdParameter>(MailUserSchema.ForwardingAddress, this.ForwardingAddress, syncMailUser, new GetRecipientDelegate<RecipientIdParameter>(this.GetRecipient));
			base.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(MailEnabledRecipientSchema.BypassModerationFrom, this.BypassModerationFrom, syncMailUser, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual));
			base.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(MailEnabledRecipientSchema.BypassModerationFromDLMembers, this.BypassModerationFromDLMembers, syncMailUser, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionGroup));
			if (base.Fields.IsModified("IntendedMailboxPlan"))
			{
				this.intendedMailboxPlanObject = null;
				if (this.IntendedMailboxPlanName != null)
				{
					this.intendedMailboxPlanObject = base.ProvisioningCache.TryAddAndGetOrganizationDictionaryValue<ADUser, string>(CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId, base.CurrentOrganizationId, this.IntendedMailboxPlanName.RawIdentity, () => (ADUser)base.GetDataObject<ADUser>(this.IntendedMailboxPlanName, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.IntendedMailboxPlanName.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.IntendedMailboxPlanName.ToString())), ExchangeErrorCategory.Client));
				}
			}
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawAcceptMessagesOnlyFrom", MailEnabledRecipientSchema.AcceptMessagesOnlyFrom, this.RawAcceptMessagesOnlyFrom, "RawAcceptMessagesOnlyFrom", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawBypassModerationFrom", MailEnabledRecipientSchema.BypassModerationFrom, this.RawBypassModerationFrom, "RawBypassModerationFrom", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawRejectMessagesFrom", MailEnabledRecipientSchema.RejectMessagesFrom, this.RawRejectMessagesFrom, "RawRejectMessagesFrom", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawGrantSendOnBehalfTo", MailEnabledRecipientSchema.GrantSendOnBehalfTo, this.RawGrantSendOnBehalfTo, "RawGrantSendOnBehalfTo", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateGrantSendOnBehalfTo)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<ModeratorIDParameter>>("RawModeratedBy", MailEnabledRecipientSchema.ModeratedBy, this.RawModeratedBy, "RawModeratedBy", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<ModeratorIDParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(RecipientTaskHelper.ValidateModeratedBy)));
			base.SetReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawForwardingAddress", MailUserSchema.ForwardingAddress, this.RawForwardingAddress, "RawForwardingAddress", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), null);
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawSiteMailboxOwners", SyncMailUserSchema.SiteMailboxOwners, this.RawSiteMailboxOwners, "RawSiteMailboxOwners", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(SetSyncMailUser.ValidateSiteMailboxUsers)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawSiteMailboxUsers", SyncMailUserSchema.SiteMailboxUsers, this.RawSiteMailboxUsers, "RawSiteMailboxUsers", syncMailUser, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(SetSyncMailUser.ValidateSiteMailboxUsers)));
			if (base.Fields.IsModified(SyncMailUserSchema.SiteMailboxOwners))
			{
				MultiValuedProperty<ADObjectId> value = SetSyncMailUser.ResolveSiteMailboxOwnersReferenceParameter(this.SiteMailboxOwners, base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new WriteWarningDelegate(this.WriteWarning));
				syncMailUser[SyncMailUserSchema.SiteMailboxOwners] = value;
			}
			base.SetMultiReferenceParameter<RecipientIdParameter>(SyncMailUserSchema.SiteMailboxUsers, SyncMailUserSchema.SiteMailboxUsers, this.SiteMailboxUsers, "SiteMailboxUsers", syncMailUser, new GetRecipientDelegate<RecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(SetSyncMailUser.ValidateSiteMailboxUsers));
		}

		private static void ValidateSiteMailboxUsers(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError)
		{
			if (!TeamMailboxMembershipHelper.IsUserQualifiedType(recipient))
			{
				writeError(new TaskInvalidOperationException(Strings.ErrorTeamMailboxUserNotResolved(recipient.Identity.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
			}
		}

		private Dictionary<object, ArrayList> GetIncompatibleParametersDictionary()
		{
			return MultiLinkSyncHelper.GetIncompatibleParametersDictionaryForCommonMultiLink();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.intendedMailboxPlanObject != null)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, this.intendedMailboxPlanObject, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			aduser.BypassModerationCheck = true;
			if (base.Fields.IsModified("IntendedMailboxPlan"))
			{
				aduser.IntendedMailboxPlan = ((this.intendedMailboxPlanObject == null) ? null : this.intendedMailboxPlanObject.Id);
			}
			if (this.SmtpAndX500Addresses != null && this.SmtpAndX500Addresses.Count > 0)
			{
				aduser.EmailAddresses = SyncTaskHelper.ReplaceSmtpAndX500Addresses(this.SmtpAndX500Addresses, aduser.EmailAddresses);
			}
			if (base.Fields.IsModified("ReleaseTrack"))
			{
				aduser.ReleaseTrack = this.ReleaseTrack;
			}
			if (base.Fields.IsModified("SipAddresses"))
			{
				aduser.EmailAddresses = SyncTaskHelper.ReplaceSipAddresses(this.SipAddresses, aduser.EmailAddresses);
			}
			if (this.DataObject.IsModified(MailEnabledRecipientSchema.EmailAddresses))
			{
				aduser.EmailAddresses = SyncTaskHelper.FilterDuplicateEmailAddresses(base.TenantGlobalCatalogSession, this.DataObject.EmailAddresses, this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADUser dataObject2 = (ADUser)dataObject;
			return new SyncMailUser(dataObject2);
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADUser aduser = (ADUser)dataObject;
			if (this.Instance.IsModified(SyncMailUserSchema.ElcMailboxFlags) && !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.OverWriteElcMailboxFlags.Enabled)
			{
				ElcMailboxFlags elcMailboxFlags = aduser.ElcMailboxFlags & ElcMailboxFlags.ValidArchiveDatabase;
				this.Instance.ElcMailboxFlags |= elcMailboxFlags;
			}
			base.StampChangesOn(dataObject);
		}

		private BatchReferenceErrorReporter batchReferenceErrorReporter;

		private ADUser intendedMailboxPlanObject;
	}
}
