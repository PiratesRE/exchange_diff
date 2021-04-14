using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Microsoft.Exchange.ProcessManager
{
	internal class IPConnectionTable
	{
		public IPConnectionTable(int connectionRate)
		{
			this.ipTableSweeper = new Timer(new TimerCallback(this.CleanupIPTable), null, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(2.0));
			this.connectionRateLimit = connectionRate;
		}

		public int ConnectionRateLimit
		{
			get
			{
				return this.connectionRateLimit;
			}
			set
			{
				this.connectionRateLimit = value;
			}
		}

		public bool CanAcceptConnection(IPAddress ipAddress)
		{
			object syncRoot = ((ICollection)this.ipTable).SyncRoot;
			bool result;
			try
			{
				if (!Monitor.TryEnter(syncRoot))
				{
					result = true;
				}
				else
				{
					TokenRateLimiter tokenRateLimiter;
					if (!this.ipTable.TryGetValue(ipAddress, out tokenRateLimiter))
					{
						tokenRateLimiter = new TokenRateLimiter();
						this.ipTable.Add(ipAddress, tokenRateLimiter);
					}
					result = tokenRateLimiter.TryFetchToken(this.connectionRateLimit);
				}
			}
			finally
			{
				if (Monitor.IsEntered(syncRoot))
				{
					Monitor.Exit(syncRoot);
				}
			}
			return result;
		}

		private void CleanupIPTable(object state)
		{
			DateTime utcNow = DateTime.UtcNow;
			lock (((ICollection)this.ipTable).SyncRoot)
			{
				List<IPAddress> list = new List<IPAddress>();
				foreach (KeyValuePair<IPAddress, TokenRateLimiter> keyValuePair in this.ipTable)
				{
					if (keyValuePair.Value.IsIdle(utcNow))
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (IPAddress key in list)
				{
					this.ipTable.Remove(key);
				}
			}
		}

		public void Close()
		{
			if (this.ipTableSweeper != null)
			{
				this.ipTableSweeper.Dispose();
				this.ipTableSweeper = null;
			}
		}

		private Dictionary<IPAddress, TokenRateLimiter> ipTable = new Dictionary<IPAddress, TokenRateLimiter>();

		private Timer ipTableSweeper;

		private int connectionRateLimit;
	}
}
