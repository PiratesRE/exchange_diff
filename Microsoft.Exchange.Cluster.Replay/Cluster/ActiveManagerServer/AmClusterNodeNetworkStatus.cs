using System;
using Microsoft.Exchange.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Serializable]
	public class AmClusterNodeNetworkStatus
	{
		public AmClusterNodeNetworkStatus()
		{
			this.IsHealthy = true;
			this.HasADAccess = true;
			this.ClusterErrorOverride = false;
		}

		public bool IsHealthy { get; set; }

		public bool HasADAccess
		{
			get
			{
				return this.m_hasADAccess;
			}
			set
			{
				this.m_hasADAccess = value;
				if (!value)
				{
					this.IsHealthy = false;
				}
			}
		}

		public bool ClusterErrorOverride { get; set; }

		public DateTime LastUpdate { get; set; }

		public override string ToString()
		{
			return string.Format("IsHealthy={0},HasADAccess={1},ClusterErrorOverride{2},LastUpdate={3}UTC", new object[]
			{
				this.IsHealthy,
				this.HasADAccess,
				this.ClusterErrorOverride,
				this.LastUpdate
			});
		}

		public bool IsEqual(AmClusterNodeNetworkStatus other)
		{
			return this.IsHealthy == other.IsHealthy && this.HasADAccess == other.HasADAccess && this.ClusterErrorOverride == other.ClusterErrorOverride;
		}

		internal static AmClusterNodeNetworkStatus Deserialize(string xmlText)
		{
			return (AmClusterNodeNetworkStatus)SerializationUtil.XmlToObject(xmlText, typeof(AmClusterNodeNetworkStatus));
		}

		internal string Serialize()
		{
			return SerializationUtil.ObjectToXml(this);
		}

		private bool m_hasADAccess;
	}
}
