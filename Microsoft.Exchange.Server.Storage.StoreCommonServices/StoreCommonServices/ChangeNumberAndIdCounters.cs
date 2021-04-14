using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ChangeNumberAndIdCounters : LockableMailboxComponent
	{
		public ChangeNumberAndIdCounters(MailboxState mailboxState)
		{
			this.mailboxLockName = mailboxState;
		}

		public override LockManager.LockType ReaderLockType
		{
			get
			{
				return LockManager.LockType.ChangeNumberAndIdCountersShared;
			}
		}

		public override LockManager.LockType WriterLockType
		{
			get
			{
				return LockManager.LockType.ChangeNumberAndIdCountersExclusive;
			}
		}

		public override MailboxComponentId MailboxComponentId
		{
			get
			{
				return ChangeNumberAndIdCounters.ComponentId;
			}
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.mailboxLockName.DatabaseGuid;
			}
		}

		public override int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxLockName.MailboxPartitionNumber;
			}
		}

		public override bool Committable
		{
			get
			{
				return false;
			}
		}

		public static void Initialize()
		{
			if (ChangeNumberAndIdCounters.changeNumberAndIdCountersCacheSlot == -1)
			{
				ChangeNumberAndIdCounters.changeNumberAndIdCountersCacheSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		public static ChangeNumberAndIdCounters GetCacheForMailbox(Mailbox mailbox)
		{
			ChangeNumberAndIdCounters changeNumberAndIdCounters = (ChangeNumberAndIdCounters)mailbox.SharedState.GetComponentData(ChangeNumberAndIdCounters.changeNumberAndIdCountersCacheSlot);
			if (changeNumberAndIdCounters == null)
			{
				changeNumberAndIdCounters = new ChangeNumberAndIdCounters(mailbox.SharedState);
				ChangeNumberAndIdCounters changeNumberAndIdCounters2 = (ChangeNumberAndIdCounters)mailbox.SharedState.CompareExchangeComponentData(ChangeNumberAndIdCounters.changeNumberAndIdCountersCacheSlot, null, changeNumberAndIdCounters);
				if (changeNumberAndIdCounters2 != null)
				{
					changeNumberAndIdCounters = changeNumberAndIdCounters2;
				}
			}
			return changeNumberAndIdCounters;
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (table.Equals(DatabaseSchema.MailboxIdentityTable(context.Database).Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
				if (operationType == Connection.OperationType.Update)
				{
					return this.TestExclusiveLock();
				}
			}
			return false;
		}

		public void InitializeCounterCaches(Context context, Mailbox mailbox)
		{
			this.AllocateChangeNumberCounterRange(context, mailbox, 0U, false);
			this.AllocateObjectIdCounterRange(context, mailbox, 0U, false);
		}

		public Guid GetLocalIdGuid(Context context, Mailbox mailbox)
		{
			Guid result;
			using (context.MailboxComponentReadOperation(this))
			{
				if (this.localIdGuid == Guid.Empty)
				{
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
					{
						new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
					}))
					{
						this.localIdGuid = (Guid)dataRow.GetValue(context, mailbox.MailboxIdentityTable.LocalIdGuid);
					}
				}
				result = this.localIdGuid;
			}
			return result;
		}

		public void GetGlobalCounters(Context context, Mailbox mailbox, out ulong idCounter, out ulong cnCounter)
		{
			using (context.MailboxComponentReadOperation(this))
			{
				ulong num;
				ulong num2;
				using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
				{
					new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
				}))
				{
					num = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.IdCounter));
					num2 = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.CnCounter));
				}
				idCounter = num;
				cnCounter = num2;
			}
		}

		public ulong AllocateObjectIdCounter(Context context, Mailbox mailbox)
		{
			return this.AllocateObjectIdCounterRange(context, mailbox, 1U, true);
		}

		public ulong AllocateChangeNumberCounter(Context context, Mailbox mailbox)
		{
			return this.AllocateChangeNumberCounterRange(context, mailbox, 1U, true);
		}

		public ulong GetLastChangeNumber(Context context, Mailbox mailbox)
		{
			return this.AllocateChangeNumberCounterRange(context, mailbox, 0U, true);
		}

		public ulong GetNextIdCounterAndReserveRange(Context context, Mailbox mailbox, uint reservedRange)
		{
			return this.AllocateObjectIdCounterRange(context, mailbox, reservedRange, true);
		}

		public ulong GetNextIdCounterAndReserveRange(Context context, Mailbox mailbox, uint reservedRange, bool separateTransaction)
		{
			return this.AllocateObjectIdCounterRange(context, mailbox, reservedRange, separateTransaction);
		}

		public ulong GetNextCnCounterAndReserveRange(Context context, Mailbox mailbox, uint reservedRange)
		{
			return this.AllocateChangeNumberCounterRange(context, mailbox, reservedRange, true);
		}

		public void SetGlobalCounters(Context context, Mailbox mailbox, ulong newIdCounter, ulong newCnCounter)
		{
			this.SetGlobalCounters(context, mailbox, newIdCounter, newCnCounter, null);
		}

		public void SetGlobalCounters(Context context, Mailbox mailbox, ulong newIdCounter, ulong newCnCounter, Guid? newLocalIdGuid)
		{
			bool flag = false;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
			{
				context.PushConnection();
				try
				{
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
					{
						new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
					}))
					{
						if (newLocalIdGuid != null)
						{
							Guid guid = (Guid)dataRow.GetValue(context, mailbox.MailboxIdentityTable.LocalIdGuid);
							dataRow.SetValue(context, mailbox.MailboxIdentityTable.LocalIdGuid, newLocalIdGuid.Value);
						}
						else
						{
							ulong num = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.IdCounter));
							ulong num2 = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.CnCounter));
							if (num > newIdCounter)
							{
								throw new CorruptDataException((LID)62536U, "New ID counter out of range.");
							}
							if (num2 > newCnCounter)
							{
								throw new CorruptDataException((LID)37960U, "New CN counter out of range.");
							}
						}
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.IdCounter, (long)newIdCounter);
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.CnCounter, (long)newCnCounter);
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.LastCounterPatchingTime, mailbox.SharedState.UtcNow);
						dataRow.Flush(context);
					}
					context.Commit();
					flag = true;
					mailboxComponentOperationFrame.Success();
					mailbox.SharedState.GlobalIdLowWatermark = newIdCounter;
					mailbox.SharedState.GlobalCnLowWatermark = newCnCounter;
					this.cnAllocationCache = null;
					this.idAllocationCache = null;
				}
				finally
				{
					if (!flag)
					{
						context.Abort();
					}
					context.PopConnection();
				}
			}
		}

		public void UpdateMailboxGlobalIDs(Context context, Mailbox mailbox, StoreDatabase database, bool separateTransaction)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
			{
				if (!mailbox.SharedState.CountersAlreadyPatched && !mailbox.SharedState.IsRemoved)
				{
					if (mailbox.GetMRSPreservingMailboxSignature(context) || mailbox.GetPreservingMailboxSignature(context))
					{
						mailbox.SharedState.CountersAlreadyPatched = true;
					}
					else
					{
						bool flag = false;
						if (separateTransaction)
						{
							context.PushConnection();
						}
						try
						{
							ulong globalIdLowWatermark;
							ulong globalCnLowWatermark;
							using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
							{
								new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
							}))
							{
								ulong num = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.IdCounter));
								ulong num2 = (ulong)((long)dataRow.GetValue(context, mailbox.MailboxIdentityTable.CnCounter));
								if (database.IsReadOnly)
								{
									globalIdLowWatermark = num;
									globalCnLowWatermark = num2;
								}
								else
								{
									DateTime utcNow = mailbox.SharedState.UtcNow;
									DateTime dateTime = (DateTime)dataRow.GetValue(context, mailbox.MailboxIdentityTable.LastCounterPatchingTime);
									uint num3;
									if (utcNow > dateTime)
									{
										num3 = (uint)(utcNow - dateTime).TotalSeconds;
									}
									else
									{
										num3 = 0U;
									}
									ulong num4 = (ulong)Math.Max(num3 * 128U, 3840U);
									long num5;
									long num6;
									if (num < 281474976645120UL - num4 && num2 < 281474976645120UL - num4)
									{
										num5 = (long)(num + num4);
										num6 = (long)(num2 + num4);
									}
									else
									{
										num5 = 281474976645120L;
										num6 = 281474976645120L;
									}
									dataRow.SetValue(context, mailbox.MailboxIdentityTable.IdCounter, num5);
									dataRow.SetValue(context, mailbox.MailboxIdentityTable.CnCounter, num6);
									dataRow.SetValue(context, mailbox.MailboxIdentityTable.LastCounterPatchingTime, utcNow);
									dataRow.Flush(context);
									globalIdLowWatermark = (ulong)num5;
									globalCnLowWatermark = (ulong)num6;
								}
							}
							if (separateTransaction)
							{
								context.Commit();
								flag = true;
							}
							mailbox.SharedState.CountersAlreadyPatched = true;
							mailbox.SharedState.GlobalIdLowWatermark = globalIdLowWatermark;
							mailbox.SharedState.GlobalCnLowWatermark = globalCnLowWatermark;
							mailboxComponentOperationFrame.Success();
						}
						finally
						{
							if (separateTransaction)
							{
								if (!flag)
								{
									context.Abort();
								}
								context.PopConnection();
							}
						}
					}
				}
			}
		}

		private void AllocateCounterRange(Context context, Mailbox mailbox, uint rangeSize, bool separateTransaction, StorePropTag propTagCounterRangeUpperLimit, PhysicalColumn nextUnusedCounterPhysicalColumn, ref GlobcntAllocationCache allocationCache, out bool needNewGlobCountSet, out ulong globCount)
		{
			needNewGlobCountSet = false;
			globCount = 0UL;
			uint num = (allocationCache == null) ? 0U : allocationCache.CountAvailable;
			if (allocationCache == null || num < rangeSize)
			{
				bool flag = false;
				ulong num2 = 0UL;
				bool preservingMailboxSignature = mailbox.GetPreservingMailboxSignature(context);
				bool mrspreservingMailboxSignature = mailbox.GetMRSPreservingMailboxSignature(context);
				if (ExTraceGlobals.MailboxCountersTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxCountersTracer.TraceDebug(59680L, "Request counter range for mailbox {0} : {1} : Store preserving mailbox signature {2} : MRS preserving mailbox signature {3} : counter {4} : available range {5}, requested range {6} : stack {7}", new object[]
					{
						mailbox.GetDisplayName(context),
						mailbox.MailboxGuid,
						preservingMailboxSignature,
						mrspreservingMailboxSignature,
						nextUnusedCounterPhysicalColumn,
						num,
						rangeSize,
						new StackTrace()
					});
				}
				if (!preservingMailboxSignature && mrspreservingMailboxSignature)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_InvalidMailboxGlobcntAllocation, new object[]
					{
						mailbox.MailboxGuid,
						mailbox.Database.MdbGuid,
						context.Diagnostics.OpSource,
						context.Diagnostics.OpNumber,
						new StackTrace()
					});
					throw new StoreException((LID)43296U, ErrorCodeValue.NotSupported, "Invalid counter range allocation at this time.");
				}
				if (preservingMailboxSignature)
				{
					object propertyValue = mailbox.GetPropertyValue(context, propTagCounterRangeUpperLimit);
					num2 = (ulong)((long)propertyValue);
				}
				if (separateTransaction)
				{
					context.PushConnection();
				}
				try
				{
					uint num3 = rangeSize - num;
					ulong num4 = (ulong)Math.Max(num3, 500U);
					ulong nextUnallocated;
					ulong num7;
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
					{
						new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
					}))
					{
						ulong num5 = (ulong)((long)dataRow.GetValue(context, nextUnusedCounterPhysicalColumn));
						if (allocationCache == null)
						{
							nextUnallocated = num5;
						}
						else
						{
							nextUnallocated = allocationCache.Allocate(0U);
						}
						if (preservingMailboxSignature)
						{
							ulong num6 = num2 - num5;
							if (num6 <= (ulong)num3)
							{
								if (ExTraceGlobals.MailboxCountersTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.MailboxCountersTracer.TraceDebug(35104L, "Failed with GlobalCounterRangeExceeded the request for counter range for mailbox {0} : {1} : Store preserving mailbox signature {2} : MRS preserving mailbox signature {3} : counter {4} : available range {5}, requested range {6} : stack {7}", new object[]
									{
										mailbox.GetDisplayName(context),
										mailbox.MailboxGuid,
										preservingMailboxSignature,
										mrspreservingMailboxSignature,
										nextUnusedCounterPhysicalColumn,
										num,
										rangeSize,
										new StackTrace()
									});
								}
								throw new StoreException((LID)35912U, ErrorCodeValue.GlobalCounterRangeExceeded, (PropTag.Mailbox.ReservedIdCounterRangeUpperLimit == propTagCounterRangeUpperLimit) ? "Out of IDs." : "Out of CNs.");
							}
							num4 = Math.Min(num6, num4);
						}
						num7 = num5 + num4;
						if (num7 > 281474976645120UL)
						{
							needNewGlobCountSet = true;
							return;
						}
						dataRow.SetValue(context, nextUnusedCounterPhysicalColumn, (long)num7);
						dataRow.Flush(context);
						if (ExTraceGlobals.MailboxCountersTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MailboxCountersTracer.TraceDebug<Guid, PhysicalColumn, ulong>(0L, "Updated counter for mailbox {0} : counter {1} : updated value {2}", mailbox.MailboxGuid, nextUnusedCounterPhysicalColumn, num7);
						}
					}
					if (separateTransaction)
					{
						context.Commit();
						flag = true;
					}
					if (allocationCache == null)
					{
						allocationCache = new GlobcntAllocationCache(nextUnallocated, num7);
					}
					else
					{
						allocationCache.SetMax(num7);
					}
				}
				finally
				{
					if (separateTransaction)
					{
						if (!flag)
						{
							context.Abort();
						}
						context.PopConnection();
					}
				}
			}
			needNewGlobCountSet = false;
			globCount = allocationCache.Allocate(rangeSize);
		}

		private ulong AllocateCounterRange(Context context, Mailbox mailbox, uint rangeSize, bool separateTransaction, StorePropTag propTagCounterRangeUpperLimit, PhysicalColumn nextUnusedCounterPhysicalColumn, ref GlobcntAllocationCache allocationCache)
		{
			ulong result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
			{
				bool flag;
				ulong num;
				this.AllocateCounterRange(context, mailbox, rangeSize, separateTransaction, propTagCounterRangeUpperLimit, nextUnusedCounterPhysicalColumn, ref allocationCache, out flag, out num);
				if (flag)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxGlobcntRolledOver, new object[]
					{
						mailbox.MailboxGuid
					});
					this.GetNewGlobCountSets(context, mailbox);
					this.AllocateCounterRange(context, mailbox, rangeSize, separateTransaction, propTagCounterRangeUpperLimit, nextUnusedCounterPhysicalColumn, ref allocationCache, out flag, out num);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!flag, "Cannot exhaust the new global counter set in one allocation.");
				}
				mailboxComponentOperationFrame.Success();
				result = num;
			}
			return result;
		}

		private ulong AllocateObjectIdCounterRange(Context context, Mailbox mailbox, uint rangeSize, bool separateTransaction)
		{
			return this.AllocateCounterRange(context, mailbox, rangeSize, separateTransaction, PropTag.Mailbox.ReservedIdCounterRangeUpperLimit, mailbox.MailboxIdentityTable.IdCounter, ref this.idAllocationCache);
		}

		private ulong AllocateChangeNumberCounterRange(Context context, Mailbox mailbox, uint rangeSize, bool separateTransaction)
		{
			return this.AllocateCounterRange(context, mailbox, rangeSize, separateTransaction, PropTag.Mailbox.ReservedCnCounterRangeUpperLimit, mailbox.MailboxIdentityTable.CnCounter, ref this.cnAllocationCache);
		}

		private void GetNewGlobCountSets(Context context, Mailbox mailbox)
		{
			ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, mailbox.SharedState);
			Guid guid = cacheForMailbox.RegisterNewLocalIdGuid(context, mailbox, Guid.Empty);
			this.idAllocationCache = null;
			this.cnAllocationCache = null;
			this.localIdGuid = guid;
		}

		internal const uint DefaultIdReserveChunk = 500U;

		private static readonly MailboxComponentId ComponentId = MailboxComponentId.ChangeNumberAndIdCounters;

		private static int changeNumberAndIdCountersCacheSlot = -1;

		private MailboxLockNameBase mailboxLockName;

		private GlobcntAllocationCache idAllocationCache;

		private GlobcntAllocationCache cnAllocationCache;

		private Guid localIdGuid = Guid.Empty;
	}
}
