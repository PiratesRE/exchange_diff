using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ReducedRecipientSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition ArchiveGuid = ADUserSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition AuthenticationType = ADRecipientSchema.AuthenticationType;

		public static readonly ADPropertyDefinition City = ADUserSchema.City;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition Company = ADUserSchema.Company;

		public static readonly ADPropertyDefinition CountryOrRegion = ADUserSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition PostalCode = ADUserSchema.PostalCode;

		public static readonly ADPropertyDefinition CustomAttribute1 = ADRecipientSchema.CustomAttribute1;

		public static readonly ADPropertyDefinition CustomAttribute2 = ADRecipientSchema.CustomAttribute2;

		public static readonly ADPropertyDefinition CustomAttribute3 = ADRecipientSchema.CustomAttribute3;

		public static readonly ADPropertyDefinition CustomAttribute4 = ADRecipientSchema.CustomAttribute4;

		public static readonly ADPropertyDefinition CustomAttribute5 = ADRecipientSchema.CustomAttribute5;

		public static readonly ADPropertyDefinition CustomAttribute6 = ADRecipientSchema.CustomAttribute6;

		public static readonly ADPropertyDefinition CustomAttribute7 = ADRecipientSchema.CustomAttribute7;

		public static readonly ADPropertyDefinition CustomAttribute8 = ADRecipientSchema.CustomAttribute8;

		public static readonly ADPropertyDefinition CustomAttribute9 = ADRecipientSchema.CustomAttribute9;

		public static readonly ADPropertyDefinition CustomAttribute10 = ADRecipientSchema.CustomAttribute10;

		public static readonly ADPropertyDefinition CustomAttribute11 = ADRecipientSchema.CustomAttribute11;

		public static readonly ADPropertyDefinition CustomAttribute12 = ADRecipientSchema.CustomAttribute12;

		public static readonly ADPropertyDefinition CustomAttribute13 = ADRecipientSchema.CustomAttribute13;

		public static readonly ADPropertyDefinition CustomAttribute14 = ADRecipientSchema.CustomAttribute14;

		public static readonly ADPropertyDefinition CustomAttribute15 = ADRecipientSchema.CustomAttribute15;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute1 = ADRecipientSchema.ExtensionCustomAttribute1;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute2 = ADRecipientSchema.ExtensionCustomAttribute2;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute3 = ADRecipientSchema.ExtensionCustomAttribute3;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute4 = ADRecipientSchema.ExtensionCustomAttribute4;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute5 = ADRecipientSchema.ExtensionCustomAttribute5;

		public static readonly ADPropertyDefinition Database = ADMailboxRecipientSchema.Database;

		public static readonly ADPropertyDefinition ArchiveDatabase = ADUserSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition Department = ADUserSchema.Department;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition ManagedFolderMailboxPolicy = ADUserSchema.ManagedFolderMailboxPolicy;

		public static readonly ADPropertyDefinition AddressListMembership = ADRecipientSchema.AddressListMembership;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ExpansionServer = ADGroupSchema.ExpansionServer;

		public static readonly ADPropertyDefinition ExternalEmailAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition FirstName = ADUserSchema.FirstName;

		public static readonly ADPropertyDefinition HiddenFromAddressListsEnabled = ADRecipientSchema.HiddenFromAddressListsEnabled;

		public static readonly ADPropertyDefinition IsDirSynced = ADRecipientSchema.IsDirSynced;

		public static readonly ADPropertyDefinition DirSyncAuthorityMetadata = ADRecipientSchema.DirSyncAuthorityMetadata;

		public static readonly ADPropertyDefinition LastName = ADUserSchema.LastName;

		public static readonly ADPropertyDefinition ResourceType = MailboxSchema.ResourceType;

		public static readonly ADPropertyDefinition ManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition Manager = ADUserSchema.Manager;

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicy = ADUserSchema.ActiveSyncMailboxPolicy;

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicyIsDefaulted = ADUserSchema.ActiveSyncMailboxPolicyIsDefaulted;

		public static readonly ADPropertyDefinition OwaMailboxPolicy = ADUserSchema.OwaMailboxPolicy;

		public static readonly ADPropertyDefinition AddressBookPolicy = ADRecipientSchema.AddressBookPolicy;

		public static readonly ADPropertyDefinition SharingPolicy = ADUserSchema.SharingPolicy;

		public static readonly ADPropertyDefinition Office = ADUserSchema.Office;

		public static readonly ADPropertyDefinition Phone = ADUserSchema.Phone;

		public static readonly ADPropertyDefinition PoliciesIncluded = ADRecipientSchema.PoliciesIncluded;

		public static readonly ADPropertyDefinition PoliciesExcluded = ADRecipientSchema.PoliciesExcluded;

		public static readonly ADPropertyDefinition UserPrincipalName = ADUserSchema.UserPrincipalName;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition ServerLegacyDN = ADMailboxRecipientSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition StateOrProvince = ADUserSchema.StateOrProvince;

		public static readonly ADPropertyDefinition Title = ADUserSchema.Title;

		public static readonly ADPropertyDefinition UMMailboxPolicy = ADUserSchema.UMMailboxPolicy;

		public static readonly ADPropertyDefinition UMRecipientDialPlanId = ADRecipientSchema.UMRecipientDialPlanId;

		public static readonly ADPropertyDefinition DatabaseName = IADMailStorageSchema.DatabaseName;

		public static readonly ADPropertyDefinition EmailAddressPolicyEnabled = ADRecipientSchema.EmailAddressPolicyEnabled;

		public static readonly ADPropertyDefinition OrganizationalUnit = ADRecipientSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition ServerName = ADMailboxRecipientSchema.ServerName;

		public static readonly ADPropertyDefinition StorageGroupName = ADUserSchema.StorageGroupName;

		public static readonly ADPropertyDefinition UMEnabled = ADUserSchema.UMEnabled;

		public static readonly ADPropertyDefinition HasActiveSyncDevicePartnership = ADUserSchema.HasActiveSyncDevicePartnership;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition Members = new ADPropertyDefinition("Members", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "member", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = ADUserSchema.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = ADUserSchema.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition MailboxMoveTargetArchiveMDB = ADUserSchema.MailboxMoveTargetArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceArchiveMDB = ADUserSchema.MailboxMoveSourceArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveFlags = ADUserSchema.MailboxMoveFlags;

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = ADUserSchema.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition MailboxMoveBatchName = ADUserSchema.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition MailboxMoveStatus = ADUserSchema.MailboxMoveStatus;

		public static readonly ADPropertyDefinition MailboxRelease = ADUserSchema.MailboxRelease;

		public static readonly ADPropertyDefinition ArchiveRelease = ADUserSchema.ArchiveRelease;

		public static readonly ADPropertyDefinition IsValidSecurityPrincipal = ADRecipientSchema.IsValidSecurityPrincipal;

		public static readonly ADPropertyDefinition RetentionPolicy = IADMailStorageSchema.RetentionPolicy;

		public static readonly ADPropertyDefinition ShouldUseDefaultRetentionPolicy = IADMailStorageSchema.ShouldUseDefaultRetentionPolicy;

		public static readonly ADPropertyDefinition LitigationHoldEnabled = IADMailStorageSchema.LitigationHoldEnabled;

		public static readonly ADPropertyDefinition ArchiveState = IADMailStorageSchema.ArchiveState;

		public static readonly ADPropertyDefinition RawCapabilities = SharedPropertyDefinitions.RawCapabilities;

		public static readonly ADPropertyDefinition Capabilities = SharedPropertyDefinitions.Capabilities;

		public static readonly ADPropertyDefinition SKUAssigned = ADRecipientSchema.SKUAssigned;

		public static readonly ADPropertyDefinition WhenMailboxCreated = ADMailboxRecipientSchema.WhenMailboxCreated;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition UsageLocation = ADRecipientSchema.UsageLocation;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition ArchiveStatus = ADUserSchema.ArchiveStatus;
	}
}
