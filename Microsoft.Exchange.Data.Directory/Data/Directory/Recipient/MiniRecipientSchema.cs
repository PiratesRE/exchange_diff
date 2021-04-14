using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MiniRecipientSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition ArchiveDatabase = ADUserSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition ArchiveGuid = ADUserSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition ArchiveName = ADUserSchema.ArchiveName;

		public static readonly ADPropertyDefinition ArchiveState = ADUserSchema.ArchiveState;

		public static readonly ADPropertyDefinition JournalArchiveAddress = ADRecipientSchema.JournalArchiveAddress;

		public static readonly ADPropertyDefinition Database = IADMailStorageSchema.Database;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition ExchangeGuid = IADMailStorageSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition MailboxContainerGuid = IADMailStorageSchema.MailboxContainerGuid;

		public static readonly ADPropertyDefinition AggregatedMailboxGuids = IADMailStorageSchema.AggregatedMailboxGuids;

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptor = IADMailStorageSchema.ExchangeSecurityDescriptor;

		public static readonly ADPropertyDefinition ExternalEmailAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition GrantSendOnBehalfTo = ADRecipientSchema.GrantSendOnBehalfTo;

		public static readonly ADPropertyDefinition Languages = ADOrgPersonSchema.Languages;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition MasterAccountSid = ADRecipientSchema.MasterAccountSid;

		public static readonly ADPropertyDefinition MAPIEnabled = ADRecipientSchema.MAPIEnabled;

		public static readonly ADPropertyDefinition OWAEnabled = ADRecipientSchema.OWAEnabled;

		public static readonly ADPropertyDefinition MOWAEnabled = ADUserSchema.OWAforDevicesEnabled;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition QueryBaseDN = ADUserSchema.QueryBaseDN;

		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition IsResource = ADRecipientSchema.IsResource;

		public static readonly ADPropertyDefinition DefaultPublicFolderMailbox = ADRecipientSchema.DefaultPublicFolderMailbox;

		public static readonly ADPropertyDefinition ServerLegacyDN = IADMailStorageSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition Sid = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = IADSecurityPrincipalSchema.SidHistory;

		public static readonly ADPropertyDefinition IsPersonToPersonTextMessagingEnabled = ADRecipientSchema.IsPersonToPersonTextMessagingEnabled;

		public static readonly ADPropertyDefinition IsMachineToPersonTextMessagingEnabled = ADRecipientSchema.IsMachineToPersonTextMessagingEnabled;

		public static readonly ADPropertyDefinition OWAMailboxPolicy = ADUserSchema.OwaMailboxPolicy;

		public static readonly ADPropertyDefinition MobileDeviceMailboxPolicy = ADUserSchema.ActiveSyncMailboxPolicy;

		public static readonly ADPropertyDefinition AddressBookPolicy = ADRecipientSchema.AddressBookPolicy;

		public static readonly ADPropertyDefinition ThrottlingPolicy = ADRecipientSchema.ThrottlingPolicy;

		public static readonly ADPropertyDefinition UserPrincipalName = ADUserSchema.UserPrincipalName;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition NetID = ADUserSchema.NetID;

		public static readonly ADPropertyDefinition PersistedCapabilities = SharedPropertyDefinitions.PersistedCapabilities;

		public static readonly ADPropertyDefinition SKUAssigned = ADRecipientSchema.SKUAssigned;

		public static readonly ADPropertyDefinition SharePointUrl = IADMailStorageSchema.SharePointUrl;

		public static readonly ADPropertyDefinition WhenMailboxCreated = ADMailboxRecipientSchema.WhenMailboxCreated;

		public static readonly ADPropertyDefinition AuditEnabled = ADRecipientSchema.AuditEnabled;

		public static readonly ADPropertyDefinition AuditLogAgeLimit = ADRecipientSchema.AuditLogAgeLimit;

		public static readonly ADPropertyDefinition AuditAdminFlags = ADRecipientSchema.AuditAdminFlags;

		public static readonly ADPropertyDefinition AuditDelegateAdminFlags = ADRecipientSchema.AuditDelegateAdminFlags;

		public static readonly ADPropertyDefinition AuditDelegateFlags = ADRecipientSchema.AuditDelegateFlags;

		public static readonly ADPropertyDefinition AuditOwnerFlags = ADRecipientSchema.AuditOwnerFlags;

		public static readonly ADPropertyDefinition AuditBypassEnabled = ADRecipientSchema.AuditBypassEnabled;

		public static readonly ADPropertyDefinition AuditLastAdminAccess = ADRecipientSchema.AuditLastAdminAccess;

		public static readonly ADPropertyDefinition AuditLastDelegateAccess = ADRecipientSchema.AuditLastDelegateAccess;

		public static readonly ADPropertyDefinition AuditLastExternalAccess = ADRecipientSchema.AuditLastExternalAccess;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ReleaseTrack = ADRecipientSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition ConfigurationXML = ADRecipientSchema.ConfigurationXML;

		public static readonly ADPropertyDefinition ModernGroupType = ADRecipientSchema.ModernGroupType;

		public static readonly ADPropertyDefinition PublicToGroupSids = ADMailboxRecipientSchema.PublicToGroupSids;
	}
}
