using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MailboxPlan : Mailbox
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return Microsoft.Exchange.Data.Directory.Management.MailboxPlan.schema;
			}
		}

		public MailboxPlan()
		{
			base.SetObjectClass("user");
		}

		public MailboxPlan(ADUser dataObject) : base(dataObject)
		{
		}

		internal new static MailboxPlan FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new MailboxPlan(dataObject);
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[MailboxPlanSchema.IsDefault];
			}
			internal set
			{
				this[MailboxPlanSchema.IsDefault] = value;
			}
		}

		public bool IsDefaultForPreviousVersion
		{
			get
			{
				return (bool)this[MailboxPlanSchema.IsDefault_R3];
			}
			internal set
			{
				this[MailboxPlanSchema.IsDefault_R3] = value;
			}
		}

		public MailboxPlanRelease MailboxPlanRelease
		{
			get
			{
				return (MailboxPlanRelease)this[MailboxPlanSchema.MailboxPlanRelease];
			}
			internal set
			{
				this[MailboxPlanSchema.MailboxPlanRelease] = value;
			}
		}

		public bool IsPilotMailboxPlan
		{
			get
			{
				return (bool)this[MailboxPlanSchema.IsPilotMailboxPlan];
			}
			internal set
			{
				this[MailboxPlanSchema.IsPilotMailboxPlan] = value;
			}
		}

		internal new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new ADObjectId ArbitrationMailbox
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> BypassModerationFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute10
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute11
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute12
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute13
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute14
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute15
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute2
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute3
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute4
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute5
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute6
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute7
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute8
		{
			get
			{
				return null;
			}
		}

		internal new string CustomAttribute9
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return null;
			}
		}

		internal new ProxyAddressCollection EmailAddresses
		{
			get
			{
				return null;
			}
		}

		internal new ADObjectId ForwardingAddress
		{
			get
			{
				return null;
			}
		}

		internal new ProxyAddress ForwardingSmtpAddress
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return null;
			}
		}

		internal new string LinkedMasterAccount
		{
			get
			{
				return null;
			}
		}

		internal new string MailTip
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return null;
			}
		}

		internal new bool ModerationEnabled
		{
			get
			{
				return false;
			}
		}

		internal new string Office
		{
			get
			{
				return null;
			}
		}

		internal new SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return base.PrimarySmtpAddress;
			}
		}

		internal new MultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<ADObjectId> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return null;
			}
		}

		internal new int? ResourceCapacity
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> ResourceCustom
		{
			get
			{
				return null;
			}
		}

		internal new string SamAccountName
		{
			get
			{
				return null;
			}
		}

		internal new TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return base.SendModerationNotifications;
			}
		}

		internal new string SimpleDisplayName
		{
			get
			{
				return null;
			}
		}

		internal new MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return null;
			}
		}

		internal new SmtpAddress WindowsEmailAddress
		{
			get
			{
				return base.WindowsEmailAddress;
			}
		}

		internal new SmtpAddress WindowsLiveID
		{
			get
			{
				return base.WindowsLiveID;
			}
		}

		internal new SmtpAddress MicrosoftOnlineServicesID
		{
			get
			{
				return base.MicrosoftOnlineServicesID;
			}
		}

		internal new string ImmutableId
		{
			get
			{
				return base.ImmutableId;
			}
		}

		internal new bool? SKUAssigned
		{
			get
			{
				return new bool?(false);
			}
		}

		private new CountryInfo UsageLocation
		{
			get
			{
				return base.UsageLocation;
			}
		}

		public string MailboxPlanIndex
		{
			get
			{
				return (string)this[MailboxPlanSchema.MailboxPlanIndex];
			}
			internal set
			{
				this[MailboxPlanSchema.MailboxPlanIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.HiddenFromAddressListsValue];
			}
			set
			{
				this[MailEnabledRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}

		internal static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 8UL));
		}

		internal static QueryFilter IsDefault_R3_FilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 4UL));
		}

		internal static object GetMailboxPlanRelease(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ADRecipientSchema.ProvisioningFlags];
			return (MailboxPlanRelease)(num & 48);
		}

		internal static void SetMailboxPlanRelease(object value, IPropertyBag propertyBag)
		{
			ProvisioningFlagValues provisioningFlagValues = (ProvisioningFlagValues)value;
			int num = (int)propertyBag[ADRecipientSchema.ProvisioningFlags];
			num &= -49;
			propertyBag[ADRecipientSchema.ProvisioningFlags] = (num | (int)provisioningFlagValues);
		}

		internal static QueryFilter MailboxPlanReleaseFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			if (!(comparisonFilter.PropertyValue is MailboxPlanRelease))
			{
				throw new ArgumentException("filter.PropertyValue");
			}
			MailboxPlanRelease mailboxPlanRelease = (MailboxPlanRelease)comparisonFilter.PropertyValue;
			if (mailboxPlanRelease != MailboxPlanRelease.AllReleases)
			{
				if (mailboxPlanRelease != MailboxPlanRelease.CurrentRelease)
				{
					if (mailboxPlanRelease != MailboxPlanRelease.NonCurrentRelease)
					{
						throw new ArgumentException("filter.PropertyValue");
					}
					if (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
					{
						return Microsoft.Exchange.Data.Directory.Management.MailboxPlan.NonCurrentReleaseFilter;
					}
					return new NotFilter(Microsoft.Exchange.Data.Directory.Management.MailboxPlan.NonCurrentReleaseFilter);
				}
				else
				{
					if (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
					{
						return Microsoft.Exchange.Data.Directory.Management.MailboxPlan.CurrentReleaseFilter;
					}
					return new NotFilter(Microsoft.Exchange.Data.Directory.Management.MailboxPlan.CurrentReleaseFilter);
				}
			}
			else
			{
				QueryFilter queryFilter = new OrFilter(new QueryFilter[]
				{
					Microsoft.Exchange.Data.Directory.Management.MailboxPlan.CurrentReleaseFilter,
					Microsoft.Exchange.Data.Directory.Management.MailboxPlan.NonCurrentReleaseFilter
				});
				if (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator)
				{
					return queryFilter;
				}
				return new NotFilter(queryFilter);
			}
		}

		private static MailboxPlanSchema schema = ObjectSchema.GetInstance<MailboxPlanSchema>();

		internal static readonly BitMaskAndFilter CurrentReleaseFilter = new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 16UL);

		internal static readonly BitMaskAndFilter NonCurrentReleaseFilter = new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 32UL);
	}
}
