using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface IExEventLog
	{
		ExEventSourceInfo EventSource { get; }

		bool IsEventCategoryEnabled(short categoryNumber, ExEventLog.EventLevel level);

		void SetEventPeriod(int seconds);

		bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs);

		bool LogEventWithExtraData(ExEventLog.EventTuple tuple, string periodicKey, byte[] extraData, params object[] messageArgs);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2, object arg3);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs);

		bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs);

		bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs);
	}
}
