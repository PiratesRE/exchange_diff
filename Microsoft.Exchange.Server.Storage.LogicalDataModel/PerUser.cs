using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class PerUser : ILockName, IEquatable<ILockName>, IComparable<ILockName>
	{
		public static Task<StoreDatabase>.TaskCallback WrappedFlushCallback(Guid databaseGuid)
		{
			return TaskExecutionWrapper<StoreDatabase>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.FlushDirtyPerUserCaches, ClientType.System, databaseGuid), new TaskExecutionWrapper<StoreDatabase>.TaskCallback<Context>(PerUser.PerUserCache.FlushDirtyPerUserCachesTaskCallback));
		}

		public static void Initialize()
		{
			PerUser.PerUserCache.Initialize();
		}

		public static void MountEventHandler(Context context, StoreDatabase database, bool readOnly)
		{
			PerUser.PerUserCache.MountEventHandler(context, database, readOnly);
		}

		public static void DismountEventHandler(Context context, StoreDatabase database)
		{
			PerUser.PerUserCache.DismountEventHandler(context, database);
		}

		public static bool InsertInResident(Context context, Mailbox mailbox, Guid ownerMailboxGuid, ExchangeId folderId, ExchangeId cn)
		{
			PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
			bool result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = perUserCache.TakeWriteLock(context))
			{
				PerUser.ResidentPerUser residentPerUser = PerUser.ResidentPerUser.Load(context, mailbox, ownerMailboxGuid, folderId);
				if (residentPerUser == null)
				{
					residentPerUser = new PerUser.ResidentPerUser(ownerMailboxGuid, folderId, new IdSet(), mailbox.UtcNow);
					perUserCache.Insert(context, residentPerUser);
				}
				bool flag = residentPerUser.Insert(mailbox, cn);
				mailboxComponentOperationFrame.Success();
				result = flag;
			}
			return result;
		}

		public static bool RemoveFromResident(Context context, Mailbox mailbox, Guid ownerMailboxGuid, ExchangeId folderId, ExchangeId cn)
		{
			PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
			bool result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = perUserCache.TakeWriteLock(context))
			{
				PerUser.ResidentPerUser residentPerUser = PerUser.ResidentPerUser.Load(context, mailbox, ownerMailboxGuid, folderId);
				if (residentPerUser == null)
				{
					residentPerUser = new PerUser.ResidentPerUser(ownerMailboxGuid, folderId, new IdSet(), mailbox.UtcNow);
					perUserCache.Insert(context, residentPerUser);
				}
				bool flag = residentPerUser.Remove(mailbox, cn);
				mailboxComponentOperationFrame.Success();
				result = flag;
			}
			return result;
		}

		public static PerUser LoadResident(Context context, Mailbox mailbox, Guid ownerMailboxGuid, ExchangeId folderId)
		{
			using (PerUser.PerUserCache.TakeReadLock(context, mailbox))
			{
				PerUser.ResidentPerUser residentPerUser = PerUser.ResidentPerUser.Load(context, mailbox, ownerMailboxGuid, folderId);
				if (residentPerUser != null)
				{
					return residentPerUser;
				}
			}
			PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
			PerUser result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame2 = perUserCache.TakeWriteLock(context))
			{
				PerUser.ResidentPerUser residentPerUser = PerUser.ResidentPerUser.Load(context, mailbox, ownerMailboxGuid, folderId);
				mailboxComponentOperationFrame2.Success();
				result = residentPerUser;
			}
			return result;
		}

		internal static PerUser CreateResident(Context context, Mailbox mailbox, Guid mailboxGuid, ExchangeId folderId, IdSet cnSet)
		{
			PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
			PerUser result;
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = perUserCache.TakeWriteLock(context))
			{
				PerUser.ResidentPerUser residentPerUser = new PerUser.ResidentPerUser(mailboxGuid, folderId, cnSet, mailbox.UtcNow);
				perUserCache.Insert(context, residentPerUser);
				mailboxComponentOperationFrame.Success();
				result = residentPerUser;
			}
			return result;
		}

		internal static void CreateResidentAndSave(Context context, Mailbox mailbox, Guid mailboxGuid, ExchangeId folderId, IdSet cnSet, DateTime? lastModifiedTime)
		{
			PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = perUserCache.TakeWriteLock(context))
			{
				PerUser.ResidentPerUser residentPerUser = new PerUser.ResidentPerUser(mailboxGuid, folderId, cnSet, lastModifiedTime ?? mailbox.UtcNow);
				perUserCache.Insert(context, residentPerUser);
				residentPerUser.Save(context, mailbox.SharedState);
				mailboxComponentOperationFrame.Success();
			}
		}

		public static IEnumerable<PerUser> ResidentEntries(Context context, Mailbox mailbox)
		{
			PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailbox.SharedState);
			if (perUserCacheNoCreate != null)
			{
				perUserCacheNoCreate.FlushAllDirtyEntries(context);
			}
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			ReplidGuidMap replidGuidMap = mailbox.ReplidGuidMap;
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				true
			});
			List<PerUser> results = new List<PerUser>(100);
			using (PerUser.PerUserCache.TakeReadLock(context, mailbox))
			{
				PerUser.EnumerateRows(context, startStopKey, startStopKey, delegate(DataRow dataRow)
				{
					results.Add(new PerUser.ResidentPerUser(context, perUserTable, dataRow, replidGuidMap));
				});
			}
			return results;
		}

		public static IEnumerable<PerUser> ResidentEntriesForFolder(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailbox.SharedState);
			if (perUserCacheNoCreate != null)
			{
				perUserCacheNoCreate.FlushAllDirtyEntries(context);
			}
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			ReplidGuidMap replidGuidMap = mailbox.ReplidGuidMap;
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				true,
				folderId.To26ByteArray()
			});
			List<PerUser> results = new List<PerUser>(100);
			using (PerUser.PerUserCache.TakeReadLock(context, mailbox))
			{
				PerUser.EnumerateRows(context, startStopKey, startStopKey, delegate(DataRow dataRow)
				{
					results.Add(new PerUser.ResidentPerUser(context, perUserTable, dataRow, replidGuidMap));
				});
			}
			return results;
		}

		public static void DeleteAllResidentEntriesForFolder(Context context, Folder folder)
		{
			PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(folder.Mailbox.SharedState);
			if (perUserCacheNoCreate != null)
			{
				perUserCacheNoCreate.FlushAllDirtyEntries(context);
				perUserCacheNoCreate.Reset();
			}
			ExchangeId id = folder.GetId(context);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				folder.Mailbox.MailboxPartitionNumber,
				true,
				id.To26ByteArray()
			});
			PerUser.DeleteRows(context, startStopKey);
		}

		public static void DeleteAllResidentEntries(Context context, Mailbox mailbox)
		{
			PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailbox.SharedState);
			if (perUserCacheNoCreate != null)
			{
				perUserCacheNoCreate.FlushAllDirtyEntries(context);
				perUserCacheNoCreate.Reset();
			}
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				true
			});
			PerUser.DeleteRows(context, startStopKey);
		}

		public static PerUser LoadForeign(Context context, Mailbox mailbox, byte[] foreignFolderId)
		{
			PerUser.ForeignPerUser perUser = null;
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				false,
				foreignFolderId
			});
			using (PerUser.PerUserCache.TakeReadLock(context, mailbox))
			{
				PerUser.EnumerateRows(context, startStopKey, startStopKey, delegate(DataRow dataRow)
				{
					perUser = new PerUser.ForeignPerUser(context, perUserTable, dataRow);
				});
			}
			return perUser;
		}

		public static PerUser CreateForeign(Guid replicaGuid, byte[] foreignFolderId, byte[] foreignCNSet)
		{
			return new PerUser.ForeignPerUser(replicaGuid, foreignFolderId, foreignCNSet, DateTime.UtcNow);
		}

		public static IEnumerable<PerUser> ForeignEntries(Context context, Mailbox mailbox)
		{
			byte[] startFolderId = new byte[22];
			return PerUser.ForeignEntries(context, mailbox, startFolderId);
		}

		public static IEnumerable<PerUser> ForeignEntries(Context context, Mailbox mailbox, byte[] startFolderId)
		{
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			StartStopKey startKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				false,
				startFolderId
			});
			StartStopKey stopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				false
			});
			List<PerUser> results = new List<PerUser>(100);
			using (PerUser.PerUserCache.TakeReadLock(context, mailbox))
			{
				PerUser.EnumerateRows(context, startKey, stopKey, delegate(DataRow dataRow)
				{
					results.Add(new PerUser.ForeignPerUser(context, perUserTable, dataRow));
				});
			}
			return results;
		}

		public static PerUser Parse(Context context, byte[] buffer, ReplidGuidMap replidGuidMap)
		{
			MDBEFCollection mdbefcollection = MDBEFCollection.CreateFrom(buffer, Encoding.UTF8);
			Guid mailboxGuid = Guid.Empty;
			ExchangeId folderId = ExchangeId.Zero;
			IdSet cnSet = null;
			DateTime dateTime = DateTime.MinValue;
			foreach (AnnotatedPropertyValue annotatedPropertyValue in mdbefcollection)
			{
				uint num = annotatedPropertyValue.PropertyTag;
				if (num <= 131330U)
				{
					if (num != 65608U)
					{
						if (num == 131330U)
						{
							folderId = ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, (byte[])annotatedPropertyValue.PropertyValue.Value);
						}
					}
					else
					{
						mailboxGuid = (Guid)annotatedPropertyValue.PropertyValue.Value;
					}
				}
				else if (num != 196866U)
				{
					if (num == 262208U)
					{
						dateTime = (DateTime)((ExDateTime)annotatedPropertyValue.PropertyValue.Value);
					}
				}
				else
				{
					cnSet = IdSet.Parse(context, (byte[])annotatedPropertyValue.PropertyValue.Value);
				}
			}
			return new PerUser.ResidentPerUser(mailboxGuid, folderId, cnSet, dateTime);
		}

		public abstract byte[] Serialize(Context context);

		public abstract void Save(Context context, MailboxState mailboxState);

		public abstract bool Contains(Mailbox mailbox, ExchangeId cn);

		public abstract Guid Guid { get; }

		public abstract ExchangeId FolderId { get; }

		public abstract byte[] FolderIdBytes { get; }

		internal abstract IdSet CNSet { get; }

		public abstract byte[] CNSetBytes { get; }

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public DateTime LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public LockManager.LockLevel LockLevel
		{
			get
			{
				return LockManager.LockLevel.PerUser;
			}
		}

		public LockManager.NamedLockObject CachedLockObject { get; set; }

		public abstract ILockName GetLockNameToCache();

		public virtual bool Equals(ILockName other)
		{
			return other != null && this.CompareTo(other) == 0;
		}

		public virtual int CompareTo(ILockName other)
		{
			int num = this.LockLevel.CompareTo(other.LockLevel);
			if (num == 0)
			{
				PerUser perUser = other as PerUser;
				num = this.Guid.CompareTo(perUser.Guid);
				if (num == 0)
				{
					num = ValueHelper.ArraysCompare<byte>(this.FolderIdBytes, perUser.FolderIdBytes);
				}
			}
			return num;
		}

		public override int GetHashCode()
		{
			if (this.cachedHashCode == 0)
			{
				this.cachedHashCode = (this.LockLevel.GetHashCode() ^ this.Guid.GetHashCode());
				if (this.FolderIdBytes != null)
				{
					for (int i = 0; i < this.FolderIdBytes.Length; i++)
					{
						this.cachedHashCode ^= ((int)this.FolderIdBytes[i] << 8 * (i % 4)).GetHashCode();
					}
				}
				if (this.cachedHashCode == 0)
				{
					this.cachedHashCode = 1;
				}
			}
			return this.cachedHashCode;
		}

		public void SaveWithCacheLock(Context context, Mailbox mailbox)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = PerUser.PerUserCache.TakeWriteLock(context, mailbox))
			{
				this.Save(context, mailbox.SharedState);
				mailboxComponentOperationFrame.Success();
			}
		}

		private static void EnumerateRows(Context context, StartStopKey startKey, StartStopKey stopKey, Action<DataRow> callback)
		{
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, perUserTable.Table, perUserTable.Table.PrimaryKeyIndex, perUserTable.Table.Columns.ToArray<PhysicalColumn>(), null, null, 0, 0, new KeyRange(startKey, stopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, perUserTable.Table, false, reader))
						{
							if (dataRow == null)
							{
								break;
							}
							callback(dataRow);
						}
					}
				}
			}
		}

		protected static void DeleteRows(Context context, StartStopKey startStopKey)
		{
			PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, perUserTable.Table, perUserTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), true))
			{
				deleteOperator.ExecuteScalar();
			}
		}

		private const uint MailboxGuidPropertyTagInt = 65608U;

		private const uint FolderIdPropertyTagInt = 131330U;

		private const uint CNSetPropertyTagInt = 196866U;

		private const uint LastModPropertyTagInt = 262208U;

		private const uint TypeTagInt = 327683U;

		internal static readonly PropertyTag MailboxGuidPropertyTag = new PropertyTag(65608U);

		internal static readonly PropertyTag FolderIdPropertyTag = new PropertyTag(131330U);

		internal static readonly PropertyTag CNSetPropertyTag = new PropertyTag(196866U);

		internal static readonly PropertyTag LastModPropertyTag = new PropertyTag(262208U);

		internal static readonly PropertyTag TypeTag = new PropertyTag(327683U);

		protected DateTime lastModificationTime;

		protected bool isDirty;

		protected int cachedHashCode;

		internal class ResidentPerUser : PerUser
		{
			internal static IDisposable SetLoadHook(Action action)
			{
				return PerUser.ResidentPerUser.loadHook.SetTestHook(action);
			}

			public static PerUser.ResidentPerUser Load(Context context, Mailbox mailbox, Guid ownerMailboxGuid, ExchangeId folderId)
			{
				if (PerUser.ResidentPerUser.loadHook.Value != null)
				{
					PerUser.ResidentPerUser.loadHook.Value();
				}
				PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailbox.SharedState);
				PerUser.ResidentPerUser perUser = perUserCacheNoCreate.Find(context, ownerMailboxGuid, folderId);
				if (perUser == null && perUserCacheNoCreate.HasWriteLock())
				{
					ReplidGuidMap replidGuidMap = mailbox.ReplidGuidMap;
					PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						mailbox.MailboxPartitionNumber,
						true,
						folderId.To26ByteArray(),
						ownerMailboxGuid
					});
					PerUser.EnumerateRows(context, startStopKey, startStopKey, delegate(DataRow dataRow)
					{
						perUser = new PerUser.ResidentPerUser(context, perUserTable, dataRow, replidGuidMap);
					});
					if (perUser != null)
					{
						perUserCacheNoCreate.Insert(context, perUser);
					}
				}
				return perUser;
			}

			public override byte[] Serialize(Context context)
			{
				MDBEFCollection mdbefcollection = new MDBEFCollection();
				mdbefcollection.AddAnnotatedProperty(new AnnotatedPropertyValue(PerUser.MailboxGuidPropertyTag, new PropertyValue(PerUser.MailboxGuidPropertyTag, this.mailboxGuid), null));
				mdbefcollection.AddAnnotatedProperty(new AnnotatedPropertyValue(PerUser.FolderIdPropertyTag, new PropertyValue(PerUser.FolderIdPropertyTag, this.folderId.To26ByteArray()), null));
				mdbefcollection.AddAnnotatedProperty(new AnnotatedPropertyValue(PerUser.CNSetPropertyTag, new PropertyValue(PerUser.CNSetPropertyTag, this.cnSet.Serialize()), null));
				mdbefcollection.AddAnnotatedProperty(new AnnotatedPropertyValue(PerUser.LastModPropertyTag, new PropertyValue(PerUser.LastModPropertyTag, (ExDateTime)this.lastModificationTime), null));
				return mdbefcollection.Serialize(Encoding.UTF8);
			}

			public override void Save(Context context, MailboxState mailboxState)
			{
				using (LockManager.Lock(this, LockManager.LockType.PerUserExclusive, context.Diagnostics))
				{
					PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						mailboxState.MailboxPartitionNumber,
						true,
						this.folderId.To26ByteArray(),
						this.mailboxGuid
					});
					PerUser.DeleteRows(context, startStopKey);
					if (!this.cnSet.IsEmpty)
					{
						using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, perUserTable.Table, false, new ColumnValue[]
						{
							new ColumnValue(perUserTable.MailboxPartitionNumber, mailboxState.MailboxPartitionNumber),
							new ColumnValue(perUserTable.ResidentFolder, true),
							new ColumnValue(perUserTable.FolderId, this.folderId.To26ByteArray()),
							new ColumnValue(perUserTable.Guid, this.mailboxGuid)
						}))
						{
							this.SaveIntoDataRow(context, mailboxState, perUserTable, dataRow);
						}
					}
					this.isDirty = false;
				}
			}

			internal bool Insert(Mailbox mailbox, ExchangeId cn)
			{
				bool result;
				using (LockManager.Lock(this, LockManager.LockType.PerUserExclusive))
				{
					if (cn.IsValid && this.cnSet.Insert(cn))
					{
						this.lastModificationTime = mailbox.UtcNow;
						this.isDirty = true;
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}

			internal bool Remove(Mailbox mailbox, ExchangeId cn)
			{
				bool result;
				using (LockManager.Lock(this, LockManager.LockType.PerUserExclusive))
				{
					if (cn.IsValid && this.cnSet.Remove(cn))
					{
						this.lastModificationTime = mailbox.UtcNow;
						this.isDirty = true;
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}

			public override bool Contains(Mailbox mailbox, ExchangeId cn)
			{
				bool result;
				using (LockManager.Lock(this, LockManager.LockType.PerUserShared))
				{
					result = (cn.IsValid && this.cnSet.Contains(cn));
				}
				return result;
			}

			public override Guid Guid
			{
				get
				{
					return this.mailboxGuid;
				}
			}

			public override ExchangeId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			public override byte[] FolderIdBytes
			{
				get
				{
					return this.folderId.To26ByteArray();
				}
			}

			internal override IdSet CNSet
			{
				get
				{
					return this.cnSet;
				}
			}

			public override byte[] CNSetBytes
			{
				get
				{
					return this.CNSet.Serialize();
				}
			}

			public ResidentPerUser(Context context, PerUserTable perUserTable, DataRow dataRow, ReplidGuidMap replidGuidMap) : this((Guid)dataRow.GetValue(context, perUserTable.Guid), ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, (byte[])dataRow.GetValue(context, perUserTable.FolderId)), IdSet.Parse(context, (byte[])dataRow.GetValue(context, perUserTable.CnsetRead)), (DateTime)dataRow.GetValue(context, perUserTable.LastModificationTime))
			{
				this.isDirty = false;
			}

			internal ResidentPerUser(Guid mailboxGuid, ExchangeId folderId, IdSet cnSet, DateTime lastModificationTime)
			{
				this.mailboxGuid = mailboxGuid;
				this.folderId = folderId;
				this.cnSet = cnSet;
				this.lastModificationTime = lastModificationTime;
				this.isDirty = true;
			}

			public override ILockName GetLockNameToCache()
			{
				return new PerUser.ResidentPerUser(this.mailboxGuid, this.folderId, null, this.lastModificationTime);
			}

			private void SaveIntoDataRow(Context context, MailboxState mailboxState, PerUserTable perUserTable, DataRow dataRow)
			{
				dataRow.SetValue(context, perUserTable.ResidentFolder, true);
				dataRow.SetValue(context, perUserTable.Guid, this.mailboxGuid);
				dataRow.SetValue(context, perUserTable.FolderId, this.FolderIdBytes);
				dataRow.SetValue(context, perUserTable.CnsetRead, this.CNSetBytes);
				dataRow.SetValue(context, perUserTable.LastModificationTime, this.lastModificationTime);
				dataRow.Flush(context);
			}

			private static Hookable<Action> loadHook = Hookable<Action>.Create(true, null);

			private readonly Guid mailboxGuid;

			private readonly ExchangeId folderId;

			private readonly IdSet cnSet;
		}

		internal class ForeignPerUser : PerUser
		{
			public ForeignPerUser(Context context, PerUserTable perUserTable, DataRow dataRow) : this((Guid)dataRow.GetValue(context, perUserTable.Guid), (byte[])dataRow.GetValue(context, perUserTable.FolderId), (byte[])dataRow.GetValue(context, perUserTable.CnsetRead), (DateTime)dataRow.GetValue(context, perUserTable.LastModificationTime))
			{
			}

			public ForeignPerUser(Guid replicaGuid, byte[] foreignFolderId, byte[] foreignCNSet, DateTime lastModificationTime)
			{
				this.replicaGuid = replicaGuid;
				this.folderIdBytes = foreignFolderId;
				this.cnSetBytes = foreignCNSet;
				this.lastModificationTime = lastModificationTime;
			}

			public override ILockName GetLockNameToCache()
			{
				return new PerUser.ForeignPerUser(this.replicaGuid, this.folderIdBytes, null, this.lastModificationTime);
			}

			public override byte[] Serialize(Context context)
			{
				throw new InvalidOperationException();
			}

			public override bool Contains(Mailbox mailbox, ExchangeId cn)
			{
				throw new InvalidOperationException();
			}

			public override void Save(Context context, MailboxState mailboxState)
			{
				using (LockManager.Lock(this, LockManager.LockType.PerUserExclusive, context.Diagnostics))
				{
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						mailboxState.MailboxPartitionNumber,
						false,
						this.folderIdBytes
					});
					PerUser.DeleteRows(context, startStopKey);
					if (this.cnSetBytes.Length > 0)
					{
						PerUserTable perUserTable = DatabaseSchema.PerUserTable(context.Database);
						using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, perUserTable.Table, false, new ColumnValue[]
						{
							new ColumnValue(perUserTable.MailboxPartitionNumber, mailboxState.MailboxPartitionNumber),
							new ColumnValue(perUserTable.ResidentFolder, false),
							new ColumnValue(perUserTable.FolderId, this.folderIdBytes),
							new ColumnValue(perUserTable.Guid, this.replicaGuid)
						}))
						{
							this.SaveIntoDataRow(context, perUserTable, dataRow);
						}
					}
				}
			}

			public override Guid Guid
			{
				get
				{
					return this.replicaGuid;
				}
			}

			public override ExchangeId FolderId
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override byte[] FolderIdBytes
			{
				get
				{
					return this.folderIdBytes;
				}
			}

			internal override IdSet CNSet
			{
				get
				{
					throw new InvalidOperationException();
				}
			}

			public override byte[] CNSetBytes
			{
				get
				{
					return this.cnSetBytes;
				}
			}

			private void SaveIntoDataRow(Context context, PerUserTable perUserTable, DataRow dataRow)
			{
				dataRow.SetValue(context, perUserTable.ResidentFolder, false);
				dataRow.SetValue(context, perUserTable.Guid, this.replicaGuid);
				dataRow.SetValue(context, perUserTable.FolderId, this.folderIdBytes);
				dataRow.SetValue(context, perUserTable.CnsetRead, this.cnSetBytes);
				dataRow.SetValue(context, perUserTable.LastModificationTime, this.lastModificationTime);
				dataRow.Flush(context);
			}

			private readonly Guid replicaGuid;

			private readonly byte[] folderIdBytes;

			private readonly byte[] cnSetBytes;
		}

		internal struct PerUserKey : IComparable, IComparable<PerUser.PerUserKey>, IEquatable<PerUser.PerUserKey>
		{
			public PerUserKey(Guid mailboxGuid, ExchangeId folderId)
			{
				this.mailboxGuid = mailboxGuid;
				this.folderId = folderId;
			}

			public int CompareTo(object other)
			{
				return this.CompareTo((PerUser.PerUserKey)other);
			}

			public int CompareTo(PerUser.PerUserKey other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return 0;
				}
				int num = this.folderId.CompareTo(other.folderId);
				if (num == 0)
				{
					num = this.mailboxGuid.CompareTo(other.mailboxGuid);
				}
				return num;
			}

			public override bool Equals(object other)
			{
				return this.Equals((PerUser.PerUserKey)other);
			}

			public bool Equals(PerUser.PerUserKey other)
			{
				return this.CompareTo(other) == 0;
			}

			public override int GetHashCode()
			{
				return this.folderId.GetHashCode() ^ this.mailboxGuid.GetHashCode();
			}

			private readonly Guid mailboxGuid;

			private readonly ExchangeId folderId;
		}

		internal class PerUserCache : SingleKeyCache<PerUser.PerUserKey, PerUser.ResidentPerUser>, ICache
		{
			internal static void Initialize()
			{
			}

			internal static void MountEventHandler(Context context, StoreDatabase database, bool readOnly)
			{
				if (!readOnly && !PerUser.PerUserCache.skipFlushDirtyPerUserCachesTaskTestHook.Value)
				{
					Task<StoreDatabase>.TaskCallback callback = PerUser.WrappedFlushCallback(database.MdbGuid);
					RecurringTask<StoreDatabase> task = new RecurringTask<StoreDatabase>(callback, database, TimeSpan.FromHours(1.0), false);
					database.TaskList.Add(task, true);
				}
			}

			internal static void DismountEventHandler(Context context, StoreDatabase database)
			{
				using (database.SharedLock(context.Diagnostics))
				{
					PerUser.PerUserCache.FlushDirtyPerUserCaches(context, database, () => true);
				}
			}

			internal static void FlushDirtyPerUserCachesTaskCallback(Context context, StoreDatabase database, Func<bool> shouldTaskContinue)
			{
				using (context.AssociateWithDatabase(database))
				{
					PerUser.PerUserCache.FlushDirtyPerUserCaches(context, database, shouldTaskContinue);
				}
			}

			internal static IDisposable SetSkipFlushDirtyPerUserCachesTaskTestHook()
			{
				return PerUser.PerUserCache.skipFlushDirtyPerUserCachesTaskTestHook.SetTestHook(true);
			}

			private static void FlushDirtyPerUserCaches(Context context, StoreDatabase database, Func<bool> shouldTaskContinue)
			{
				if (database.IsReadOnly)
				{
					return;
				}
				List<int> activeMailboxNumbers = MailboxStateCache.GetActiveMailboxNumbers(context);
				if (activeMailboxNumbers == null)
				{
					return;
				}
				for (int i = 0; i < activeMailboxNumbers.Count; i++)
				{
					if (!shouldTaskContinue())
					{
						return;
					}
					context.ResetFailureHistory();
					context.InitializeMailboxExclusiveOperation(activeMailboxNumbers[i], ExecutionDiagnostics.OperationSource.PerUserCacheFlush, TimeSpan.FromMinutes(1.0));
					bool commit = false;
					try
					{
						ErrorCode first = context.StartMailboxOperation(MailboxCreation.DontAllow, false, true);
						if (!(first != ErrorCode.NoError))
						{
							MailboxState lockedMailboxState = context.LockedMailboxState;
							try
							{
								lockedMailboxState.AddReference();
								if (!lockedMailboxState.CurrentlyActive)
								{
									goto IL_B4;
								}
								ICache perUserCache = lockedMailboxState.PerUserCache;
								if (perUserCache != null)
								{
									perUserCache.FlushAllDirtyEntries(context);
								}
							}
							finally
							{
								lockedMailboxState.ReleaseReference();
							}
							commit = true;
						}
					}
					finally
					{
						if (context.IsMailboxOperationStarted)
						{
							context.EndMailboxOperation(commit);
						}
					}
					IL_B4:;
				}
			}

			public static PerUser.PerUserCache GetPerUserCache(Context context, MailboxState mailboxState)
			{
				PerUser.PerUserCache perUserCache = mailboxState.PerUserCache as PerUser.PerUserCache;
				if (perUserCache == null)
				{
					EvictionPolicy<PerUser.PerUserKey> evictionPolicy = new LRU2WithTimeToLiveExpirationPolicy<PerUser.PerUserKey>(ConfigurationSchema.PerUserCacheSize.Value, ConfigurationSchema.PerUserCacheExpiration.Value, false);
					mailboxState.PerUserCache = new PerUser.PerUserCache(mailboxState, evictionPolicy, null);
					perUserCache = (PerUser.PerUserCache)mailboxState.PerUserCache;
				}
				return perUserCache;
			}

			public static PerUser.PerUserCache GetPerUserCacheNoCreate(MailboxState mailboxState)
			{
				return mailboxState.PerUserCache as PerUser.PerUserCache;
			}

			public static MailboxComponentOperationFrame TakeReadLock(Context context, Mailbox mailbox)
			{
				PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
				return perUserCache.TakeReadLock(context);
			}

			public static MailboxComponentOperationFrame TakeWriteLock(Context context, Mailbox mailbox)
			{
				PerUser.PerUserCache perUserCache = PerUser.PerUserCache.GetPerUserCache(context, mailbox.SharedState);
				return perUserCache.TakeWriteLock(context);
			}

			[Conditional("DEBUG")]
			public static void AssertWriteLockHeld(MailboxState mailboxState)
			{
				PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailboxState);
			}

			[Conditional("DEBUG")]
			public static void AssertLockHeld(MailboxState mailboxState)
			{
				PerUser.PerUserCache perUserCacheNoCreate = PerUser.PerUserCache.GetPerUserCacheNoCreate(mailboxState);
			}

			public MailboxComponentOperationFrame TakeReadLock(Context context)
			{
				return context.MailboxComponentReadOperation(this.cacheLock);
			}

			public bool HasReadLock()
			{
				return this.cacheLock.TestSharedLock();
			}

			public MailboxComponentOperationFrame TakeWriteLock(Context context)
			{
				return context.MailboxComponentWriteOperation(this.cacheLock);
			}

			public bool HasWriteLock()
			{
				return this.cacheLock.TestExclusiveLock();
			}

			[Conditional("DEBUG")]
			public void AssertLockHeld()
			{
			}

			[Conditional("DEBUG")]
			public void AssertWriteLockHeld()
			{
			}

			internal PerUserCache(MailboxState mailboxState, EvictionPolicy<PerUser.PerUserKey> evictionPolicy, ICachePerformanceCounters perfCounters) : base(evictionPolicy, perfCounters)
			{
				this.mailboxState = mailboxState;
				this.cacheLock = new PerUser.PerUserCache.PerUserCacheLockableComponent(this);
			}

			public override void Reset()
			{
				base.Reset();
			}

			public bool FlushAllDirtyEntries(Context context)
			{
				if (this.mailboxState.Status != MailboxStatus.UserAccessible)
				{
					this.mailboxState.PerUserCache = null;
					return false;
				}
				bool result;
				using (MailboxComponentOperationFrame mailboxComponentOperationFrame = this.TakeWriteLock(context))
				{
					SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser> sortedDictionary = new SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser>();
					foreach (KeyValuePair<PerUser.PerUserKey, PerUser.ResidentPerUser> keyValuePair in this.keyToData)
					{
						if (keyValuePair.Value.IsDirty)
						{
							sortedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					if (sortedDictionary.Count != 0)
					{
						this.Flush(context, sortedDictionary);
					}
					mailboxComponentOperationFrame.Success();
					result = (sortedDictionary.Count != 0);
				}
				return result;
			}

			public void Insert(Context context, PerUser.ResidentPerUser perUser)
			{
				PerUser.PerUserKey key = new PerUser.PerUserKey(perUser.Guid, perUser.FolderId);
				try
				{
					PerUser.ResidentPerUser perUser2;
					if (this.keyToData.TryGetValue(key, out perUser2))
					{
						this.RemoveNoLock(context, perUser2);
					}
					this.currentOperationContext = context;
					base.Insert(key, perUser);
				}
				finally
				{
					this.currentOperationContext = null;
				}
			}

			public PerUser.ResidentPerUser Find(Context context, Guid mailboxGuid, ExchangeId folderId)
			{
				PerUser.PerUserKey key = new PerUser.PerUserKey(mailboxGuid, folderId);
				PerUser.ResidentPerUser result;
				using (LockManager.Lock(this, context.Diagnostics))
				{
					result = base.Find(key, false);
				}
				return result;
			}

			public void Remove(Context context, PerUser.ResidentPerUser perUser)
			{
				using (MailboxComponentOperationFrame mailboxComponentOperationFrame = this.TakeWriteLock(context))
				{
					this.RemoveNoLock(context, perUser);
					mailboxComponentOperationFrame.Success();
				}
			}

			public void RemoveNoLock(Context context, PerUser.ResidentPerUser perUser)
			{
				PerUser.PerUserKey key = new PerUser.PerUserKey(perUser.Guid, perUser.FolderId);
				try
				{
					this.currentOperationContext = context;
					base.Remove(key);
				}
				finally
				{
					this.currentOperationContext = null;
				}
				if (perUser.IsDirty)
				{
					this.Flush(context, new SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser>
					{
						{
							key,
							perUser
						}
					});
				}
			}

			public override void EvictionCheckpoint()
			{
				this.evictionPolicy.EvictionCheckpoint();
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
				}
				if (this.evictionPolicy.CountOfKeysToCleanup > 0)
				{
					SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser> sortedDictionary = new SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser>();
					foreach (PerUser.PerUserKey key in this.evictionPolicy.GetKeysToCleanup(true))
					{
						PerUser.ResidentPerUser residentPerUser;
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.keyToData.TryGetValue(key, out residentPerUser), "We should have the eviction key in our cache");
						if (residentPerUser.IsDirty)
						{
							sortedDictionary.Add(key, residentPerUser);
						}
						this.keyToData.Remove(key);
						if (this.performanceCounters != null)
						{
							this.performanceCounters.CacheRemoves.Increment();
						}
					}
					if (sortedDictionary.Count != 0)
					{
						this.Flush(this.currentOperationContext, sortedDictionary);
					}
				}
				if (this.performanceCounters != null)
				{
					this.performanceCounters.CacheSize.RawValue = (long)this.keyToData.Count;
					this.performanceCounters.CacheExpirationQueueLength.RawValue = (long)this.evictionPolicy.CountOfKeysToCleanup;
				}
			}

			private void Flush(Context context, SortedDictionary<PerUser.PerUserKey, PerUser.ResidentPerUser> toFlush)
			{
				foreach (KeyValuePair<PerUser.PerUserKey, PerUser.ResidentPerUser> keyValuePair in toFlush)
				{
					ICachePerformanceCounters performanceCounters = this.performanceCounters;
					keyValuePair.Value.Save(context, this.mailboxState);
				}
			}

			private static readonly Hookable<bool> skipFlushDirtyPerUserCachesTaskTestHook = Hookable<bool>.Create(true, false);

			private PerUser.PerUserCache.PerUserCacheLockableComponent cacheLock;

			private MailboxState mailboxState;

			private Context currentOperationContext;

			private class PerUserCacheLockableComponent : LockableMailboxComponent
			{
				public PerUserCacheLockableComponent(PerUser.PerUserCache perUserCache)
				{
					this.perUserCache = perUserCache;
				}

				public override int MailboxPartitionNumber
				{
					get
					{
						return this.perUserCache.mailboxState.MailboxPartitionNumber;
					}
				}

				public override Guid DatabaseGuid
				{
					get
					{
						return this.perUserCache.mailboxState.DatabaseGuid;
					}
				}

				public override LockManager.LockType ReaderLockType
				{
					get
					{
						return LockManager.LockType.PerUserCacheShared;
					}
				}

				public override LockManager.LockType WriterLockType
				{
					get
					{
						return LockManager.LockType.PerUserCacheExclusive;
					}
				}

				public override MailboxComponentId MailboxComponentId
				{
					get
					{
						return MailboxComponentId.PerUserCache;
					}
				}

				public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
				{
					switch (operationType)
					{
					case Connection.OperationType.CreateTable:
					case Connection.OperationType.DeleteTable:
						return this.TestExclusiveLock();
					default:
					{
						PerUserTable perUserTable = DatabaseSchema.PerUserTable(this.perUserCache.mailboxState.Database);
						bool flag = table.Equals(perUserTable.Table);
						switch (operationType)
						{
						case Connection.OperationType.Query:
							return (this.TestSharedLock() || this.TestExclusiveLock()) && flag;
						case Connection.OperationType.Insert:
						case Connection.OperationType.Update:
						case Connection.OperationType.Delete:
							return this.TestExclusiveLock() && flag;
						default:
							return flag;
						}
						break;
					}
					}
				}

				private readonly PerUser.PerUserCache perUserCache;
			}
		}
	}
}
