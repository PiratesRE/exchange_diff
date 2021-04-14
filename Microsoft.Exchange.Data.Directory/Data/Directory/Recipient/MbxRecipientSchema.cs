using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MbxRecipientSchema : ObjectSchema
	{
		public MbxRecipientSchema()
		{
			this.InitializePropertyTagMap();
		}

		private void InitializePropertyTagMap()
		{
			this.mbxPropertyDefinitionsDictionary = new Dictionary<PropTag, MbxPropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in base.AllProperties)
			{
				MbxPropertyDefinition mbxPropertyDefinition = propertyDefinition as MbxPropertyDefinition;
				if (mbxPropertyDefinition != null && mbxPropertyDefinition.PropTag != PropTag.Null)
				{
					this.mbxPropertyDefinitionsDictionary[mbxPropertyDefinition.PropTag] = mbxPropertyDefinition;
				}
			}
		}

		internal MbxPropertyDefinition FindPropertyDefinitionByPropTag(PropTag propTag)
		{
			return this.mbxPropertyDefinitionsDictionary[propTag];
		}

		public static readonly MbxPropertyDefinition DisplayName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.DisplayName, "DisplayName", false);

		public static readonly MbxPropertyDefinition ActiveSyncAllowedDeviceIDs = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationActiveSyncAllowedDeviceIDs, "ActiveSyncAllowedDeviceIDs", true);

		public static readonly MbxPropertyDefinition ActiveSyncBlockedDeviceIDs = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationActiveSyncBlockedDeviceIDs, "ActiveSyncBlockedDeviceIDs", true);

		public static readonly MbxPropertyDefinition ActiveSyncDebugLogging = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationActiveSyncDebugLogging, "ActiveSyncDebugLogging", false);

		public static readonly MbxPropertyDefinition UserInformationActiveSyncEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationActiveSyncEnabled, null, false);

		public static readonly MbxPropertyDefinition ActiveSyncEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ActiveSyncEnabled", MbxRecipientSchema.UserInformationActiveSyncEnabled, false);

		public static readonly MbxPropertyDefinition AdminDisplayName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.ProviderDllName, "AdminDisplayName", false);

		public static readonly MbxPropertyDefinition AggregationSubscriptionCredential = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationAggregationSubscriptionCredential, "AggregationSubscriptionCredential", true);

		public static readonly MbxPropertyDefinition UserInformationAllowArchiveAddressSync = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationAllowArchiveAddressSync, null, false);

		public static readonly MbxPropertyDefinition AllowArchiveAddressSync = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("AllowArchiveAddressSync", MbxRecipientSchema.UserInformationAllowArchiveAddressSync, false);

		public static readonly MbxPropertyDefinition Altitude = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.ProviderOrdinal, "Altitude", false);

		public static readonly MbxPropertyDefinition UserInformationAntispamBypassEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationAntispamBypassEnabled, null, false);

		public static readonly MbxPropertyDefinition AntispamBypassEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("AntispamBypassEnabled", MbxRecipientSchema.UserInformationAntispamBypassEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationArchiveDomain = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationArchiveDomain, null, false);

		public static readonly MbxPropertyDefinition ArchiveDomain = MbxPropertyDefinition.SmtpDomainFromStringPropertyDefinition("ArchiveDomain", MbxRecipientSchema.UserInformationArchiveDomain, false);

		public static readonly MbxPropertyDefinition UserInformationArchiveGuid = MbxPropertyDefinition.NullableGuidPropertyDefinition(PropTag.UserInformationArchiveGuid, null, false);

		public static readonly MbxPropertyDefinition ArchiveGuid = MbxPropertyDefinition.GuidFromNullableGuidPropertyDefinition("ArchiveGuid", MbxRecipientSchema.UserInformationArchiveGuid, false);

		public static readonly MbxPropertyDefinition ArchiveName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationArchiveName, "ArchiveName", true);

		public static readonly MbxPropertyDefinition UserInformationArchiveQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.ConversationIdObsolete, "UserInformationArchiveQuota", false);

		public static readonly MbxPropertyDefinition ArchiveQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("ArchiveQuota", MbxRecipientSchema.UserInformationArchiveQuota, false);

		public static readonly MbxPropertyDefinition ArchiveRelease = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationArchiveRelease, "ArchiveRelease", false);

		public static readonly MbxPropertyDefinition UserInformationArchiveStatus = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationArchiveStatus, null, false);

		public static readonly MbxPropertyDefinition ArchiveStatus = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<ArchiveStatusFlags>("ArchiveStatus", MbxRecipientSchema.UserInformationArchiveStatus, false);

		public static readonly MbxPropertyDefinition UserInformationArchiveWarningQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationArchiveWarningQuota, null, false);

		public static readonly MbxPropertyDefinition ArchiveWarningQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("ArchiveWarningQuota", MbxRecipientSchema.UserInformationArchiveWarningQuota, false);

		public static readonly MbxPropertyDefinition AssistantName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationAssistantName, "AssistantName", false);

		public static readonly MbxPropertyDefinition Birthdate = MbxPropertyDefinition.NullableDateTimePropertyDefinition(PropTag.UserInformationBirthdate, "Birthdate", false);

		public static readonly MbxPropertyDefinition UserInformationBypassNestedModerationEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationBypassNestedModerationEnabled, null, false);

		public static readonly MbxPropertyDefinition BypassNestedModerationEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("BypassNestedModerationEnabled", MbxRecipientSchema.UserInformationBypassNestedModerationEnabled, false);

		public static readonly MbxPropertyDefinition C = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationC, "C", false);

		public static readonly MbxPropertyDefinition UserInformationCalendarLoggingQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationCalendarLoggingQuota, null, false);

		public static readonly MbxPropertyDefinition CalendarLoggingQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("CalendarLoggingQuota", MbxRecipientSchema.UserInformationCalendarLoggingQuota, false);

		public static readonly MbxPropertyDefinition UserInformationCalendarRepairDisabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationCalendarRepairDisabled, null, false);

		public static readonly MbxPropertyDefinition CalendarRepairDisabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("CalendarRepairDisabled", MbxRecipientSchema.UserInformationCalendarRepairDisabled, false);

		public static readonly MbxPropertyDefinition UserInformationCalendarVersionStoreDisabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationCalendarVersionStoreDisabled, null, false);

		public static readonly MbxPropertyDefinition CalendarVersionStoreDisabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("CalendarVersionStoreDisabled", MbxRecipientSchema.UserInformationCalendarVersionStoreDisabled, false);

		public static readonly MbxPropertyDefinition City = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationCity, "City", false);

		public static readonly MbxPropertyDefinition Country = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationCountry, "Country", false);

		public static readonly MbxPropertyDefinition UserInformationCountryCode = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationCountryCode, null, false);

		public static readonly MbxPropertyDefinition CountryCode = MbxPropertyDefinition.Int32FromNullableInt32PropertyDefinition("CountryCode", MbxRecipientSchema.UserInformationCountryCode, false);

		public static readonly MbxPropertyDefinition UserInformationCountryOrRegion = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationCountryOrRegion, null, false);

		public static readonly MbxPropertyDefinition CountryOrRegion = MbxPropertyDefinition.CountryInfoFromStringPropertyDefinition("CountryOrRegion", MbxRecipientSchema.UserInformationCountryOrRegion, false);

		public static readonly MbxPropertyDefinition DefaultMailTip = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationDefaultMailTip, "DefaultMailTip", false);

		public static readonly MbxPropertyDefinition UserInformationDeliverToMailboxAndForward = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationDeliverToMailboxAndForward, null, false);

		public static readonly MbxPropertyDefinition DeliverToMailboxAndForward = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("DeliverToMailboxAndForward", MbxRecipientSchema.UserInformationDeliverToMailboxAndForward, false);

		public static readonly MbxPropertyDefinition Description = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationDescription, "Description", true);

		public static readonly MbxPropertyDefinition UserInformationDisabledArchiveGuid = MbxPropertyDefinition.NullableGuidPropertyDefinition(PropTag.UserInformationDisabledArchiveGuid, null, false);

		public static readonly MbxPropertyDefinition DisabledArchiveGuid = MbxPropertyDefinition.GuidFromNullableGuidPropertyDefinition("DisabledArchiveGuid", MbxRecipientSchema.UserInformationDisabledArchiveGuid, false);

		public static readonly MbxPropertyDefinition UserInformationDowngradeHighPriorityMessagesEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationDowngradeHighPriorityMessagesEnabled, null, false);

		public static readonly MbxPropertyDefinition DowngradeHighPriorityMessagesEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("DowngradeHighPriorityMessagesEnabled", MbxRecipientSchema.UserInformationDowngradeHighPriorityMessagesEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationECPEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationECPEnabled, null, false);

		public static readonly MbxPropertyDefinition ECPEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ECPEnabled", MbxRecipientSchema.UserInformationECPEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationEmailAddressPolicyEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationEmailAddressPolicyEnabled, null, false);

		public static readonly MbxPropertyDefinition EmailAddressPolicyEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("EmailAddressPolicyEnabled", MbxRecipientSchema.UserInformationEmailAddressPolicyEnabled, false);

		public static readonly MbxPropertyDefinition EwsAllowEntourage = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationEwsAllowEntourage, "EwsAllowEntourage", false);

		public static readonly MbxPropertyDefinition EwsAllowMacOutlook = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationEwsAllowMacOutlook, "EwsAllowMacOutlook", false);

		public static readonly MbxPropertyDefinition EwsAllowOutlook = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationEwsAllowOutlook, "EwsAllowOutlook", false);

		public static readonly MbxPropertyDefinition UserInformationEwsApplicationAccessPolicy = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationEwsApplicationAccessPolicy, null, false);

		public static readonly MbxPropertyDefinition EwsApplicationAccessPolicy = MbxPropertyDefinition.NullableEnumFromNullableInt32PropertyDefinition<EwsApplicationAccessPolicy>("EwsApplicationAccessPolicy", MbxRecipientSchema.UserInformationEwsApplicationAccessPolicy, false);

		public static readonly MbxPropertyDefinition EwsEnabled = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationEwsEnabled, "EwsEnabled", false);

		public static readonly MbxPropertyDefinition EwsExceptions = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationEwsExceptions, "EwsExceptions", true);

		public static readonly MbxPropertyDefinition EwsWellKnownApplicationAccessPolicies = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationEwsWellKnownApplicationAccessPolicies, "EwsWellKnownApplicationAccessPolicies", true);

		public static readonly MbxPropertyDefinition UserInformationExchangeGuid = MbxPropertyDefinition.NullableGuidPropertyDefinition(PropTag.UserInformationExchangeGuid, null, false);

		public static readonly MbxPropertyDefinition ExchangeGuid = MbxPropertyDefinition.GuidFromNullableGuidPropertyDefinition("ExchangeGuid", MbxRecipientSchema.UserInformationExchangeGuid, false);

		public static readonly MbxPropertyDefinition UserInformationExternalOofOptions = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationExternalOofOptions, null, false);

		public static readonly MbxPropertyDefinition ExternalOofOptions = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<ExternalOofOptions>("ExternalOofOptions", MbxRecipientSchema.UserInformationExternalOofOptions, false);

		public static readonly MbxPropertyDefinition FirstName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationFirstName, "FirstName", false);

		public static readonly MbxPropertyDefinition UserInformationForwardingSmtpAddress = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationForwardingSmtpAddress, null, false);

		public static readonly MbxPropertyDefinition ForwardingSmtpAddress = MbxPropertyDefinition.ProxyAddressFromStringPropertyDefinition("ForwardingSmtpAddress", MbxRecipientSchema.UserInformationForwardingSmtpAddress, false);

		public static readonly MbxPropertyDefinition Gender = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationGender, "Gender", false);

		public static readonly MbxPropertyDefinition UserInformationGenericForwardingAddress = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationGenericForwardingAddress, null, false);

		public static readonly MbxPropertyDefinition GenericForwardingAddress = MbxPropertyDefinition.ProxyAddressFromStringPropertyDefinition("GenericForwardingAddress", MbxRecipientSchema.UserInformationGenericForwardingAddress, false);

		public static readonly MbxPropertyDefinition UserInformationGeoCoordinates = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationGeoCoordinates, null, false);

		public static readonly MbxPropertyDefinition GeoCoordinates = MbxPropertyDefinition.GeoCoordinatesFromStringPropertyDefinition("GeoCoordinates", MbxRecipientSchema.UserInformationGeoCoordinates, false);

		public static readonly MbxPropertyDefinition HABSeniorityIndex = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationHABSeniorityIndex, "HABSeniorityIndex", false);

		public static readonly MbxPropertyDefinition UserInformationHasActiveSyncDevicePartnership = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationHasActiveSyncDevicePartnership, null, false);

		public static readonly MbxPropertyDefinition HasActiveSyncDevicePartnership = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("HasActiveSyncDevicePartnership", MbxRecipientSchema.UserInformationHasActiveSyncDevicePartnership, false);

		public static readonly MbxPropertyDefinition UserInformationHiddenFromAddressListsEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationHiddenFromAddressListsEnabled, null, false);

		public static readonly MbxPropertyDefinition HiddenFromAddressListsEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("HiddenFromAddressListsEnabled", MbxRecipientSchema.UserInformationHiddenFromAddressListsEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationHiddenFromAddressListsValue = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationHiddenFromAddressListsValue, null, false);

		public static readonly MbxPropertyDefinition HiddenFromAddressListsValue = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("HiddenFromAddressListsValue", MbxRecipientSchema.UserInformationHiddenFromAddressListsValue, false);

		public static readonly MbxPropertyDefinition HomePhone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationHomePhone, "HomePhone", false);

		public static readonly MbxPropertyDefinition UserInformationImapEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationImapEnabled, null, false);

		public static readonly MbxPropertyDefinition ImapEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ImapEnabled", MbxRecipientSchema.UserInformationImapEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationImapEnableExactRFC822Size = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationImapEnableExactRFC822Size, null, false);

		public static readonly MbxPropertyDefinition ImapEnableExactRFC822Size = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ImapEnableExactRFC822Size", MbxRecipientSchema.UserInformationImapEnableExactRFC822Size, false);

		public static readonly MbxPropertyDefinition UserInformationImapForceICalForCalendarRetrievalOption = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationImapForceICalForCalendarRetrievalOption, null, false);

		public static readonly MbxPropertyDefinition ImapForceICalForCalendarRetrievalOption = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ImapForceICalForCalendarRetrievalOption", MbxRecipientSchema.UserInformationImapForceICalForCalendarRetrievalOption, false);

		public static readonly MbxPropertyDefinition UserInformationImapMessagesRetrievalMimeFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationImapMessagesRetrievalMimeFormat, null, false);

		public static readonly MbxPropertyDefinition ImapMessagesRetrievalMimeFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MimeTextFormat>("ImapMessagesRetrievalMimeFormat", MbxRecipientSchema.UserInformationImapMessagesRetrievalMimeFormat, false);

		public static readonly MbxPropertyDefinition ImapProtocolLoggingEnabled = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationImapProtocolLoggingEnabled, "ImapProtocolLoggingEnabled", false);

		public static readonly MbxPropertyDefinition UserInformationImapSuppressReadReceipt = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationImapSuppressReadReceipt, null, false);

		public static readonly MbxPropertyDefinition ImapSuppressReadReceipt = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ImapSuppressReadReceipt", MbxRecipientSchema.UserInformationImapSuppressReadReceipt, false);

		public static readonly MbxPropertyDefinition UserInformationImapUseProtocolDefaults = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationImapUseProtocolDefaults, null, false);

		public static readonly MbxPropertyDefinition ImapUseProtocolDefaults = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ImapUseProtocolDefaults", MbxRecipientSchema.UserInformationImapUseProtocolDefaults, false);

		public static readonly MbxPropertyDefinition UserInformationIncludeInGarbageCollection = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIncludeInGarbageCollection, null, false);

		public static readonly MbxPropertyDefinition IncludeInGarbageCollection = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IncludeInGarbageCollection", MbxRecipientSchema.UserInformationIncludeInGarbageCollection, false);

		public static readonly MbxPropertyDefinition Initials = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationInitials, "Initials", false);

		public static readonly MbxPropertyDefinition InPlaceHolds = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationInPlaceHolds, "InPlaceHolds", true);

		public static readonly MbxPropertyDefinition UserInformationInternalOnly = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationInternalOnly, null, false);

		public static readonly MbxPropertyDefinition InternalOnly = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("InternalOnly", MbxRecipientSchema.UserInformationInternalOnly, false);

		public static readonly MbxPropertyDefinition InternalUsageLocation = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationInternalUsageLocation, "InternalUsageLocation", false);

		public static readonly MbxPropertyDefinition UserInformationInternetEncoding = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationInternetEncoding, null, false);

		public static readonly MbxPropertyDefinition InternetEncoding = MbxPropertyDefinition.Int32FromNullableInt32PropertyDefinition("InternetEncoding", MbxRecipientSchema.UserInformationInternetEncoding, false);

		public static readonly MbxPropertyDefinition UserInformationIsCalculatedTargetAddress = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsCalculatedTargetAddress, null, false);

		public static readonly MbxPropertyDefinition IsCalculatedTargetAddress = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsCalculatedTargetAddress", MbxRecipientSchema.UserInformationIsCalculatedTargetAddress, false);

		public static readonly MbxPropertyDefinition UserInformationIsExcludedFromServingHierarchy = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsExcludedFromServingHierarchy, null, false);

		public static readonly MbxPropertyDefinition IsExcludedFromServingHierarchy = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsExcludedFromServingHierarchy", MbxRecipientSchema.UserInformationIsExcludedFromServingHierarchy, false);

		public static readonly MbxPropertyDefinition UserInformationIsHierarchyReady = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsHierarchyReady, null, false);

		public static readonly MbxPropertyDefinition IsHierarchyReady = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsHierarchyReady", MbxRecipientSchema.UserInformationIsHierarchyReady, false);

		public static readonly MbxPropertyDefinition UserInformationIsInactiveMailbox = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsInactiveMailbox, null, false);

		public static readonly MbxPropertyDefinition IsInactiveMailbox = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsInactiveMailbox", MbxRecipientSchema.UserInformationIsInactiveMailbox, false);

		public static readonly MbxPropertyDefinition UserInformationIsSoftDeletedByDisable = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsSoftDeletedByDisable, null, false);

		public static readonly MbxPropertyDefinition IsSoftDeletedByDisable = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsSoftDeletedByDisable", MbxRecipientSchema.UserInformationIsSoftDeletedByDisable, false);

		public static readonly MbxPropertyDefinition UserInformationIsSoftDeletedByRemove = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsSoftDeletedByRemove, null, false);

		public static readonly MbxPropertyDefinition IsSoftDeletedByRemove = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsSoftDeletedByRemove", MbxRecipientSchema.UserInformationIsSoftDeletedByRemove, false);

		public static readonly MbxPropertyDefinition UserInformationIssueWarningQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationIssueWarningQuota, null, false);

		public static readonly MbxPropertyDefinition IssueWarningQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("IssueWarningQuota", MbxRecipientSchema.UserInformationIssueWarningQuota, false);

		public static readonly MbxPropertyDefinition UserInformationJournalArchiveAddress = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationJournalArchiveAddress, null, false);

		public static readonly MbxPropertyDefinition JournalArchiveAddress = MbxPropertyDefinition.SmtpAddressFromStringPropertyDefinition("JournalArchiveAddress", MbxRecipientSchema.UserInformationJournalArchiveAddress, false);

		public static readonly MbxPropertyDefinition UserInformationLanguages = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationLanguages, null, true);

		public static readonly MbxPropertyDefinition Languages = MbxPropertyDefinition.CultureInfoFromStringPropertyDefinition("Languages", MbxRecipientSchema.UserInformationLanguages, true);

		public static readonly MbxPropertyDefinition LastExchangeChangedTime = MbxPropertyDefinition.NullableDateTimePropertyDefinition(PropTag.UserInformationLastExchangeChangedTime, "LastExchangeChangedTime", false);

		public static readonly MbxPropertyDefinition LastName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationLastName, "LastName", false);

		public static readonly MbxPropertyDefinition Latitude = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationLatitude, "Latitude", false);

		public static readonly MbxPropertyDefinition UserInformationLEOEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationLEOEnabled, null, false);

		public static readonly MbxPropertyDefinition LEOEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("LEOEnabled", MbxRecipientSchema.UserInformationLEOEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationLocaleID = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationLocaleID, null, true);

		public static readonly MbxPropertyDefinition LocaleID = MbxPropertyDefinition.Int32FromNullableInt32PropertyDefinition("LocaleID", MbxRecipientSchema.UserInformationLocaleID, true);

		public static readonly MbxPropertyDefinition Longitude = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationLongitude, "Longitude", false);

		public static readonly MbxPropertyDefinition UserInformationMacAttachmentFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMacAttachmentFormat, null, false);

		public static readonly MbxPropertyDefinition MacAttachmentFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MacAttachmentFormat>("MacAttachmentFormat", MbxRecipientSchema.UserInformationMacAttachmentFormat, false);

		public static readonly MbxPropertyDefinition MailboxContainerGuid = MbxPropertyDefinition.NullableGuidPropertyDefinition(PropTag.UserInformationMailboxContainerGuid, "MailboxContainerGuid", false);

		public static readonly MbxPropertyDefinition MailboxMoveBatchName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMailboxMoveBatchName, "MailboxMoveBatchName", false);

		public static readonly MbxPropertyDefinition MailboxMoveRemoteHostName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMailboxMoveRemoteHostName, "MailboxMoveRemoteHostName", false);

		public static readonly MbxPropertyDefinition UserInformationMailboxMoveStatus = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMailboxMoveStatus, null, false);

		public static readonly MbxPropertyDefinition MailboxMoveStatus = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<RequestStatus>("MailboxMoveStatus", MbxRecipientSchema.UserInformationMailboxMoveStatus, false);

		public static readonly MbxPropertyDefinition MailboxRelease = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMailboxRelease, "MailboxRelease", false);

		public static readonly MbxPropertyDefinition MailTipTranslations = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMailTipTranslations, "MailTipTranslations", true);

		public static readonly MbxPropertyDefinition UserInformationMAPIBlockOutlookNonCachedMode = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMAPIBlockOutlookNonCachedMode, null, false);

		public static readonly MbxPropertyDefinition MAPIBlockOutlookNonCachedMode = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("MAPIBlockOutlookNonCachedMode", MbxRecipientSchema.UserInformationMAPIBlockOutlookNonCachedMode, false);

		public static readonly MbxPropertyDefinition UserInformationMAPIBlockOutlookRpcHttp = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMAPIBlockOutlookRpcHttp, null, false);

		public static readonly MbxPropertyDefinition MAPIBlockOutlookRpcHttp = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("MAPIBlockOutlookRpcHttp", MbxRecipientSchema.UserInformationMAPIBlockOutlookRpcHttp, false);

		public static readonly MbxPropertyDefinition MAPIBlockOutlookExternalConnectivity = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMAPIBlockOutlookExternalConnectivity, "MAPIBlockOutlookExternalConnectivity", false);

		public static readonly MbxPropertyDefinition MAPIBlockOutlookVersions = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMAPIBlockOutlookVersions, "MAPIBlockOutlookVersions", false);

		public static readonly MbxPropertyDefinition UserInformationMAPIEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMAPIEnabled, null, false);

		public static readonly MbxPropertyDefinition MAPIEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("MAPIEnabled", MbxRecipientSchema.UserInformationMAPIEnabled, false);

		public static readonly MbxPropertyDefinition MapiHttpEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMapiHttpEnabled, "MapiHttpEnabled", false);

		public static readonly MbxPropertyDefinition MapiRecipient = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMapiRecipient, "MapiRecipient", false);

		public static readonly MbxPropertyDefinition MaxBlockedSenders = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMaxBlockedSenders, "MaxBlockedSenders", false);

		public static readonly MbxPropertyDefinition UserInformationMaxReceiveSize = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMaxReceiveSize, null, false);

		public static readonly MbxPropertyDefinition MaxReceiveSize = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("MaxReceiveSize", MbxRecipientSchema.UserInformationMaxReceiveSize, false);

		public static readonly MbxPropertyDefinition MaxSafeSenders = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMaxSafeSenders, "MaxSafeSenders", false);

		public static readonly MbxPropertyDefinition UserInformationMaxSendSize = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMaxSendSize, null, false);

		public static readonly MbxPropertyDefinition MaxSendSize = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("MaxSendSize", MbxRecipientSchema.UserInformationMaxSendSize, false);

		public static readonly MbxPropertyDefinition MemberName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMemberName, "MemberName", false);

		public static readonly MbxPropertyDefinition UserInformationMessageBodyFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMessageBodyFormat, null, false);

		public static readonly MbxPropertyDefinition MessageBodyFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MessageBodyFormat>("MessageBodyFormat", MbxRecipientSchema.UserInformationMessageBodyFormat, false);

		public static readonly MbxPropertyDefinition UserInformationMessageFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMessageFormat, null, false);

		public static readonly MbxPropertyDefinition MessageFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MessageFormat>("MessageFormat", MbxRecipientSchema.UserInformationMessageFormat, false);

		public static readonly MbxPropertyDefinition UserInformationMessageTrackingReadStatusDisabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMessageTrackingReadStatusDisabled, null, false);

		public static readonly MbxPropertyDefinition MessageTrackingReadStatusDisabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("MessageTrackingReadStatusDisabled", MbxRecipientSchema.UserInformationMessageTrackingReadStatusDisabled, false);

		public static readonly MbxPropertyDefinition UserInformationMobileFeaturesEnabled = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationMobileFeaturesEnabled, null, false);

		public static readonly MbxPropertyDefinition MobileFeaturesEnabled = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MobileFeaturesEnabled>("MobileFeaturesEnabled", MbxRecipientSchema.UserInformationMobileFeaturesEnabled, false);

		public static readonly MbxPropertyDefinition MobilePhone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationMobilePhone, "MobilePhone", false);

		public static readonly MbxPropertyDefinition UserInformationModerationFlags = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationModerationFlags, null, false);

		public static readonly MbxPropertyDefinition ModerationFlags = MbxPropertyDefinition.Int32FromNullableInt32PropertyDefinition("ModerationFlags", MbxRecipientSchema.UserInformationModerationFlags, false);

		public static readonly MbxPropertyDefinition Notes = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationNotes, "Notes", false);

		public static readonly MbxPropertyDefinition Occupation = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationOccupation, "Occupation", false);

		public static readonly MbxPropertyDefinition UserInformationOpenDomainRoutingDisabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationOpenDomainRoutingDisabled, null, false);

		public static readonly MbxPropertyDefinition OpenDomainRoutingDisabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("OpenDomainRoutingDisabled", MbxRecipientSchema.UserInformationOpenDomainRoutingDisabled, false);

		public static readonly MbxPropertyDefinition OtherHomePhone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationOtherHomePhone, "OtherHomePhone", true);

		public static readonly MbxPropertyDefinition OtherMobile = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationOtherMobile, "OtherMobile", true);

		public static readonly MbxPropertyDefinition OtherTelephone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationOtherTelephone, "OtherTelephone", true);

		public static readonly MbxPropertyDefinition UserInformationOWAEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationOWAEnabled, null, false);

		public static readonly MbxPropertyDefinition OWAEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("OWAEnabled", MbxRecipientSchema.UserInformationOWAEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationOWAforDevicesEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationOWAforDevicesEnabled, null, false);

		public static readonly MbxPropertyDefinition OWAforDevicesEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("OWAforDevicesEnabled", MbxRecipientSchema.UserInformationOWAforDevicesEnabled, false);

		public static readonly MbxPropertyDefinition Pager = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationPager, "Pager", false);

		public static readonly MbxPropertyDefinition UserInformationPersistedCapabilities = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationPersistedCapabilities, null, true);

		public static readonly MbxPropertyDefinition PersistedCapabilities = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<Capability>("PersistedCapabilities", MbxRecipientSchema.UserInformationPersistedCapabilities, true);

		public static readonly MbxPropertyDefinition Phone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationPhone, "Phone", false);

		public static readonly MbxPropertyDefinition PhoneProviderId = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationPhoneProviderId, "PhoneProviderId", false);

		public static readonly MbxPropertyDefinition UserInformationPopEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationPopEnabled, null, false);

		public static readonly MbxPropertyDefinition PopEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("PopEnabled", MbxRecipientSchema.UserInformationPopEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationPopEnableExactRFC822Size = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationPopEnableExactRFC822Size, null, false);

		public static readonly MbxPropertyDefinition PopEnableExactRFC822Size = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("PopEnableExactRFC822Size", MbxRecipientSchema.UserInformationPopEnableExactRFC822Size, false);

		public static readonly MbxPropertyDefinition UserInformationPopForceICalForCalendarRetrievalOption = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationPopForceICalForCalendarRetrievalOption, null, false);

		public static readonly MbxPropertyDefinition PopForceICalForCalendarRetrievalOption = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("PopForceICalForCalendarRetrievalOption", MbxRecipientSchema.UserInformationPopForceICalForCalendarRetrievalOption, false);

		public static readonly MbxPropertyDefinition UserInformationPopMessagesRetrievalMimeFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationPopMessagesRetrievalMimeFormat, null, false);

		public static readonly MbxPropertyDefinition PopMessagesRetrievalMimeFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<MimeTextFormat>("PopMessagesRetrievalMimeFormat", MbxRecipientSchema.UserInformationPopMessagesRetrievalMimeFormat, false);

		public static readonly MbxPropertyDefinition PopProtocolLoggingEnabled = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationPopProtocolLoggingEnabled, "PopProtocolLoggingEnabled", false);

		public static readonly MbxPropertyDefinition UserInformationPopSuppressReadReceipt = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationPopSuppressReadReceipt, null, false);

		public static readonly MbxPropertyDefinition PopSuppressReadReceipt = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("PopSuppressReadReceipt", MbxRecipientSchema.UserInformationPopSuppressReadReceipt, false);

		public static readonly MbxPropertyDefinition UserInformationPopUseProtocolDefaults = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationPopUseProtocolDefaults, null, false);

		public static readonly MbxPropertyDefinition PopUseProtocolDefaults = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("PopUseProtocolDefaults", MbxRecipientSchema.UserInformationPopUseProtocolDefaults, false);

		public static readonly MbxPropertyDefinition PostalCode = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationPostalCode, "PostalCode", false);

		public static readonly MbxPropertyDefinition PostOfficeBox = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationPostOfficeBox, "PostOfficeBox", true);

		public static readonly MbxPropertyDefinition UserInformationPreviousExchangeGuid = MbxPropertyDefinition.NullableGuidPropertyDefinition(PropTag.UserInformationPreviousExchangeGuid, null, false);

		public static readonly MbxPropertyDefinition PreviousExchangeGuid = MbxPropertyDefinition.GuidFromNullableGuidPropertyDefinition("PreviousExchangeGuid", MbxRecipientSchema.UserInformationPreviousExchangeGuid, false);

		public static readonly MbxPropertyDefinition UserInformationPreviousRecipientTypeDetails = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationPreviousRecipientTypeDetails, null, false);

		public static readonly MbxPropertyDefinition PreviousRecipientTypeDetails = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<RecipientTypeDetails>("PreviousRecipientTypeDetails", MbxRecipientSchema.UserInformationPreviousRecipientTypeDetails, false);

		public static readonly MbxPropertyDefinition UserInformationProhibitSendQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationProhibitSendQuota, null, false);

		public static readonly MbxPropertyDefinition ProhibitSendQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("ProhibitSendQuota", MbxRecipientSchema.UserInformationProhibitSendQuota, false);

		public static readonly MbxPropertyDefinition UserInformationProhibitSendReceiveQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationProhibitSendReceiveQuota, null, false);

		public static readonly MbxPropertyDefinition ProhibitSendReceiveQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("ProhibitSendReceiveQuota", MbxRecipientSchema.UserInformationProhibitSendReceiveQuota, false);

		public static readonly MbxPropertyDefinition UserInformationQueryBaseDNRestrictionEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationQueryBaseDNRestrictionEnabled, null, false);

		public static readonly MbxPropertyDefinition QueryBaseDNRestrictionEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("QueryBaseDNRestrictionEnabled", MbxRecipientSchema.UserInformationQueryBaseDNRestrictionEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationRecipientDisplayType = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationRecipientDisplayType, null, false);

		public static readonly MbxPropertyDefinition RecipientDisplayType = MbxPropertyDefinition.NullableEnumFromNullableInt32PropertyDefinition<RecipientDisplayType>("RecipientDisplayType", MbxRecipientSchema.UserInformationRecipientDisplayType, false);

		public static readonly MbxPropertyDefinition UserInformationRecipientLimits = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationRecipientLimits, null, false);

		public static readonly MbxPropertyDefinition RecipientLimits = MbxPropertyDefinition.UnlimitedInt32FromStringPropertyDefinition("RecipientLimits", MbxRecipientSchema.UserInformationRecipientLimits, false);

		public static readonly MbxPropertyDefinition UserInformationRecipientSoftDeletedStatus = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationRecipientSoftDeletedStatus, null, false);

		public static readonly MbxPropertyDefinition RecipientSoftDeletedStatus = MbxPropertyDefinition.Int32FromNullableInt32PropertyDefinition("RecipientSoftDeletedStatus", MbxRecipientSchema.UserInformationRecipientSoftDeletedStatus, false);

		public static readonly MbxPropertyDefinition UserInformationRecoverableItemsQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationRecoverableItemsQuota, null, false);

		public static readonly MbxPropertyDefinition RecoverableItemsQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("RecoverableItemsQuota", MbxRecipientSchema.UserInformationRecoverableItemsQuota, false);

		public static readonly MbxPropertyDefinition UserInformationRecoverableItemsWarningQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationRecoverableItemsWarningQuota, null, false);

		public static readonly MbxPropertyDefinition RecoverableItemsWarningQuota = MbxPropertyDefinition.UnlimitedByteQuantifiedSizeFromStringPropertyDefinition("RecoverableItemsWarningQuota", MbxRecipientSchema.UserInformationRecoverableItemsWarningQuota, false);

		public static readonly MbxPropertyDefinition Region = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationRegion, "Region", false);

		public static readonly MbxPropertyDefinition UserInformationRemotePowerShellEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationRemotePowerShellEnabled, null, false);

		public static readonly MbxPropertyDefinition RemotePowerShellEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("RemotePowerShellEnabled", MbxRecipientSchema.UserInformationRemotePowerShellEnabled, false);

		public static readonly MbxPropertyDefinition UserInformationRemoteRecipientType = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationRemoteRecipientType, null, false);

		public static readonly MbxPropertyDefinition RemoteRecipientType = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<RemoteRecipientType>("RemoteRecipientType", MbxRecipientSchema.UserInformationRemoteRecipientType, false);

		public static readonly MbxPropertyDefinition UserInformationRequireAllSendersAreAuthenticated = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationRequireAllSendersAreAuthenticated, null, false);

		public static readonly MbxPropertyDefinition RequireAllSendersAreAuthenticated = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("RequireAllSendersAreAuthenticated", MbxRecipientSchema.UserInformationRequireAllSendersAreAuthenticated, false);

		public static readonly MbxPropertyDefinition UserInformationResetPasswordOnNextLogon = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationResetPasswordOnNextLogon, null, false);

		public static readonly MbxPropertyDefinition ResetPasswordOnNextLogon = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ResetPasswordOnNextLogon", MbxRecipientSchema.UserInformationResetPasswordOnNextLogon, false);

		public static readonly MbxPropertyDefinition UserInformationRetainDeletedItemsFor = MbxPropertyDefinition.NullableInt64PropertyDefinition(PropTag.UserInformationRetainDeletedItemsFor, null, false);

		public static readonly MbxPropertyDefinition RetainDeletedItemsFor = MbxPropertyDefinition.EnhancedTimeSpanFromNullableInt64PropertyDefinition("RetainDeletedItemsFor", MbxRecipientSchema.UserInformationRetainDeletedItemsFor, false);

		public static readonly MbxPropertyDefinition UserInformationRetainDeletedItemsUntilBackup = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationRetainDeletedItemsUntilBackup, null, false);

		public static readonly MbxPropertyDefinition RetainDeletedItemsUntilBackup = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("RetainDeletedItemsUntilBackup", MbxRecipientSchema.UserInformationRetainDeletedItemsUntilBackup, false);

		public static readonly MbxPropertyDefinition UserInformationRulesQuota = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationRulesQuota, null, false);

		public static readonly MbxPropertyDefinition RulesQuota = MbxPropertyDefinition.ByteQuantifiedSizeFromStringPropertyDefinition("RulesQuota", MbxRecipientSchema.UserInformationRulesQuota, false);

		public static readonly MbxPropertyDefinition UserInformationShouldUseDefaultRetentionPolicy = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationShouldUseDefaultRetentionPolicy, null, false);

		public static readonly MbxPropertyDefinition ShouldUseDefaultRetentionPolicy = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("ShouldUseDefaultRetentionPolicy", MbxRecipientSchema.UserInformationShouldUseDefaultRetentionPolicy, false);

		public static readonly MbxPropertyDefinition SimpleDisplayName = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationSimpleDisplayName, "SimpleDisplayName", false);

		public static readonly MbxPropertyDefinition UserInformationSingleItemRecoveryEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationSingleItemRecoveryEnabled, null, false);

		public static readonly MbxPropertyDefinition SingleItemRecoveryEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("SingleItemRecoveryEnabled", MbxRecipientSchema.UserInformationSingleItemRecoveryEnabled, false);

		public static readonly MbxPropertyDefinition StateOrProvince = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationStateOrProvince, "StateOrProvince", false);

		public static readonly MbxPropertyDefinition StreetAddress = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationStreetAddress, "StreetAddress", false);

		public static readonly MbxPropertyDefinition UserInformationSubscriberAccessEnabled = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationSubscriberAccessEnabled, null, false);

		public static readonly MbxPropertyDefinition SubscriberAccessEnabled = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("SubscriberAccessEnabled", MbxRecipientSchema.UserInformationSubscriberAccessEnabled, false);

		public static readonly MbxPropertyDefinition TextEncodedORAddress = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationTextEncodedORAddress, "TextEncodedORAddress", false);

		public static readonly MbxPropertyDefinition UserInformationTextMessagingState = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationTextMessagingState, null, true);

		public static readonly MbxPropertyDefinition TextMessagingState = MbxPropertyDefinition.TextMessagingStateBaseFromStringPropertyDefinition("TextMessagingState", MbxRecipientSchema.UserInformationTextMessagingState, true);

		public static readonly MbxPropertyDefinition Timezone = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationTimezone, "Timezone", false);

		public static readonly MbxPropertyDefinition UserInformationUCSImListMigrationCompleted = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationUCSImListMigrationCompleted, null, false);

		public static readonly MbxPropertyDefinition UCSImListMigrationCompleted = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("UCSImListMigrationCompleted", MbxRecipientSchema.UserInformationUCSImListMigrationCompleted, false);

		public static readonly MbxPropertyDefinition UpgradeDetails = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationUpgradeDetails, "UpgradeDetails", false);

		public static readonly MbxPropertyDefinition UpgradeMessage = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationUpgradeMessage, "UpgradeMessage", false);

		public static readonly MbxPropertyDefinition UserInformationUpgradeRequest = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationUpgradeRequest, null, false);

		public static readonly MbxPropertyDefinition UpgradeRequest = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<UpgradeRequestTypes>("UpgradeRequest", MbxRecipientSchema.UserInformationUpgradeRequest, false);

		public static readonly MbxPropertyDefinition UserInformationUpgradeStage = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationUpgradeStage, null, false);

		public static readonly MbxPropertyDefinition UpgradeStage = MbxPropertyDefinition.NullableEnumFromNullableInt32PropertyDefinition<UpgradeStage>("UpgradeStage", MbxRecipientSchema.UserInformationUpgradeStage, false);

		public static readonly MbxPropertyDefinition UpgradeStageTimeStamp = MbxPropertyDefinition.NullableDateTimePropertyDefinition(PropTag.UserInformationUpgradeStageTimeStamp, "UpgradeStageTimeStamp", false);

		public static readonly MbxPropertyDefinition UserInformationUpgradeStatus = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationUpgradeStatus, null, false);

		public static readonly MbxPropertyDefinition UpgradeStatus = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<UpgradeStatusTypes>("UpgradeStatus", MbxRecipientSchema.UserInformationUpgradeStatus, false);

		public static readonly MbxPropertyDefinition UserInformationUsageLocation = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationUsageLocation, null, false);

		public static readonly MbxPropertyDefinition UsageLocation = MbxPropertyDefinition.CountryInfoFromStringPropertyDefinition("UsageLocation", MbxRecipientSchema.UserInformationUsageLocation, false);

		public static readonly MbxPropertyDefinition UserInformationUseMapiRichTextFormat = MbxPropertyDefinition.NullableInt32PropertyDefinition(PropTag.UserInformationUseMapiRichTextFormat, null, false);

		public static readonly MbxPropertyDefinition UseMapiRichTextFormat = MbxPropertyDefinition.EnumFromNullableInt32PropertyDefinition<UseMapiRichTextFormat>("UseMapiRichTextFormat", MbxRecipientSchema.UserInformationUseMapiRichTextFormat, false);

		public static readonly MbxPropertyDefinition UserInformationUsePreferMessageFormat = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationUsePreferMessageFormat, null, false);

		public static readonly MbxPropertyDefinition UsePreferMessageFormat = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("UsePreferMessageFormat", MbxRecipientSchema.UserInformationUsePreferMessageFormat, false);

		public static readonly MbxPropertyDefinition UserInformationUseUCCAuditConfig = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationUseUCCAuditConfig, null, false);

		public static readonly MbxPropertyDefinition UseUCCAuditConfig = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("UseUCCAuditConfig", MbxRecipientSchema.UserInformationUseUCCAuditConfig, false);

		public static readonly MbxPropertyDefinition WebPage = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationWebPage, "WebPage", false);

		public static readonly MbxPropertyDefinition WhenMailboxCreated = MbxPropertyDefinition.NullableDateTimePropertyDefinition(PropTag.UserInformationWhenMailboxCreated, "WhenMailboxCreated", false);

		public static readonly MbxPropertyDefinition WhenSoftDeleted = MbxPropertyDefinition.NullableDateTimePropertyDefinition(PropTag.UserInformationWhenSoftDeleted, "WhenSoftDeleted", false);

		public static readonly MbxPropertyDefinition BirthdayPrecision = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationBirthdayPrecision, "BirthdayPrecision", false);

		public static readonly MbxPropertyDefinition NameVersion = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationNameVersion, "NameVersion", false);

		public static readonly MbxPropertyDefinition UserInformationOptInUser = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationOptInUser, null, false);

		public static readonly MbxPropertyDefinition OptInUser = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("OptInUser", MbxRecipientSchema.UserInformationOptInUser, false);

		public static readonly MbxPropertyDefinition UserInformationIsMigratedConsumerMailbox = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsMigratedConsumerMailbox, null, false);

		public static readonly MbxPropertyDefinition IsMigratedConsumerMailbox = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsMigratedConsumerMailbox", MbxRecipientSchema.UserInformationIsMigratedConsumerMailbox, false);

		public static readonly MbxPropertyDefinition UserInformationMigrationDryRun = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationMigrationDryRun, null, false);

		public static readonly MbxPropertyDefinition MigrationDryRun = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("MigrationDryRun", MbxRecipientSchema.UserInformationMigrationDryRun, false);

		public static readonly MbxPropertyDefinition UserInformationIsPremiumConsumerMailbox = MbxPropertyDefinition.NullableBoolPropertyDefinition(PropTag.UserInformationIsPremiumConsumerMailbox, null, false);

		public static readonly MbxPropertyDefinition IsPremiumConsumerMailbox = MbxPropertyDefinition.BoolFromNullableBoolPropertyDefinition("IsPremiumConsumerMailbox", MbxRecipientSchema.UserInformationIsPremiumConsumerMailbox, false);

		public static readonly MbxPropertyDefinition AlternateSupportEmailAddresses = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationAlternateSupportEmailAddresses, "AlternateSupportEmailAddresses", false);

		public static readonly MbxPropertyDefinition UserInformationEmailAddresses = MbxPropertyDefinition.StringPropertyDefinition(PropTag.UserInformationEmailAddresses, null, true);

		public static readonly MbxPropertyDefinition EmailAddresses = MbxPropertyDefinition.ProxyAddressFromStringPropertyDefinition("EmailAddresses", MbxRecipientSchema.UserInformationEmailAddresses, true);

		private Dictionary<PropTag, MbxPropertyDefinition> mbxPropertyDefinitionsDictionary;
	}
}
