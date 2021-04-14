using System;
using System.Net;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal abstract class IADMailStorageSchema
	{
		public static readonly ADPropertyDefinition RecipientTypeDetailsValue = new ADPropertyDefinition("RecipientTypeDetailsValue", ExchangeObjectVersion.Exchange2003, typeof(RecipientTypeDetails), "msExchRecipientTypeDetails", ADPropertyDefinitionFlags.None, RecipientTypeDetails.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PreviousRecipientTypeDetails = new ADPropertyDefinition("PreviousRecipientTypeDetails", ExchangeObjectVersion.Exchange2003, typeof(RecipientTypeDetails), "msExchPreviousRecipientTypeDetails", ADPropertyDefinitionFlags.None, RecipientTypeDetails.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.PreviousRecipientTypeDetails);

		public static readonly ADPropertyDefinition Database = new ADPropertyDefinition("Database", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "homeMDB", null, "msExchHomeMDBSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, MServRecipientSchema.Database, null);

		public static readonly ADPropertyDefinition PreviousDatabase = new ADPropertyDefinition("PreviousDatabase", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchPreviousHomeMDB", null, "msExchPreviousHomeMDBSL", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DeletedItemFlags = new ADPropertyDefinition("DeletedItemFlags", ExchangeObjectVersion.Exchange2003, typeof(DeletedItemRetention), "deletedItemFlags", ADPropertyDefinitionFlags.None, DeletedItemRetention.DatabaseDefault, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DeletedItemRetention))
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = new ADPropertyDefinition("DeliverToMailboxAndForward", ExchangeObjectVersion.Exchange2003, typeof(bool), "deliverAndRedirect", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.DeliverToMailboxAndForward);

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEndDate = new ADPropertyDefinition("ElcExpirationSuspensionEndDate", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), "msExchELCExpirySuspensionEnd", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ElcExpirationSuspensionStartDate = new ADPropertyDefinition("ElcExpirationSuspensionStartDate", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), "msExchELCExpirySuspensionStart", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ElcMailboxFlags = new ADPropertyDefinition("ElcMailboxFlags", ExchangeObjectVersion.Exchange2007, typeof(ElcMailboxFlags), "msExchELCMailboxFlags", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.ElcMailboxFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition RetentionComment = new ADPropertyDefinition("RetentionComment", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchRetentionComment", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition RetentionUrl = new ADPropertyDefinition("RetentionUrl", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchRetentionURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 2048)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ElcPolicyTemplate = new ADPropertyDefinition("ElcPolicyTemplate", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchMailboxTemplateLink", ADPropertyDefinitionFlags.ValidateInSharedConfig, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ExchangeGuid = new ADPropertyDefinition("ExchangeGuid", ExchangeObjectVersion.Exchange2003, typeof(Guid), "msExchMailboxGuid", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, MServRecipientSchema.ExchangeGuid, null);

		public static readonly ADPropertyDefinition MailboxContainerGuid = new ADPropertyDefinition("MailboxContainerGuid", ExchangeObjectVersion.Exchange2012, typeof(Guid?), "msExchMailboxContainerGuid", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.MailboxContainerGuid);

		public static readonly ADPropertyDefinition AggregatedMailboxGuidsRaw = new ADPropertyDefinition("AggregatedMailboxGuidsRaw", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAlternateMailboxes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AggregatedMailboxGuids = new ADPropertyDefinition("AggregatedMailboxGuids", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchAlternateMailboxes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.AggregatedMailboxGuidsRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), ADPropertyDefinition.RawStringGetterIgnoringInvalid<Guid>(IADMailStorageSchema.AggregatedMailboxGuidsRaw, IADMailStorageSchema.AggregatedMailboxGuids), ADPropertyDefinition.RawStringSetter<Guid>(IADMailStorageSchema.AggregatedMailboxGuidsRaw), null, null);

		public static readonly ADPropertyDefinition UnifiedMailbox = new ADPropertyDefinition("UnifiedMailboxAccount", ExchangeObjectVersion.Exchange2012, typeof(CrossTenantObjectId), "msExchUnifiedMailbox", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition PreviousExchangeGuid = new ADPropertyDefinition("PreviousExchangeGuid", ExchangeObjectVersion.Exchange2003, typeof(Guid), "msExchPreviousMailboxGuid", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.PreviousExchangeGuid);

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptorRaw = new ADPropertyDefinition("ExchangeSecurityDescriptorRaw", ExchangeObjectVersion.Exchange2003, typeof(RawSecurityDescriptor), "msExchMailboxSecurityDescriptor", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptor = new ADPropertyDefinition("ExchangeSecurityDescriptor", ExchangeObjectVersion.Exchange2003, typeof(RawSecurityDescriptor), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ExchangeSecurityDescriptorRaw
		}, null, new GetterDelegate(ADUser.ExchangeSecurityDescriptorGetter), new SetterDelegate(ADUser.ExchangeSecurityDescriptorSetter), null, null);

		public static readonly ADPropertyDefinition ExternalOofOptions = new ADPropertyDefinition("ExternalOofOptions", ExchangeObjectVersion.Exchange2007, typeof(ExternalOofOptions), "msExchExternalOOFOptions", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.ExternalOofOptions.External, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ExternalOofOptions))
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.ExternalOofOptions);

		public static readonly ADPropertyDefinition OfflineAddressBook = new ADPropertyDefinition("OfflineAddressBook", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchUseOAB", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ServerLegacyDN = new ADPropertyDefinition("ServerLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchHomeServerName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UseDatabaseQuotaDefaults = new ADPropertyDefinition("UseDatabaseQuotaDefaults", ExchangeObjectVersion.Exchange2003, typeof(bool?), "mDBUseDefaults", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition RetainDeletedItemsFor = new ADPropertyDefinition("RetainDeletedItemsFor", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "garbageCollPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(14.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.RetainDeletedItemsFor);

		public static readonly ADPropertyDefinition RulesQuota = new ADPropertyDefinition("RulesQuota", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchMDBRulesQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(64UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(32UL), ByteQuantifiedSize.FromKB(256UL))
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.RulesQuota);

		public static readonly ADPropertyDefinition ApprovalApplications = new ADPropertyDefinition("ApprovalApplications", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchApprovalApplicationLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ProhibitSendQuota = new ADPropertyDefinition("ProhibitSendQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBOverQuotaLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ProhibitSendQuota);

		public static readonly ADPropertyDefinition ProhibitSendReceiveQuota = new ADPropertyDefinition("ProhibitSendReceiveQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBOverHardQuotaLimit", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ProhibitSendReceiveQuota);

		public static readonly ADPropertyDefinition IssueWarningQuota = new ADPropertyDefinition("IssueWarningQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBStorageQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.IssueWarningQuota);

		public static readonly ADPropertyDefinition RecoverableItemsQuota = new ADPropertyDefinition("RecoverableItemsQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchDumpsterQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.RecoverableItemsQuota);

		public static readonly ADPropertyDefinition RecoverableItemsWarningQuota = new ADPropertyDefinition("RecoverableItemsWarningQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchDumpsterWarningQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.RecoverableItemsWarningQuota);

		public static readonly ADPropertyDefinition CalendarLoggingQuota = new ADPropertyDefinition("CalendarLoggingQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchCalendarLoggingQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.CalendarLoggingQuota);

		public static readonly ADPropertyDefinition RemoteRecipientType = new ADPropertyDefinition("RemoteRecipientType", ExchangeObjectVersion.Exchange2003, typeof(RemoteRecipientType), "msExchRemoteRecipientType", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.RemoteRecipientType.None, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateRemoteRecipientType))
		}, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateRemoteRecipientType))
		}, null, MbxRecipientSchema.RemoteRecipientType);

		public static readonly ADPropertyDefinition ArchiveDatabaseRaw = new ADPropertyDefinition("ArchiveDatabaseRaw", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, "msExchArchiveDatabaseLink", null, "msExchArchiveDatabaseLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ArchiveGuid = new ADPropertyDefinition("ArchiveGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), "msExchArchiveGUID", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveGuid);

		public static readonly ADPropertyDefinition ArchiveName = new ADPropertyDefinition("ArchiveName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchArchiveName", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 512)
		}, null, MbxRecipientSchema.ArchiveName);

		public static readonly ADPropertyDefinition ArchiveQuota = new ADPropertyDefinition("ArchiveQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchArchiveQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveQuota);

		public static readonly ADPropertyDefinition ArchiveWarningQuota = new ADPropertyDefinition("ArchiveWarningQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchArchiveWarnQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveWarningQuota);

		public static readonly ADPropertyDefinition ArchiveDomain = new ADPropertyDefinition("ArchiveDomain", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomain), "msExchArchiveAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveDomain);

		public static readonly ADPropertyDefinition ArchiveStatus = new ADPropertyDefinition("ArchiveStatus", ExchangeObjectVersion.Exchange2010, typeof(ArchiveStatusFlags), "msExchArchiveStatus", ADPropertyDefinitionFlags.None, ArchiveStatusFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveStatus);

		public static readonly ADPropertyDefinition ArchiveState = new ADPropertyDefinition("ArchiveState", ExchangeObjectVersion.Exchange2010, typeof(ArchiveState), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.Recipient.ArchiveState.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ArchiveGuid,
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ArchiveDatabaseRaw,
			IADMailStorageSchema.ArchiveDomain,
			IADMailStorageSchema.Database,
			IADMailStorageSchema.ArchiveStatus,
			IADMailStorageSchema.RemoteRecipientType,
			IADMailStorageSchema.RecipientTypeDetailsValue
		}, new CustomFilterBuilderDelegate(ADRecipient.ArchiveStateFilterBuilder), new GetterDelegate(ADUser.ArchiveStateGetter), null, null, null);

		public static readonly ADPropertyDefinition DisabledArchiveDatabase = new ADPropertyDefinition("DisabledArchiveDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, "msExchDisabledArchiveDatabaseLink", null, "msExchDisabledArchiveDatabaseLinkSL", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DisabledArchiveGuid = new ADPropertyDefinition("DisabledArchiveGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), "msExchDisabledArchiveGUID", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.DisabledArchiveGuid);

		public static readonly ADPropertyDefinition ArchiveDatabase = new ADPropertyDefinition("ArchiveDatabase", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ArchiveDatabaseRaw,
			IADMailStorageSchema.ArchiveDomain,
			IADMailStorageSchema.Database,
			IADMailStorageSchema.ArchiveGuid
		}, new CustomFilterBuilderDelegate(ADRecipient.ArchiveDatabaseFilterBuilder), new GetterDelegate(ADRecipient.ArchiveDatabaseGetter), new SetterDelegate(ADRecipient.ArchiveDatabaseSetter), null, null);

		public static readonly ADPropertyDefinition IsAuxMailbox = new ADPropertyDefinition("IsAuxMailbox", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 131072), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 131072), null, null);

		public static readonly ADPropertyDefinition AuxMailboxParentObjectId = new ADPropertyDefinition("AuxMailboxParentObjectId", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAuxMailboxParentObjectIdLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuxMailboxParentObjectIdBL = new ADPropertyDefinition("AuxMailboxParentObjectIdBL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAuxMailboxParentObjectIdBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxRelationType = new ADPropertyDefinition("MailboxRelationType", ExchangeObjectVersion.Exchange2010, typeof(MailboxRelationType), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.Recipient.MailboxRelationType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.AuxMailboxParentObjectIdBL,
			IADMailStorageSchema.AuxMailboxParentObjectId
		}, null, new GetterDelegate(ADRecipient.MailboxRelationTypeGetter), null, null, null);

		public static readonly ADPropertyDefinition SharingPolicy = new ADPropertyDefinition("SharingPolicy", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchSharingPolicyLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteAccountPolicy = new ADPropertyDefinition("RemoteAccountPolicy", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchSyncAccountsPolicyDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharingPartnerIdentitiesRaw = new ADPropertyDefinition("SharingPartnerIdentitiesRaw", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchSharingPartnerIdentities", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharingPartnerIdentities = new ADPropertyDefinition("SharingPartnerIdentities", ExchangeObjectVersion.Exchange2010, typeof(SharingPartnerIdentityCollection), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.SharingPartnerIdentitiesRaw
		}, null, new GetterDelegate(ADUser.SharingPartnerIdentitiesGetter), new SetterDelegate(ADUser.SharingPartnerIdentitiesSetter), null, null);

		public static readonly ADPropertyDefinition SharingAnonymousIdentitiesRaw = new ADPropertyDefinition("SharingAnonymousIdentitiesRaw", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchSharingAnonymousIdentities", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharingAnonymousIdentities = new ADPropertyDefinition("SharingAnonymousIdentities", ExchangeObjectVersion.Exchange2010, typeof(SharingAnonymousIdentityCollection), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.SharingAnonymousIdentitiesRaw
		}, null, new GetterDelegate(ADUser.SharingAnonymousIdentitiesGetter), new SetterDelegate(ADUser.SharingAnonymousIdentitiesSetter), null, null);

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = SharedPropertyDefinitions.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = SharedPropertyDefinitions.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition MailboxMoveTargetArchiveMDB = new ADPropertyDefinition("MailboxMoveTargetArchiveMDB", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchMailboxMoveTargetArchiveMDBLink", null, "msExchMailboxMoveTargetArchiveMDBLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MailboxMoveSourceArchiveMDB = new ADPropertyDefinition("MailboxMoveSourceArchiveMDB", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchMailboxMoveSourceArchiveMDBLink", null, "msExchMailboxMoveSourceArchiveMDBLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MailboxMoveFlags = SharedPropertyDefinitions.MailboxMoveFlags;

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = SharedPropertyDefinitions.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition MailboxMoveBatchName = SharedPropertyDefinitions.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition MailboxMoveStatus = SharedPropertyDefinitions.MailboxMoveStatus;

		public static readonly ADPropertyDefinition MailboxRelease = SharedPropertyDefinitions.MailboxRelease;

		public static readonly ADPropertyDefinition ArchiveRelease = SharedPropertyDefinitions.ArchiveRelease;

		public static readonly ADPropertyDefinition TeamMailboxClosedTime = new ADPropertyDefinition("TeamMailboxExpiration", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchTeamMailboxExpiration", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharePointUrl = new ADPropertyDefinition("SharePointUrl", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchTeamMailboxSharePointUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition SharePointSiteInfo = new ADPropertyDefinition("SharePointSiteInfo", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailboxUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharePointResources = new ADPropertyDefinition("SharePointResources", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchLabeledURI", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SharePointLinkedBy = new ADPropertyDefinition("SharePointLinkedBy", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchTeamMailboxSharePointLinkedBy", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Owners = new ADPropertyDefinition("Owners", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchTeamMailboxOwners", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TeamMailboxShowInClientList = new ADPropertyDefinition("TeamMailboxShowInClientList", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchTeamMailboxShowInClientList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DelegateListLink = new ADPropertyDefinition("DelegateListLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDelegateListLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DelegateListBL = new ADPropertyDefinition("DelegateListBL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDelegateListBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LitigationHoldDate = new ADPropertyDefinition("LitigationHoldDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchLitigationHoldDate", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition LitigationHoldOwner = new ADPropertyDefinition("LitigationHoldOwner", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchLitigationHoldOwner", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxGuidsRaw = new ADPropertyDefinition("MailboxGuidsRaw", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchMultiMailboxGUIDs", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxLocationsRaw = new ADPropertyDefinition("MailboxLocationsRaw", ExchangeObjectVersion.Exchange2012, typeof(ADObjectIdWithString), "msExchMultiMailboxLocationsLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxLocations = new ADPropertyDefinition("MailboxLocations", ExchangeObjectVersion.Exchange2012, typeof(IMailboxLocationCollection), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ArchiveDatabaseRaw,
			IADMailStorageSchema.ArchiveDomain,
			IADMailStorageSchema.ArchiveGuid,
			IADMailStorageSchema.DisabledArchiveGuid,
			IADMailStorageSchema.ExchangeGuid,
			IADMailStorageSchema.Database,
			IADMailStorageSchema.MailboxLocationsRaw,
			IADMailStorageSchema.MailboxGuidsRaw
		}, null, new GetterDelegate(ADRecipient.MultiMailboxLocationsGetter), new SetterDelegate(ADRecipient.MultiMailboxLocationsSetter), null, null);

		public static readonly ADPropertyDefinition DatabaseAndLocation = new ADPropertyDefinition("DatabaseAndLocation", ExchangeObjectVersion.Exchange2010, typeof(object), null, ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SatchmoClusterIp = new ADPropertyDefinition("SatchmoClusterIp", ExchangeObjectVersion.Exchange2010, typeof(IPAddress), null, ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.NonADProperty, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, MServRecipientSchema.SatchmoClusterIp, null);

		public static readonly ADPropertyDefinition SatchmoDGroup = new ADPropertyDefinition("SatchmoDGroup", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, MServRecipientSchema.SatchmoDGroup, null);

		public static readonly ADPropertyDefinition PrimaryMailboxSource = new ADPropertyDefinition("PrimaryMailboxSource", ExchangeObjectVersion.Exchange2010, typeof(PrimaryMailboxSourceType), null, ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.NonADProperty, PrimaryMailboxSourceType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, MServRecipientSchema.PrimaryMailboxSource, null);

		public static readonly ADPropertyDefinition FblEnabled = new ADPropertyDefinition("FblEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.NonADProperty, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, MServRecipientSchema.FblEnabled, null);

		public static readonly ADPropertyDefinition DatabaseName = new ADPropertyDefinition("DatabaseName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.Database
		}, null, new GetterDelegate(ADUser.DatabaseNameGetter), null, null, null);

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEnabled = new ADPropertyDefinition("ElcExpirationSuspensionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ElcExpirationSuspensionEndDate,
			IADMailStorageSchema.ElcExpirationSuspensionStartDate
		}, null, new GetterDelegate(ADUser.ElcExpirationSuspensionEnabledGetter), new SetterDelegate(ADUser.ElcExpirationSuspensionEnabledSetter), null, null);

		public static readonly ADPropertyDefinition LitigationHoldEnabled = new ADPropertyDefinition("LitigationHoldEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags
		}, new CustomFilterBuilderDelegate(ADRecipient.LitigationHoldFilterBuilder), new GetterDelegate(ADUser.LitigationHoldEnabledGetter), new SetterDelegate(ADUser.LitigationHoldEnabledSetter), null, null);

		public static readonly ADPropertyDefinition SingleItemRecoveryEnabled = new ADPropertyDefinition("SingleItemRecoveryEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags
		}, new CustomFilterBuilderDelegate(ADRecipient.SingleItemRecoveryFilterBuilder), new GetterDelegate(ADUser.SingleItemRecoveryEnabledGetter), new SetterDelegate(ADUser.SingleItemRecoveryEnabledSetter), null, MbxRecipientSchema.SingleItemRecoveryEnabled);

		public static readonly ADPropertyDefinition CalendarVersionStoreDisabled = new ADPropertyDefinition("CalendarVersionStoreDisabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags
		}, null, new GetterDelegate(ADUser.CalendarVersionStoreDisabledGetter), new SetterDelegate(ADUser.CalendarVersionStoreDisabledSetter), null, MbxRecipientSchema.CalendarVersionStoreDisabled);

		public static readonly ADPropertyDefinition SiteMailboxMessageDedupEnabled = new ADPropertyDefinition("SiteMailboxMessageDedupEnabled", ExchangeObjectVersion.Exchange2012, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags
		}, null, new GetterDelegate(ADUser.SiteMailboxMessageDedupEnabledGetter), new SetterDelegate(ADUser.SiteMailboxMessageDedupEnabledSetter), null, null);

		public static readonly ADPropertyDefinition IsMailboxEnabled = new ADPropertyDefinition("IsMailboxEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.Database
		}, new CustomFilterBuilderDelegate(ADUser.IsMailboxEnabledFilterBuilder), new GetterDelegate(ADUser.IsMailboxEnabledGetter), null, null, null);

		public static readonly ADPropertyDefinition ServerName = new ADPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ServerLegacyDN
		}, new CustomFilterBuilderDelegate(ADUser.ServerNameFilterBuilder), new GetterDelegate(ADUser.ServerNameGetter), null, null, null);

		public static readonly ADPropertyDefinition StorageGroupName = new ADPropertyDefinition("StorageGroupName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.Database
		}, null, new GetterDelegate(ADUser.StorageGroupNameGetter), null, null, null);

		public static readonly ADPropertyDefinition UseDatabaseRetentionDefaults = new ADPropertyDefinition("UseDatabaseRetentionDefaults", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.DeletedItemFlags
		}, null, new GetterDelegate(ADUser.UseDatabaseRetentionDefaultsGetter), new SetterDelegate(ADUser.UseDatabaseRetentionDefaultsSetter), null, null);

		public static readonly ADPropertyDefinition RetainDeletedItemsUntilBackup = new ADPropertyDefinition("RetainDeletedItemsUntilBackup", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.DeletedItemFlags
		}, null, new GetterDelegate(ADUser.RetainDeletedItemsUntilBackupGetter), new SetterDelegate(ADUser.RetainDeletedItemsUntilBackupSetter), null, MbxRecipientSchema.RetainDeletedItemsUntilBackup);

		public static readonly ADPropertyDefinition ManagedFolderMailboxPolicy = new ADPropertyDefinition("ManagedFolderMailboxPolicy", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ElcPolicyTemplate
		}, new CustomFilterBuilderDelegate(ADRecipient.ManagedFolderFilterBuilder), new GetterDelegate(ADUser.ManagedFolderMailboxPolicyGetter), new SetterDelegate(ADUser.ManagedFolderMailboxPolicySetter), null, null);

		public static readonly ADPropertyDefinition RetentionPolicy = new ADPropertyDefinition("RetentionPolicy", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInSharedConfig, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags,
			IADMailStorageSchema.ElcPolicyTemplate
		}, new CustomFilterBuilderDelegate(ADRecipient.RetentionPolicyFilterBuilder), new GetterDelegate(ADUser.RetentionPolicyGetter), new SetterDelegate(ADUser.RetentionPolicySetter), null, null);

		public static readonly ADPropertyDefinition ShouldUseDefaultRetentionPolicy = new ADPropertyDefinition("ShouldUseDefaultRetentionPolicy", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IADMailStorageSchema.ElcMailboxFlags
		}, null, new GetterDelegate(ADRecipient.ShouldUseDefaultRetentionPolicyGetter), new SetterDelegate(ADRecipient.ShouldUseDefaultRetentionPolicySetter), null, MbxRecipientSchema.ShouldUseDefaultRetentionPolicy);
	}
}
