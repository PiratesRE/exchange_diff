using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ReplidGuidMap : LockableMailboxComponent, IReplidGuidMap, IStateObject
	{
		public static bool IsReservedGuid(Guid guid)
		{
			return ReplidGuidMap.reservedGuids.Contains(guid);
		}

		public ReplidGuidMap(MailboxState mailboxState)
		{
			this.mailboxLockName = mailboxState;
		}

		private ReplidGuidMap(Context context, MailboxState mailboxState, Dictionary<ushort, Guid> replidGuidMap)
		{
			this.mailboxLockName = mailboxState;
			this.replidGuidMap = replidGuidMap;
			this.guidReplidMap = new Dictionary<Guid, ushort>(this.replidGuidMap.Count);
			foreach (KeyValuePair<ushort, Guid> keyValuePair in replidGuidMap)
			{
				if (this.guidReplidMap.ContainsKey(keyValuePair.Value))
				{
					throw new CorruptDataException((LID)54096U, "Duplicate Guid.");
				}
				this.AddGuidToDatabase(context, keyValuePair.Value, keyValuePair.Key);
				this.guidReplidMap.Add(keyValuePair.Value, keyValuePair.Key);
				if (!context.IsStateObjectRegistered(this))
				{
					context.RegisterStateObject(this);
				}
				this.maxReplid = Math.Max(this.maxReplid, keyValuePair.Key);
			}
			this.shouldReloadMapping = false;
		}

		public static void Initialize()
		{
			if (ReplidGuidMap.replidGuidMappingCacheDataSlot == -1)
			{
				ReplidGuidMap.replidGuidMappingCacheDataSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		public override MailboxComponentId MailboxComponentId
		{
			get
			{
				return ReplidGuidMap.ComponentId;
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

		public static bool IsReplidValid(ushort replid)
		{
			return replid > 0 && replid <= ReplidGuidMap.MaxReplidGuidNumber;
		}

		public static void CreateCacheForNewMailbox(Context context, MailboxState mailboxState, Dictionary<ushort, Guid> replidGuidMap)
		{
			ReplidGuidMap value = new ReplidGuidMap(context, mailboxState, replidGuidMap);
			mailboxState.SetComponentData(ReplidGuidMap.replidGuidMappingCacheDataSlot, value);
		}

		public static ReplidGuidMap GetCacheForMailbox(Context context, MailboxState mailboxState)
		{
			ReplidGuidMap replidGuidMap = (ReplidGuidMap)mailboxState.GetComponentData(ReplidGuidMap.replidGuidMappingCacheDataSlot);
			if (replidGuidMap == null)
			{
				replidGuidMap = new ReplidGuidMap(mailboxState);
				using (context.MailboxComponentReadOperation(replidGuidMap))
				{
					replidGuidMap.LoadReplidGuidMap(context);
				}
				ReplidGuidMap replidGuidMap2 = (ReplidGuidMap)mailboxState.CompareExchangeComponentData(ReplidGuidMap.replidGuidMappingCacheDataSlot, null, replidGuidMap);
				if (replidGuidMap2 != null)
				{
					replidGuidMap = replidGuidMap2;
				}
			}
			return replidGuidMap;
		}

		public static void DiscardCacheForMailbox(MailboxState mailboxState)
		{
			mailboxState.SetComponentData(ReplidGuidMap.replidGuidMappingCacheDataSlot, null);
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (table.Equals(DatabaseSchema.MailboxIdentityTable(context.Database).Table))
			{
				if (operationType == Connection.OperationType.Update)
				{
					return this.TestExclusiveLock();
				}
			}
			else if (table.Equals(DatabaseSchema.ReplidGuidMapTable(context.Database).Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
				if (operationType == Connection.OperationType.Insert)
				{
					return this.TestExclusiveLock();
				}
			}
			return false;
		}

		public int GetReplidCount(Context context)
		{
			int result;
			using (context.MailboxComponentReadOperation(this))
			{
				this.LoadReplidGuidMap(context);
				result = (int)this.maxReplid;
			}
			return result;
		}

		public Guid InternalGetGuidFromReplid(Context context, ushort replid)
		{
			if (replid == 0)
			{
				return Guid.Empty;
			}
			Dictionary<ushort, Guid> dictionary = this.replidGuidMap;
			if (dictionary == null)
			{
				using (context.MailboxComponentReadOperation(this))
				{
					this.LoadReplidGuidMap(context);
					dictionary = this.replidGuidMap;
				}
			}
			Guid result;
			if (!dictionary.TryGetValue(replid, out result))
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "We are trying to map non-existing replid");
				return Guid.Empty;
			}
			return result;
		}

		public Guid GetGuidFromReplid(Context context, ushort replid)
		{
			if (replid == 0)
			{
				return Guid.Empty;
			}
			Guid result;
			using (context.MailboxComponentReadOperation(this))
			{
				this.LoadReplidGuidMap(context);
				Guid guid;
				if (!this.replidGuidMap.TryGetValue(replid, out guid))
				{
					throw new ReplidNotFoundException((LID)45176U, replid);
				}
				result = guid;
			}
			return result;
		}

		public ushort GetReplidFromGuid(Context context, Guid guid)
		{
			if (guid == Guid.Empty)
			{
				return 0;
			}
			ushort result;
			using (context.MailboxComponentReadOperation(this))
			{
				this.LoadReplidGuidMap(context);
				if (this.guidReplidMap.TryGetValue(guid, out result))
				{
					return result;
				}
			}
			if (context.Database.IsReadOnly)
			{
				DiagnosticContext.TraceDword((LID)53756U, (uint)context.ClientType);
				throw new CannotRegisterNewReplidGuidMapping((LID)41468U, "Cannot register new replid-guid mapping because the database is read-only.");
			}
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame2 = context.MailboxComponentWriteOperation(this))
			{
				this.LoadReplidGuidMap(context);
				if (!this.guidReplidMap.TryGetValue(guid, out result))
				{
					result = this.RegisterReplIdGuid(context, guid);
				}
				mailboxComponentOperationFrame2.Success();
			}
			return result;
		}

		internal bool IsGuidInMap(Guid guid)
		{
			return this.guidReplidMap.ContainsKey(guid);
		}

		internal bool IsGuidInMap(Context context, Guid guid)
		{
			bool result = false;
			using (context.MailboxComponentReadOperation(this))
			{
				this.LoadReplidGuidMap(context);
				result = this.guidReplidMap.ContainsKey(guid);
			}
			return result;
		}

		internal Guid RegisterNewLocalIdGuid(Context context, Mailbox mailbox, Guid newLocalIdGuid)
		{
			if (!context.IsStateObjectRegistered(this))
			{
				context.RegisterStateObject(this);
			}
			Guid result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this))
			{
				bool flag = false;
				context.PushConnection();
				try
				{
					if (newLocalIdGuid.Equals(Guid.Empty))
					{
						do
						{
							newLocalIdGuid = Guid.NewGuid();
						}
						while (this.IsGuidInMap(newLocalIdGuid) || ReplidGuidMap.IsReservedGuid(newLocalIdGuid));
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.IsGuidInMap(newLocalIdGuid) && !ReplidGuidMap.IsReservedGuid(newLocalIdGuid), "New local Id Guid already in use.");
					this.RegisterReplIdGuid(context, newLocalIdGuid);
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, mailbox.MailboxIdentityTable.Table, true, new ColumnValue[]
					{
						new ColumnValue(mailbox.MailboxIdentityTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
					}))
					{
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.LocalIdGuid, newLocalIdGuid);
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.IdCounter, 1L);
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.CnCounter, 1L);
						dataRow.SetValue(context, mailbox.MailboxIdentityTable.LastCounterPatchingTime, mailbox.SharedState.UtcNow);
						dataRow.Flush(context);
					}
					context.Commit();
					flag = true;
					mailboxComponentOperationFrame.Success();
					result = newLocalIdGuid;
				}
				finally
				{
					if (!flag)
					{
						context.Abort();
						((IStateObject)this).OnAbort(context);
					}
					context.PopConnection();
				}
			}
			return result;
		}

		public void ForEachElement(ReplidGuidMap.Callback callback)
		{
			foreach (KeyValuePair<ushort, Guid> replidGuidPair in this.replidGuidMap)
			{
				callback(replidGuidPair);
			}
		}

		public void RegisterReservedGuidValues(Context context)
		{
			foreach (Guid guid in ReplidGuidMap.reservedGuids)
			{
				this.GetReplidFromGuid(context, guid);
			}
		}

		public void Process(Context context, Dictionary<ushort, Guid> specifiedReplidGuidMap)
		{
			ushort num = 0;
			uint num2;
			using (context.MailboxComponentReadOperation(this))
			{
				this.ValidateReplidGuidMappingAgainstSpecifiedReplidGuidMapping(context, specifiedReplidGuidMap, out num2);
				if (num2 > 0U)
				{
					num = this.maxReplid;
				}
			}
			if (num2 == 0U)
			{
				return;
			}
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame2 = context.MailboxComponentWriteOperation(this))
			{
				for (uint num3 = 0U; num3 < num2; num3 += 1U)
				{
					num += 1;
					Guid guid;
					if (this.replidGuidMap.TryGetValue(num, out guid))
					{
						uint num4;
						this.ValidateReplidGuidMappingAgainstSpecifiedReplidGuidMapping(context, specifiedReplidGuidMap, out num4);
						num2 = num4;
						break;
					}
				}
				int num5 = 0;
				while ((long)num5 < (long)((ulong)num2))
				{
					num = (this.maxReplid += 1);
					Guid guid2;
					if (specifiedReplidGuidMap.TryGetValue(num, out guid2))
					{
						this.AddGuidToDatabase(context, guid2, num);
						if (!context.IsStateObjectRegistered(this))
						{
							context.RegisterStateObject(this);
						}
						this.replidGuidMap.Add(num, guid2);
						this.guidReplidMap.Add(guid2, num);
					}
					else
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "This cannot be happening!");
					}
					num5++;
				}
				mailboxComponentOperationFrame2.Success();
			}
		}

		public byte[] GetSerializedMap(Context context)
		{
			byte[] result = null;
			using (context.MailboxComponentReadOperation(this))
			{
				this.LoadReplidGuidMap(context);
				ushort num = Math.Min(this.maxReplid, ReplidGuidMap.MaxMappingsToPrefetch);
				if (num > 0)
				{
					byte[] array = new byte[(int)(num * 18)];
					int num2 = 0;
					for (ushort num3 = 1; num3 <= num; num3 += 1)
					{
						Guid value;
						if (this.replidGuidMap.TryGetValue(num3, out value))
						{
							ExBitConverter.Write(num3, array, num2);
							num2 += 2;
							ExBitConverter.Write(value, array, num2);
							num2 += 16;
						}
					}
					result = array;
				}
			}
			return result;
		}

		private void ValidateReplidGuidMappingAgainstSpecifiedReplidGuidMapping(Context context, Dictionary<ushort, Guid> specifiedReplidGuidMap, out uint newReplidGuidCounter)
		{
			newReplidGuidCounter = 0U;
			foreach (KeyValuePair<ushort, Guid> keyValuePair in specifiedReplidGuidMap)
			{
				bool flag = false;
				bool flag2 = false;
				Guid guid;
				ushort num;
				if (this.replidGuidMap.TryGetValue(keyValuePair.Key, out guid))
				{
					flag2 = true;
					if (!guid.Equals(keyValuePair.Value))
					{
						flag = true;
					}
				}
				else if (this.guidReplidMap.TryGetValue(keyValuePair.Value, out num))
				{
					flag2 = true;
					flag = true;
				}
				if (!flag2)
				{
					newReplidGuidCounter += 1U;
				}
				if (flag)
				{
					string friendlyNameForLogging = this.mailboxLockName.GetFriendlyNameForLogging();
					string message = string.Format("On database {0} the signature of the mailbox {1} was changed by the new long term ID GUID: {2}", context.Database.MdbName, friendlyNameForLogging, keyValuePair.Value);
					if (ExTraceGlobals.MailboxSignatureTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.MailboxSignatureTracer.TraceError(64696L, message);
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_RepidGuidMappingChange, new object[]
					{
						context.Database.MdbName,
						friendlyNameForLogging,
						keyValuePair.Value
					});
					throw new CannotRegisterNewReplidGuidMapping((LID)40120U, message);
				}
			}
		}

		private ushort GetNextReplid(Context context)
		{
			if (this.maxReplid < ReplidGuidMap.MaxReplidGuidNumber)
			{
				if (this.maxReplid == ReplidGuidMap.ReplidExhaustionWarningThreshold - 1)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxReplidExhaustionApproaching, new object[]
					{
						this.mailboxLockName.GetFriendlyNameForLogging()
					});
				}
				return this.maxReplid += 1;
			}
			ExTraceGlobals.ContextTracer.TraceDebug<int>(0L, "Exceeded max replid count for mailbox partition {0}", this.mailboxLockName.MailboxPartitionNumber);
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxReplidExhaustion, new object[]
			{
				this.mailboxLockName.GetFriendlyNameForLogging()
			});
			throw new ParameterOverflow((LID)61560U, "Replid exceeds the maximum allowed");
		}

		private ushort RegisterReplIdGuid(Context context, Guid guid)
		{
			ushort nextReplid = this.GetNextReplid(context);
			this.AddGuidInternal(context, guid, nextReplid);
			return nextReplid;
		}

		private void AddGuidToDatabase(Context context, Guid guid, ushort replid)
		{
			ReplidGuidMapTable replidGuidMapTable = DatabaseSchema.ReplidGuidMapTable(context.Database);
			using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, replidGuidMapTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(replidGuidMapTable.MailboxPartitionNumber, this.mailboxLockName.MailboxPartitionNumber),
				new ColumnValue(replidGuidMapTable.Guid, guid),
				new ColumnValue(replidGuidMapTable.Replid, (short)replid)
			}))
			{
				dataRow.Flush(context);
			}
		}

		private void AddGuidInternal(Context context, Guid guid, ushort replid)
		{
			DatabaseSchema.ReplidGuidMapTable(context.Database);
			this.AddGuidToDatabase(context, guid, replid);
			using (LockManager.Lock(this, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				Dictionary<Guid, ushort> dictionary = new Dictionary<Guid, ushort>(this.guidReplidMap);
				Dictionary<ushort, Guid> dictionary2 = new Dictionary<ushort, Guid>(this.replidGuidMap);
				dictionary.Add(guid, replid);
				dictionary2.Add(replid, guid);
				this.guidReplidMap = dictionary;
				this.replidGuidMap = dictionary2;
			}
			if (!context.IsStateObjectRegistered(this))
			{
				context.RegisterStateObject(this);
			}
		}

		private void LoadReplidGuidMap(Context context)
		{
			if (!this.shouldReloadMapping)
			{
				return;
			}
			Dictionary<Guid, ushort> dictionary = new Dictionary<Guid, ushort>();
			Dictionary<ushort, Guid> dictionary2 = new Dictionary<ushort, Guid>();
			ushort val = 0;
			ushort num = ReplidGuidMap.MaxReplidGuidNumber;
			uint num2 = 0U;
			ushort num3 = 0;
			ReplidGuidMapTable replidGuidMapTable = DatabaseSchema.ReplidGuidMapTable(context.Database);
			Column[] columnsToFetch = new Column[]
			{
				replidGuidMapTable.Guid,
				replidGuidMapTable.Replid
			};
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				this.mailboxLockName.MailboxPartitionNumber
			});
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, replidGuidMapTable.Table, replidGuidMapTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						Guid guid = reader.GetGuid(replidGuidMapTable.Guid);
						ushort num4 = (ushort)reader.GetInt16(replidGuidMapTable.Replid);
						if (num4 > ReplidGuidMap.MaxReplidGuidNumber)
						{
							throw new CorruptDataException((LID)41808U, "Invalid replid value.");
						}
						if (dictionary.ContainsKey(guid))
						{
							throw new CorruptDataException((LID)57465U, "Duplicate Guid.");
						}
						if (dictionary2.ContainsKey(num4))
						{
							throw new CorruptDataException((LID)58192U, "Duplicate replid.");
						}
						dictionary.Add(guid, num4);
						dictionary2.Add(num4, guid);
						val = Math.Max(val, num4);
						num = Math.Min(num, num4);
						num2 += (uint)num4;
						num3 += 1;
					}
				}
			}
			if (num3 > 0 && num != 1)
			{
				throw new CorruptDataException((LID)33616U, "Invalid minimum replid.");
			}
			if ((ulong)num2 != (ulong)((long)(num3 * (num3 + 1) / 2)))
			{
				throw new CorruptDataException((LID)50000U, "One of more replid values are invalid.");
			}
			using (LockManager.Lock(this, context.Diagnostics))
			{
				if (this.guidReplidMap == null)
				{
					this.maxReplid = val;
					this.guidReplidMap = dictionary;
					this.replidGuidMap = dictionary2;
					this.shouldReloadMapping = false;
				}
			}
		}

		private void ResetInMemoryCache()
		{
			this.guidReplidMap = null;
			this.replidGuidMap = null;
			this.maxReplid = 0;
			this.shouldReloadMapping = true;
		}

		void IStateObject.OnBeforeCommit(Context context)
		{
		}

		void IStateObject.OnCommit(Context context)
		{
		}

		void IStateObject.OnAbort(Context context)
		{
			this.ResetInMemoryCache();
		}

		private static readonly MailboxComponentId ComponentId = MailboxComponentId.ReplidGuidMap;

		public static readonly Guid ReservedGuidForCategorizedViews = new Guid("ed33cbe5-94e2-48b6-8bea-bba984896933");

		public static readonly Guid ReservedPerUserIndicatorGuid = new Guid("68349a54-323d-4a38-9aa9-e00a683131ba");

		public static readonly Guid ReservedNonPerUserIndicatorGuid = new Guid("bb0754de-7f26-4d08-932f-fe7a9d22f8bd");

		private static readonly HashSet<Guid> reservedGuids = new HashSet<Guid>(new Guid[]
		{
			ReplidGuidMap.ReservedGuidForCategorizedViews,
			ReplidGuidMap.ReservedPerUserIndicatorGuid,
			ReplidGuidMap.ReservedNonPerUserIndicatorGuid
		});

		private static readonly ushort MaxMappingsToPrefetch = 512;

		public static ushort MaxReplidGuidNumber = 32767;

		public static ushort ReplidExhaustionWarningThreshold = 30000;

		private static int replidGuidMappingCacheDataSlot = -1;

		private MailboxLockNameBase mailboxLockName;

		private Dictionary<Guid, ushort> guidReplidMap;

		private Dictionary<ushort, Guid> replidGuidMap;

		private ushort maxReplid;

		private bool shouldReloadMapping = true;

		public delegate void Callback(KeyValuePair<ushort, Guid> replidGuidPair);
	}
}
