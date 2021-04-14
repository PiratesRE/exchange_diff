using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class FederatedDirectoryLogger
	{
		public static void AppendToLog(ILogEvent logEvent)
		{
			ArgumentValidator.ThrowIfNull("logEvent", logEvent);
			FederatedDirectoryLogger.AppendEventData(logEvent.GetEventData());
			FederatedDirectoryLogger.Instance.Value.LogEvent(logEvent);
		}

		private static void AppendEventData(ICollection<KeyValuePair<string, object>> eventData)
		{
			eventData.Add(FederatedDirectoryLogger.ApplicationIdKeyValuePair.Value);
		}

		private static readonly Lazy<ExtensibleLogger> Instance = new Lazy<ExtensibleLogger>(() => new ExtensibleLogger(FederatedDirectoryLogConfiguration.Default));

		private static readonly string ApplicationIdName = "ApplicationId";

		private static readonly Lazy<KeyValuePair<string, object>> ApplicationIdKeyValuePair = new Lazy<KeyValuePair<string, object>>(() => new KeyValuePair<string, object>(FederatedDirectoryLogger.ApplicationIdName, ApplicationName.Current.Name));
	}
}
