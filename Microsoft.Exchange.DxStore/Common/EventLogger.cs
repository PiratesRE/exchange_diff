using System;

namespace Microsoft.Exchange.DxStore.Common
{
	internal static class EventLogger
	{
		public static IDxStoreEventLogger Instance { get; set; }

		public static void LogErr(string formatString, params object[] args)
		{
			IDxStoreEventLogger instance = EventLogger.Instance;
			if (instance != null)
			{
				instance.Log(DxEventSeverity.Error, 0, formatString, args);
			}
		}

		public static void LogInfo(string formatString, params object[] args)
		{
			IDxStoreEventLogger instance = EventLogger.Instance;
			if (instance != null)
			{
				instance.Log(DxEventSeverity.Info, 0, formatString, args);
			}
		}
	}
}
