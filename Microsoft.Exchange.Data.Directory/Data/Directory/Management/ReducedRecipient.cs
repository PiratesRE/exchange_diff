using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ValidationRules;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ReducedRecipient : ADObject
	{
		internal static Dictionary<PropertySet, ADPropertyDefinition[]> Properties
		{
			get
			{
				if (ReducedRecipient.properties == null)
				{
					ReducedRecipient.properties = new Dictionary<PropertySet, ADPropertyDefinition[]>
					{
						{
							PropertySet.All,
							null
						},
						{
							PropertySet.ControlPanel,
							new ADPropertyDefinition[]
							{
								ReducedRecipientSchema.PrimarySmtpAddress,
								ReducedRecipientSchema.DisplayName,
								ReducedRecipientSchema.ArchiveGuid,
								ReducedRecipientSchema.AuthenticationType,
								ReducedRecipientSchema.RecipientType,
								ReducedRecipientSchema.RecipientTypeDetails,
								ReducedRecipientSchema.ResourceType,
								ReducedRecipientSchema.WindowsLiveID,
								ADObjectSchema.Id,
								ADObjectSchema.ExchangeVersion,
								ADObjectSchema.OrganizationId
							}
						},
						{
							PropertySet.ConsoleSmallSet,
							new ADPropertyDefinition[]
							{
								ReducedRecipientSchema.DisplayName,
								ReducedRecipientSchema.Alias,
								ReducedRecipientSchema.OrganizationalUnit,
								ReducedRecipientSchema.PrimarySmtpAddress,
								ReducedRecipientSchema.EmailAddresses,
								ReducedRecipientSchema.HiddenFromAddressListsEnabled,
								ADObjectSchema.Name,
								ADObjectSchema.WhenChanged,
								ReducedRecipientSchema.City,
								ReducedRecipientSchema.Company,
								ReducedRecipientSchema.CountryOrRegion,
								ReducedRecipientSchema.DatabaseName,
								ReducedRecipientSchema.Department,
								ReducedRecipientSchema.ExpansionServer,
								ReducedRecipientSchema.ExternalEmailAddress,
								ReducedRecipientSchema.FirstName,
								ReducedRecipientSchema.LastName,
								ReducedRecipientSchema.Office,
								ReducedRecipientSchema.StateOrProvince,
								ReducedRecipientSchema.Title,
								ReducedRecipientSchema.UMEnabled,
								ReducedRecipientSchema.HasActiveSyncDevicePartnership,
								ReducedRecipientSchema.Manager,
								ReducedRecipientSchema.SharingPolicy,
								ReducedRecipientSchema.ArchiveGuid,
								ReducedRecipientSchema.IsValidSecurityPrincipal,
								ReducedRecipientSchema.ArchiveState,
								ReducedRecipientSchema.MailboxMoveTargetMDB,
								ReducedRecipientSchema.MailboxMoveSourceMDB,
								ReducedRecipientSchema.MailboxMoveTargetArchiveMDB,
								ReducedRecipientSchema.MailboxMoveSourceArchiveMDB,
								ReducedRecipientSchema.MailboxMoveFlags,
								ReducedRecipientSchema.MailboxMoveRemoteHostName,
								ReducedRecipientSchema.MailboxMoveBatchName,
								ReducedRecipientSchema.MailboxMoveStatus,
								ReducedRecipientSchema.MailboxRelease,
								ReducedRecipientSchema.ArchiveRelease,
								ReducedRecipientSchema.RecipientType,
								ReducedRecipientSchema.RecipientTypeDetails,
								ADObjectSchema.Id,
								ADObjectSchema.ExchangeVersion,
								ADObjectSchema.OrganizationId
							}
						},
						{
							PropertySet.ConsoleLargeSet,
							new ADPropertyDefinition[]
							{
								ReducedRecipientSchema.DisplayName,
								ReducedRecipientSchema.Alias,
								ReducedRecipientSchema.OrganizationalUnit,
								ReducedRecipientSchema.PrimarySmtpAddress,
								ReducedRecipientSchema.CustomAttribute1,
								ReducedRecipientSchema.CustomAttribute2,
								ReducedRecipientSchema.CustomAttribute3,
								ReducedRecipientSchema.CustomAttribute4,
								ReducedRecipientSchema.CustomAttribute5,
								ReducedRecipientSchema.CustomAttribute6,
								ReducedRecipientSchema.CustomAttribute7,
								ReducedRecipientSchema.CustomAttribute8,
								ReducedRecipientSchema.CustomAttribute9,
								ReducedRecipientSchema.CustomAttribute10,
								ReducedRecipientSchema.CustomAttribute11,
								ReducedRecipientSchema.CustomAttribute12,
								ReducedRecipientSchema.CustomAttribute13,
								ReducedRecipientSchema.CustomAttribute14,
								ReducedRecipientSchema.CustomAttribute15,
								ReducedRecipientSchema.ExtensionCustomAttribute1,
								ReducedRecipientSchema.ExtensionCustomAttribute2,
								ReducedRecipientSchema.ExtensionCustomAttribute3,
								ReducedRecipientSchema.ExtensionCustomAttribute4,
								ReducedRecipientSchema.ExtensionCustomAttribute5,
								ReducedRecipientSchema.EmailAddresses,
								ReducedRecipientSchema.HiddenFromAddressListsEnabled,
								ADObjectSchema.Name,
								ADObjectSchema.WhenChanged,
								ReducedRecipientSchema.City,
								ReducedRecipientSchema.Company,
								ReducedRecipientSchema.CountryOrRegion,
								ReducedRecipientSchema.DatabaseName,
								ReducedRecipientSchema.ArchiveDatabase,
								ReducedRecipientSchema.Department,
								ReducedRecipientSchema.ExpansionServer,
								ReducedRecipientSchema.ExternalEmailAddress,
								ReducedRecipientSchema.ExternalDirectoryObjectId,
								ReducedRecipientSchema.FirstName,
								ReducedRecipientSchema.LastName,
								ReducedRecipientSchema.Office,
								ReducedRecipientSchema.Phone,
								ReducedRecipientSchema.PostalCode,
								ReducedRecipientSchema.StateOrProvince,
								ReducedRecipientSchema.Title,
								ReducedRecipientSchema.UMEnabled,
								ReducedRecipientSchema.UMMailboxPolicy,
								ReducedRecipientSchema.UMRecipientDialPlanId,
								ReducedRecipientSchema.ManagedFolderMailboxPolicy,
								ReducedRecipientSchema.ActiveSyncMailboxPolicy,
								ReducedRecipientSchema.OwaMailboxPolicy,
								ReducedRecipientSchema.AddressBookPolicy,
								ReducedRecipientSchema.HasActiveSyncDevicePartnership,
								ReducedRecipientSchema.SharingPolicy,
								ReducedRecipientSchema.ArchiveGuid,
								ReducedRecipientSchema.IsValidSecurityPrincipal,
								ReducedRecipientSchema.ArchiveState,
								ReducedRecipientSchema.MailboxMoveTargetMDB,
								ReducedRecipientSchema.MailboxMoveSourceMDB,
								ReducedRecipientSchema.MailboxMoveFlags,
								ReducedRecipientSchema.MailboxMoveRemoteHostName,
								ReducedRecipientSchema.MailboxMoveBatchName,
								ReducedRecipientSchema.MailboxMoveStatus,
								ReducedRecipientSchema.MailboxRelease,
								ReducedRecipientSchema.ArchiveRelease,
								ReducedRecipientSchema.RecipientType,
								ReducedRecipientSchema.RecipientTypeDetails,
								ADObjectSchema.Id,
								ADObjectSchema.ExchangeVersion,
								ADObjectSchema.OrganizationId
							}
						},
						{
							PropertySet.Minimum,
							new ADPropertyDefinition[]
							{
								ReducedRecipientSchema.RecipientType,
								ReducedRecipientSchema.RecipientTypeDetails,
								ADObjectSchema.Id,
								ADObjectSchema.ExchangeVersion,
								ADObjectSchema.OrganizationId
							}
						}
					};
				}
				return ReducedRecipient.properties;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId objectId = base.Identity;
				if (objectId is ADObjectId && SuppressingPiiContext.NeedPiiSuppression)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, objectId);
				}
				return objectId;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ReducedRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return null;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return Filters.DefaultRecipientFilter;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		internal override bool SkipPiiRedaction
		{
			get
			{
				return ADRecipient.IsSystemMailbox(this.RecipientTypeDetails);
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Alias];
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[ReducedRecipientSchema.ArchiveGuid];
			}
		}

		public AuthenticationType? AuthenticationType
		{
			get
			{
				return (AuthenticationType?)this[ReducedRecipientSchema.AuthenticationType];
			}
			internal set
			{
				this[ReducedRecipientSchema.AuthenticationType] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)this[ReducedRecipientSchema.City];
			}
		}

		public string Notes
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Notes];
			}
		}

		public string Company
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Company];
			}
		}

		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[ReducedRecipientSchema.CountryOrRegion];
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this[ReducedRecipientSchema.PostalCode];
			}
		}

		public string CustomAttribute1
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute1];
			}
		}

		public string CustomAttribute2
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute2];
			}
		}

		public string CustomAttribute3
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute3];
			}
		}

		public string CustomAttribute4
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute4];
			}
		}

		public string CustomAttribute5
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute5];
			}
		}

		public string CustomAttribute6
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute6];
			}
		}

		public string CustomAttribute7
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute7];
			}
		}

		public string CustomAttribute8
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute8];
			}
		}

		public string CustomAttribute9
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute9];
			}
		}

		public string CustomAttribute10
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute10];
			}
		}

		public string CustomAttribute11
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute11];
			}
		}

		public string CustomAttribute12
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute12];
			}
		}

		public string CustomAttribute13
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute13];
			}
		}

		public string CustomAttribute14
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute14];
			}
		}

		public string CustomAttribute15
		{
			get
			{
				return (string)this[ReducedRecipientSchema.CustomAttribute15];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.ExtensionCustomAttribute1];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.ExtensionCustomAttribute2];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.ExtensionCustomAttribute3];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.ExtensionCustomAttribute4];
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.ExtensionCustomAttribute5];
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.Database];
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.ArchiveDatabase];
			}
		}

		public string DatabaseName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.DatabaseName];
			}
		}

		public string Department
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Department];
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[ReducedRecipientSchema.ExternalDirectoryObjectId];
			}
		}

		public ADObjectId ManagedFolderMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.ManagedFolderMailboxPolicy];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ReducedRecipientSchema.EmailAddresses];
			}
		}

		public string ExpansionServer
		{
			get
			{
				return (string)this[ReducedRecipientSchema.ExpansionServer];
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[ReducedRecipientSchema.ExternalEmailAddress];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.DisplayName];
			}
			internal set
			{
				this[ReducedRecipientSchema.DisplayName] = value;
			}
		}

		public string FirstName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.FirstName];
			}
		}

		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.HiddenFromAddressListsEnabled];
			}
		}

		public bool EmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.EmailAddressPolicyEnabled];
			}
		}

		internal bool IsDirSynced
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.IsDirSynced];
			}
		}

		internal bool IsDirSyncEnabled
		{
			get
			{
				return ADObject.IsRecipientDirSynced(this.IsDirSynced);
			}
		}

		internal MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[ReducedRecipientSchema.DirSyncAuthorityMetadata];
			}
		}

		public string LastName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.LastName];
			}
		}

		public ExchangeResourceType? ResourceType
		{
			get
			{
				return (ExchangeResourceType?)this[ReducedRecipientSchema.ResourceType];
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ReducedRecipientSchema.ManagedBy];
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.Manager];
			}
		}

		public ADObjectId ActiveSyncMailboxPolicy
		{
			get
			{
				return this.activeSyncMailboxPolicy ?? ((ADObjectId)this[ReducedRecipientSchema.ActiveSyncMailboxPolicy]);
			}
			internal set
			{
				this.activeSyncMailboxPolicy = value;
			}
		}

		public bool ActiveSyncMailboxPolicyIsDefaulted
		{
			get
			{
				return (bool)(this[ReducedRecipientSchema.ActiveSyncMailboxPolicyIsDefaulted] ?? false);
			}
			internal set
			{
				this[ReducedRecipientSchema.ActiveSyncMailboxPolicyIsDefaulted] = value;
			}
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public string Office
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Office];
			}
		}

		public new ADObjectId ObjectCategory
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.ObjectCategory];
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[ReducedRecipientSchema.OrganizationalUnit];
			}
		}

		public string Phone
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Phone];
			}
		}

		public MultiValuedProperty<string> PoliciesIncluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.PoliciesIncluded];
			}
		}

		public MultiValuedProperty<string> PoliciesExcluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.PoliciesExcluded];
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ReducedRecipientSchema.PrimarySmtpAddress];
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[ReducedRecipientSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[ReducedRecipientSchema.RecipientTypeDetails];
			}
			internal set
			{
				this[ReducedRecipientSchema.RecipientTypeDetails] = value;
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.SamAccountName];
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[ReducedRecipientSchema.ServerLegacyDN];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.ServerName];
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)this[ReducedRecipientSchema.StateOrProvince];
			}
		}

		public string StorageGroupName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.StorageGroupName];
			}
		}

		public string Title
		{
			get
			{
				return (string)this[ReducedRecipientSchema.Title];
			}
		}

		public bool UMEnabled
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.UMEnabled];
			}
		}

		public ADObjectId UMMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.UMMailboxPolicy];
			}
		}

		public ADObjectId UMRecipientDialPlanId
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.UMRecipientDialPlanId];
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[ReducedRecipientSchema.WindowsLiveID];
			}
		}

		public bool HasActiveSyncDevicePartnership
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.HasActiveSyncDevicePartnership];
			}
		}

		public MultiValuedProperty<ADObjectId> AddressListMembership
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ReducedRecipientSchema.AddressListMembership];
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return this.owaMailboxPolicy ?? ((ADObjectId)this[ReducedRecipientSchema.OwaMailboxPolicy]);
			}
			internal set
			{
				this.owaMailboxPolicy = value;
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return this.addressBookPolicy ?? ((ADObjectId)this[ReducedRecipientSchema.AddressBookPolicy]);
			}
			internal set
			{
				this.addressBookPolicy = value;
			}
		}

		public ADObjectId SharingPolicy
		{
			get
			{
				return this.sharingPolicy ?? ((ADObjectId)this[ReducedRecipientSchema.SharingPolicy]);
			}
			internal set
			{
				this.sharingPolicy = value;
			}
		}

		public ADObjectId RetentionPolicy
		{
			get
			{
				return this.retentionPolicy ?? ((ADObjectId)this[ReducedRecipientSchema.RetentionPolicy]);
			}
			internal set
			{
				this.retentionPolicy = value;
			}
		}

		public bool ShouldUseDefaultRetentionPolicy
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.ShouldUseDefaultRetentionPolicy];
			}
			internal set
			{
				this[ReducedRecipientSchema.ShouldUseDefaultRetentionPolicy] = value;
			}
		}

		public ADObjectId MailboxMoveTargetMDB
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.MailboxMoveTargetMDB];
			}
		}

		public ADObjectId MailboxMoveSourceMDB
		{
			get
			{
				return (ADObjectId)this[ReducedRecipientSchema.MailboxMoveSourceMDB];
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[ReducedRecipientSchema.MailboxMoveFlags];
			}
		}

		public string MailboxMoveRemoteHostName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.MailboxMoveRemoteHostName];
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[ReducedRecipientSchema.MailboxMoveBatchName];
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[ReducedRecipientSchema.MailboxMoveStatus];
			}
		}

		public string MailboxRelease
		{
			get
			{
				return (string)this[ReducedRecipientSchema.MailboxRelease];
			}
		}

		public string ArchiveRelease
		{
			get
			{
				return (string)this[ReducedRecipientSchema.ArchiveRelease];
			}
		}

		public bool IsValidSecurityPrincipal
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.IsValidSecurityPrincipal];
			}
		}

		public bool LitigationHoldEnabled
		{
			get
			{
				return (bool)this[ReducedRecipientSchema.LitigationHoldEnabled];
			}
		}

		public MultiValuedProperty<Capability> Capabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[ReducedRecipientSchema.Capabilities];
			}
			private set
			{
				this[RoleGroupSchema.Capabilities] = value;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return (ArchiveState)this[ReducedRecipientSchema.ArchiveState];
			}
		}

		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[ReducedRecipientSchema.SKUAssigned];
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return (DateTime?)this[ReducedRecipientSchema.WhenMailboxCreated];
			}
		}

		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[ReducedRecipientSchema.UsageLocation];
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[ReducedRecipientSchema.ExchangeGuid];
			}
			internal set
			{
				this[ReducedRecipientSchema.ExchangeGuid] = value;
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[ReducedRecipientSchema.ArchiveStatus];
			}
			internal set
			{
				this[ReducedRecipientSchema.ArchiveStatus] = value;
			}
		}

		internal void PopulateCapabilitiesProperty()
		{
			this.Capabilities = CapabilityIdentifierEvaluatorFactory.GetCapabilities(this);
		}

		private static Dictionary<PropertySet, ADPropertyDefinition[]> properties;

		private static ReducedRecipientSchema schema = ObjectSchema.GetInstance<ReducedRecipientSchema>();

		private ADObjectId addressBookPolicy;

		private ADObjectId activeSyncMailboxPolicy;

		private ADObjectId owaMailboxPolicy;

		private ADObjectId sharingPolicy;

		private ADObjectId retentionPolicy;
	}
}
