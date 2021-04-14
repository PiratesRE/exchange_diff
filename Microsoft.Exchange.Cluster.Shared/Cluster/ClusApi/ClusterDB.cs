using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	public class ClusterDB : DisposeTrackableBase, IClusterDB, IDisposable
	{
		public static IClusterDB Open()
		{
			return ClusterDB.Open(AmServerName.LocalComputerName);
		}

		internal static IClusterDB Open(AmServerName serverName)
		{
			return ClusterDB.hookableFactory.Value(serverName);
		}

		internal static IDisposable SetTestHook(Func<AmServerName, IClusterDB> newFactory)
		{
			return ClusterDB.hookableFactory.SetTestHook(newFactory);
		}

		private static IClusterDB Factory(AmServerName serverName)
		{
			return new ClusterDB(serverName);
		}

		private ClusterDB(AmServerName serverName)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<ClusterDB>(this);
				this.isInstalled = AmCluster.IsInstalled(serverName);
				if (AmCluster.IsRunning(serverName))
				{
					this.amCluster = AmCluster.OpenByName(serverName);
					this.rootHandle = DistributedStore.Instance.GetClusterKey(this.amCluster.Handle, null, serverName.Fqdn, DxStoreKeyAccessMode.Read, false);
					this.openWriteBatches = new List<ClusterDB.WriteBatch>(10);
				}
				disposeGuard.Success();
			}
		}

		public bool IsInstalled
		{
			get
			{
				return this.isInstalled;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.amCluster != null;
			}
		}

		public IEnumerable<string> GetSubKeyNames(string registryKey)
		{
			IEnumerable<string> subkeyNames;
			using (IDistributedStoreKey distributedStoreKey = this.OpenRegKey(registryKey, false, false))
			{
				subkeyNames = distributedStoreKey.GetSubkeyNames(null);
			}
			return subkeyNames;
		}

		public IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfos(string registryKey)
		{
			IEnumerable<Tuple<string, RegistryValueKind>> valueInfos;
			using (IDistributedStoreKey distributedStoreKey = this.OpenRegKey(registryKey, false, false))
			{
				valueInfos = distributedStoreKey.GetValueInfos(null);
			}
			return valueInfos;
		}

		public T GetValue<T>(string keyName, string valueName, T defaultValue)
		{
			T result;
			try
			{
				using (IDistributedStoreKey distributedStoreKey = this.OpenRegKey(keyName, false, false))
				{
					result = distributedStoreKey.GetValue(valueName, defaultValue, null);
				}
			}
			catch (ClusterApiException)
			{
				result = defaultValue;
			}
			return result;
		}

		public void SetValue<T>(string keyName, string propertyName, T propetyValue)
		{
			using (IDistributedStoreKey distributedStoreKey = this.OpenRegKey(keyName, true, false))
			{
				distributedStoreKey.SetValue(propertyName, propetyValue, false, null);
			}
		}

		public void DeleteValue(string keyName, string propertyName)
		{
			using (IDistributedStoreKey distributedStoreKey = this.OpenRegKey(keyName, false, true))
			{
				distributedStoreKey.DeleteValue(propertyName, true, null);
			}
		}

		public IClusterDBWriteBatch CreateWriteBatch(string registryKey)
		{
			ClusterDB.WriteBatch writeBatch = new ClusterDB.WriteBatch(this, registryKey);
			this.openWriteBatches.Add(writeBatch);
			return writeBatch;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ClusterDB>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.openWriteBatches != null)
				{
					this.openWriteBatches = null;
				}
				if (this.rootHandle != null)
				{
					this.rootHandle.Dispose();
					this.rootHandle = null;
				}
				if (this.amCluster != null)
				{
					this.amCluster.Dispose();
					this.amCluster = null;
				}
			}
		}

		private IDistributedStoreKey OpenRegKey(string keyName, bool createIfNotExists, bool ignoreIfNotExits = false)
		{
			return this.OpenRegKey(this.rootHandle, keyName, createIfNotExists, false);
		}

		private IDistributedStoreKey OpenRegKey(IDistributedStoreKey rootKey, string keyName, bool createIfNotExists, bool ignoreIfNotExits = false)
		{
			if (rootKey == null)
			{
				throw AmExceptionHelper.ConstructClusterApiException(5004, "OpenRegKey()", new object[0]);
			}
			if (!createIfNotExists)
			{
				return rootKey.OpenKey(keyName, DxStoreKeyAccessMode.Write, ignoreIfNotExits, null);
			}
			return rootKey.OpenKey(keyName, DxStoreKeyAccessMode.CreateIfNotExist, false, null);
		}

		private void RemoveWriteBatch(ClusterDB.WriteBatch toRemove)
		{
			int num = this.openWriteBatches.IndexOf(toRemove);
			if (num >= 0)
			{
				this.openWriteBatches.RemoveAt(num);
			}
		}

		private static Hookable<Func<AmServerName, IClusterDB>> hookableFactory = Hookable<Func<AmServerName, IClusterDB>>.Create(false, new Func<AmServerName, IClusterDB>(ClusterDB.Factory));

		private readonly bool isInstalled;

		private AmCluster amCluster;

		private IDistributedStoreKey rootHandle;

		private List<ClusterDB.WriteBatch> openWriteBatches;

		private class WriteBatch : DisposeTrackableBase, IClusterDBWriteBatch, IDisposable
		{
			public WriteBatch(ClusterDB parent, string registryKey)
			{
				this.parent = parent;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					disposeGuard.Add<ClusterDB.WriteBatch>(this);
					this.regKeyHandle = parent.OpenRegKey(registryKey, true, false);
					this.batchHandle = this.regKeyHandle.CreateBatchUpdateRequest();
					disposeGuard.Success();
				}
			}

			public void CreateOrOpenKey(string keyName)
			{
				this.batchHandle.CreateKey(keyName);
			}

			public void DeleteKey(string keyName)
			{
				this.batchHandle.DeleteKey(keyName);
			}

			public void SetValue(string valueName, string value)
			{
				this.batchHandle.SetValue(valueName, value, RegistryValueKind.Unknown);
			}

			public void SetValue(string valueName, IEnumerable<string> value)
			{
				this.batchHandle.SetValue(valueName, value.ToArray<string>(), RegistryValueKind.Unknown);
			}

			public void SetValue(string valueName, int value)
			{
				this.batchHandle.SetValue(valueName, value, RegistryValueKind.Unknown);
			}

			public void SetValue(string valueName, long value)
			{
				this.batchHandle.SetValue(valueName, value, RegistryValueKind.Unknown);
			}

			public void DeleteValue(string valueName)
			{
				this.batchHandle.DeleteValue(valueName);
			}

			public void Execute()
			{
				try
				{
					if (this.batchHandle != null)
					{
						try
						{
							this.batchHandle.Execute(null);
						}
						finally
						{
							try
							{
								this.batchHandle.Dispose();
							}
							finally
							{
								this.batchHandle = null;
							}
						}
					}
				}
				finally
				{
					this.parent.RemoveWriteBatch(this);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<ClusterDB.WriteBatch>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					try
					{
						if (this.batchHandle != null)
						{
							try
							{
								this.batchHandle.Dispose();
							}
							finally
							{
								this.batchHandle = null;
							}
						}
					}
					finally
					{
						if (this.regKeyHandle != null)
						{
							this.regKeyHandle.Dispose();
							this.regKeyHandle = null;
						}
						this.parent.RemoveWriteBatch(this);
					}
				}
			}

			private IDistributedStoreKey regKeyHandle;

			private IDistributedStoreBatchRequest batchHandle;

			private ClusterDB parent;
		}
	}
}
