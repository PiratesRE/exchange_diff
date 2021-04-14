using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class ExEventLog
	{
		public ExEventLog(Guid componentGuid, string sourceName)
		{
			this.exEventLog = new ExEventLog(componentGuid, sourceName);
		}

		public ExEventLog(Guid componentGuid, string sourceName, string logName)
		{
			this.exEventLog = new ExEventLog(componentGuid, sourceName, logName);
		}

		public bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			return this.exEventLog.LogEvent(tuple.EventTupleImpl, periodicKey, messageArgs);
		}

		private ExEventLog exEventLog;

		public enum EventLevel
		{
			Lowest,
			Low,
			Medium = 3,
			High = 5,
			Expert = 7
		}

		public enum EventPeriod
		{
			LogAlways,
			LogOneTime,
			LogPeriodic
		}

		public struct EventTuple
		{
			public EventTuple(ExEventLog.EventTuple exEventTuple)
			{
				this.exEventTuple = exEventTuple;
			}

			public uint EventId
			{
				get
				{
					return this.exEventTuple.EventId;
				}
			}

			public short CategoryId
			{
				get
				{
					return this.exEventTuple.CategoryId;
				}
			}

			public ExEventLog.EventLevel Level
			{
				get
				{
					return (ExEventLog.EventLevel)this.exEventTuple.Level;
				}
			}

			public ExEventLog.EventPeriod Period
			{
				get
				{
					return (ExEventLog.EventPeriod)this.exEventTuple.Period;
				}
			}

			public EventLogEntryType EntryType
			{
				get
				{
					return this.exEventTuple.EntryType;
				}
			}

			internal ExEventLog.EventTuple EventTupleImpl
			{
				get
				{
					return this.exEventTuple;
				}
			}

			private readonly ExEventLog.EventTuple exEventTuple;
		}
	}
}
