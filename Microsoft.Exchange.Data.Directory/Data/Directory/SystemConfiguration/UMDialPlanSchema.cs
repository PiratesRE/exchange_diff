using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UMDialPlanSchema : ADConfigurationObjectSchema
	{
		internal static GetterDelegate AudioCodecGetterDelegate(ProviderPropertyDefinition propertyDef, ProviderPropertyDefinition legacyPropertyDef, AudioCodecEnum? defaultValueIfInvalid)
		{
			return delegate(IPropertyBag bag)
			{
				AudioCodecEnum? audioCodecEnum = (AudioCodecEnum?)bag[propertyDef];
				audioCodecEnum = ((audioCodecEnum != null) ? new AudioCodecEnum?(audioCodecEnum.Value) : ((AudioCodecEnum?)bag[legacyPropertyDef]));
				return (audioCodecEnum == null || audioCodecEnum.Value > AudioCodecEnum.Mp3) ? defaultValueIfInvalid : audioCodecEnum;
			};
		}

		internal static SetterDelegate AudioCodecSetterDelegate(ProviderPropertyDefinition propertyDef, ProviderPropertyDefinition legacyPropertyDef)
		{
			return delegate(object value, IPropertyBag bag)
			{
				AudioCodecEnum? audioCodecEnum = (AudioCodecEnum?)value;
				bag[propertyDef] = audioCodecEnum;
				if (audioCodecEnum != null && audioCodecEnum <= AudioCodecEnum.Gsm)
				{
					bag[legacyPropertyDef] = audioCodecEnum;
				}
			};
		}

		internal const int MaxCallDurationUpperBoundMinutes = 120;

		internal const string DigitStringOrEmptyRegEx = "^[\\d]*$";

		private const string DigitStringRegEx = "^[\\d]+$";

		private const string NumberFormatRegEx = "^[1-9]+\\+[1-9]+$";

		private static readonly CharacterConstraint numberConstraint = new CharacterConstraint(new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9'
		}, true);

		public static readonly ADPropertyDefinition NumberOfDigitsInExtension = new ADPropertyDefinition("NumberOfDigitsInExtension", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMNumberingPlanDigits", ADPropertyDefinitionFlags.PersistDefaultValue, 5, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 20)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogonFailuresBeforeDisconnect = new ADPropertyDefinition("LogonFailuresBeforeDisconnect", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMLogonFailuresBeforeDisconnect", ADPropertyDefinitionFlags.PersistDefaultValue, 3, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 20)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AccessTelephoneNumbers = new ADPropertyDefinition("AccessTelephoneNumbers", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMVoiceMailPilotNumbers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FaxEnabled = new ADPropertyDefinition("FaxEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMFaxEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InputFailuresBeforeDisconnect = new ADPropertyDefinition("InputFailuresBeforeDisconnect", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMCallFailuresToDisconnect", ADPropertyDefinitionFlags.PersistDefaultValue, 3, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 999)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OutsideLineAccessCode = new ADPropertyDefinition("OutsideLineAccessCode", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMTrunkAccessCode", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 16),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringOrEmptyPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition DialByNamePrimary = new ADPropertyDefinition("DialByNamePrimary", ExchangeObjectVersion.Exchange2007, typeof(DialByNamePrimaryEnum), "msExchUMDialByNamePrimary", ADPropertyDefinitionFlags.PersistDefaultValue, DialByNamePrimaryEnum.LastFirst, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DialByNamePrimaryEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DialByNameSecondary = new ADPropertyDefinition("DialByNameSecondary", ExchangeObjectVersion.Exchange2007, typeof(DialByNameSecondaryEnum), "msExchUMDialByNameSecondary", ADPropertyDefinitionFlags.PersistDefaultValue, DialByNameSecondaryEnum.SMTPAddress, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DialByNameSecondaryEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AudioCodecLegacy = new ADPropertyDefinition("AudioCodecLegacy", ExchangeObjectVersion.Exchange2007, typeof(AudioCodecEnum), "msExchUMAudioCodec", ADPropertyDefinitionFlags.PersistDefaultValue, AudioCodecEnum.Wma, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AudioCodecEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AudioCodec2 = new ADPropertyDefinition("AudioCodec2", ExchangeObjectVersion.Exchange2007, typeof(AudioCodecEnum?), "msExchUMAudioCodec2", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AudioCodec = new ADPropertyDefinition("AudioCodec", ExchangeObjectVersion.Exchange2007, typeof(AudioCodecEnum), null, ADPropertyDefinitionFlags.Calculated, AudioCodecEnum.Mp3, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AudioCodecEnum))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.AudioCodecLegacy,
			UMDialPlanSchema.AudioCodec2
		}, null, UMDialPlanSchema.AudioCodecGetterDelegate(UMDialPlanSchema.AudioCodec2, UMDialPlanSchema.AudioCodecLegacy, new AudioCodecEnum?(AudioCodecEnum.Mp3)), UMDialPlanSchema.AudioCodecSetterDelegate(UMDialPlanSchema.AudioCodec2, UMDialPlanSchema.AudioCodecLegacy), null, null);

		public static readonly ADPropertyDefinition DefaultLanguageTemp = new ADPropertyDefinition("DefaultLanguageTemp", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMDefaultLanguage", ADPropertyDefinitionFlags.PersistDefaultValue, UMLanguage.DefaultLanguage.LCID, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1048576)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultLanguage = new ADPropertyDefinition("DefaultLanguage", ExchangeObjectVersion.Exchange2007, typeof(UMLanguage), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.DefaultLanguageTemp
		}, null, delegate(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				int lcid = (int)propertyBag[UMDialPlanSchema.DefaultLanguageTemp];
				result = new UMLanguage(lcid);
			}
			catch (ArgumentException)
			{
				result = new UMLanguage(UMLanguage.DefaultLanguage.LCID);
			}
			return result;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			UMLanguage umlanguage = value as UMLanguage;
			if (umlanguage != null)
			{
				propertyBag[UMDialPlanSchema.DefaultLanguageTemp] = umlanguage.LCID;
				return;
			}
			propertyBag[UMDialPlanSchema.DefaultLanguageTemp] = null;
		}, null, null);

		public static readonly ADPropertyDefinition URIType = new ADPropertyDefinition("URIType", ExchangeObjectVersion.Exchange2007, typeof(UMUriType), "msExchUMDialPlanURIType", ADPropertyDefinitionFlags.PersistDefaultValue, UMUriType.TelExtn, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMUriType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SubscriberType = new ADPropertyDefinition("SubscriberType", ExchangeObjectVersion.Exchange2010, typeof(UMSubscriberType), "msExchUMDialPlanSubscriberType", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, UMSubscriberType.Enterprise, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMSubscriberType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GlobalCallRoutingScheme = new ADPropertyDefinition("GlobalCallRoutingScheme", ExchangeObjectVersion.Exchange2010, typeof(UMGlobalCallRoutingScheme), "msExchUMGlobalCallRoutingScheme", ADPropertyDefinitionFlags.PersistDefaultValue, UMGlobalCallRoutingScheme.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMGlobalCallRoutingScheme))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VoIPSecurity = new ADPropertyDefinition("VoIPSecurity", ExchangeObjectVersion.Exchange2007, typeof(UMVoIPSecurityType), "msExchUMDialPlanVoipSecurity", ADPropertyDefinitionFlags.PersistDefaultValue, UMVoIPSecurityType.Unsecured, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMVoIPSecurityType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxCallDuration = new ADPropertyDefinition("MaxCallDuration", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMMaxCallDuration", ADPropertyDefinitionFlags.PersistDefaultValue, 30, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(10, 120)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxRecordingDuration = new ADPropertyDefinition("MaxRecordingDuration", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMMaxRecordingDuration", ADPropertyDefinitionFlags.PersistDefaultValue, 20, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 100)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecordingIdleTimeout = new ADPropertyDefinition("RecordingIdleTimeout", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMRecordingIdleTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, 5, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(2, 10)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PilotIdentifierList = new ADPropertyDefinition("PilotIdentifierList", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMDialPlanDialedNumbers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMServers = new ADPropertyDefinition("UMServers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMServerDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMMailboxPolicies = new ADPropertyDefinition("MailboxPolicies", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMMailboxPolicyDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMAutoAttendants = new ADPropertyDefinition("UMAutoAttendants", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMAutoAttendantDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutomaticSpeechRecognitionEnabled = new ADPropertyDefinition("AutomaticSpeechRecognitionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMASREnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WelcomeGreetingEnabled = new ADPropertyDefinition("WelcomeGreetingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMWelcomeGreetingEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MatchedNameSelectionMethod = new ADPropertyDefinition("MatchedNameSelectionMethod", ExchangeObjectVersion.Exchange2007, typeof(DisambiguationFieldEnum), "msExchUMDisambiguationField", ADPropertyDefinitionFlags.None, DisambiguationFieldEnum.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DisambiguationFieldEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InfoAnnouncementEnabled = new ADPropertyDefinition("InfoAnnouncementEnabled", ExchangeObjectVersion.Exchange2007, typeof(InfoAnnouncementEnabledEnum), "msExchUMInfoAnnouncementStatus", ADPropertyDefinitionFlags.None, InfoAnnouncementEnabledEnum.False, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(InfoAnnouncementEnabledEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PhoneContext = new ADPropertyDefinition("PhoneContext", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMPhoneContext", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WelcomeGreetingFilename = new ADPropertyDefinition("WelcomeGreetingFilename", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMWelcomeGreetingFile", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, null, null);

		public static readonly ADPropertyDefinition InfoAnnouncementFilename = SharedPropertyDefinitions.InfoAnnouncementFilename;

		public static readonly ADPropertyDefinition OperatorExtension = new ADPropertyDefinition("OperatorExtension", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMOperatorExtension", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 20),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringOrEmptyPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition Extension = new ADPropertyDefinition("Extension", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMOverrideExtension", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 20),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition DefaultOutboundCallingLineId = new ADPropertyDefinition("DefaultOutboundCallingLineId", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMDefaultOutboundCallingLineID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternationalAccessCode = new ADPropertyDefinition("InternationalAccessCode", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMInternationalAccessCode", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 4),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringOrEmptyPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition NationalNumberPrefix = new ADPropertyDefinition("NationalNumberPrefix", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMNationalNumberPrefix", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 4),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringOrEmptyPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition CountryOrRegionCode = new ADPropertyDefinition("CountryOrRegionCode", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMCountryCode", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(1, 4),
			new RegexConstraint("^[\\d]*$", DataStrings.DigitStringOrEmptyPatternDescription),
			UMDialPlanSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition InCountryOrRegionNumberFormat = new ADPropertyDefinition("InCountryOrRegionNumberFormat", ExchangeObjectVersion.Exchange2007, typeof(NumberFormat), "msExchUMInCountryNumberFormat", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternationalNumberFormat = new ADPropertyDefinition("InternationalNumberFormat", ExchangeObjectVersion.Exchange2007, typeof(NumberFormat), "msExchUMInternationalNumberFormat", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CallSomeoneEnabled = new ADPropertyDefinition("CallSomeoneEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMCallSomeoneEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContactScope = new ADPropertyDefinition("ContactScope", ExchangeObjectVersion.Exchange2007, typeof(CallSomeoneScopeEnum), "msExchUMCallSomeoneScope", ADPropertyDefinitionFlags.None, CallSomeoneScopeEnum.DialPlan, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(CallSomeoneScopeEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ContactAddressLists = SharedPropertyDefinitions.ContactAddressLists;

		public static readonly ADPropertyDefinition ContactAddressList = new ADPropertyDefinition("ContactAddressList", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.ContactAddressLists
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[UMDialPlanSchema.ContactAddressLists];
			ADObjectId result = null;
			if (multiValuedProperty != null)
			{
				ADObjectId[] array = multiValuedProperty.ToArray();
				if (array != null && array.Length > 0)
				{
					result = array[0];
				}
			}
			return result;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			ADObjectId value2 = value as ADObjectId;
			propertyBag[UMDialPlanSchema.ContactAddressLists] = new MultiValuedProperty<ADObjectId>(value2);
		}, null, null);

		public static readonly ADPropertyDefinition SendVoiceMsgEnabled = new ADPropertyDefinition("SendVoiceMsgEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMSendVoiceMessageEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowDialPlanSubscribers = new ADPropertyDefinition("AllowDialPlanSubscribers", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMDialPlanSubscribersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowExtensions = new ADPropertyDefinition("AllowExtensions", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMExtensionLengthNumbersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedInCountryOrRegionGroups = SharedPropertyDefinitions.AllowedInCountryOrRegionGroups;

		public static readonly ADPropertyDefinition AllowedInternationalGroups = SharedPropertyDefinitions.AllowedInternationalGroups;

		public static readonly ADPropertyDefinition ConfiguredInCountryOrRegionGroups = new ADPropertyDefinition("ConfiguredInCountryOrRegionGroups", ExchangeObjectVersion.Exchange2007, typeof(DialGroupEntry), "msExchUMAvailableInCountryGroups", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfiguredInternationalGroups = new ADPropertyDefinition("ConfiguredInternationalGroups", ExchangeObjectVersion.Exchange2007, typeof(DialGroupEntry), "msExchUMAvailableInternationalGroups", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMAutoAttendant = new ADPropertyDefinition("UMAutoAttendant", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMDialPlanDefaultAutoAttendantLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedUsers = new ADPropertyDefinition("AssociatedUsers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMRecipientDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedPolicies = new ADPropertyDefinition("AssociatedPolicies", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMMailboxPolicyDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PromptPublishingPoint = new ADPropertyDefinition("LegacyPromptPublishingPoint", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchPromptPublishingPoint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		private static readonly ADPropertyDefinition HuntGroups = new ADPropertyDefinition("HuntGroups", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMHuntGroupDialPlanBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FDSPollingInterval = new ADPropertyDefinition("FDSPollingInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPollInterval", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		private static readonly ADPropertyDefinition DialPlanFlags = new ADPropertyDefinition("DialPlanFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMDialPlanFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		private static readonly ADPropertyDefinition DialPlanFlags2 = new ADPropertyDefinition("DialPlanFlags2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchUMDialPlanFlags2", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMIPGateway = new ADPropertyDefinition("UMIPGateway", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.HuntGroups
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[UMDialPlanSchema.HuntGroups];
			List<string> list = new List<string>();
			List<ADObjectId> list2 = new List<ADObjectId>();
			foreach (ADObjectId adobjectId in multiValuedProperty)
			{
				if (!list.Contains(adobjectId.Parent.DistinguishedName))
				{
					list2.Add(ADObjectId.ParseExtendedDN(adobjectId.Parent.ToExtendedDN(), (OrganizationId)propertyBag[ADObjectSchema.OrganizationId]));
					list.Add(adobjectId.Parent.DistinguishedName);
				}
			}
			return new MultiValuedProperty<ADObjectId>(list2.ToArray());
		}, null, null, null);

		public static readonly ADPropertyDefinition TUIPromptEditingEnabled = new ADPropertyDefinition("TUIPromptEditingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.DialPlanFlags
		}, null, ADObject.FlagGetterDelegate(UMDialPlanSchema.DialPlanFlags, 1), ADObject.FlagSetterDelegate(UMDialPlanSchema.DialPlanFlags, 1), null, null);

		public static readonly ADPropertyDefinition PersonalAutoAttendantEnabled = new ADPropertyDefinition("PersonalAutoAttendantEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.DialPlanFlags2
		}, null, ADObject.FlagGetterDelegate(UMDialPlanSchema.DialPlanFlags2, 2), ADObject.FlagSetterDelegate(UMDialPlanSchema.DialPlanFlags2, 2), null, null);

		public static readonly ADPropertyDefinition SipResourceIdentifierRequired = new ADPropertyDefinition("SipResourceIdentifierRequired", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMDialPlanSchema.DialPlanFlags2
		}, null, ADObject.FlagGetterDelegate(UMDialPlanSchema.DialPlanFlags2, 4), ADObject.FlagSetterDelegate(UMDialPlanSchema.DialPlanFlags2, 4), null, null);

		public static readonly ADPropertyDefinition EquivalentDialPlanPhoneContexts = new ADPropertyDefinition("EquivalentDialPlanPhoneContexts", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMEquivalentDialPlanPhoneContexts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NumberingPlanFormats = new ADPropertyDefinition("NumberingPlanFormats", ExchangeObjectVersion.Exchange2010, typeof(UMNumberingPlanFormat), "msExchUMCallingLineIdFormats", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowHeuristicADCallingLineIdResolutionValue = new ADPropertyDefinition("AllowHeuristicADCallingLineIdResolutionValue", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAllowHeuristicADCallingLineIdResolution", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowHeuristicADCallingLineIdResolution = new ADPropertyDefinition("AllowHeuristicADCallingLineIdResolution", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			UMDialPlanSchema.AllowHeuristicADCallingLineIdResolutionValue
		}, null, (IPropertyBag propertyBag) => 0 != (int)propertyBag[UMDialPlanSchema.AllowHeuristicADCallingLineIdResolutionValue], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[UMDialPlanSchema.AllowHeuristicADCallingLineIdResolutionValue] = (((bool)value) ? 1 : 0);
		}, null, null);

		public static readonly ADPropertyDefinition PromptChangeKey = new ADPropertyDefinition("PromptChangeKey", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMDialPlanPromptChangeKey", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
