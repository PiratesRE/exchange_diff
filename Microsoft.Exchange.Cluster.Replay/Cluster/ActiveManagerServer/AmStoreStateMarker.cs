using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmStoreStateMarker
	{
		internal AmStoreStateMarker()
		{
			this.m_storeStateMap = new Dictionary<AmServerName, AmSystemEventCode>(4);
		}

		internal void SetStoreState(AmServerName serverName, AmSystemEventCode eventCode)
		{
			lock (this.m_locker)
			{
				this.m_storeStateMap[serverName] = eventCode;
			}
		}

		internal bool CheckIfStoreStartMarkedAndClear(AmServerName serverName)
		{
			bool result;
			lock (this.m_locker)
			{
				result = this.CheckIfEventMarked(serverName, AmSystemEventCode.StoreServiceStarted, true);
			}
			return result;
		}

		internal bool CheckIfStoreStopMarkedAndClear(AmServerName serverName)
		{
			bool result;
			lock (this.m_locker)
			{
				result = this.CheckIfEventMarked(serverName, AmSystemEventCode.StoreServiceStopped, true);
			}
			return result;
		}

		internal bool IsStoreGracefullyStoppedOn(AmServerName serverName)
		{
			bool result;
			lock (this.m_locker)
			{
				result = this.CheckIfEventMarked(serverName, AmSystemEventCode.StoreServiceStopped, false);
			}
			return result;
		}

		internal bool Clear(AmServerName serverName)
		{
			bool result;
			lock (this.m_locker)
			{
				result = this.m_storeStateMap.Remove(serverName);
			}
			return result;
		}

		internal void ClearAllStoreStartRequests()
		{
			lock (this.m_locker)
			{
				List<AmServerName> list = new List<AmServerName>();
				foreach (AmServerName amServerName in this.m_storeStateMap.Keys)
				{
					if (this.CheckIfEventMarked(amServerName, AmSystemEventCode.StoreServiceStarted, false))
					{
						list.Add(amServerName);
					}
				}
				foreach (AmServerName serverName in list)
				{
					this.CheckIfStoreStartMarkedAndClear(serverName);
				}
			}
		}

		internal void Clear()
		{
			lock (this.m_locker)
			{
				this.m_storeStateMap.Clear();
			}
		}

		private bool CheckIfEventMarked(AmServerName serverName, AmSystemEventCode compareEventCode, bool isRemove)
		{
			AmSystemEventCode amSystemEventCode;
			if (this.m_storeStateMap.TryGetValue(serverName, out amSystemEventCode) && amSystemEventCode == compareEventCode)
			{
				if (isRemove)
				{
					this.m_storeStateMap.Remove(serverName);
				}
				return true;
			}
			return false;
		}

		private object m_locker = new object();

		private Dictionary<AmServerName, AmSystemEventCode> m_storeStateMap;
	}
}
