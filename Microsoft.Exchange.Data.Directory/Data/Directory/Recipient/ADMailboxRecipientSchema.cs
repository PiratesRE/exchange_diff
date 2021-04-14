using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADMailboxRecipientSchema : ADRecipientSchema
	{
		public static readonly ADPropertyDefinition IsSecurityPrincipal = IADSecurityPrincipalSchema.IsSecurityPrincipal;

		public static readonly ADPropertyDefinition SamAccountName = IADSecurityPrincipalSchema.SamAccountName;

		public static readonly ADPropertyDefinition Sid = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = IADSecurityPrincipalSchema.SidHistory;

		public static readonly ADPropertyDefinition Database = IADMailStorageSchema.Database;

		public static readonly ADPropertyDefinition DelegateListLink = IADMailStorageSchema.DelegateListLink;

		public static readonly ADPropertyDefinition DelegateListBL = IADMailStorageSchema.DelegateListBL;

		public static readonly ADPropertyDefinition DeletedItemFlags = IADMailStorageSchema.DeletedItemFlags;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = IADMailStorageSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition ExchangeGuid = IADMailStorageSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptor = IADMailStorageSchema.ExchangeSecurityDescriptor;

		public static readonly ADPropertyDefinition ExternalOofOptions = IADMailStorageSchema.ExternalOofOptions;

		public static readonly ADPropertyDefinition RetainDeletedItemsFor = IADMailStorageSchema.RetainDeletedItemsFor;

		public static readonly ADPropertyDefinition IsMailboxEnabled = IADMailStorageSchema.IsMailboxEnabled;

		public static readonly ADPropertyDefinition IssueWarningQuota = IADMailStorageSchema.IssueWarningQuota;

		public static readonly ADPropertyDefinition OfflineAddressBook = IADMailStorageSchema.OfflineAddressBook;

		public static readonly ADPropertyDefinition ProhibitSendQuota = IADMailStorageSchema.ProhibitSendQuota;

		public static readonly ADPropertyDefinition ProhibitSendReceiveQuota = IADMailStorageSchema.ProhibitSendReceiveQuota;

		public static readonly ADPropertyDefinition RulesQuota = IADMailStorageSchema.RulesQuota;

		public static readonly ADPropertyDefinition ServerLegacyDN = IADMailStorageSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition ServerName = IADMailStorageSchema.ServerName;

		public static readonly ADPropertyDefinition SharePointResources = IADMailStorageSchema.SharePointResources;

		public static readonly ADPropertyDefinition SharePointUrl = IADMailStorageSchema.SharePointUrl;

		public static readonly ADPropertyDefinition UseDatabaseQuotaDefaults = IADMailStorageSchema.UseDatabaseQuotaDefaults;

		public static readonly ADPropertyDefinition YammerGroupAddress = new ADPropertyDefinition("YammerGroupAddress", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchFBURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition WhenMailboxCreated = new ADPropertyDefinition("WhenMailboxCreated", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), "msExchWhenMailboxCreated", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateWhenMailboxCreated))
		}, null, MbxRecipientSchema.WhenMailboxCreated);

		public static readonly ADPropertyDefinition PublicToGroupSids = new ADPropertyDefinition("PublicToGroupSids", ExchangeObjectVersion.Exchange2010, typeof(SecurityIdentifier), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMailboxRecipientSchema.DelegateListLink
		}, null, new GetterDelegate(GroupMailbox.PublicToGroupSidsGetter), null, null, null);

		public static readonly ADPropertyDefinition GroupMailboxSharePointSiteUrl = new ADPropertyDefinition("GroupMailboxSharePointSiteUrl", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMailboxRecipientSchema.SharePointResources
		}, null, new GetterDelegate(GroupMailbox.SharePointSiteUrlGetter), null, null, null);

		public static readonly ADPropertyDefinition GroupMailboxSharePointDocumentsUrl = new ADPropertyDefinition("GroupMailboxSharePointDocumentsUrl", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMailboxRecipientSchema.SharePointResources
		}, null, new GetterDelegate(GroupMailbox.SharePointDocumentsUrlGetter), null, null, null);
	}
}
