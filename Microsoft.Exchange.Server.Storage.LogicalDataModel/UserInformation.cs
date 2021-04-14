using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class UserInformation : PrivateObjectPropertyBag
	{
		internal static void Initialize()
		{
			UserInfoUpgrader.InitializeUpgraderAction(delegate(Context context)
			{
				UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(context.Database);
				userInfoTable.Table.CreateTable(context, UserInfoUpgrader.Instance.To.Value);
			}, delegate(StoreDatabase database)
			{
				UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(database);
				userInfoTable.Table.MinVersion = UserInfoUpgrader.Instance.To.Value;
			});
		}

		internal static void LockUserEntryForModification(Guid userInformationGuid)
		{
			using (LockManager.Lock(UserInformation.lockedEntryGuids, LockManager.LockType.LeafMonitorLock))
			{
				UserInformation.lockedEntryGuids.Add(userInformationGuid);
			}
		}

		internal static void UnlockUserEntryForModification(Guid userInformationGuid)
		{
			using (LockManager.Lock(UserInformation.lockedEntryGuids, LockManager.LockType.LeafMonitorLock))
			{
				UserInformation.lockedEntryGuids.Remove(userInformationGuid);
			}
		}

		internal static bool IsEntryLockedForModification(Guid userInformationGuid)
		{
			bool result;
			using (LockManager.Lock(UserInformation.lockedEntryGuids, LockManager.LockType.LeafMonitorLock))
			{
				result = UserInformation.lockedEntryGuids.Contains(userInformationGuid);
			}
			return result;
		}

		private UserInformation(Context context, ColumnValue userInformationGuid) : base(context, DatabaseSchema.UserInfoTable(context.Database).Table, true, false, true, true, new ColumnValue[]
		{
			userInformationGuid
		})
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.currentOperationContext = context;
				base.LoadData(context);
				disposeGuard.Success();
			}
		}

		private UserInformation(Context context, Reader reader) : base(context, DatabaseSchema.UserInfoTable(context.Database).Table, false, true, reader)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.currentOperationContext = context;
				base.LoadData(context);
				disposeGuard.Success();
			}
		}

		public static void Create(Context context, Guid userInformationGuid, Properties initialProperties)
		{
			UserInformation.Create(context, userInformationGuid, initialProperties, false);
		}

		public static void Create(Context context, Guid userInformationGuid, Properties initialProperties, bool moveMailbox)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)62028U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)47868U, ErrorCodeValue.NotSupported);
			}
			UserInformation.ValidatePropertiesOnSet(context, initialProperties, moveMailbox);
			UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(context.Database);
			using (UserInformation.LockExclusive(context, userInformationGuid))
			{
				if (UserInformation.IsEntryLockedForModification(userInformationGuid))
				{
					throw new StoreException((LID)44492U, ErrorCodeValue.UserInformationNoAccess);
				}
				try
				{
					using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (reader.Read())
							{
								UserInformation.UserInformationStatus @int = (UserInformation.UserInformationStatus)reader.GetInt16(userInfoTable.Status);
								if (moveMailbox || @int == UserInformation.UserInformationStatus.SoftDeleted)
								{
									DiagnosticContext.TraceLocation((LID)46156U);
									using (UserInformation userInformation = new UserInformation(context, reader))
									{
										userInformation.Delete(context, false);
										goto IL_EE;
									}
								}
								throw new StoreException((LID)64252U, ErrorCodeValue.UserInformationAlreadyExists);
							}
							IL_EE:;
						}
					}
					using (UserInformation userInformation2 = new UserInformation(context, new ColumnValue(userInfoTable.UserGuid, userInformationGuid)))
					{
						foreach (Property property in initialProperties)
						{
							ErrorCode errorCode = userInformation2.SetProperty(context, property.Tag, property.Value);
							if (errorCode != ErrorCode.NoError)
							{
								DiagnosticContext.TracePropTagError((LID)56060U, (uint)((int)errorCode), property.Tag.PropTag);
								throw new StoreException((LID)43772U, errorCode);
							}
						}
						DateTime utcNow = DateTime.UtcNow;
						userInformation2.SetColumn(context, userInfoTable.Status, 1);
						if (userInformation2.GetColumnValue(context, userInfoTable.CreationTime) == null)
						{
							userInformation2.SetColumn(context, userInfoTable.CreationTime, utcNow);
						}
						if (userInformation2.GetColumnValue(context, userInfoTable.LastModificationTime) == null)
						{
							userInformation2.SetColumn(context, userInfoTable.LastModificationTime, utcNow);
						}
						if (userInformation2.GetColumnValue(context, userInfoTable.ChangeNumber) == null)
						{
							userInformation2.SetColumn(context, userInfoTable.ChangeNumber, 1L);
						}
						userInformation2.Flush(context);
					}
					context.Commit();
				}
				finally
				{
					context.Abort();
				}
			}
		}

		public static Properties Read(Context context, Guid userInformationGuid, StorePropTag[] propertyTags)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)48588U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)60156U, ErrorCodeValue.NotSupported);
			}
			UserInformation.ValidatePropertiesOnGet(context, propertyTags);
			Properties result;
			using (UserInformation.LockShared(context, userInformationGuid))
			{
				using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							throw new StoreException((LID)51964U, ErrorCodeValue.UserInformationNotFound);
						}
						using (UserInformation userInformation = new UserInformation(context, reader))
						{
							Properties properties = new Properties(propertyTags.Length);
							foreach (StorePropTag storePropTag in propertyTags)
							{
								object propertyValue = userInformation.GetPropertyValue(context, storePropTag);
								if (propertyValue != null)
								{
									properties.Add(storePropTag, propertyValue);
								}
								else
								{
									properties.Add(Property.NotFoundError(storePropTag));
								}
							}
							result = properties;
						}
					}
				}
			}
			return result;
		}

		public static void Update(Context context, Guid userInformationGuid, Properties properties)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)36300U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)45820U, ErrorCodeValue.NotSupported);
			}
			UserInformation.ValidatePropertiesOnSet(context, properties, false);
			UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(context.Database);
			using (UserInformation.LockExclusive(context, userInformationGuid))
			{
				if (UserInformation.IsEntryLockedForModification(userInformationGuid))
				{
					throw new StoreException((LID)49740U, ErrorCodeValue.UserInformationNoAccess);
				}
				try
				{
					using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (!reader.Read())
							{
								throw new StoreException((LID)62204U, ErrorCodeValue.UserInformationNotFound);
							}
							using (UserInformation userInformation = new UserInformation(context, reader))
							{
								UserInformation.UserInformationStatus @int = (UserInformation.UserInformationStatus)reader.GetInt16(userInfoTable.Status);
								if (@int == UserInformation.UserInformationStatus.SoftDeleted)
								{
									throw new StoreException((LID)37452U, ErrorCodeValue.UserInformationSoftDeleted);
								}
								if (@int == UserInformation.UserInformationStatus.Disabled)
								{
									userInformation.SetColumn(context, userInfoTable.Status, 1);
								}
								foreach (Property property in properties)
								{
									ErrorCode errorCode = userInformation.SetProperty(context, property.Tag, property.Value);
									if (errorCode != ErrorCode.NoError)
									{
										DiagnosticContext.TracePropTagError((LID)54012U, (uint)((int)errorCode), property.Tag.PropTag);
										throw new StoreException((LID)33532U, errorCode);
									}
								}
								userInformation.SetColumn(context, userInfoTable.LastModificationTime, DateTime.UtcNow);
								userInformation.SetColumn(context, userInfoTable.ChangeNumber, (long)userInformation.GetColumnValue(context, userInfoTable.ChangeNumber) + 1L);
								userInformation.Flush(context);
							}
						}
					}
					context.Commit();
				}
				finally
				{
					context.Abort();
				}
			}
		}

		public static bool TryMarkAsSoftDeleted(Context context, Guid userInformationGuid)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)35404U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)53836U, ErrorCodeValue.NotSupported);
			}
			UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(context.Database);
			using (UserInformation.LockExclusive(context, userInformationGuid))
			{
				using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							return false;
						}
						using (UserInformation userInformation = new UserInformation(context, reader))
						{
							userInformation.SetColumn(context, userInfoTable.Status, 3);
							userInformation.SetColumn(context, userInfoTable.DeletedOn, DateTime.UtcNow);
							userInformation.Flush(context);
						}
					}
				}
				context.Commit();
			}
			return true;
		}

		public static void Delete(Context context, Guid userInformationGuid)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)52684U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)64828U, ErrorCodeValue.NotSupported);
			}
			using (UserInformation.LockExclusive(context, userInformationGuid))
			{
				using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							throw new StoreException((LID)58108U, ErrorCodeValue.UserInformationNotFound);
						}
						using (UserInformation userInformation = new UserInformation(context, reader))
						{
							userInformation.Delete(context, false);
						}
					}
				}
				context.Commit();
			}
		}

		public static Properties GetAllProperties(Context context, Guid userInformationGuid)
		{
			if (!DefaultSettings.Get.UserInformationIsEnabled)
			{
				throw new StoreException((LID)46540U, ErrorCodeValue.NotSupported);
			}
			if (!UserInfoUpgrader.IsReady(context, context.Database))
			{
				throw new StoreException((LID)64588U, ErrorCodeValue.NotSupported);
			}
			Properties result;
			using (UserInformation.LockShared(context, userInformationGuid))
			{
				using (TableOperator tableOperator = UserInformation.CreateTableOperator(context, userInformationGuid))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							DiagnosticContext.TraceLocation((LID)52300U);
							result = Microsoft.Exchange.Server.Storage.StoreCommonServices.Properties.Empty;
						}
						else
						{
							using (UserInformation userInformation = new UserInformation(context, reader))
							{
								Properties properties = new Properties(10);
								userInformation.EnumerateProperties(context, delegate(StorePropTag propTag, object propValue)
								{
									properties.Add(propTag, propValue);
									return true;
								}, true);
								result = properties;
							}
						}
					}
				}
			}
			return result;
		}

		private static TableOperator CreateTableOperator(Context context, Guid userInformationGuid)
		{
			UserInfoTable userInfoTable = DatabaseSchema.UserInfoTable(context.Database);
			KeyRange keyRange = new KeyRange(new StartStopKey(true, new object[]
			{
				userInformationGuid
			}), new StartStopKey(true, new object[]
			{
				userInformationGuid
			}));
			List<Column> list = new List<Column>(userInfoTable.Table.CommonColumns.Count + 1);
			list.AddRange(userInfoTable.Table.CommonColumns);
			list.Add(userInfoTable.UserGuid);
			return Factory.CreateTableOperator(context.Culture, context, userInfoTable.Table, userInfoTable.UserInfoPK, list, null, null, 0, 0, keyRange, false, false);
		}

		private static void ValidatePropertiesOnSet(Context context, Properties properties, bool moveMailbox)
		{
			foreach (Property property in properties)
			{
				if (property.Tag.PropInfo != null && property.Tag.IsCategory(3) && (!moveMailbox || !property.Tag.IsCategory(4)))
				{
					DiagnosticContext.TracePropTagError((LID)49916U, 2833U, property.Tag.PropTag);
					throw new StoreException((LID)41724U, ErrorCodeValue.UserInformationNoAccess);
				}
				if (property.Tag.IsNamedProperty)
				{
					DiagnosticContext.TracePropTagError((LID)63308U, 2834U, property.Tag.PropTag);
					throw new StoreException((LID)47948U, ErrorCodeValue.UserInformationPropertyError);
				}
				if (property.Value != null && property.Value is byte[] && ((byte[])property.Value).Length > 65536)
				{
					DiagnosticContext.TracePropTagError((LID)40252U, 2834U, property.Tag.PropTag);
					throw new StoreException((LID)56636U, ErrorCodeValue.UserInformationPropertyError);
				}
			}
		}

		private static void ValidatePropertiesOnGet(Context context, StorePropTag[] propertyTags)
		{
			foreach (StorePropTag storePropTag in propertyTags)
			{
				if (storePropTag.IsNamedProperty)
				{
					DiagnosticContext.TracePropTagError((LID)55116U, 2834U, storePropTag.PropTag);
					throw new StoreException((LID)35660U, ErrorCodeValue.UserInformationPropertyError);
				}
			}
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.UserInfo;
		}

		public override Context CurrentOperationContext
		{
			get
			{
				return this.currentOperationContext;
			}
		}

		public override ObjectPropertySchema Schema
		{
			get
			{
				if (this.propertySchema == null)
				{
					this.propertySchema = PropertySchema.GetObjectSchema(this.CurrentOperationContext.Database, ObjectType.UserInfo);
				}
				return this.propertySchema;
			}
		}

		public override ReplidGuidMap ReplidGuidMap
		{
			get
			{
				return null;
			}
		}

		protected override StorePropTag MapPropTag(Context context, uint propertyTag)
		{
			return WellKnownProperties.GetPropTag(propertyTag, ObjectType.UserInfo);
		}

		private static UserInformation.UserInformationLockFrame LockShared(Context context, Guid userInformationGuid)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!context.TransactionStarted, "Transaction leaked.");
			return new UserInformation.UserInformationLockFrame(new UserInformation.UserInformationLockName(userInformationGuid), true, context.Diagnostics);
		}

		private static UserInformation.UserInformationLockFrame LockExclusive(Context context, Guid userInformationGuid)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!context.TransactionStarted, "Transaction leaked.");
			return new UserInformation.UserInformationLockFrame(new UserInformation.UserInformationLockName(userInformationGuid), false, context.Diagnostics);
		}

		private const int MaxBinaryPropertySize = 65536;

		private static readonly TimeSpan lockTimeout = TimeSpan.FromSeconds(20.0);

		private static readonly HashSet<Guid> lockedEntryGuids = new HashSet<Guid>();

		private readonly Context currentOperationContext;

		private ObjectPropertySchema propertySchema;

		private enum UserInformationStatus
		{
			Invalid,
			Active,
			Disabled,
			SoftDeleted
		}

		private class UserInformationLockName : ILockName, IEquatable<ILockName>, IComparable<ILockName>
		{
			internal UserInformationLockName(Guid userInformationGuid)
			{
				this.userInformationGuid = userInformationGuid;
			}

			public LockManager.LockLevel LockLevel
			{
				get
				{
					return LockManager.LockLevel.UserInformation;
				}
			}

			public LockManager.NamedLockObject CachedLockObject
			{
				get
				{
					return this.cachedLockObject;
				}
				set
				{
					this.cachedLockObject = value;
				}
			}

			public virtual ILockName GetLockNameToCache()
			{
				return this;
			}

			public override bool Equals(object other)
			{
				return this.Equals(other as ILockName);
			}

			public virtual bool Equals(ILockName other)
			{
				return other != null && this.CompareTo(other) == 0;
			}

			public virtual int CompareTo(ILockName other)
			{
				int num = ((int)this.LockLevel).CompareTo((int)other.LockLevel);
				if (num == 0)
				{
					UserInformation.UserInformationLockName userInformationLockName = other as UserInformation.UserInformationLockName;
					num = this.userInformationGuid.CompareTo(userInformationLockName.userInformationGuid);
				}
				return num;
			}

			public override int GetHashCode()
			{
				return (int)(this.LockLevel ^ (LockManager.LockLevel)this.userInformationGuid.GetHashCode());
			}

			public override string ToString()
			{
				return "UserInformation " + this.userInformationGuid.ToString();
			}

			private readonly Guid userInformationGuid;

			private LockManager.NamedLockObject cachedLockObject;
		}

		private struct UserInformationLockFrame : IDisposable
		{
			internal UserInformationLockFrame(UserInformation.UserInformationLockName lockName, bool sharedLock, ILockStatistics lockStats)
			{
				if (!LockManager.TryGetLock(lockName, sharedLock ? LockManager.LockType.UserInformationShared : LockManager.LockType.UserInformationExclusive, UserInformation.lockTimeout, lockStats))
				{
					throw new StoreException((LID)48444U, ErrorCodeValue.UserInformationLockTimeout);
				}
				this.lockName = lockName;
				this.sharedLock = sharedLock;
			}

			public void Dispose()
			{
				if (this.lockName != null)
				{
					LockManager.ReleaseLock(this.lockName, this.sharedLock ? LockManager.LockType.UserInformationShared : LockManager.LockType.UserInformationExclusive);
					this.lockName = null;
				}
			}

			private UserInformation.UserInformationLockName lockName;

			private bool sharedLock;
		}
	}
}
