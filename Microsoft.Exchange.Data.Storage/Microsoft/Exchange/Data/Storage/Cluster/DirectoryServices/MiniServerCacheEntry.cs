using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MiniServerCacheEntry
	{
		public MiniServerCacheEntry(IADServer miniServer, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan timeToLiveMaximum)
		{
			this.m_timeRetrieved = DateTime.UtcNow;
			if (miniServer != null)
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToLive);
			}
			else
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToNegativeLive);
			}
			this.m_maximumTimeToExpire = this.m_timeRetrieved.Add(timeToLiveMaximum);
			this.m_miniServerData = miniServer;
		}

		public DateTime TimeRetrieved
		{
			get
			{
				return this.m_timeRetrieved;
			}
		}

		public DateTime TimeToExpire
		{
			get
			{
				return this.m_timeToExpire;
			}
		}

		public DateTime MaximumTimeToExpire
		{
			get
			{
				return this.m_maximumTimeToExpire;
			}
		}

		public IADServer MiniServerData
		{
			get
			{
				return this.m_miniServerData;
			}
		}

		public override string ToString()
		{
			string arg = (this.m_miniServerData == null) ? string.Empty : this.m_miniServerData.Name;
			return string.Format("[{0};exp={1:s},maxtime={2:s}]", arg, this.m_timeToExpire, this.m_maximumTimeToExpire);
		}

		private readonly DateTime m_timeRetrieved;

		private readonly DateTime m_timeToExpire;

		private readonly DateTime m_maximumTimeToExpire;

		private readonly IADServer m_miniServerData;
	}
}
