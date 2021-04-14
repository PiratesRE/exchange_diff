using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal sealed class MonadMediatorPool : IDisposable
	{
		public MonadMediatorPool() : this(1)
		{
		}

		public MonadMediatorPool(int runspaceMediatorSize) : this(runspaceMediatorSize, TimeSpan.Zero)
		{
		}

		public MonadMediatorPool(int runspaceMediatorSize, TimeSpan inactivityCacheCleanupThreshold)
		{
			this.runspaceMediatorSize = runspaceMediatorSize;
			this.inactivityCacheCleanupThreshold = inactivityCacheCleanupThreshold;
			this.lastUpdatedKey = -1;
		}

		internal MonadRunspaceCache[] RunspaceCache
		{
			get
			{
				if (!Environment.StackTrace.Contains("RemoteMonadCommandTester"))
				{
					throw new NotSupportedException("RunspaceCache property should never be accessed from Production Code.");
				}
				return this.connectionPooledInstanceCache;
			}
		}

		public void Dispose()
		{
			if (this.connectionPooledInstanceCache != null)
			{
				foreach (MonadRunspaceCache monadRunspaceCache in this.connectionPooledInstanceCache)
				{
					try
					{
						monadRunspaceCache.Dispose();
					}
					catch (Exception)
					{
					}
				}
			}
			this.currentKey = null;
			this.connectionPooledInstace = null;
			this.connectionPooledInstanceCache = null;
		}

		internal RunspaceMediator GetRunspacePooledMediatorInstance()
		{
			if (this.pooledInstance == null)
			{
				lock (this.syncInstance)
				{
					if (this.pooledInstance == null)
					{
						this.pooledInstance = new RunspaceMediator(MonadRunspaceFactory.GetInstance(), new MonadRunspaceCache());
					}
				}
			}
			return this.pooledInstance;
		}

		internal RunspaceMediator GetRunspaceNonPooledMediatorInstance()
		{
			if (this.nonPooledInstance == null)
			{
				lock (this.syncInstance)
				{
					if (this.nonPooledInstance == null)
					{
						this.nonPooledInstance = new RunspaceMediator(MonadRunspaceFactory.GetInstance(), new EmptyRunspaceCache());
					}
				}
			}
			return this.nonPooledInstance;
		}

		internal RunspaceMediator GetRunspacePooledMediatorInstance(MonadMediatorPoolKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.connectionPooledInstace != null)
			{
				RunspaceMediator runspaceMediator = null;
				MonadRunspaceCache result = null;
				for (int i = 0; i < this.runspaceMediatorSize; i++)
				{
					if (key.Equals(this.currentKey[i]) && !this.connectionPooledInstanceCache[i].IsDisposed)
					{
						runspaceMediator = this.connectionPooledInstace[i];
						result = this.connectionPooledInstanceCache[i];
						break;
					}
				}
				if (runspaceMediator != null)
				{
					this.CleanUpCache(this.connectionPooledInstanceCache, this.inactivityCacheCleanupThreshold, result);
					return runspaceMediator;
				}
			}
			else
			{
				lock (this.syncInstance)
				{
					if (this.connectionPooledInstace == null)
					{
						this.connectionPooledInstace = new RunspaceMediator[this.runspaceMediatorSize];
						this.connectionPooledInstanceCache = new MonadRunspaceCache[this.runspaceMediatorSize];
						this.currentKey = new MonadMediatorPoolKey[this.runspaceMediatorSize];
					}
				}
			}
			RunspaceMediator result2;
			lock (this.syncInstance)
			{
				this.CleanUpCache(this.connectionPooledInstanceCache, this.inactivityCacheCleanupThreshold, null);
				int num = this.ResolveNextCacheToReplace();
				if (this.connectionPooledInstanceCache[num] != null && !this.connectionPooledInstanceCache[num].IsDisposed)
				{
					this.connectionPooledInstanceCache[num].Dispose();
				}
				this.connectionPooledInstanceCache[num] = new MonadRunspaceCache();
				this.connectionPooledInstace[num] = new RunspaceMediator(new MonadRemoteRunspaceFactory(key.ConnectionInfo, key.ServerSettings), this.connectionPooledInstanceCache[num]);
				this.currentKey[num] = key;
				this.lastUpdatedKey = num;
				result2 = this.connectionPooledInstace[num];
			}
			return result2;
		}

		private int ResolveNextCacheToReplace()
		{
			int num = (this.lastUpdatedKey + 1) % this.runspaceMediatorSize;
			if (this.connectionPooledInstanceCache[num] == null || this.connectionPooledInstanceCache[num].IsDisposed)
			{
				return num;
			}
			for (int i = 0; i < this.runspaceMediatorSize; i++)
			{
				if (this.connectionPooledInstanceCache[i] == null || this.connectionPooledInstanceCache[i].IsDisposed)
				{
					return i;
				}
			}
			return num;
		}

		private void CleanUpCache(MonadRunspaceCache[] connectionPooledInstanceCache, TimeSpan inactivityCacheCleanupThreshold, MonadRunspaceCache result)
		{
			if (connectionPooledInstanceCache == null)
			{
				return;
			}
			if (inactivityCacheCleanupThreshold == TimeSpan.Zero)
			{
				return;
			}
			for (int i = 0; i < connectionPooledInstanceCache.Length; i++)
			{
				if (connectionPooledInstanceCache[i] != null && (result == null || result != connectionPooledInstanceCache[i]) && !connectionPooledInstanceCache[i].IsDisposed)
				{
					TimeSpan t = ExDateTime.UtcNow - connectionPooledInstanceCache[i].LastTimeCacheUsed;
					if (t > inactivityCacheCleanupThreshold)
					{
						connectionPooledInstanceCache[i].Dispose();
					}
				}
			}
		}

		private const int DefaultRunspaceMediatorSize = 1;

		private object syncInstance = new object();

		private RunspaceMediator pooledInstance;

		private RunspaceMediator nonPooledInstance;

		private MonadRunspaceCache[] connectionPooledInstanceCache;

		private volatile RunspaceMediator[] connectionPooledInstace;

		private MonadMediatorPoolKey[] currentKey;

		private int runspaceMediatorSize;

		private int lastUpdatedKey;

		private TimeSpan inactivityCacheCleanupThreshold;
	}
}
