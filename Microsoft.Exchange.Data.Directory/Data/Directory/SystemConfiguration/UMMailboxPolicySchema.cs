using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UMMailboxPolicySchema : MailboxPolicySchema
	{
		public static readonly ADPropertyDefinition AllowCommonPatternsValue = new ADPropertyDefinition("AllowCommonPatterns", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMPinPolicyDisallowCommonPatterns", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedInCountryOrRegionGroups = SharedPropertyDefinitions.AllowedInCountryOrRegionGroups;

		public static readonly ADPropertyDefinition AllowedInternationalGroups = SharedPropertyDefinitions.AllowedInternationalGroups;

		public static readonly ADPropertyDefinition AllowDialPlanSubscribers = new ADPropertyDefinition("AllowDialPlanSubscribers", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMDialPlanSubscribersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowExtensions = new ADPropertyDefinition("AllowExtensions", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMExtensionLengthNumbersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedUsers = new ADPropertyDefinition("AssociatedUsers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMTemplateBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FaxMessageText = new ADPropertyDefinition("FaxMessageText", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMFaxMessageText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition LogonFailuresBeforePINReset = new ADPropertyDefinition("LogonFailuresBeforePINReset", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchUMLogonFailuresBeforePINReset", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, 999)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxGreetingDuration = new ADPropertyDefinition("MaxGreetingDuration", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMMaxGreetingDuration", ADPropertyDefinitionFlags.PersistDefaultValue, 5, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 10)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxLogonAttempts = new ADPropertyDefinition("MaxLogonAttempts", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchUMPinPolicyAccountLockoutFailures", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, 999)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinPINLength = new ADPropertyDefinition("MinPINLength", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMPinPolicyMinPasswordLength", ADPropertyDefinitionFlags.PersistDefaultValue, 6, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(4, 24)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PINHistoryCount = new ADPropertyDefinition("PINHistoryCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMPinPolicyNumberOfPreviousPasswordsDisallowed", ADPropertyDefinitionFlags.PersistDefaultValue, 5, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 20)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PINLifetime = new ADPropertyDefinition("PINLifetime", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<EnhancedTimeSpan>), "msExchUMPinPolicyExpiryDays", ADPropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(999.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtectUnauthenticatedVoiceMail = new ADPropertyDefinition("ProtectUnauthenticatedVoiceMail", ExchangeObjectVersion.Exchange2010, typeof(DRMProtectionOptions), "msExchUMProtectUnauthenticatedVoiceMail", ADPropertyDefinitionFlags.PersistDefaultValue, DRMProtectionOptions.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DRMProtectionOptions))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtectAuthenticatedVoiceMail = new ADPropertyDefinition("ProtectAuthenticatedVoiceMail", ExchangeObjectVersion.Exchange2010, typeof(DRMProtectionOptions), "msExchUMProtectAuthenticatedVoiceMail", ADPropertyDefinitionFlags.PersistDefaultValue, DRMProtectionOptions.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DRMProtectionOptions))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtectedVoiceMailText = new ADPropertyDefinition("ProtectedVoiceMailText", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMProtectedVoiceMailText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition RequireProtectedPlayOnPhone = new ADPropertyDefinition("RequireProtectedPlayOnPhone", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchUMRequireProtectedPlayOnPhone", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ResetPINText = new ADPropertyDefinition("ResetPINText", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMResetPinText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition SourceForestPolicyNames = new ADPropertyDefinition("SourceForestPolicyNames", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMSourceForestPolicyNames", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMEnabledText = new ADPropertyDefinition("UMEnabledText", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMEnabledText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition VoiceMailText = new ADPropertyDefinition("VoiceMailText", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMVoiceMailText", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition UMDialPlan = new ADPropertyDefinition("UMDialPlan", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMMailboxPolicyDialPlanLink", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMEnabledFlagsBits = new ADPropertyDefinition("UMEnabledFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMEnabledFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMEnabledFlags2Bits = new ADPropertyDefinition("UMEnabledFlags2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchUMEnabledFlags2", ADPropertyDefinitionFlags.PersistDefaultValue, 65534, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VoiceMailPreviewPartnerAddress = new ADPropertyDefinition("VoiceMailPreviewPartnerAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress?), "msExchVoiceMailPreviewPartnerAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VoiceMailPreviewPartnerAssignedID = new ADPropertyDefinition("VoiceMailPreviewPartnerAssignedID", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchVoiceMailPreviewPartnerAssignedID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VoiceMailPreviewPartnerMaxDeliveryDelay = new ADPropertyDefinition("VoiceMailPreviewPartnerMaxDeliveryDelay", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchVoiceMailPreviewPartnerMaxDeliveryDelay", ADPropertyDefinitionFlags.PersistDefaultValue, 1200, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(300, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VoiceMailPreviewPartnerMaxMessageDuration = new ADPropertyDefinition("VoiceMailPreviewPartnerMaxMessageDuration", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchVoiceMailPreviewPartnerMaxMessageDuration", ADPropertyDefinitionFlags.PersistDefaultValue, 180, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(60, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowCommonPatterns = new ADPropertyDefinition("AllowCommonPatterns", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			UMMailboxPolicySchema.AllowCommonPatternsValue
		}, null, (IPropertyBag propertyBag) => 0 != (int)propertyBag[UMMailboxPolicySchema.AllowCommonPatternsValue], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[UMMailboxPolicySchema.AllowCommonPatternsValue] = (((bool)value) ? 1 : 0);
		}, null, null);

		public static readonly ADPropertyDefinition AllowMissedCallNotifications = new ADPropertyDefinition("AllowMissedCallNotifications", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlagsBits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 1), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 1), null, null);

		public static readonly ADPropertyDefinition AllowVirtualNumber = new ADPropertyDefinition("AllowVirtualNumber", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlagsBits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 4), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 4), null, null);

		public static readonly ADPropertyDefinition AllowFax = new ADPropertyDefinition("AllowFax", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 1), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 1), null, null);

		public static readonly ADPropertyDefinition AllowTUIAccessToCalendar = new ADPropertyDefinition("AllowTUIAccessToCalendar", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 2), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 2), null, null);

		public static readonly ADPropertyDefinition AllowTUIAccessToEmail = new ADPropertyDefinition("AllowTUIAccessToEmail", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 4), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 4), null, null);

		public static readonly ADPropertyDefinition AllowSubscriberAccess = new ADPropertyDefinition("AllowSubscriberAccess", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 8), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 8), null, null);

		public static readonly ADPropertyDefinition AllowTUIAccessToDirectory = new ADPropertyDefinition("AllowTUIAccessToDirectory", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 16), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 16), null, null);

		public static readonly ADPropertyDefinition AllowTUIAccessToPersonalContacts = new ADPropertyDefinition("AllowTUIAccessToPersonalContacts", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 1024), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 1024), null, null);

		public static readonly ADPropertyDefinition AllowASR = new ADPropertyDefinition("AllowASR", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 32), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 32), null, null);

		public static readonly ADPropertyDefinition AllowPlayOnPhone = new ADPropertyDefinition("AllowPlayOnPhone", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 64), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 64), null, null);

		public static readonly ADPropertyDefinition AllowVoiceMailPreview = new ADPropertyDefinition("AllowVoiceMailPreview", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 128), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 128), null, null);

		public static readonly ADPropertyDefinition AllowPersonalAutoAttendant = new ADPropertyDefinition("AllowPersonalAutoAttendant", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 256), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 256), null, null);

		public static readonly ADPropertyDefinition AllowMessageWaitingIndicator = new ADPropertyDefinition("AllowMessageWaitingIndicator", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 512), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 512), null, null);

		public static readonly ADPropertyDefinition AllowSMSNotification = new ADPropertyDefinition("AllowSMSNotification", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 2048), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 2048), null, null);

		public static readonly ADPropertyDefinition AllowPinlessVoiceMailAccess = new ADPropertyDefinition("AllowPinlessVoiceMailAccess", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlagsBits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 8), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 8), null, null);

		public static readonly ADPropertyDefinition AllowVoiceResponseToOtherMessageTypes = new ADPropertyDefinition("AllowVoiceResponseToOtherMessageTypes", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 4096), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 4096), null, null);

		public static readonly ADPropertyDefinition FaxServerURI = new ADPropertyDefinition("FaxServerURI", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMFaxServerURI", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition AllowVoiceMailAnalysis = new ADPropertyDefinition("AllowVoiceMailAnalysis", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlagsBits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 16), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlagsBits, 16), null, null);

		public static readonly ADPropertyDefinition InformCallerOfVoiceMailAnalysis = new ADPropertyDefinition("InformCallerOfVoiceMailAnalysis", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 8192), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 8192), null, null);

		public static readonly ADPropertyDefinition AllowVoiceNotification = new ADPropertyDefinition("AllowVoiceNotification", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMMailboxPolicySchema.UMEnabledFlags2Bits
		}, null, ADObject.FlagGetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 16384), ADObject.FlagSetterDelegate(UMMailboxPolicySchema.UMEnabledFlags2Bits, 16384), null, null);
	}
}
