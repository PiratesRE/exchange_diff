using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class CalendarStrings
	{
		static CalendarStrings()
		{
			CalendarStrings.stringIDs.Add(1651570042U, "PropertyOutsideOfComponent");
			CalendarStrings.stringIDs.Add(2204331647U, "EmptyParameterName");
			CalendarStrings.stringIDs.Add(1363187990U, "ByMonthDayOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(3611840403U, "ByMonthDayOutOfRange");
			CalendarStrings.stringIDs.Add(1799497654U, "ByYearDayOutOfRange");
			CalendarStrings.stringIDs.Add(1321868363U, "InvalidReaderState");
			CalendarStrings.stringIDs.Add(2313244969U, "ByMinuteOutOfRange");
			CalendarStrings.stringIDs.Add(3235259578U, "NotAllComponentsClosed");
			CalendarStrings.stringIDs.Add(1503765159U, "MultivalueNotPermittedOnWkSt");
			CalendarStrings.stringIDs.Add(1299235546U, "BySecondOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(1730444109U, "ExpectedS");
			CalendarStrings.stringIDs.Add(1379077484U, "StreamHasAlreadyBeenClosed");
			CalendarStrings.stringIDs.Add(4275464087U, "UnknownRecurrenceProperty");
			CalendarStrings.stringIDs.Add(3837794365U, "ByYearDayOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(3721804466U, "ByMonthOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(1219670026U, "ParameterNameMissing");
			CalendarStrings.stringIDs.Add(4016499044U, "InvalidValueTypeForProperty");
			CalendarStrings.stringIDs.Add(3742088828U, "InvalidDateTimeLength");
			CalendarStrings.stringIDs.Add(3387824169U, "UnknownDayOfWeek");
			CalendarStrings.stringIDs.Add(3544698171U, "CountLessThanZero");
			CalendarStrings.stringIDs.Add(1730444116U, "ExpectedZ");
			CalendarStrings.stringIDs.Add(861244994U, "ExpectedWOrD");
			CalendarStrings.stringIDs.Add(3395058257U, "ParametersNotPermittedOnComponentTag");
			CalendarStrings.stringIDs.Add(226115752U, "DurationDataNotEndedProperly");
			CalendarStrings.stringIDs.Add(1672858014U, "ByDayOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(1478317853U, "PropertyTruncated");
			CalendarStrings.stringIDs.Add(1956952952U, "IntervalMustBePositive");
			CalendarStrings.stringIDs.Add(3830022583U, "MultivalueNotPermittedOnCount");
			CalendarStrings.stringIDs.Add(3872783322U, "ExpectedPlusMinus");
			CalendarStrings.stringIDs.Add(3537482179U, "InvalidParameterValue");
			CalendarStrings.stringIDs.Add(250418233U, "EmptyPropertyName");
			CalendarStrings.stringIDs.Add(2555380673U, "InvalidParameterId");
			CalendarStrings.stringIDs.Add(1730444106U, "ExpectedT");
			CalendarStrings.stringIDs.Add(1565539446U, "ByWeekNoOutOfRange");
			CalendarStrings.stringIDs.Add(472912471U, "BySecondOutOfRange");
			CalendarStrings.stringIDs.Add(1730444131U, "ExpectedM");
			CalendarStrings.stringIDs.Add(2118029539U, "UnknownFrequencyValue");
			CalendarStrings.stringIDs.Add(4188603411U, "InvalidTimeFormat");
			CalendarStrings.stringIDs.Add(2880341830U, "InvalidCharacterInQuotedString");
			CalendarStrings.stringIDs.Add(3073868642U, "CountNotPermittedWithUntil");
			CalendarStrings.stringIDs.Add(716036182U, "InvalidState");
			CalendarStrings.stringIDs.Add(3099580008U, "StreamMustAllowRead");
			CalendarStrings.stringIDs.Add(3740878728U, "StreamIsReadOnly");
			CalendarStrings.stringIDs.Add(3606802482U, "InvalidToken");
			CalendarStrings.stringIDs.Add(1140546334U, "InvalidDateFormat");
			CalendarStrings.stringIDs.Add(680249202U, "InvalidStateForOperation");
			CalendarStrings.stringIDs.Add(2830151143U, "InvalidCharacterInRecurrence");
			CalendarStrings.stringIDs.Add(2466566499U, "InvalidCharacterInPropertyName");
			CalendarStrings.stringIDs.Add(1110024291U, "ParameterValuesCannotContainDoubleQuote");
			CalendarStrings.stringIDs.Add(27421126U, "ByMinuteOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(2905506355U, "InvalidCharacterInParameterText");
			CalendarStrings.stringIDs.Add(1730444110U, "ExpectedP");
			CalendarStrings.stringIDs.Add(1177599875U, "CannotStartOnProperty");
			CalendarStrings.stringIDs.Add(738606535U, "StreamMustAllowWrite");
			CalendarStrings.stringIDs.Add(797792475U, "UnexpectedEndOfStream");
			CalendarStrings.stringIDs.Add(3550209514U, "WkStOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(318352933U, "InvalidValueFormat");
			CalendarStrings.stringIDs.Add(1631030892U, "InvalidCharacter");
			CalendarStrings.stringIDs.Add(608577876U, "CannotStartPropertyInParameter");
			CalendarStrings.stringIDs.Add(861974305U, "InvalidCharacterInParameterName");
			CalendarStrings.stringIDs.Add(1430833556U, "EndTagWithoutBegin");
			CalendarStrings.stringIDs.Add(1184842547U, "InvalidStateToWriteValue");
			CalendarStrings.stringIDs.Add(3689402045U, "InvalidTimeStringLength");
			CalendarStrings.stringIDs.Add(3128171671U, "InvalidComponentId");
			CalendarStrings.stringIDs.Add(3822021355U, "ByHourOutOfRange");
			CalendarStrings.stringIDs.Add(3394177979U, "ByMonthOutOfRange");
			CalendarStrings.stringIDs.Add(1045189704U, "ExpectedTAfterDate");
			CalendarStrings.stringIDs.Add(3955879836U, "UntilNotPermittedWithCount");
			CalendarStrings.stringIDs.Add(3018409416U, "UtcOffsetTimespanCannotContainDays");
			CalendarStrings.stringIDs.Add(3513540288U, "MultivalueNotPermittedOnFreq");
			CalendarStrings.stringIDs.Add(2311921382U, "ExpectedHMS");
			CalendarStrings.stringIDs.Add(1558925939U, "InvalidPropertyId");
			CalendarStrings.stringIDs.Add(1793182840U, "CountOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(1766527382U, "MultivalueNotPermittedOnUntil");
			CalendarStrings.stringIDs.Add(3223577931U, "ValueAlreadyRead");
			CalendarStrings.stringIDs.Add(2347585579U, "MultivalueNotPermittedOnInterval");
			CalendarStrings.stringIDs.Add(401491057U, "BySetPosOutOfRange");
			CalendarStrings.stringIDs.Add(2998051666U, "InvalidUtcOffsetLength");
			CalendarStrings.stringIDs.Add(411844418U, "ComponentNameMismatch");
			CalendarStrings.stringIDs.Add(3608438261U, "NonwritableStream");
			CalendarStrings.stringIDs.Add(3273146598U, "IntervalOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(522329279U, "InvalidCharacterInPropertyValue");
			CalendarStrings.stringIDs.Add(2128475288U, "ByHourOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(1746426911U, "UntilOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(3590683541U, "OffsetOutOfRange");
			CalendarStrings.stringIDs.Add(3651971229U, "EmptyComponentName");
			CalendarStrings.stringIDs.Add(2363588404U, "BySetPosOnlyPermittedOnce");
			CalendarStrings.stringIDs.Add(3089920790U, "StateMustBeComponent");
			CalendarStrings.stringIDs.Add(4182777507U, "ByWeekNoOnlyPermittedOnce");
		}

		public static string PropertyOutsideOfComponent
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("PropertyOutsideOfComponent");
			}
		}

		public static string EmptyParameterName
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("EmptyParameterName");
			}
		}

		public static string ByMonthDayOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMonthDayOnlyPermittedOnce");
			}
		}

		public static string ByMonthDayOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMonthDayOutOfRange");
			}
		}

		public static string ByYearDayOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByYearDayOutOfRange");
			}
		}

		public static string InvalidReaderState
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidReaderState");
			}
		}

		public static string ByMinuteOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMinuteOutOfRange");
			}
		}

		public static string NotAllComponentsClosed
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("NotAllComponentsClosed");
			}
		}

		public static string MultivalueNotPermittedOnWkSt
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("MultivalueNotPermittedOnWkSt");
			}
		}

		public static string BySecondOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("BySecondOnlyPermittedOnce");
			}
		}

		public static string ExpectedS
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedS");
			}
		}

		public static string StreamHasAlreadyBeenClosed
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("StreamHasAlreadyBeenClosed");
			}
		}

		public static string UnknownRecurrenceProperty
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UnknownRecurrenceProperty");
			}
		}

		public static string ByYearDayOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByYearDayOnlyPermittedOnce");
			}
		}

		public static string ByMonthOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMonthOnlyPermittedOnce");
			}
		}

		public static string ParameterNameMissing
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ParameterNameMissing");
			}
		}

		public static string InvalidValueTypeForProperty
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidValueTypeForProperty");
			}
		}

		public static string InvalidDateTimeLength
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidDateTimeLength");
			}
		}

		public static string UnknownDayOfWeek
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UnknownDayOfWeek");
			}
		}

		public static string CountLessThanZero
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("CountLessThanZero");
			}
		}

		public static string ExpectedZ
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedZ");
			}
		}

		public static string ExpectedWOrD
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedWOrD");
			}
		}

		public static string ParametersNotPermittedOnComponentTag
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ParametersNotPermittedOnComponentTag");
			}
		}

		public static string DurationDataNotEndedProperly
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("DurationDataNotEndedProperly");
			}
		}

		public static string ByDayOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByDayOnlyPermittedOnce");
			}
		}

		public static string PropertyTruncated
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("PropertyTruncated");
			}
		}

		public static string IntervalMustBePositive
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("IntervalMustBePositive");
			}
		}

		public static string MultivalueNotPermittedOnCount
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("MultivalueNotPermittedOnCount");
			}
		}

		public static string ExpectedPlusMinus
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedPlusMinus");
			}
		}

		public static string InvalidParameterValue
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidParameterValue");
			}
		}

		public static string EmptyPropertyName
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("EmptyPropertyName");
			}
		}

		public static string InvalidParameterId
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidParameterId");
			}
		}

		public static string ExpectedT
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedT");
			}
		}

		public static string ByWeekNoOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByWeekNoOutOfRange");
			}
		}

		public static string BySecondOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("BySecondOutOfRange");
			}
		}

		public static string ExpectedM
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedM");
			}
		}

		public static string UnknownFrequencyValue
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UnknownFrequencyValue");
			}
		}

		public static string InvalidTimeFormat
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidTimeFormat");
			}
		}

		public static string InvalidCharacterInQuotedString
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInQuotedString");
			}
		}

		public static string CountNotPermittedWithUntil
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("CountNotPermittedWithUntil");
			}
		}

		public static string InvalidState
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidState");
			}
		}

		public static string StreamMustAllowRead
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("StreamMustAllowRead");
			}
		}

		public static string StreamIsReadOnly
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("StreamIsReadOnly");
			}
		}

		public static string InvalidToken
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidToken");
			}
		}

		public static string InvalidDateFormat
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidDateFormat");
			}
		}

		public static string InvalidStateForOperation
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidStateForOperation");
			}
		}

		public static string InvalidCharacterInRecurrence
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInRecurrence");
			}
		}

		public static string InvalidCharacterInPropertyName
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInPropertyName");
			}
		}

		public static string ParameterValuesCannotContainDoubleQuote
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ParameterValuesCannotContainDoubleQuote");
			}
		}

		public static string ByMinuteOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMinuteOnlyPermittedOnce");
			}
		}

		public static string InvalidCharacterInParameterText
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInParameterText");
			}
		}

		public static string ExpectedP
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedP");
			}
		}

		public static string CannotStartOnProperty
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("CannotStartOnProperty");
			}
		}

		public static string StreamMustAllowWrite
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("StreamMustAllowWrite");
			}
		}

		public static string UnexpectedEndOfStream
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UnexpectedEndOfStream");
			}
		}

		public static string WkStOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("WkStOnlyPermittedOnce");
			}
		}

		public static string InvalidValueFormat
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidValueFormat");
			}
		}

		public static string InvalidCharacter
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacter");
			}
		}

		public static string CannotStartPropertyInParameter
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("CannotStartPropertyInParameter");
			}
		}

		public static string InvalidCharacterInParameterName
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInParameterName");
			}
		}

		public static string EndTagWithoutBegin
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("EndTagWithoutBegin");
			}
		}

		public static string InvalidStateToWriteValue
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidStateToWriteValue");
			}
		}

		public static string InvalidTimeStringLength
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidTimeStringLength");
			}
		}

		public static string InvalidComponentId
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidComponentId");
			}
		}

		public static string ByHourOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByHourOutOfRange");
			}
		}

		public static string ByMonthOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByMonthOutOfRange");
			}
		}

		public static string ExpectedTAfterDate
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedTAfterDate");
			}
		}

		public static string UntilNotPermittedWithCount
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UntilNotPermittedWithCount");
			}
		}

		public static string UtcOffsetTimespanCannotContainDays
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UtcOffsetTimespanCannotContainDays");
			}
		}

		public static string MultivalueNotPermittedOnFreq
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("MultivalueNotPermittedOnFreq");
			}
		}

		public static string ExpectedHMS
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ExpectedHMS");
			}
		}

		public static string InvalidPropertyId
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidPropertyId");
			}
		}

		public static string CountOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("CountOnlyPermittedOnce");
			}
		}

		public static string MultivalueNotPermittedOnUntil
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("MultivalueNotPermittedOnUntil");
			}
		}

		public static string ValueAlreadyRead
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ValueAlreadyRead");
			}
		}

		public static string MultivalueNotPermittedOnInterval
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("MultivalueNotPermittedOnInterval");
			}
		}

		public static string BySetPosOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("BySetPosOutOfRange");
			}
		}

		public static string InvalidUtcOffsetLength
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidUtcOffsetLength");
			}
		}

		public static string ComponentNameMismatch
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ComponentNameMismatch");
			}
		}

		public static string NonwritableStream
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("NonwritableStream");
			}
		}

		public static string IntervalOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("IntervalOnlyPermittedOnce");
			}
		}

		public static string InvalidCharacterInPropertyValue
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("InvalidCharacterInPropertyValue");
			}
		}

		public static string ByHourOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByHourOnlyPermittedOnce");
			}
		}

		public static string UntilOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("UntilOnlyPermittedOnce");
			}
		}

		public static string OffsetOutOfRange
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("OffsetOutOfRange");
			}
		}

		public static string EmptyComponentName
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("EmptyComponentName");
			}
		}

		public static string BySetPosOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("BySetPosOnlyPermittedOnce");
			}
		}

		public static string StateMustBeComponent
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("StateMustBeComponent");
			}
		}

		public static string ByWeekNoOnlyPermittedOnce
		{
			get
			{
				return CalendarStrings.ResourceManager.GetString("ByWeekNoOnlyPermittedOnce");
			}
		}

		public static string GetLocalizedString(CalendarStrings.IDs key)
		{
			return CalendarStrings.ResourceManager.GetString(CalendarStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(89);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.CalendarStrings", typeof(CalendarStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			PropertyOutsideOfComponent = 1651570042U,
			EmptyParameterName = 2204331647U,
			ByMonthDayOnlyPermittedOnce = 1363187990U,
			ByMonthDayOutOfRange = 3611840403U,
			ByYearDayOutOfRange = 1799497654U,
			InvalidReaderState = 1321868363U,
			ByMinuteOutOfRange = 2313244969U,
			NotAllComponentsClosed = 3235259578U,
			MultivalueNotPermittedOnWkSt = 1503765159U,
			BySecondOnlyPermittedOnce = 1299235546U,
			ExpectedS = 1730444109U,
			StreamHasAlreadyBeenClosed = 1379077484U,
			UnknownRecurrenceProperty = 4275464087U,
			ByYearDayOnlyPermittedOnce = 3837794365U,
			ByMonthOnlyPermittedOnce = 3721804466U,
			ParameterNameMissing = 1219670026U,
			InvalidValueTypeForProperty = 4016499044U,
			InvalidDateTimeLength = 3742088828U,
			UnknownDayOfWeek = 3387824169U,
			CountLessThanZero = 3544698171U,
			ExpectedZ = 1730444116U,
			ExpectedWOrD = 861244994U,
			ParametersNotPermittedOnComponentTag = 3395058257U,
			DurationDataNotEndedProperly = 226115752U,
			ByDayOnlyPermittedOnce = 1672858014U,
			PropertyTruncated = 1478317853U,
			IntervalMustBePositive = 1956952952U,
			MultivalueNotPermittedOnCount = 3830022583U,
			ExpectedPlusMinus = 3872783322U,
			InvalidParameterValue = 3537482179U,
			EmptyPropertyName = 250418233U,
			InvalidParameterId = 2555380673U,
			ExpectedT = 1730444106U,
			ByWeekNoOutOfRange = 1565539446U,
			BySecondOutOfRange = 472912471U,
			ExpectedM = 1730444131U,
			UnknownFrequencyValue = 2118029539U,
			InvalidTimeFormat = 4188603411U,
			InvalidCharacterInQuotedString = 2880341830U,
			CountNotPermittedWithUntil = 3073868642U,
			InvalidState = 716036182U,
			StreamMustAllowRead = 3099580008U,
			StreamIsReadOnly = 3740878728U,
			InvalidToken = 3606802482U,
			InvalidDateFormat = 1140546334U,
			InvalidStateForOperation = 680249202U,
			InvalidCharacterInRecurrence = 2830151143U,
			InvalidCharacterInPropertyName = 2466566499U,
			ParameterValuesCannotContainDoubleQuote = 1110024291U,
			ByMinuteOnlyPermittedOnce = 27421126U,
			InvalidCharacterInParameterText = 2905506355U,
			ExpectedP = 1730444110U,
			CannotStartOnProperty = 1177599875U,
			StreamMustAllowWrite = 738606535U,
			UnexpectedEndOfStream = 797792475U,
			WkStOnlyPermittedOnce = 3550209514U,
			InvalidValueFormat = 318352933U,
			InvalidCharacter = 1631030892U,
			CannotStartPropertyInParameter = 608577876U,
			InvalidCharacterInParameterName = 861974305U,
			EndTagWithoutBegin = 1430833556U,
			InvalidStateToWriteValue = 1184842547U,
			InvalidTimeStringLength = 3689402045U,
			InvalidComponentId = 3128171671U,
			ByHourOutOfRange = 3822021355U,
			ByMonthOutOfRange = 3394177979U,
			ExpectedTAfterDate = 1045189704U,
			UntilNotPermittedWithCount = 3955879836U,
			UtcOffsetTimespanCannotContainDays = 3018409416U,
			MultivalueNotPermittedOnFreq = 3513540288U,
			ExpectedHMS = 2311921382U,
			InvalidPropertyId = 1558925939U,
			CountOnlyPermittedOnce = 1793182840U,
			MultivalueNotPermittedOnUntil = 1766527382U,
			ValueAlreadyRead = 3223577931U,
			MultivalueNotPermittedOnInterval = 2347585579U,
			BySetPosOutOfRange = 401491057U,
			InvalidUtcOffsetLength = 2998051666U,
			ComponentNameMismatch = 411844418U,
			NonwritableStream = 3608438261U,
			IntervalOnlyPermittedOnce = 3273146598U,
			InvalidCharacterInPropertyValue = 522329279U,
			ByHourOnlyPermittedOnce = 2128475288U,
			UntilOnlyPermittedOnce = 1746426911U,
			OffsetOutOfRange = 3590683541U,
			EmptyComponentName = 3651971229U,
			BySetPosOnlyPermittedOnce = 2363588404U,
			StateMustBeComponent = 3089920790U,
			ByWeekNoOnlyPermittedOnce = 4182777507U
		}
	}
}
