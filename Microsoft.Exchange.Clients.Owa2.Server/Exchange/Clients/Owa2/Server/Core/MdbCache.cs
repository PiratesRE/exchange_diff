using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class MdbCache
	{
		private MdbCache(TimeSpan updateInterval, MdbCache.IQuery query)
		{
			this.updateInterval = updateInterval;
			this.query = query;
		}

		public Action<IList<string>> ExecuteAfterAsyncUpdate
		{
			get
			{
				return this.executeAfterAysncUpdate;
			}
			set
			{
				this.executeAfterAysncUpdate = value;
			}
		}

		public static MdbCache GetInstance()
		{
			if (MdbCache.singleton == null)
			{
				lock (MdbCache.creationLock)
				{
					if (MdbCache.singleton == null)
					{
						MdbCache.singleton = new MdbCache(WacConfiguration.Instance.MdbCacheInterval, new MdbCacheQuery());
					}
				}
			}
			return MdbCache.singleton;
		}

		internal static MdbCache GetTestInstance(TimeSpan updateInterval, MdbCache.IQuery query)
		{
			return new MdbCache(updateInterval, query);
		}

		public void BeginAsyncUpdate()
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				this.OptionalCacheUpdate();
				if (this.executeAfterAysncUpdate != null && this.databasePaths != null)
				{
					string[] array = new string[this.databasePaths.Count];
					this.databasePaths.Values.CopyTo(array, 0);
					this.executeAfterAysncUpdate(array);
				}
			});
		}

		public string GetPath(Guid mdb)
		{
			this.EnsureCacheIsLoaded();
			string result;
			if (this.databasePaths.TryGetValue(mdb, out result))
			{
				return result;
			}
			this.MandatoryCacheUpdate();
			if (this.databasePaths.TryGetValue(mdb, out result))
			{
				return result;
			}
			throw new OwaInvalidRequestException(string.Format("MDB {0} not found on this server.", mdb));
		}

		private void EnsureCacheIsLoaded()
		{
			if (this.databasePaths == null)
			{
				this.MandatoryCacheUpdate();
				if (this.databasePaths == null)
				{
					throw new InvalidOperationException("Unable to acquire MDB information.");
				}
			}
		}

		private void OptionalCacheUpdate()
		{
			try
			{
				if (Monitor.TryEnter(this.updateLock))
				{
					if (this.UpdateIntervalElapsed())
					{
						this.UpdateWithinLock();
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.updateLock))
				{
					Monitor.Exit(this.updateLock);
				}
			}
		}

		private void MandatoryCacheUpdate()
		{
			try
			{
				if (!Monitor.TryEnter(this.updateLock))
				{
					lock (this.updateLock)
					{
					}
				}
				else
				{
					this.UpdateWithinLock();
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.updateLock))
				{
					Monitor.Exit(this.updateLock);
				}
			}
		}

		private void UpdateWithinLock()
		{
			if (!Monitor.IsEntered(this.updateLock))
			{
				throw new InvalidOperationException("MdbCache.UpdateWithinLock must only be called after acquiring the lock.");
			}
			Dictionary<Guid, string> dictionary;
			if (this.query.TryGetDatabasePaths(out dictionary))
			{
				this.databasePaths = dictionary;
			}
			this.lastUpdate = ExDateTime.Now;
		}

		private bool UpdateIntervalElapsed()
		{
			return this.lastUpdate + this.updateInterval < ExDateTime.Now;
		}

		private static readonly object creationLock = new object();

		private static MdbCache singleton;

		private readonly object updateLock = new object();

		private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(30.0);

		private ExDateTime lastUpdate = ExDateTime.MinValue;

		private Dictionary<Guid, string> databasePaths;

		private readonly MdbCache.IQuery query;

		private Action<IList<string>> executeAfterAysncUpdate;

		public interface IQuery
		{
			bool TryGetDatabasePaths(out Dictionary<Guid, string> newPaths);
		}
	}
}
