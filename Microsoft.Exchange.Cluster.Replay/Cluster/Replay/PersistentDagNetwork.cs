using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	public class PersistentDagNetwork
	{
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		public bool ReplicationEnabled
		{
			get
			{
				return this.m_replicationEnabled;
			}
			set
			{
				this.m_replicationEnabled = value;
			}
		}

		public bool IgnoreNetwork
		{
			get
			{
				return this.m_ignoreNetwork;
			}
			set
			{
				this.m_ignoreNetwork = value;
			}
		}

		public List<string> Subnets
		{
			get
			{
				return this.m_subnets;
			}
		}

		private string m_name;

		private string m_description;

		private bool m_replicationEnabled;

		private bool m_ignoreNetwork;

		private List<string> m_subnets = new List<string>();
	}
}
