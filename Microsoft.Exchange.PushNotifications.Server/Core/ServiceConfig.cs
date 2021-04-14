using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal static class ServiceConfig
	{
		public static bool IsWriteStackTraceOnResponseEnabled { get; private set; } = AppConfigLoader.GetConfigBoolValue("WriteStackTraceOnResponse", false);

		public static int ConfigurationRefreshRateInMinutes { get; private set; } = AppConfigLoader.GetConfigIntValue("ConfigurationRefreshRateInMinutes", 0, int.MaxValue, 15);

		public static bool IgnoreCertificateErrors { get; private set; } = false;

		public const string WriteStackTraceOnResponseKey = "WriteStackTraceOnResponse";

		public const string ConfigurationRefreshRateInMinutesKey = "ConfigurationRefreshRateInMinutes";

		public const string IgnoreCertificateErrorsKey = "IgnoreCertificateErrors";

		public const string UseDebugTenantIdKey = "UseDebugTenantId";

		public const bool WriteStackTraceOnResponseDefaultValue = false;

		public const int ConfigurationRefreshRateInMinutesDefaultValue = 15;

		public const bool IgnoreCertificateErrorsDefaultValue = false;
	}
}
