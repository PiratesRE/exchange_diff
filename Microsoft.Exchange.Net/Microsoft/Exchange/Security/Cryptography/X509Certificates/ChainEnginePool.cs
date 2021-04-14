using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainEnginePool : IDisposable, IEnginePool
	{
		public ChainEnginePool() : this(10, ChainEngineOptions.CacheEndCert | ChainEngineOptions.UseLocalMachineStore | ChainEngineOptions.EnableCacheAutoUpdate | ChainEngineOptions.EnableShareStore, ChainEnginePool.DefaultTimeout, 0, null, false)
		{
		}

		public ChainEnginePool(int count, ChainEngineOptions options, TimeSpan timeout, int cacheLimit, X509Store store = null, bool exclusiveTrustMode = false)
		{
			this.configuration = new ChainEnginePool.ChainEngineConfig(options, (int)timeout.TotalMilliseconds, cacheLimit);
			if (store != null && exclusiveTrustMode)
			{
				this.configuration.HExclusiveRoot = store.StoreHandle;
			}
			this.engines = new Stack<SafeChainEngineHandle>(count);
		}

		public void Dispose()
		{
			this.Close();
		}

		public void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ChainEngine GetEngine()
		{
			lock (this)
			{
				if (this.disposed)
				{
					return new ChainEngine(null, SafeChainEngineHandle.Create(this.configuration));
				}
				if (this.engines.Count > 0)
				{
					return new ChainEngine(this, this.engines.Pop());
				}
			}
			return new ChainEngine(this, SafeChainEngineHandle.Create(this.configuration));
		}

		void IEnginePool.ReturnTo(SafeChainEngineHandle item)
		{
			if (item == null)
			{
				return;
			}
			if (this.disposed)
			{
				item.Close();
				return;
			}
			lock (this)
			{
				if (this.disposed)
				{
					item.Close();
				}
				else
				{
					this.engines.Push(item);
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					this.disposed = true;
				}
				Stack<SafeChainEngineHandle> stack = Interlocked.CompareExchange<Stack<SafeChainEngineHandle>>(ref this.engines, null, null);
				if (stack != null)
				{
					while (stack.Count > 0)
					{
						SafeChainEngineHandle safeChainEngineHandle = stack.Pop();
						safeChainEngineHandle.Close();
					}
				}
			}
		}

		private const int DefaultCacheLimit = 0;

		private const ChainEngineOptions DefaultOptions = ChainEngineOptions.CacheEndCert | ChainEngineOptions.UseLocalMachineStore | ChainEngineOptions.EnableCacheAutoUpdate | ChainEngineOptions.EnableShareStore;

		private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(10000.0);

		private ChainEnginePool.ChainEngineConfig configuration;

		private bool disposed;

		private Stack<SafeChainEngineHandle> engines;

		internal struct ChainEngineConfig
		{
			public ChainEngineConfig(ChainEngineOptions flags, int timeout, int maximum)
			{
				this.size = Marshal.SizeOf(typeof(ChainEnginePool.ChainEngineConfig));
				this.restrictedRootHandle = IntPtr.Zero;
				this.restrictedTrustHandle = IntPtr.Zero;
				this.restrictedOtherHandle = IntPtr.Zero;
				this.additionalStoresCount = 0;
				this.additionalStores = IntPtr.Zero;
				this.options = flags;
				this.urlRetrievalTimeout = timeout;
				this.maximumCachedCertificates = maximum;
				this.cycleDetectionModulus = 0;
				this.hExclusiveRoot = IntPtr.Zero;
				this.hExclusiveTrustedPeople = IntPtr.Zero;
			}

			public IntPtr HExclusiveRoot
			{
				get
				{
					return this.hExclusiveRoot;
				}
				set
				{
					this.hExclusiveRoot = value;
				}
			}

			public ChainEngineOptions Options
			{
				get
				{
					return this.options;
				}
			}

			public int UrlRetrievalTimeout
			{
				get
				{
					return this.urlRetrievalTimeout;
				}
			}

			public int MaximumCachedCertificates
			{
				get
				{
					return this.maximumCachedCertificates;
				}
			}

			private int size;

			private IntPtr restrictedRootHandle;

			private IntPtr restrictedTrustHandle;

			private IntPtr restrictedOtherHandle;

			private int additionalStoresCount;

			private IntPtr additionalStores;

			private ChainEngineOptions options;

			private int urlRetrievalTimeout;

			private int maximumCachedCertificates;

			private int cycleDetectionModulus;

			private IntPtr hExclusiveRoot;

			private IntPtr hExclusiveTrustedPeople;
		}
	}
}
