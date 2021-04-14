using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "SyncDistributionGroup", SupportsShouldProcess = true)]
	public sealed class NewSyncDistributionGroup : NewDistributionGroupBase
	{
		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] AcceptMessagesOnlyFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return this.DataObject.BlockedSendersHash;
			}
			set
			{
				this.DataObject.BlockedSendersHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] BypassModerationFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[MailEnabledRecipientSchema.BypassModerationFrom];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] BypassModerationFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				base.Fields[MailEnabledRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return this.DataObject.DirSyncId;
			}
			set
			{
				this.DataObject.DirSyncId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute1
		{
			get
			{
				return this.DataObject.CustomAttribute1;
			}
			set
			{
				this.DataObject.CustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute10
		{
			get
			{
				return this.DataObject.CustomAttribute10;
			}
			set
			{
				this.DataObject.CustomAttribute10 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute11
		{
			get
			{
				return this.DataObject.CustomAttribute11;
			}
			set
			{
				this.DataObject.CustomAttribute11 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute12
		{
			get
			{
				return this.DataObject.CustomAttribute12;
			}
			set
			{
				this.DataObject.CustomAttribute12 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute13
		{
			get
			{
				return this.DataObject.CustomAttribute13;
			}
			set
			{
				this.DataObject.CustomAttribute13 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute14
		{
			get
			{
				return this.DataObject.CustomAttribute14;
			}
			set
			{
				this.DataObject.CustomAttribute14 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute15
		{
			get
			{
				return this.DataObject.CustomAttribute15;
			}
			set
			{
				this.DataObject.CustomAttribute15 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute2
		{
			get
			{
				return this.DataObject.CustomAttribute2;
			}
			set
			{
				this.DataObject.CustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute3
		{
			get
			{
				return this.DataObject.CustomAttribute3;
			}
			set
			{
				this.DataObject.CustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute4
		{
			get
			{
				return this.DataObject.CustomAttribute4;
			}
			set
			{
				this.DataObject.CustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute5
		{
			get
			{
				return this.DataObject.CustomAttribute5;
			}
			set
			{
				this.DataObject.CustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute6
		{
			get
			{
				return this.DataObject.CustomAttribute6;
			}
			set
			{
				this.DataObject.CustomAttribute6 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute7
		{
			get
			{
				return this.DataObject.CustomAttribute7;
			}
			set
			{
				this.DataObject.CustomAttribute7 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute8
		{
			get
			{
				return this.DataObject.CustomAttribute8;
			}
			set
			{
				this.DataObject.CustomAttribute8 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CustomAttribute9
		{
			get
			{
				return this.DataObject.CustomAttribute9;
			}
			set
			{
				this.DataObject.CustomAttribute9 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute1;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute2;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute3;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute4;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return this.DataObject.ExtensionCustomAttribute5;
			}
			set
			{
				this.DataObject.ExtensionCustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return this.DataObject.EmailAddresses;
			}
			set
			{
				this.DataObject.EmailAddresses = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] GrantSendOnBehalfTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[ADRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				base.Fields[ADRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return this.DataObject.HiddenFromAddressListsEnabled;
			}
			set
			{
				this.DataObject.HiddenFromAddressListsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return base.MailTipTranslations;
			}
			set
			{
				base.MailTipTranslations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return this.DataObject.RecipientDisplayType;
			}
			set
			{
				this.DataObject.RecipientDisplayType = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return this.DataObject.SafeRecipientsHash;
			}
			set
			{
				this.DataObject.SafeRecipientsHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return this.DataObject.SafeSendersHash;
			}
			set
			{
				this.DataObject.SafeSendersHash = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] RejectMessagesFrom
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeliveryRecipientIdParameter[] RejectMessagesFromDLMembers
		{
			get
			{
				return (DeliveryRecipientIdParameter[])base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)(base.Fields["RequireAllSendersAreAuthenticated"] ?? false);
			}
			set
			{
				base.Fields["RequireAllSendersAreAuthenticated"] = value;
			}
		}

		[Parameter]
		public bool ReportToManagerEnabled
		{
			get
			{
				return this.DataObject.ReportToManagerEnabled;
			}
			set
			{
				this.DataObject.ReportToManagerEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReportToOriginatorEnabled
		{
			get
			{
				return (bool)(base.Fields["ReportToOriginatorEnabled"] ?? false);
			}
			set
			{
				base.Fields["ReportToOriginatorEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SendOofMessageToOriginatorEnabled
		{
			get
			{
				return this.DataObject.SendOofMessageToOriginatorEnabled;
			}
			set
			{
				this.DataObject.SendOofMessageToOriginatorEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return this.DataObject.HABSeniorityIndex;
			}
			set
			{
				this.DataObject.HABSeniorityIndex = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return this.DataObject.PhoneticDisplayName;
			}
			set
			{
				this.DataObject.PhoneticDisplayName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHierarchicalGroup
		{
			get
			{
				return this.DataObject.IsOrganizationalGroup;
			}
			set
			{
				this.DataObject.IsOrganizationalGroup = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return this.DataObject.OnPremisesObjectId;
			}
			set
			{
				this.DataObject.OnPremisesObjectId = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return this.DataObject.IsDirSynced;
			}
			set
			{
				this.DataObject.IsDirSynced = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return this.DataObject.DirSyncAuthorityMetadata;
			}
			set
			{
				this.DataObject.DirSyncAuthorityMetadata = value;
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
		public string ValidationOrganization
		{
			get
			{
				return (string)base.Fields["ValidationOrganization"];
			}
			set
			{
				base.Fields["ValidationOrganization"] = value;
			}
		}

		protected override bool ShouldCheckAcceptedDomains()
		{
			return !this.DoNotCheckAcceptedDomains;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if ((this.SmtpAndX500Addresses != null && this.SmtpAndX500Addresses.Count > 0) || (this.SipAddresses != null && this.SipAddresses.Count > 0))
			{
				this.DataObject.EmailAddresses = SyncTaskHelper.MergeAddresses(this.SmtpAndX500Addresses, this.SipAddresses);
			}
			base.InternalBeginProcessing();
			if (this.ValidationOrganization != null && !string.Equals(this.ValidationOrganization, base.CurrentOrganizationId.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowTerminatingError(new ValidationOrgCurrentOrgNotMatchException(this.ValidationOrganization, base.CurrentOrganizationId.ToString()), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.IsModified(ADGroupSchema.ManagedBy) && base.ManagedBy != null && base.ManagedBy.Count != 0)
			{
				this.managedByRecipients = new MultiValuedProperty<ADRecipient>();
				foreach (RecipientIdParameter recipientIdParameter in base.ManagedBy)
				{
					ADRecipient item = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
					this.managedByRecipients.Add(item);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.GrantSendOnBehalfTo) && this.GrantSendOnBehalfTo != null && this.GrantSendOnBehalfTo.Length != 0)
			{
				this.grantSendOnBehalfTo = new MultiValuedProperty<ADRecipient>();
				foreach (RecipientIdParameter recipientIdParameter2 in this.GrantSendOnBehalfTo)
				{
					ADRecipient item2 = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
					this.grantSendOnBehalfTo.Add(item2);
				}
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom) && this.BypassModerationFrom != null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty;
				MultiValuedProperty<ADObjectId> multiValuedProperty2;
				this.bypassModerationFromRecipient = SetMailEnabledRecipientObjectTask<DistributionGroupIdParameter, SyncDistributionGroup, ADGroup>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFrom, "BypassModerationFrom", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty, out multiValuedProperty2);
				if (multiValuedProperty != null && multiValuedProperty.Count > 0)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorIndividualRecipientNeeded(multiValuedProperty[0].ToString())), ExchangeErrorCategory.Client, null);
				}
				this.bypassModerationFrom = multiValuedProperty2;
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromDLMembers) && this.BypassModerationFromDLMembers != null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty3;
				MultiValuedProperty<ADObjectId> multiValuedProperty4;
				this.bypassModerationFromDLMembersRecipient = SetMailEnabledRecipientObjectTask<DistributionGroupIdParameter, SyncDistributionGroup, ADGroup>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFromDLMembers, "BypassModerationFromDLMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty3, out multiValuedProperty4);
				if (multiValuedProperty4 != null && multiValuedProperty4.Count > 0)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorGroupRecipientNeeded(multiValuedProperty4[0].ToString())), ExchangeErrorCategory.Client, null);
				}
				this.bypassModerationFromDLMembers = multiValuedProperty3;
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom) && this.AcceptMessagesOnlyFrom != null && this.AcceptMessagesOnlyFrom.Length != 0)
			{
				this.acceptMessagesOnlyFrom = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter in this.AcceptMessagesOnlyFrom)
				{
					ADRecipient item3 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter.ToString())), ExchangeErrorCategory.Client);
					this.acceptMessagesOnlyFrom.Add(item3);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers) && this.AcceptMessagesOnlyFromDLMembers != null && this.AcceptMessagesOnlyFromDLMembers.Length != 0)
			{
				this.acceptMessagesOnlyFromDLMembers = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter2 in this.AcceptMessagesOnlyFromDLMembers)
				{
					ADRecipient item4 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter2, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter2.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter2.ToString())), ExchangeErrorCategory.Client);
					this.acceptMessagesOnlyFromDLMembers.Add(item4);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFrom) && this.RejectMessagesFrom != null && this.RejectMessagesFrom.Length != 0)
			{
				this.rejectMessagesFrom = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter3 in this.RejectMessagesFrom)
				{
					ADRecipient item5 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter3, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter3.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter3.ToString())), ExchangeErrorCategory.Client);
					this.rejectMessagesFrom.Add(item5);
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromDLMembers) && this.RejectMessagesFromDLMembers != null && this.RejectMessagesFromDLMembers.Length != 0)
			{
				this.rejectMessagesFromDLMembers = new MultiValuedProperty<ADRecipient>();
				foreach (DeliveryRecipientIdParameter deliveryRecipientIdParameter4 in this.RejectMessagesFromDLMembers)
				{
					ADRecipient item6 = (ADRecipient)base.GetDataObject<ADRecipient>(deliveryRecipientIdParameter4, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(deliveryRecipientIdParameter4.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(deliveryRecipientIdParameter4.ToString())), ExchangeErrorCategory.Client);
					this.rejectMessagesFromDLMembers.Add(item6);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.grantSendOnBehalfTo != null)
			{
				foreach (ADRecipient recipient in this.grantSendOnBehalfTo)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.bypassModerationFromRecipient != null)
			{
				foreach (ADRecipient recipient2 in this.bypassModerationFromRecipient)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient2, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.bypassModerationFromDLMembersRecipient != null)
			{
				foreach (ADRecipient recipient3 in this.bypassModerationFromDLMembersRecipient)
				{
					RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, recipient3, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(ADGroup group)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(group);
			if (base.ManagedBy == null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				RoleGroupIdParameter roleGroupIdParameter = new RoleGroupIdParameter("Organization Management");
				ADGroup adgroup = (ADGroup)base.GetDataObject<ADGroup>(roleGroupIdParameter, base.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(roleGroupIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(roleGroupIdParameter.ToString())), ExchangeErrorCategory.Client);
				multiValuedProperty.Add(adgroup.Id);
				group.ManagedBy = multiValuedProperty;
			}
			group.BypassModerationCheck = true;
			if (base.Fields.IsModified(ADRecipientSchema.GrantSendOnBehalfTo) && this.grantSendOnBehalfTo != null)
			{
				foreach (ADRecipient adrecipient in this.grantSendOnBehalfTo)
				{
					group.GrantSendOnBehalfTo.Add(adrecipient.Identity as ADObjectId);
				}
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFrom))
			{
				group.BypassModerationFrom = this.bypassModerationFrom;
			}
			if (base.Fields.IsModified(MailEnabledRecipientSchema.BypassModerationFromDLMembers))
			{
				group.BypassModerationFromDLMembers = this.bypassModerationFromDLMembers;
			}
			if (this.DataObject != null && this.DataObject.IsModified(ADRecipientSchema.EmailAddresses))
			{
				group.EmailAddresses = SyncTaskHelper.FilterDuplicateEmailAddresses(base.TenantGlobalCatalogSession, this.DataObject.EmailAddresses, this.DataObject, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFrom))
			{
				group.AcceptMessagesOnlyFrom = (from c in this.acceptMessagesOnlyFrom
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers))
			{
				group.AcceptMessagesOnlyFromDLMembers = (from c in this.acceptMessagesOnlyFromDLMembers
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFrom))
			{
				group.RejectMessagesFrom = (from c in this.rejectMessagesFrom
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromDLMembers))
			{
				group.RejectMessagesFromDLMembers = (from c in this.rejectMessagesFromDLMembers
				select c.Identity as ADObjectId).ToArray<ADObjectId>();
			}
			group.ReportToOriginatorEnabled = this.ReportToOriginatorEnabled;
			group.RequireAllSendersAreAuthenticated = this.RequireSenderAuthenticationEnabled;
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			SyncDistributionGroup result2 = new SyncDistributionGroup((ADGroup)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(SyncDistributionGroup).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncDistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		private MultiValuedProperty<ADRecipient> grantSendOnBehalfTo;

		private MultiValuedProperty<ADObjectId> bypassModerationFrom;

		private MultiValuedProperty<ADRecipient> bypassModerationFromRecipient;

		private MultiValuedProperty<ADObjectId> bypassModerationFromDLMembers;

		private MultiValuedProperty<ADRecipient> bypassModerationFromDLMembersRecipient;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFrom;

		private MultiValuedProperty<ADRecipient> acceptMessagesOnlyFromDLMembers;

		private MultiValuedProperty<ADRecipient> rejectMessagesFrom;

		private MultiValuedProperty<ADRecipient> rejectMessagesFromDLMembers;
	}
}
