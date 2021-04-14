using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.EseRepl
{
	[Serializable]
	public class DagNetConfig : IEquatable<DagNetConfig>
	{
		public int ReplicationPort
		{
			get
			{
				return this.replicationPort;
			}
			set
			{
				this.replicationPort = value;
			}
		}

		public NetworkOption NetworkCompression
		{
			get
			{
				return this.networkCompression;
			}
			set
			{
				this.networkCompression = value;
			}
		}

		public NetworkOption NetworkEncryption
		{
			get
			{
				return this.networkEncryption;
			}
			set
			{
				this.networkEncryption = value;
			}
		}

		public List<DagNetwork> Networks
		{
			get
			{
				return this.networks;
			}
		}

		public List<DagNode> Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		public bool Equals(DagNetConfig other)
		{
			return this.replicationPort == other.replicationPort && this.networkCompression == other.networkCompression && this.networkEncryption == other.networkEncryption && this.networks.SequenceEqual(other.networks) && this.nodes.SequenceEqual(other.nodes);
		}

		internal static DagNetConfig Deserialize(string xmlText)
		{
			return (DagNetConfig)Dependencies.Serializer.XmlToObject(xmlText, typeof(DagNetConfig));
		}

		internal string Serialize()
		{
			return Dependencies.Serializer.ObjectToXml(this);
		}

		internal DagNetConfig Copy()
		{
			string xmlText = this.Serialize();
			return DagNetConfig.Deserialize(xmlText);
		}

		public const int DefaultReplicationPort = 64327;

		public const NetworkOption DefaultNetworkOption = NetworkOption.InterSubnetOnly;

		private int replicationPort = 64327;

		private NetworkOption networkCompression = NetworkOption.InterSubnetOnly;

		private NetworkOption networkEncryption = NetworkOption.InterSubnetOnly;

		private List<DagNetwork> networks = new List<DagNetwork>(3);

		private List<DagNode> nodes = new List<DagNode>(12);
	}
}
