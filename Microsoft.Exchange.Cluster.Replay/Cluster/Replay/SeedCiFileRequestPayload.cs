using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay
{
	[DataContract]
	internal sealed class SeedCiFileRequestPayload
	{
		internal SeedCiFileRequestPayload(string endpoint, string reason)
		{
			this.Endpoint = endpoint;
			this.Reason = reason;
		}

		[DataMember]
		internal string Endpoint { get; private set; }

		[DataMember]
		internal string Reason { get; private set; }
	}
}
