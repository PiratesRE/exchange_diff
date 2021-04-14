using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	public sealed class ExEventLog
	{
		public ExEventLog(Guid componentGuid, string sourceName) : this(componentGuid, sourceName, null)
		{
		}

		public ExEventLog(Guid componentGuid, string sourceName, string logName)
		{
			this.impl = ExEventLog.hookableEventLogFactory.Value(componentGuid, sourceName, logName);
		}

		public ExEventSourceInfo EventSource
		{
			get
			{
				return this.impl.EventSource;
			}
		}

		internal ExEventLog.IImpl TestHook
		{
			get
			{
				if (this.impl is ExEventLog.Impl)
				{
					return null;
				}
				return this.impl;
			}
		}

		public bool IsEventCategoryEnabled(short categoryNumber, ExEventLog.EventLevel level)
		{
			return this.impl.IsEventCategoryEnabled(categoryNumber, level);
		}

		public void SetEventPeriod(int seconds)
		{
			this.EventSource.EventPeriodTime = seconds;
		}

		public bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			bool flag;
			return this.LogEvent(tuple, periodicKey, out flag, messageArgs);
		}

		public bool LogEventWithExtraData(ExEventLog.EventTuple tuple, string periodicKey, byte[] extraData, params object[] messageArgs)
		{
			bool flag;
			return this.impl.LogEvent(string.Empty, tuple.EventId, tuple.CategoryId, tuple.Level, tuple.EntryType, tuple.Period, periodicKey, out flag, extraData, messageArgs);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0)
		{
			return this.LogEvent(organizationId, tuple, periodicKey, new object[]
			{
				arg0
			});
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1)
		{
			return this.LogEvent(organizationId, tuple, periodicKey, new object[]
			{
				arg0,
				arg1
			});
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2)
		{
			return this.LogEvent(organizationId, tuple, periodicKey, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, object arg0, object arg1, object arg2, object arg3)
		{
			return this.LogEvent(organizationId, tuple, periodicKey, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			});
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			if (organizationId != null && !string.IsNullOrEmpty(organizationId.IdForEventLog) && tuple.Period == ExEventLog.EventPeriod.LogOneTime)
			{
				throw new ArgumentException("Per-tenant one-time events are not supported.", "tuple");
			}
			bool flag;
			return this.impl.LogEvent((organizationId != null) ? organizationId.IdForEventLog : string.Empty, tuple.EventId, tuple.CategoryId, tuple.Level, tuple.EntryType, tuple.Period, periodicKey, out flag, null, messageArgs);
		}

		public bool LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			if (organizationId != null && !string.IsNullOrEmpty(organizationId.IdForEventLog) && tuple.Period == ExEventLog.EventPeriod.LogOneTime)
			{
				throw new ArgumentException("Per-tenant one-time events are not supported.", "tuple");
			}
			return this.impl.LogEvent((organizationId != null) ? organizationId.IdForEventLog : string.Empty, tuple.EventId, tuple.CategoryId, tuple.Level, tuple.EntryType, tuple.Period, periodicKey, out fEventSuppressed, null, messageArgs);
		}

		public bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			return this.impl.LogEvent(string.Empty, tuple.EventId, tuple.CategoryId, tuple.Level, tuple.EntryType, tuple.Period, periodicKey, out fEventSuppressed, null, messageArgs);
		}

		internal static IDisposable SetFactoryTestHook(Func<Guid, string, string, ExEventLog.IImpl> eventLogFactory)
		{
			return ExEventLog.hookableEventLogFactory.SetTestHook(eventLogFactory);
		}

		private static readonly Hookable<Func<Guid, string, string, ExEventLog.IImpl>> hookableEventLogFactory = Hookable<Func<Guid, string, string, ExEventLog.IImpl>>.Create(true, (Guid componentGuid, string sourceName, string logName) => new ExEventLog.Impl(componentGuid, sourceName, logName));

		private ExEventLog.IImpl impl;

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

		[CLSCompliant(false)]
		public interface IImpl
		{
			ExEventSourceInfo EventSource { get; }

			bool IsEventCategoryEnabled(short categoryNumber, ExEventLog.EventLevel level);

			bool LogEvent(string organizationId, uint eventId, short category, ExEventLog.EventLevel level, EventLogEntryType type, ExEventLog.EventPeriod period, string periodicKey, out bool fEventSuppressed, byte[] extraData, params object[] messageArgs);
		}

		public struct EventTuple
		{
			[CLSCompliant(false)]
			public EventTuple(uint eventId, short categoryId, EventLogEntryType entryType, ExEventLog.EventLevel level, ExEventLog.EventPeriod period)
			{
				this.eventId = eventId;
				this.categoryId = categoryId;
				this.entryType = entryType;
				this.level = level;
				this.period = period;
			}

			[CLSCompliant(false)]
			public uint EventId
			{
				get
				{
					return this.eventId;
				}
			}

			[CLSCompliant(false)]
			public short CategoryId
			{
				get
				{
					return this.categoryId;
				}
			}

			[CLSCompliant(false)]
			public ExEventLog.EventLevel Level
			{
				get
				{
					return this.level;
				}
			}

			[CLSCompliant(false)]
			public ExEventLog.EventPeriod Period
			{
				get
				{
					return this.period;
				}
			}

			[CLSCompliant(false)]
			public EventLogEntryType EntryType
			{
				get
				{
					return this.entryType;
				}
			}

			private readonly uint eventId;

			private readonly short categoryId;

			private readonly ExEventLog.EventLevel level;

			private readonly ExEventLog.EventPeriod period;

			private readonly EventLogEntryType entryType;
		}

		private struct PeriodicCheckKey
		{
			public PeriodicCheckKey(uint eid, string sn)
			{
				this.EventId = eid;
				this.SourceName = sn;
			}

			public uint EventId;

			public string SourceName;
		}

		internal sealed class PeriodicEventsHistory<T>
		{
			internal PeriodicEventsHistory(int length)
			{
				if (length < 0)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				this.length = length;
			}

			internal bool Log(T evt, long eventTime)
			{
				this.EnsureHistoryInitialized();
				if (this.InHistory(evt, eventTime))
				{
					return false;
				}
				this.RecordEvent(evt, eventTime);
				return true;
			}

			private bool InHistory(T evt, long eventTime)
			{
				long num = this.mostRecentEventTime - (long)this.length + 1L;
				long num2 = eventTime - (long)this.length + 1L;
				if (this.mostRecentEventTime < num2)
				{
					return false;
				}
				if (eventTime < num)
				{
					return false;
				}
				long time = Math.Max(num, num2);
				long time2 = Math.Min(this.mostRecentEventTime, eventTime);
				long lowerOffset = this.MapTimeToHistory(time);
				long upperOffset = this.MapTimeToHistory(time2);
				return this.history.PeekRange(lowerOffset, upperOffset).Any((HashSet<T> h) => h.Contains(evt));
			}

			private long MapTimeToHistory(long time)
			{
				return time - this.mostRecentEventTime;
			}

			private void RecordEvent(T evt, long eventTime)
			{
				if (eventTime <= this.mostRecentEventTime)
				{
					this.history.Peek().Add(evt);
					return;
				}
				for (long num = Math.Min(eventTime - this.mostRecentEventTime, (long)this.length); num > 0L; num -= 1L)
				{
					this.history.Advance().Clear();
				}
				this.mostRecentEventTime = eventTime;
				this.history.Peek().Add(evt);
			}

			private void EnsureHistoryInitialized()
			{
				if (this.history == null)
				{
					this.history = new ExEventLog.PeriodicEventsHistory<T>.SlidingWindow<HashSet<T>>(this.length);
				}
			}

			private readonly int length;

			private ExEventLog.PeriodicEventsHistory<T>.SlidingWindow<HashSet<T>> history;

			private long mostRecentEventTime;

			private sealed class SlidingWindow<U> where U : new()
			{
				public SlidingWindow(int length)
				{
					if (length < 0)
					{
						throw new ArgumentOutOfRangeException();
					}
					this.readingPositon = 0;
					this.length = length;
					this.buffer = new U[length];
					for (int i = 0; i < length; i++)
					{
						this.buffer[i] = ((default(U) == null) ? Activator.CreateInstance<U>() : default(U));
					}
				}

				internal U Peek()
				{
					return this.Peek(0L);
				}

				internal IEnumerable<U> PeekRange(long lowerOffset, long upperOffset)
				{
					for (long offset = lowerOffset; offset <= upperOffset; offset += 1L)
					{
						yield return this.Peek(offset);
					}
					yield break;
				}

				internal U Advance()
				{
					this.readingPositon = (this.readingPositon + 1) % this.length;
					return this.buffer[this.readingPositon];
				}

				private U Peek(long offset)
				{
					long num = ((long)this.readingPositon + offset) % (long)this.length;
					return this.buffer[(int)(checked((IntPtr)((num < 0L) ? unchecked(num + (long)this.length) : num)))];
				}

				private readonly int length;

				private U[] buffer;

				private int readingPositon;
			}
		}

		private class Impl : ExEventLog.IImpl
		{
			public Impl(Guid componentGuid, string sourceName, string logName)
			{
				if (string.IsNullOrEmpty(sourceName))
				{
					throw new ArgumentException("sourceName must be non-NULL and non-zero-length", "sourceName");
				}
				if (!EventLog.SourceExists(sourceName))
				{
					ExTraceGlobals.EventLogTracer.TraceInformation(22683, 0L, "Creating Event Source");
					try
					{
						EventLog.CreateEventSource(sourceName, logName);
					}
					catch (ArgumentException)
					{
					}
				}
				this.eventLog = new EventLog();
				this.eventLog.Source = sourceName;
				ExTraceGlobals.EventLogTracer.TraceInformation(31849, 0L, "RegisterEventSource succeeded");
				this.periodicKeys = new Dictionary<ExEventLog.PeriodicCheckKey, DateTime>(new ExEventLog.PeriodicKeysComparer());
				this.perTenantPeriodicEventsHistory = new ExEventLog.PeriodicEventsHistory<int>(15);
				this.eventSource = new ExEventSourceInfo(sourceName);
			}

			public ExEventSourceInfo EventSource
			{
				get
				{
					return this.eventSource;
				}
			}

			public bool IsEventCategoryEnabled(short categoryNumber, ExEventLog.EventLevel level)
			{
				switch (level)
				{
				case ExEventLog.EventLevel.Lowest:
					return true;
				case ExEventLog.EventLevel.Low:
				case ExEventLog.EventLevel.Medium:
				case ExEventLog.EventLevel.High:
				case ExEventLog.EventLevel.Expert:
					goto IL_2F;
				}
				level = ExEventLog.EventLevel.Expert;
				IL_2F:
				ExEventLog.EventLevel eventLevel = ExEventLog.EventLevel.Lowest;
				ExEventCategory category = this.EventSource.GetCategory((int)categoryNumber);
				if (category != null)
				{
					eventLevel = category.EventLevel;
				}
				return level <= eventLevel;
			}

			public bool LogEvent(string organizationId, uint eventId, short category, ExEventLog.EventLevel level, EventLogEntryType type, ExEventLog.EventPeriod period, string periodicKey, out bool fEventSuppressed, byte[] extraData, params object[] messageArgs)
			{
				fEventSuppressed = false;
				if (messageArgs != null && messageArgs.Length > 32767)
				{
					throw new ArgumentException("There were too many strings passed in as messageArgs", "messageArgs");
				}
				if (!this.IsEventCategoryEnabled(category, level))
				{
					return true;
				}
				if (!this.CanLogPeriodic(period, eventId, periodicKey, organizationId))
				{
					fEventSuppressed = true;
					return true;
				}
				EventInstance instance = new EventInstance((long)((ulong)eventId), (int)category, type);
				Exception ex = null;
				try
				{
					byte[] array;
					if (!string.IsNullOrEmpty(organizationId))
					{
						int num = string.IsNullOrEmpty(periodicKey) ? 0 : periodicKey.GetHashCode();
						array = Encoding.UTF8.GetBytes(string.Format("<ExchangeEventInfo><OrganizationId>{0}</OrganizationId><PeriodicKey>{1}</PeriodicKey></ExchangeEventInfo>", organizationId, num.ToString("X", CultureInfo.InvariantCulture)));
						if (extraData != null)
						{
							array = array.Concat(extraData).ToArray<byte>();
						}
					}
					else
					{
						array = extraData;
					}
					if (messageArgs != null)
					{
						for (int i = 0; i < messageArgs.Length; i++)
						{
							string text = (messageArgs[i] != null) ? messageArgs[i].ToString() : string.Empty;
							if (!string.IsNullOrEmpty(text) && text.Length > 31000)
							{
								messageArgs[i] = text.Substring(0, 31000) + "...";
							}
						}
					}
					this.eventLog.WriteEvent(instance, array, messageArgs);
				}
				catch (Win32Exception ex2)
				{
					ex = ex2;
				}
				catch (InvalidOperationException ex3)
				{
					ex = ex3;
				}
				catch (AccessViolationException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.EventLogTracer.TraceInformation<Exception>(17513, 0L, "WriteEvent returned {0}", ex);
					return false;
				}
				return true;
			}

			private bool CanLogPeriodic(ExEventLog.EventPeriod period, uint eventId, string eventKey, string organizationId)
			{
				if (period == ExEventLog.EventPeriod.LogAlways)
				{
					return true;
				}
				if (!string.IsNullOrEmpty(organizationId))
				{
					return this.CanLogPeriodicTenantEvent(eventId, eventKey, organizationId);
				}
				ExEventLog.PeriodicCheckKey key = new ExEventLog.PeriodicCheckKey(eventId, eventKey);
				DateTime dateTime;
				bool flag2;
				lock (this.periodicKeys)
				{
					flag2 = this.periodicKeys.TryGetValue(key, out dateTime);
				}
				if (!flag2)
				{
					lock (this.periodicKeys)
					{
						this.periodicKeys[key] = DateTime.UtcNow;
					}
					return true;
				}
				if (ExEventLog.EventPeriod.LogOneTime == period)
				{
					return false;
				}
				int eventPeriodTime = this.EventSource.EventPeriodTime;
				if (DateTime.UtcNow >= dateTime.AddSeconds((double)eventPeriodTime))
				{
					lock (this.periodicKeys)
					{
						this.periodicKeys.Remove(key);
						this.periodicKeys[key] = DateTime.UtcNow;
					}
					return true;
				}
				return false;
			}

			private bool CanLogPeriodicTenantEvent(uint eventId, string periodicKey, string organizationId)
			{
				int evt = eventId.GetHashCode() ^ (string.IsNullOrEmpty(periodicKey) ? 0 : periodicKey.GetHashCode()) ^ (string.IsNullOrEmpty(organizationId) ? 0 : organizationId.ToLowerInvariant().GetHashCode());
				long eventTime = DateTime.UtcNow.Ticks / 600000000L;
				bool result;
				lock (this.perTenantPeriodicEventsHistory)
				{
					result = this.perTenantPeriodicEventsHistory.Log(evt, eventTime);
				}
				return result;
			}

			private const int PerTenantEventPeriodInMinutes = 15;

			private const int MaxLogEntryStringLength = 31000;

			private readonly EventLog eventLog;

			private Dictionary<ExEventLog.PeriodicCheckKey, DateTime> periodicKeys;

			private ExEventLog.PeriodicEventsHistory<int> perTenantPeriodicEventsHistory;

			private ExEventSourceInfo eventSource;
		}

		private class PeriodicKeysComparer : IEqualityComparer<ExEventLog.PeriodicCheckKey>
		{
			bool IEqualityComparer<ExEventLog.PeriodicCheckKey>.Equals(ExEventLog.PeriodicCheckKey a, ExEventLog.PeriodicCheckKey b)
			{
				return a.EventId == b.EventId && 0 == string.Compare(a.SourceName, b.SourceName, StringComparison.Ordinal);
			}

			int IEqualityComparer<ExEventLog.PeriodicCheckKey>.GetHashCode(ExEventLog.PeriodicCheckKey key)
			{
				return (int)(key.EventId ^ (uint)((key.SourceName != null) ? key.SourceName.GetHashCode() : 0));
			}
		}
	}
}
