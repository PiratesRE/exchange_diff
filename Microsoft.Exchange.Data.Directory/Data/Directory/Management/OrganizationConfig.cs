using System;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class OrganizationConfig : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return OrganizationConfig.schema;
			}
		}

		internal MultiValuedProperty<string> AcceptedDomainNames
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.AcceptedDomainNames];
			}
		}

		public OrganizationConfig()
		{
		}

		public OrganizationConfig(ADOrganizationConfig dataObject) : base(dataObject)
		{
		}

		public OrganizationConfig(ADOrganizationConfig dataObject, bool valuesQueriedFromDC) : base(dataObject)
		{
			this.valuesQueriedFromDC = valuesQueriedFromDC;
		}

		public new OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[OrganizationConfigSchema.OrganizationId];
			}
		}

		public new string Name
		{
			get
			{
				if (base.DataObject.OrganizationalUnitRoot != null)
				{
					return base.DataObject.OrganizationId.OrganizationalUnit.Name;
				}
				return base.Name;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				if (base.DataObject.OrganizationalUnitRoot == null)
				{
					return base.Identity;
				}
				ObjectId objectId = base.DataObject.OrganizationId.OrganizationalUnit;
				object obj;
				if (base.TryConvertOutputProperty(objectId, "Identity", out obj))
				{
					objectId = (ObjectId)obj;
				}
				return objectId;
			}
		}

		public new Guid Guid
		{
			get
			{
				if (base.DataObject.OrganizationalUnitRoot != null)
				{
					return base.DataObject.OrganizationId.OrganizationalUnit.ObjectGuid;
				}
				return base.Guid;
			}
		}

		public int ObjectVersion
		{
			get
			{
				return (int)this.propertyBag[OrganizationConfigSchema.ObjectVersion];
			}
		}

		public EnhancedTimeSpan? DefaultPublicFolderAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationConfigSchema.DefaultPublicFolderAgeLimit];
			}
		}

		public Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationConfigSchema.DefaultPublicFolderIssueWarningQuota];
			}
		}

		public Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationConfigSchema.DefaultPublicFolderProhibitPostQuota];
			}
		}

		public Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationConfigSchema.DefaultPublicFolderMaxItemSize];
			}
		}

		public EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationConfigSchema.DefaultPublicFolderDeletedItemRetention];
			}
		}

		public EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan?)this[OrganizationConfigSchema.DefaultPublicFolderMovedItemRetention];
			}
		}

		public bool PublicFoldersLockedForMigration
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.PublicFoldersLockedForMigration];
			}
		}

		public bool PublicFolderMigrationComplete
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.PublicFolderMigrationComplete];
			}
		}

		public bool PublicFolderMailboxesLockedForNewConnections
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.PublicFolderMailboxesLockedForNewConnections];
			}
		}

		public bool PublicFolderMailboxesMigrationComplete
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.PublicFolderMailboxesMigrationComplete];
			}
		}

		public PublicFoldersDeployment PublicFoldersEnabled
		{
			get
			{
				return (PublicFoldersDeployment)this[OrganizationConfigSchema.PublicFoldersEnabled];
			}
		}

		public bool ActivityBasedAuthenticationTimeoutEnabled
		{
			get
			{
				return !(bool)this[OrganizationConfigSchema.ActivityBasedAuthenticationTimeoutDisabled];
			}
		}

		public EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[OrganizationConfigSchema.ActivityBasedAuthenticationTimeoutInterval];
			}
		}

		public bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
		{
			get
			{
				return !(bool)this[OrganizationConfigSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled];
			}
		}

		public bool AppsForOfficeEnabled
		{
			get
			{
				return !(bool)this[OrganizationConfigSchema.AppsForOfficeDisabled];
			}
		}

		public ProtocolConnectionSettings AVAuthenticationService
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationConfigSchema.AVAuthenticationService];
			}
		}

		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)this[OrganizationConfigSchema.CustomerFeedbackEnabled];
			}
		}

		public ADObjectId DistributionGroupDefaultOU
		{
			get
			{
				return (ADObjectId)this[OrganizationConfigSchema.DistributionGroupDefaultOU];
			}
		}

		public MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationConfigSchema.DistributionGroupNameBlockedWordsList];
			}
		}

		public DistributionGroupNamingPolicy DistributionGroupNamingPolicy
		{
			get
			{
				return (DistributionGroupNamingPolicy)this[OrganizationConfigSchema.DistributionGroupNamingPolicy];
			}
		}

		public bool? EwsAllowEntourage
		{
			get
			{
				return (bool?)this[OrganizationConfigSchema.EwsAllowEntourage];
			}
		}

		public MultiValuedProperty<string> EwsAllowList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[OrganizationConfigSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceAllowList)
				{
					return (MultiValuedProperty<string>)this[OrganizationConfigSchema.EwsExceptions];
				}
				return null;
			}
		}

		public bool? EwsAllowMacOutlook
		{
			get
			{
				return (bool?)this[OrganizationConfigSchema.EwsAllowMacOutlook];
			}
		}

		public bool? EwsAllowOutlook
		{
			get
			{
				return (bool?)this[OrganizationConfigSchema.EwsAllowOutlook];
			}
		}

		public EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return (EwsApplicationAccessPolicy?)this[OrganizationConfigSchema.EwsApplicationAccessPolicy];
			}
		}

		public MultiValuedProperty<string> EwsBlockList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[OrganizationConfigSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceBlockList)
				{
					return (MultiValuedProperty<string>)this[OrganizationConfigSchema.EwsExceptions];
				}
				return null;
			}
		}

		public bool? EwsEnabled
		{
			get
			{
				return CASMailboxHelper.ToBooleanNullable((int?)this[OrganizationConfigSchema.EwsEnabled]);
			}
		}

		public bool ExchangeNotificationEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.ExchangeNotificationEnabled];
			}
		}

		public MultiValuedProperty<SmtpAddress> ExchangeNotificationRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[OrganizationConfigSchema.ExchangeNotificationRecipients];
			}
		}

		public ADObjectId HierarchicalAddressBookRoot
		{
			get
			{
				return (ADObjectId)this[OrganizationConfigSchema.HABRootDepartmentLink];
			}
		}

		public IndustryType Industry
		{
			get
			{
				return (IndustryType)this[OrganizationConfigSchema.Industry];
			}
		}

		public bool MailTipsAllTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.MailTipsAllTipsEnabled];
			}
		}

		public bool MailTipsExternalRecipientsTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.MailTipsExternalRecipientsTipsEnabled];
			}
		}

		public bool MailTipsGroupMetricsEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.MailTipsGroupMetricsEnabled];
			}
		}

		public uint MailTipsLargeAudienceThreshold
		{
			get
			{
				return (uint)((long)this[OrganizationConfigSchema.MailTipsLargeAudienceThreshold]);
			}
		}

		public bool MailTipsMailboxSourcedTipsEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.MailTipsMailboxSourcedTipsEnabled];
			}
		}

		public string ManagedFolderHomepage
		{
			get
			{
				return (string)this[OrganizationConfigSchema.ManagedFolderHomepage];
			}
		}

		public ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[OrganizationConfigSchema.MicrosoftExchangeRecipientEmailAddresses];
			}
		}

		public bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled];
			}
		}

		public SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[OrganizationConfigSchema.MicrosoftExchangeRecipientPrimarySmtpAddress];
			}
		}

		public ADObjectId MicrosoftExchangeRecipientReplyRecipient
		{
			get
			{
				return (ADObjectId)this[OrganizationConfigSchema.MicrosoftExchangeRecipientReplyRecipient];
			}
		}

		public bool ForwardSyncLiveIdBusinessInstance
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.ForwardSyncLiveIdBusinessInstance];
			}
		}

		public MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
		{
			get
			{
				return (MultiValuedProperty<OrganizationSummaryEntry>)this[OrganizationConfigSchema.OrganizationSummary];
			}
		}

		public bool ReadTrackingEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.ReadTrackingEnabled];
			}
		}

		public int SCLJunkThreshold
		{
			get
			{
				return (int)this[OrganizationConfigSchema.SCLJunkThreshold];
			}
		}

		public ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationConfigSchema.SIPAccessService];
			}
		}

		public ProtocolConnectionSettings SIPSessionBorderController
		{
			get
			{
				return (ProtocolConnectionSettings)this[OrganizationConfigSchema.SIPSessionBorderController];
			}
		}

		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)(this[OrganizationConfigSchema.MaxConcurrentMigrations] ?? Unlimited<int>.UnlimitedValue);
			}
		}

		public int? MaxAddressBookPolicies
		{
			get
			{
				return (int?)this[OrganizationConfigSchema.MaxAddressBookPolicies];
			}
		}

		public int? MaxOfflineAddressBooks
		{
			get
			{
				return (int?)this[OrganizationConfigSchema.MaxOfflineAddressBooks];
			}
		}

		public bool IsExcludedFromOnboardMigration
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.IsExcludedFromOnboardMigration] ?? false);
			}
		}

		public bool IsExcludedFromOffboardMigration
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.IsExcludedFromOffboardMigration] ?? false);
			}
		}

		public bool IsFfoMigrationInProgress
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.IsFfoMigrationInProgress] ?? false);
			}
		}

		public bool IsProcessEhaMigratedMessagesEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsProcessEhaMigratedMessagesEnabled];
			}
		}

		public bool TenantRelocationsAllowed
		{
			get
			{
				return (bool)(this[OrganizationSchema.TenantRelocationsAllowed] ?? false);
			}
		}

		public bool ACLableSyncedObjectEnabled
		{
			get
			{
				return (bool)(this[OrganizationSchema.ACLableSyncedObjectEnabled] ?? false);
			}
		}

		public int PreferredInternetCodePageForShiftJis
		{
			get
			{
				return Organization.MapIntToPreferredInternetCodePageForShiftJis((int)this[OrganizationConfigSchema.PreferredInternetCodePageForShiftJis]);
			}
		}

		public int RequiredCharsetCoverage
		{
			get
			{
				return (int)this[OrganizationConfigSchema.RequiredCharsetCoverage];
			}
		}

		public int ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return (int)this[OrganizationConfigSchema.ByteEncoderTypeFor7BitCharsets];
			}
		}

		public bool PublicComputersDetectionEnabled
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.PublicComputersDetectionEnabled] ?? false);
			}
		}

		public RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
		{
			get
			{
				return (RmsoSubscriptionStatusFlags)this[OrganizationConfigSchema.RmsoSubscriptionStatus];
			}
		}

		public bool IntuneManagedStatus
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IntuneManagedStatus];
			}
		}

		public HybridConfigurationStatusFlags HybridConfigurationStatus
		{
			get
			{
				return (HybridConfigurationStatusFlags)this[OrganizationConfigSchema.HybridConfigurationStatus];
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[OrganizationConfigSchema.ReleaseTrack];
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[OrganizationConfigSchema.SharePointUrl];
			}
		}

		public bool MapiHttpEnabled
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.MapiHttpEnabled] ?? false);
			}
		}

		public bool OAuth2ClientProfileEnabled
		{
			get
			{
				return (bool)(this[OrganizationConfigSchema.OAuth2ClientProfileEnabled] ?? false);
			}
		}

		public string OrganizationConfigHash
		{
			get
			{
				if (!this.valuesQueriedFromDC)
				{
					return string.Empty;
				}
				string value = this.PreviousAdminDisplayVersion.ToString();
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
				{
					xmlWriter.WriteStartDocument();
					xmlWriter.WriteStartElement("DCHybridConfiguration");
					xmlWriter.WriteElementString("IsDataCenter", this.valuesQueriedFromDC.ToString());
					xmlWriter.WriteElementString("AdminDisplayVersion", this.AdminDisplayVersion.ToString());
					xmlWriter.WriteElementString("IsUpgradingOrganization", this.IsUpgradingOrganization.ToString());
					xmlWriter.WriteElementString("PreviousAdminDisplayVersion", value);
					xmlWriter.WriteStartElement("AcceptedDomain");
					if (this.AcceptedDomainNames == null || this.AcceptedDomainNames.ToArray().Length == 0)
					{
						return string.Empty;
					}
					foreach (string value2 in this.AcceptedDomainNames.ToArray())
					{
						xmlWriter.WriteElementString("DomainName", value2);
					}
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndDocument();
				}
				byte[] inArray;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
					{
						using (StreamWriter streamWriter = new StreamWriter(deflateStream, Encoding.UTF8))
						{
							streamWriter.Write(stringBuilder.ToString());
						}
					}
					inArray = memoryStream.ToArray();
				}
				return Convert.ToBase64String(inArray);
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[OrganizationConfigSchema.LegacyExchangeDN];
			}
		}

		public HeuristicsFlags Heuristics
		{
			get
			{
				return (HeuristicsFlags)this[OrganizationConfigSchema.Heuristics];
			}
		}

		public MultiValuedProperty<ADObjectId> ResourceAddressLists
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrganizationConfigSchema.ResourceAddressLists];
			}
		}

		public bool IsMixedMode
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsMixedMode];
			}
		}

		public ExchangeObjectVersion PreviousAdminDisplayVersion
		{
			get
			{
				MailboxRelease mailboxRelease;
				if (!Enum.TryParse<MailboxRelease>((string)this[OrganizationConfigSchema.PreviousAdminDisplayVersion], true, out mailboxRelease))
				{
					mailboxRelease = MailboxRelease.E14;
				}
				if (mailboxRelease == MailboxRelease.E15)
				{
					return ExchangeObjectVersion.Exchange2012;
				}
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public bool IsAddressListPagingEnabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsAddressListPagingEnabled];
			}
		}

		public MultiValuedProperty<string> ForeignForestFQDN
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationConfigSchema.ForeignForestFQDN];
			}
		}

		public SecurityIdentifier ForeignForestOrgAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationConfigSchema.ForeignForestOrgAdminUSGSid];
			}
		}

		public SecurityIdentifier ForeignForestRecipientAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationConfigSchema.ForeignForestRecipientAdminUSGSid];
			}
		}

		public SecurityIdentifier ForeignForestViewOnlyAdminUSGSid
		{
			get
			{
				return (SecurityIdentifier)this[OrganizationConfigSchema.ForeignForestViewOnlyAdminUSGSid];
			}
		}

		public MultiValuedProperty<string> MimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationConfigSchema.MimeTypes];
			}
		}

		public bool IsLicensingEnforced
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsLicensingEnforced];
			}
		}

		public bool IsTenantAccessBlocked
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsTenantAccessBlocked];
			}
		}

		public bool IsDehydrated
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsDehydrated];
			}
		}

		public bool IsGuidPrefixedLegacyDnDisabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsGuidPrefixedLegacyDnDisabled];
			}
		}

		public bool IsMailboxForcedReplicationDisabled
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsMailboxForcedReplicationDisabled];
			}
		}

		public ExchangeObjectVersion RBACConfigurationVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[OrganizationConfigSchema.RBACConfigurationVersion];
			}
		}

		public bool IsSyncPropertySetUpgradeAllowed
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsSyncPropertySetUpgradeAllowed];
			}
		}

		public PublicFolderInformation RootPublicFolderMailbox
		{
			get
			{
				return (PublicFolderInformation)this[OrganizationConfigSchema.DefaultPublicFolderMailbox];
			}
		}

		public MultiValuedProperty<ADObjectId> RemotePublicFolderMailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrganizationConfigSchema.RemotePublicFolderMailboxes];
			}
		}

		public ExchangeObjectVersion AdminDisplayVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[OrganizationConfigSchema.AdminDisplayVersion];
			}
		}

		public bool IsUpgradingOrganization
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsUpgradingOrganization];
			}
		}

		public bool IsUpdatingServicePlan
		{
			get
			{
				return (bool)this[OrganizationConfigSchema.IsUpdatingServicePlan];
			}
		}

		public string ServicePlan
		{
			get
			{
				return (string)this[OrganizationConfigSchema.ServicePlan];
			}
		}

		public string TargetServicePlan
		{
			get
			{
				return (string)this[OrganizationConfigSchema.TargetServicePlan];
			}
		}

		public string WACDiscoveryEndpoint
		{
			get
			{
				return (string)this[OrganizationConfigSchema.WACDiscoveryEndpoint];
			}
		}

		public MultiValuedProperty<UMLanguage> UMAvailableLanguages
		{
			get
			{
				return (MultiValuedProperty<UMLanguage>)this[OrganizationConfigSchema.UMAvailableLanguages];
			}
		}

		public string AdfsAuthenticationConfiguration
		{
			get
			{
				string result = null;
				AdfsAuthenticationConfig.TryDecode((string)this[OrganizationSchema.AdfsAuthenticationRawConfiguration], out result);
				return result;
			}
		}

		public Uri AdfsIssuer
		{
			get
			{
				return (Uri)this[OrganizationConfigSchema.AdfsIssuer];
			}
		}

		public MultiValuedProperty<Uri> AdfsAudienceUris
		{
			get
			{
				return (MultiValuedProperty<Uri>)this[OrganizationConfigSchema.AdfsAudienceUris];
			}
		}

		public MultiValuedProperty<string> AdfsSignCertificateThumbprints
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationConfigSchema.AdfsSignCertificateThumbprints];
			}
		}

		public string AdfsEncryptCertificateThumbprint
		{
			get
			{
				return (string)this[OrganizationConfigSchema.AdfsEncryptCertificateThumbprint];
			}
		}

		public Uri SiteMailboxCreationURL
		{
			get
			{
				return (Uri)this[OrganizationConfigSchema.SiteMailboxCreationURL];
			}
		}

		private readonly bool valuesQueriedFromDC;

		private static readonly OrganizationConfigSchema schema = ObjectSchema.GetInstance<OrganizationConfigSchema>();
	}
}
