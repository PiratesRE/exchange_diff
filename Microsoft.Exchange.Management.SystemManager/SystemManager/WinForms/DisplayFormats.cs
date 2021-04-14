using System;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class DisplayFormats
	{
		public static ICustomTextConverter Default
		{
			get
			{
				return TextConverter.DefaultConverter;
			}
		}

		public static ICustomTextConverter EnhancedTimeSpanAsDays
		{
			get
			{
				return DisplayFormats.enhancedTimeSpanAsDays;
			}
		}

		public static ICustomTextConverter EnhancedTimeSpanAsHours
		{
			get
			{
				return DisplayFormats.enhancedTimeSpanAsHours;
			}
		}

		public static ICustomTextConverter EnhancedTimeSpanAsMinutes
		{
			get
			{
				return DisplayFormats.enhancedTimeSpanAsMinutes;
			}
		}

		public static ICustomTextConverter EnhancedTimeSpanAsSeconds
		{
			get
			{
				return DisplayFormats.enhancedTimeSpanAsSeconds;
			}
		}

		public static ICustomTextConverter EnhancedTimeSpanAsDetailedFormat
		{
			get
			{
				return DisplayFormats.enhancedTimeSpanAsDetailedFormat;
			}
		}

		public static ICustomTextConverter ByteQuantifiedSizeAsKb
		{
			get
			{
				return DisplayFormats.byteQuantifiedSizeAsKb;
			}
		}

		public static ICustomTextConverter ByteQuantifiedSizeAsMb
		{
			get
			{
				return DisplayFormats.byteQuantifiedSizeAsMb;
			}
		}

		public static ICustomTextConverter ByteQuantifiedSizeAsDetailedFormat
		{
			get
			{
				return DisplayFormats.byteQuantifiedSizeAsDetailedFormat;
			}
		}

		public static ICustomTextConverter BooleanAsStatus
		{
			get
			{
				return DisplayFormats.booleanAsStatus;
			}
		}

		public static ICustomTextConverter BooleanAsMountStatus
		{
			get
			{
				return DisplayFormats.booleanAsMountStatus;
			}
		}

		public static ICustomTextConverter BooleanAsYesNo
		{
			get
			{
				return DisplayFormats.booleanAsYesNo;
			}
		}

		public static ICustomTextConverter AdObjectIdAsName
		{
			get
			{
				return DisplayFormats.adObjectIdAsName;
			}
		}

		public static ICustomTextConverter NullableDateTimeAsLogTime
		{
			get
			{
				return DisplayFormats.nullableDateTimeAsLogTime;
			}
		}

		public static ICustomTextConverter IntegerAsPercentage
		{
			get
			{
				return DisplayFormats.integerAsPercentage;
			}
		}

		public static ICustomTextConverter SmtpDomainWithSubdomainsListAsString
		{
			get
			{
				return DisplayFormats.smtpDomainWithSubdomainsListAsStringCoverter;
			}
		}

		private static readonly ICustomTextConverter enhancedTimeSpanAsDays = new EnhancedTimeSpanAsDaysCoverter();

		private static readonly ICustomTextConverter enhancedTimeSpanAsHours = new EnhancedTimeSpanAsHoursCoverter();

		private static readonly ICustomTextConverter enhancedTimeSpanAsMinutes = new EnhancedTimeSpanAsMinutesCoverter();

		private static readonly ICustomTextConverter enhancedTimeSpanAsSeconds = new EnhancedTimeSpanAsSecondsCoverter();

		private static readonly ICustomTextConverter enhancedTimeSpanAsDetailedFormat = new EnhancedTimeSpanAsDetailedFormatCoverter();

		private static readonly ICustomTextConverter byteQuantifiedSizeAsKb = new ByteQuantifiedSizeAsKbCoverter();

		private static readonly ICustomTextConverter byteQuantifiedSizeAsMb = new ByteQuantifiedSizeAsMbCoverter();

		private static readonly ICustomTextConverter byteQuantifiedSizeAsDetailedFormat = new ByteQuantifiedSizeAsDetailedFormatCoverter();

		private static readonly ICustomTextConverter booleanAsStatus = new BooleanAsStatusCoverter();

		private static readonly ICustomTextConverter booleanAsMountStatus = new BooleanAsMountStatusCoverter();

		private static readonly ICustomTextConverter booleanAsYesNo = new BooleanAsYesNoConverter();

		private static readonly ICustomTextConverter adObjectIdAsName = new AdObjectIdAsNameCoverter();

		private static readonly ICustomTextConverter nullableDateTimeAsLogTime = new NullableDateTimeAsLogTimeCoverter();

		private static readonly ICustomTextConverter integerAsPercentage = new IntegerAsPercentageConverter();

		private static readonly ICustomTextConverter smtpDomainWithSubdomainsListAsStringCoverter = new SmtpDomainWithSubdomainsListAsStringCoverter();
	}
}
