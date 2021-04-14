using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SyncDistributionGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSyncDistributionGroup : SetDistributionGroupBase<SyncDistributionGroup>
	{
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
		public MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>> RawMembers
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>)base.Fields["RawMembers"];
			}
			set
			{
				base.Fields["RawMembers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientWithAdUserIdParameter<RecipientIdParameter> RawManagedBy
		{
			get
			{
				return (RecipientWithAdUserIdParameter<RecipientIdParameter>)base.Fields[ADGroupSchema.RawManagedBy];
			}
			set
			{
				base.Fields[ADGroupSchema.RawManagedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> RawCoManagedBy
		{
			get
			{
				return (MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>>)base.Fields["RawCoManagedBy"];
			}
			set
			{
				base.Fields["RawCoManagedBy"] = value;
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

		[Parameter]
		public GroupType Type
		{
			get
			{
				return (GroupType)(base.Fields[ADGroupSchema.GroupType] ?? GroupType.Distribution);
			}
			set
			{
				base.Fields[ADGroupSchema.GroupType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedObjects
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

		protected override bool ShouldCheckAcceptedDomains()
		{
			return !this.DoNotCheckAcceptedDomains;
		}

		internal override ADRecipient GetRecipient(RecipientIdParameter recipientIdParameter, Task.ErrorLoggerDelegate writeError)
		{
			bool includeSoftDeletedObjects = base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects;
			if (this.IncludeSoftDeletedObjects.IsPresent)
			{
				base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = true;
			}
			ADRecipient recipient = base.GetRecipient(recipientIdParameter, writeError);
			base.TenantGlobalCatalogSession.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
			return recipient;
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
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			SyncDistributionGroup dataObject = (SyncDistributionGroup)this.GetDynamicParameters();
			base.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(MailEnabledRecipientSchema.BypassModerationFrom, this.BypassModerationFrom, dataObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual));
			base.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(MailEnabledRecipientSchema.BypassModerationFromDLMembers, this.BypassModerationFromDLMembers, dataObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionGroup));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawAcceptMessagesOnlyFrom", MailEnabledRecipientSchema.AcceptMessagesOnlyFrom, this.RawAcceptMessagesOnlyFrom, "RawAcceptMessagesOnlyFrom", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawBypassModerationFrom", MailEnabledRecipientSchema.BypassModerationFrom, this.RawBypassModerationFrom, "RawBypassModerationFrom", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>("RawRejectMessagesFrom", MailEnabledRecipientSchema.RejectMessagesFrom, this.RawRejectMessagesFrom, "RawRejectMessagesFrom", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<DeliveryRecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateMessageDeliveryRestrictionIndividual)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawGrantSendOnBehalfTo", MailEnabledRecipientSchema.GrantSendOnBehalfTo, this.RawGrantSendOnBehalfTo, "RawGrantSendOnBehalfTo", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(base.ValidateGrantSendOnBehalfTo)));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<ModeratorIDParameter>>("RawModeratedBy", MailEnabledRecipientSchema.ModeratedBy, this.RawModeratedBy, "RawModeratedBy", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<ModeratorIDParameter>>(this.GetRecipient), SyncTaskHelper.ValidateBypassADUser(new ValidateRecipientDelegate(RecipientTaskHelper.ValidateModeratedBy)));
			base.SetReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>(ADGroupSchema.RawManagedBy, this.RawManagedBy, dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient));
			base.SetMultiReferenceParameter<RecipientWithAdUserIdParameter<RecipientIdParameter>>("RawCoManagedBy", SyncDistributionGroupSchema.CoManagedBy, this.RawCoManagedBy, "RawCoManagedBy", dataObject, new GetRecipientDelegate<RecipientWithAdUserIdParameter<RecipientIdParameter>>(this.GetRecipient), null);
			base.SetMultiReferenceParameter<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>("RawMembers", SyncDistributionGroupSchema.Members, this.RawMembers, "RawMembers", dataObject, new GetRecipientDelegate<RecipientWithAdUserGroupIdParameter<RecipientIdParameter>>(this.GetRecipient), null);
		}

		private Dictionary<object, ArrayList> GetIncompatibleParametersDictionary()
		{
			Dictionary<object, ArrayList> incompatibleParametersDictionaryForCommonMultiLink = MultiLinkSyncHelper.GetIncompatibleParametersDictionaryForCommonMultiLink();
			incompatibleParametersDictionaryForCommonMultiLink[DistributionGroupSchema.ManagedBy] = new ArrayList(new object[]
			{
				ADGroupSchema.RawManagedBy,
				"RawCoManagedBy",
				"RawCoManagedBy"
			});
			return incompatibleParametersDictionaryForCommonMultiLink;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			base.ValidateReferenceParameter(SyncDistributionGroupSchema.RawManagedBy, SyncTaskHelper.ValidateWithBaseObjectBypassADUser<ADGroup>(new ValidateRecipientWithBaseObjectDelegate<ADGroup>(MailboxTaskHelper.ValidateGroupManagedBy)));
			base.ValidateMultiReferenceParameter("RawCoManagedBy", SyncTaskHelper.ValidateWithBaseObjectBypassADUser<ADGroup>(new ValidateRecipientWithBaseObjectDelegate<ADGroup>(MailboxTaskHelper.ValidateGroupManagedBy)));
			base.ValidateMultiReferenceParameter("RawMembers", SyncTaskHelper.ValidateWithBaseObjectBypassADUser<ADGroup>(new ValidateRecipientWithBaseObjectDelegate<ADGroup>(MailboxTaskHelper.ValidateGroupMember)));
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			adgroup.BypassModerationCheck = true;
			if (this.SmtpAndX500Addresses != null && this.SmtpAndX500Addresses.Count > 0)
			{
				adgroup.EmailAddresses = SyncTaskHelper.ReplaceSmtpAndX500Addresses(this.SmtpAndX500Addresses, adgroup.EmailAddresses);
			}
			if (base.Fields.IsModified("SipAddresses"))
			{
				adgroup.EmailAddresses = SyncTaskHelper.ReplaceSipAddresses(this.SipAddresses, adgroup.EmailAddresses);
			}
			if (adgroup.IsModified(MailEnabledRecipientSchema.EmailAddresses))
			{
				adgroup.EmailAddresses = SyncTaskHelper.FilterDuplicateEmailAddresses(base.TenantGlobalCatalogSession, adgroup.EmailAddresses, adgroup, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			if (base.Fields.IsModified(ADGroupSchema.GroupType))
			{
				if (this.Type != GroupType.Distribution && this.Type != GroupType.Security)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorGroupTypeInvalid), ExchangeErrorCategory.Client, null);
				}
				bool flag = (adgroup.GroupType & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled;
				if (this.Type == GroupType.Distribution && flag)
				{
					adgroup.GroupType &= (GroupTypeFlags)2147483647;
					if ((adgroup.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.Universal)
					{
						adgroup.RecipientTypeDetails = RecipientTypeDetails.MailUniversalDistributionGroup;
					}
					else
					{
						adgroup.RecipientTypeDetails = RecipientTypeDetails.MailNonUniversalGroup;
					}
					base.DesiredRecipientType = adgroup.RecipientType;
					if (!adgroup.IsChanged(ADRecipientSchema.RecipientDisplayType))
					{
						adgroup.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DistributionGroup);
					}
				}
				else if (this.Type == GroupType.Security && !flag)
				{
					if (adgroup.RecipientTypeDetails == RecipientTypeDetails.RoomList)
					{
						base.WriteError(new TaskInvalidOperationException(Strings.ErrorConvertNonUniversalDistributionGroup(adgroup.Identity.ToString())), ExchangeErrorCategory.Client, adgroup.Identity);
					}
					adgroup.GroupType |= GroupTypeFlags.SecurityEnabled;
					if ((adgroup.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.Universal)
					{
						adgroup.RecipientTypeDetails = RecipientTypeDetails.MailUniversalSecurityGroup;
					}
					else
					{
						adgroup.RecipientTypeDetails = RecipientTypeDetails.MailNonUniversalGroup;
					}
					base.DesiredRecipientType = adgroup.RecipientType;
					if (!adgroup.IsChanged(ADRecipientSchema.RecipientDisplayType))
					{
						adgroup.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.SecurityDistributionGroup);
					}
				}
			}
			TaskLogger.LogExit();
			return adgroup;
		}

		protected override void UpdateRecipientDisplayType(ADGroup group)
		{
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncDistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		private BatchReferenceErrorReporter batchReferenceErrorReporter;
	}
}
