using System;
using System.Net;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.EseRepl
{
	public class DagNetRoute : IEquatable<DagNetRoute>
	{
		public IPAddress SourceIPAddr { get; set; }

		public IPAddress TargetIPAddr { get; set; }

		public int TargetPort { get; set; }

		public string NetworkName { get; set; }

		public bool IsCrossSubnet { get; set; }

		public bool Equals(DagNetRoute other)
		{
			return this.SourceIPAddr.Equals(other.SourceIPAddr) && this.TargetIPAddr.Equals(other.TargetIPAddr) && this.TargetPort == other.TargetPort && this.IsCrossSubnet == other.IsCrossSubnet && StringUtil.IsEqualIgnoreCase(this.NetworkName, other.NetworkName);
		}
	}
}
