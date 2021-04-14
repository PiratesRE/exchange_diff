using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	public class ProvisioningCache
	{
		protected ProvisioningCache(string identification, bool enabled)
		{
			this.InitializeSettings(identification, enabled);
		}

		protected ProvisioningCache()
		{
			this.CreateBasicCache();
		}

		public static ProvisioningCache Instance
		{
			get
			{
				if (!ProvisioningCache.appRegistryInitialized)
				{
					if (ProvisioningCache.disabledInstance == null)
					{
						lock (ProvisioningCache.disabledInstanceLock)
						{
							if (ProvisioningCache.disabledInstance == null)
							{
								ProvisioningCache.disabledInstance = new ProvisioningCache(null, false);
							}
						}
					}
					return ProvisioningCache.disabledInstance;
				}
				if (ProvisioningCache.cacheInstance == null)
				{
					lock (ProvisioningCache.cacheInstanceLock)
					{
						if (ProvisioningCache.cacheInstance == null)
						{
							ProvisioningCache.cacheInstance = new ProvisioningCache(ProvisioningCache.cacheIdentification, ProvisioningCache.configuredEnable);
						}
					}
				}
				return ProvisioningCache.cacheInstance;
			}
		}

		private static MSExchangeProvisioningCacheInstance PerfCounters
		{
			get
			{
				return ProvisioningCache.perfCounters;
			}
		}

		private static void IncreaseCacheHitCount()
		{
			ProvisioningCache.IncreaseCacheHitOrMissCount(CmdletThreadStaticData.CacheHitCount, delegate(int value)
			{
				CmdletThreadStaticData.CacheHitCount = new int?(value);
			}, RpsCmdletMetadata.CacheHitCount);
		}

		private static void IncreaseCacheMissCount()
		{
			ProvisioningCache.IncreaseCacheHitOrMissCount(CmdletThreadStaticData.CacheMissCount, delegate(int value)
			{
				CmdletThreadStaticData.CacheMissCount = new int?(value);
			}, RpsCmdletMetadata.CacheMissCount);
		}

		private static void IncreaseCacheHitOrMissCount(int? valueGetter, Action<int> valueSetter, Enum logMetadata)
		{
			int? num = valueGetter;
			if (num == null)
			{
				return;
			}
			int num2 = num.Value + 1;
			valueSetter(num2);
			CmdletLogger.SafeSetLogger(logMetadata, num2);
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		private static void GetAppRegistry(string identification, out Dictionary<Guid, TimeSpan> configuredExpirations, out bool enabled)
		{
			configuredExpirations = null;
			enabled = false;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ProvisioningCache.MSExchangeProvisioningCacheRegistryPath))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(ProvisioningCache.ProvisioningCacheExpiration))
					{
						if (registryKey2 != null)
						{
							string text = registryKey2.GetValue(identification) as string;
							if (!string.IsNullOrWhiteSpace(text))
							{
								configuredExpirations = ProvisioningCache.ConvertToExpirations(text);
							}
						}
					}
					object value = registryKey.GetValue(ProvisioningCache.ProvisioningCacheEnabled);
					enabled = (value == null || StringComparer.OrdinalIgnoreCase.Equals("true", value.ToString()));
				}
			}
			catch (ArgumentNullException ex)
			{
				ProvisioningCache.ProvisioningCacheInitializationFailedHandler(identification, ex.Message);
			}
			catch (ObjectDisposedException ex2)
			{
				ProvisioningCache.ProvisioningCacheInitializationFailedHandler(identification, ex2.Message);
			}
			catch (SecurityException ex3)
			{
				ProvisioningCache.ProvisioningCacheInitializationFailedHandler(identification, ex3.Message);
			}
			catch (ArgumentException ex4)
			{
				ProvisioningCache.ProvisioningCacheInitializationFailedHandler(identification, ex4.Message);
			}
			catch (Exception ex5)
			{
				ProvisioningCache.ProvisioningCacheInitializationFailedHandler(identification, ex5.Message);
			}
		}

		private static void ProvisioningCacheInitializationFailedHandler(string identification, string errorMsg)
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCProvisioningCacheInitializationFailed, identification, new object[]
			{
				errorMsg
			});
		}

		public static void InitializeAppRegistrySettings(string identification)
		{
			if (ProvisioningCache.appRegistryInitialized)
			{
				return;
			}
			Dictionary<Guid, TimeSpan> dictionary;
			bool flag;
			ProvisioningCache.GetAppRegistry(identification, out dictionary, out flag);
			if (flag && !string.IsNullOrEmpty(identification))
			{
				Dictionary<Guid, TimeSpan> dictionary2 = new Dictionary<Guid, TimeSpan>();
				if (dictionary != null && dictionary.Count != 0)
				{
					foreach (KeyValuePair<Guid, TimeSpan> keyValuePair in dictionary)
					{
						Guid key = keyValuePair.Key;
						if (CannedProvisioningExpirationTime.AllCannedProvisioningCacheKeys.Contains(key))
						{
							TimeSpan value = keyValuePair.Value;
							dictionary2.Add(key, value);
						}
					}
				}
				ProvisioningCache.InitializeAppRegistrySettings(identification, flag, dictionary2);
			}
		}

		private static Dictionary<Guid, TimeSpan> ConvertToExpirations(string expirationString)
		{
			Dictionary<Guid, TimeSpan> result;
			try
			{
				Dictionary<Guid, TimeSpan> dictionary = new Dictionary<Guid, TimeSpan>();
				string[] array = expirationString.Split(new char[]
				{
					';'
				});
				foreach (string text in array)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						Guid key;
						try
						{
							key = Guid.Parse(array3[0]);
						}
						catch (Exception ex)
						{
							throw new ArgumentException(ex.Message);
						}
						TimeSpan value;
						try
						{
							value = TimeSpan.Parse(array3[1]);
						}
						catch (Exception ex2)
						{
							throw new ArgumentException(ex2.Message);
						}
						dictionary.Add(key, value);
					}
				}
				result = dictionary;
			}
			catch (Exception ex3)
			{
				throw new ArgumentException(ex3.Message);
			}
			return result;
		}

		private static void InitializeAppRegistrySettings(string identification, bool enabled, Dictionary<Guid, TimeSpan> configuredExpirations)
		{
			if (ProvisioningCache.appRegistryInitialized)
			{
				return;
			}
			lock (ProvisioningCache.appRegistryInitializedLockObj)
			{
				if (!ProvisioningCache.appRegistryInitialized)
				{
					ProvisioningCache.cacheIdentification = identification;
					ProvisioningCache.configuredEnable = enabled;
					if (configuredExpirations != null)
					{
						ProvisioningCache.configuredExpirationTime = new Dictionary<Guid, TimeSpan>(configuredExpirations);
					}
					ProvisioningCache.appRegistryInitialized = true;
				}
			}
		}

		public static void InvalidCacheEntry(object changedObject)
		{
			IProvisioningCacheInvalidation provisioningCacheInvalidation = changedObject as IProvisioningCacheInvalidation;
			if (provisioningCacheInvalidation == null)
			{
				return;
			}
			OrganizationId organizationId = null;
			Guid[] array = null;
			if (provisioningCacheInvalidation.ShouldInvalidProvisioningCache(out organizationId, out array) && array != null && array.Length > 0)
			{
				if (InvalidationMessage.IsGlobalCacheEntry(array))
				{
					ProvisioningCache.Instance.RemoveGlobalDatas(array);
				}
				else if (OrganizationId.ForestWideOrgId.Equals(organizationId))
				{
					ProvisioningCache.Instance.RemoveOrganizationDatas(Guid.Empty, array);
				}
				else
				{
					ProvisioningCache.Instance.RemoveOrganizationDatas(organizationId.ConfigurationUnit.ObjectGuid, array);
				}
				ProvisioningCache.Instance.BroadcastInvalidationMessage(organizationId, array);
			}
		}

		private static void InitializePerfCounters()
		{
			ProvisioningCache.perfCounters = MSExchangeProvisioningCache.GetInstance(ProvisioningCache.cacheIdentification);
		}

		public T TryAddAndGetGlobalData<T>(Guid key, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			return this.TryAddAndGetGlobalData<T>(key, this.GetExpirationTime(key), getter);
		}

		public T TryAddAndGetGlobalData<T>(Guid key, TimeSpan expirationTime, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			if (expirationTime < TimeSpan.Zero)
			{
				throw new ArgumentException("expirationTime");
			}
			if (!this.globalLocks.ContainsKey(key))
			{
				throw new ArgumentException("The key is not a valid global entry cache key.");
			}
			T result = default(T);
			if (this.TryEnterReadLock(this.globalLocks[key], key, Guid.Empty))
			{
				bool flag = true;
				try
				{
					if (!this.globalData.ContainsKey(key) || ProvisioningCache.NeedWriteLockForCachedObject<T>(this.globalData[key], out result))
					{
						this.globalLocks[key].ExitReadLock();
						flag = false;
						ProvisioningCache.IncrementGlobalMissesCounter();
						if (this.TryEnterWriteLock(this.globalLocks[key], key, Guid.Empty))
						{
							try
							{
								if (!this.globalData.ContainsKey(key))
								{
									this.globalData[key] = new ExpiringCacheObject(expirationTime);
									ProvisioningCache.IncrementByTotalCachedObjectNum(1L);
								}
								result = ProvisioningCache.RetrieveData<T>(key, this.globalData[key], getter);
								goto IL_130;
							}
							finally
							{
								this.globalLocks[key].ExitWriteLock();
							}
						}
						return ProvisioningCache.CallGetterWithTrace<T>(key, getter);
					}
					ProvisioningCache.IncrementGlobalHitsCounter();
					IL_130:
					return result;
				}
				finally
				{
					if (flag)
					{
						this.globalLocks[key].ExitReadLock();
					}
				}
			}
			ProvisioningCache.IncrementGlobalMissesCounter();
			return ProvisioningCache.CallGetterWithTrace<T>(key, getter);
		}

		public T TryAddAndGetGlobalDictionaryValue<T, K>(Guid key, K subKey, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			return this.TryAddAndGetGlobalDictionaryValue<T, K>(key, subKey, this.GetExpirationTime(key), getter);
		}

		public T TryAddAndGetGlobalDictionaryValue<T, K>(Guid key, K subKey, TimeSpan expirationTime, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			if (subKey == null)
			{
				throw new ArgumentNullException("subKey");
			}
			if (expirationTime < TimeSpan.Zero)
			{
				throw new ArgumentException("expirationTime");
			}
			if (!this.globalLocks.ContainsKey(key))
			{
				throw new ArgumentException("The key is not a valid global entry cache key.");
			}
			T result = default(T);
			if (this.TryEnterReadLock(this.globalLocks[key], key, Guid.Empty))
			{
				bool flag = true;
				try
				{
					if (!this.globalData.ContainsKey(key) || ProvisioningCache.NeedWriteLockForCachedDictionary<K, T>(this.globalData[key], subKey, out result))
					{
						this.globalLocks[key].ExitReadLock();
						flag = false;
						ProvisioningCache.IncrementGlobalMissesCounter();
						if (this.TryEnterWriteLock(this.globalLocks[key], key, Guid.Empty))
						{
							try
							{
								if (!this.globalData.ContainsKey(key))
								{
									this.globalData[key] = new ExpiringCacheObject(expirationTime, new Dictionary<K, T>());
								}
								result = ProvisioningCache.RetrieveDataFromDictionary<T, K>(key, subKey, this.globalData[key], getter);
								goto IL_148;
							}
							finally
							{
								this.globalLocks[key].ExitWriteLock();
							}
						}
						return ProvisioningCache.CallGetterWithTrace<T, K>(key, subKey, getter);
					}
					ProvisioningCache.IncrementGlobalHitsCounter();
					IL_148:
					return result;
				}
				finally
				{
					if (flag)
					{
						this.globalLocks[key].ExitReadLock();
					}
				}
			}
			ProvisioningCache.IncrementGlobalMissesCounter();
			return ProvisioningCache.CallGetterWithTrace<T, K>(key, subKey, getter);
		}

		public T TryAddAndGetOrganizationData<T>(Guid key, OrganizationId organizationId, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			return this.TryAddAndGetOrganizationData<T>(key, organizationId, this.GetExpirationTime(key), getter);
		}

		public T TryAddAndGetOrganizationData<T>(Guid key, OrganizationId organizationId, TimeSpan expirationTime, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			if (key == Guid.Empty)
			{
				throw new ArgumentException("key");
			}
			if (expirationTime < TimeSpan.Zero)
			{
				throw new ArgumentException("expirationTime");
			}
			T result = default(T);
			Guid guid;
			if (organizationId == null || organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				guid = Guid.Empty;
			}
			else
			{
				guid = organizationId.ConfigurationUnit.ObjectGuid;
				if (guid.Equals(Guid.Empty))
				{
					return (T)((object)getter());
				}
			}
			ReaderWriterLockSlim readerWriterLockSlim = this.RetrieveOrganizationLock(guid);
			if (readerWriterLockSlim == null)
			{
				ProvisioningCache.IncrementOrganizationMissesCounter();
				return ProvisioningCache.CallGetterWithTrace<T>(key, getter);
			}
			if (this.TryEnterReadLock(readerWriterLockSlim, key, guid))
			{
				bool flag = true;
				try
				{
					Dictionary<Guid, ExpiringCacheObject> dictionary = this.organizationData[guid];
					if (ProvisioningCache.NeedWriteLockForCachedOrgData<T>(key, dictionary, out result))
					{
						readerWriterLockSlim.ExitReadLock();
						flag = false;
						ProvisioningCache.IncrementOrganizationMissesCounter();
						if (this.TryEnterWriteLock(readerWriterLockSlim, key, guid))
						{
							try
							{
								if (!dictionary.ContainsKey(key))
								{
									dictionary[key] = new ExpiringCacheObject(expirationTime);
									ProvisioningCache.IncrementByTotalCachedObjectNum(1L);
								}
								result = ProvisioningCache.RetrieveData<T>(key, dictionary[key], getter);
								goto IL_146;
							}
							finally
							{
								readerWriterLockSlim.ExitWriteLock();
							}
						}
						return ProvisioningCache.CallGetterWithTrace<T>(key, getter);
					}
					ProvisioningCache.IncrementOrganizationHitsCounter();
					IL_146:
					return result;
				}
				finally
				{
					if (flag)
					{
						readerWriterLockSlim.ExitReadLock();
					}
				}
			}
			ProvisioningCache.IncrementOrganizationMissesCounter();
			return ProvisioningCache.CallGetterWithTrace<T>(key, getter);
		}

		public T TryAddAndGetOrganizationDictionaryValue<T, K>(Guid key, OrganizationId organizationId, K subKey, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			return this.TryAddAndGetOrganizationDictionaryValue<T, K>(key, organizationId, subKey, this.GetExpirationTime(key), getter);
		}

		public T TryAddAndGetOrganizationDictionaryValue<T, K>(Guid key, OrganizationId organizationId, K subKey, TimeSpan expirationTime, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			if (!this.enabled)
			{
				return (T)((object)getter());
			}
			if (key == Guid.Empty)
			{
				throw new ArgumentException("key");
			}
			if (subKey == null)
			{
				throw new ArgumentNullException("subKey");
			}
			if (expirationTime < TimeSpan.Zero)
			{
				throw new ArgumentException("expirationTime");
			}
			Guid guid;
			if (organizationId == null || organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				guid = Guid.Empty;
			}
			else
			{
				guid = organizationId.ConfigurationUnit.ObjectGuid;
				if (guid.Equals(Guid.Empty))
				{
					return (T)((object)getter());
				}
			}
			ReaderWriterLockSlim readerWriterLockSlim = this.RetrieveOrganizationLock(guid);
			if (readerWriterLockSlim == null)
			{
				ProvisioningCache.IncrementOrganizationMissesCounter();
				return (T)((object)getter());
			}
			T result = default(T);
			if (this.TryEnterReadLock(readerWriterLockSlim, key, guid))
			{
				bool flag = true;
				try
				{
					Dictionary<Guid, ExpiringCacheObject> dictionary = this.organizationData[guid];
					if (ProvisioningCache.NeedWriteLockForCachedOrgDictionary<K, T>(key, dictionary, subKey, out result))
					{
						readerWriterLockSlim.ExitReadLock();
						flag = false;
						ProvisioningCache.IncrementOrganizationMissesCounter();
						if (this.TryEnterWriteLock(readerWriterLockSlim, key, guid))
						{
							try
							{
								if (!dictionary.ContainsKey(key))
								{
									dictionary[key] = new ExpiringCacheObject(expirationTime, new Dictionary<K, T>());
								}
								result = ProvisioningCache.RetrieveDataFromDictionary<T, K>(key, subKey, dictionary[key], getter);
								goto IL_160;
							}
							finally
							{
								readerWriterLockSlim.ExitWriteLock();
							}
						}
						return ProvisioningCache.CallGetterWithTrace<T, K>(key, subKey, getter);
					}
					ProvisioningCache.IncrementOrganizationHitsCounter();
					IL_160:
					return result;
				}
				finally
				{
					if (flag)
					{
						readerWriterLockSlim.ExitReadLock();
					}
				}
			}
			ProvisioningCache.IncrementOrganizationMissesCounter();
			return ProvisioningCache.CallGetterWithTrace<T, K>(key, subKey, getter);
		}

		public void RemoveGlobalData(Guid key)
		{
			if (!this.enabled)
			{
				return;
			}
			if (!this.globalLocks.ContainsKey(key))
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCBadGlobalCacheKeyReceived, key.ToString(), new object[]
				{
					key.ToString()
				});
				return;
			}
			try
			{
				if (this.TryEnterEntryRemovalWriteLock(this.globalLocks[key], key, Guid.Empty) && this.globalData.ContainsKey(key))
				{
					int num = -ProvisioningCache.GetActualObjectNum(this.globalData[key].Data);
					this.globalData.Remove(key);
					ProvisioningCache.IncrementByTotalCachedObjectNum((long)num);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCGlobalDataInvalidated, key.ToString(), new object[]
					{
						key.ToString(),
						ProvisioningCache.cacheIdentification
					});
				}
			}
			finally
			{
				try
				{
					this.globalLocks[key].ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void RemoveGlobalDatas(ICollection<Guid> keys)
		{
			if (!this.enabled)
			{
				return;
			}
			if (keys == null)
			{
				return;
			}
			if (keys.Count == 0)
			{
				this.ResetGlobalData();
				return;
			}
			foreach (Guid key in keys)
			{
				this.RemoveGlobalData(key);
			}
		}

		public void RemoveOrganizationData(OrganizationId organizationId, Guid key)
		{
			Guid orgId;
			if (organizationId == null || organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				orgId = Guid.Empty;
			}
			else
			{
				orgId = organizationId.ConfigurationUnit.ObjectGuid;
			}
			this.RemoveOrganizationData(orgId, key);
		}

		public void RemoveOrganizationData(Guid orgId, Guid key)
		{
			if (!this.enabled)
			{
				return;
			}
			ReaderWriterLockSlim readerWriterLockSlim = this.RetrieveOrganizationLock(orgId);
			if (readerWriterLockSlim == null)
			{
				return;
			}
			try
			{
				if (this.TryEnterEntryRemovalWriteLock(readerWriterLockSlim, key, orgId))
				{
					Dictionary<Guid, ExpiringCacheObject> dictionary = this.organizationData[orgId];
					if (dictionary.ContainsKey(key) && dictionary[key] != null && !dictionary[key].IsExpired)
					{
						int num = -ProvisioningCache.GetActualObjectNum(dictionary[key]);
						dictionary.Remove(key);
						ProvisioningCache.IncrementByTotalCachedObjectNum((long)num);
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCOrganizationDataInvalidated, key.ToString(), new object[]
						{
							key.ToString(),
							orgId.ToString(),
							ProvisioningCache.cacheIdentification
						});
					}
				}
			}
			finally
			{
				try
				{
					readerWriterLockSlim.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void RemoveOrganizationDatas(Guid orgId, ICollection<Guid> keys)
		{
			if (!this.enabled)
			{
				return;
			}
			ReaderWriterLockSlim readerWriterLockSlim = this.RetrieveOrganizationLock(orgId);
			if (readerWriterLockSlim == null)
			{
				return;
			}
			try
			{
				if (this.TryEnterEntryRemovalWriteLock(readerWriterLockSlim, Guid.Empty, orgId))
				{
					Dictionary<Guid, ExpiringCacheObject> dictionary = this.organizationData[orgId];
					if (keys.Count == 0)
					{
						int num = 0;
						foreach (object obj in this.organizationData[orgId].Values)
						{
							num -= ProvisioningCache.GetActualObjectNum(obj);
						}
						this.organizationData[orgId].Clear();
						ProvisioningCache.IncrementByTotalCachedObjectNum((long)num);
					}
					else
					{
						foreach (Guid key in keys)
						{
							if (dictionary.ContainsKey(key) && dictionary[key] != null && !dictionary[key].IsExpired)
							{
								int num2 = -ProvisioningCache.GetActualObjectNum(dictionary[key]);
								dictionary.Remove(key);
								ProvisioningCache.IncrementByTotalCachedObjectNum((long)num2);
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCOrganizationDataInvalidated, key.ToString(), new object[]
								{
									key.ToString(),
									orgId.ToString(),
									ProvisioningCache.cacheIdentification
								});
							}
						}
					}
				}
			}
			finally
			{
				try
				{
					readerWriterLockSlim.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		internal void ResetGlobalData()
		{
			if (!this.enabled)
			{
				return;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingGlobalData, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification
			});
			foreach (Guid key in this.globalLocks.Keys)
			{
				this.RemoveGlobalData(key);
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingGlobalDataFinished, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification
			});
		}

		internal void ResetOrganizationData()
		{
			if (!this.enabled)
			{
				return;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingOrganizationData, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification
			});
			Guid[] array = null;
			try
			{
				if (this.TryEnterEntryRemovalReadLock(this.organizationTableLock))
				{
					array = new Guid[this.organizationLocks.Count];
					this.organizationLocks.Keys.CopyTo(array, 0);
				}
			}
			finally
			{
				try
				{
					this.organizationTableLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (array != null && array.Length > 0)
			{
				foreach (Guid guid in array)
				{
					try
					{
						if (this.TryEnterEntryRemovalWriteLock(this.organizationLocks[guid], Guid.Empty, guid))
						{
							int num = 0;
							foreach (object obj in this.organizationData[guid].Values)
							{
								num -= ProvisioningCache.GetActualObjectNum(obj);
							}
							this.organizationData[guid].Clear();
							ProvisioningCache.IncrementByTotalCachedObjectNum((long)num);
						}
					}
					finally
					{
						try
						{
							this.organizationLocks[guid].ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingOrganizationDataFinished, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification
			});
		}

		internal IEnumerable<CachedEntryObject> DumpCachedEntries(ICollection<Guid> orgs, ICollection<Guid> keys)
		{
			if (orgs == null)
			{
				return this.DumpGlobalCachedEntries(keys);
			}
			return this.DumpOrganizationCachedEntries(orgs, keys);
		}

		internal void BroadcastInvalidationMessage(OrganizationId orgId, Guid[] keys)
		{
			if (this.broadcaster == null)
			{
				lock (this.cacheBroadcasterLock)
				{
					if (this.broadcaster == null)
					{
						this.broadcaster = new CacheBroadcaster(9050U);
					}
				}
			}
			this.broadcaster.BroadcastInvalidationMessage(orgId, keys);
		}

		internal void ClearExpireOrganizations()
		{
			if (!this.enabled)
			{
				return;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCClearingExpiredOrganizations, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification
			});
			Guid[] array = null;
			int num = 0;
			try
			{
				if (this.TryEnterEntryRemovalReadLock(this.organizationTableLock))
				{
					array = new Guid[this.organizationLocks.Count];
					this.organizationLocks.Keys.CopyTo(array, 0);
				}
			}
			finally
			{
				try
				{
					this.organizationTableLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (array != null && array.Length > 0)
			{
				foreach (Guid guid in array)
				{
					try
					{
						if (this.TryEnterEntryRemovalWriteLock(this.organizationLocks[guid], Guid.Empty, guid))
						{
							Dictionary<Guid, ExpiringCacheObject> dictionary = this.organizationData[guid];
							bool flag = true;
							foreach (Guid key in dictionary.Keys)
							{
								if (!dictionary[key].IsExpired)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								dictionary.Clear();
								num++;
							}
						}
					}
					finally
					{
						try
						{
							this.organizationLocks[guid].ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCClearingExpiredOrganizationsFinished, ProvisioningCache.cacheIdentification, new object[]
			{
				ProvisioningCache.cacheIdentification,
				num
			});
		}

		internal void Reset()
		{
			if (this.enabled)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingWholeProvisioningCache, ProvisioningCache.cacheIdentification, new object[]
				{
					ProvisioningCache.cacheIdentification
				});
				this.ResetGlobalData();
				this.ResetOrganizationData();
			}
		}

		internal void StopProvisioningCacheActivities()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (this.invalidationActivity != null)
			{
				this.invalidationActivity.StopExecute();
			}
			if (this.cleanerActivity != null)
			{
				this.cleanerActivity.StopExecute();
			}
			if (this.diagnosticActivity != null)
			{
				this.diagnosticActivity.StopExecute();
			}
		}

		private static bool NeedWriteLockForCachedObject<T>(ExpiringCacheObject entry, out T result)
		{
			result = default(T);
			if (entry == null || entry.IsExpired)
			{
				return true;
			}
			result = (T)((object)entry.Data);
			return false;
		}

		private static bool NeedWriteLockForCachedDictionary<K, T>(ExpiringCacheObject entry, K subKey, out T result)
		{
			result = default(T);
			if (entry == null || entry.IsExpired)
			{
				return true;
			}
			Dictionary<K, T> dictionary = (Dictionary<K, T>)entry.Data;
			if (dictionary == null || !dictionary.ContainsKey(subKey) || dictionary[subKey] == null)
			{
				return true;
			}
			result = dictionary[subKey];
			return false;
		}

		private static bool NeedWriteLockForCachedOrgData<T>(Guid key, Dictionary<Guid, ExpiringCacheObject> orgEntry, out T result)
		{
			result = default(T);
			return !orgEntry.ContainsKey(key) || orgEntry[key] == null || ProvisioningCache.NeedWriteLockForCachedObject<T>(orgEntry[key], out result);
		}

		private static bool NeedWriteLockForCachedOrgDictionary<K, T>(Guid key, Dictionary<Guid, ExpiringCacheObject> orgEntry, K subKey, out T result)
		{
			result = default(T);
			return !orgEntry.ContainsKey(key) || orgEntry[key] == null || ProvisioningCache.NeedWriteLockForCachedDictionary<K, T>(orgEntry[key], subKey, out result);
		}

		private static T AddToDictionary<T, K>(Guid key, Dictionary<K, T> dict, K subKey, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			T t;
			if (!dict.ContainsKey(subKey) || dict[subKey] == null)
			{
				t = ProvisioningCache.CallGetterWithTrace<T, K>(key, subKey, getter);
				dict[subKey] = t;
				ProvisioningCache.IncrementByTotalCachedObjectNum(1L);
			}
			else
			{
				t = dict[subKey];
			}
			return t;
		}

		private static T CallGetterWithTrace<T>(Guid key, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			return (T)((object)getter());
		}

		private static T CallGetterWithTrace<T, K>(Guid key, K subKey, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			return (T)((object)getter());
		}

		private static T RetrieveData<T>(Guid key, ExpiringCacheObject entry, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (entry.IsExpired)
			{
				entry.Data = ProvisioningCache.CallGetterWithTrace<T>(key, getter);
			}
			return (T)((object)entry.Data);
		}

		private static T RetrieveDataFromDictionary<T, K>(Guid key, K subKey, ExpiringCacheObject entry, ProvisioningCache.CacheObjectGetterDelegate getter)
		{
			if (entry.IsExpired)
			{
				int num = -ProvisioningCache.GetActualObjectNum(entry.Data);
				entry.Data = new Dictionary<K, T>();
				ProvisioningCache.IncrementByTotalCachedObjectNum((long)num);
			}
			Dictionary<K, T> dict = (Dictionary<K, T>)entry.Data;
			return ProvisioningCache.AddToDictionary<T, K>(key, dict, subKey, getter);
		}

		private static void IncrementGlobalHitsCounter()
		{
			if (LoggerSettings.LogEnabled)
			{
				ProvisioningCache.IncreaseCacheHitCount();
			}
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.GlobalAggregateHits.Increment();
			}
		}

		private static void IncrementGlobalMissesCounter()
		{
			if (LoggerSettings.LogEnabled)
			{
				ProvisioningCache.IncreaseCacheMissCount();
			}
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.GlobalAggregateMisses.Increment();
			}
		}

		private static void IncrementOrganizationHitsCounter()
		{
			if (LoggerSettings.LogEnabled)
			{
				ProvisioningCache.IncreaseCacheHitCount();
			}
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.OrganizationAggregateHits.Increment();
			}
		}

		private static void IncrementOrganizationMissesCounter()
		{
			if (LoggerSettings.LogEnabled)
			{
				ProvisioningCache.IncreaseCacheMissCount();
			}
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.OrganizationAggregateMisses.Increment();
			}
		}

		internal static void IncrementReceivedInvalidationMsgNum()
		{
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.ReceivedInvalidationMsgNum.Increment();
			}
		}

		private static void IncrementByTotalCachedObjectNum(long increment)
		{
			if (ProvisioningCache.PerfCounters != null)
			{
				ProvisioningCache.PerfCounters.TotalCachedObjectNum.IncrementBy(increment);
			}
		}

		private static int GetActualObjectNum(object obj)
		{
			int result = 1;
			IDictionary dictionary = obj as IDictionary;
			if (dictionary != null)
			{
				result = dictionary.Count;
			}
			return result;
		}

		private void InitializeSettings(string identification, bool enabled)
		{
			if (enabled && (string.Equals(identification, "Powershell", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "Powershell-LiveId", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "Powershell-Proxy", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "PowershellLiveId-Proxy", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "Psws", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "Ecp", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "Owa", StringComparison.OrdinalIgnoreCase) || string.Equals(identification, "HxS", StringComparison.OrdinalIgnoreCase)))
			{
				this.CreateBasicCache();
				this.broadcaster = new CacheBroadcaster(9050U);
				try
				{
					this.invalidationActivity = new InvalidationRecvActivity(this, 9050U);
				}
				catch (SocketException ex)
				{
					this.enabled = false;
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCSocketExceptionDisabledProvisioningCache, identification, new object[]
					{
						identification,
						ex.Message
					});
					return;
				}
				this.cleanerActivity = new ObsoluteOrgCleanerActivity(this);
				try
				{
					this.diagnosticActivity = new DiagnosticActivity(this, CacheApplicationManager.GetAppPipeName(identification));
				}
				catch (IOException)
				{
				}
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCProvisioningCacheEnabled, identification, new object[]
				{
					identification
				});
				this.invalidationActivity.ExecuteAsync(new Action<Activity, Exception>(this.OnActivityCompleted));
				this.cleanerActivity.ExecuteAsync(new Action<Activity, Exception>(this.OnActivityCompleted));
				if (this.diagnosticActivity != null)
				{
					this.diagnosticActivity.ExecuteAsync(new Action<Activity, Exception>(this.OnActivityCompleted));
				}
				ProvisioningCache.InitializePerfCounters();
			}
		}

		private IEnumerable<CachedEntryObject> DumpGlobalCachedEntries(ICollection<Guid> keys)
		{
			if (keys.Count == 0)
			{
				keys = CannedProvisioningCacheKeys.GlobalCacheKeys;
			}
			foreach (Guid key in keys)
			{
				object value = null;
				if (this.globalLocks.ContainsKey(key))
				{
					try
					{
						if (this.TryEnterReadLock(this.globalLocks[key], key, Guid.Empty) && this.globalData.ContainsKey(key))
						{
							ExpiringCacheObject expiringCacheObject = this.globalData[key];
							if (!expiringCacheObject.IsExpired)
							{
								value = expiringCacheObject.Data;
							}
						}
					}
					finally
					{
						this.globalLocks[key].ExitReadLock();
					}
					yield return new CachedEntryObject(key, value);
				}
			}
			yield break;
		}

		private IEnumerable<CachedEntryObject> DumpOrganizationCachedEntries(ICollection<Guid> orgs, ICollection<Guid> keys)
		{
			Guid[] allOrgs = null;
			bool dumpAll = keys.Count == 0;
			try
			{
				if (this.TryEnterEntryRemovalReadLock(this.organizationTableLock))
				{
					allOrgs = new Guid[this.organizationLocks.Count];
					this.organizationLocks.Keys.CopyTo(allOrgs, 0);
				}
			}
			finally
			{
				try
				{
					this.organizationTableLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (orgs.Count == 0)
			{
				orgs = allOrgs;
			}
			foreach (Guid orgId in orgs)
			{
				if (this.organizationLocks.ContainsKey(orgId) && this.TryEnterReadLock(this.organizationLocks[orgId], Guid.Empty, orgId))
				{
					try
					{
						Dictionary<Guid, ExpiringCacheObject> dict = this.organizationData[orgId];
						if (dumpAll)
						{
							foreach (Guid guid in dict.Keys)
							{
								if (!dict[guid].IsExpired)
								{
									yield return new CachedEntryObject(guid, orgId, dict[guid].Data);
								}
							}
						}
						else
						{
							foreach (Guid guid2 in keys)
							{
								if (dict.ContainsKey(guid2) && !dict[guid2].IsExpired)
								{
									yield return new CachedEntryObject(guid2, orgId, dict[guid2].Data);
								}
							}
						}
					}
					finally
					{
						this.organizationLocks[orgId].ExitReadLock();
					}
				}
			}
			yield break;
		}

		private void CreateBasicCache()
		{
			this.enabled = true;
			this.globalLocks = new Dictionary<Guid, ReaderWriterLockSlim>();
			foreach (Guid key in CannedProvisioningCacheKeys.GlobalCacheKeys)
			{
				this.globalLocks[key] = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			}
			this.globalData = new Dictionary<Guid, ExpiringCacheObject>();
			this.organizationTableLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			this.organizationLocks = new Dictionary<Guid, ReaderWriterLockSlim>();
			this.organizationData = new Dictionary<Guid, Dictionary<Guid, ExpiringCacheObject>>();
		}

		private ReaderWriterLockSlim RetrieveOrganizationLock(Guid orgId)
		{
			ReaderWriterLockSlim result = null;
			if (this.TryEnterReadLock(this.organizationTableLock, Guid.Empty, orgId))
			{
				bool flag = true;
				try
				{
					if (!this.organizationLocks.ContainsKey(orgId))
					{
						this.organizationTableLock.ExitReadLock();
						flag = false;
						if (!this.TryEnterWriteLock(this.organizationTableLock, Guid.Empty, orgId))
						{
							goto IL_A3;
						}
						try
						{
							if (!this.organizationLocks.ContainsKey(orgId))
							{
								this.organizationLocks[orgId] = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
								this.organizationData[orgId] = new Dictionary<Guid, ExpiringCacheObject>();
							}
							result = this.organizationLocks[orgId];
							goto IL_A3;
						}
						finally
						{
							this.organizationTableLock.ExitWriteLock();
						}
					}
					result = this.organizationLocks[orgId];
					IL_A3:;
				}
				finally
				{
					if (flag)
					{
						this.organizationTableLock.ExitReadLock();
					}
				}
			}
			return result;
		}

		private TimeSpan GetExpirationTime(Guid key)
		{
			TimeSpan zero = TimeSpan.Zero;
			if (ProvisioningCache.configuredExpirationTime.TryGetValue(key, out zero))
			{
				return zero;
			}
			return CannedProvisioningExpirationTime.GetDefaultExpirationTime(key);
		}

		private bool TryEnterReadLock(ReaderWriterLockSlim lockSlim, Guid key, Guid orgId)
		{
			if (lockSlim.TryEnterReadLock(ProvisioningCache.lockEnterExpirationTime))
			{
				return true;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCEnterReadLockFailed, key.ToString(), new object[]
			{
				ProvisioningCache.lockEnterExpirationTime,
				key.ToString(),
				orgId.ToString()
			});
			return false;
		}

		private bool TryEnterWriteLock(ReaderWriterLockSlim lockSlim, Guid key, Guid orgId)
		{
			if (lockSlim.TryEnterWriteLock(ProvisioningCache.lockEnterExpirationTime))
			{
				return true;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCEnterWriteLockFailed, key.ToString(), new object[]
			{
				ProvisioningCache.lockEnterExpirationTime,
				key.ToString(),
				orgId.ToString()
			});
			return false;
		}

		private bool TryEnterEntryRemovalReadLock(ReaderWriterLockSlim lockSlim)
		{
			if (lockSlim.TryEnterReadLock(ProvisioningCache.lockEnterExpirationTimeForEntryRemoval))
			{
				return true;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCEnterReadLockForOrgRemovalFailed, lockSlim.ToString(), new object[]
			{
				ProvisioningCache.lockEnterExpirationTimeForEntryRemoval
			});
			return false;
		}

		private bool TryEnterEntryRemovalWriteLock(ReaderWriterLockSlim lockSlim, Guid key, Guid orgId)
		{
			if (lockSlim.TryEnterWriteLock(ProvisioningCache.lockEnterExpirationTimeForEntryRemoval))
			{
				return true;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCEnterWriteLockForOrgDataRemovalFailed, key.ToString(), new object[]
			{
				ProvisioningCache.lockEnterExpirationTimeForEntryRemoval,
				key.ToString(),
				orgId.ToString()
			});
			return false;
		}

		private void OnActivityCompleted(Activity activity, Exception exception)
		{
			if (exception != null)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCUnhandledExceptionInActivity, activity.Name, new object[]
				{
					activity.Name,
					exception.Message
				});
				if (!activity.GotStopSignalFromTestCode)
				{
					throw exception;
				}
			}
		}

		private const uint BroadcastPort = 9050U;

		private static readonly string MSExchangeProvisioningCacheRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\ProvisioningCache";

		private static readonly string ProvisioningCacheExpiration = "ProvisioningCacheExpiration";

		private static readonly string ProvisioningCacheEnabled = "ProvisioningCacheEnabled";

		private static readonly TimeSpan lockEnterExpirationTime = new TimeSpan(0, 0, 1);

		private static readonly TimeSpan lockEnterExpirationTimeForEntryRemoval = new TimeSpan(0, 0, 15);

		private static ProvisioningCache cacheInstance = null;

		private static ProvisioningCache disabledInstance = null;

		private static object cacheInstanceLock = new object();

		private static object disabledInstanceLock = new object();

		private static string cacheIdentification = null;

		private static bool configuredEnable = false;

		private static Dictionary<Guid, TimeSpan> configuredExpirationTime = new Dictionary<Guid, TimeSpan>();

		private static bool appRegistryInitialized = false;

		private static object appRegistryInitializedLockObj = new object();

		private static MSExchangeProvisioningCacheInstance perfCounters = null;

		private object cacheBroadcasterLock = new object();

		private bool enabled;

		private Dictionary<Guid, ReaderWriterLockSlim> globalLocks;

		private Dictionary<Guid, ExpiringCacheObject> globalData;

		private ReaderWriterLockSlim organizationTableLock;

		private Dictionary<Guid, ReaderWriterLockSlim> organizationLocks;

		private Dictionary<Guid, Dictionary<Guid, ExpiringCacheObject>> organizationData;

		private InvalidationRecvActivity invalidationActivity;

		private ObsoluteOrgCleanerActivity cleanerActivity;

		private DiagnosticActivity diagnosticActivity;

		private CacheBroadcaster broadcaster;

		public delegate object CacheObjectGetterDelegate();
	}
}
