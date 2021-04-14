using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AggregatedUserConfiguration : DisposableObject
	{
		private AggregatedUserConfiguration(AggregatedUserConfigurationDescriptor descriptor, IUserConfigurationManager manager)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.descriptor = descriptor;
				this.manager = manager;
				this.Load(out this.aggregatedConfiguration, out this.memento, out this.parts, out this.typeBag);
				this.RemoveOrphanedPendingUpdates();
				disposeGuard.Success();
			}
		}

		public Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> Parts
		{
			get
			{
				return this.parts;
			}
		}

		public Dictionary<string, SerializableDataBase> TypeBag
		{
			get
			{
				return this.typeBag;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AggregatedUserConfiguration>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			DisposeGuard.DisposeIfPresent(this.aggregatedConfiguration);
			base.InternalDispose(disposing);
		}

		private void Load(out IUserConfiguration initConfig, out AggregatedUserConfiguration.MementoClass initMemento, out Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> initParts, out Dictionary<string, SerializableDataBase> initTypeBag)
		{
			initConfig = null;
			initMemento = null;
			initParts = null;
			Func<IUserConfiguration> read = () => this.manager.GetMailboxConfiguration(this.descriptor.Name, UserConfigurationTypes.XML);
			Func<IUserConfiguration> rebuild = delegate()
			{
				this.manager.DeleteMailboxConfigurations(new string[]
				{
					this.descriptor.Name
				});
				return this.manager.CreateMailboxConfiguration(this.descriptor.Name, UserConfigurationTypes.XML);
			};
			this.TryInternalLoadConfiguration(read, rebuild, delegate(IUserConfiguration c)
			{
			}, out initConfig);
			initMemento = this.ReadMemento(initConfig);
			initParts = new Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>();
			foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart.MementoClass> keyValuePair in initMemento.Parts)
			{
				AggregatedUserConfigurationPart value = AggregatedUserConfigurationPart.FromMemento(keyValuePair.Value);
				initParts.Add(keyValuePair.Key, value);
			}
			initTypeBag = initMemento.TypeBag;
		}

		private AggregatedUserConfiguration.PendingUpdate PrepareUpdate(UserConfigurationDescriptor descriptor, ICoreObject item)
		{
			UserConfigurationDescriptor.MementoClass descriptorMemento = descriptor.ToMemento(this.manager.MailboxSession);
			if (this.memento.Pending.Exists((AggregatedUserConfiguration.PendingUpdate p) => descriptorMemento.Equals(p.DescriptorMemento)))
			{
				ExTraceGlobals.StorageTracer.TraceWarning<StoreObjectId, string>((long)this.GetHashCode(), "The core item with id=[{0}] and name=[{1}] already has a pending update.  It will be marked for rebuild.", item.StoreObjectId, descriptor.ConfigurationName);
				this.memento.ConcurrentUpdates.Add(descriptorMemento);
			}
			AggregatedUserConfiguration.PendingUpdate pendingUpdate = new AggregatedUserConfiguration.PendingUpdate
			{
				Guid = Guid.NewGuid().ToString(),
				PreparedUtc = ExDateTime.UtcNow.ToBinary(),
				Descriptor = descriptor,
				DescriptorMemento = descriptorMemento
			};
			this.memento.Pending.Add(pendingUpdate);
			this.Parts.Remove(descriptorMemento);
			this.memento.Parts.Remove(descriptorMemento);
			this.SaveMemento(AggregatedUserConfiguration.SaveFailureMode.Fail);
			return pendingUpdate;
		}

		private void CommitUpdate(AggregatedUserConfiguration.PendingUpdate pending)
		{
			bool flag = true;
			if (this.memento.ConcurrentUpdates.Contains(pending.DescriptorMemento))
			{
				flag = false;
				ExTraceGlobals.StorageTracer.TraceWarning<string>((long)this.GetHashCode(), "The configuration object with name[{0}] will not be committed because it is in the concurrent update list.", pending.Descriptor.ConfigurationName);
			}
			if (this.RemoveFromPendingAndConcurrent(pending) == 0)
			{
				flag = false;
				ExTraceGlobals.StorageTracer.TraceWarning<string>((long)this.GetHashCode(), "The configuration object with name[{0}] will not be committed because it was not found as a pending update.", pending.Descriptor.ConfigurationName);
			}
			if (flag)
			{
				IUserConfiguration userConfiguration = null;
				AggregatedUserConfigurationPart aggregatedUserConfigurationPart = null;
				try
				{
					if (this.TryInternalLoadConfiguration(() => pending.Descriptor.GetConfiguration(this.manager), null, delegate(IUserConfiguration c)
					{
						pending.Descriptor.Validate(c);
					}, out userConfiguration))
					{
						aggregatedUserConfigurationPart = AggregatedUserConfigurationPart.FromConfiguration(userConfiguration);
						this.memento.Parts.Add(pending.DescriptorMemento, aggregatedUserConfigurationPart.Memento);
						this.memento.FailedToLoad.Remove(pending.DescriptorMemento);
					}
				}
				finally
				{
					DisposeGuard.DisposeIfPresent(userConfiguration);
					DisposeGuard.DisposeIfPresent(aggregatedUserConfigurationPart);
				}
			}
			this.SaveMemento(AggregatedUserConfiguration.SaveFailureMode.Fail);
		}

		private void RemovePart(UserConfigurationDescriptor.MementoClass descriptor)
		{
			this.parts.Remove(descriptor);
			this.memento.Parts.Remove(descriptor);
		}

		private void RemoveFromFailedToLoad(UserConfigurationDescriptor.MementoClass descriptor)
		{
			this.memento.FailedToLoad.Remove(descriptor);
		}

		internal void SetType(string key, SerializableDataBase value)
		{
			this.memento.TypeBag[key] = value;
			this.typeBag[key] = value;
		}

		private int RemoveFromPendingAndConcurrent(AggregatedUserConfiguration.PendingUpdate pending)
		{
			int result = this.memento.Pending.RemoveAll((AggregatedUserConfiguration.PendingUpdate p) => pending.Guid.Equals(p.Guid));
			if (!this.memento.Pending.Exists((AggregatedUserConfiguration.PendingUpdate p) => pending.DescriptorMemento.Equals(p.DescriptorMemento)))
			{
				this.memento.ConcurrentUpdates.Remove(pending.DescriptorMemento);
			}
			return result;
		}

		private AggregatedUserConfiguration.MementoClass ReadMemento(IUserConfiguration aggregated)
		{
			AggregatedUserConfiguration.MementoClass mementoClass = null;
			try
			{
				if (aggregated != null)
				{
					DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(AggregatedUserConfiguration.MementoClass));
					using (Stream xmlStream = aggregated.GetXmlStream())
					{
						if (xmlStream != null && xmlStream.Length > 0L)
						{
							mementoClass = (AggregatedUserConfiguration.MementoClass)dataContractSerializer.ReadObject(xmlStream);
						}
					}
				}
			}
			catch (SerializationException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<SerializationException>((long)this.GetHashCode(), "The aggregated configuration exists, but it could not be deserialized into a memento", arg);
			}
			catch (XmlException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceError<XmlException>((long)this.GetHashCode(), "The aggregated configuration exists, but it could not be deserialized memento", arg2);
			}
			catch (ArgumentException arg3)
			{
				ExTraceGlobals.StorageTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "The aggregated configuration exists, but it could not be deserialized into a memento", arg3);
			}
			if (mementoClass == null || 1 != mementoClass.Version)
			{
				mementoClass = (mementoClass ?? new AggregatedUserConfiguration.MementoClass());
			}
			mementoClass.Version = 1;
			mementoClass.Parts = (mementoClass.Parts ?? new Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart.MementoClass>());
			mementoClass.Pending = (mementoClass.Pending ?? new List<AggregatedUserConfiguration.PendingUpdate>());
			mementoClass.ConcurrentUpdates = (mementoClass.ConcurrentUpdates ?? new HashSet<UserConfigurationDescriptor.MementoClass>());
			mementoClass.FailedToLoad = (mementoClass.FailedToLoad ?? new HashSet<UserConfigurationDescriptor.MementoClass>());
			mementoClass.TypeBag = (mementoClass.TypeBag ?? new Dictionary<string, SerializableDataBase>());
			return mementoClass;
		}

		private void SaveMemento(AggregatedUserConfiguration.SaveFailureMode failMode)
		{
			if (this.aggregatedConfiguration != null)
			{
				DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(AggregatedUserConfiguration.MementoClass));
				using (Stream xmlStream = this.aggregatedConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					dataContractSerializer.WriteObject(xmlStream, this.memento);
				}
				try
				{
					this.aggregatedConfiguration.Save(SaveMode.FailOnAnyConflict);
				}
				catch (AccessDeniedException arg)
				{
					ExTraceGlobals.StorageTracer.TraceError<AccessDeniedException>((long)this.GetHashCode(), "The aggregated configuration failed to save due to access denied exception", arg);
					if (failMode == AggregatedUserConfiguration.SaveFailureMode.Fail)
					{
						throw;
					}
				}
				catch (QuotaExceededException arg2)
				{
					ExTraceGlobals.StorageTracer.TraceError<QuotaExceededException>((long)this.GetHashCode(), "The aggregated configuration failed to save due to quota exceeded exception", arg2);
					if (failMode == AggregatedUserConfiguration.SaveFailureMode.Fail)
					{
						throw;
					}
				}
				catch (SaveConflictException arg3)
				{
					ExTraceGlobals.StorageTracer.TraceError<SaveConflictException>((long)this.GetHashCode(), "The aggregated configuration failed to save due to a save conflict exception", arg3);
					if (failMode == AggregatedUserConfiguration.SaveFailureMode.Fail)
					{
						throw;
					}
				}
				catch (ObjectExistedException arg4)
				{
					ExTraceGlobals.StorageTracer.TraceError<ObjectExistedException>((long)this.GetHashCode(), "The aggregated configuration failed to save due to an object existed exception", arg4);
					if (failMode == AggregatedUserConfiguration.SaveFailureMode.Fail)
					{
						throw;
					}
				}
			}
		}

		private void RemoveOrphanedPendingUpdates()
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			List<AggregatedUserConfiguration.PendingUpdate> list = this.memento.Pending.Where(delegate(AggregatedUserConfiguration.PendingUpdate p)
			{
				ExDateTime value = ExDateTime.FromBinary(p.PreparedUtc);
				return utcNow.Subtract(value) > AggregatedUserConfiguration.OrphanedThreshold;
			}).ToList<AggregatedUserConfiguration.PendingUpdate>();
			foreach (AggregatedUserConfiguration.PendingUpdate pending in list)
			{
				this.RemoveFromPendingAndConcurrent(pending);
			}
		}

		private void InitializeMissingParts()
		{
			bool flag = false;
			IEnumerable<UserConfigurationDescriptor> enumerable = Enumerable.Empty<UserConfigurationDescriptor>();
			if (this.aggregatedConfiguration != null)
			{
				enumerable = from d in this.descriptor.Sources
				where !this.parts.ContainsKey(d.ToMemento(this.manager.MailboxSession))
				select d;
			}
			using (IEnumerator<UserConfigurationDescriptor> enumerator = enumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AggregatedUserConfiguration.<>c__DisplayClass1c CS$<>8__locals1 = new AggregatedUserConfiguration.<>c__DisplayClass1c();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.missingDescriptor = enumerator.Current;
					UserConfigurationDescriptor.MementoClass missingDescriptorMemento = CS$<>8__locals1.missingDescriptor.ToMemento(this.manager.MailboxSession);
					if (this.memento.FailedToLoad.Contains(missingDescriptorMemento))
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "the missing configuration part with name=[{0}] failed to load once already and will not be initialized again", CS$<>8__locals1.missingDescriptor.ConfigurationName);
					}
					else
					{
						IUserConfiguration userConfiguration = null;
						try
						{
							if (!this.TryInternalLoadConfiguration(() => CS$<>8__locals1.missingDescriptor.GetConfiguration(CS$<>8__locals1.<>4__this.manager), null, delegate(IUserConfiguration c)
							{
								CS$<>8__locals1.missingDescriptor.Validate(c);
							}, out userConfiguration))
							{
								ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "the missing configuration part with name=[{0}] failed to load", CS$<>8__locals1.missingDescriptor.ConfigurationName);
								this.memento.FailedToLoad.Add(missingDescriptorMemento);
								flag = true;
							}
							else
							{
								AggregatedUserConfigurationPart aggregatedUserConfigurationPart = AggregatedUserConfigurationPart.FromConfiguration(userConfiguration);
								this.parts[missingDescriptorMemento] = aggregatedUserConfigurationPart;
								if (this.memento.Pending.Exists((AggregatedUserConfiguration.PendingUpdate p) => missingDescriptorMemento.Equals(p.DescriptorMemento)))
								{
									ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "the missing configuration part with name=[{0}] will not be persisted because it has a pending update", CS$<>8__locals1.missingDescriptor.ConfigurationName);
								}
								else
								{
									ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "adding missing part {0} to the persisted aggregated configuration", CS$<>8__locals1.missingDescriptor.ConfigurationName);
									flag = true;
									this.memento.Parts.Add(missingDescriptorMemento, aggregatedUserConfigurationPart.Memento);
								}
							}
						}
						finally
						{
							DisposeGuard.DisposeIfPresent(userConfiguration);
						}
					}
				}
			}
			if (flag)
			{
				this.SaveMemento(AggregatedUserConfiguration.SaveFailureMode.Ignore);
			}
		}

		private bool TryInternalLoadConfiguration(Func<IUserConfiguration> read, Func<IUserConfiguration> rebuild, Action<IUserConfiguration> validate, out IUserConfiguration userConfiguration)
		{
			IUserConfiguration tmp = null;
			Exception ex = this.HandleCommonUserConfigurationExceptions(delegate
			{
				tmp = read();
				validate(tmp);
			});
			if (ex != null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Unable to read the configuration. {0}", ex);
				if (rebuild != null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "Creating a new configuration in its place.");
					ex = this.HandleCommonUserConfigurationExceptions(delegate
					{
						try
						{
							tmp = rebuild();
						}
						catch (ObjectExistedException)
						{
							ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "failed to create a configuration because it already existed");
							tmp = read();
						}
					});
					ExTraceGlobals.StorageTracer.TraceDebug<bool>((long)this.GetHashCode(), "recreate success? {0}", null != ex);
				}
			}
			userConfiguration = tmp;
			return null != userConfiguration;
		}

		private Exception HandleCommonUserConfigurationExceptions(Action code)
		{
			Exception result = null;
			try
			{
				code();
				result = null;
			}
			catch (InvalidDataException ex)
			{
				ExTraceGlobals.StorageTracer.TraceError<InvalidDataException>((long)this.GetHashCode(), "The user configuration exists but is corrupt", ex);
				result = ex;
			}
			catch (CorruptDataException ex2)
			{
				ExTraceGlobals.StorageTracer.TraceError<CorruptDataException>((long)this.GetHashCode(), "The user configuration exists but is corrupt", ex2);
				result = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				ExTraceGlobals.StorageTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "The user configuration exists but is corrupt", ex3);
				result = ex3;
			}
			catch (ObjectNotFoundException ex4)
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "The user configuration does not exist.");
				result = ex4;
			}
			catch (StoragePermanentException ex5)
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "The user configuration failed to load with a permanent exception.");
				result = ex5;
			}
			catch (Exception ex6)
			{
				ExTraceGlobals.StorageTracer.TraceError<Exception>((long)this.GetHashCode(), "Unknown user configuration error not being handled ", ex6);
				result = ex6;
				throw;
			}
			return result;
		}

		public static IAggregatedUserConfigurationReader GetReader(AggregatedUserConfigurationDescriptor descriptor, IUserConfigurationManager manager)
		{
			return new AggregatedUserConfiguration.Reader(descriptor, manager);
		}

		public static IList<IAggregatedUserConfigurationWriter> GetWriters(IAggregatedUserConfigurationSchema schema, IMailboxSession mailboxSession, ICoreItem item)
		{
			IList<IAggregatedUserConfigurationWriter> result = null;
			if (AggregatedUserConfiguration.IsConfigurationObject(item) && mailboxSession != null && mailboxSession.Capabilities.CanHaveUserConfigurationManager && mailboxSession.UserConfigurationManager != null)
			{
				AggregatedUserConfiguration.EnsureRequiredDescriptorPropertyDefinitions(item);
				UserConfigurationDescriptor configurationDescriptor = null;
				if (AggregatedUserConfiguration.TryGetUserConfigurationDescriptor((StorePropertyDefinition prop) => item.PropertyBag.TryGetProperty(prop), item.PropertyBag.GetValueOrDefault<StoreObjectId>(InternalSchema.ParentItemId), out configurationDescriptor))
				{
					IUserConfigurationManager configManager = mailboxSession.UserConfigurationManager;
					UserConfigurationDescriptor.UserConfigurationDescriptorEqualityComparer comparer = new UserConfigurationDescriptor.UserConfigurationDescriptorEqualityComparer(mailboxSession);
					result = (from s in schema.All
					where s.Sources.Contains(configurationDescriptor, comparer)
					select new AggregatedUserConfiguration.Writer(s, configurationDescriptor, item, configManager)).ToList<IAggregatedUserConfigurationWriter>();
				}
			}
			return result;
		}

		private static bool IsConfigurationObject(ICoreItem item)
		{
			bool result = false;
			IDirectPropertyBag directPropertyBag = item.PropertyBag as IDirectPropertyBag;
			if (directPropertyBag != null && directPropertyBag.IsLoaded(InternalSchema.ItemClass))
			{
				result = UserConfigurationName.IsValidName(item.ClassName(), ConfigurationNameKind.ItemClass);
			}
			return result;
		}

		private static bool TryGetUserConfigurationDescriptor(Func<StorePropertyDefinition, object> propertyGetter, StoreObjectId parentId, out UserConfigurationDescriptor descriptor)
		{
			bool result = false;
			descriptor = null;
			UserConfigurationName userConfigurationName = null;
			UserConfigurationTypes types = (UserConfigurationTypes)0;
			if (AggregatedUserConfiguration.TryGetUserConfigurationName(propertyGetter, out userConfigurationName) && AggregatedUserConfiguration.TryGetUserConfigurationType(propertyGetter, out types))
			{
				descriptor = UserConfigurationDescriptor.CreateFolderDescriptor(userConfigurationName.Name, types, parentId);
				result = true;
			}
			return result;
		}

		private static void EnsureRequiredDescriptorPropertyDefinitions(ICoreItem item)
		{
			IDirectPropertyBag directPropertyBag = (IDirectPropertyBag)item.PropertyBag;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			if (!directPropertyBag.IsLoaded(InternalSchema.UserConfigurationType))
			{
				list.Add(InternalSchema.UserConfigurationType);
			}
			if (list.Count > 0)
			{
				item.PropertyBag.Load(list);
			}
		}

		private static bool TryGetUserConfigurationType(Func<StorePropertyDefinition, object> propertyGetter, out UserConfigurationTypes configType)
		{
			bool result = false;
			configType = (UserConfigurationTypes)0;
			object obj = propertyGetter(InternalSchema.UserConfigurationType);
			if (!PropertyError.IsPropertyError(obj))
			{
				try
				{
					configType = UserConfiguration.CheckUserConfigurationType((int)obj);
					result = true;
				}
				catch (CorruptDataException arg)
				{
					ExTraceGlobals.StorageTracer.TraceError<CorruptDataException>(0L, "could not read the user configuration type because it is corrupt", arg);
				}
			}
			return result;
		}

		private static bool TryGetUserConfigurationName(Func<StorePropertyDefinition, object> propertyGetter, out UserConfigurationName configName)
		{
			bool result = false;
			configName = null;
			object obj = propertyGetter(InternalSchema.ItemClass);
			if (!PropertyError.IsPropertyError(obj))
			{
				string text = obj as string;
				if (!string.IsNullOrEmpty(text) && UserConfigurationName.IsValidName(text, ConfigurationNameKind.ItemClass))
				{
					configName = new UserConfigurationName(text, ConfigurationNameKind.ItemClass);
					result = true;
				}
			}
			return result;
		}

		public const int Version = 1;

		public static readonly TimeSpan OrphanedThreshold = TimeSpan.FromHours(1.0);

		private readonly AggregatedUserConfigurationDescriptor descriptor;

		private readonly IUserConfigurationManager manager;

		private readonly IUserConfiguration aggregatedConfiguration;

		private readonly AggregatedUserConfiguration.MementoClass memento;

		private readonly Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> parts;

		private readonly Dictionary<string, SerializableDataBase> typeBag;

		private enum SaveFailureMode
		{
			Ignore,
			Fail
		}

		private class Reader : IAggregatedUserConfigurationReader
		{
			public Reader(AggregatedUserConfigurationDescriptor descriptor, IUserConfigurationManager manager)
			{
				this.descriptor = descriptor;
				this.sources = new HashSet<UserConfigurationDescriptor.MementoClass>();
				this.removeFromFailedToLoad = new HashSet<UserConfigurationDescriptor.MementoClass>();
				foreach (UserConfigurationDescriptor userConfigurationDescriptor in this.descriptor.Sources)
				{
					this.sources.Add(userConfigurationDescriptor.ToMemento(manager.MailboxSession));
				}
				this.LoadAndInitializeAggregator(manager, out this.parts, out this.bag);
			}

			public IReadableUserConfiguration Read(IMailboxSession session, UserConfigurationDescriptor descriptor)
			{
				IReadableUserConfiguration readableUserConfiguration = null;
				UserConfigurationDescriptor.MementoClass mementoClass = descriptor.ToMemento(session);
				if (this.IsAggregating(mementoClass))
				{
					AggregatedUserConfigurationPart aggregatedUserConfigurationPart = null;
					if (this.parts.TryGetValue(mementoClass, out aggregatedUserConfigurationPart))
					{
						readableUserConfiguration = aggregatedUserConfigurationPart;
					}
					else
					{
						readableUserConfiguration = descriptor.GetConfiguration(session.UserConfigurationManager);
						if (readableUserConfiguration != null)
						{
							this.removeFromFailedToLoad.Add(mementoClass);
						}
					}
				}
				return readableUserConfiguration;
			}

			public bool TryRead<T>(string key, out T result) where T : SerializableDataBase
			{
				SerializableDataBase serializableDataBase = null;
				bool result2 = this.bag.TryGetValue(key, out serializableDataBase);
				result = (serializableDataBase as T);
				return result2;
			}

			public void Validate(IUserConfigurationManager manager, IXSOFactory xsoFactory, IAggregationReValidator validator, Action<IEnumerable<UserConfigurationDescriptor.MementoClass>, IEnumerable<string>> callback)
			{
				List<UserConfigurationDescriptor.MementoClass> list = new List<UserConfigurationDescriptor.MementoClass>();
				List<string> list2 = new List<string>();
				Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId> persistedSourceVersions = this.GetPersistedSourceVersions(manager, xsoFactory);
				List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> sourcesWithWrongVersion = this.GetSourcesWithWrongVersion(persistedSourceVersions);
				List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> deletedSources = this.GetDeletedSources(persistedSourceVersions);
				if (sourcesWithWrongVersion.Count + deletedSources.Count + this.removeFromFailedToLoad.Count > 0 || validator.IsTypeReValidationRequired())
				{
					using (AggregatedUserConfiguration aggregatedUserConfiguration = new AggregatedUserConfiguration(this.descriptor, manager))
					{
						foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> keyValuePair in deletedSources)
						{
							aggregatedUserConfiguration.RemovePart(keyValuePair.Key);
							list.Add(keyValuePair.Key);
						}
						foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> keyValuePair2 in sourcesWithWrongVersion)
						{
							AggregatedUserConfigurationPart aggregatedUserConfigurationPart = null;
							if (aggregatedUserConfiguration.Parts.TryGetValue(keyValuePair2.Key, out aggregatedUserConfigurationPart) && aggregatedUserConfigurationPart.VersionedId.Equals(keyValuePair2.Value.VersionedId))
							{
								ExTraceGlobals.StorageTracer.TraceWarning<string>((long)this.GetHashCode(), "The configuration name =[{0}] source has changed and the aggregator has not been updated.", keyValuePair2.Key.ConfigurationName);
								aggregatedUserConfiguration.RemovePart(keyValuePair2.Key);
								list.Add(keyValuePair2.Key);
							}
						}
						foreach (UserConfigurationDescriptor.MementoClass mementoClass in this.removeFromFailedToLoad)
						{
							aggregatedUserConfiguration.RemoveFromFailedToLoad(mementoClass);
						}
						IEnumerable<KeyValuePair<string, SerializableDataBase>> revalidatedTypes = validator.RevalidatedTypes();
						int num = this.UpdateValidatedTypes(revalidatedTypes, aggregatedUserConfiguration, list2);
						if (list.Count > 0 || num > 0)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<int, int>((long)this.GetHashCode(), "invalid configuration count: {0}.  updated type count {1}.", list.Count, num);
							aggregatedUserConfiguration.SaveMemento(AggregatedUserConfiguration.SaveFailureMode.Ignore);
						}
					}
				}
				callback(list, list2);
			}

			private int UpdateValidatedTypes(IEnumerable<KeyValuePair<string, SerializableDataBase>> revalidatedTypes, AggregatedUserConfiguration aggregated, List<string> invalidTypes)
			{
				int num = 0;
				if (revalidatedTypes != null)
				{
					foreach (KeyValuePair<string, SerializableDataBase> keyValuePair in revalidatedTypes)
					{
						string key = keyValuePair.Key;
						SerializableDataBase value = keyValuePair.Value;
						SerializableDataBase serializableDataBase = null;
						if (!this.TryRead<SerializableDataBase>(keyValuePair.Key, out serializableDataBase))
						{
							aggregated.SetType(key, value);
							num++;
						}
						else if (value == null && serializableDataBase != null)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "the type {0} was aggregated non-null, but now is null", key);
							aggregated.SetType(key, null);
							invalidTypes.Add(key);
							num++;
						}
						else if (value != null && !value.Equals(serializableDataBase))
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "the type {0} has changed since we aggregated it", key);
							aggregated.SetType(key, value);
							invalidTypes.Add(key);
							num++;
						}
					}
				}
				return num;
			}

			private List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> GetSourcesWithWrongVersion(Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId> sources)
			{
				List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> list = new List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>>();
				foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> item in this.parts)
				{
					VersionedId other = null;
					if (sources.TryGetValue(item.Key, out other) && !item.Value.VersionedId.Equals(other))
					{
						list.Add(item);
						ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "The configuration name =[{0}] has a different versioned id than the discovered source.  It will be removed from aggregator unless its been updated in the aggregator, too", item.Key.ConfigurationName);
					}
				}
				return list;
			}

			private List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> GetDeletedSources(Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId> sources)
			{
				List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>> list = new List<KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart>>();
				foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> item in this.parts)
				{
					UserConfigurationDescriptor.MementoClass key = item.Key;
					bool flag = false;
					foreach (KeyValuePair<UserConfigurationDescriptor.MementoClass, VersionedId> keyValuePair in sources)
					{
						UserConfigurationDescriptor.MementoClass key2 = keyValuePair.Key;
						if (key2.IsSuperSetOf(key))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(item);
						ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "The configuration name =[{0}] has been deleted.  It will be removed from the aggregator", item.Key.ConfigurationName);
					}
				}
				return list;
			}

			private Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId> GetPersistedSourceVersions(IUserConfigurationManager manager, IXSOFactory xsoFactory)
			{
				Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId> dictionary = new Dictionary<UserConfigurationDescriptor.MementoClass, VersionedId>();
				IEnumerable<StoreObjectId> enumerable = (from p in this.parts
				select p.Key.FolderId).Distinct<StoreObjectId>();
				PropertyDefinition[] columns = new PropertyDefinition[]
				{
					InternalSchema.ItemClass,
					InternalSchema.ItemId,
					InternalSchema.UserConfigurationType,
					InternalSchema.LastModifiedTime
				};
				foreach (StoreObjectId storeObjectId in enumerable)
				{
					using (IFolder folder = xsoFactory.BindToFolder(manager.MailboxSession, storeObjectId))
					{
						IList<IStorePropertyBag> list = manager.FetchAllConfigurations(folder, null, 10000, columns);
						using (IEnumerator<IStorePropertyBag> enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IStorePropertyBag item = enumerator2.Current;
								UserConfigurationDescriptor userConfigurationDescriptor = null;
								if (AggregatedUserConfiguration.TryGetUserConfigurationDescriptor((StorePropertyDefinition prop) => item.TryGetProperty(prop), storeObjectId, out userConfigurationDescriptor))
								{
									VersionedId versionedId = (VersionedId)item.TryGetProperty(InternalSchema.ItemId);
									if (!PropertyError.IsPropertyError(versionedId))
									{
										dictionary.Add(userConfigurationDescriptor.ToMemento(manager.MailboxSession), versionedId);
									}
								}
							}
						}
					}
				}
				return dictionary;
			}

			private bool IsAggregating(UserConfigurationDescriptor.MementoClass descriptor)
			{
				return this.sources.Contains(descriptor);
			}

			private void LoadAndInitializeAggregator(IUserConfigurationManager manager, out Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> parts, out Dictionary<string, SerializableDataBase> bag)
			{
				using (AggregatedUserConfiguration aggregatedUserConfiguration = new AggregatedUserConfiguration(this.descriptor, manager))
				{
					aggregatedUserConfiguration.InitializeMissingParts();
					parts = aggregatedUserConfiguration.Parts;
					bag = aggregatedUserConfiguration.TypeBag;
				}
			}

			private readonly AggregatedUserConfigurationDescriptor descriptor;

			private readonly Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart> parts;

			private readonly HashSet<UserConfigurationDescriptor.MementoClass> sources;

			private readonly Dictionary<string, SerializableDataBase> bag;

			private readonly HashSet<UserConfigurationDescriptor.MementoClass> removeFromFailedToLoad;
		}

		private class Writer : IAggregatedUserConfigurationWriter
		{
			public Writer(AggregatedUserConfigurationDescriptor aggregatorDescriptor, UserConfigurationDescriptor itemDescriptor, ICoreObject item, IUserConfigurationManager manager)
			{
				this.aggregatorDescriptor = aggregatorDescriptor;
				this.itemDescriptor = itemDescriptor;
				this.item = item;
				this.manager = manager;
			}

			public void Prepare()
			{
				using (AggregatedUserConfiguration aggregatedUserConfiguration = new AggregatedUserConfiguration(this.aggregatorDescriptor, this.manager))
				{
					this.pendingUpdate = aggregatedUserConfiguration.PrepareUpdate(this.itemDescriptor, this.item);
				}
			}

			public void Commit()
			{
				if (this.pendingUpdate == null)
				{
					throw new InvalidOperationException("you cannot commit an aggregated configuration update without first preparing it.");
				}
				using (AggregatedUserConfiguration aggregatedUserConfiguration = new AggregatedUserConfiguration(this.aggregatorDescriptor, this.manager))
				{
					aggregatedUserConfiguration.CommitUpdate(this.pendingUpdate);
					this.pendingUpdate = null;
				}
			}

			private readonly AggregatedUserConfigurationDescriptor aggregatorDescriptor;

			private readonly UserConfigurationDescriptor itemDescriptor;

			private readonly ICoreObject item;

			private readonly IUserConfigurationManager manager;

			private AggregatedUserConfiguration.PendingUpdate pendingUpdate;
		}

		[DataContract]
		public class PendingUpdate
		{
			[DataMember]
			public string Guid { get; set; }

			[DataMember]
			public long PreparedUtc { get; set; }

			[DataMember]
			public UserConfigurationDescriptor.MementoClass DescriptorMemento { get; set; }

			[IgnoreDataMember]
			public UserConfigurationDescriptor Descriptor
			{
				get
				{
					if (this.descriptor == null && this.DescriptorMemento != null)
					{
						this.descriptor = UserConfigurationDescriptor.FromMemento(this.DescriptorMemento);
					}
					return this.descriptor;
				}
				set
				{
					this.descriptor = value;
				}
			}

			private UserConfigurationDescriptor descriptor;
		}

		[DataContract]
		public class MementoClass
		{
			[DataMember]
			public int Version { get; set; }

			[DataMember]
			public Dictionary<UserConfigurationDescriptor.MementoClass, AggregatedUserConfigurationPart.MementoClass> Parts { get; set; }

			[DataMember]
			public HashSet<UserConfigurationDescriptor.MementoClass> ConcurrentUpdates { get; set; }

			[DataMember]
			public List<AggregatedUserConfiguration.PendingUpdate> Pending { get; set; }

			[DataMember]
			public HashSet<UserConfigurationDescriptor.MementoClass> FailedToLoad { get; set; }

			[DataMember]
			public Dictionary<string, SerializableDataBase> TypeBag { get; set; }
		}
	}
}
