using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[DebuggerDisplay("{PartitionFqdn}-{Version}")]
	[DataContract]
	internal class TopologyVersion : IExtensibleDataObject
	{
		public TopologyVersion(string partitionFqdn, int version)
		{
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentException("partitionFqdn");
			}
			this.PartitionFqdn = partitionFqdn;
			this.Version = version;
		}

		[DataMember(IsRequired = true)]
		public string PartitionFqdn { get; set; }

		[DataMember(IsRequired = true)]
		public int Version { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		public override string ToString()
		{
			return this.PartitionFqdn + ":" + this.Version;
		}
	}
}
