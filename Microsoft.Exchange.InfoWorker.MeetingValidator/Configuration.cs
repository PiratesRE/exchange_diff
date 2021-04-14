using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal static class Configuration
	{
		static Configuration()
		{
			Configuration.parameterCollection = ConfigurationManager.AppSettings;
		}

		internal static int DLExpansionLimit
		{
			get
			{
				if (Configuration.dlExpansionLimit == -1)
				{
					Configuration.dlExpansionLimit = Configuration.ReadIntValue("DLExpansionLimit", 10);
				}
				return Configuration.dlExpansionLimit;
			}
			set
			{
				Configuration.dlExpansionLimit = value;
			}
		}

		internal static int MaxNumberOfLocalMeetingsPerBatch
		{
			get
			{
				if (Configuration.maxNumberOfLocalMeetingsPerBatch == -1)
				{
					Configuration.maxNumberOfLocalMeetingsPerBatch = Configuration.ReadIntValue("MaxNumberOfLocalMeetingsPerBatch", 200);
				}
				return Configuration.maxNumberOfLocalMeetingsPerBatch;
			}
			set
			{
				Configuration.maxNumberOfLocalMeetingsPerBatch = value;
			}
		}

		internal static int WebRequestTimeoutInSeconds
		{
			get
			{
				if (Configuration.webRequestTimeoutInSeconds == -1)
				{
					Configuration.webRequestTimeoutInSeconds = Configuration.ReadIntValue("WebRequestTimeoutInSeconds", 25);
				}
				return Configuration.webRequestTimeoutInSeconds;
			}
		}

		internal static int MaxMeetingsToProcessIncludingDuplicates
		{
			get
			{
				if (Configuration.maxMeetingsToProcessIncludingDuplicates == -1)
				{
					Configuration.maxMeetingsToProcessIncludingDuplicates = Configuration.ReadIntValue("MaxMeetingsToProcessIncludingDuplicates", 1000);
				}
				return Configuration.maxMeetingsToProcessIncludingDuplicates;
			}
		}

		internal static int MaxMeetingsPerMailbox
		{
			get
			{
				if (Configuration.maxMeetingsPerMailbox == -1)
				{
					Configuration.maxMeetingsPerMailbox = Configuration.ReadIntValue("MaxMeetingsPerMailbox", 500);
				}
				return Configuration.maxMeetingsPerMailbox;
			}
		}

		internal static bool IgnoreCertificateErrors
		{
			get
			{
				if (Configuration.ignoreCertificateErrors == null)
				{
					Configuration.ignoreCertificateErrors = new bool?(Configuration.ReadBooleanValue("IgnoreCertificateErrors", false));
				}
				return Configuration.ignoreCertificateErrors.Value;
			}
			set
			{
				Configuration.ignoreCertificateErrors = new bool?(value);
			}
		}

		internal static bool CalendarRepairOppositeMailboxEwsEnabled
		{
			get
			{
				if (Configuration.calendarRepairOppositeMailboxEwsEnabled == null)
				{
					Configuration.calendarRepairOppositeMailboxEwsEnabled = new bool?(Configuration.ReadBooleanValue("CalendarRepairOppositeMailboxEwsEnabled", true));
				}
				return Configuration.calendarRepairOppositeMailboxEwsEnabled.Value;
			}
		}

		internal static bool CalendarRepairForceEwsUsage
		{
			get
			{
				if (Configuration.calendarRepairForceEwsUsage == null)
				{
					Configuration.calendarRepairForceEwsUsage = new bool?(Configuration.ReadBooleanValue("CalendarRepairForceEwsUsage", false));
				}
				return Configuration.calendarRepairForceEwsUsage.Value;
			}
			set
			{
				Configuration.calendarRepairForceEwsUsage = new bool?(value);
			}
		}

		private static int ReadIntValue(string name, int defaultValue)
		{
			int result;
			if (int.TryParse(Configuration.parameterCollection[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static bool ReadBooleanValue(string name, bool defaultValue)
		{
			bool result;
			if (bool.TryParse(Configuration.parameterCollection[name], out result))
			{
				return result;
			}
			return defaultValue;
		}

		private const int DefaultDLExpansionLimit = 10;

		private const bool DefaultIgnoreCertificateErrors = false;

		private const bool DefaultCalendarRepairOppositeMailboxEwsEnabled = true;

		private const bool DefaultCalendarRepairForceEwsUsage = false;

		private const int DefaultMaxNumberOfLocalMeetingsPerBatch = 200;

		private const int DefaultWebRequestTimeoutInSeconds = 25;

		private const int DefaultMaxMeetingsPerMailbox = 500;

		private const int DefaultMaxMeetingsToProcessIncludingDuplicates = 1000;

		private static NameValueCollection parameterCollection;

		private static int maxNumberOfLocalMeetingsPerBatch = -1;

		private static int dlExpansionLimit = -1;

		private static int webRequestTimeoutInSeconds = -1;

		private static int maxMeetingsPerMailbox = -1;

		private static int maxMeetingsToProcessIncludingDuplicates = -1;

		private static bool? ignoreCertificateErrors;

		private static bool? calendarRepairOppositeMailboxEwsEnabled;

		private static bool? calendarRepairForceEwsUsage;
	}
}
