using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class PhysicalIndexCache
	{
		private PhysicalIndexCache(StoreDatabase database)
		{
			this.cache = new Dictionary<int, PhysicalIndex>(255);
			this.lockName = new LockName<Guid>(database.MdbGuid, LockManager.LockLevel.PhysicalIndexCache);
		}

		public static void Initialize()
		{
			if (PhysicalIndexCache.physicalIndexCacheDataSlot == -1)
			{
				PhysicalIndexCache.physicalIndexCacheDataSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		public static void MountEventHandler(Context context)
		{
			PhysicalIndexCache physicalIndexCache = new PhysicalIndexCache(context.Database);
			context.Database.ComponentData[PhysicalIndexCache.physicalIndexCacheDataSlot] = physicalIndexCache;
			List<int> listOfPhysicalIndexesFromDatabase = PhysicalIndexCache.GetListOfPhysicalIndexesFromDatabase(context);
			for (int i = 0; i < listOfPhysicalIndexesFromDatabase.Count; i++)
			{
				PhysicalIndex physicalIndex = PhysicalIndex.GetPhysicalIndex(context, listOfPhysicalIndexesFromDatabase[i]);
				physicalIndexCache.cache[listOfPhysicalIndexesFromDatabase[i]] = physicalIndex;
			}
			physicalIndexCache.CleanupUnusedPhysicalIndexesHelper(context);
		}

		public static void DismountEventHandler(StoreDatabase database)
		{
			database.ComponentData[PhysicalIndexCache.physicalIndexCacheDataSlot] = null;
		}

		internal static PhysicalIndexCache GetPhysicalIndexCache(StoreDatabase database)
		{
			return database.ComponentData[PhysicalIndexCache.physicalIndexCacheDataSlot] as PhysicalIndexCache;
		}

		internal static PhysicalIndex GetPhysicalIndex(Context context, int physicalIndexNumber)
		{
			PhysicalIndexCache physicalIndexCache = PhysicalIndexCache.GetPhysicalIndexCache(context.Database);
			return physicalIndexCache.GetPhysicalIndexHelper(context, physicalIndexNumber);
		}

		internal static PhysicalIndex GetPhysicalIndex(Context context, int keyColumnCount, short identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings, bool permitReverseOrder)
		{
			PhysicalIndexCache physicalIndexCache = PhysicalIndexCache.GetPhysicalIndexCache(context.Database);
			return physicalIndexCache.GetPhysicalIndexHelper(context, keyColumnCount, identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings, permitReverseOrder);
		}

		internal static PhysicalIndex FindExistingPhysicalIndex(Context context, int keyColumnCount, int lcid, short identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings)
		{
			PhysicalIndexCache physicalIndexCache = PhysicalIndexCache.GetPhysicalIndexCache(context.Database);
			return physicalIndexCache.FindExistingPhysicalIndexHelper(context, keyColumnCount, lcid, identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings);
		}

		internal static void CleanupUnusedPhysicalIndexes(Context context)
		{
			PhysicalIndexCache physicalIndexCache = PhysicalIndexCache.GetPhysicalIndexCache(context.Database);
			physicalIndexCache.CleanupUnusedPhysicalIndexesHelper(context);
		}

		internal static void DeletePhysicalIndex(Context context, int indexNum)
		{
			PhysicalIndexCache physicalIndexCache = PhysicalIndexCache.GetPhysicalIndexCache(context.Database);
			physicalIndexCache.DeletePhysicalIndexHelper(context, indexNum);
		}

		private static List<int> GetListOfPhysicalIndexesFromDatabase(Context context)
		{
			List<int> list = new List<int>(255);
			PseudoIndexDefinitionTable pseudoIndexDefinitionTable = DatabaseSchema.PseudoIndexDefinitionTable(context.Database);
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexDefinitionTable.Table, pseudoIndexDefinitionTable.Table.PrimaryKeyIndex, new Column[]
			{
				pseudoIndexDefinitionTable.PhysicalIndexNumber
			}, null, null, 0, 0, KeyRange.AllRows, false, false))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						list.Add(reader.GetInt32(pseudoIndexDefinitionTable.PhysicalIndexNumber));
					}
				}
			}
			return list;
		}

		private PhysicalIndex GetPhysicalIndexHelper(Context context, int physicalIndexNumber)
		{
			PhysicalIndex result;
			using (LockManager.Lock(this.lockName, LockManager.LockType.PhysicalIndexCache, context.Diagnostics))
			{
				this.cache.TryGetValue(physicalIndexNumber, out result);
			}
			return result;
		}

		private PhysicalIndex GetPhysicalIndexHelper(Context context, int keyColumnCount, short identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings, bool permitReverseOrder)
		{
			PhysicalIndex physicalIndex = null;
			bool flag = context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet;
			Connection connection = context.GetConnection();
			bool databaseTransactionStarted = context.DatabaseTransactionStarted;
			long num = databaseTransactionStarted ? connection.TransactionTimeStamp : long.MaxValue;
			using (LockManager.Lock(this.lockName, LockManager.LockType.PhysicalIndexCache, context.Diagnostics))
			{
				foreach (PhysicalIndex physicalIndex2 in this.cache.Values)
				{
					if (physicalIndex2.IndexIsVisibleForConnection(connection) && (!flag || !databaseTransactionStarted || num > physicalIndex2.CreationTimeStamp) && physicalIndex2.IndexMatch(CultureHelper.GetLcidFromCulture(context.Culture), keyColumnCount, (int)identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings, permitReverseOrder))
					{
						physicalIndex = physicalIndex2;
						break;
					}
				}
				if (physicalIndex == null)
				{
					bool flag2 = false;
					if (!flag || !databaseTransactionStarted)
					{
						context.PushConnection();
					}
					try
					{
						physicalIndex = PhysicalIndex.CreatePhysicalIndex(context, keyColumnCount, identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings);
						if (!flag || !databaseTransactionStarted)
						{
							context.Commit();
							flag2 = true;
							physicalIndex.CreationTimeStamp = context.GetConnection().TransactionTimeStamp;
							this.cache.Add(physicalIndex.PhysicalIndexNumber, physicalIndex);
						}
						else
						{
							physicalIndex.ConnectionLimitVisibility = connection;
							this.cache.Add(physicalIndex.PhysicalIndexNumber, physicalIndex);
							PhysicalIndexCache.PhysicalIndexTableCommitCallback stateObject = new PhysicalIndexCache.PhysicalIndexTableCommitCallback(this, physicalIndex);
							context.RegisterStateObject(stateObject);
						}
					}
					finally
					{
						if (!flag || !databaseTransactionStarted)
						{
							if (!flag2)
							{
								context.Abort();
							}
							context.PopConnection();
						}
					}
				}
			}
			return physicalIndex;
		}

		private PhysicalIndex FindExistingPhysicalIndexHelper(Context context, int keyColumnCount, int lcid, short identityColumnIndex, PropertyType[] columnTypes, int[] columnMaxLengths, bool[] columnFixedLengths, bool[] columnAscendings)
		{
			PhysicalIndex result = null;
			using (LockManager.Lock(this.lockName, LockManager.LockType.PhysicalIndexCache, context.Diagnostics))
			{
				foreach (PhysicalIndex physicalIndex in this.cache.Values)
				{
					if (physicalIndex.IndexIsVisibleForConnection(context.GetConnection()) && physicalIndex.IndexMatch(lcid, keyColumnCount, (int)identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings, true))
					{
						result = physicalIndex;
						break;
					}
				}
			}
			return result;
		}

		private void CleanupUnusedPhysicalIndexesHelper(Context context)
		{
		}

		private void DeletePhysicalIndexHelper(Context context, int indexNum)
		{
			if (context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Jet)
			{
				this.cache.Remove(indexNum);
				PseudoIndexDefinitionTable pseudoIndexDefinitionTable = DatabaseSchema.PseudoIndexDefinitionTable(context.Database);
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					indexNum
				});
				using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, pseudoIndexDefinitionTable.Table, pseudoIndexDefinitionTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false), false))
				{
					int num = (int)deleteOperator.ExecuteScalar();
				}
				string tableName = PhysicalIndex.GetTableName(indexNum);
				Factory.DeleteTable(context, tableName);
				context.Database.PhysicalDatabase.RemoveTableMetadata(tableName);
			}
		}

		private const int AvgPhysicalIndexPerMDB = 255;

		private static int physicalIndexCacheDataSlot = -1;

		private readonly LockName<Guid> lockName;

		private Dictionary<int, PhysicalIndex> cache;

		private class PhysicalIndexTableCommitCallback : IStateObject
		{
			public PhysicalIndexTableCommitCallback(PhysicalIndexCache state, PhysicalIndex indexDef)
			{
				this.state = state;
				this.indexDef = indexDef;
			}

			void IStateObject.OnBeforeCommit(Context context)
			{
			}

			void IStateObject.OnCommit(Context context)
			{
				this.OnEndTransaction(context, true);
			}

			void IStateObject.OnAbort(Context context)
			{
				this.OnEndTransaction(context, false);
			}

			public void OnEndTransaction(Context context, bool committed)
			{
				using (LockManager.Lock(this.state.lockName, LockManager.LockType.PhysicalIndexCache, context.Diagnostics))
				{
					if (committed)
					{
						this.indexDef.CreationTimeStamp = context.GetConnection().TransactionTimeStamp;
						this.indexDef.ConnectionLimitVisibility = null;
					}
					else
					{
						this.state.cache.Remove(this.indexDef.PhysicalIndexNumber);
						context.Database.PhysicalDatabase.RemoveTableMetadata(this.indexDef.Table.Name);
					}
				}
			}

			private PhysicalIndexCache state;

			private PhysicalIndex indexDef;
		}
	}
}
