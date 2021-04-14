using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UMAutoAttendantSchema : ADConfigurationObjectSchema
	{
		private static AutoAttendantSettings GetAASettingsFromPropertyBag(IPropertyBag propertyBag, AASettingsEnum settingsType)
		{
			ADPropertyDefinition propertyDefinition = (settingsType == AASettingsEnum.BusinessHourSettings) ? UMAutoAttendantSchema.BusinessHourFeatures : UMAutoAttendantSchema.AfterHourFeatures;
			string text = (string)propertyBag[propertyDefinition];
			if (string.IsNullOrEmpty(text))
			{
				return new AutoAttendantSettings();
			}
			return AutoAttendantSettings.FromXml(text);
		}

		private static readonly ADPropertyDefinition AvailableLanguages = new ADPropertyDefinition("AvailableLanguages", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMAvailableLanguages", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1048576)
		}, PropertyDefinitionConstraint.None, null, null);

		private static CharacterConstraint numberConstraint = new CharacterConstraint(new char[]
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

		public static readonly ADPropertyDefinition AllowDialPlanSubscribers = new ADPropertyDefinition("AllowDialPlanSubscribers", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMDialPlanSubscribersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CallSomeoneEnabled = new ADPropertyDefinition("CallSomeoneEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMCallSomeoneEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContactScope = new ADPropertyDefinition("ContactScope", ExchangeObjectVersion.Exchange2007, typeof(DialScopeEnum), "msExchUMCallSomeoneScope", ADPropertyDefinitionFlags.None, DialScopeEnum.DialPlan, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DialScopeEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContactAddressLists = SharedPropertyDefinitions.ContactAddressLists;

		public static readonly ADPropertyDefinition ContactAddressList = new ADPropertyDefinition("ContactAddressList", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.ContactAddressLists
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[UMAutoAttendantSchema.ContactAddressLists];
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
			propertyBag[UMAutoAttendantSchema.ContactAddressLists] = new MultiValuedProperty<ADObjectId>(value2);
		}, null, null);

		public static readonly ADPropertyDefinition SendVoiceMsgEnabled = new ADPropertyDefinition("SendVoiceMsgEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMSendVoiceMessageEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MatchedNameSelectionMethod = new ADPropertyDefinition("MatchedNameSelectionMethod", ExchangeObjectVersion.Exchange2007, typeof(AutoAttendantDisambiguationFieldEnum), "msExchUMDisambiguationField", ADPropertyDefinitionFlags.None, AutoAttendantDisambiguationFieldEnum.InheritFromDialPlan, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AutoAttendantDisambiguationFieldEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowExtensions = new ADPropertyDefinition("AllowExtensions", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMExtensionLengthNumbersAllowed", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutomaticSpeechRecognitionEnabled = new ADPropertyDefinition("AutomaticSpeechRecognitionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMASREnabled", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedInCountryOrRegionGroups = SharedPropertyDefinitions.AllowedInCountryOrRegionGroups;

		public static readonly ADPropertyDefinition AllowedInternationalGroups = SharedPropertyDefinitions.AllowedInternationalGroups;

		public static readonly ADPropertyDefinition AfterHourFeatures = new ADPropertyDefinition("AfterHourFeatures", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMAutoAttendantAfterHourFeatures", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BusinessHourFeatures = new ADPropertyDefinition("BusinessHourFeatures", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMAutoAttendantBusinessHourFeatures", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HolidayScheduleBase = new ADPropertyDefinition("HolidayScheduleBase", ExchangeObjectVersion.Exchange2007, typeof(HolidaySchedule), "msExchUMAutoAttendantHolidaySchedule", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BusinessHoursSchedule = new ADPropertyDefinition("BusinessHoursSchedule", ExchangeObjectVersion.Exchange2007, typeof(ScheduleInterval[]), "msExchUMAutoAttendantBusinessHourSchedule", ADPropertyDefinitionFlags.Binary, new ScheduleInterval[]
		{
			new ScheduleInterval(DayOfWeek.Sunday, 0, 0, DayOfWeek.Saturday, 23, 45),
			new ScheduleInterval(DayOfWeek.Saturday, 23, 45, DayOfWeek.Sunday, 0, 0)
		}, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PilotIdentifierList = new ADPropertyDefinition("PilotIdentifierList", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMAutoAttendantDialedNumbers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMDialPlan = new ADPropertyDefinition("UMDialPlan", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMAutoAttendantDialPlanLink", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DTMFFallbackAutoAttendant = new ADPropertyDefinition("DTMFFallbackAutoAttendant", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMDTMFFallbackAutoAttendantLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoAttendantFlags = new ADPropertyDefinition("AutoAttendantFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMAutoAttendantFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TimeZone = new ADPropertyDefinition("TimeZone", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.TimeZoneKeyName ?? string.Empty;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			if (value == null)
			{
				aasettingsFromPropertyBag.TimeZoneKeyName = string.Empty;
			}
			else
			{
				aasettingsFromPropertyBag.TimeZoneKeyName = (string)value;
			}
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition HolidaySchedule = new ADPropertyDefinition("HolidaySchedule", ExchangeObjectVersion.Exchange2007, typeof(HolidaySchedule), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.HolidayScheduleBase
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<HolidaySchedule> multiValuedProperty = (MultiValuedProperty<HolidaySchedule>)propertyBag[UMAutoAttendantSchema.HolidayScheduleBase];
			if (multiValuedProperty == null)
			{
				return null;
			}
			MultiValuedProperty<HolidaySchedule> multiValuedProperty2 = new MultiValuedProperty<HolidaySchedule>();
			if (multiValuedProperty.Count > 0)
			{
				UMTimeZone umtimeZone = (UMTimeZone)propertyBag[UMAutoAttendantSchema.TimeZoneName];
				foreach (HolidaySchedule holidaySchedule in multiValuedProperty)
				{
					multiValuedProperty2.Add(new HolidaySchedule(holidaySchedule.Name, holidaySchedule.Greeting, (DateTime)umtimeZone.TimeZone.ConvertDateTime(new ExDateTime(ExTimeZone.UtcTimeZone, holidaySchedule.StartDate)), (DateTime)umtimeZone.TimeZone.ConvertDateTime(new ExDateTime(ExTimeZone.UtcTimeZone, holidaySchedule.EndDate))));
				}
			}
			return multiValuedProperty2;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<HolidaySchedule> multiValuedProperty = (MultiValuedProperty<HolidaySchedule>)value;
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				MultiValuedProperty<HolidaySchedule> multiValuedProperty2 = new MultiValuedProperty<HolidaySchedule>();
				UMTimeZone umtimeZone = (UMTimeZone)propertyBag[UMAutoAttendantSchema.TimeZoneName];
				foreach (HolidaySchedule holidaySchedule in multiValuedProperty)
				{
					multiValuedProperty2.Add(new HolidaySchedule(holidaySchedule.Name, holidaySchedule.Greeting, (DateTime)ExTimeZone.UtcTimeZone.ConvertDateTime(new ExDateTime(umtimeZone.TimeZone, holidaySchedule.StartDate)), (DateTime)ExTimeZone.UtcTimeZone.ConvertDateTime(new ExDateTime(umtimeZone.TimeZone, holidaySchedule.EndDate))));
				}
				propertyBag[UMAutoAttendantSchema.HolidayScheduleBase] = multiValuedProperty2;
				return;
			}
			propertyBag[UMAutoAttendantSchema.HolidayScheduleBase] = multiValuedProperty;
		}, null, null);

		public static readonly ADPropertyDefinition TimeZoneName = new ADPropertyDefinition("TimeZoneName", ExchangeObjectVersion.Exchange2007, typeof(UMTimeZone), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			string id = (string)propertyBag[UMAutoAttendantSchema.TimeZone];
			return new UMTimeZone(id);
		}, delegate(object value, IPropertyBag propertyBag)
		{
			UMTimeZone umtimeZone = value as UMTimeZone;
			string value2 = string.Empty;
			if (umtimeZone != null)
			{
				value2 = umtimeZone.Id;
			}
			propertyBag[UMAutoAttendantSchema.TimeZone] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition PromptChangeKey = new ADPropertyDefinition("PromptChangeKey", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchUMAutoAttendantPromptChangeKey", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AutoAttendantFlags
		}, null, ADObject.FlagGetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 1), ADObject.FlagSetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 1), null, null);

		public static readonly ADPropertyDefinition Language = new ADPropertyDefinition("Language", ExchangeObjectVersion.Exchange2007, typeof(UMLanguage), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AvailableLanguages
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)propertyBag[UMAutoAttendantSchema.AvailableLanguages];
			int num = UMLanguage.DefaultLanguage.LCID;
			int[] array = multiValuedProperty.ToArray();
			if (array.Length > 0)
			{
				num = array[0];
			}
			object result;
			try
			{
				result = new UMLanguage(num);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.UMAutoAttendantTracer.TraceError<ADPropertyDefinition, int>(0L, "AA: {0} Invalid culture: {1}", ADObjectSchema.Name, num);
				result = new UMLanguage(UMLanguage.DefaultLanguage.LCID);
			}
			return result;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			UMLanguage umlanguage = (UMLanguage)value;
			if (umlanguage != null)
			{
				propertyBag[UMAutoAttendantSchema.AvailableLanguages] = new MultiValuedProperty<int>(umlanguage.LCID);
				return;
			}
			propertyBag[UMAutoAttendantSchema.AvailableLanguages] = null;
		}, null, null);

		public static readonly ADPropertyDefinition InfoAnnouncementFilename = SharedPropertyDefinitions.InfoAnnouncementFilename;

		public static readonly ADPropertyDefinition OperatorExtension = new ADPropertyDefinition("OperatorExtension", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMOperatorExtension", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 20),
			UMAutoAttendantSchema.numberConstraint
		}, null, null);

		public static readonly ADPropertyDefinition DefaultMailboxLegacyDN = new ADPropertyDefinition("DefaultMailboxLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMDefaultMailbox", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BusinessName = new ADPropertyDefinition("BusinessName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMBusinessName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, null, null);

		public static readonly ADPropertyDefinition StarOutToDialPlanEnabled = new ADPropertyDefinition("StarOutToDialPlanEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AutoAttendantFlags
		}, null, ADObject.FlagGetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 16), ADObject.FlagSetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 16), null, null);

		public static readonly ADPropertyDefinition ForwardCallsToDefaultMailbox = new ADPropertyDefinition("ForwardCallsToDefaultMailbox", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AutoAttendantFlags
		}, null, ADObject.FlagGetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 8), ADObject.FlagSetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 8), null, null);

		public static readonly ADPropertyDefinition InfoAnnouncementEnabled = new ADPropertyDefinition("InfoAnnouncementEnabled", ExchangeObjectVersion.Exchange2007, typeof(InfoAnnouncementEnabledEnum), "msExchUMInfoAnnouncementStatus", ADPropertyDefinitionFlags.None, InfoAnnouncementEnabledEnum.False, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(InfoAnnouncementEnabledEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NameLookupEnabled = new ADPropertyDefinition("NameLookupEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AutoAttendantFlags
		}, null, ADObject.FlagGetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 2), ADObject.FlagSetterDelegate(UMAutoAttendantSchema.AutoAttendantFlags, 2), null, null);

		public static readonly ADPropertyDefinition BusinessLocation = new ADPropertyDefinition("BusinessLocation", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMBusinessLocation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition WeekStartDay = new ADPropertyDefinition("WeekStartDayName", ExchangeObjectVersion.Exchange2010, typeof(DayOfWeek), "msExchUMWeekStartDay", ADPropertyDefinitionFlags.None, DayOfWeek.Monday, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DayOfWeek))
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursWelcomeGreetingFilename = new ADPropertyDefinition("BusinessHoursWelcomeGreetingFilename", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.WelcomeGreetingFilename;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			if (value == null)
			{
				aasettingsFromPropertyBag.WelcomeGreetingFilename = null;
			}
			else
			{
				aasettingsFromPropertyBag.WelcomeGreetingFilename = (string)value;
			}
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursWelcomeGreetingEnabled = new ADPropertyDefinition("BusinessHoursWelcomeGreetingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.WelcomeGreetingEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			aasettingsFromPropertyBag.WelcomeGreetingEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursMainMenuCustomPromptFilename = new ADPropertyDefinition("BusinessHoursMainMenuCustomPromptFilename", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.MainMenuCustomPromptFilename;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			aasettingsFromPropertyBag.MainMenuCustomPromptFilename = (string)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursMainMenuCustomPromptEnabled = new ADPropertyDefinition("BusinessHoursMainMenuCustomPromptEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.MainMenuCustomPromptEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			aasettingsFromPropertyBag.MainMenuCustomPromptEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursTransferToOperatorEnabled = new ADPropertyDefinition("BusinessHoursTransferToOperatorEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.TransferToOperatorEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			aasettingsFromPropertyBag.TransferToOperatorEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursKeyMappingEnabled = new ADPropertyDefinition("BusinessHoursKeyMappingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			return aasettingsFromPropertyBag.KeyMappingEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			aasettingsFromPropertyBag.KeyMappingEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition BusinessHoursKeyMapping = new ADPropertyDefinition("BusinessHoursKeyMapping", ExchangeObjectVersion.Exchange2007, typeof(CustomMenuKeyMapping), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.BusinessHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			if (aasettingsFromPropertyBag.KeyMapping == null)
			{
				return new MultiValuedProperty<CustomMenuKeyMapping>();
			}
			MultiValuedProperty<CustomMenuKeyMapping> multiValuedProperty = new MultiValuedProperty<CustomMenuKeyMapping>(false, null, aasettingsFromPropertyBag.KeyMapping);
			multiValuedProperty.Sort();
			return multiValuedProperty;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.BusinessHourSettings);
			if (value == null)
			{
				aasettingsFromPropertyBag.KeyMapping = null;
			}
			else
			{
				aasettingsFromPropertyBag.KeyMapping = ((MultiValuedProperty<CustomMenuKeyMapping>)value).ToArray();
			}
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.BusinessHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursWelcomeGreetingFilename = new ADPropertyDefinition("AfterHoursWelcomeGreetingFilename", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.WelcomeGreetingFilename;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			if (value == null)
			{
				aasettingsFromPropertyBag.WelcomeGreetingFilename = null;
			}
			else
			{
				aasettingsFromPropertyBag.WelcomeGreetingFilename = (string)value;
			}
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursWelcomeGreetingEnabled = new ADPropertyDefinition("AfterHoursWelcomeGreetingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.WelcomeGreetingEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			aasettingsFromPropertyBag.WelcomeGreetingEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursMainMenuCustomPromptFilename = new ADPropertyDefinition("AfterHoursMainMenuCustomPromptFilename", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255),
			new RegexConstraint("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase, DataStrings.CustomGreetingFilePatternDescription)
		}, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.MainMenuCustomPromptFilename;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			aasettingsFromPropertyBag.MainMenuCustomPromptFilename = (string)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursMainMenuCustomPromptEnabled = new ADPropertyDefinition("AfterHoursMainMenuCustomPromptEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.MainMenuCustomPromptEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			aasettingsFromPropertyBag.MainMenuCustomPromptEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursTransferToOperatorEnabled = new ADPropertyDefinition("AfterHoursTransferToOperatorEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.TransferToOperatorEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			aasettingsFromPropertyBag.TransferToOperatorEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursKeyMappingEnabled = new ADPropertyDefinition("AfterHoursKeyMappingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			return aasettingsFromPropertyBag.KeyMappingEnabled;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			aasettingsFromPropertyBag.KeyMappingEnabled = (bool)value;
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);

		public static readonly ADPropertyDefinition AfterHoursKeyMapping = new ADPropertyDefinition("AfterHoursKeyMapping", ExchangeObjectVersion.Exchange2007, typeof(CustomMenuKeyMapping), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMAutoAttendantSchema.AfterHourFeatures
		}, null, delegate(IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			if (aasettingsFromPropertyBag.KeyMapping == null)
			{
				return new MultiValuedProperty<CustomMenuKeyMapping>();
			}
			return new MultiValuedProperty<CustomMenuKeyMapping>(false, null, aasettingsFromPropertyBag.KeyMapping);
		}, delegate(object value, IPropertyBag propertyBag)
		{
			AutoAttendantSettings aasettingsFromPropertyBag = UMAutoAttendantSchema.GetAASettingsFromPropertyBag(propertyBag, AASettingsEnum.AfterHourSettings);
			if (value == null)
			{
				aasettingsFromPropertyBag.KeyMapping = null;
			}
			else
			{
				aasettingsFromPropertyBag.KeyMapping = ((MultiValuedProperty<CustomMenuKeyMapping>)value).ToArray();
			}
			string value2 = SerializationHelper.Serialize(aasettingsFromPropertyBag);
			propertyBag[UMAutoAttendantSchema.AfterHourFeatures] = value2;
		}, null, null);
	}
}
