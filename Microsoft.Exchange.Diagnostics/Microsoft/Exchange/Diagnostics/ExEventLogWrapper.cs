using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ExEventLogWrapper : IExEventLog
	{
		public ExEventLogWrapper(ExEventLog eventLog)
		{
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			this.eventLog = eventLog;
		}

		public ExEventSourceInfo EventSource
		{
			get
			{
				return this.eventLog.EventSource;
			}
		}

		public bool IsEventCategoryEnabled(short categoryNumber, ExEventLog.EventLevel level)
		{
			return this.eventLog.IsEventCategoryEnabled(categoryNumber, level);
		}

		public void SetEventPeriod(int seconds)
		{
			this.eventLog.SetEventPeriod(seconds);
		}

		public bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			return this.eventLog.LogEvent(tuple, periodicKey, messageArgs);
		}

		public bool LogEventWithExtraData(ExEventLog.EventTuple tuple, string periodicKey, byte[] extraData, params object[] messageArgs)
		{
			return this.eventLog.LogEventWithExtraData(tuple, periodicKey, extraData, messageArgs);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, periodicKey, arg0);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, periodicKey, arg0, arg1);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, periodicKey, arg0, arg1, arg2);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2, object arg3)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, new object[]
			{
				periodicKey,
				arg0,
				arg1,
				arg2,
				arg3
			});
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, periodicKey, messageArgs);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			return this.eventLog.LogEvent(organizationId, tuple, periodicKey, out fEventSuppressed, messageArgs);
		}

		public bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			return this.eventLog.LogEvent(tuple, periodicKey, out fEventSuppressed, messageArgs);
		}

		private readonly ExEventLog eventLog;
	}
}
