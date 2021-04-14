using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	internal static class SetupLogger
	{
		public static bool IsPrereqLogging
		{
			get
			{
				return SetupLogger.Logger.IsPrereqLogging;
			}
			set
			{
				SetupLogger.Logger.IsPrereqLogging = value;
			}
		}

		public static LocalizedString HalfAsterixLine
		{
			get
			{
				return SetupLogger.halfAsterixLine;
			}
		}

		public static ISetupLogger Logger
		{
			get
			{
				ISetupLogger result;
				if ((result = SetupLogger.logger) == null)
				{
					result = (SetupLogger.logger = new SetupLoggerImpl());
				}
				return result;
			}
			set
			{
				SetupLogger.logger = value;
			}
		}

		public static void StartSetupLogging()
		{
			SetupLogger.Logger.StartLogging();
		}

		public static void StopSetupLogging()
		{
			SetupLogger.Logger.StopLogging();
		}

		public static void Log(LocalizedString localizedString)
		{
			SetupLogger.Logger.Log(localizedString);
		}

		public static void LogWarning(LocalizedString localizedString)
		{
			SetupLogger.Logger.LogWarning(localizedString);
		}

		public static void LogError(Exception e)
		{
			SetupLogger.Logger.LogError(e);
		}

		public static void TraceEnter(params object[] arguments)
		{
			SetupLogger.Logger.TraceEnter(arguments);
		}

		public static void TraceExit()
		{
			SetupLogger.Logger.TraceExit();
		}

		public static void IncreaseIndentation(LocalizedString tag)
		{
			SetupLogger.Logger.IncreaseIndentation(tag);
		}

		public static void DecreaseIndentation()
		{
			SetupLogger.Logger.DecreaseIndentation();
		}

		public static void LogForDataMining(string task, DateTime startTime)
		{
			SetupLogger.Logger.LogForDataMining(task, startTime);
		}

		private static readonly LocalizedString halfAsterixLine = new LocalizedString("**************");

		private static ISetupLogger logger;
	}
}
