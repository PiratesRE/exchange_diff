using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal interface ICalendarInteropLog
	{
		void LogEntry(IStoreSession session, string entry, params object[] args);

		void LogEntry(IStoreSession session, Exception e, bool logWatsonReport, string entry, params object[] args);

		void SafeLogEntry(IStoreSession session, Exception exceptionToReport, bool logWatsonReport, string entry, params object[] args);
	}
}
