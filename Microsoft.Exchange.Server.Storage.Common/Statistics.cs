using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class Statistics
	{
		internal static RecurringTask<object> DumpStatisticsTask
		{
			get
			{
				return Statistics.dumpStatisticsTask;
			}
			set
			{
				Statistics.dumpStatisticsTask = value;
			}
		}

		internal static List<Statistics.StatisticsGroup> Groups
		{
			get
			{
				return Statistics.groups;
			}
			set
			{
				Statistics.groups = value;
			}
		}

		[Conditional("STATISTICS")]
		public static void Initialize()
		{
			if (Statistics.dumpStatisticsTask == null)
			{
				Type typeFromHandle = typeof(Statistics);
				foreach (TypeInfo typeInfo in typeFromHandle.GetTypeInfo().DeclaredNestedTypes)
				{
					if (typeInfo.IsClass)
					{
						List<Statistics.StatisticsEntry> list = null;
						foreach (FieldInfo fieldInfo in typeInfo.DeclaredFields)
						{
							if (fieldInfo.IsStatic && fieldInfo.FieldType.GetTypeInfo().IsSubclassOf(typeof(Statistics.StatisticElement)))
							{
								Statistics.CounterNameAttribute customAttribute = fieldInfo.GetCustomAttribute(false);
								string name;
								if (customAttribute != null)
								{
									name = customAttribute.Name;
								}
								else
								{
									name = fieldInfo.Name;
								}
								object value = fieldInfo.GetValue(null);
								if (list == null)
								{
									list = new List<Statistics.StatisticsEntry>();
								}
								((Statistics.StatisticElement)value).Initialize();
								list.Add(new Statistics.StatisticsEntry(name, (Statistics.StatisticElement)value));
							}
						}
						if (list != null)
						{
							Statistics.GroupNameAttribute customAttribute2 = typeInfo.GetCustomAttribute(false);
							string name2;
							if (customAttribute2 != null)
							{
								name2 = customAttribute2.Name;
							}
							else
							{
								name2 = typeInfo.Name;
							}
							if (Statistics.groups == null)
							{
								Statistics.groups = new List<Statistics.StatisticsGroup>();
							}
							Statistics.groups.Add(new Statistics.StatisticsGroup(name2, list));
						}
					}
				}
				foreach (Statistics.StatisticsGroup statisticsGroup in Statistics.groups)
				{
					foreach (Statistics.StatisticsEntry statisticsEntry in statisticsGroup.Entries)
					{
						statisticsEntry.Element.Reset();
					}
				}
				Statistics.DumpStatisticsTask = new RecurringTask<object>(new Task<object>.TaskCallback(Statistics.DumpStatisticsTaskCallback), null, Statistics.tickInterval, true);
				Statistics.lastTimeDumped = Environment.TickCount;
			}
		}

		[Conditional("STATISTICS")]
		public static void Terminate()
		{
			if (Statistics.dumpStatisticsTask != null)
			{
				Statistics.dumpStatisticsTask.Dispose();
				Statistics.dumpStatisticsTask = null;
			}
			if (Statistics.groups != null)
			{
				Statistics.groups.Clear();
			}
		}

		private static void DumpStatisticsTaskCallback(TaskExecutionDiagnosticsProxy diagnosticsContext, object context, Func<bool> shouldCallbackContinue)
		{
			if (shouldCallbackContinue())
			{
				if (ExTraceGlobals.StatisticsTracer.IsTraceEnabled(TraceType.PerformanceTrace))
				{
					int num = Statistics.lastTimeDumped;
					int tickCount = Environment.TickCount;
					if (tickCount - num > Statistics.dumpStatisticsInterval && num == Interlocked.CompareExchange(ref Statistics.lastTimeDumped, tickCount, num) && Statistics.groups != null)
					{
						foreach (Statistics.StatisticsGroup statisticsGroup in Statistics.groups)
						{
							StringBuilder stringBuilder = new StringBuilder(statisticsGroup.Entries.Count * 20);
							stringBuilder.Append(statisticsGroup.GroupName);
							stringBuilder.Append(" statistics:[");
							foreach (Statistics.StatisticsEntry statisticsEntry in statisticsGroup.Entries)
							{
								stringBuilder.Append(" ");
								stringBuilder.Append(statisticsEntry.EntryName);
								stringBuilder.Append(":[");
								stringBuilder.Append(statisticsEntry.Element.ToString());
								stringBuilder.Append("]");
							}
							stringBuilder.Append("]");
							ExTraceGlobals.StatisticsTracer.TracePerformance(0L, stringBuilder.ToString());
						}
					}
				}
				if (ExTraceGlobals.ResetStatisticsTracer.IsTraceEnabled(TraceType.PerformanceTrace))
				{
					if (!Statistics.lastTimeResetTag && Statistics.groups != null)
					{
						foreach (Statistics.StatisticsGroup statisticsGroup2 in Statistics.groups)
						{
							foreach (Statistics.StatisticsEntry statisticsEntry2 in statisticsGroup2.Entries)
							{
								statisticsEntry2.Element.Reset();
							}
						}
					}
					Statistics.lastTimeResetTag = true;
					return;
				}
				Statistics.lastTimeResetTag = false;
			}
		}

		private static int dumpStatisticsInterval = 300000;

		private static TimeSpan tickInterval = TimeSpan.FromMinutes(1.0);

		private static RecurringTask<object> dumpStatisticsTask;

		private static int lastTimeDumped;

		private static bool lastTimeResetTag = false;

		private static List<Statistics.StatisticsGroup> groups = null;

		[Statistics.GroupNameAttribute("Logon Notifications")]
		public static class LogonNotifications
		{
			public static Statistics.Counter32 Total = new Statistics.Counter32();

			public static Statistics.Counter32 Redundant = new Statistics.Counter32();

			public static Statistics.Counter32 DropOld = new Statistics.Counter32();

			public static Statistics.Counter32 DropNew = new Statistics.Counter32();

			public static Statistics.Counter32 Merge = new Statistics.Counter32();

			public static Statistics.Counter32 ReplaceOld = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("OverflowTC")]
			public static Statistics.Counter32 OverflowFlushWithTableChanged = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("OverflowDrop")]
			public static Statistics.Counter32 OverflowDrop = new Statistics.Counter32();
		}

		[Statistics.GroupNameAttribute("Context Notifications")]
		public static class ContextNotifications
		{
			public static Statistics.Counter32 Total = new Statistics.Counter32();

			public static Statistics.Counter32Max Max = new Statistics.Counter32Max();
		}

		[Statistics.GroupNameAttribute("Miscelaneous Notifications")]
		public static class MiscelaneousNotifications
		{
			[Statistics.CounterNameAttribute("SkipFld")]
			public static Statistics.Counter32 SkippedFolderTableNotifications = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("SkipMsg")]
			public static Statistics.Counter32 SkippedMessageTableNotifications = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewTC")]
			public static Statistics.Counter32 NewTableChangedWashesAnyOld = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("OldTC")]
			public static Statistics.Counter32 OldTableChangedWashesAnyNew = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewRM+OldRA")]
			public static Statistics.Counter32 NewRowModifiedWashesOldRowAdded = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewRM+OldRM")]
			public static Statistics.Counter32 NewRowModifiedWashesOldRowModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewRD+OldRA")]
			public static Statistics.Counter32 NewRowDeletedWashesOldRowAdded = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewRD+OldRM")]
			public static Statistics.Counter32 NewRowDeletedWashesOldRowModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("RestartTRN")]
			public static Statistics.Counter32 NotificationHandlingRestartedTransaction = new Statistics.Counter32();
		}

		[Statistics.GroupNameAttribute("Notification Types")]
		public static class NotificationTypes
		{
			[Statistics.CounterNameAttribute("MsgCreated")]
			public static Statistics.Counter32 MessageCreated = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgModified")]
			public static Statistics.Counter32 MessageModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgDeleted")]
			public static Statistics.Counter32 MessageDeleted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgMoved")]
			public static Statistics.Counter32 MessageMoved = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgCopied")]
			public static Statistics.Counter32 MessageCopied = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgUnlinked")]
			public static Statistics.Counter32 MessageUnlinked = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgSubmitted")]
			public static Statistics.Counter32 MailSubmitted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("NewMail")]
			public static Statistics.Counter32 NewMail = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("FldCreated")]
			public static Statistics.Counter32 FolderCreated = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("FldModified")]
			public static Statistics.Counter32 FolderModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("FldDeleted")]
			public static Statistics.Counter32 FolderDeleted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("FldMoved")]
			public static Statistics.Counter32 FolderMoved = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("FldCopied")]
			public static Statistics.Counter32 FolderCopied = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("TableModified")]
			public static Statistics.Counter32 TableModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("CatRowAdded")]
			public static Statistics.Counter32 CategorizedRowAdded = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("CatRowModified")]
			public static Statistics.Counter32 CategorizedRowModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("CatRowDeleted")]
			public static Statistics.Counter32 CategorizedRowDeleted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MsgsLinked")]
			public static Statistics.Counter32 MessagesLinked = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("SearchComplete")]
			public static Statistics.Counter32 SearchComplete = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("BeginLongOp")]
			public static Statistics.Counter32 BeginLongOperation = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("EndLongOp")]
			public static Statistics.Counter32 EndLongOperation = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("nonStandardObjectModification")]
			public static Statistics.Counter32 NonStandardObjectModification = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("Ics")]
			public static Statistics.Counter32 Ics = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxCreated")]
			public static Statistics.Counter32 MailboxCreated = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxModified")]
			public static Statistics.Counter32 MailboxModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxDeleted")]
			public static Statistics.Counter32 MailboxDeleted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxDusconnected")]
			public static Statistics.Counter32 MailboxDisconnected = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxReconnected")]
			public static Statistics.Counter32 MailboxReconnected = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxMoveStarted")]
			public static Statistics.Counter32 MailboxMoveStarted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxMoveSucceeded")]
			public static Statistics.Counter32 MailboxMoveSucceeded = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("MbxMoveFailed")]
			public static Statistics.Counter32 MailboxMoveFailed = new Statistics.Counter32();
		}

		[Statistics.GroupNameAttribute("Table Notification Sub-Types")]
		public static class TableNotificationTypes
		{
			[Statistics.CounterNameAttribute("Changed")]
			public static Statistics.Counter32 Changed = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("Error")]
			public static Statistics.Counter32 Error = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("RowAdded")]
			public static Statistics.Counter32 RowAdded = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("RowModified")]
			public static Statistics.Counter32 RowModified = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("RowDeleted")]
			public static Statistics.Counter32 RowDeleted = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("SortDone")]
			public static Statistics.Counter32 SortDone = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("RestrictDone")]
			public static Statistics.Counter32 RestrictDone = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("SetcolDone")]
			public static Statistics.Counter32 SetcolDone = new Statistics.Counter32();

			[Statistics.CounterNameAttribute("Reload")]
			public static Statistics.Counter32 Reload = new Statistics.Counter32();
		}

		[Statistics.GroupNameAttribute("Unsorted")]
		public static class Unsorted
		{
			public static Statistics.Counter32Max MaxPIColumnIndex = new Statistics.Counter32Max();
		}

		[Statistics.GroupNameAttribute("StatementLength")]
		public static class StatementLength
		{
			public static Statistics.AveragesGroup Averages = new Statistics.AveragesGroup();
		}

		public class GroupNameAttribute : Attribute
		{
			public GroupNameAttribute(string name)
			{
				this.name = name;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			private string name;
		}

		public class CounterNameAttribute : Attribute
		{
			public CounterNameAttribute(string name)
			{
				this.name = name;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			private string name;
		}

		public abstract class StatisticElement
		{
			public virtual void Initialize()
			{
			}

			public virtual void Reset()
			{
			}
		}

		public abstract class Counter : Statistics.StatisticElement
		{
			public abstract long CurrentValue { get; }

			public override string ToString()
			{
				return this.CurrentValue.ToString();
			}
		}

		public class Counter32 : Statistics.Counter
		{
			[Conditional("STATISTICS")]
			public void Bump()
			{
				Interlocked.Increment(ref this.value);
			}

			public override long CurrentValue
			{
				get
				{
					return (long)this.value;
				}
			}

			public override void Reset()
			{
				this.value = 0;
			}

			private int value;
		}

		public class Counter32Max : Statistics.Counter
		{
			[Conditional("STATISTICS")]
			public void Bump(int newValue)
			{
				int num = this.value;
				while (newValue > num)
				{
					int num2 = Interlocked.CompareExchange(ref this.value, newValue, num);
					if (num2 == num)
					{
						return;
					}
					num = num2;
				}
			}

			public override long CurrentValue
			{
				get
				{
					return (long)this.value;
				}
			}

			public override void Reset()
			{
				this.value = 0;
			}

			private int value;
		}

		public class AveragesGroup : Statistics.StatisticElement
		{
			public override void Initialize()
			{
				this.averages = new Dictionary<string, List<int>>(50);
			}

			public override void Reset()
			{
				if (this.averages != null)
				{
					this.averages.Clear();
				}
			}

			[Conditional("STATISTICS")]
			public void AddSample(string name, int sample)
			{
				if (this.averages != null && ExTraceGlobals.StatisticsTracer.IsTraceEnabled(TraceType.PerformanceTrace))
				{
					using (LockManager.Lock(this.averages))
					{
						List<int> list;
						if (!this.averages.TryGetValue(name, out list))
						{
							list = new List<int>(200);
							this.averages.Add(name, list);
						}
						list.Add(sample);
					}
				}
			}

			public override string ToString()
			{
				if (this.averages != null)
				{
					StringBuilder stringBuilder = new StringBuilder(this.averages.Count * 40);
					using (LockManager.Lock(this.averages))
					{
						foreach (KeyValuePair<string, List<int>> keyValuePair in this.averages)
						{
							keyValuePair.Value.Sort();
							stringBuilder.Append(" name:[");
							stringBuilder.Append(keyValuePair.Key);
							stringBuilder.Append("] cnt:[");
							stringBuilder.Append(keyValuePair.Value.Count);
							stringBuilder.Append("] avg:[");
							long num = 0L;
							foreach (int num2 in keyValuePair.Value)
							{
								num += (long)num2;
							}
							stringBuilder.Append(num / (long)keyValuePair.Value.Count);
							stringBuilder.Append("]");
							if (keyValuePair.Value.Count <= 10)
							{
								stringBuilder.Append(" all:[");
								stringBuilder.AppendAsString(keyValuePair.Value);
								stringBuilder.Append("]");
							}
							else
							{
								for (int i = 70; i < 100; i += 5)
								{
									stringBuilder.Append(" ");
									stringBuilder.Append(i);
									stringBuilder.Append("%:[");
									stringBuilder.AppendAsString(keyValuePair.Value[(int)((double)keyValuePair.Value.Count / 100.0 * (double)i) - 1]);
									stringBuilder.Append("]");
								}
								stringBuilder.Append(" 100%:[");
								stringBuilder.Append(keyValuePair.Value[keyValuePair.Value.Count - 1]);
								stringBuilder.Append("]");
							}
						}
					}
					return stringBuilder.ToString();
				}
				return "empty";
			}

			private Dictionary<string, List<int>> averages;
		}

		internal struct StatisticsEntry
		{
			public StatisticsEntry(string name, Statistics.StatisticElement element)
			{
				this.EntryName = name;
				this.Element = element;
			}

			public string EntryName;

			public Statistics.StatisticElement Element;
		}

		internal struct StatisticsGroup
		{
			public StatisticsGroup(string name, List<Statistics.StatisticsEntry> entries)
			{
				this.GroupName = name;
				this.Entries = entries;
			}

			public string GroupName;

			public List<Statistics.StatisticsEntry> Entries;
		}
	}
}
