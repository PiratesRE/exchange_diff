using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SetDagNetworkRequest
	{
		public string Name { get; set; }

		public string NewName { get; set; }

		public string Description { get; set; }

		public string LatestName
		{
			get
			{
				return this.NewName ?? this.Name;
			}
		}

		public SortedList<DatabaseAvailabilityGroupSubnetId, object> Subnets
		{
			get
			{
				return this.m_subnets;
			}
		}

		public bool SubnetListIsSet
		{
			get
			{
				return this.m_subnetsWereSet || this.m_subnets.Count > 0;
			}
			set
			{
				this.m_subnetsWereSet = value;
			}
		}

		public bool IsReplicationChanged { get; set; }

		public bool ReplicationEnabled
		{
			get
			{
				return this.m_replEnabled;
			}
			set
			{
				this.IsReplicationChanged = true;
				this.m_replEnabled = value;
			}
		}

		public bool IsIgnoreChanged { get; set; }

		public bool IgnoreNetwork
		{
			get
			{
				return this.m_ignoreNetwork;
			}
			set
			{
				this.IsIgnoreChanged = true;
				this.m_ignoreNetwork = value;
			}
		}

		private SortedList<DatabaseAvailabilityGroupSubnetId, object> m_subnets = new SortedList<DatabaseAvailabilityGroupSubnetId, object>(DagSubnetIdComparer.Comparer);

		private bool m_subnetsWereSet;

		private bool m_replEnabled = true;

		private bool m_ignoreNetwork;
	}
}
