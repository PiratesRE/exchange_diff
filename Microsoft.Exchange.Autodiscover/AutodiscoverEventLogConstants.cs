using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class AutodiscoverEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrWebException = new ExEventLog.EventTuple(3221225473U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrWebAnonymousRequest = new ExEventLog.EventTuple(3221225474U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrWebBasicAuthRequest = new ExEventLog.EventTuple(3221225475U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrCoreNoProvidersFound = new ExEventLog.EventTuple(3221225573U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrCoreInvalidRedirectionUrl = new ExEventLog.EventTuple(3221225574U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrProviderRedirectServerCertificate = new ExEventLog.EventTuple(3221225673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrProviderFormatException = new ExEventLog.EventTuple(3221225674U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrProviderRegistryMisconfiguration = new ExEventLog.EventTuple(3221225675U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrProviderOabMisconfiguration = new ExEventLog.EventTuple(3221225676U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrProviderOabNotExist = new ExEventLog.EventTuple(3221225677U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreElementIsEmpty = new ExEventLog.EventTuple(2147484749U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreElementsAreEmpty = new ExEventLog.EventTuple(2147484750U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreValidationError = new ExEventLog.EventTuple(2147484751U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreValidationException = new ExEventLog.EventTuple(2147484752U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderNotFound = new ExEventLog.EventTuple(2147484753U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderLoadError = new ExEventLog.EventTuple(2147484754U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderFileLoadException = new ExEventLog.EventTuple(2147484756U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderReflectionTypeLoadException = new ExEventLog.EventTuple(2147484757U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderBadImageFormatException = new ExEventLog.EventTuple(2147484758U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderSecurityException = new ExEventLog.EventTuple(2147484759U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderFileNotFoundException = new ExEventLog.EventTuple(2147484760U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCoreProviderAttributeException = new ExEventLog.EventTuple(2147484761U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCorePerfCounterInitializationFailed = new ExEventLog.EventTuple(2147484762U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnCorePerfCounterIncrementFailed = new ExEventLog.EventTuple(2147484763U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarnProvErrorResponse = new ExEventLog.EventTuple(2147484849U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoWebApplicationStart = new ExEventLog.EventTuple(2001U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoWebApplicationStop = new ExEventLog.EventTuple(2002U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoWebSessionStart = new ExEventLog.EventTuple(2003U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoWebSessionSuccess = new ExEventLog.EventTuple(2004U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoWebSessionFailure = new ExEventLog.EventTuple(2005U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoCoreProvidersLoaded = new ExEventLog.EventTuple(2101U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoProvRedirectionResponse = new ExEventLog.EventTuple(2201U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoProvConfigurationResponse = new ExEventLog.EventTuple(2202U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_StreamInsightsDataUploadFailed = new ExEventLog.EventTuple(2147485851U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoProvRedirectBypassConfigurationResponse = new ExEventLog.EventTuple(2204U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Core = 1,
			Web,
			Provider
		}

		internal enum Message : uint
		{
			ErrWebException = 3221225473U,
			ErrWebAnonymousRequest,
			ErrWebBasicAuthRequest,
			ErrCoreNoProvidersFound = 3221225573U,
			ErrCoreInvalidRedirectionUrl,
			ErrProviderRedirectServerCertificate = 3221225673U,
			ErrProviderFormatException,
			ErrProviderRegistryMisconfiguration,
			ErrProviderOabMisconfiguration,
			ErrProviderOabNotExist,
			WarnCoreElementIsEmpty = 2147484749U,
			WarnCoreElementsAreEmpty,
			WarnCoreValidationError,
			WarnCoreValidationException,
			WarnCoreProviderNotFound,
			WarnCoreProviderLoadError,
			WarnCoreProviderFileLoadException = 2147484756U,
			WarnCoreProviderReflectionTypeLoadException,
			WarnCoreProviderBadImageFormatException,
			WarnCoreProviderSecurityException,
			WarnCoreProviderFileNotFoundException,
			WarnCoreProviderAttributeException,
			WarnCorePerfCounterInitializationFailed,
			WarnCorePerfCounterIncrementFailed,
			WarnProvErrorResponse = 2147484849U,
			InfoWebApplicationStart = 2001U,
			InfoWebApplicationStop,
			InfoWebSessionStart,
			InfoWebSessionSuccess,
			InfoWebSessionFailure,
			InfoCoreProvidersLoaded = 2101U,
			InfoProvRedirectionResponse = 2201U,
			InfoProvConfigurationResponse,
			StreamInsightsDataUploadFailed = 2147485851U,
			InfoProvRedirectBypassConfigurationResponse = 2204U
		}
	}
}
