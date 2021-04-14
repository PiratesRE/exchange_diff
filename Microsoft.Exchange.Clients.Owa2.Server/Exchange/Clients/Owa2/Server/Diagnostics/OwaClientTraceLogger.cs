using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaClientTraceLogger : ExtensibleLogger
	{
		public OwaClientTraceLogger() : base(new OwaClientTraceLogConfiguration(), OwaClientTraceLogger.GetEscapeLineBreaksConfigValue())
		{
		}

		public static void Initialize()
		{
			if (OwaClientTraceLogger.instance == null)
			{
				OwaClientTraceLogger.instance = new OwaClientTraceLogger();
			}
		}

		public static void AppendToLog(ILogEvent logEvent)
		{
			if (OwaClientTraceLogger.instance != null)
			{
				OwaClientTraceLogger.instance.LogEvent(logEvent);
			}
		}

		public static void AppendToLog(IEnumerable<ILogEvent> logEvent)
		{
			if (OwaClientTraceLogger.instance != null)
			{
				OwaClientTraceLogger.instance.LogEvent(logEvent);
			}
		}

		private static bool GetEscapeLineBreaksConfigValue()
		{
			return AppConfigLoader.GetConfigBoolValue("Test_OWAClientLogEscapeLineBreaks", LogRowFormatter.DefaultEscapeLineBreaks);
		}

		private static OwaClientTraceLogger instance;
	}
}
