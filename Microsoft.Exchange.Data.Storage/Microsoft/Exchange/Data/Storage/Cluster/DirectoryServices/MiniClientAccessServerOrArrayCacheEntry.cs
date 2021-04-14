using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MiniClientAccessServerOrArrayCacheEntry
	{
		public MiniClientAccessServerOrArrayCacheEntry(IADMiniClientAccessServerOrArray miniClientAccessServerOrArray, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan timeToLiveMaximum)
		{
			this.m_timeRetrieved = DateTime.UtcNow;
			if (miniClientAccessServerOrArray != null)
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToLive);
			}
			else
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToNegativeLive);
			}
			this.m_maximumTimeToExpire = this.m_timeRetrieved.Add(timeToLiveMaximum);
			this.m_miniClientAccessServerOrArrayData = miniClientAccessServerOrArray;
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

		public IADMiniClientAccessServerOrArray MiniClientAccessServerOrArrayData
		{
			get
			{
				return this.m_miniClientAccessServerOrArrayData;
			}
		}

		public override string ToString()
		{
			string arg = (this.m_miniClientAccessServerOrArrayData == null) ? string.Empty : this.m_miniClientAccessServerOrArrayData.Name;
			return string.Format("[{0};exp={1:s},maxtime={2:s}]", arg, this.m_timeToExpire, this.m_maximumTimeToExpire);
		}

		private readonly DateTime m_timeRetrieved;

		private readonly DateTime m_timeToExpire;

		private readonly DateTime m_maximumTimeToExpire;

		private readonly IADMiniClientAccessServerOrArray m_miniClientAccessServerOrArrayData;
	}
}
