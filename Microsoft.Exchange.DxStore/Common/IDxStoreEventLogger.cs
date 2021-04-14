using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface IDxStoreEventLogger
	{
		void Log(DxEventSeverity severity, int id, string formatString, params object[] args);

		void LogPeriodic(string periodicKey, TimeSpan periodicDuration, DxEventSeverity severity, int id, string formatString, params object[] args);
	}
}
