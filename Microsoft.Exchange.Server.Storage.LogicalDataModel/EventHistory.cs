using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class EventHistory : IStateObject
	{
		private EventHistory(Context context)
		{
			this.database = context.Database;
			this.mdbVersionGuid = Guid.NewGuid();
			this.watermarkTableLockName = EventHistory.WatermarkTableLockName(context.Database.MdbGuid);
			this.eventsTable = DatabaseSchema.EventsTable(context.Database);
			this.watermarksTable = DatabaseSchema.WatermarksTable(context.Database);
			this.eventsFetchList = new Column[]
			{
				this.eventsTable.EventCounter,
				this.eventsTable.CreateTime,
				this.eventsTable.TransactionId,
				this.eventsTable.EventType,
				this.eventsTable.MailboxNumber,
				this.eventsTable.ClientType,
				this.eventsTable.Flags,
				this.eventsTable.ObjectClass,
				this.eventsTable.Fid,
				this.eventsTable.Mid,
				this.eventsTable.ParentFid,
				this.eventsTable.OldFid,
				this.eventsTable.OldMid,
				this.eventsTable.OldParentFid,
				this.eventsTable.ItemCount,
				this.eventsTable.UnreadCount,
				this.eventsTable.ExtendedFlags,
				this.eventsTable.Sid,
				this.eventsTable.DocumentId
			};
			this.lastEventFetchList = new Column[]
			{
				this.eventsTable.EventCounter,
				this.eventsTable.CreateTime
			};
			this.watermarksFetchList = new Column[]
			{
				this.watermarksTable.ConsumerGuid,
				this.watermarksTable.MailboxNumber,
				this.watermarksTable.EventCounter
			};
			this.lastEventCounter = this.ComputeEventCounterBound(context, true);
			this.highestCommittedEventCounter = this.lastEventCounter;
			this.eventCounterUpperBound = this.lastEventCounter + 1L;
			this.eventCounterLowerBound = this.ComputeEventCounterBound(context, false);
			this.eventCounterAllocationPotentiallyLost = true;
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public Guid MdbVersionGuid
		{
			get
			{
				return this.mdbVersionGuid;
			}
		}

		internal long LastEventCounter
		{
			get
			{
				return this.lastEventCounter;
			}
		}

		internal long EventCounterUpperBound
		{
			get
			{
				return this.eventCounterUpperBound;
			}
		}

		internal bool EventCounterAllocationPotentiallyLost
		{
			get
			{
				return this.eventCounterAllocationPotentiallyLost;
			}
		}

		internal bool IsEventCounterUpperBoundFlushNeeded
		{
			get
			{
				return this.flushEventCounterUpperBoundTaskManager.IsFlushNeeded;
			}
		}

		public static void Initialize()
		{
			if (EventHistory.eventHistoryDataSlot == -1)
			{
				EventHistory.eventHistoryDataSlot = StoreDatabase.AllocateComponentDataSlot();
				EventHistory.eventHistoryCleanupMaintenance = MaintenanceHandler.RegisterDatabaseMaintenance(EventHistory.EventHistoryCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.DatabaseMaintenanceDelegate(EventHistory.EventHistoryCleanupMaintenance), "EventHistory.EventHistoryCleanupMaintenance");
			}
		}

		internal static void MountEventHandler(Context context, bool readOnly)
		{
			EventHistory eventHistory = new EventHistory(context);
			if (AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database) && context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				try
				{
					context.Database.GetSharedLock();
					eventHistory.SyncEventCounterBounds(context, readOnly);
				}
				finally
				{
					context.Database.ReleaseSharedLock();
				}
			}
			context.Database.ComponentData[EventHistory.eventHistoryDataSlot] = eventHistory;
		}

		internal static void MountedEventHandler(Context context)
		{
			EventHistory.eventHistoryCleanupMaintenance.ScheduleMarkForMaintenance(context, TimeSpan.FromDays(1.0));
		}

		internal static void DismountEventHandler(StoreDatabase database)
		{
			database.ComponentData[EventHistory.eventHistoryDataSlot] = null;
		}

		internal static IDisposable SetEventHistoryCleanupChunkSizeForTest(int chunkSize)
		{
			return EventHistory.eventHistoryCleanupChunkSize.SetTestHook(chunkSize);
		}

		internal static IDisposable SetEventHistoryCleanupRowsDeletedTestHook(Action<int> testDelegate)
		{
			return EventHistory.eventHistoryCleanupRowsDeletedTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetSimulateReadEventsFromPassiveTestHook(bool simulateReadEventsFromPassive)
		{
			return EventHistory.simulateReadEventsFromPassiveTestHook.SetTestHook(simulateReadEventsFromPassive);
		}

		internal static IDisposable SetInsertedEventHistoryRecordTestHook(Action<long> testDelegate)
		{
			return EventHistory.insertedEventHistoryRecordTestHook.SetTestHook(testDelegate);
		}

		internal static IDisposable SetEventCounterAllocatedTestHook(Action<int, EventType, long> testDelegate)
		{
			return EventHistory.eventCounterAllocatedTestHook.SetTestHook(testDelegate);
		}

		public static EventHistory GetEventHistory(StoreDatabase database)
		{
			return database.ComponentData[EventHistory.eventHistoryDataSlot] as EventHistory;
		}

		private long ComputeEventCounterBound(Context context, bool upperBound)
		{
			Column[] columnsToFetch = new Column[]
			{
				this.eventsTable.EventCounter
			};
			long result;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, KeyRange.AllRows, upperBound, false))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (reader.Read())
					{
						result = reader.GetInt64(this.eventsTable.EventCounter);
					}
					else
					{
						result = 0L;
					}
				}
			}
			return result;
		}

		private void SyncEventCounterBounds(Context context, bool readOnly)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(context.Database);
			Column[] columnsToFetch = new Column[]
			{
				globalsTable.EventCounterLowerBound,
				globalsTable.EventCounterUpperBound
			};
			using (TableOperator globalsTableRow = GlobalsTableHelper.GetGlobalsTableRow(context, columnsToFetch))
			{
				using (Reader reader = globalsTableRow.ExecuteReader(false))
				{
					reader.Read();
					long @int = reader.GetInt64(globalsTable.EventCounterLowerBound);
					long int2 = reader.GetInt64(globalsTable.EventCounterUpperBound);
					using (LockManager.Lock(this, LockManager.LockType.EventCounterBounds, context.Diagnostics))
					{
						if (@int > this.eventCounterLowerBound)
						{
							this.eventCounterLowerBound = @int;
						}
						else if (@int < this.eventCounterLowerBound)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(@int == 0L, "Persisted lower bound should never be lower than the actual EventHistory table contents.");
							if (!readOnly)
							{
								GlobalsTableHelper.UpdateGlobalsTableRow(context, new Column[]
								{
									globalsTable.EventCounterLowerBound
								}, new object[]
								{
									this.eventCounterLowerBound
								});
							}
						}
						if (int2 > this.eventCounterUpperBound)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(readOnly, "Persisted upper bound should never be ahead of the cached value on active databases.");
							this.eventCounterUpperBound = int2;
						}
						else if (int2 < this.eventCounterUpperBound && !readOnly)
						{
							GlobalsTableHelper.UpdateGlobalsTableRow(context, new Column[]
							{
								globalsTable.EventCounterUpperBound
							}, new object[]
							{
								this.eventCounterUpperBound
							});
						}
					}
				}
			}
		}

		public ErrorCode ReadEvents(Context context, long startCounter, uint eventsWant, uint eventsToCheck, Restriction restriction, EventHistory.ReadEventsFlags readFlags, out List<EventEntry> events, out long endCounter)
		{
			bool flag = this.IsReadingEventsFromPassive(context);
			events = null;
			endCounter = 0L;
			if (restriction != null)
			{
				if (flag)
				{
					if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ReadEventsTracer.TraceDebug<Restriction>(0L, "ReadEvents() on a passive database failed with a NotSupported error because a restriction was specified: {0}", restriction);
					}
					return ErrorCode.CreateNotSupported((LID)63692U);
				}
				if ((readFlags & (EventHistory.ReadEventsFlags)2147483648U) == EventHistory.ReadEventsFlags.None)
				{
					ErrorCode errorCode = this.VerifyRestriction(restriction);
					if (errorCode != ErrorCode.NoError)
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug<ErrorCode, Restriction>(0L, "ReadEvents() failed with error code {0} attempting to verify the specified restriction: {1}", errorCode, restriction);
						}
						return errorCode.Propagate((LID)46165U);
					}
				}
			}
			Interlocked.Increment(ref this.readerCount);
			try
			{
				if (flag)
				{
					if (!AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database))
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug(0L, "ReadEvents() from a passive database failed with a NotSupported error because the database hasn't been sufficiently upgraded yet.");
						}
						return ErrorCode.CreateNotSupported((LID)49484U);
					}
					context.BeginTransactionIfNeeded();
					this.SyncEventCounterBounds(context, true);
					if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadEvents() synced event counter bounds for the passive database: eventCounterLowerBound={0}, eventCounterUpperBound={1}", this.eventCounterLowerBound, this.eventCounterUpperBound);
					}
				}
				if (startCounter < this.eventCounterLowerBound)
				{
					if ((readFlags & EventHistory.ReadEventsFlags.FailIfEventsDeleted) != EventHistory.ReadEventsFlags.None)
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadEvents() failed with an EventsDeleted error (LID 62549) because the FailIfEventsDeleted flag was specified and the startCounter ({0}) is less than the event counter lower bound ({1}).", startCounter, this.eventCounterLowerBound);
						}
						return ErrorCode.CreateEventsDeleted((LID)62549U);
					}
					startCounter = this.eventCounterLowerBound;
				}
				if (eventsWant == 0U)
				{
					if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ReadEventsTracer.TraceDebug(0L, "ReadEvents() could not return any events because eventsWant=0.");
					}
					return ErrorCode.NoError;
				}
				if (eventsWant > 1000U)
				{
					eventsWant = 1000U;
				}
				if (eventsToCheck > 10000U || eventsToCheck < 1000U)
				{
					eventsToCheck = 10000U;
				}
				long num = this.eventCounterUpperBound;
				long num2;
				if (flag)
				{
					num2 = startCounter + (long)((ulong)eventsToCheck);
				}
				else
				{
					num2 = num;
					if (startCounter >= num2)
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadEvents() could not return any events because the start counter equals or exceeds the upper bound (startCounter={0}, effectiveUpperBound={1}).", startCounter, num2);
						}
						return ErrorCode.NoError;
					}
					if (num2 - startCounter > (long)((ulong)eventsToCheck))
					{
						num2 = startCounter + (long)((ulong)eventsToCheck);
					}
				}
				long num3 = (long)((ulong)((eventsWant < 50U) ? eventsWant : 50U));
				if (num3 > num2 - startCounter)
				{
					num3 = num2 - startCounter;
				}
				events = new List<EventEntry>((int)num3);
				SearchCriteria searchCriteria = null;
				bool flag2 = false;
				if (!ConfigurationSchema.SkipMoveEventExclusion.Value && (readFlags & EventHistory.ReadEventsFlags.IncludeMoveDestinationEvents) == EventHistory.ReadEventsFlags.None)
				{
					flag2 = true;
					if (!flag)
					{
						Restriction restriction2 = new RestrictionBitmask(PropTag.Event.EventExtendedFlags, 16L, BitmaskOperation.EqualToZero);
						if (restriction != null)
						{
							restriction = new RestrictionAND(new Restriction[]
							{
								restriction2,
								restriction
							});
						}
						else
						{
							restriction = restriction2;
						}
					}
				}
				if (restriction != null)
				{
					searchCriteria = restriction.ToSearchCriteria(this.database, ObjectType.Event);
					searchCriteria = this.FixEventReadCriteria(context, searchCriteria);
				}
				if (searchCriteria is SearchCriteriaFalse)
				{
					if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ReadEventsTracer.TraceDebug<Restriction>(0L, "ReadEvents() didn't return any events because it was determined that no entries would satisfy the specified restriction: {0}", restriction);
					}
				}
				else
				{
					long num4 = startCounter - 1L;
					StartStopKey startKey = new StartStopKey(true, new object[]
					{
						startCounter
					});
					StartStopKey stopKey = new StartStopKey(false, new object[]
					{
						num2
					});
					using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, this.eventsFetchList, searchCriteria, null, 0, 0, new KeyRange(startKey, stopKey), false, true))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
							{
								context.BeginTransactionIfNeeded();
							}
							int num5 = 0;
							while ((long)events.Count < (long)((ulong)eventsWant))
							{
								if (++num5 % 128 == 0 && context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
								{
									context.Commit();
									context.BeginTransactionIfNeeded();
								}
								if (!reader.Read())
								{
									break;
								}
								if (EventHistory.readerRaceTestHook != null)
								{
									EventHistory.readerRaceTestHook();
								}
								long @int = reader.GetInt64(this.eventsTable.EventCounter);
								if (flag)
								{
									if (@int != num4 + 1L)
									{
										long num6 = EventHistory.simulateReadEventsFromPassiveTestHook.Value ? this.GetPersistedEventCounterBound(context, true) : this.eventCounterUpperBound;
										if (@int >= num6)
										{
											if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
											{
												ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long, long>(0L, "ReadEvents() stopped before reaching the end of the EventHistory table because there was a counter gap between {0} and {1} (eventCounterUpperBound={2}).", num4, @int, num6);
												break;
											}
											break;
										}
									}
									num4 = @int;
									if (flag2)
									{
										long? nullableInt = reader.GetNullableInt64(this.eventsTable.ExtendedFlags);
										ExtendedEventFlags? extendedEventFlags = (nullableInt != null) ? new ExtendedEventFlags?((ExtendedEventFlags)nullableInt.GetValueOrDefault()) : null;
										if (extendedEventFlags != null && (extendedEventFlags & ExtendedEventFlags.MoveDestination) != ExtendedEventFlags.None)
										{
											continue;
										}
									}
								}
								int int2 = reader.GetInt32(this.eventsTable.MailboxNumber);
								MailboxState mailboxState = MailboxStateCache.Get(context, int2);
								if (mailboxState != null && !mailboxState.IsTombstone)
								{
									EventType int3 = (EventType)reader.GetInt32(this.eventsTable.EventType);
									if ((int3 & (EventType.MailboxCreated | EventType.MailboxDeleted | EventType.MailboxMoveStarted | EventType.MailboxMoveSucceeded | EventType.MailboxMoveFailed)) != (EventType)0 || !mailboxState.IsDeleted)
									{
										Guid mailboxGuid = mailboxState.MailboxGuid;
										Guid mailboxInstanceGuid = mailboxState.MailboxInstanceGuid;
										DateTime dateTime = reader.GetDateTime(this.eventsTable.CreateTime);
										int int4 = reader.GetInt32(this.eventsTable.TransactionId);
										ClientType clientType = (ClientType)reader.GetInt32(this.eventsTable.ClientType);
										EventFlags int5 = (EventFlags)reader.GetInt32(this.eventsTable.Flags);
										long? nullableInt2 = reader.GetNullableInt64(this.eventsTable.ExtendedFlags);
										ExtendedEventFlags? extendedFlags = (nullableInt2 != null) ? new ExtendedEventFlags?((ExtendedEventFlags)nullableInt2.GetValueOrDefault()) : null;
										string @string = reader.GetString(this.eventsTable.ObjectClass);
										byte[] binary = reader.GetBinary(this.eventsTable.Fid);
										byte[] binary2 = reader.GetBinary(this.eventsTable.Mid);
										byte[] binary3 = reader.GetBinary(this.eventsTable.ParentFid);
										byte[] binary4 = reader.GetBinary(this.eventsTable.OldFid);
										byte[] binary5 = reader.GetBinary(this.eventsTable.OldMid);
										byte[] binary6 = reader.GetBinary(this.eventsTable.OldParentFid);
										int? nullableInt3 = reader.GetNullableInt32(this.eventsTable.ItemCount);
										int? nullableInt4 = reader.GetNullableInt32(this.eventsTable.UnreadCount);
										byte[] binary7 = reader.GetBinary(this.eventsTable.Sid);
										int? nullableInt5 = reader.GetNullableInt32(this.eventsTable.DocumentId);
										if ((readFlags & EventHistory.ReadEventsFlags.FailIfEventsDeleted) != EventHistory.ReadEventsFlags.None && startCounter < this.eventCounterLowerBound)
										{
											if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
											{
												ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadEvents() failed with an EventsDeleted error (LID 37973) because the FailIfEventsDeleted flag was specified and the startCounter ({0}) is less than the event counter lower bound ({1}).", startCounter, this.eventCounterLowerBound);
											}
											return ErrorCode.CreateEventsDeleted((LID)37973U);
										}
										Guid? unifiedMailboxGuid = null;
										if (mailboxState.UnifiedState != null)
										{
											unifiedMailboxGuid = new Guid?(mailboxState.UnifiedState.UnifiedMailboxGuid);
										}
										EventEntry item = new EventEntry(@int, dateTime, int4, int3, new int?(int2), new Guid?(mailboxGuid), new Guid?(mailboxInstanceGuid), @string, binary, binary2, binary3, binary4, binary5, binary6, nullableInt3, nullableInt4, int5, extendedFlags, clientType, binary7, nullableInt5, mailboxState.TenantHint, unifiedMailboxGuid);
										events.Add(item);
										if ((long)events.Count == (long)((ulong)eventsWant))
										{
											break;
										}
									}
								}
							}
						}
					}
					if ((readFlags & EventHistory.ReadEventsFlags.FailIfEventsDeleted) != EventHistory.ReadEventsFlags.None && startCounter < this.eventCounterLowerBound)
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadEvents() failed with an EventsDeleted error (LID 54357) because the FailIfEventsDeleted flag was specified and the startCounter ({0}) is less than the event counter lower bound ({1}).", startCounter, this.eventCounterLowerBound);
						}
						return ErrorCode.CreateEventsDeleted((LID)54357U);
					}
				}
				if (flag)
				{
					if (events.Count > 0)
					{
						endCounter = events[events.Count - 1].EventCounter;
						this.UpdateEventCounterUpperBoundAfterGapScan(context, endCounter);
					}
					else if (num > startCounter)
					{
						endCounter = num - 1L;
					}
					else
					{
						endCounter = 0L;
						events = null;
					}
				}
				else if ((long)events.Count == (long)((ulong)eventsWant))
				{
					endCounter = events[events.Count - 1].EventCounter;
				}
				else
				{
					endCounter = num2 - 1L;
				}
				if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ReadEventsTracer.TraceDebug(0L, "ReadEvents() is returning {0} events (isPassive={1}, startCounter={2}, endCounter={3}, effectiveUpperBound={4}, initialUpperBound={5}, eventsWant={6}, eventsToCheck={7}, readFlags=[{8}], restriction=[{9}]).", new object[]
					{
						(events != null) ? events.Count : 0,
						flag,
						startCounter,
						endCounter,
						num2,
						num,
						eventsWant,
						eventsToCheck,
						readFlags,
						searchCriteria
					});
				}
			}
			finally
			{
				context.Commit();
				Interlocked.Decrement(ref this.readerCount);
			}
			return ErrorCode.NoError;
		}

		public ErrorCode ReadLastEvent(Context context, out EventEntry e)
		{
			bool flag = this.IsReadingEventsFromPassive(context);
			if (flag)
			{
				if (!AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database))
				{
					if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ReadEventsTracer.TraceDebug(0L, "ReadLastEvent() from a passive database failed with a NotSupported error because the database hasn't been sufficiently upgraded yet.");
					}
					e = null;
					return ErrorCode.CreateNotSupported((LID)48972U);
				}
				this.SyncEventCounterBounds(context, true);
				if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadLastEvent() synced event counter bounds for the passive database: eventCounterLowerBound={0}, eventCounterUpperBound={1}", this.eventCounterLowerBound, this.eventCounterUpperBound);
				}
			}
			ErrorCode errorCode = ErrorCode.NoError;
			long num = EventHistory.simulateReadEventsFromPassiveTestHook.Value ? this.GetPersistedEventCounterBound(context, true) : this.eventCounterUpperBound;
			StartStopKey startKey = new StartStopKey(false, new object[]
			{
				num
			});
			StartStopKey empty = StartStopKey.Empty;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, this.lastEventFetchList, null, null, 0, 1, new KeyRange(startKey, empty), true, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (reader.Read())
					{
						long @int = reader.GetInt64(this.eventsTable.EventCounter);
						DateTime dateTime = reader.GetDateTime(this.eventsTable.CreateTime);
						e = new EventEntry(@int, dateTime);
					}
					else if (num == 1L)
					{
						e = new EventEntry(0L, DateTime.UtcNow);
					}
					else
					{
						DiagnosticContext.TraceDword((LID)41504U, (uint)((ulong)num >> 32));
						DiagnosticContext.TraceDword((LID)53792U, (uint)num);
						errorCode = ErrorCode.CreateEventNotFound((LID)42069U);
						e = null;
					}
				}
			}
			if (flag && e != null)
			{
				long num2 = e.EventCounter;
				DateTime createTime = e.CreateTime;
				if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "ReadLastEvent() is scanning for event counter gaps beginning from counter value {0} (effectiveUpperBound={1}).", num2, num);
				}
				startKey = new StartStopKey(false, new object[]
				{
					num2
				});
				empty = StartStopKey.Empty;
				using (TableOperator tableOperator2 = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, this.lastEventFetchList, null, null, 0, 0, new KeyRange(startKey, empty), false, true))
				{
					using (Reader reader2 = tableOperator2.ExecuteReader(false))
					{
						try
						{
							if (context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
							{
								context.BeginTransactionIfNeeded();
							}
							int num3 = 0;
							while (reader2.Read())
							{
								if (++num3 % 128 == 0 && context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
								{
									context.Commit();
									context.BeginTransactionIfNeeded();
								}
								long int2 = reader2.GetInt64(this.eventsTable.EventCounter);
								if (int2 != num2 + 1L)
								{
									if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long, long>(0L, "ReadLastEvent() stopped before reaching the end of the EventHistory table because there was a counter gap between {0} and {1} (eventCounterUpperBound={2}).", num2, int2, num);
										break;
									}
									break;
								}
								else
								{
									num2 = int2;
									createTime = reader2.GetDateTime(this.eventsTable.CreateTime);
								}
							}
							if (num3 > 0)
							{
								e = new EventEntry(num2, createTime);
							}
						}
						finally
						{
							context.Commit();
						}
					}
				}
				this.UpdateEventCounterUpperBoundAfterGapScan(context, e.EventCounter);
			}
			if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ReadEventsTracer.TraceDebug(0L, "ReadLastEvent(): isPassive={0}, eventCounter={1}, createTime={2}, ec={3}, effectiveUpperBound={4}", new object[]
				{
					flag,
					(e != null) ? e.EventCounter : 0L,
					(e != null) ? e.CreateTime : DateTime.MinValue,
					errorCode,
					num
				});
			}
			return errorCode;
		}

		private bool IsReadingEventsFromPassive(Context context)
		{
			bool flag = context.Database.IsSharedLockHeld() || context.Database.IsExclusiveLockHeld();
			return EventHistory.simulateReadEventsFromPassiveTestHook.Value || (flag && context.Database.IsOnlinePassiveAttachedReadOnly);
		}

		internal long GetPersistedEventCounterBound(Context context, bool upperBound)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(context.Database);
			Column column = upperBound ? globalsTable.EventCounterUpperBound : globalsTable.EventCounterLowerBound;
			long @int;
			using (TableOperator globalsTableRow = GlobalsTableHelper.GetGlobalsTableRow(context, new Column[]
			{
				column
			}))
			{
				using (Reader reader = globalsTableRow.ExecuteReader(false))
				{
					reader.Read();
					@int = reader.GetInt64(column);
				}
			}
			return @int;
		}

		private void UpdateEventCounterUpperBoundAfterGapScan(Context context, long endCounter)
		{
			if (endCounter + 1L > this.eventCounterUpperBound)
			{
				using (LockManager.Lock(this, LockManager.LockType.EventCounterBounds, context.Diagnostics))
				{
					if (endCounter + 1L > this.eventCounterUpperBound)
					{
						if (ExTraceGlobals.ReadEventsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ReadEventsTracer.TraceDebug<long, long>(0L, "The event counter upper bound will be updated from {0} to {1} after scanning events and determining that there were no counter gaps between the two values.", this.eventCounterUpperBound, endCounter + 1L);
						}
						this.eventCounterUpperBound = endCounter + 1L;
					}
				}
			}
		}

		public ErrorCode WriteEvents(Context context, List<EventEntry> events, out List<long> eventCounters)
		{
			eventCounters = new List<long>(events.Count);
			if (events.Count != 0)
			{
				for (int i = 0; i < events.Count; i++)
				{
					EventEntry eventEntry = events[i];
					int value;
					if (eventEntry.MailboxNumber == null)
					{
						if (!MailboxStateCache.TryGetMailboxNumber(context, eventEntry.MailboxGuid.Value, true, out value))
						{
							return ErrorCode.CreateUnknownMailbox((LID)44056U);
						}
					}
					else
					{
						value = eventEntry.MailboxNumber.Value;
					}
					long item;
					this.InsertOneEvent(context, eventEntry.TransactionId, eventEntry.EventType, value, eventEntry.ObjectClass, eventEntry.Fid24, eventEntry.Mid24, eventEntry.ParentFid24, eventEntry.OldFid24, eventEntry.OldMid24, eventEntry.OldParentFid24, eventEntry.ItemCount, eventEntry.UnreadCount, eventEntry.Flags, eventEntry.ExtendedFlags, eventEntry.ClientType, eventEntry.Sid, null, out item);
					eventCounters.Add(item);
				}
			}
			return ErrorCode.NoError;
		}

		public ErrorCode SaveWatermarks(Context context, List<EventWatermark> watermarks)
		{
			ErrorCode result = ErrorCode.NoError;
			if (watermarks != null && watermarks.Count > 0)
			{
				bool flag = false;
				for (int i = 1; i < watermarks.Count; i++)
				{
					if (watermarks[i].ConsumerGuid != watermarks[0].ConsumerGuid)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableExclusive, context.Diagnostics))
					{
						return this.WriteWatermarks(context, watermarks);
					}
				}
				using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableShared, context.Diagnostics))
				{
					using (LockManager.Lock(EventHistory.WatermarksConsumerLockName(this.database.MdbGuid, watermarks[0].ConsumerGuid), LockManager.LockType.WatermarkConsumer, context.Diagnostics))
					{
						result = this.WriteWatermarks(context, watermarks);
					}
				}
			}
			return result;
		}

		public ErrorCode ReadWatermarksForMailbox(Context context, Guid mailboxGuid, out List<EventWatermark> watermarks)
		{
			ErrorCode result = ErrorCode.NoError;
			int estimateNumber = 10;
			int value = 0;
			if (mailboxGuid != Guid.Empty && !MailboxStateCache.TryGetMailboxNumber(context, mailboxGuid, true, out value))
			{
				watermarks = new List<EventWatermark>(0);
				return ErrorCode.NoError;
			}
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableExclusive, context.Diagnostics))
			{
				result = this.ReadWatermarks(context, new int?(value), null, estimateNumber, out watermarks);
			}
			return result;
		}

		public ErrorCode ReadWatermarksForMailboxForTest(Context context, int mailboxNumber, out List<EventWatermark> watermarks)
		{
			ErrorCode result;
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableExclusive, context.Diagnostics))
			{
				result = this.ReadWatermarks(context, new int?(mailboxNumber), null, 10, out watermarks);
			}
			return result;
		}

		public ErrorCode ReadWatermarksForConsumer(Context context, Guid consumerGuid, Guid? mailboxGuid, out List<EventWatermark> watermarks)
		{
			ErrorCode result = ErrorCode.NoError;
			int estimateNumber = 100;
			int? mailboxNumber = null;
			if (mailboxGuid != null)
			{
				int value = 0;
				if (mailboxGuid != Guid.Empty && !MailboxStateCache.TryGetMailboxNumber(context, mailboxGuid.Value, true, out value))
				{
					watermarks = new List<EventWatermark>(0);
					return ErrorCode.NoError;
				}
				mailboxNumber = new int?(value);
				estimateNumber = 1;
			}
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableShared, context.Diagnostics))
			{
				using (LockManager.Lock(EventHistory.WatermarksConsumerLockName(this.database.MdbGuid, consumerGuid), LockManager.LockType.WatermarkConsumer, context.Diagnostics))
				{
					result = this.ReadWatermarks(context, mailboxNumber, new Guid?(consumerGuid), estimateNumber, out watermarks);
				}
			}
			return result;
		}

		public ErrorCode DeleteWatermarksForMailbox(Context context, Guid mailboxGuid, out uint deletedCount)
		{
			ErrorCode noError = ErrorCode.NoError;
			int mailboxNumber = 0;
			if (mailboxGuid != Guid.Empty && !MailboxStateCache.TryGetMailboxNumber(context, mailboxGuid, true, out mailboxNumber))
			{
				deletedCount = 0U;
				return ErrorCode.NoError;
			}
			this.DeleteWatermarksForMailbox(context, mailboxNumber, out deletedCount);
			return noError;
		}

		public void DeleteWatermarksForMailbox(Context context, int mailboxNumber, out uint deletedCount)
		{
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableExclusive, context.Diagnostics))
			{
				this.DeleteWatermarks(context, new int?(mailboxNumber), null, out deletedCount);
			}
		}

		public ErrorCode DeleteWatermarksForConsumer(Context context, Guid consumerGuid, Guid? mailboxGuid, out uint deletedCount)
		{
			ErrorCode noError = ErrorCode.NoError;
			int? mailboxNumber = null;
			if (mailboxGuid != null)
			{
				int value = 0;
				if (mailboxGuid != Guid.Empty && !MailboxStateCache.TryGetMailboxNumber(context, mailboxGuid.Value, true, out value))
				{
					deletedCount = 0U;
					return ErrorCode.NoError;
				}
				mailboxNumber = new int?(value);
			}
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableShared, context.Diagnostics))
			{
				using (LockManager.Lock(EventHistory.WatermarksConsumerLockName(this.database.MdbGuid, consumerGuid), LockManager.LockType.WatermarkConsumer, context.Diagnostics))
				{
					this.DeleteWatermarks(context, mailboxNumber, new Guid?(consumerGuid), out deletedCount);
				}
			}
			return noError;
		}

		public static void EventHistoryCleanupMaintenance(Context context, DatabaseInfo databaseInfo, out bool completed)
		{
			EventHistory eventHistory = EventHistory.GetEventHistory(context.Database);
			if (!eventHistory.CleanupEventHistory(context, databaseInfo.EventHistoryRetentionPeriod))
			{
				completed = false;
				return;
			}
			eventHistory.CleanupWatermarks(context);
			completed = true;
		}

		private ErrorCode WriteWatermarks(Context context, List<EventWatermark> watermarks)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			long effectiveUpperBound = this.eventCounterUpperBound;
			try
			{
				for (int i = 0; i < watermarks.Count; i++)
				{
					EventWatermark eventWatermark = watermarks[i];
					int num;
					if (!this.WatermarkValid(context, ref eventWatermark, effectiveUpperBound, out num))
					{
						if (errorCode == ErrorCode.NoError)
						{
							errorCode = ErrorCode.CreatePartialCompletion((LID)58453U);
						}
					}
					else
					{
						DataRow dataRow = null;
						try
						{
							dataRow = Factory.OpenDataRow(context.Culture, context, this.watermarksTable.Table, true, new ColumnValue[]
							{
								new ColumnValue(this.watermarksTable.MailboxNumber, num),
								new ColumnValue(this.watermarksTable.ConsumerGuid, eventWatermark.ConsumerGuid)
							});
							if (dataRow != null)
							{
								dataRow.SetValue(context, this.watermarksTable.EventCounter, eventWatermark.EventCounter);
							}
							else
							{
								dataRow = Factory.CreateDataRow(context.Culture, context, this.watermarksTable.Table, true, new ColumnValue[]
								{
									new ColumnValue(this.watermarksTable.MailboxNumber, num),
									new ColumnValue(this.watermarksTable.ConsumerGuid, eventWatermark.ConsumerGuid),
									new ColumnValue(this.watermarksTable.EventCounter, eventWatermark.EventCounter)
								});
							}
							dataRow.Flush(context);
						}
						finally
						{
							if (dataRow != null)
							{
								dataRow.Dispose();
							}
						}
					}
				}
				context.Commit();
			}
			finally
			{
				if (context.TransactionStarted)
				{
					context.Abort();
				}
			}
			return errorCode;
		}

		private bool WatermarkValid(Context context, ref EventWatermark watermark, long effectiveUpperBound, out int mailboxNumber)
		{
			mailboxNumber = 0;
			return watermark.EventCounter <= effectiveUpperBound && (!(watermark.MailboxGuid != Guid.Empty) || MailboxStateCache.TryGetMailboxNumber(context, watermark.MailboxGuid, true, out mailboxNumber));
		}

		private ErrorCode ReadWatermarks(Context context, int? mailboxNumber, Guid? consumerGuid, int estimateNumber, out List<EventWatermark> watermarks)
		{
			watermarks = new List<EventWatermark>(estimateNumber);
			StartStopKey empty = StartStopKey.Empty;
			SearchCriteriaCompare restriction = null;
			if (consumerGuid != null)
			{
				if (mailboxNumber != null)
				{
					empty = new StartStopKey(true, new object[]
					{
						consumerGuid.Value,
						mailboxNumber.Value
					});
				}
				else
				{
					empty = new StartStopKey(true, new object[]
					{
						consumerGuid.Value
					});
				}
			}
			else
			{
				restriction = Factory.CreateSearchCriteriaCompare(this.watermarksTable.MailboxNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailboxNumber));
			}
			try
			{
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, this.watermarksTable.Table, this.watermarksTable.Table.PrimaryKeyIndex, this.watermarksFetchList, restriction, null, 0, 0, new KeyRange(empty, empty), false, false))
				{
					using (SortOperator sortOperator = this.CreateWatermarkTableSortOperatorIfNeeded(context, tableOperator, mailboxNumber, consumerGuid))
					{
						SimpleQueryOperator simpleQueryOperator = (sortOperator != null) ? sortOperator : tableOperator;
						using (Reader reader = simpleQueryOperator.ExecuteReader(false))
						{
							while (reader.Read())
							{
								Guid guid = reader.GetGuid(this.watermarksTable.ConsumerGuid);
								int @int = reader.GetInt32(this.watermarksTable.MailboxNumber);
								long int2 = reader.GetInt64(this.watermarksTable.EventCounter);
								Guid mailboxGuid = Guid.Empty;
								if (@int != 0)
								{
									MailboxState mailboxState = MailboxStateCache.Get(context, @int);
									if (mailboxState == null || mailboxState.IsDeleted || mailboxState.IsDisconnected)
									{
										continue;
									}
									mailboxGuid = mailboxState.MailboxGuid;
								}
								EventWatermark item = new EventWatermark(mailboxGuid, guid, int2);
								watermarks.Add(item);
							}
						}
					}
				}
				context.Commit();
			}
			finally
			{
				if (context.TransactionStarted)
				{
					context.Abort();
				}
			}
			return ErrorCode.NoError;
		}

		private SortOperator CreateWatermarkTableSortOperatorIfNeeded(IConnectionProvider connectionProvider, TableOperator tableOperator, int? mailboxNumber, Guid? consumerGuid)
		{
			if (mailboxNumber != null && consumerGuid != null)
			{
				return null;
			}
			SortOrder sortOrder = new SortOrder(new Column[]
			{
				this.watermarksTable.EventCounter,
				this.watermarksTable.ConsumerGuid,
				this.watermarksTable.MailboxNumber
			}, new bool[]
			{
				true,
				true,
				true
			});
			return Factory.CreateSortOperator(tableOperator.Culture, connectionProvider, tableOperator, 0, 0, sortOrder, KeyRange.AllRows, false, false);
		}

		private void DeleteWatermarks(Context context, int? mailboxNumber, Guid? consumerGuid, out uint deletedCount)
		{
			deletedCount = 0U;
			StartStopKey empty = StartStopKey.Empty;
			StartStopKey empty2 = StartStopKey.Empty;
			SearchCriteriaCompare restriction = null;
			if (consumerGuid != null)
			{
				if (mailboxNumber != null)
				{
					empty = new StartStopKey(true, new object[]
					{
						consumerGuid.Value,
						mailboxNumber.Value
					});
				}
				else
				{
					empty = new StartStopKey(true, new object[]
					{
						consumerGuid.Value
					});
				}
				empty2 = new StartStopKey(true, new object[]
				{
					consumerGuid.Value
				});
			}
			if (mailboxNumber != null)
			{
				restriction = Factory.CreateSearchCriteriaCompare(this.watermarksTable.MailboxNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailboxNumber));
			}
			try
			{
				using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, this.watermarksTable.Table, this.watermarksTable.Table.PrimaryKeyIndex, null, restriction, null, 0, 0, new KeyRange(empty, empty2), false, false), false))
				{
					deletedCount = (uint)((int)deleteOperator.ExecuteScalar());
				}
				context.Commit();
			}
			finally
			{
				if (context.TransactionStarted)
				{
					context.Abort();
				}
			}
		}

		internal void InsertOneEvent(Context context, int transactionId, EventType eventType, int mailboxNumber, string objectClass, byte[] fid24, byte[] mid24, byte[] parentFid24, byte[] oldFid24, byte[] oldMid24, byte[] oldParentFid24, int? itemCount, int? unreadCount, EventFlags flags, ExtendedEventFlags? extendedFlags, ClientType clientType, byte[] sid, int? documentId, out long eventCounter)
		{
			bool flag = false;
			if (objectClass != null && objectClass.Length > this.eventsTable.ObjectClass.MaxLength)
			{
				objectClass = objectClass.Substring(0, this.eventsTable.ObjectClass.MaxLength);
				flag = true;
			}
			if (transactionId == 0)
			{
				transactionId = context.GetConnection().TransactionId;
			}
			MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
			if (mailboxState.IsMailboxLocked())
			{
				InTransitStatus inTransitStatus = InTransitInfo.GetInTransitStatus(mailboxState);
				if ((inTransitStatus & InTransitStatus.DirectionMask) == InTransitStatus.DestinationOfMove && (eventType & (EventType.MailboxMoveStarted | EventType.MailboxMoveSucceeded | EventType.MailboxMoveFailed)) == (EventType)0)
				{
					if (extendedFlags == null)
					{
						extendedFlags = new ExtendedEventFlags?(ExtendedEventFlags.None);
					}
					extendedFlags = new ExtendedEventFlags?(extendedFlags.Value | ExtendedEventFlags.MoveDestination);
				}
			}
			switch (mailboxState.MailboxType)
			{
			case MailboxInfo.MailboxType.Private:
				break;
			case MailboxInfo.MailboxType.PublicFolderPrimary:
			case MailboxInfo.MailboxType.PublicFolderSecondary:
				extendedFlags = new ExtendedEventFlags?((extendedFlags != null) ? (extendedFlags.Value | ExtendedEventFlags.PublicFolderMailbox) : ExtendedEventFlags.PublicFolderMailbox);
				break;
			default:
				throw new StoreException((LID)45992U, ErrorCodeValue.InvalidParameter, "Invalid mailbox type");
			}
			ColumnValue[] array = new ColumnValue[18];
			array[0] = new ColumnValue(this.eventsTable.CreateTime, DateTime.UtcNow);
			array[1] = new ColumnValue(this.eventsTable.TransactionId, transactionId);
			array[2] = new ColumnValue(this.eventsTable.EventType, SerializedValue.GetBoxedInt32((int)eventType));
			array[3] = new ColumnValue(this.eventsTable.MailboxNumber, mailboxNumber);
			array[4] = new ColumnValue(this.eventsTable.ObjectClass, objectClass);
			array[5] = new ColumnValue(this.eventsTable.Fid, fid24);
			array[6] = new ColumnValue(this.eventsTable.Mid, mid24);
			array[7] = new ColumnValue(this.eventsTable.ParentFid, parentFid24);
			array[8] = new ColumnValue(this.eventsTable.OldFid, oldFid24);
			array[9] = new ColumnValue(this.eventsTable.OldMid, oldMid24);
			array[10] = new ColumnValue(this.eventsTable.OldParentFid, oldParentFid24);
			array[11] = new ColumnValue(this.eventsTable.ItemCount, itemCount);
			array[12] = new ColumnValue(this.eventsTable.UnreadCount, unreadCount);
			array[13] = new ColumnValue(this.eventsTable.Flags, SerializedValue.GetBoxedInt32((int)flags));
			ColumnValue[] array2 = array;
			int num = 14;
			Column extendedFlags2 = this.eventsTable.ExtendedFlags;
			ExtendedEventFlags? extendedEventFlags = extendedFlags;
			array2[num] = new ColumnValue(extendedFlags2, (extendedEventFlags != null) ? new long?((long)extendedEventFlags.GetValueOrDefault()) : null);
			array[15] = new ColumnValue(this.eventsTable.ClientType, SerializedValue.GetBoxedInt32((int)clientType));
			array[16] = new ColumnValue(this.eventsTable.Sid, sid);
			array[17] = new ColumnValue(this.eventsTable.DocumentId, documentId);
			ColumnValue[] columnValues = array;
			bool flag2 = false;
			if (context.EventHistoryUncommittedTransactionLink != null)
			{
				LinkedListNode<EventHistory.UncommittedListEntry> linkedListNode = context.EventHistoryUncommittedTransactionLink as LinkedListNode<EventHistory.UncommittedListEntry>;
				eventCounter = this.InsertEventHistoryRecord(context, columnValues);
				flag2 = true;
				linkedListNode.Value.LastCounter = eventCounter;
				if (EventHistory.eventCounterAllocatedTestHook.Value != null)
				{
					EventHistory.eventCounterAllocatedTestHook.Value(mailboxNumber, eventType, eventCounter);
				}
			}
			else
			{
				eventCounter = 0L;
			}
			using (LockManager.Lock(this, LockManager.LockType.EventCounterBounds, context.Diagnostics))
			{
				if (!flag2)
				{
					bool flag3 = this.eventCounterAllocationPotentiallyLost;
					this.eventCounterAllocationPotentiallyLost = true;
					eventCounter = this.InsertEventHistoryRecord(context, columnValues);
					this.lastEventCounter = eventCounter;
					LinkedListNode<EventHistory.UncommittedListEntry> eventHistoryUncommittedTransactionLink = this.uncommittedList.AddLast(new EventHistory.UncommittedListEntry(context, eventCounter));
					context.EventHistoryUncommittedTransactionLink = eventHistoryUncommittedTransactionLink;
					context.RegisterStateObject(this);
					if (flag3)
					{
						this.flushEventCounterUpperBoundTaskManager.HandlePotentiallyLostEventCounterAllocation(context, this);
					}
					this.eventCounterAllocationPotentiallyLost = false;
				}
				else if (eventCounter > this.lastEventCounter)
				{
					this.lastEventCounter = eventCounter;
				}
			}
			if (EventHistory.eventCounterAllocatedTestHook.Value != null && !flag2)
			{
				EventHistory.eventCounterAllocatedTestHook.Value(mailboxNumber, eventType, eventCounter);
			}
			if (ExTraceGlobals.EventsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				stringBuilder.Append("Event Registered: eventCounter:[");
				stringBuilder.Append(eventCounter);
				stringBuilder.Append("] eventType:[");
				stringBuilder.Append(eventType);
				stringBuilder.Append("] Database:[");
				stringBuilder.Append(context.Database.MdbGuid);
				stringBuilder.Append("] mailboxNumber:[");
				stringBuilder.AppendAsString(mailboxNumber);
				stringBuilder.Append("] transactionId:[");
				stringBuilder.Append(transactionId);
				stringBuilder.Append("] fid:[");
				stringBuilder.AppendAsString(fid24);
				stringBuilder.Append("] mid:[");
				stringBuilder.AppendAsString(mid24);
				stringBuilder.Append("] parentFid:[");
				stringBuilder.AppendAsString(parentFid24);
				stringBuilder.Append("] oldFid:[");
				stringBuilder.AppendAsString(oldFid24);
				stringBuilder.Append("] oldMid:[");
				stringBuilder.AppendAsString(oldMid24);
				stringBuilder.Append("] oldParentFid:[");
				stringBuilder.AppendAsString(oldParentFid24);
				stringBuilder.Append("] objectClass:[");
				stringBuilder.AppendAsString(objectClass);
				if (flag)
				{
					stringBuilder.Append("...");
				}
				stringBuilder.Append("] itemCount:[");
				stringBuilder.AppendAsString(itemCount);
				stringBuilder.Append("] unreadCount:[");
				stringBuilder.AppendAsString(unreadCount);
				stringBuilder.Append("] flags:[");
				stringBuilder.Append(flags);
				stringBuilder.Append("] extendedFlags:[");
				stringBuilder.AppendAsString(extendedFlags);
				stringBuilder.Append("] clientType:[");
				stringBuilder.Append(clientType);
				stringBuilder.Append("] sid:[");
				stringBuilder.AppendAsString(sid);
				stringBuilder.Append("] documentId:[");
				stringBuilder.AppendAsString(documentId);
				stringBuilder.Append("]");
				ExTraceGlobals.EventsTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private long InsertEventHistoryRecord(Context context, ColumnValue[] columnValues)
		{
			long num;
			using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, this.eventsTable.Table, true, columnValues))
			{
				dataRow.Flush(context);
				num = (long)dataRow.GetValue(context, this.eventsTable.EventCounter);
				if (EventHistory.insertedEventHistoryRecordTestHook.Value != null)
				{
					EventHistory.insertedEventHistoryRecordTestHook.Value(num);
				}
			}
			return num;
		}

		void IStateObject.OnBeforeCommit(Context context)
		{
		}

		void IStateObject.OnCommit(Context context)
		{
			this.EndTransactionHandler(context, true);
		}

		void IStateObject.OnAbort(Context context)
		{
			this.EndTransactionHandler(context, false);
		}

		private void EndTransactionHandler(Context context, bool committed)
		{
			LinkedListNode<EventHistory.UncommittedListEntry> linkedListNode = context.EventHistoryUncommittedTransactionLink as LinkedListNode<EventHistory.UncommittedListEntry>;
			bool flag = false;
			bool flag2 = false;
			using (LockManager.Lock(this, LockManager.LockType.EventCounterBounds, context.Diagnostics))
			{
				long lastCounter = linkedListNode.Value.LastCounter;
				if (committed && lastCounter > this.highestCommittedEventCounter)
				{
					this.highestCommittedEventCounter = lastCounter;
					flag = true;
				}
				if (linkedListNode == this.uncommittedList.First)
				{
					long num;
					if (linkedListNode.Next != null && linkedListNode.Next.Value.FirstCounter < this.highestCommittedEventCounter)
					{
						num = linkedListNode.Next.Value.FirstCounter;
					}
					else
					{
						num = this.highestCommittedEventCounter + 1L;
					}
					if (this.eventCounterUpperBound != num)
					{
						this.eventCounterUpperBound = num;
						flag2 = true;
					}
				}
				this.uncommittedList.Remove(linkedListNode);
				this.flushEventCounterUpperBoundTaskManager.EndTransactionHandler(context, this, lastCounter, committed);
			}
			context.EventHistoryUncommittedTransactionLink = null;
			if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.EventCounterBoundsTracer.TraceDebug(0L, "Transaction {0}: highestCommittedEventCounter={1} ({2} updated), eventCounterUpperBound={3} ({4} updated)", new object[]
				{
					committed ? "committed" : "rolled back",
					this.highestCommittedEventCounter,
					flag ? "was" : "was not",
					this.eventCounterUpperBound,
					flag2 ? "was" : "was not"
				});
			}
		}

		private long FindCleanupBoundary(Context context, TimeSpan retentionPeriod)
		{
			long num = this.eventCounterLowerBound;
			long @int = this.eventCounterUpperBound;
			StartStopKey startStopKey = new StartStopKey(false, new object[]
			{
				@int
			});
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, new Column[]
			{
				this.eventsTable.EventCounter
			}, null, null, 0, 1, new KeyRange(startStopKey, StartStopKey.Empty), true, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (!reader.Read())
					{
						return num;
					}
					@int = reader.GetInt64(this.eventsTable.EventCounter);
					startStopKey = new StartStopKey(false, new object[]
					{
						@int
					});
				}
			}
			DateTime value = DateTime.UtcNow.Subtract(retentionPeriod);
			Column[] columnsToFetch = new Column[]
			{
				this.eventsTable.CreateTime
			};
			long num2 = num;
			long num3 = @int - 1L;
			while (num2 <= num3)
			{
				long num4 = num2 + (num3 - num2 >> 1);
				int num5 = 1;
				StartStopKey startKey = new StartStopKey(true, new object[]
				{
					num4
				});
				using (TableOperator tableOperator2 = Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, new KeyRange(startKey, startStopKey), false, true))
				{
					using (Reader reader2 = tableOperator2.ExecuteReader(false))
					{
						if (reader2.Read())
						{
							num5 = reader2.GetDateTime(this.eventsTable.CreateTime).CompareTo(value);
						}
					}
				}
				if (num5 < 0)
				{
					num2 = num4 + 1L;
				}
				else
				{
					num3 = num4 - 1L;
				}
			}
			return num2;
		}

		internal bool CleanupEventHistory(Context context, TimeSpan retentionPeriod)
		{
			long num = this.FindCleanupBoundary(context, retentionPeriod);
			using (LockManager.Lock(this, LockManager.LockType.EventCounterBounds, context.Diagnostics))
			{
				if (this.eventCounterLowerBound < num)
				{
					this.eventCounterLowerBound = num;
					if (AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database))
					{
						GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(context.Database);
						GlobalsTableHelper.UpdateGlobalsTableRow(context, new Column[]
						{
							globalsTable.EventCounterLowerBound,
							globalsTable.EventCounterUpperBound
						}, new object[]
						{
							num,
							this.eventCounterUpperBound
						});
					}
				}
			}
			if (this.readerCount != 0)
			{
				return false;
			}
			if (num != 0L)
			{
				StartStopKey startKey = new StartStopKey(true, new object[]
				{
					0L
				});
				StartStopKey stopKey = new StartStopKey(false, new object[]
				{
					num
				});
				using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, this.eventsTable.Table, this.eventsTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startKey, stopKey), false, false), false))
				{
					deleteOperator.EnableInterrupts(new WriteChunkingInterruptControl(EventHistory.eventHistoryCleanupChunkSize.Value, null));
					int obj = (int)deleteOperator.ExecuteScalar();
					while (deleteOperator.Interrupted)
					{
						if (EventHistory.eventHistoryCleanupRowsDeletedTestHook.Value != null)
						{
							EventHistory.eventHistoryCleanupRowsDeletedTestHook.Value(obj);
						}
						context.Commit();
						if (MaintenanceHandler.ShouldStopDatabaseMaintenanceTask(context, EventHistory.EventHistoryCleanupMaintenanceId))
						{
							return false;
						}
						obj = (int)deleteOperator.ExecuteScalar();
					}
					if (EventHistory.eventHistoryCleanupRowsDeletedTestHook.Value != null)
					{
						EventHistory.eventHistoryCleanupRowsDeletedTestHook.Value(obj);
					}
					context.Commit();
				}
				return true;
			}
			return true;
		}

		internal void CleanupWatermarks(Context context)
		{
			using (LockManager.Lock(this.watermarkTableLockName, LockManager.LockType.WatermarkTableExclusive, context.Diagnostics))
			{
				SearchCriteriaCompare restriction = Factory.CreateSearchCriteriaCompare(this.watermarksTable.EventCounter, SearchCriteriaCompare.SearchRelOp.LessThan, Factory.CreateConstantColumn(this.eventCounterLowerBound));
				try
				{
					using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, this.watermarksTable.Table, this.watermarksTable.Table.PrimaryKeyIndex, null, restriction, null, 0, 0, KeyRange.AllRows, false, false), false))
					{
						deleteOperator.ExecuteScalar();
					}
					context.Commit();
				}
				finally
				{
					if (context.TransactionStarted)
					{
						context.Abort();
					}
				}
			}
		}

		private ErrorCode VerifyRestriction(Restriction restriction)
		{
			if (restriction is RestrictionProperty)
			{
				RestrictionProperty restrictionProperty = restriction as RestrictionProperty;
				if (restrictionProperty.PropertyTag != PropTag.Event.EventMailboxGuid || restrictionProperty.Operator != RelationOperator.Equal)
				{
					return ErrorCode.CreateTooComplex((LID)60440U);
				}
			}
			else if (restriction is RestrictionBitmask)
			{
				RestrictionBitmask restrictionBitmask = restriction as RestrictionBitmask;
				if (restrictionBitmask.PropertyTag != PropTag.Event.EventMask || restrictionBitmask.Operation == BitmaskOperation.EqualToZero)
				{
					return ErrorCode.CreateTooComplex((LID)35864U);
				}
			}
			else
			{
				if (!(restriction is RestrictionAND))
				{
					return ErrorCode.CreateTooComplex((LID)42008U);
				}
				RestrictionAND restrictionAND = restriction as RestrictionAND;
				if (restrictionAND.NestedRestrictions.Length != 2)
				{
					return ErrorCode.CreateTooComplex((LID)52248U);
				}
				bool flag = false;
				bool flag2 = false;
				foreach (Restriction restriction2 in restrictionAND.NestedRestrictions)
				{
					if (restriction2 is RestrictionProperty)
					{
						RestrictionProperty restrictionProperty2 = restriction2 as RestrictionProperty;
						if ((restrictionProperty2.PropertyTag != PropTag.Event.EventMailboxGuid && restrictionProperty2.PropertyTag != PropTag.Event.EventMask) || restrictionProperty2.Operator != RelationOperator.Equal)
						{
							return ErrorCode.CreateTooComplex((LID)46104U);
						}
						if (restrictionProperty2.PropertyTag == PropTag.Event.EventMailboxGuid)
						{
							if (flag2)
							{
								return ErrorCode.CreateTooComplex((LID)39960U);
							}
							flag2 = true;
						}
						else if (restrictionProperty2.PropertyTag == PropTag.Event.EventMask)
						{
							if (flag)
							{
								return ErrorCode.CreateTooComplex((LID)56344U);
							}
							flag = true;
						}
					}
					else
					{
						if (!(restriction2 is RestrictionBitmask))
						{
							return ErrorCode.CreateTooComplex((LID)54296U);
						}
						RestrictionBitmask restrictionBitmask2 = restriction2 as RestrictionBitmask;
						if (restrictionBitmask2.PropertyTag != PropTag.Event.EventMask || restrictionBitmask2.Operation == BitmaskOperation.EqualToZero)
						{
							return ErrorCode.CreateTooComplex((LID)62488U);
						}
						if (flag)
						{
							return ErrorCode.CreateTooComplex((LID)37912U);
						}
						flag = true;
					}
				}
			}
			return ErrorCode.NoError;
		}

		private SearchCriteria FixEventReadCriteria(Context context, SearchCriteria criteria)
		{
			criteria = criteria.InspectAndFix(delegate(SearchCriteria inspectCriteria, CompareInfo compareInfo)
			{
				SearchCriteriaCompare searchCriteriaCompare = inspectCriteria as SearchCriteriaCompare;
				if (searchCriteriaCompare != null)
				{
					PropertyColumn propertyColumn = searchCriteriaCompare.Lhs as PropertyColumn;
					if (propertyColumn != null && propertyColumn.StorePropTag == PropTag.Event.EventMailboxGuid)
					{
						if (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal || searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.NotEqual)
						{
							ConstantColumn constantColumn = searchCriteriaCompare.Rhs as ConstantColumn;
							if (constantColumn != null)
							{
								byte[] array = (byte[])constantColumn.Value;
								if (array.Length != 16)
								{
									throw new StoreException((LID)43000U, ErrorCodeValue.InvalidParameter, "invalid mailbox GUID value");
								}
								int num = 0;
								Guid guid = ParseSerialize.GetGuid(array, ref num, array.Length);
								int mailboxNumber;
								MailboxState mailboxState;
								if (MailboxStateCache.TryGetMailboxNumber(context, guid, false, out mailboxNumber) && (mailboxState = MailboxStateCache.Get(context, mailboxNumber)) != null && !mailboxState.IsDeleted)
								{
									return Factory.CreateSearchCriteriaCompare(this.eventsTable.MailboxNumber, searchCriteriaCompare.RelOp, Factory.CreateConstantColumn(mailboxState.MailboxNumber));
								}
								if (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal)
								{
									return Factory.CreateSearchCriteriaFalse();
								}
								return Factory.CreateSearchCriteriaTrue();
							}
						}
						throw new StoreException((LID)59384U, ErrorCodeValue.TooComplex, criteria.ToString());
					}
				}
				return inspectCriteria;
			}, null, true);
			criteria.EnumerateColumns(delegate(Column column, object state)
			{
				if (!(column is PhysicalColumn) && !(column is ConstantColumn) && (!(column is MappedPropertyColumn) || !(column.ActualColumn is PhysicalColumn)))
				{
					throw new StoreException((LID)34808U, ErrorCodeValue.TooComplex, state.ToString());
				}
			}, criteria);
			return criteria;
		}

		private static LockName<Guid> WatermarkTableLockName(Guid databaseGuid)
		{
			return new LockName<Guid>(databaseGuid, LockManager.LockLevel.WatermarkTable);
		}

		private static LockName<Guid, Guid> WatermarksConsumerLockName(Guid databaseGuid, Guid consumerGuid)
		{
			return new LockName<Guid, Guid>(databaseGuid, consumerGuid, LockManager.LockLevel.WatermarkConsumer);
		}

		private const int DefaultEventHistoryCleanupChunkSize = 1000;

		private const int NumEventsToReadPerTransaction = 128;

		public static readonly Guid EventHistoryCleanupMaintenanceId = new Guid("{9a0932ca-268a-4a60-b90e-fa9335a2f139}");

		private static IDatabaseMaintenance eventHistoryCleanupMaintenance;

		private static Hookable<int> eventHistoryCleanupChunkSize = Hookable<int>.Create(true, 1000);

		private static Hookable<Action<int>> eventHistoryCleanupRowsDeletedTestHook = Hookable<Action<int>>.Create(true, null);

		private static Action readerRaceTestHook = null;

		private static readonly Hookable<bool> simulateReadEventsFromPassiveTestHook = Hookable<bool>.Create(true, false);

		private static readonly Hookable<Action<long>> insertedEventHistoryRecordTestHook = Hookable<Action<long>>.Create(true, null);

		private static readonly Hookable<Action<int, EventType, long>> eventCounterAllocatedTestHook = Hookable<Action<int, EventType, long>>.Create(true, null);

		private readonly Column[] eventsFetchList;

		private readonly Column[] lastEventFetchList;

		private readonly Column[] watermarksFetchList;

		private StoreDatabase database;

		private Guid mdbVersionGuid;

		private long lastEventCounter;

		private long eventCounterLowerBound;

		private long eventCounterUpperBound;

		private long highestCommittedEventCounter;

		private EventHistory.FlushEventCounterUpperBoundTaskManager flushEventCounterUpperBoundTaskManager = new EventHistory.FlushEventCounterUpperBoundTaskManager();

		private bool eventCounterAllocationPotentiallyLost;

		private readonly LockName<Guid> watermarkTableLockName;

		private LinkedList<EventHistory.UncommittedListEntry> uncommittedList = new LinkedList<EventHistory.UncommittedListEntry>();

		private EventsTable eventsTable;

		private WatermarksTable watermarksTable;

		private int readerCount;

		private static int eventHistoryDataSlot = -1;

		[Flags]
		public enum ReadEventsFlags : uint
		{
			None = 0U,
			FailIfEventsDeleted = 1U,
			IncludeMoveDestinationEvents = 2U,
			EnableArbitraryRestriction = 2147483648U
		}

		private class UncommittedListEntry
		{
			public UncommittedListEntry(Context context, long firstCounter)
			{
				this.context = context;
				this.firstCounter = firstCounter;
				this.lastCounter = firstCounter;
			}

			public Context Context
			{
				get
				{
					return this.context;
				}
			}

			public long FirstCounter
			{
				get
				{
					return this.firstCounter;
				}
			}

			public long LastCounter
			{
				get
				{
					return this.lastCounter;
				}
				set
				{
					this.lastCounter = value;
				}
			}

			private readonly Context context;

			private readonly long firstCounter;

			private long lastCounter;
		}

		internal class FlushEventCounterUpperBoundTaskManager
		{
			internal bool IsFlushNeeded
			{
				get
				{
					return this.minimumEventCounterUpperBoundToFlush > 0L;
				}
			}

			private static void FlushEventCounterUpperBoundTask(Context context, EventHistory eventHistory, Func<bool> shouldTaskContinue)
			{
				using (context.AssociateWithDatabase(eventHistory.Database))
				{
					if (EventHistory.FlushEventCounterUpperBoundTaskManager.taskInvokedTestHook.Value != null)
					{
						EventHistory.FlushEventCounterUpperBoundTaskManager.taskInvokedTestHook.Value(context);
					}
					int num = 0;
					for (;;)
					{
						bool preemptFlush = !context.Database.IsOnlineActive || !shouldTaskContinue();
						num++;
						if (eventHistory.flushEventCounterUpperBoundTaskManager.TryFlushEventCounterUpperBound(context, eventHistory, num >= 10, preemptFlush))
						{
							break;
						}
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num <= 10, "Should not be possible to make more than the max attempts to flush the event counter upper bound.");
						Thread.Sleep(TimeSpan.FromMilliseconds(500.0));
					}
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<int>(0L, "The FlushEventCounterUpperBound task is terminating. It made {0} flush attempts.", num);
					}
					if (EventHistory.FlushEventCounterUpperBoundTaskManager.taskExecutedTestHook.Value != null)
					{
						EventHistory.FlushEventCounterUpperBoundTaskManager.taskExecutedTestHook.Value(num);
					}
				}
			}

			internal static IDisposable SetTaskInvokedTestHook(Action<Context> testDelegate)
			{
				return EventHistory.FlushEventCounterUpperBoundTaskManager.taskInvokedTestHook.SetTestHook(testDelegate);
			}

			internal static IDisposable SetTaskExecutedTestHook(Action<int> testDelegate)
			{
				return EventHistory.FlushEventCounterUpperBoundTaskManager.taskExecutedTestHook.SetTestHook(testDelegate);
			}

			public void HandlePotentiallyLostEventCounterAllocation(Context context, EventHistory eventHistory)
			{
				if (AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database))
				{
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "A potential event counter allocation leak was detected. Updating minimumEventCounterUpperBoundToFlush from {0} to {1}.", this.minimumEventCounterUpperBoundToFlush, eventHistory.LastEventCounter);
					}
					this.minimumEventCounterUpperBoundToFlush = eventHistory.LastEventCounter;
					return;
				}
				if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.EventCounterBoundsTracer.TraceDebug(0L, "A potential event counter allocation leak was detected after successfully allocating event counter {0}. However, the database has not been sufficiently upgraded to be able to flush the event counter upper bound.");
				}
			}

			public void EndTransactionHandler(Context context, EventHistory eventHistory, long lastCounter, bool committed)
			{
				if (AddEventCounterBoundsToGlobalsTable.IsReady(context, context.Database))
				{
					if (!committed)
					{
						if (lastCounter + 1L > this.minimumEventCounterUpperBoundToFlush)
						{
							this.minimumEventCounterUpperBoundToFlush = lastCounter + 1L;
							if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "Rollback performed. Updating minimumEventCounterUpperBoundToFlush from {0} to {1}.", this.minimumEventCounterUpperBoundToFlush, lastCounter + 1L);
							}
						}
						else if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "Rollback performed. Last counter was {0}, but minimumEventCounterUpperBoundToFlush is {1}, so it won't be updated.", lastCounter, this.minimumEventCounterUpperBoundToFlush);
						}
					}
					if (this.IsFlushNeeded)
					{
						this.CheckAndDispatchFlushEventCounterUpperBoundTask(eventHistory);
						return;
					}
				}
				else if (!committed && ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.EventCounterBoundsTracer.TraceDebug(0L, "A rollback was performed which will result in event counter gaps. However, the database has not been sufficiently upgraded to be able to flush the event counter upper bound.");
				}
			}

			private void CheckAndDispatchFlushEventCounterUpperBoundTask(EventHistory eventHistory)
			{
				bool flag;
				if (!this.isFlushTaskOutstanding)
				{
					flag = true;
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long>(0L, "Dispatching the FlushEventCounterUpperBound task (minimumEventCounterUpperBoundToFlush={0}).", this.minimumEventCounterUpperBoundToFlush);
					}
				}
				else if (DateTime.UtcNow - this.lastFlushTaskDispatched < TimeSpan.FromSeconds(100.0))
				{
					flag = false;
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<DateTime>(0L, "The FlushEventCounterUpperBound task will not be dispatched because it was already recently dispatched at {0}.", this.lastFlushTaskDispatched);
					}
				}
				else
				{
					this.numLostFlushTasks++;
					flag = true;
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<DateTime, long>(0L, "The FlushEventCounterUpperBound task dispatched at {0} has been outstanding for too long and is now considered 'lost'. Another task will be dispatched (minimumEventCounterUpperBoundToFlush={1}).", this.lastFlushTaskDispatched, this.minimumEventCounterUpperBoundToFlush);
					}
				}
				if (flag)
				{
					SingleExecutionTask<EventHistory>.CreateSingleExecutionTask(eventHistory.Database.TaskList, TaskExecutionWrapper<EventHistory>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.FlushEventCounterUpperBound, ClientType.System, eventHistory.Database.MdbGuid), new TaskExecutionWrapper<EventHistory>.TaskCallback<Context>(EventHistory.FlushEventCounterUpperBoundTaskManager.FlushEventCounterUpperBoundTask)), eventHistory, true);
					this.lastFlushTaskDispatched = DateTime.UtcNow;
					this.isFlushTaskOutstanding = true;
					this.numFlushTasksDispatched++;
				}
			}

			private bool TryFlushEventCounterUpperBound(Context context, EventHistory eventHistory, bool forceTaskCompletion, bool preemptFlush)
			{
				bool? flag = null;
				using (LockManager.Lock(eventHistory, LockManager.LockType.EventCounterBounds, context.Diagnostics))
				{
					if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long>(0L, "Executing the FlushEventCounterUpperBound task (minimumEventCounterUpperBoundToFlush={0}).", this.minimumEventCounterUpperBoundToFlush);
					}
					try
					{
						if (preemptFlush)
						{
							if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.EventCounterBoundsTracer.TraceDebug(0L, "The flush task was pre-empted.");
							}
							flag = new bool?(true);
						}
						else if (this.lastFlushedEventCounterUpperBound >= this.minimumEventCounterUpperBoundToFlush)
						{
							if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "The flush task skipped flushing because the last flushed event counter upper bound {0} already covers the minimum event counter upper bound to flush {1}.", this.lastFlushedEventCounterUpperBound, this.minimumEventCounterUpperBoundToFlush);
							}
							this.minimumEventCounterUpperBoundToFlush = 0L;
							flag = new bool?(true);
						}
						else
						{
							if (this.lastFlushedEventCounterUpperBound != eventHistory.EventCounterUpperBound)
							{
								Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.lastFlushedEventCounterUpperBound < eventHistory.EventCounterUpperBound, "Yikes! Event counter upper bound is moving backwards.");
								GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(context.Database);
								GlobalsTableHelper.UpdateGlobalsTableRow(context, new Column[]
								{
									globalsTable.EventCounterUpperBound
								}, new object[]
								{
									eventHistory.EventCounterUpperBound
								});
								this.lastFlushedEventCounterUpperBound = eventHistory.EventCounterUpperBound;
								if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "The flush task successfully flushed event counter upper bound {0} (minimumEventCounterUpperBoundToFlush={1}).", this.lastFlushedEventCounterUpperBound, this.minimumEventCounterUpperBoundToFlush);
								}
							}
							else if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long>(0L, "The flush task skipped flushing because the current event counter upper bound {0} is also the last flushed value.", this.lastFlushedEventCounterUpperBound);
							}
							if (this.minimumEventCounterUpperBoundToFlush <= this.lastFlushedEventCounterUpperBound)
							{
								if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long>(0L, "The event counter upper bound has been flushed up to or exceeding the minimum ({0}), so the flush task is being marked as completed.", this.minimumEventCounterUpperBoundToFlush);
								}
								this.minimumEventCounterUpperBoundToFlush = 0L;
								flag = new bool?(true);
							}
							else
							{
								if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long, string>(0L, "The last flushed event counter upper bound ({0}) was not sufficient to satisfy the minimum ({1}). {2}.", this.lastFlushedEventCounterUpperBound, this.minimumEventCounterUpperBoundToFlush, forceTaskCompletion ? "However, the task is being forced to complete" : "The task will retry flushing");
								}
								flag = new bool?(forceTaskCompletion);
							}
						}
					}
					finally
					{
						if (flag == null || flag == true)
						{
							this.lastFlushTaskCompleted = DateTime.UtcNow;
							this.isFlushTaskOutstanding = false;
							this.numFlushTasksCompleted++;
							if (flag == null)
							{
								if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.EventCounterBoundsTracer.TraceDebug<long, long>(0L, "The FlushEventCounterUpperBound task raised an exception and is terminating prematurely (minimumEventCounterUpperBoundToFlush={0}, lastFlushedEventCounterUpperBound={1}). ", this.minimumEventCounterUpperBoundToFlush, this.lastFlushedEventCounterUpperBound);
								}
								this.numFlushTaskFailures++;
							}
							if (ExTraceGlobals.EventCounterBoundsTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.EventCounterBoundsTracer.TraceDebug(0L, "FlushEventCounterUpperBound task summary: lastDispatched={0}, lastCompleted={1}, numDispatched={2}, numCompleted={3}, numLost={4}, numFailed={5}", new object[]
								{
									this.lastFlushTaskDispatched,
									this.lastFlushTaskCompleted,
									this.numFlushTasksDispatched,
									this.numFlushTasksCompleted,
									this.numLostFlushTasks,
									this.numFlushTaskFailures
								});
							}
						}
					}
				}
				return flag.Value;
			}

			internal const int MaxFlushAttemptsPerTaskDispatch = 10;

			private const int WaitTimeInMillisecondsBetweenFlushAttempts = 500;

			private const int LostFlushTaskThresholdInSeconds = 100;

			private static readonly Hookable<Action<Context>> taskInvokedTestHook = Hookable<Action<Context>>.Create(true, null);

			private static readonly Hookable<Action<int>> taskExecutedTestHook = Hookable<Action<int>>.Create(true, null);

			private long minimumEventCounterUpperBoundToFlush;

			private long lastFlushedEventCounterUpperBound;

			private bool isFlushTaskOutstanding;

			private DateTime lastFlushTaskDispatched;

			private DateTime lastFlushTaskCompleted;

			private int numFlushTasksDispatched;

			private int numFlushTasksCompleted;

			private int numLostFlushTasks;

			private int numFlushTaskFailures;
		}
	}
}
