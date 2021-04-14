using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbNodeAttemptTable
	{
		internal bool IsOkayForAction(IADDatabase db, AmServerName nodeName, AmDbActionCode actionCode)
		{
			return this.IsOkayForAction(db, nodeName, actionCode, RegistryParameters.FailureItemStromCoolingDurationInSec);
		}

		internal bool IsOkayForAction(IADDatabase db, AmServerName nodeName, AmDbActionCode actionCode, int coolingDuration)
		{
			bool result = true;
			if (actionCode == null)
			{
				return true;
			}
			if (!actionCode.IsAutomaticFailureItem)
			{
				return true;
			}
			if (db.ReplicationType == ReplicationType.None)
			{
				return true;
			}
			lock (this.m_locker)
			{
				Dictionary<AmServerName, AmDbAttemptInfo> dictionary = null;
				if (this.m_dbMap.TryGetValue(db.Guid, out dictionary))
				{
					AmDbAttemptInfo amDbAttemptInfo = null;
					if (dictionary.TryGetValue(nodeName, out amDbAttemptInfo))
					{
						DateTime lastAttemptTime = amDbAttemptInfo.LastAttemptTime;
						if ((DateTime)ExDateTime.Now < lastAttemptTime.AddSeconds((double)coolingDuration))
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		internal void MarkFailedTime(Guid dbGuid, AmServerName nodeName, AmDbActionCode actionCode)
		{
			lock (this.m_locker)
			{
				Dictionary<AmServerName, AmDbAttemptInfo> dictionary = null;
				if (!this.m_dbMap.TryGetValue(dbGuid, out dictionary))
				{
					dictionary = new Dictionary<AmServerName, AmDbAttemptInfo>();
					this.m_dbMap[dbGuid] = dictionary;
				}
				AmDbAttemptInfo value = new AmDbAttemptInfo(dbGuid, actionCode, (DateTime)ExDateTime.Now);
				dictionary[nodeName] = value;
			}
		}

		internal void ClearFailedTime(Guid dbGuid, AmServerName nodeName)
		{
			lock (this.m_locker)
			{
				if (nodeName != null)
				{
					Dictionary<AmServerName, AmDbAttemptInfo> dictionary = null;
					if (this.m_dbMap.TryGetValue(dbGuid, out dictionary))
					{
						dictionary.Remove(nodeName);
					}
				}
				else
				{
					this.m_dbMap.Remove(dbGuid);
				}
			}
		}

		internal void ClearFailedTime(Guid dbGuid)
		{
			this.ClearFailedTime(dbGuid, null);
		}

		internal void ClearFailedTime(AmServerName nodeName)
		{
			lock (this.m_locker)
			{
				foreach (Guid dbGuid in this.GetKeysCopy())
				{
					this.ClearFailedTime(dbGuid, nodeName);
				}
			}
		}

		internal void ClearFailedTime()
		{
			lock (this.m_locker)
			{
				foreach (Guid dbGuid in this.GetKeysCopy())
				{
					this.ClearFailedTime(dbGuid);
				}
			}
		}

		private Guid[] GetKeysCopy()
		{
			Guid[] array = new Guid[this.m_dbMap.Keys.Count];
			this.m_dbMap.Keys.CopyTo(array, 0);
			return array;
		}

		private object m_locker = new object();

		private Dictionary<Guid, Dictionary<AmServerName, AmDbAttemptInfo>> m_dbMap = new Dictionary<Guid, Dictionary<AmServerName, AmDbAttemptInfo>>();
	}
}
