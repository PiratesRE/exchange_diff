using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OwaClientLogger : ExtensibleLogger
	{
		public OwaClientLogger() : base(new OwaClientLogConfiguration(), OwaClientLogger.GetEscapeLineBreaksConfigValue())
		{
		}

		public static void Initialize()
		{
			if (OwaClientLogger.instance == null)
			{
				OwaClientLogger.instance = new OwaClientLogger();
			}
		}

		public static void TestInitialize(IExtensibleLogger logger)
		{
			OwaClientLogger.instance = logger;
		}

		public static void AppendToLog(ILogEvent logEvent)
		{
			if (OwaClientLogger.instance != null)
			{
				OwaClientLogger.instance.LogEvent(logEvent);
			}
		}

		public static void AppendToLog(IEnumerable<ILogEvent> logEvent)
		{
			if (OwaClientLogger.instance != null)
			{
				OwaClientLogger.instance.LogEvent(logEvent);
			}
		}

		private static bool GetEscapeLineBreaksConfigValue()
		{
			return AppConfigLoader.GetConfigBoolValue("Test_OWAClientLogEscapeLineBreaks", LogRowFormatter.DefaultEscapeLineBreaks);
		}

		private static IExtensibleLogger instance;
	}
}
