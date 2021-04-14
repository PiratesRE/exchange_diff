using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserConfigurationManager : IUserConfigurationManager
	{
		public UserConfigurationManager(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
			this.userConfigurationCache = new UserConfigurationCache(mailboxSession);
		}

		public static bool IsValidName(string configurationName)
		{
			if (configurationName == null)
			{
				throw new ArgumentNullException("configurationName");
			}
			return UserConfigurationName.IsValidName(configurationName, ConfigurationNameKind.Name);
		}

		public IReadableUserConfiguration GetReadOnlyMailboxConfiguration(string configName, UserConfigurationTypes freefetchDataTypes)
		{
			IReadableUserConfiguration result = null;
			if (!this.TryGetAggregatedConfiguration(() => UserConfigurationDescriptor.CreateMailboxDescriptor(configName, freefetchDataTypes), out result))
			{
				result = this.GetMailboxConfiguration(configName, freefetchDataTypes);
			}
			return result;
		}

		public IReadableUserConfiguration GetReadOnlyFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId)
		{
			IReadableUserConfiguration result = null;
			if (!this.TryGetAggregatedConfiguration(() => UserConfigurationDescriptor.CreateFolderDescriptor(configName, freefetchDataTypes, StoreId.GetStoreObjectId(folderId)), out result))
			{
				result = this.GetFolderConfiguration(configName, freefetchDataTypes, folderId);
			}
			return result;
		}

		IUserConfiguration IUserConfigurationManager.GetMailboxConfiguration(string configName, UserConfigurationTypes freefetchDataTypes)
		{
			return this.GetMailboxConfiguration(configName, freefetchDataTypes);
		}

		IUserConfiguration IUserConfigurationManager.GetFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId)
		{
			return this.GetFolderConfiguration(configName, freefetchDataTypes, folderId);
		}

		IUserConfiguration IUserConfigurationManager.CreateMailboxConfiguration(string configurationName, UserConfigurationTypes dataTypes)
		{
			return this.CreateMailboxConfiguration(configurationName, dataTypes);
		}

		IUserConfiguration IUserConfigurationManager.CreateFolderConfiguration(string configurationName, UserConfigurationTypes dataTypes, StoreId folderId)
		{
			return this.CreateFolderConfiguration(configurationName, dataTypes, folderId);
		}

		OperationResult IUserConfigurationManager.DeleteMailboxConfigurations(params string[] configurationNames)
		{
			return this.DeleteMailboxConfigurations(configurationNames);
		}

		OperationResult IUserConfigurationManager.DeleteFolderConfigurations(StoreId folderId, params string[] configurationNames)
		{
			return this.DeleteFolderConfigurations(folderId, configurationNames);
		}

		IList<IStorePropertyBag> IUserConfigurationManager.FetchAllConfigurations(IFolder folder, SortBy[] sorts, int maxRow, params PropertyDefinition[] columns)
		{
			return UserConfiguration.FetchAllConfigurations(folder, sorts, maxRow, columns);
		}

		IMailboxSession IUserConfigurationManager.MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		public UserConfiguration GetMailboxConfiguration(string configName, UserConfigurationTypes freefetchDataTypes)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(freefetchDataTypes, "freefetchDataTypes");
			return this.InternalGetUserConfiguration(this.GetDefaultFolderId(DefaultFolderType.Configuration), new UserConfigurationName(configName, ConfigurationNameKind.Name), freefetchDataTypes);
		}

		public UserConfiguration GetFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(freefetchDataTypes, "freefetchDataTypes");
			return this.InternalGetUserConfiguration(folderId, new UserConfigurationName(configName, ConfigurationNameKind.Name), freefetchDataTypes);
		}

		public UserConfiguration GetFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId, StoreObjectId messageId)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(freefetchDataTypes, "freefetchDataTypes");
			return this.InternalGetUserConfiguration(folderId, new UserConfigurationName(configName, ConfigurationNameKind.Name), freefetchDataTypes, messageId);
		}

		public OperationResult DeleteMailboxConfigurations(params string[] configurationNames)
		{
			if (configurationNames == null || configurationNames.Length == 0)
			{
				return OperationResult.Succeeded;
			}
			return this.DeleteFolderConfigurations(this.GetDefaultFolderId(DefaultFolderType.Configuration), configurationNames);
		}

		public OperationResult DeleteFolderConfigurations(StoreId folderId, params string[] configurationNames)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (configurationNames == null || configurationNames.Length == 0)
			{
				return OperationResult.Succeeded;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
			{
				for (int i = 0; i < configurationNames.Length; i++)
				{
					try
					{
						this.InternalDeleteUserConfiguration(folder, configurationNames[i]);
					}
					catch (InvalidOperationException arg)
					{
						stringBuilder.Append(string.Format("{0}: Failed to delete user configuration {1}. Exception = {2}\n", i, configurationNames[i], arg));
					}
				}
			}
			if (stringBuilder.Length != 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<StringBuilder>((long)this.GetHashCode(), "UserConfigurationManager::DeleteFolderConfigurations. Operation failed due to exception(s). Exception(s) = {0}.", stringBuilder);
				return OperationResult.PartiallySucceeded;
			}
			return OperationResult.Succeeded;
		}

		public ICollection<UserConfiguration> FindMailboxConfigurations(string searchString, UserConfigurationSearchFlags searchFlags)
		{
			ICollection<UserConfiguration> result;
			using (Folder folder = Folder.Bind(this.mailboxSession, this.GetDefaultFolderId(DefaultFolderType.Configuration)))
			{
				result = this.InternalFindUserConfigurations(folder, searchString, searchFlags);
			}
			return result;
		}

		public ICollection<UserConfiguration> FindFolderConfigurations(string searchString, UserConfigurationSearchFlags searchFlags, StoreId folderId)
		{
			ICollection<UserConfiguration> result;
			using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
			{
				result = this.InternalFindUserConfigurations(folder, searchString, searchFlags);
			}
			return result;
		}

		public UserConfiguration CreateMailboxConfiguration(string configurationName, UserConfigurationTypes dataTypes)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(dataTypes, "dataTypes");
			UserConfiguration result;
			using (Folder folder = Folder.Bind(this.mailboxSession, this.GetDefaultFolderId(DefaultFolderType.Configuration)))
			{
				result = this.InternalCreateUserConfiguration(folder, new UserConfigurationName(configurationName, ConfigurationNameKind.Name), dataTypes);
			}
			return result;
		}

		public UserConfiguration CreateFolderConfiguration(string configurationName, UserConfigurationTypes dataTypes, StoreId folderId)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(dataTypes, "dataTypes");
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			UserConfiguration result;
			using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
			{
				result = this.InternalCreateUserConfiguration(folder, new UserConfigurationName(configurationName, ConfigurationNameKind.Name), dataTypes);
			}
			return result;
		}

		public UserConfigurationManager.IAggregationContext AttachAggregator(AggregatedUserConfigurationDescriptor aggregatorDescription)
		{
			UserConfigurationManager.AggregationContext aggregationContext = null;
			if (ConfigurationItemSchema.IsEnabledForConfigurationAggregation(this.mailboxSession.MailboxOwner))
			{
				IAggregatedUserConfigurationReader reader = AggregatedUserConfiguration.GetReader(aggregatorDescription, this);
				aggregationContext = new UserConfigurationManager.AggregationContext(this, reader);
				lock (this.aggregators)
				{
					this.aggregators.Add(aggregationContext);
				}
			}
			return aggregationContext;
		}

		public UserConfigurationManager.IAggregationContext AttachAggregator(UserConfigurationManager.IAggregationContext ictx)
		{
			UserConfigurationManager.AggregationContext aggregationContext = null;
			if (ConfigurationItemSchema.IsEnabledForConfigurationAggregation(this.mailboxSession.MailboxOwner))
			{
				UserConfigurationManager.AggregationContext aggregationContext2 = ictx as UserConfigurationManager.AggregationContext;
				if (aggregationContext2 == null)
				{
					throw new ArgumentException("The shared context must be non-null and have been created by a UserConfigurationManager");
				}
				aggregationContext = aggregationContext2.Clone(this);
				lock (this.aggregators)
				{
					this.aggregators.Add(aggregationContext);
				}
			}
			return aggregationContext;
		}

		private StoreId GetDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			StoreId defaultFolderId = this.mailboxSession.GetDefaultFolderId(defaultFolderType);
			if (defaultFolderId == null)
			{
				throw new AccessDeniedException(ServerStrings.NotEnoughPermissionsToPerformOperation);
			}
			return defaultFolderId;
		}

		private ICollection<UserConfiguration> InternalFindUserConfigurations(Folder folder, string searchString, UserConfigurationSearchFlags searchFlags)
		{
			EnumValidator.ThrowIfInvalid<UserConfigurationSearchFlags>(searchFlags, "searchFlags");
			if (searchString == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Folder::FindUserConfigurations. Argument {0} is Null.", "searchString");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("searchString", 1));
			}
			return UserConfiguration.Find(folder, searchString, searchFlags);
		}

		private void InternalDeleteUserConfiguration(Folder folder, string configurationName)
		{
			ExTraceGlobals.StorageTracer.Information<string>((long)this.GetHashCode(), "Folder::DeleteUserConfiguration. configurationName = {0}.", (configurationName == null) ? "<Null>" : configurationName);
			if (configurationName == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Folder::DeleteUserConfiguration. Argument {0} is Null.", "configurationName");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("configurationName", 1));
			}
			if (configurationName.Length == 0)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Folder::DeleteUserConfiguration. Argument {0} is Empty.", "configurationName");
				throw new ArgumentException(ServerStrings.ExInvalidParameter("configurationName", 1));
			}
			UserConfiguration.Delete(folder, configurationName, UserConfigurationSearchFlags.FullString);
		}

		private UserConfiguration InternalGetUserConfiguration(StoreId folderId, UserConfigurationName configurationName, UserConfigurationTypes freefetchDataType)
		{
			return this.InternalGetUserConfiguration(folderId, configurationName, freefetchDataType, null);
		}

		private UserConfiguration InternalGetUserConfiguration(StoreId folderId, UserConfigurationName configurationName, UserConfigurationTypes freefetchDataType, StoreObjectId messageId)
		{
			if (this.aggregators.Count > 0)
			{
				lock (this.aggregators)
				{
					foreach (UserConfigurationManager.AggregationContext aggregationContext in this.aggregators)
					{
						ExTraceGlobals.StorageTracer.TraceWarning<UserConfigurationName>((long)this.GetHashCode(), "UserConfigurationManager::InternalGetUserConfiguration cache miss = {0}.", configurationName);
						aggregationContext.FaiCacheMiss();
					}
				}
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			UserConfiguration userConfiguration = null;
			bool flag2 = false;
			UserConfiguration result;
			try
			{
				userConfiguration = this.userConfigurationCache.Get(configurationName, StoreId.GetStoreObjectId(folderId));
				if (userConfiguration == null)
				{
					if (messageId != null)
					{
						userConfiguration = this.GetMessageConfiguration(configurationName, freefetchDataType, messageId);
					}
					if (userConfiguration == null)
					{
						ExTraceGlobals.UserConfigurationTracer.TraceDebug<string>((long)this.GetHashCode(), "UserConfigurationManager::InternalBindAndGetUserConfiguration. Miss the cache. ConfigName = {0}.", configurationName.Name);
						userConfiguration = this.InternalBindAndGetUserConfiguration(folderId, configurationName, freefetchDataType);
					}
				}
				if ((userConfiguration.DataTypes & freefetchDataType) == (UserConfigurationTypes)0)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "The configuration data's field has been corrupted. Field = UserConfigurationType.");
					throw new CorruptDataException(ServerStrings.ExConfigDataCorrupted("UserConfigurationType"));
				}
				flag2 = true;
				result = userConfiguration;
			}
			finally
			{
				if (!flag2 && userConfiguration != null)
				{
					userConfiguration.Dispose();
					userConfiguration = null;
				}
			}
			return result;
		}

		private UserConfiguration InternalCreateUserConfiguration(Folder folder, UserConfigurationName configurationName, UserConfigurationTypes dataTypes)
		{
			ExTraceGlobals.StorageTracer.Information<UserConfigurationName, UserConfigurationTypes>((long)this.GetHashCode(), "UserConfigurationManager::InternalCreateUserConfiguration. configurationName = {0}, dataTypes = {1}.", configurationName, dataTypes);
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (!EnumValidator.IsValidValue<UserConfigurationTypes>(dataTypes))
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "UserConfigurationManager::InternalCreateUserConfiguration. dataTypes is invalid.");
				throw new ArgumentException("dataTypes");
			}
			return UserConfiguration.Create(folder, configurationName, dataTypes);
		}

		private UserConfiguration GetMessageConfiguration(UserConfigurationName configurationName, UserConfigurationTypes freefetchDataTypes, StoreObjectId messageId)
		{
			UserConfiguration result = null;
			ConfigurationItem configurationItem = null;
			EnumValidator.ThrowIfInvalid<UserConfigurationTypes>(freefetchDataTypes, "freefetchDataTypes");
			try
			{
				configurationItem = ConfigurationItem.Bind(this.mailboxSession, messageId);
				result = new UserConfiguration(configurationItem, (StoreObjectId)configurationItem.TryGetProperty(StoreObjectSchema.ParentItemId), configurationName, freefetchDataTypes, true);
			}
			catch (ObjectNotFoundException arg)
			{
				result = null;
				if (configurationItem != null)
				{
					configurationItem.Dispose();
				}
				ExTraceGlobals.StorageTracer.TraceError<ObjectNotFoundException>(0L, "UserConfigurationManager::GetMessageConfiguration. Message object not found. Exception = {0}.", arg);
			}
			catch (Exception arg2)
			{
				result = null;
				if (configurationItem != null)
				{
					configurationItem.Dispose();
				}
				ExTraceGlobals.StorageTracer.TraceError<Exception>(0L, "UserConfigurationManager::GetMessageConfiguration. Unable to create user configuration. Exception = {0}.", arg2);
			}
			return result;
		}

		private UserConfiguration InternalBindAndGetUserConfiguration(StoreId folderId, UserConfigurationName configurationName, UserConfigurationTypes freefetchDataType)
		{
			UserConfiguration result = null;
			using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
			{
				if (folder.IsNew)
				{
					ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "UserConfigurationManager::InternalGetUserConfiguration. The folder Id is null maybe it is because the folder has not been saved yet. Id = {0}.", "null");
					throw new InvalidOperationException(ServerStrings.ExFolderWithoutMapiProp);
				}
				if (!EnumValidator.IsValidValue<UserConfigurationTypes>(freefetchDataType))
				{
					ExTraceGlobals.StorageTracer.TraceError<UserConfigurationTypes>((long)this.GetHashCode(), "UserConfigurationManager::InternalGetUserConfiguration. freefetchDataType is not allowed. freefetchDataType = {0}.", freefetchDataType);
					throw new ArgumentException("freefetchDataType");
				}
				ExTraceGlobals.UserConfigurationTracer.TraceDebug<UserConfigurationName>((long)this.GetHashCode(), "UserConfigurationManager::InternalGetUserConfiguration. Hit code GetIgnoringCache. ConfigName = {0}.", configurationName);
				result = UserConfiguration.GetIgnoringCache(this, folder, configurationName, freefetchDataType);
			}
			return result;
		}

		internal UserConfigurationCache UserConfigurationCache
		{
			get
			{
				return this.userConfigurationCache;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		private void RemoveAggregator(UserConfigurationManager.AggregationContext reader)
		{
			lock (this.aggregators)
			{
				this.aggregators.Remove(reader);
			}
		}

		private bool TryGetAggregatedConfiguration(Func<UserConfigurationDescriptor> descriptorFactory, out IReadableUserConfiguration config)
		{
			config = null;
			if (this.aggregators.Count > 0)
			{
				lock (this.aggregators)
				{
					UserConfigurationDescriptor descriptor = descriptorFactory();
					foreach (UserConfigurationManager.AggregationContext aggregationContext in this.aggregators)
					{
						config = aggregationContext.Read(this.mailboxSession, descriptor);
						if (config != null)
						{
							break;
						}
					}
				}
			}
			return null != config;
		}

		private UserConfigurationCache userConfigurationCache;

		private MailboxSession mailboxSession;

		private List<UserConfigurationManager.AggregationContext> aggregators = new List<UserConfigurationManager.AggregationContext>(8);

		public interface IAggregationContext : IDisposable
		{
			int FaiCacheHits { get; }

			int FaiCacheMisses { get; }

			int TypeCacheHits { get; }

			int TypeCacheMisses { get; }

			void Validate(IMailboxSession session, Action<IEnumerable<UserConfigurationDescriptor.MementoClass>, IEnumerable<string>> callback);

			T ReadType<T>(string key, Func<T> factory) where T : SerializableDataBase;

			void Detach();
		}

		private class AggregationStats
		{
			public int FaiCacheHits;

			public int FaiCacheMisses;

			public int TypeCacheHits;

			public int TypeCacheMisses;
		}

		private class AggregationContext : DisposableObject, UserConfigurationManager.IAggregationContext, IDisposable, IAggregationReValidator
		{
			public AggregationContext(UserConfigurationManager manager, IAggregatedUserConfigurationReader reader) : this(manager, reader, new UserConfigurationManager.AggregationStats(), new ConcurrentDictionary<string, Func<SerializableDataBase>>())
			{
			}

			private AggregationContext(UserConfigurationManager manager, IAggregatedUserConfigurationReader reader, UserConfigurationManager.AggregationStats stats, ConcurrentDictionary<string, Func<SerializableDataBase>> requestedTypes)
			{
				this.manager = manager;
				this.reader = reader;
				this.stats = stats;
				this.requestedTypes = requestedTypes;
				this.createdTypes = new ConcurrentDictionary<string, SerializableDataBase>();
			}

			public int FaiCacheHits
			{
				get
				{
					return this.stats.FaiCacheHits;
				}
			}

			public int FaiCacheMisses
			{
				get
				{
					return this.stats.FaiCacheMisses;
				}
			}

			public int TypeCacheHits
			{
				get
				{
					return this.stats.TypeCacheHits;
				}
			}

			public int TypeCacheMisses
			{
				get
				{
					return this.stats.TypeCacheMisses;
				}
			}

			public void FaiCacheHit()
			{
				Interlocked.Increment(ref this.stats.FaiCacheHits);
			}

			public void FaiCacheMiss()
			{
				Interlocked.Increment(ref this.stats.FaiCacheMisses);
			}

			public void TypeCacheHit()
			{
				Interlocked.Increment(ref this.stats.TypeCacheHits);
			}

			public void TypeCacheMiss()
			{
				Interlocked.Increment(ref this.stats.TypeCacheMisses);
			}

			public void Validate(IMailboxSession session, Action<IEnumerable<UserConfigurationDescriptor.MementoClass>, IEnumerable<string>> callback)
			{
				this.reader.Validate(session.UserConfigurationManager, XSOFactory.Default, this, callback);
			}

			public void Detach()
			{
				this.manager.RemoveAggregator(this);
			}

			public bool IsTypeReValidationRequired()
			{
				return this.requestedTypes.Count > 0;
			}

			public IEnumerable<KeyValuePair<string, SerializableDataBase>> RevalidatedTypes()
			{
				List<KeyValuePair<string, SerializableDataBase>> list = new List<KeyValuePair<string, SerializableDataBase>>(this.requestedTypes.Count);
				foreach (KeyValuePair<string, Func<SerializableDataBase>> keyValuePair in this.requestedTypes)
				{
					list.Add(new KeyValuePair<string, SerializableDataBase>(keyValuePair.Key, keyValuePair.Value()));
				}
				return list;
			}

			public T ReadType<T>(string key, Func<T> factory) where T : SerializableDataBase
			{
				this.requestedTypes[key] = factory;
				T t = default(T);
				if (this.reader.TryRead<T>(key, out t))
				{
					this.TypeCacheHit();
				}
				else
				{
					SerializableDataBase serializableDataBase = null;
					if (this.createdTypes.TryGetValue(key, out serializableDataBase))
					{
						t = (serializableDataBase as T);
						this.TypeCacheHit();
					}
					if (t == null)
					{
						t = factory();
						this.createdTypes[key] = t;
						this.TypeCacheMiss();
					}
				}
				return t;
			}

			public IReadableUserConfiguration Read(IMailboxSession session, UserConfigurationDescriptor descriptor)
			{
				IReadableUserConfiguration readableUserConfiguration = this.reader.Read(session, descriptor);
				if (readableUserConfiguration != null)
				{
					this.FaiCacheHit();
				}
				return readableUserConfiguration;
			}

			public UserConfigurationManager.AggregationContext Clone(UserConfigurationManager manager)
			{
				return new UserConfigurationManager.AggregationContext(manager, this.reader, this.stats, this.requestedTypes);
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<UserConfigurationManager.AggregationContext>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				base.InternalDispose(disposing);
				if (disposing)
				{
					this.Detach();
				}
			}

			private readonly UserConfigurationManager manager;

			private readonly IAggregatedUserConfigurationReader reader;

			private readonly UserConfigurationManager.AggregationStats stats;

			private readonly ConcurrentDictionary<string, Func<SerializableDataBase>> requestedTypes;

			private readonly ConcurrentDictionary<string, SerializableDataBase> createdTypes;
		}
	}
}
