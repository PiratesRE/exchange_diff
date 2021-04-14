using System;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	[Serializable]
	internal class ReputationQueryInput
	{
		public ReputationQueryInput(byte entityType, string entityKey, int dataPointType, int ttl, int udpTimeout, int flags = 0)
		{
			this.EntityType = entityType;
			this.EntityKey = entityKey;
			this.DataPointType = dataPointType;
			this.Ttl = ttl;
			this.Flags = flags;
			this.UdpTimeout = udpTimeout;
		}

		public byte EntityType { get; set; }

		public string EntityKey { get; set; }

		public int DataPointType { get; set; }

		public int Ttl { get; set; }

		public int Flags { get; set; }

		public int UdpTimeout { get; set; }
	}
}
