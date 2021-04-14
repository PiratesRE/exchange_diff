using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.EseRepl
{
	[Serializable]
	public class DagNetwork : IEquatable<DagNetwork>
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public bool ReplicationEnabled { get; set; }

		public bool IsDnsMapped { get; set; }

		public List<string> Subnets
		{
			get
			{
				return this.subnets;
			}
		}

		public bool Equals(DagNetwork other)
		{
			return this.subnets.SequenceEqual(other.subnets) && StringUtil.IsEqualIgnoreCase(this.Name, other.Name) && StringUtil.IsEqualIgnoreCase(this.Description, other.Description) && this.ReplicationEnabled == other.ReplicationEnabled && this.IsDnsMapped == other.IsDnsMapped;
		}

		private List<string> subnets = new List<string>();
	}
}
