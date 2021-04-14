using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AdObjectCacheEntry<TADWrapperObject> where TADWrapperObject : class, IADObjectCommon
	{
		public AdObjectCacheEntry(TADWrapperObject adObjectType, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan timeToLiveMaximum)
		{
			this.m_timeRetrieved = DateTime.UtcNow;
			if (adObjectType != null)
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToLive);
			}
			else
			{
				this.m_timeToExpire = this.m_timeRetrieved.Add(timeToNegativeLive);
			}
			this.m_maximumTimeToExpire = this.m_timeRetrieved.Add(timeToLiveMaximum);
			this.m_adObjectData = adObjectType;
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

		public TADWrapperObject AdObjectData
		{
			get
			{
				return this.m_adObjectData;
			}
		}

		public override string ToString()
		{
			string text;
			if (this.m_adObjectData != null)
			{
				TADWrapperObject adObjectData = this.m_adObjectData;
				text = adObjectData.Name;
			}
			else
			{
				text = string.Empty;
			}
			string arg = text;
			return string.Format("[{0};exp={1:s},maxtime={2:s}]", arg, this.m_timeToExpire, this.m_maximumTimeToExpire);
		}

		private readonly DateTime m_timeRetrieved;

		private readonly DateTime m_timeToExpire;

		private readonly DateTime m_maximumTimeToExpire;

		private readonly TADWrapperObject m_adObjectData;
	}
}
