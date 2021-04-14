using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class Organization : ADLegacyVersionableObject
	{
		public static int OrgConfigurationVersion
		{
			get
			{
				return OrganizationSchema.OrgConfigurationVersion;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return Organization.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Organization.MostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return Organization.CurrentExchangeRootOrgVersion;
			}
		}

		public bool AllowDeleteOfExternalIdentityUponRemove
		{
			get
			{
				return (bool)this[OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove];
			}
			set
			{
				this[OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove] = value;
			}
		}

		public bool HostingDeploymentEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.HostingDeploymentEnabled];
			}
			internal set
			{
				this[OrganizationSchema.HostingDeploymentEnabled] = value;
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[OrganizationSchema.LegacyExchangeDN];
			}
			internal set
			{
				this[OrganizationSchema.LegacyExchangeDN] = value;
			}
		}

		public HeuristicsFlags Heuristics
		{
			get
			{
				return (HeuristicsFlags)this[OrganizationSchema.Heuristics];
			}
			internal set
			{
				this[OrganizationSchema.Heuristics] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ResourceAddressLists
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrganizationSchema.ResourceAddressLists];
			}
			internal set
			{
				this[OrganizationSchema.ResourceAddressLists] = value;
			}
		}

		public bool IsMixedMode
		{
			get
			{
				return (bool)this[OrganizationSchema.IsMixedMode];
			}
		}

		public bool IsAddressListPagingEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.IsAddressListPagingEnabled];
			}
			internal set
			{
				this[OrganizationSchema.IsAddressListPagingEnabled] = value;
			}
		}

		public virtual string BuildNumber
		{
			get
			{
				return (string)this[OrganizationSchema.BuildNumber];
			}
			set
			{
				this[OrganizationSchema.BuildNumber] = value;
			}
		}

		public virtual string ManagedFolderHomepage
		{
			get
			{
				return (string)this[OrganizationSchema.ManagedFolderHomepage];
			}
			set
			{
				this[OrganizationSchema.ManagedFolderHomepage] = value;
			}
		}

		public virtual EnhancedTimeSpan? DefaultPublicFolderAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationSchema.DefaultPublicFolderAgeLimit];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderAgeLimit] = value;
			}
		}

		public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderIssueWarningQuota];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderIssueWarningQuota] = value;
			}
		}

		public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderProhibitPostQuota];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderProhibitPostQuota] = value;
			}
		}

		public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderMaxItemSize];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderMaxItemSize] = value;
			}
		}

		public virtual EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationSchema.DefaultPublicFolderDeletedItemRetention];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderDeletedItemRetention] = value;
			}
		}

		public virtual EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationSchema.DefaultPublicFolderMovedItemRetention];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderMovedItemRetention] = value;
			}
		}

		public virtual string ServiceEndpoints
		{
			get
			{
				return (string)this[OrganizationSchema.ServiceEndpoints];
			}
			set
			{
				this[OrganizationSchema.ServiceEndpoints] = value;
			}
		}

		public MultiValuedProperty<string> ForeignForestFQDN
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.ForeignForestFQDN];
			}
			internal set
			{
				this[OrganizationSchema.ForeignForestFQDN] = value;
			}
		}

		public SecurityIdentifier ForeignForestOrgAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationSchema.ForeignForestOrgAdminUSGSid];
			}
			internal set
			{
				this[OrganizationSchema.ForeignForestOrgAdminUSGSid] = value;
			}
		}

		public SecurityIdentifier ForeignForestRecipientAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationSchema.ForeignForestRecipientAdminUSGSid];
			}
			internal set
			{
				this[OrganizationSchema.ForeignForestRecipientAdminUSGSid] = value;
			}
		}

		public SecurityIdentifier ForeignForestViewOnlyAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationSchema.ForeignForestViewOnlyAdminUSGSid];
			}
			internal set
			{
				this[OrganizationSchema.ForeignForestViewOnlyAdminUSGSid] = value;
			}
		}

		public int ObjectVersion
		{
			get
			{
				return (int)this.propertyBag[OrganizationSchema.ObjectVersion];
			}
			internal set
			{
				this.propertyBag[OrganizationSchema.ObjectVersion] = value;
			}
		}

		public int SCLJunkThreshold
		{
			get
			{
				return (int)this[OrganizationSchema.SCLJunkThreshold];
			}
			set
			{
				this[OrganizationSchema.SCLJunkThreshold] = value;
			}
		}

		public MultiValuedProperty<string> AcceptedDomainNames
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.AcceptedDomainNames];
			}
			set
			{
				this[OrganizationSchema.AcceptedDomainNames] = value;
			}
		}

		public MultiValuedProperty<string> MimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.MimeTypes];
			}
			internal set
			{
				this[OrganizationSchema.MimeTypes] = value;
			}
		}

		public virtual ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[OrganizationSchema.MicrosoftExchangeRecipientEmailAddresses];
			}
			set
			{
				this[OrganizationSchema.MicrosoftExchangeRecipientEmailAddresses] = value;
			}
		}

		public virtual ADObjectId MicrosoftExchangeRecipientReplyRecipient
		{
			get
			{
				return (ADObjectId)this[OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient];
			}
			set
			{
				this[OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient] = value;
			}
		}

		public int? MaxAddressBookPolicies
		{
			get
			{
				return (int?)this[OrganizationSchema.MaxAddressBookPolicies];
			}
			set
			{
				this[OrganizationSchema.MaxAddressBookPolicies] = value;
			}
		}

		public int? MaxOfflineAddressBooks
		{
			get
			{
				return (int?)this[OrganizationSchema.MaxOfflineAddressBooks];
			}
			set
			{
				this[OrganizationSchema.MaxOfflineAddressBooks] = value;
			}
		}

		public virtual SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress];
			}
			set
			{
				this[OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress] = value;
			}
		}

		public virtual bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled];
			}
			set
			{
				this[OrganizationSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled] = value;
			}
		}

		public virtual IndustryType Industry
		{
			get
			{
				return (IndustryType)this[OrganizationSchema.Industry];
			}
			set
			{
				this[OrganizationSchema.Industry] = value;
			}
		}

		public virtual bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)this[OrganizationSchema.CustomerFeedbackEnabled];
			}
			set
			{
				this[OrganizationSchema.CustomerFeedbackEnabled] = value;
			}
		}

		public virtual MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
		{
			get
			{
				return (MultiValuedProperty<OrganizationSummaryEntry>)this[OrganizationSchema.OrganizationSummary];
			}
			set
			{
				this[OrganizationSchema.OrganizationSummary] = value;
			}
		}

		public virtual bool MailTipsExternalRecipientsTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MailTipsExternalRecipientsTipsEnabled];
			}
			set
			{
				this[OrganizationSchema.MailTipsExternalRecipientsTipsEnabled] = value;
			}
		}

		public virtual uint MailTipsLargeAudienceThreshold
		{
			get
			{
				return (uint)((long)this[OrganizationSchema.MailTipsLargeAudienceThreshold]);
			}
			set
			{
				this[OrganizationSchema.MailTipsLargeAudienceThreshold] = (long)((ulong)value);
			}
		}

		public virtual bool MailTipsMailboxSourcedTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MailTipsMailboxSourcedTipsEnabled];
			}
			set
			{
				this[OrganizationSchema.MailTipsMailboxSourcedTipsEnabled] = value;
			}
		}

		public virtual bool MailTipsGroupMetricsEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MailTipsGroupMetricsEnabled];
			}
			set
			{
				this[OrganizationSchema.MailTipsGroupMetricsEnabled] = value;
			}
		}

		public virtual bool MailTipsAllTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MailTipsAllTipsEnabled];
			}
			set
			{
				this[OrganizationSchema.MailTipsAllTipsEnabled] = value;
			}
		}

		public virtual bool IsProcessEhaMigratedMessagesEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.IsProcessEhaMigratedMessagesEnabled];
			}
			set
			{
				this[OrganizationSchema.IsProcessEhaMigratedMessagesEnabled] = value;
			}
		}

		public virtual bool ReadTrackingEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.ReadTrackingEnabled];
			}
			set
			{
				this[OrganizationSchema.ReadTrackingEnabled] = value;
			}
		}

		public ADObjectId DistributionGroupDefaultOU
		{
			get
			{
				return (ADObjectId)this[OrganizationSchema.DistributionGroupDefaultOU];
			}
			set
			{
				this[OrganizationSchema.DistributionGroupDefaultOU] = value;
			}
		}

		public virtual MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.DistributionGroupNameBlockedWordsList];
			}
			set
			{
				this[OrganizationSchema.DistributionGroupNameBlockedWordsList] = value;
			}
		}

		public virtual DistributionGroupNamingPolicy DistributionGroupNamingPolicy
		{
			get
			{
				return (DistributionGroupNamingPolicy)this[OrganizationSchema.DistributionGroupNamingPolicy];
			}
			set
			{
				this[OrganizationSchema.DistributionGroupNamingPolicy] = value;
			}
		}

		public virtual bool IsExcludedFromOnboardMigration
		{
			get
			{
				return (bool)this[OrganizationSchema.IsExcludedFromOnboardMigration];
			}
			set
			{
				this[OrganizationSchema.IsExcludedFromOnboardMigration] = value;
			}
		}

		public virtual bool IsExcludedFromOffboardMigration
		{
			get
			{
				return (bool)this[OrganizationSchema.IsExcludedFromOffboardMigration];
			}
			set
			{
				this[OrganizationSchema.IsExcludedFromOffboardMigration] = value;
			}
		}

		public virtual bool IsFfoMigrationInProgress
		{
			get
			{
				return (bool)this[OrganizationSchema.IsFfoMigrationInProgress];
			}
			set
			{
				this[OrganizationSchema.IsFfoMigrationInProgress] = value;
			}
		}

		public virtual bool TenantRelocationsAllowed
		{
			get
			{
				return (bool)this[OrganizationSchema.TenantRelocationsAllowed];
			}
			set
			{
				this[OrganizationSchema.TenantRelocationsAllowed] = value;
			}
		}

		public virtual bool ACLableSyncedObjectEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.ACLableSyncedObjectEnabled];
			}
			set
			{
				this[OrganizationSchema.ACLableSyncedObjectEnabled] = value;
			}
		}

		public bool OpenTenantFull
		{
			get
			{
				return (bool)this[OrganizationSchema.OpenTenantFull];
			}
			set
			{
				this[OrganizationSchema.OpenTenantFull] = value;
			}
		}

		public virtual bool MapiHttpEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MapiHttpEnabled];
			}
			set
			{
				this[OrganizationSchema.MapiHttpEnabled] = value;
			}
		}

		public virtual bool OAuth2ClientProfileEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.OAuth2ClientProfileEnabled];
			}
			set
			{
				this[OrganizationSchema.OAuth2ClientProfileEnabled] = value;
			}
		}

		public virtual bool IntuneManagedStatus
		{
			get
			{
				return (bool)this[OrganizationSchema.IntuneManagedStatus];
			}
			set
			{
				this[OrganizationSchema.IntuneManagedStatus] = value;
			}
		}

		public virtual Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)(this[OrganizationSchema.MaxConcurrentMigrations] ?? Unlimited<int>.UnlimitedValue);
			}
			set
			{
				this[OrganizationSchema.MaxConcurrentMigrations] = value;
			}
		}

		public virtual string WACDiscoveryEndpoint
		{
			get
			{
				return (string)this[OrganizationSchema.WACDiscoveryEndpoint];
			}
			set
			{
				this[OrganizationSchema.WACDiscoveryEndpoint] = value;
			}
		}

		public virtual bool ForwardSyncLiveIdBusinessInstance
		{
			get
			{
				return (bool)this[OrganizationSchema.ForwardSyncLiveIdBusinessInstance];
			}
			set
			{
				this[OrganizationSchema.ForwardSyncLiveIdBusinessInstance] = value;
			}
		}

		public ADObjectId HierarchicalAddressBookRoot
		{
			get
			{
				return (ADObjectId)this[OrganizationSchema.HABRootDepartmentLink];
			}
			internal set
			{
				this[OrganizationSchema.HABRootDepartmentLink] = value;
			}
		}

		public virtual ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationSchema.SIPAccessService];
			}
			set
			{
				this[OrganizationSchema.SIPAccessService] = value;
			}
		}

		public virtual ProtocolConnectionSettings AVAuthenticationService
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationSchema.AVAuthenticationService];
			}
			set
			{
				this[OrganizationSchema.AVAuthenticationService] = value;
			}
		}

		public virtual ProtocolConnectionSettings SIPSessionBorderController
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationSchema.SIPSessionBorderController];
			}
			set
			{
				this[OrganizationSchema.SIPSessionBorderController] = value;
			}
		}

		public virtual bool ExchangeNotificationEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.ExchangeNotificationEnabled];
			}
			set
			{
				this[OrganizationSchema.ExchangeNotificationEnabled] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> ExchangeNotificationRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[OrganizationSchema.ExchangeNotificationRecipients];
			}
			set
			{
				this[OrganizationSchema.ExchangeNotificationRecipients] = value;
			}
		}

		public virtual bool? EwsEnabled
		{
			get
			{
				return CASMailboxHelper.ToBooleanNullable((int?)this[OrganizationSchema.EwsEnabled]);
			}
			set
			{
				this[OrganizationSchema.EwsEnabled] = CASMailboxHelper.ToInt32Nullable(value);
			}
		}

		public virtual bool? EwsAllowOutlook
		{
			get
			{
				return (bool?)this[OrganizationSchema.EwsAllowOutlook];
			}
			set
			{
				this[OrganizationSchema.EwsAllowOutlook] = value;
			}
		}

		public virtual bool? EwsAllowMacOutlook
		{
			get
			{
				return (bool?)this[OrganizationSchema.EwsAllowMacOutlook];
			}
			set
			{
				this[OrganizationSchema.EwsAllowMacOutlook] = value;
			}
		}

		public virtual bool? EwsAllowEntourage
		{
			get
			{
				return (bool?)this[OrganizationSchema.EwsAllowEntourage];
			}
			set
			{
				this[OrganizationSchema.EwsAllowEntourage] = value;
			}
		}

		public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return (EwsApplicationAccessPolicy?)this[OrganizationSchema.EwsApplicationAccessPolicy];
			}
			set
			{
				this[OrganizationSchema.EwsApplicationAccessPolicy] = value;
			}
		}

		public virtual MultiValuedProperty<string> EwsAllowList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[OrganizationSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceAllowList)
				{
					return (MultiValuedProperty<string>)this[OrganizationSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[OrganizationSchema.EwsExceptions] = value;
				this.ewsAllowListSpecified = true;
			}
		}

		public virtual MultiValuedProperty<string> EwsBlockList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[OrganizationSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceBlockList)
				{
					return (MultiValuedProperty<string>)this[OrganizationSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[OrganizationSchema.EwsExceptions] = value;
				this.ewsBlockListSpecified = true;
			}
		}

		internal MultiValuedProperty<string> EwsExceptions
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.EwsExceptions];
			}
			set
			{
				this[OrganizationSchema.EwsExceptions] = value;
			}
		}

		internal bool EwsAllowListSpecified
		{
			get
			{
				return this.ewsAllowListSpecified;
			}
		}

		internal bool EwsBlockListSpecified
		{
			get
			{
				return this.ewsBlockListSpecified;
			}
		}

		internal int BuildMajor
		{
			get
			{
				return (int)this[OrganizationSchema.BuildMajor];
			}
		}

		internal int BuildMinor
		{
			get
			{
				return (int)this[OrganizationSchema.BuildMinor];
			}
		}

		public virtual EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval];
			}
			set
			{
				this[OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval] = value;
			}
		}

		public virtual bool ActivityBasedAuthenticationTimeoutEnabled
		{
			get
			{
				return !(bool)this[OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled];
			}
			set
			{
				this[OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled] = !value;
			}
		}

		public virtual bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
		{
			get
			{
				return !(bool)this[OrganizationSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled];
			}
			set
			{
				this[OrganizationSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled] = !value;
			}
		}

		internal OfflineAuthenticationProvisioningFlags OfflineAuthFlags
		{
			get
			{
				if (this[OrganizationSchema.OrganizationFlags2] != null)
				{
					return (OfflineAuthenticationProvisioningFlags)(((int)this[OrganizationSchema.OrganizationFlags2] & 24) >> 3);
				}
				return OfflineAuthenticationProvisioningFlags.Disabled;
			}
		}

		internal static int MapIntToPreferredInternetCodePageForShiftJis(int preferredInternetCodePageForShiftJis)
		{
			PreferredInternetCodePageForShiftJisEnum result;
			switch (preferredInternetCodePageForShiftJis)
			{
			case 0:
				result = PreferredInternetCodePageForShiftJisEnum.Undefined;
				break;
			case 1:
				result = PreferredInternetCodePageForShiftJisEnum.Iso2022Jp;
				break;
			case 2:
				result = PreferredInternetCodePageForShiftJisEnum.Esc2022Jp;
				break;
			case 3:
				result = PreferredInternetCodePageForShiftJisEnum.Sio2022Jp;
				break;
			default:
				result = PreferredInternetCodePageForShiftJisEnum.Undefined;
				break;
			}
			return (int)result;
		}

		public virtual int PreferredInternetCodePageForShiftJis
		{
			get
			{
				return Organization.MapIntToPreferredInternetCodePageForShiftJis((int)this[OrganizationSchema.PreferredInternetCodePageForShiftJis]);
			}
			set
			{
				PreferredInternetCodePageForShiftJisToIntEnum preferredInternetCodePageForShiftJisToIntEnum;
				if (value != 0)
				{
					switch (value)
					{
					case 50220:
						preferredInternetCodePageForShiftJisToIntEnum = PreferredInternetCodePageForShiftJisToIntEnum.Iso2022Jp;
						break;
					case 50221:
						preferredInternetCodePageForShiftJisToIntEnum = PreferredInternetCodePageForShiftJisToIntEnum.Esc2022Jp;
						break;
					case 50222:
						preferredInternetCodePageForShiftJisToIntEnum = PreferredInternetCodePageForShiftJisToIntEnum.Sio2022Jp;
						break;
					default:
						preferredInternetCodePageForShiftJisToIntEnum = PreferredInternetCodePageForShiftJisToIntEnum.Undefined;
						break;
					}
				}
				else
				{
					preferredInternetCodePageForShiftJisToIntEnum = PreferredInternetCodePageForShiftJisToIntEnum.Undefined;
				}
				this[OrganizationSchema.PreferredInternetCodePageForShiftJis] = (int)preferredInternetCodePageForShiftJisToIntEnum;
			}
		}

		internal static void RequiredCharsetCoverageSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[OrganizationSchema.ContentConversionFlags];
			int num2 = (int)value;
			int num3 = 127;
			num &= ~(num3 << 5);
			num |= (num2 & num3) << 5;
			num |= 524288;
			propertyBag[OrganizationSchema.ContentConversionFlags] = num;
		}

		internal static object RequiredCharsetCoverageGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[OrganizationSchema.ContentConversionFlags];
			int num2 = 127;
			int num3 = num2 & num >> 5;
			if ((num & 524288) != 0 || num3 != 0)
			{
				return num3;
			}
			return OrganizationSchema.RequiredCharsetCoverage.DefaultValue;
		}

		public virtual int RequiredCharsetCoverage
		{
			get
			{
				return (int)this[OrganizationSchema.RequiredCharsetCoverage];
			}
			set
			{
				this[OrganizationSchema.RequiredCharsetCoverage] = value;
			}
		}

		public virtual int ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return (int)this[OrganizationSchema.ByteEncoderTypeFor7BitCharsets];
			}
			set
			{
				this[OrganizationSchema.ByteEncoderTypeFor7BitCharsets] = value;
			}
		}

		public SoftDeletedFeatureStatusFlags SoftDeletedFeatureStatus
		{
			get
			{
				return (SoftDeletedFeatureStatusFlags)this[OrganizationSchema.SoftDeletedFeatureStatus];
			}
			set
			{
				this[OrganizationSchema.SoftDeletedFeatureStatus] = (int)value;
			}
		}

		public virtual bool AppsForOfficeEnabled
		{
			get
			{
				return !(bool)this[OrganizationSchema.AppsForOfficeDisabled];
			}
			set
			{
				this[OrganizationSchema.AppsForOfficeDisabled] = !value;
			}
		}

		public OrganizationConfigXML ConfigXML
		{
			get
			{
				return (OrganizationConfigXML)this[OrganizationSchema.ConfigurationXML];
			}
			set
			{
				this[OrganizationSchema.ConfigurationXML] = value;
			}
		}

		public int DefaultMovePriority
		{
			get
			{
				return (int)this[OrganizationSchema.DefaultMovePriority];
			}
			set
			{
				this[OrganizationSchema.DefaultMovePriority] = value;
			}
		}

		public string UpgradeMessage
		{
			get
			{
				return (string)this[OrganizationSchema.UpgradeMessage];
			}
			set
			{
				this[OrganizationSchema.UpgradeMessage] = value;
			}
		}

		public string UpgradeDetails
		{
			get
			{
				return (string)this[OrganizationSchema.UpgradeDetails];
			}
			set
			{
				this[OrganizationSchema.UpgradeDetails] = value;
			}
		}

		public UpgradeConstraintArray UpgradeConstraints
		{
			get
			{
				return (UpgradeConstraintArray)this[OrganizationSchema.UpgradeConstraints];
			}
			set
			{
				this[OrganizationSchema.UpgradeConstraints] = value;
			}
		}

		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)this[OrganizationSchema.UpgradeStage];
			}
			set
			{
				this[OrganizationSchema.UpgradeStage] = value;
			}
		}

		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)this[OrganizationSchema.UpgradeStageTimeStamp];
			}
			set
			{
				this[OrganizationSchema.UpgradeStageTimeStamp] = value;
			}
		}

		public int? UpgradeE14MbxCountForCurrentStage
		{
			get
			{
				return (int?)this[OrganizationSchema.UpgradeE14MbxCountForCurrentStage];
			}
			set
			{
				this[OrganizationSchema.UpgradeE14MbxCountForCurrentStage] = value;
			}
		}

		public int? UpgradeE14RequestCountForCurrentStage
		{
			get
			{
				return (int?)this[OrganizationSchema.UpgradeE14RequestCountForCurrentStage];
			}
			set
			{
				this[OrganizationSchema.UpgradeE14RequestCountForCurrentStage] = value;
			}
		}

		public bool? UpgradeConstraintsDisabled
		{
			get
			{
				return (bool?)this[OrganizationSchema.UpgradeConstraintsDisabled];
			}
			set
			{
				this[OrganizationSchema.UpgradeConstraintsDisabled] = value;
			}
		}

		public int? UpgradeUnitsOverride
		{
			get
			{
				return (int?)this[OrganizationSchema.UpgradeUnitsOverride];
			}
			set
			{
				this[OrganizationSchema.UpgradeUnitsOverride] = value;
			}
		}

		public DateTime? UpgradeLastE14CountsUpdateTime
		{
			get
			{
				return (DateTime?)this[OrganizationSchema.UpgradeLastE14CountsUpdateTime];
			}
			set
			{
				this[OrganizationSchema.UpgradeLastE14CountsUpdateTime] = value;
			}
		}

		internal RelocationConstraintArray PersistedRelocationConstraints
		{
			get
			{
				return (RelocationConstraintArray)this[OrganizationSchema.PersistedRelocationConstraints];
			}
			set
			{
				this[OrganizationSchema.PersistedRelocationConstraints] = value;
			}
		}

		internal bool OriginatedInDifferentForest
		{
			get
			{
				return (bool)this[OrganizationSchema.OriginatedInDifferentForest];
			}
		}

		internal static object MimeTypesGetter(IPropertyBag propertyBag)
		{
			return Organization.ParseMimeTypesBlob((byte[])propertyBag[OrganizationSchema.BlobMimeTypes]);
		}

		internal static void MimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OrganizationSchema.BlobMimeTypes] = Organization.BuildMimeTypesBlob((MultiValuedProperty<string>)value);
		}

		private static MultiValuedProperty<string> ParseMimeTypesBlob(byte[] blob)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (blob == null)
			{
				return multiValuedProperty;
			}
			int num = 0;
			for (int i = 0; i < blob.Length; i++)
			{
				if (blob[i] == 0)
				{
					int num2 = i - num;
					if (num2 > 0)
					{
						try
						{
							string @string = Encoding.ASCII.GetString(blob, num, num2);
							if (!multiValuedProperty.Contains(@string))
							{
								multiValuedProperty.Add(@string);
							}
						}
						catch (ArgumentOutOfRangeException innerException)
						{
							throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotParseMimeTypes, OrganizationSchema.BlobMimeTypes, blob), innerException);
						}
					}
					num = i + 1;
				}
			}
			return multiValuedProperty;
		}

		private static byte[] BuildMimeTypesBlob(MultiValuedProperty<string> mimeTypes)
		{
			if (mimeTypes == null || mimeTypes.Count == 0)
			{
				return null;
			}
			int num = 0;
			foreach (string text in mimeTypes)
			{
				num += text.Length + 1;
			}
			num++;
			byte[] array = new byte[num];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					foreach (string s in mimeTypes)
					{
						binaryWriter.Write(Encoding.ASCII.GetBytes(s));
						binaryWriter.Write(0);
					}
					binaryWriter.Write(0);
					binaryWriter.Flush();
				}
			}
			return array;
		}

		internal static object ForeignForestFQDNGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[OrganizationSchema.ForeignForestFQDNRaw];
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (string.IsNullOrEmpty(text))
			{
				return multiValuedProperty;
			}
			if (text.IndexOf(',') != -1)
			{
				string[] array = text.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length != 0)
				{
					foreach (string item in array)
					{
						multiValuedProperty.Add(item);
					}
				}
			}
			else
			{
				multiValuedProperty.Add(text);
			}
			return multiValuedProperty;
		}

		internal static void ForeignForestFQDNSetter(object value, IPropertyBag propertyBag)
		{
			string value2 = null;
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)value;
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				foreach (string value3 in multiValuedProperty)
				{
					stringBuilder.Append(value3);
					stringBuilder.Append(',');
				}
				stringBuilder.Length--;
				value2 = stringBuilder.ToString();
			}
			propertyBag[OrganizationSchema.ForeignForestFQDNRaw] = value2;
		}

		internal static object DistributionGroupNamingPolicyGetter(IPropertyBag propertyBag)
		{
			string namingPolicy = (string)propertyBag[OrganizationSchema.DistributionGroupNamingPolicyRaw];
			object result;
			try
			{
				result = DistributionGroupNamingPolicy.Parse(namingPolicy);
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(OrganizationSchema.DistributionGroupNamingPolicy.Name, ex.Message), ADUserSchema.CertificateSubject, propertyBag[OrganizationSchema.DistributionGroupNamingPolicyRaw]), ex);
			}
			return result;
		}

		internal static void DistributionGroupNamingPolicySetter(object value, IPropertyBag propertyBag)
		{
			string value2 = null;
			if (value != null)
			{
				value2 = ((DistributionGroupNamingPolicy)value).ToString();
			}
			propertyBag[OrganizationSchema.DistributionGroupNamingPolicyRaw] = value2;
		}

		internal static object OriginatedInDifferentForestGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.CorrelationIdRaw] != null && (Guid)propertyBag[ADObjectSchema.CorrelationIdRaw] != Guid.Empty;
		}

		internal static object ShowAdminAccessWarningGetter(IPropertyBag propertyBag)
		{
			return !(bool)propertyBag[OrganizationSchema.HideAdminAccessWarning] && !Organization.IsLegacyIOwnServicePlan((string)propertyBag[ExchangeConfigurationUnitSchema.ServicePlan]);
		}

		internal static MultiValuedProperty<UMLanguage> UMAvailableLanguagesGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)propertyBag[OrganizationSchema.UMAvailableLanguagesRaw];
			MultiValuedProperty<UMLanguage> multiValuedProperty2 = new MultiValuedProperty<UMLanguage>();
			if (multiValuedProperty != null)
			{
				foreach (int lcid in multiValuedProperty)
				{
					try
					{
						UMLanguage item = new UMLanguage(lcid);
						multiValuedProperty2.Add(item);
					}
					catch (ArgumentException)
					{
					}
				}
			}
			return multiValuedProperty2;
		}

		internal static void UMAvailableLanguagesSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<UMLanguage> multiValuedProperty = value as MultiValuedProperty<UMLanguage>;
			if (multiValuedProperty != null)
			{
				MultiValuedProperty<int> multiValuedProperty2 = new MultiValuedProperty<int>();
				foreach (UMLanguage umlanguage in multiValuedProperty)
				{
					multiValuedProperty2.Add(umlanguage.LCID);
				}
				propertyBag[OrganizationSchema.UMAvailableLanguagesRaw] = multiValuedProperty2;
				return;
			}
			propertyBag[OrganizationSchema.UMAvailableLanguagesRaw] = null;
		}

		internal static QueryFilter EnableAsSharedConfigurationFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 1UL));
		}

		internal static QueryFilter ImmutableConfigurationFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 1024UL));
		}

		internal static QueryFilter HostingDeploymentEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 32UL));
		}

		internal static QueryFilter LicensingEnforcedFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 2UL));
		}

		internal static QueryFilter IsTenantAccessBlockedFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 4194304UL));
		}

		internal static QueryFilter UseServicePlanAsCounterInstanceNameFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 64UL));
		}

		internal static object AdminDisplayVersionGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[OrganizationSchema.ObjectVersion];
			int num2 = (int)OrganizationSchema.BuildMajor.GetterDelegate(propertyBag);
			int num3 = (int)OrganizationSchema.BuildMinor.GetterDelegate(propertyBag);
			byte majorBuild = 0;
			byte minorBuild = 0;
			if (num >= 15000)
			{
				majorBuild = 15;
			}
			else if (num >= 12000)
			{
				majorBuild = ExchangeObjectVersion.Exchange2010.ExchangeBuild.Major;
				if (num >= 14700)
				{
					minorBuild = 17;
				}
				else if (num >= 14000)
				{
					minorBuild = 16;
				}
				else if (num >= 13000)
				{
					minorBuild = 15;
				}
			}
			return new ExchangeObjectVersion(ExchangeObjectVersion.Current.Major, ExchangeObjectVersion.Current.Minor, majorBuild, minorBuild, (ushort)num2, (ushort)num3);
		}

		internal void SetBuildVersion(int major, int minor)
		{
			this[OrganizationSchema.BuildMajor] = major;
			this[OrganizationSchema.BuildMinor] = minor;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.ResourceAddressLists.Count > 1)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorOrganizationResourceAddressListsCount, this.Identity, base.OriginatingServer));
			}
			if (this.SupportedSharedConfigurations.Count != 0 && this.SharedConfigurationInfo != null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSharedConfigurationBothRoles, this.Identity, base.OriginatingServer));
			}
			if (this.EnableAsSharedConfiguration)
			{
				if (this.SupportedSharedConfigurations.Count != 0)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorSharedConfigurationCannotBeEnabled, this.Identity, base.OriginatingServer));
				}
				if (this.SharedConfigurationInfo == null)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorNoSharedConfigurationInfo, this.Identity, base.OriginatingServer));
				}
			}
			if (this.propertyBag.IsModified(OrganizationSchema.AdfsAuthenticationRawConfiguration))
			{
				AdfsAuthenticationConfig adfsAuthenticationConfig = Organization.GetAdfsAuthenticationConfig(this.propertyBag);
				adfsAuthenticationConfig.Validate(errors);
			}
			if (this.TenantRelocationsAllowed && !base.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorTenantRelocationsAllowedOnlyForRootOrg, this.Identity, base.OriginatingServer));
			}
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				OrganizationSchema.DefaultPublicFolderIssueWarningQuota,
				OrganizationSchema.DefaultPublicFolderProhibitPostQuota
			}, this.Identity));
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(OrganizationSchema.MailTipsLargeAudienceThreshold))
			{
				this.MailTipsLargeAudienceThreshold = 25U;
			}
			if (!base.IsModified(OrganizationSchema.UMAvailableLanguages))
			{
				this.UMAvailableLanguages = new MultiValuedProperty<UMLanguage>(UMLanguage.DefaultLanguage);
			}
			base.StampPersistableDefaultValues();
		}

		private static bool IsLegacyIOwnServicePlan(string servicePlanName)
		{
			if (string.IsNullOrEmpty(servicePlanName))
			{
				return false;
			}
			foreach (string value in Organization.r3iOwnServicePlans)
			{
				if (servicePlanName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsGuidPrefixedLegacyDnDisabled
		{
			get
			{
				return (bool)this[OrganizationSchema.IsGuidPrefixedLegacyDnDisabled];
			}
			set
			{
				this[OrganizationSchema.IsGuidPrefixedLegacyDnDisabled] = value;
			}
		}

		public virtual bool IsMailboxForcedReplicationDisabled
		{
			get
			{
				return (bool)this[OrganizationSchema.IsMailboxForcedReplicationDisabled];
			}
			set
			{
				this[OrganizationSchema.IsMailboxForcedReplicationDisabled] = value;
			}
		}

		public virtual bool IsSyncPropertySetUpgradeAllowed
		{
			get
			{
				return (bool)this[OrganizationSchema.IsSyncPropertySetUpgradeAllowed];
			}
			set
			{
				this[OrganizationSchema.IsSyncPropertySetUpgradeAllowed] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SupportedSharedConfigurations
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrganizationSchema.SupportedSharedConfigurations];
			}
			internal set
			{
				this[OrganizationSchema.SupportedSharedConfigurations] = value;
			}
		}

		public SharedConfigurationInfo SharedConfigurationInfo
		{
			get
			{
				return (SharedConfigurationInfo)this[OrganizationSchema.SharedConfigurationInfo];
			}
			internal set
			{
				this[OrganizationSchema.SharedConfigurationInfo] = value;
			}
		}

		public bool EnableAsSharedConfiguration
		{
			get
			{
				return (bool)this[OrganizationSchema.EnableAsSharedConfiguration];
			}
			internal set
			{
				this[OrganizationSchema.EnableAsSharedConfiguration] = value;
			}
		}

		public bool ImmutableConfiguration
		{
			get
			{
				return (bool)this[OrganizationSchema.ImmutableConfiguration];
			}
			internal set
			{
				this[OrganizationSchema.ImmutableConfiguration] = value;
			}
		}

		public bool IsLicensingEnforced
		{
			get
			{
				return (bool)this[OrganizationSchema.IsLicensingEnforced];
			}
			internal set
			{
				this[OrganizationSchema.IsLicensingEnforced] = value;
			}
		}

		public bool IsTenantAccessBlocked
		{
			get
			{
				return (bool)this[OrganizationSchema.IsTenantAccessBlocked];
			}
			internal set
			{
				this[OrganizationSchema.IsTenantAccessBlocked] = value;
			}
		}

		public bool IsDehydrated
		{
			get
			{
				return (bool)this[OrganizationSchema.IsDehydrated];
			}
			internal set
			{
				this[OrganizationSchema.IsDehydrated] = value;
			}
		}

		public bool UseServicePlanAsCounterInstanceName
		{
			get
			{
				return (bool)this[OrganizationSchema.UseServicePlanAsCounterInstanceName];
			}
			set
			{
				this[OrganizationSchema.UseServicePlanAsCounterInstanceName] = value;
			}
		}

		public ExchangeObjectVersion RBACConfigurationVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[OrganizationSchema.RBACConfigurationVersion];
			}
			internal set
			{
				this[OrganizationSchema.RBACConfigurationVersion] = value;
			}
		}

		public virtual bool CalendarVersionStoreEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.CalendarVersionStoreEnabled];
			}
			set
			{
				this[OrganizationSchema.CalendarVersionStoreEnabled] = value;
			}
		}

		public virtual PublicFolderInformation DefaultPublicFolderMailbox
		{
			get
			{
				return (PublicFolderInformation)this[OrganizationSchema.DefaultPublicFolderMailbox];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderMailbox] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RemotePublicFolderMailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrganizationSchema.RemotePublicFolderMailboxes];
			}
			set
			{
				this[OrganizationSchema.RemotePublicFolderMailboxes] = value;
			}
		}

		public virtual PublicFoldersDeployment PublicFoldersEnabled
		{
			get
			{
				return (PublicFoldersDeployment)this[OrganizationSchema.PublicFoldersEnabled];
			}
			set
			{
				this[OrganizationSchema.PublicFoldersEnabled] = value;
			}
		}

		public ForestModeFlags ForestMode
		{
			get
			{
				return (ForestModeFlags)this[OrganizationSchema.ForestMode];
			}
		}

		public virtual MultiValuedProperty<UMLanguage> UMAvailableLanguages
		{
			get
			{
				return (MultiValuedProperty<UMLanguage>)this[OrganizationSchema.UMAvailableLanguages];
			}
			set
			{
				this[OrganizationSchema.UMAvailableLanguages] = value;
			}
		}

		private static AdfsAuthenticationConfig GetAdfsAuthenticationConfig(IPropertyBag propertyBag)
		{
			string encodedRawConfig = (string)propertyBag[OrganizationSchema.AdfsAuthenticationRawConfiguration];
			return new AdfsAuthenticationConfig(encodedRawConfig);
		}

		private static void SetAdfsAuthenticationConfig(IPropertyBag propertyBag, AdfsAuthenticationConfig obj)
		{
			propertyBag[OrganizationSchema.AdfsAuthenticationRawConfiguration] = obj.EncodedConfig;
		}

		public virtual string AdfsAuthenticationConfiguration
		{
			get
			{
				string result = null;
				AdfsAuthenticationConfig.TryDecode((string)this[OrganizationSchema.AdfsAuthenticationRawConfiguration], out result);
				return result;
			}
			set
			{
				this[OrganizationSchema.AdfsAuthenticationRawConfiguration] = AdfsAuthenticationConfig.Encode(value);
			}
		}

		public virtual Uri AdfsIssuer
		{
			get
			{
				return (Uri)this[OrganizationSchema.AdfsIssuer];
			}
			set
			{
				this[OrganizationSchema.AdfsIssuer] = value;
			}
		}

		internal static object AdfsIssuerGetter(IPropertyBag propertyBag)
		{
			return Organization.GetAdfsAuthenticationConfig(propertyBag).Issuer;
		}

		internal static void AdfsIssuerSetter(object value, IPropertyBag propertyBag)
		{
			AdfsAuthenticationConfig adfsAuthenticationConfig = Organization.GetAdfsAuthenticationConfig(propertyBag);
			adfsAuthenticationConfig.Issuer = (value as Uri);
			Organization.SetAdfsAuthenticationConfig(propertyBag, adfsAuthenticationConfig);
		}

		public virtual MultiValuedProperty<Uri> AdfsAudienceUris
		{
			get
			{
				return (MultiValuedProperty<Uri>)this[OrganizationSchema.AdfsAudienceUris];
			}
			set
			{
				this[OrganizationSchema.AdfsAudienceUris] = value;
			}
		}

		internal static object AdfsAudienceUrisGetter(IPropertyBag propertyBag)
		{
			return Organization.GetAdfsAuthenticationConfig(propertyBag).AudienceUris;
		}

		internal static void AdfsAudienceUrisSetter(object value, IPropertyBag propertyBag)
		{
			AdfsAuthenticationConfig adfsAuthenticationConfig = Organization.GetAdfsAuthenticationConfig(propertyBag);
			adfsAuthenticationConfig.AudienceUris = (value as MultiValuedProperty<Uri>);
			Organization.SetAdfsAuthenticationConfig(propertyBag, adfsAuthenticationConfig);
		}

		public virtual MultiValuedProperty<string> AdfsSignCertificateThumbprints
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.AdfsSignCertificateThumbprints];
			}
			set
			{
				this[OrganizationSchema.AdfsSignCertificateThumbprints] = value;
			}
		}

		internal static object AdfsSignCertificateThumbprintsGetter(IPropertyBag propertyBag)
		{
			return Organization.GetAdfsAuthenticationConfig(propertyBag).SignCertificateThumbprints;
		}

		internal static void AdfsSignCertificateThumbprintsSetter(object value, IPropertyBag propertyBag)
		{
			AdfsAuthenticationConfig adfsAuthenticationConfig = Organization.GetAdfsAuthenticationConfig(propertyBag);
			adfsAuthenticationConfig.SignCertificateThumbprints = (value as MultiValuedProperty<string>);
			Organization.SetAdfsAuthenticationConfig(propertyBag, adfsAuthenticationConfig);
		}

		public virtual string AdfsEncryptCertificateThumbprint
		{
			get
			{
				return (string)this[OrganizationSchema.AdfsEncryptCertificateThumbprint];
			}
			set
			{
				this[OrganizationSchema.AdfsEncryptCertificateThumbprint] = value;
			}
		}

		internal static object AdfsEncryptCertificateThumbprintGetter(IPropertyBag propertyBag)
		{
			return Organization.GetAdfsAuthenticationConfig(propertyBag).EncryptCertificateThumbprint;
		}

		internal static void AdfsEncryptCertificateThumbprintSetter(object value, IPropertyBag propertyBag)
		{
			AdfsAuthenticationConfig adfsAuthenticationConfig = Organization.GetAdfsAuthenticationConfig(propertyBag);
			adfsAuthenticationConfig.EncryptCertificateThumbprint = (value as string);
			Organization.SetAdfsAuthenticationConfig(propertyBag, adfsAuthenticationConfig);
		}

		public virtual Uri SiteMailboxCreationURL
		{
			get
			{
				return (Uri)this[OrganizationSchema.SiteMailboxCreationURL];
			}
			set
			{
				this[OrganizationSchema.SiteMailboxCreationURL] = value;
			}
		}

		internal static object PublicFoldersEnabledGetter(IPropertyBag propertyBag)
		{
			if ((bool)OrganizationSchema.HeuristicsFlagsGetter(HeuristicsFlags.PublicFoldersDisabled, OrganizationSchema.Heuristics, propertyBag))
			{
				return PublicFoldersDeployment.None;
			}
			if ((bool)OrganizationSchema.HeuristicsFlagsGetter(HeuristicsFlags.RemotePublicFolders, OrganizationSchema.Heuristics, propertyBag))
			{
				return PublicFoldersDeployment.Remote;
			}
			return PublicFoldersDeployment.Local;
		}

		internal static void PublicFoldersEnabledSetter(object value, IPropertyBag propertyBag)
		{
			PublicFoldersDeployment publicFoldersDeployment = (PublicFoldersDeployment)value;
			if (publicFoldersDeployment == PublicFoldersDeployment.Local)
			{
				OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.PublicFoldersDisabled, OrganizationSchema.Heuristics, false, propertyBag);
				OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.RemotePublicFolders, OrganizationSchema.Heuristics, false, propertyBag);
				return;
			}
			if (publicFoldersDeployment == PublicFoldersDeployment.Remote)
			{
				OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.PublicFoldersDisabled, OrganizationSchema.Heuristics, false, propertyBag);
				OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.RemotePublicFolders, OrganizationSchema.Heuristics, true, propertyBag);
				return;
			}
			OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.PublicFoldersDisabled, OrganizationSchema.Heuristics, true, propertyBag);
			OrganizationSchema.HeuristicsFlagsSetter(HeuristicsFlags.RemotePublicFolders, OrganizationSchema.Heuristics, false, propertyBag);
		}

		internal static object SiteMailboxCreationURLGetter(IPropertyBag propertyBag)
		{
			Uri result = null;
			string serviceEndPoints = (string)propertyBag[OrganizationSchema.ServiceEndpoints];
			Dictionary<string, string> dictionary = Organization.ParseServiceEndpointsAttribute(serviceEndPoints);
			if (dictionary.ContainsKey(OrganizationSchema.SiteMailboxCreationURL.Name))
			{
				result = new Uri(dictionary[OrganizationSchema.SiteMailboxCreationURL.Name]);
			}
			return result;
		}

		internal static void SiteMailboxCreationURLSetter(object value, IPropertyBag propertyBag)
		{
			string serviceEndPoints = (string)propertyBag[OrganizationSchema.ServiceEndpoints];
			Dictionary<string, string> dictionary = Organization.ParseServiceEndpointsAttribute(serviceEndPoints);
			dictionary[OrganizationSchema.SiteMailboxCreationURL.Name] = ((value == null) ? string.Empty : ((Uri)value).AbsoluteUri);
			propertyBag[OrganizationSchema.ServiceEndpoints] = Organization.FormatServiceEndpointsAttribute(dictionary);
		}

		internal static Dictionary<string, string> ParseServiceEndpointsAttribute(string serviceEndPoints)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(serviceEndPoints))
			{
				string[] array = serviceEndPoints.Split(new char[]
				{
					Organization.serviceEndpointSeparator
				});
				string key = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					if (i % 2 == 0)
					{
						key = array[i];
					}
					else
					{
						string text = array[i];
						if (!dictionary.ContainsKey(key) && Uri.IsWellFormedUriString(text, UriKind.Absolute))
						{
							dictionary.Add(key, text);
						}
					}
				}
			}
			return dictionary;
		}

		private static string FormatServiceEndpointsAttribute(Dictionary<string, string> serviceEndpointCollection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (serviceEndpointCollection != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in serviceEndpointCollection)
				{
					if (!string.IsNullOrWhiteSpace(keyValuePair.Value))
					{
						stringBuilder.AppendFormat("{1}{0}{2}{0}", Organization.serviceEndpointSeparator, keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public int MaxAddressBookMigrations { get; set; }

		public const int DefaultSCLJunkThreshold = 4;

		private static OrganizationSchema schema = ObjectSchema.GetInstance<OrganizationSchema>();

		internal static readonly string MostDerivedClass = "msExchOrganizationContainer";

		private static string[] r3iOwnServicePlans = new string[]
		{
			"EDU_I_Own_E14_R3",
			"SegmentedGalEDU_I_Own_E14_R3"
		};

		private static char serviceEndpointSeparator = ';';

		private bool ewsAllowListSpecified;

		private bool ewsBlockListSpecified;

		internal static readonly ExchangeObjectVersion CurrentExchangeRootOrgVersion = new ExchangeObjectVersion(2, 0, ExchangeObjectVersion.Exchange2012.ExchangeBuild);
	}
}
