using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.EseRepl
{
	[Serializable]
	public class DagNode : IEquatable<DagNode>
	{
		public string Name { get; set; }

		public List<DagNode.Nic> Nics
		{
			get
			{
				return this.nics;
			}
		}

		public int ReplicationPort { get; set; }

		public static DagNode FindNode(string nodeName, IEnumerable<DagNode> list)
		{
			return list.FirstOrDefault((DagNode node) => StringUtil.IsEqualIgnoreCase(nodeName, node.Name));
		}

		public bool Equals(DagNode other)
		{
			return this.nics.SequenceEqual(other.nics) && StringUtil.IsEqualIgnoreCase(this.Name, other.Name);
		}

		private List<DagNode.Nic> nics = new List<DagNode.Nic>();

		public class Nic : IEquatable<DagNode.Nic>
		{
			public string IpAddress { get; set; }

			public string NetworkName { get; set; }

			public bool Equals(DagNode.Nic other)
			{
				return StringUtil.IsEqualIgnoreCase(this.IpAddress, other.IpAddress) && StringUtil.IsEqualIgnoreCase(this.NetworkName, other.NetworkName);
			}
		}
	}
}
