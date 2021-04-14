using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class WatsonEventLog
	{
		public static bool TryLogCrash(object[] parameters)
		{
			bool result;
			try
			{
				WatsonEventLog.eventLogger.LogEvent(CommonEventLogConstants.Tuple_ExchangeCrash, null, parameters);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryLogReportError(object[] parameters)
		{
			bool result;
			try
			{
				WatsonEventLog.eventLogger.LogEvent(CommonEventLogConstants.Tuple_WatsonReportError, null, parameters);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private static ExEventLog eventLogger = new ExEventLog(new Guid("{173CD7C5-7EDB-4476-94D4-5D424A7D32B4}"), "MSExchange Common");
	}
}
