using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Rpc
{
	internal class RpcServerCache
	{
		public RpcServerCache()
		{
			this.cache = new Dictionary<Guid, RpcServerBase>();
			this.cacheLock = new ReaderWriterLockSlim();
		}

		public void Add(Guid guid, RpcServerBase server)
		{
			if (guid == Guid.Empty)
			{
				throw new ArgumentNullException("guid");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			try
			{
				this.cacheLock.EnterWriteLock();
				this.cache.Add(guid, server);
			}
			finally
			{
				try
				{
					this.cacheLock.ExitWriteLock();
				}
				catch (SynchronizationLockException ex)
				{
				}
			}
		}

		public void Remove(Guid guid)
		{
			if (guid == Guid.Empty)
			{
				throw new ArgumentNullException("guid");
			}
			try
			{
				this.cacheLock.EnterWriteLock();
				this.cache.Remove(guid);
			}
			finally
			{
				try
				{
					this.cacheLock.ExitWriteLock();
				}
				catch (SynchronizationLockException ex)
				{
				}
			}
		}

		public RpcServerBase Lookup(Guid guid)
		{
			RpcServerBase result = null;
			try
			{
				this.cacheLock.EnterReadLock();
				if (!this.cache.TryGetValue(guid, out result))
				{
					result = null;
				}
			}
			finally
			{
				try
				{
					this.cacheLock.ExitReadLock();
				}
				catch (SynchronizationLockException ex)
				{
				}
			}
			return result;
		}

		private readonly Dictionary<Guid, RpcServerBase> cache;

		private readonly ReaderWriterLockSlim cacheLock;
	}
}
