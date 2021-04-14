using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class TimedEventsQueue
	{
		internal static int TimedEventsQueueSlot
		{
			get
			{
				return TimedEventsQueue.timedEventsQueueSlot;
			}
		}

		internal static TimeSpan TimedEventsQueueInterval
		{
			get
			{
				return TimedEventsQueue.Interval;
			}
		}

		internal static bool StopDispatch
		{
			get
			{
				return TimedEventsQueue.stopDispatch;
			}
			set
			{
				TimedEventsQueue.stopDispatch = value;
			}
		}

		internal StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		internal ITimedEventHandler TimedEventDispatcher
		{
			get
			{
				return this.timedEventDispatcher;
			}
		}

		internal static void Initialize()
		{
			if (TimedEventsQueue.timedEventsQueueSlot == -1)
			{
				TimedEventsQueue.timedEventsQueueSlot = StoreDatabase.AllocateComponentDataSlot();
				if (ExTraceGlobals.TimedEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TimedEventsTracer.TraceDebug(57672L, "TimedEventsQueue.timedEventsQueueSlot=" + TimedEventsQueue.timedEventsQueueSlot.ToString());
				}
			}
		}

		internal static void TimedEventsProcessing(Context context, TimedEventsQueue queue, Func<bool> shouldCallbackContinue)
		{
			using (context.AssociateWithDatabase(queue.Database))
			{
				if (queue.Database.IsOnlineActive)
				{
					if (TimedEventsQueue.StopDispatch)
					{
						ExTraceGlobals.TimedEventsTracer.TraceDebug(33096L, "Timed event dispatch is configured to not run");
					}
					else
					{
						TimedEventsQueue.TimedEventsProcessingInternal(context, queue, shouldCallbackContinue);
					}
				}
			}
		}

		internal static void TimedEventsProcessingInternal(Context context, TimedEventsQueue queue, Func<bool> shouldCallbackContinue)
		{
			List<TimedEventEntry> list = queue.ReadTimedEventEntries(context);
			if (list == null)
			{
				return;
			}
			foreach (TimedEventEntry timedEvent in list)
			{
				if (shouldCallbackContinue())
				{
					try
					{
						try
						{
							queue.TimedEventDispatcher.Invoke(context, timedEvent);
						}
						finally
						{
							queue.DeleteTimedEventEntry(context, timedEvent);
						}
						context.Commit();
						continue;
					}
					finally
					{
						context.Abort();
					}
				}
				ExTraceGlobals.TimedEventsTracer.TraceDebug(49480L, "Task is asked to stop");
				break;
			}
		}

		internal static void MountEventHandler(Context context, StoreDatabase database, bool readOnly)
		{
			TimedEventsQueue timedEventsQueue = new TimedEventsQueue(database);
			database.ComponentData[TimedEventsQueue.timedEventsQueueSlot] = timedEventsQueue;
			if (!readOnly)
			{
				RecurringTask<TimedEventsQueue> task = new RecurringTask<TimedEventsQueue>(TaskExecutionWrapper<TimedEventsQueue>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.TimedEventsProcessing, ClientType.System, database.MdbGuid), new TaskExecutionWrapper<TimedEventsQueue>.TaskCallback<Context>(TimedEventsQueue.TimedEventsProcessing)), timedEventsQueue, TimedEventsQueue.Interval, false);
				database.TaskList.Add(task, true);
			}
		}

		internal void InsertTimedEventEntry(Context context, TimedEventEntry timedEvent)
		{
			TimedEventsTable timedEventsTable = DatabaseSchema.TimedEventsTable(context.Database);
			using (InsertOperator insertOperator = Factory.CreateInsertOperator(context.Culture, context, timedEventsTable.Table, null, new Column[]
			{
				timedEventsTable.EventTime,
				timedEventsTable.MailboxNumber,
				timedEventsTable.EventSource,
				timedEventsTable.EventType,
				timedEventsTable.QoS,
				timedEventsTable.EventData
			}, new object[]
			{
				timedEvent.EventTime,
				timedEvent.MailboxNumber,
				timedEvent.EventSource,
				timedEvent.EventType,
				(int)timedEvent.QualityOfService,
				timedEvent.EventData
			}, null, true))
			{
				int num = (int)insertOperator.ExecuteScalar();
			}
		}

		internal List<TimedEventEntry> ReadTimedEventEntries(Context context)
		{
			return this.ReadTimedEventEntries(context, DateTime.UtcNow);
		}

		internal List<TimedEventEntry> ReadTimedEventEntries(Context context, DateTime stopAt)
		{
			TimedEventsTable timedEventsTable = DatabaseSchema.TimedEventsTable(this.database);
			List<TimedEventEntry> list = new List<TimedEventEntry>(100);
			StartStopKey empty = StartStopKey.Empty;
			StartStopKey stopKey = (stopAt == DateTime.MaxValue) ? StartStopKey.Empty : new StartStopKey(true, new object[]
			{
				stopAt
			});
			Column[] columnsToFetch = new Column[]
			{
				timedEventsTable.EventTime,
				timedEventsTable.UniqueId,
				timedEventsTable.MailboxNumber,
				timedEventsTable.EventSource,
				timedEventsTable.EventType,
				timedEventsTable.QoS,
				timedEventsTable.EventData
			};
			try
			{
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, timedEventsTable.Table, timedEventsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 100, new KeyRange(empty, stopKey), false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						while (reader.Read())
						{
							DateTime dateTime = reader.GetDateTime(timedEventsTable.EventTime);
							long @int = reader.GetInt64(timedEventsTable.UniqueId);
							int? nullableInt = reader.GetNullableInt32(timedEventsTable.MailboxNumber);
							Guid guid = reader.GetGuid(timedEventsTable.EventSource);
							int int2 = reader.GetInt32(timedEventsTable.EventType);
							int int3 = reader.GetInt32(timedEventsTable.QoS);
							byte[] binary = reader.GetBinary(timedEventsTable.EventData);
							list.Add(new TimedEventEntry(dateTime, @int, nullableInt, guid, int2, (TimedEventEntry.QualityOfServiceType)int3, binary));
						}
					}
				}
				context.Commit();
			}
			finally
			{
				context.Abort();
			}
			return list;
		}

		internal void DeleteTimedEventEntry(Context context, TimedEventEntry timedEvent)
		{
			TimedEventsTable timedEventsTable = DatabaseSchema.TimedEventsTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				timedEvent.EventTime,
				timedEvent.UniqueId
			});
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, timedEventsTable.Table, timedEventsTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false), false))
			{
				deleteOperator.ExecuteScalar();
			}
		}

		private TimedEventsQueue(StoreDatabase database)
		{
			this.database = database;
			this.timedEventDispatcher = new TimedEventDispatcher();
		}

		private const int BatchReadCount = 100;

		private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30.0);

		private static int timedEventsQueueSlot = -1;

		private static bool stopDispatch = false;

		private StoreDatabase database;

		private ITimedEventHandler timedEventDispatcher;
	}
}
