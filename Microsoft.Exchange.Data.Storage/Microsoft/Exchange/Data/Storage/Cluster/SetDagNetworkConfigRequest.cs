using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SetDagNetworkConfigRequest
	{
		public ushort ReplicationPort
		{
			get
			{
				return this.m_replicationPort;
			}
			set
			{
				this.m_replicationPort = value;
				this.m_setPort = true;
			}
		}

		public bool SetPort
		{
			get
			{
				return this.m_setPort;
			}
		}

		public bool WhatIf
		{
			get
			{
				return this.m_whatIf;
			}
			set
			{
				this.m_whatIf = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkCompression
		{
			get
			{
				return this.m_networkCompression;
			}
			set
			{
				this.m_networkCompression = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
		{
			get
			{
				return this.m_networkEncryption;
			}
			set
			{
				this.m_networkEncryption = value;
			}
		}

		public bool DiscoverNetworks
		{
			get
			{
				return this.m_discoverNetworks;
			}
			set
			{
				this.m_discoverNetworks = value;
			}
		}

		public bool ManualDagNetworkConfiguration
		{
			get
			{
				return this.m_manualDagNetworkConfiguration;
			}
			set
			{
				this.m_manualDagNetworkConfiguration = value;
			}
		}

		private ushort m_replicationPort = 64327;

		private bool m_setPort;

		private bool m_whatIf;

		private DatabaseAvailabilityGroup.NetworkOption m_networkCompression = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private DatabaseAvailabilityGroup.NetworkOption m_networkEncryption = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		private bool m_discoverNetworks;

		private bool m_manualDagNetworkConfiguration;
	}
}
