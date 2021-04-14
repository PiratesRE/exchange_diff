using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WcfProxyObjectsPool : IDisposable
	{
		internal WcfProxyObjectsPool()
		{
			WcfProxyObjectsPool.s_backgroundThreadSyncPoint = 0;
			this.m_memoryCleanupBackgroundTimer = new System.Timers.Timer();
			this.m_memoryCleanupBackgroundTimer.Elapsed += this.MemoryCleanupBackgroundTimerRun;
			this.m_memoryCleanupBackgroundTimer.Interval = AmRpcClientHelper.WcfCleanupTime.TotalMilliseconds;
			this.m_memoryCleanupBackgroundTimer.AutoReset = true;
			this.m_memoryCleanupBackgroundTimer.Enabled = true;
		}

		private void MemoryCleanupBackgroundTimerRun(object sender, ElapsedEventArgs e)
		{
			int num = 0;
			if (Interlocked.CompareExchange(ref WcfProxyObjectsPool.s_backgroundThreadSyncPoint, 1, 0) == 0)
			{
				num = this.Compact();
				WcfProxyObjectsPool.s_backgroundThreadSyncPoint = 0;
			}
			if (AmRpcClientHelper.LogDiagnosticEvents)
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ActiveManagerWCFCleanup, null, new object[]
				{
					num
				});
			}
		}

		private ServerLocatorServiceClient TakeClientFromThePool(string serverName, out DateTime lastAccessTimeUtc)
		{
			ServerLocatorServiceClient result;
			lock (this.lockObject)
			{
				WcfProxyObjectsPerServerStack wcfProxyObjectsPerServerStack;
				if (this.m_dictionary.TryGetValue(serverName, out wcfProxyObjectsPerServerStack))
				{
					lastAccessTimeUtc = wcfProxyObjectsPerServerStack.LastAccessTimeUtc;
					if (wcfProxyObjectsPerServerStack.Count > 0)
					{
						result = wcfProxyObjectsPerServerStack.Pop();
					}
					else
					{
						result = null;
					}
				}
				else
				{
					lastAccessTimeUtc = DateTime.MinValue;
					result = null;
				}
			}
			return result;
		}

		internal int Count(string serverName)
		{
			int result;
			lock (this.lockObject)
			{
				WcfProxyObjectsPerServerStack wcfProxyObjectsPerServerStack;
				if (this.m_dictionary.TryGetValue(serverName, out wcfProxyObjectsPerServerStack))
				{
					result = wcfProxyObjectsPerServerStack.Count;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		internal ServerLocatorServiceClient TakeClientFromThePool(string serverName)
		{
			DateTime dateTime;
			return this.TakeClientFromThePool(serverName, out dateTime);
		}

		internal bool ReturnClientIntoThePool(string serverName, ServerLocatorServiceClient client)
		{
			bool result = false;
			lock (this.lockObject)
			{
				WcfProxyObjectsPerServerStack wcfProxyObjectsPerServerStack;
				if (this.m_dictionary.TryGetValue(serverName, out wcfProxyObjectsPerServerStack))
				{
					if (wcfProxyObjectsPerServerStack.Count < WcfProxyObjectsPool.s_maximumSizeOfThePool)
					{
						wcfProxyObjectsPerServerStack.Push(client);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					wcfProxyObjectsPerServerStack = new WcfProxyObjectsPerServerStack();
					wcfProxyObjectsPerServerStack.Push(client);
					this.m_dictionary[serverName] = wcfProxyObjectsPerServerStack;
					result = true;
				}
			}
			return result;
		}

		internal int Compact()
		{
			int num = 0;
			List<string> list = new List<string>(WcfProxyObjectsPool.s_numberOfServersPerDag);
			DateTime t = DateTime.UtcNow - AmRpcClientHelper.WcfCleanupTime;
			lock (this.lockObject)
			{
				foreach (KeyValuePair<string, WcfProxyObjectsPerServerStack> keyValuePair in this.m_dictionary)
				{
					if (keyValuePair.Value.LastAccessTimeUtc <= t)
					{
						list.Add(keyValuePair.Key);
					}
				}
			}
			foreach (string serverName in list)
			{
				DateTime minValue = DateTime.MinValue;
				ServerLocatorServiceClient serverLocatorServiceClient;
				while ((serverLocatorServiceClient = this.TakeClientFromThePool(serverName, out minValue)) != null)
				{
					if (minValue > t)
					{
						this.ReturnClientIntoThePool(serverName, serverLocatorServiceClient);
						break;
					}
					serverLocatorServiceClient.Dispose(true);
					num++;
				}
			}
			return num;
		}

		internal void Clear()
		{
			while (Interlocked.CompareExchange(ref WcfProxyObjectsPool.s_backgroundThreadSyncPoint, -1, 0) != 0)
			{
				Thread.Yield();
			}
			lock (this.lockObject)
			{
				foreach (WcfProxyObjectsPerServerStack wcfProxyObjectsPerServerStack in this.m_dictionary.Values)
				{
					while (wcfProxyObjectsPerServerStack.Count > 0)
					{
						ServerLocatorServiceClient serverLocatorServiceClient = wcfProxyObjectsPerServerStack.Pop();
						serverLocatorServiceClient.Dispose(true);
					}
				}
				this.m_dictionary.Clear();
			}
			WcfProxyObjectsPool.s_backgroundThreadSyncPoint = 0;
		}

		public void Dispose()
		{
			this.m_memoryCleanupBackgroundTimer.Stop();
			while (Interlocked.CompareExchange(ref WcfProxyObjectsPool.s_backgroundThreadSyncPoint, -1, 0) != 0)
			{
				Thread.Yield();
			}
			this.m_memoryCleanupBackgroundTimer.Dispose();
			this.m_memoryCleanupBackgroundTimer = null;
		}

		private readonly Dictionary<string, WcfProxyObjectsPerServerStack> m_dictionary = new Dictionary<string, WcfProxyObjectsPerServerStack>(WcfProxyObjectsPool.s_numberOfServersPerDag);

		private static int s_numberOfServersPerDag = 16;

		private static int s_maximumSizeOfThePool = 10;

		private object lockObject = new object();

		private System.Timers.Timer m_memoryCleanupBackgroundTimer;

		private static int s_backgroundThreadSyncPoint = 0;
	}
}
