using System;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class SharedPropertyDefinitions
	{
		internal static MultiValuedProperty<Capability> PersistedCapabilitiesGetter(IPropertyBag propertyBag)
		{
			return (MultiValuedProperty<Capability>)propertyBag[SharedPropertyDefinitions.RawCapabilities];
		}

		internal static void PersistedCapabilitiesSetter(object capabilitiesValue, IPropertyBag propertyBag)
		{
			propertyBag[SharedPropertyDefinitions.RawCapabilities] = capabilitiesValue;
		}

		internal static QueryFilter CapabilitiesFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, SharedPropertyDefinitions.RawCapabilities, comparisonFilter.PropertyValue);
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
		}

		internal static QueryFilter ProvisioningFlagsFilterBuilder(DatabaseProvisioningFlags provisioningFlag, SinglePropertyFilter filter)
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
			QueryFilter queryFilter = new BitMaskAndFilter(SharedPropertyDefinitions.ProvisioningFlags, (ulong)((long)provisioningFlag));
			if ((comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && (bool)comparisonFilter.PropertyValue) || (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator && !(bool)comparisonFilter.PropertyValue))
			{
				return queryFilter;
			}
			return new NotFilter(queryFilter);
		}

		internal static QueryFilter IsOutOfServiceFilterBuilder(SinglePropertyFilter filter)
		{
			return SharedPropertyDefinitions.ProvisioningFlagsFilterBuilder(DatabaseProvisioningFlags.IsOutOfService, filter);
		}

		public const string PrintableStringValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\"'()+,-./:? ";

		internal static readonly ADPropertyDefinition ADAllowedFileTypes = new ADPropertyDefinition("ADAllowedFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWAAllowedFileTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADAllowedMimeTypes = new ADPropertyDefinition("ADAllowedMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWAAllowedMimeTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADBlockedFileTypes = new ADPropertyDefinition("ADBlockedFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWABlockedFileTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADBlockedMimeTypes = new ADPropertyDefinition("ADBlockedMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWABlockedMimeTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADForceSaveFileTypes = new ADPropertyDefinition("ADForceSaveFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWAForceSaveFileTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADForceSaveMimeTypes = new ADPropertyDefinition("ADForceSaveMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWAForceSaveMimeTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADWebReadyFileTypes = new ADPropertyDefinition("ADWebReadyFileTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWATranscodingFileTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ADWebReadyMimeTypes = new ADPropertyDefinition("ADWebReadyMimeTypes", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOWATranscodingMimeTypes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedInCountryOrRegionGroups = new ADPropertyDefinition("AllowedInCountryOrRegionGroups", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMAllowedInCountryGroups", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedInternationalGroups = new ADPropertyDefinition("AllowedInternationalGroups", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMAllowedInternationalGroups", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BypassedRecipients = new ADPropertyDefinition("BypassedRecipients", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress), "msExchMessageHygieneBypassedRecipient", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320),
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition RawCapabilities = new ADPropertyDefinition("RawCapabilities", ExchangeObjectVersion.Exchange2003, typeof(Capability), "msExchCapabilityIdentifiers", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Capabilities = new ADPropertyDefinition("Capabilities", ExchangeObjectVersion.Exchange2003, typeof(Capability), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2007, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 255)
		}, null, null);

		public static readonly ADPropertyDefinition ContactAddressLists = new ADPropertyDefinition("ContactAddressLists", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMQueryBaseDN", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Cookie = new ADPropertyDefinition("Cookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition CopyEdbFilePath = new ADPropertyDefinition("CopyEdbFilePath", ExchangeObjectVersion.Exchange2007, typeof(EdbFilePath), "msExchCopyEDBFile", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new AsciiCharactersOnlyConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2003, typeof(string), "description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition RawDescription = new ADPropertyDefinition("RawDescription", ExchangeObjectVersion.Exchange2003, typeof(string), "description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EdbFilePath = new ADPropertyDefinition("EdbFilePath", ExchangeObjectVersion.Exchange2003, typeof(EdbFilePath), "msExchEDBFile", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition EdgeSyncCookies = new ADPropertyDefinition("EdgeSyncCookies", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchEdgeSyncCookies", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ElcFlags = new ADPropertyDefinition("ELCFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchELCFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EndOfList = new ADPropertyDefinition("EndOfList", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyDN = new ADPropertyDefinition("ExchangeLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, new PropertyDefinitionConstraint[]
		{
			new ValidLegacyDNConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InfoAnnouncementFilename = new ADPropertyDefinition("InfoAnnouncementFilename", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMInfoAnnouncementFile", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, null, null);

		public static readonly ADPropertyDefinition JournalRecipient = new ADPropertyDefinition("JournalRecipient", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchMessageJournalRecipient", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LastUpdatedRecipientFilter = new ADPropertyDefinition("LastUpdatedRecipientFilter", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchLastAppliedRecipientFilter", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LdapRecipientFilter = new ADPropertyDefinition("LdapRecipientFilter", ExchangeObjectVersion.Exchange2003, typeof(string), "purportedSearch", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LegacyExchangeDN = new ADPropertyDefinition("LegacyExchangeDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LocalizedComment = new ADPropertyDefinition("LocalizedComment", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchELCAdminDescriptionLocalized", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = new ADPropertyDefinition("MailboxMoveTargetMDB", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchMailboxMoveTargetMDBLink", null, "msExchMailboxMoveTargetMDBLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = new ADPropertyDefinition("MailboxMoveSourceMDB", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchMailboxMoveSourceMDBLink", null, "msExchMailboxMoveSourceMDBLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MailboxMoveFlags = new ADPropertyDefinition("MailboxMoveFlags", ExchangeObjectVersion.Exchange2003, typeof(RequestFlags), "msExchMailboxMoveFlags", ADPropertyDefinitionFlags.None, RequestFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = new ADPropertyDefinition("MailboxMoveRemoteHostName", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchMailboxMoveRemoteHostName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MailboxMoveRemoteHostName);

		public static readonly ADPropertyDefinition MailboxMoveBatchName = new ADPropertyDefinition("MailboxMoveBatchName", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchMailboxMoveBatchName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MailboxMoveBatchName);

		public static readonly ADPropertyDefinition MailboxMoveStatus = new ADPropertyDefinition("MailboxMoveStatus", ExchangeObjectVersion.Exchange2003, typeof(RequestStatus), "msExchMailboxMoveStatus", ADPropertyDefinitionFlags.None, RequestStatus.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MailboxMoveStatus);

		public static readonly ADPropertyDefinition MailboxRelease = new ADPropertyDefinition("MailboxRelease", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchMailboxRelease", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MailboxRelease);

		public static readonly ADPropertyDefinition ArchiveRelease = new ADPropertyDefinition("ArchiveRelease", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchArchiveRelease", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.ArchiveRelease);

		public static readonly ADPropertyDefinition MailboxPublicFolderDatabase = new ADPropertyDefinition("PublicFolderDatabase", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchHomePublicMDB", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaintenanceScheduleBitmaps = new ADPropertyDefinition("MaintenanceScheduleBitmaps", ExchangeObjectVersion.Exchange2003, typeof(Schedule), "activationSchedule", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ManagedObjects = new ADPropertyDefinition("ManagedObjects", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "managedObjects", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MandatoryDisplayName = new ADPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "displayName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 256)
		}, null, null);

		public static readonly ADPropertyDefinition OfflineAddressBook = new ADPropertyDefinition("OfflineAddressBook", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchUseOAB", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OptionalDisplayName = new ADPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "displayName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgLeadersBL = new ADPropertyDefinition("OrgLeadersBL", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msOrg-LeadersBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OriginalDatabase = new ADPropertyDefinition("OriginalDatabase", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchOrigMDB", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OtherWellKnownObjects = new ADPropertyDefinition("OtherWellKnownObjects", ExchangeObjectVersion.Exchange2003, typeof(DNWithBinary), "otherWellKnownObjects", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublicFolderDefaultAdminAcl = new ADPropertyDefinition("PublicFolderDefaultAdminAcl", ExchangeObjectVersion.Exchange2003, typeof(RawSecurityDescriptor), "msExchPFDefaultAdminACL", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublicFolderHierarchy = new ADPropertyDefinition("PublicFolderHierarchy", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchOwningPFTree", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PurportedSearchUI = new ADPropertyDefinition("PurportedSearchUI", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchPurportedSearchUI", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QuotaNotificationScheduleBitmaps = new ADPropertyDefinition("QuotaNotificationScheduleBitmaps", ExchangeObjectVersion.Exchange2003, typeof(Schedule), "quotaNotificationSchedule", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientContainer = new ADPropertyDefinition("RecipientContainer", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchSearchBase", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientFilter = new ADPropertyDefinition("RecipientFilter", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchQueryFilter", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientFilterMetadata = new ADPropertyDefinition("RecipientFilterMetadata", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchQueryFilterMetadata", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationScheduleBitmaps = new ADPropertyDefinition("ReplicationScheduleBitmaps", ExchangeObjectVersion.Exchange2003, typeof(Schedule), "msExchReplicationSchedule", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.Binary, Schedule.Always, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchOwningServer", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SimpleDisplayName = new ADPropertyDefinition("SimpleDisplayName", ExchangeObjectVersion.Exchange2003, typeof(string), "displayNamePrintable", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256),
			new CharacterConstraint("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\"'()+,-./:? ".ToCharArray(), true)
		}, null, MbxRecipientSchema.SimpleDisplayName);

		public static readonly ADPropertyDefinition SitePublicFolderDatabase = new ADPropertyDefinition("PublicFolderDatabase", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "siteFolderServer", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UPNSuffixes = new ADPropertyDefinition("UPNSuffixes", ExchangeObjectVersion.Exchange2003, typeof(string), "uPNSuffixes", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProvisioningFlags = new ADPropertyDefinition("ProvisioningFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchProvisioningFlags", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsOutOfService = new ADPropertyDefinition("IsOutOfService", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(SharedPropertyDefinitions.IsOutOfServiceFilterBuilder), ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 8), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 8), null, null);

		public static readonly ADPropertyDefinition PersistedCapabilities = new ADPropertyDefinition("PersistedCapabilities", ExchangeObjectVersion.Exchange2003, typeof(Capability), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new CollectionDelegateConstraint(new CollectionValidationDelegate(ConstraintDelegates.ValidateCapabilities))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.RawCapabilities
		}, new CustomFilterBuilderDelegate(SharedPropertyDefinitions.CapabilitiesFilterBuilder), new GetterDelegate(SharedPropertyDefinitions.PersistedCapabilitiesGetter), new SetterDelegate(SharedPropertyDefinitions.PersistedCapabilitiesSetter), null, MbxRecipientSchema.PersistedCapabilities);

		public static readonly ADPropertyDefinition UsnChanged = new ADPropertyDefinition("UsnChanged", ExchangeObjectVersion.Exchange2003, typeof(long), "uSNChanged", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FfoExpansionSizeUpperBoundFilter = new ADPropertyDefinition("FfoExpansionSizeUpperBound", ExchangeObjectVersion.Exchange2003, typeof(int?), "FfoExpansionSizeUpperBound", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
